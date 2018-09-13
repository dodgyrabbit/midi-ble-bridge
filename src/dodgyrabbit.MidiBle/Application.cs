using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using bluez.DBus;
using Tmds.DBus;

[assembly: InternalsVisibleTo(Tmds.DBus.Connection.DynamicAssemblyName)]
namespace dodgyrabbit.MidiBle
{
    // TODO:
    //  Currently getting a serialization error.
    //  Do we seperate out the interface from the class? Maybe that is a critical part of how the DBus plumbing works

    // Tried a bunch of things, getting error now:
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
            var objects = new Dictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>>();
            IDictionary<string, IDictionary<string, object>> service = new Dictionary<string, IDictionary<string, object>>();

            IDictionary<string, IDictionary<string, object>> serviceCharacteristic = new Dictionary<string, IDictionary<string, object>>();
            IDictionary<string, object> serviceCharacteristics = new Dictionary<string, object>();
            serviceCharacteristics["Descriptors"] = new ObjectPath[] {};
            serviceCharacteristics["Flags"] = new string[] {"notify"};
            // Heart rate measurement
            serviceCharacteristics["UUID"] = "00002a37-0000-1000-8000-00805f9b34fb";
            serviceCharacteristics["Service"] = new ObjectPath("/org/bluez/example/service0");
            serviceCharacteristic["org.bluez.GattCharacteristic1"] = serviceCharacteristics;

            IDictionary<string, object> serviceDetails = new Dictionary<string, object>();
            serviceDetails["Characteristics"] = new ObjectPath[] {new ObjectPath("/org/bluez/example/service0/char0")};
            // Heart rate service
            serviceDetails["UUID"] = "0000180d-0000-1000-8000-00805f9b34fb";
            serviceDetails["Primary"] = true;
            service["org.bluez.GattService1"] = serviceDetails;

            objects[new ObjectPath("/org/bluez/example/service0/char0")] = serviceCharacteristic;
            objects[new ObjectPath("/org/bluez/example/service0")] = service;
            return objects;
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
