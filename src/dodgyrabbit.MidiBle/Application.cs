using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using bluez.DBus;
using Tmds.DBus;

[assembly: InternalsVisibleTo(Tmds.DBus.Connection.DynamicAssemblyName)]
namespace dodgyrabbit.MidiBle
{
    // Do I need to override this? Would that break something?
    // Should be at "org.freedesktop.DBus.ObjectManager"
    // which is what the base class provide.
    //
    // This interface does not seem to exist, but it's used on the Python document
    //[DBusInterface("org.bluez.GattApplication1")]

    // TODO:
    //  Currently getting a serialization error.
    //  Do we seperate out the interface from the class? Maybe that is a critical part of how the DBus plumbing works

    // Tried a bunch of things, getting error now:
    // Tmds.DBus.DBusException: org.bluez.Error.Failed: No object received
    // This looks like it's happening from bluetoothd (prior to DBus?) as it's not yet on DBus
    // sudo btmon
    // Enable verbose logging probably required:
    // https://stackoverflow.com/questions/37003147/i-want-to-enable-debug-messages-on-bluez
    // https://wiki.ubuntu.com/DebuggingBluetooth
    
    public class dummy : IDisposable
    {
        public void Dispose()
        {
            
        }
    }

    //[DBusInterface("org.freedesktop.DBus.ObjectManager")]
    public class Application : IObjectManager
    {
        ObjectPath objectPath;
        public Application(ObjectPath objectPath)
        {
            this.objectPath = objectPath;
        }

        public ObjectPath ObjectPath => objectPath;

        public async Task<IDictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>>> GetManagedObjectsAsync()
        {
            Console.WriteLine("Got this far!");
            IDictionary<string, object> innerMost = new Dictionary<string, object>();
            IDictionary<string, IDictionary<string, object>> inner = new Dictionary<string, IDictionary<string, object>>();

            innerMost["UUID"] = "00002a37-0000-1000-8000-00805f9b34fb";
            innerMost["Service"] = "/org/bluez/example/service0";
            innerMost["Flags"] = new string[] {"Notify"};
            innerMost["Descriptors"] = new string[0];

            inner["org.bluez.GattCharacteristic1"] = innerMost;

            ObjectPath op = new ObjectPath("/org/bluez/example/service0/char0");

            var dict = new Dictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>>();
            dict[op] = inner;

            return dict;
            //return new Dictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>>();
            //return await Task.Run<Dictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>>>(null, null);
        }

        

        public async Task<IDisposable>  WatchInterfacesAddedAsync(Action<(ObjectPath @object, IDictionary<string, IDictionary<string, object>> interfaces)> handler, Action<Exception> onError)
        {
            Console.WriteLine("WatchInterfacesAdded called");
            return new dummy();
        }

        public async Task<IDisposable> WatchInterfacesRemovedAsync(Action<(ObjectPath @object, string[] interfaces)> handler, Action<Exception> onError)
        {
             Console.WriteLine("WatchInterfacesRemoved called");
            return new dummy();
        }
    }
}
