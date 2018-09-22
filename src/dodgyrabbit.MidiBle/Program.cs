using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bluez.DBus;
using Commons.Music.Midi;
using Tmds.DBus;

namespace dodgyrabbit.MidiBle
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("MIDI to BLE Bridge");

            IMidiAccess access = MidiAccessManager.Default;
            var input = access.OpenInputAsync (access.Inputs.Last().Id).Result;

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
                        // Note off message
                        if (e.Data[0] != 128)
                        {
                            gattCharacteristic1.PlayNote(e.Data);
                        }
                        //Console.WriteLine ($"{e.Timestamp} {e.Start} {e.Length} {e.Data [0].ToString ("X")}");
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

            input.CloseAsync();
        }
    }
}
