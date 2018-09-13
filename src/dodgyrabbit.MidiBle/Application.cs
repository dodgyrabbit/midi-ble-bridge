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
            var result = new Dictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>>();
            IDictionary<string, IDictionary<string, object>> service = new Dictionary<string, IDictionary<string, object>>();
            IDictionary<string, IDictionary<string, object>> service2 = new Dictionary<string, IDictionary<string, object>>();

            IDictionary<string, object> serviceCharacteristics = new Dictionary<string, object>();
            serviceCharacteristics["UUID"] = "00002a37-0000-1000-8000-00805f9b34fb";
            serviceCharacteristics["Service"] = new ObjectPath("/org/bluez/example/service0");
            serviceCharacteristics["Flags"] = new string[] {"notify"};
            serviceCharacteristics["Descriptors"] = new Object[] {};
            service["org.bluez.GattCharacteristic1"] = serviceCharacteristics;

            IDictionary<string, object> service0 = new Dictionary<string, object>();
            service0["Characteristics"] = new ObjectPath[] {new ObjectPath("/org/bluez/example/service0/char0")};
            service0["UUID"] = "0000180d-0000-1000-8000-00805f9b34fb";
            service0["Primary"] = true;
            service2["org.bluez.GattService1"] = service0;

            result[new ObjectPath("/org/bluez/example/service0/char0")] = service;
            result[new ObjectPath("/org/bluez/example/service0")] = service2;
            return result;
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
