using Tmds.DBus;

namespace dodgyrabbit.MidiBle
{

    [DBusInterface("org.bluez.GattService1")]
    public interface IGattService1 : IDBusObject
    {
        /// <summary>
        /// 128-bit service UUID. Read only.
        /// </summary>
    	string UUID { get; }

        /// <summary>
        /// Indicates whether or not this GATT service is a primary service. If false, the service is secondary.
        /// </summary>
		bool Primary { get; }
    }
}