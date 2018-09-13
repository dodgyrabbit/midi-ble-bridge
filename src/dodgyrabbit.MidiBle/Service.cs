using Tmds.DBus;

namespace dodgyrabbit.MidiBle
{

    public class Service : IDBusObject
    {
        ObjectPath objectPath;
        public Service(ObjectPath objectPath)
        {
            this.objectPath = objectPath;
        }

        public ObjectPath ObjectPath => objectPath;
        // TODO: Return the required dictionary object here.
        // TODO: Create interface for GattService1. Implement it here.
    }
}