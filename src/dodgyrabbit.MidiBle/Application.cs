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
        string basePath;
        ObjectPath objectPath;
        List<GattService1> services = new List<GattService1>();
        public Application(string path)
        {
            // We need to create services as children of this, so keep reference to the orignal string path
            this.basePath = path;
            this.objectPath = new ObjectPath(basePath);
        }

        public void AddService(GattService1 service)
        {
            services.Add(service);
        }

        public ObjectPath ObjectPath => objectPath;

/// <summary>
/// An API can optionally make use of this interface for one or more sub-trees of objects. The root of each sub-tree implements this interface so other applications can get all objects, interfaces and properties in a single method call. It is appropriate to use this interface if users of the tree of objects are expected to be interested in all interfaces of all objects in the tree; a more granular API should be used if users of the objects are expected to be interested in a small subset of the objects, a small subset of their interfaces, or both.
/// The method that applications can use to get all objects and properties is GetManagedObjects:
///
///  org.freedesktop.DBus.ObjectManager.GetManagedObjects (out DICT <OBJPATH,DICT <STRING,DICT<STRING,VARIANT>>> objpath_interfaces_and_properties);
///
/// The return value of this method is a dict whose keys are object paths. All returned object paths are children of the object path implementing this interface, i.e. their object paths start with the ObjectManager's object path plus '/'.
///
/// Each value is a dict whose keys are interfaces names. Each value in this inner dict is the same dict that would be returned by the org.freedesktop.DBus.Properties.GetAll() method for that combination of object path and interface. If an interface has no properties, the empty dict is returned.
/// </summary>
/// <returns></returns>
        public async Task<IDictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>>> GetManagedObjectsAsync()
        {
            var objects = new Dictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>>();
            

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

            foreach(GattService1 service in services)
            {
                IDictionary<string, IDictionary<string, object>> serviceDictionary = new Dictionary<string, IDictionary<string, object>>();
                serviceDictionary[service.GetInterfaceName()] = await service.GetAllAsync();

                objects[new ObjectPath("/org/bluez/example/service0")] = serviceDictionary;

                foreach (GattCharacteristic1 characteristic in service.GetCharacteristics())
                {
                    IDictionary<string, IDictionary<string, object>> characteristics = new Dictionary<string, IDictionary<string, object>>();
                    // TODO: helper to get object identifier here
                    characteristics[""] = await characteristic.GetAllAsync();

                // TODO: Need to lookup characteristic path
                objects["/org/bluez/example/service0/char0"] = characteristics;

                }
            }

            // objects[gattService.ObjectPath] = new Dictionary<string, IDictionary<string, object>>()
            //     {
            //         {gattService.GetInterfaceName(), await gattService.GetAllAsync()}
            //     };
            //gattService.GetAllAsync

            // objects[new ObjectPath("/org/bluez/example/service0/char0")] = serviceCharacteristic;
            // objects[new ObjectPath("/org/bluez/example/service0")] = service;
            return objects as IDictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>>;
        }

        public Task<IDisposable>  WatchInterfacesAddedAsync(Action<(ObjectPath @object, IDictionary<string, IDictionary<string, object>> interfaces)> handler, Action<Exception> onError)
        {
            Console.WriteLine("WatchInterfacesAdded called");
            return Task.FromResult(new dummy() as IDisposable);
        }

        public  Task<IDisposable> WatchInterfacesRemovedAsync(Action<(ObjectPath @object, string[] interfaces)> handler, Action<Exception> onError)
        {
             Console.WriteLine("WatchInterfacesRemoved called");
            return Task.FromResult(new dummy() as IDisposable);
        }
    }
}
