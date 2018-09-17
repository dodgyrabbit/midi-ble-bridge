using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using bluez.DBus;
using Tmds.DBus;

[assembly: InternalsVisibleTo(Tmds.DBus.Connection.DynamicAssemblyName)]
namespace dodgyrabbit.MidiBle
{
    public class DummyDisposable : IDisposable
    {
        public void Dispose() {}
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
            // TODO: The GetInterfaceName seems problematic. If we implement more than one, this may fail.
            // Better solution may be that it is returned by the object itself.
            // Alternatively we "code" it here. May be OK.

            foreach(GattService1 service in services)
            {
                IDictionary<string, IDictionary<string, object>> serviceDictionary = new Dictionary<string, IDictionary<string, object>>();
                serviceDictionary[GetInterfaceName(service)] = await service.GetAllAsync();
                objects[service.ObjectPath] = serviceDictionary;

                foreach (GattCharacteristic1 characteristic in service.GetCharacteristics())
                {
                    IDictionary<string, IDictionary<string, object>> characteristics = new Dictionary<string, IDictionary<string, object>>();
                    characteristics[GetInterfaceName(characteristic)] = await characteristic.GetAllAsync();
                    objects[characteristic.ObjectPath] = characteristics;
                }
            }
            return objects as IDictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>>;
        }

        /// <summary>
        /// Inspects the current type and returns the DBusInterface.
        /// </summary>
        /// <returns>The DBusInterface Name as decorated by the DBusInterfaceAttribute.</returns>
        private string GetInterfaceName(object o)
        {
            Attribute attribute = o.GetType().GetCustomAttribute(typeof(DBusInterfaceAttribute), false);
            DBusInterfaceAttribute dBusInterface = attribute as DBusInterfaceAttribute;
            if (dBusInterface == null)
            {
                throw new InvalidOperationException("This type does not have the DBusInterfaceAttribute");
            }
            return dBusInterface.Name;
        }

        public Task<IDisposable>  WatchInterfacesAddedAsync(Action<(ObjectPath @object, IDictionary<string, IDictionary<string, object>> interfaces)> handler, Action<Exception> onError)
        {
            Console.WriteLine("WatchInterfacesAdded called");
            return Task.FromResult(new DummyDisposable() as IDisposable);
        }

        public  Task<IDisposable> WatchInterfacesRemovedAsync(Action<(ObjectPath @object, string[] interfaces)> handler, Action<Exception> onError)
        {
             Console.WriteLine("WatchInterfacesRemoved called");
            return Task.FromResult(new DummyDisposable() as IDisposable);
        }
    }
}
