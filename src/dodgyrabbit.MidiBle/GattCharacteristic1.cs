using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Tmds.DBus;

namespace dodgyrabbit.MidiBle
{
    public class GattCharacteristic1 : IGattCharacteristic1
    {
        // TODO: GattCharacteristics should support GattDescriptors
        ObjectPath parentPath;
        ObjectPath objectPath;
        int index;

        byte[] value;

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
        public byte[] Value 
        {   
            get 
            {
                return value;
            }
            private set
            {
                this.value = value;
                OnPropertiesChanged?.Invoke(PropertyChanges.ForProperty(nameof(Value), value));
            }
        }

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
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<byte[]> ReadValueAsync(IDictionary<string, object> options)
        {
            return Task.FromResult <byte[]>(new byte[0]);
        }

        // Quick HACK to get a loop going. Do not do this at home.
        public void StartMidiHeartbeat()
        {
            Task.Run(async () =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var buffer = new byte[3];
                while (true)
                {
                    if (isRunning)
                    {

                        long millis = stopwatch.ElapsedMilliseconds;

                        buffer[0] = (byte)(((millis >> 7) & 0x3F) | (long)0x80); //6 bits plus MSB
                        buffer[1] = (byte)((millis & 0x7F) | 0x80); //7 bits plus MSB
                        buffer[2] = 0xFE;
                        Value = buffer;
                    }

                    // don't run again for at least 200 milliseconds
                    await Task.Delay(150);
                }
            });
        }

        volatile bool isRunning;

        /// <inheritdoc />
        public Task StartNotifyAsync()
        {
            isRunning = true;
            return Task.Run(() =>  {
                // Respond with empty payload on initial request
                //Value = new byte[] {};
                Console.WriteLine("Received incoming notification");
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
            return Task.FromResult<byte[]>(new byte[0]);
        }

        public event Action<PropertyChanges> OnPropertiesChanged;

        public Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler)
        {
            return SignalWatcher.AddAsync(this, nameof(OnPropertiesChanged), handler);
        }
    }
}