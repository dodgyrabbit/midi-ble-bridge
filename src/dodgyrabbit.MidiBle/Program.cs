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

                    var advertisingManager = connection.CreateProxy<ILEAdvertisingManager1>(serviceName, hci0Path);

                    // Based on BlueZ sample code. Can I make this anything?
                    var advertisingPath = new ObjectPath("/org/bluez/example/advertisement0");
                    await advertisingManager.RegisterAdvertisementAsync(advertisingPath, new Dictionary<string, object>());

                    await Task.Delay(10000);
                }
            }).Wait();
        }
    }
}
