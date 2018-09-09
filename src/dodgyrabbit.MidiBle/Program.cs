using System;
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

                    var objectPath = new ObjectPath("/org/bluez/hci0");
                    var service = "org.bluez.Adapter1";
                    var bluezService = connection.CreateProxy<IAdapter1>(service, objectPath);
                }
            }).Wait();
        }
    }
}
