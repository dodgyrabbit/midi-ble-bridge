using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using bluez.DBus;
using Tmds.DBus;

namespace dodgyrabbit.MidiBle
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("MIDI to BLE Bridge");
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
                    GattService1 service = new GattService1(application.ObjectPath, 0, "0000180d-0000-1000-8000-00805f9b34fb", true);
                    service.AddCharacteristic(new GattCharacteristic1(service.ObjectPath, 0, "00002a37-0000-1000-8000-00805f9b34fb", new string[] {"notify"}));
                    application.AddService(service);

                    await connection.RegisterObjectAsync(application);

                    var gattManager = connection.CreateProxy<IGattManager1>(serviceName, new ObjectPath(@"/org/bluez/hci0"));

                    await gattManager.RegisterApplicationAsync(application, new Dictionary<string, object>());
                    await Task.Delay(10000);
                }
            }).Wait();
        }
    }
}
