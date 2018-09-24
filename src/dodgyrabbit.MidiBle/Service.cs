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
        ObjectPath applicationPath;
        ObjectPath objectPath;
        List<GattCharacteristic1> gattCharacteristics = new List<GattCharacteristic1>();
        public GattService1(ObjectPath parentPath,int index, string UUID, Boolean primary)
        {
            this.objectPath = new ObjectPath(parentPath.ToString() + "/service" + index);
            this.UUID = UUID;
            this.Primary = primary;
        }

        public void AddCharacteristic(GattCharacteristic1 characteristic)
        {
            this.gattCharacteristics.Add(characteristic);
        }

        public IEnumerable<GattCharacteristic1> GetCharacteristics()
        {
            return gattCharacteristics;
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

        public Task<IDictionary<string, object>> GetAllAsync()
        {
            Console.WriteLine("GetAllAsync called");
            return Task.FromResult<IDictionary<string, object>>(
                new Dictionary<string, object>()
                {
                    { nameof(UUID), UUID},
                    { nameof(Primary), Primary}
                });
        }
    }
}