using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using bluez.DBus;
using Commons.Music.Midi;
using Tmds.DBus;

namespace dodgyrabbit.MidiBle
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Console.WriteLine("MIDI to BLE Bridge");

            var access = MidiAccessManager.Default;

            string id = "";
            // foreach (IMidiPortDetails port in access.Inputs)
            // {
            //     Console.WriteLine($"{port.Name}");
            //     if (port.Name.Contains("mio", StringComparison.InvariantCultureIgnoreCase))
            //     {
            //         id = port.Id;
            //         Console.WriteLine(id);
            //     }
            // }

            //var input = access.OpenInputAsync (access.Inputs.Last().Id).Result;
            //var midiId = access.Inputs.First(inp => inp.Name.Contains("mio", StringComparison.OrdinalIgnoreCase)).Id;
            //var input = access.OpenInputAsync (id).Result;

            var input = await access.OpenInputAsync("20_0");

            input.MessageReceived += (obj, e) => {
			//	Console.WriteLine ($"{e.Timestamp} {e.Start} {e.Length} {e.Data [0].ToString ("X")}");
			};

            Task.Delay(4000);

            byte lastStatusByte = 0;
            //Console.ReadLine();

            Task.Run(async () =>
            {
                Console.WriteLine("Connecting to System D-Bus...");
                using (var connection = new Connection(Address.System))
                {
                    await connection.ConnectAsync();
                    Console.WriteLine("Connected"); 

                    var hci0Path = new ObjectPath("/org/bluez/hci0");
                    var serviceName = "org.bluez";
                    var hci0Adapter = connection.CreateProxy<IAdapter1>(serviceName, hci0Path);

                    // Based on BlueZ sample code. Verify what it does. Does it really turn my BT adapeter on if it was off?
                    Console.WriteLine("Setting power to true on device 1");
                    await hci0Adapter.SetPoweredAsync(true);

                    var advertisement = new LEAdvertisement(new LEAdvertisementProperties {Type = "peripheral", LocalName="MIDI-BRIDGE"} );
                    advertisement.LEAdvertisementProperties.ServiceUUIDs = new string[] {"03B80E5A-EDE8-4B33-A751-6CE34EC4C700"};

                    // Generic computer icon
                    advertisement.LEAdvertisementProperties.Appearance = 0x0080;

                    // We need to register the LEAdvertisement object. This basically publishes the object
                    // so that when we get the DBus callback to read all the properties, we're good to go. 
                    await connection.RegisterObjectAsync(advertisement);

                    var advertisingManager = connection.CreateProxy<ILEAdvertisingManager1>(serviceName, hci0Path);
                    await advertisingManager.RegisterAdvertisementAsync(advertisement, new Dictionary<string, object>());
                    
                    Application application = new Application(@"/org/bluez/example");
                    GattService1 service = new GattService1(application.ObjectPath, 0, "03B80E5A-EDE8-4B33-A751-6CE34EC4C700", true);

                    GattCharacteristic1 gattCharacteristic1 = new GattCharacteristic1(service.ObjectPath, 0, "7772E5DB-3868-4112-A1A9-F2669D106BF3", new string[] {"notify", "read", "write-without-response"});
                    input.MessageReceived += (obj, e) =>
                    {
                        if (e.Length == 1 && e.Data[0] == 0xFE)
                        {
                            return;
                        }
						bool hasStatusByte = false;

 						List<byte> bytes = new List<byte>();
                        for (int i=0; i < e.Length; i++)
                        {
                            bool isStatusByte = ((e.Data[i] & (byte)0b1000_0000) > 0);

                            if (!hasStatusByte && !isStatusByte)
                            {
                                // Continuation from previous message. Repeat status byte.
                                if (lastStatusByte != 128)
                                {
                                    bytes.Add(lastStatusByte);
                                    bytes.Add(e.Data[i]);
                                }
                                hasStatusByte = true;
                                continue;
                            } 
                            if (!hasStatusByte && isStatusByte)
                            {
                                // New message, starting with status
                                
                                if (e.Data[i] != 128)
                                {
                                    // Skip the note-off messages
                                    bytes.Add(e.Data[i]);
                                }

                                hasStatusByte = true;
                                lastStatusByte = e.Data[i];
                                continue;
                            }

                            if (isStatusByte)
                            {
                                // Multiple status bytes in a row
                                if (e.Data[i] != 128)
                                {
                                    bytes.Add(e.Data[i]);
                                }
                                lastStatusByte = e.Data[i];
                                continue;
                            }

                            // Must be a data byte then, add it
                            if (lastStatusByte != 128)
                            {
                                bytes.Add(e.Data[i]);
                            }
                    }   
                        if (bytes.Count > 0)
                        {
                            gattCharacteristic1.PlayNote(bytes.ToArray());
                        }
                    };
                    service.AddCharacteristic(gattCharacteristic1);
                    application.AddService(service);

                    await connection.RegisterObjectAsync(application);

                    var gattManager = connection.CreateProxy<IGattManager1>(serviceName, new ObjectPath(@"/org/bluez/hci0"));

                    await connection.RegisterObjectAsync(gattCharacteristic1);
                    gattCharacteristic1.StartMidiHeartbeat();

                    

                    await gattManager.RegisterApplicationAsync(application, new Dictionary<string, object>());

                    Console.WriteLine("Press <ctrl>+c to exit...");
                    await Task.Delay(Int32.MaxValue);
                }
            }).Wait();

            await input.CloseAsync();
        }
    }
}
