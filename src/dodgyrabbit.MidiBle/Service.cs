using Tmds.DBus;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace dodgyrabbit.MidiBle
{
    [DBusInterface("org.bluez.GattService1")]
    public class GattService1 : IDBusObject
    {
        ObjectPath objectPath;
        public GattService1(ObjectPath objectPath)
        {
            this.objectPath = objectPath;
        }

        public ObjectPath ObjectPath => objectPath;
        // TODO: Return the required dictionary object here.
        // TODO: Create interface for GattService1. Implement it here.

        /// <summary>
        /// Inspects the current type and returns the DBusInterface.
        /// </summary>
        /// <returns>The DBusInterface Name as decorated by the DBusInterfaceAttribute.</returns>
        public string GetInterfaceName()
        {
            Attribute attribute = this.GetType().GetCustomAttribute(typeof(DBusInterfaceAttribute));
            DBusInterfaceAttribute dBusInterface = attribute as DBusInterfaceAttribute;
            if (dBusInterface == null)
            {
                throw new InvalidOperationException("This type does not have the DBusInterfaceAttribute");
            }
            return dBusInterface.Name;
        }

        public Task<ServiceProperties> GetAllAsync()
        {
            return null;
        }
    }

    [Dictionary]
    public class ServiceProperties 
    {
    }

}