using System;
using System.Threading.Tasks;
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
                }
            }).Wait();
        }
    }
}
