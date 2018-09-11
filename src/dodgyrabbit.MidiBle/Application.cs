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

    [Dictionary]
    public class Application : IObjectManager
    {
        ObjectPath objectPath;
        public Application(ObjectPath objectPath)
        {
            this.objectPath = objectPath;
        }

        public ObjectPath ObjectPath => objectPath;

        public Task<IDictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>>> GetManagedObjectsAsync()
        {
            Console.WriteLine("Got this far!");
            return null;
        }

        Task<IDisposable> IObjectManager.WatchInterfacesAddedAsync(Action<(ObjectPath @object, IDictionary<string, IDictionary<string, object>> interfaces)> handler, Action<Exception> onError)
        {
            Console.WriteLine("WatchInterfacesAdded called");
            return null;
        }

        Task<IDisposable> IObjectManager.WatchInterfacesRemovedAsync(Action<(ObjectPath @object, string[] interfaces)> handler, Action<Exception> onError)
        {
             Console.WriteLine("WatchInterfacesRemoved called");
            return null;
        }
    }
}
