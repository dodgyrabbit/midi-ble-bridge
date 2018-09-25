namespace Dodgyrabbit.MidiBle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Commons.Music.Midi;
    using Tmds.DBus;

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("MIDI to BLE Bridge");

            var access = MidiAccessManager.Default;
            var midiPort = access.Inputs.FirstOrDefault(inp => inp.Name.Contains("mio", StringComparison.OrdinalIgnoreCase));
            if (midiPort == null)
            {
                midiPort = access.Inputs.FirstOrDefault(inp => inp.Name.Contains("vmpk", StringComparison.OrdinalIgnoreCase));
            }

            IMidiInput input = null;
            if (midiPort != null)
            {
                Console.WriteLine($"Found {midiPort.Name}. Opening...");
                input = await access.OpenInputAsync(midiPort.Id);

                // There seems to be a pretty bad race condition inside the library.
                // If your MIDI device is streaming data but there is no MessageReceived delegate, it will
                // throw a NRE. So register this dummy here for now.
                input.MessageReceived += (obj, e) => {};
            }
            else
            {
                Console.WriteLine("No suitable MIDI port found.");
            }

            Task.Run(async () =>
            {
                using (var connection = new Connection(Address.System))
                {
                    await connection.ConnectAsync();

                    var hci0Path = new ObjectPath("/org/bluez/hci0");
                    var serviceName = "org.bluez";
                    var hci0Adapter = connection.CreateProxy<IAdapter1>(serviceName, hci0Path);

                    // Based on BlueZ sample code. Verify what it does. Does it really turn my BT adapeter on if it was off?
                    Console.WriteLine("Setting power to true on device 1");
                    await hci0Adapter.SetPoweredAsync(true);

                    var advertisement = new LEAdvertisement(new LEAdvertisementProperties { Type = "peripheral", LocalName = "MIDI-BRIDGE" });
                    advertisement.LEAdvertisementProperties.ServiceUUIDs = new string[] { "03B80E5A-EDE8-4B33-A751-6CE34EC4C700" };

                    // Generic computer icon
                    advertisement.LEAdvertisementProperties.Appearance = 0x0080;

                    // We need to register the LEAdvertisement object. This basically publishes the object
                    // so that when we get the DBus callback to read all the properties, we're good to go.
                    await connection.RegisterObjectAsync(advertisement);

                    var advertisingManager = connection.CreateProxy<ILEAdvertisingManager1>(serviceName, hci0Path);
                    try {
                    await advertisingManager.RegisterAdvertisementAsync(advertisement, new Dictionary<string, object>());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Could not advertise. Already connected?");
                    }

                    Application application = new Application(@"/org/bluez/example");
                    GattService1 service = new GattService1(application.ObjectPath, 0, "03B80E5A-EDE8-4B33-A751-6CE34EC4C700", true);

                    GattCharacteristic1 gattCharacteristic1 = new GattCharacteristic1(service.ObjectPath, 0, "7772E5DB-3868-4112-A1A9-F2669D106BF3", new string[] { "notify", "read", "write-without-response" });

                    // TODO: Remove PlayNote out of gattCharacteristic. Bridge should handle locking.
                    Bridge midiBleBridge = new Bridge(data => gattCharacteristic1.Value = data);
                    midiBleBridge.StartActiveSense();
                    if (input != null)
                    {
                        input.MessageReceived += (obj, e) =>
                        {
                            midiBleBridge.ReceiveMidiMessage(e.Length, e.Data);
                        };
                    }
                    service.AddCharacteristic(gattCharacteristic1);
                    application.AddService(service);

                    await connection.RegisterObjectAsync(application);

                    var gattManager = connection.CreateProxy<IGattManager1>(serviceName, new ObjectPath(@"/org/bluez/hci0"));
                    await connection.RegisterObjectAsync(gattCharacteristic1);

                    await gattManager.RegisterApplicationAsync(application, new Dictionary<string, object>());

                    Console.WriteLine("Press <Ctrl>+c to exit...");
                    await Task.Delay(int.MaxValue);
                }
            }).Wait();

            await input.CloseAsync();
        }
    }
}
