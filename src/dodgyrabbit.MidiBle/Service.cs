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
    public class GattService1 : IDBusObject, IGattService1
    {
        ObjectPath objectPath;
        public GattService1(ObjectPath objectPath, string UUID, Boolean primary)
        {
            this.objectPath = objectPath;
            this.UUID = UUID;
            this.Primary = primary;
        }

        /// <summary>
        /// 128-bit service UUID. Read only.
        /// </summary>
        public string UUID { get; private set; }

        /// <summary>
        /// Indicates whether or not this GATT service is a primary service. If false, the service is secondary.
        /// </summary>
        public bool Primary { get; private set; }

        public ObjectPath ObjectPath => objectPath;

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

        public Task<IDictionary<string, object>> GetAllAsync()
        {
            return Task.FromResult<IDictionary<string, object>>(
                new Dictionary<string, object>()
                {
                    { nameof(UUID), UUID},
                    { nameof(Primary), Primary}
                });
        }
    }

}