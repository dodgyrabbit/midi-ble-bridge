namespace Dodgyrabbit.MidiBle
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tmds.DBus;

    public class GattCharacteristic1 : IGattCharacteristic1
    {
        readonly ObjectPath parentPath;
        readonly ObjectPath objectPath;
        int index;
        byte[] value;

        public GattCharacteristic1(ObjectPath parentPath, int index, string uuid, string[] flags)
        {
            this.parentPath = parentPath;
            this.index = index;
            this.UUID = uuid;
            this.Flags = flags;
            this.objectPath = new ObjectPath(parentPath + "/characteristic" + index);
        }

        /// <inheritdoc />
        public string UUID { get; private set; }

        public ObjectPath Service
        {
            get
            {
                return parentPath;
            }
        }

        /// <inheritdoc />
        public byte[] Value
        {
            get
            {
                return value;
            }

            set
            {
                if (value[2] != 254)
                {
                    Console.WriteLine($"{value[0]},{value[1]},{value[2]}");
                }
                this.value = value;
                // TODO: do more testing to determine if it's needed for the Notification request to come in first.
                if (isRunning)
                {
                    OnPropertiesChanged?.Invoke(PropertyChanges.ForProperty(nameof(Value), value));
                }
            }
        }

        /// <inheritdoc />
        public bool WriteAcquired { get; private set; }

        /// <inheritdoc />
        public bool NotifyAcquired { get; private set; }

        /// <inheritdoc />
        public bool Notifying { get; private set; }

        /// <inheritdoc />
        public string[] Flags { get; private set; }

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
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<byte[]> ReadValueAsync(IDictionary<string, object> options)
        {
            return Task.FromResult(new byte[0]);
        }

        volatile bool isRunning;

        /// <inheritdoc />
        public Task StartNotifyAsync()
        {
            isRunning = true;
            return Task.Run(() =>
            {
                // Respond with empty payload on initial request
                Console.WriteLine("Start notifications requested");
            });
        }

        /// <inheritdoc />
        public Task StopNotifyAsync()
        {
            isRunning = false;
            return Task.Run(() => Console.WriteLine("Connection closed"));
        }

        /// <inheritdoc />
        public Task<byte[]> WriteValueAsync(byte[] value, IDictionary<string, object> options)
        {
            return Task.FromResult(new byte[0]);
        }

        public event Action<PropertyChanges> OnPropertiesChanged;

        public Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler)
        {
            return SignalWatcher.AddAsync(this, nameof(OnPropertiesChanged), handler);
        }
    }
}