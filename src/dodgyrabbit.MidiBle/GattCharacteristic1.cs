using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tmds.DBus;

namespace dodgyrabbit.MidiBle
{

    // TODO: Should explicity implement IPropertyObject
    [DBusInterface("org.bluez.GattCharacteristic1")]
    public class GattCharacteristic1 : IGattCharacteristic1
    {
        // TODO: GattCharacteristics should support GattDescriptors
        ObjectPath parentPath;
        ObjectPath objectPath;
        int index;

        public GattCharacteristic1 (ObjectPath parentPath, int index, string UUID, string[] flags)
        {
            this.parentPath = parentPath;
            this.index = index;
            this.UUID = UUID;
            this.Flags = flags;
            this.objectPath = new ObjectPath(parentPath.ToString() + "/characteristic" + index);
        }

        /// <inheritdoc />
        public string UUID {get; private set;}
        public ObjectPath Service 
        {
            get
            {
                return parentPath;
            }
        }
        /// <inheritdoc />
        public byte[] Value {get; private set;}

        /// <inheritdoc />
        public bool WriteAcquired {get; private set;}

        /// <inheritdoc />
        public bool NotifyAcquired {get; private set;}

        /// <inheritdoc />
        public bool Notifying {get; private set;}

        /// <inheritdoc />
        public string[] Flags {get; private set;}

        /// <inheritdoc />
        public ObjectPath ObjectPath => objectPath;

        public Task<IDictionary<string, object>> GetAllAsync()
        {
            return Task.FromResult<IDictionary<string, object>>(
                new Dictionary<string, object>()
                {
                    { nameof(Service), Service},
                    { nameof(UUID), UUID},
                    { nameof(Flags), Flags}
                });
        }

        /// <inheritdoc />
        public Task ConfirmAsync()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task<byte[]> ReadValueAsync(IDictionary<string, object> options)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task StartNotifyAsync()
        {
            return Task.Run(() => Console.WriteLine("Received incoming notification"));
        }

        /// <inheritdoc />
        public Task StopNotifyAsync()
        {
            return Task.Run(() => Console.WriteLine("Connection closed"));
        }

        /// <inheritdoc />
        public Task<byte[]> WriteValueAsync(byte[] value, IDictionary<string, object> options)
        {
            throw new System.NotImplementedException();
        }
    }
}