using System.Collections.Generic;
using System.Threading.Tasks;
using Tmds.DBus;

namespace dodgyrabbit.MidiBle
{

    public interface IGattCharacteristic1 : IDBusObject
    {
        /// <summary>
        /// Issues a request to read the value of the characteristic and returns the value if the operation was successful.
        /// </summary>
        /// <param name="options">
        /// Possible options: "offset": uint16 offset
		/// "mtu": Exchanged MTU (Server only)
		/// "device": Object Device (Server only)
        /// </param>
        /// <returns></returns>
        Task<byte[]> ReadValueAsync(IDictionary<string, object> options);

        /// <summary>
        /// Issues a request to write the value of the characteristic.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<byte[]> WriteValueAsync(byte[] value, IDictionary<string, object> options);

        /// <summary>
        /// Starts a notification session from this characteristic if it supports value notifications or indications.
        /// </summary>
        Task StartNotifyAsync();

        /// <summary>
        /// This method will cancel any previous StartNotify transaction. Note that notifications from a characteristic are shared between sessions thus  calling StopNotify will release a single session.
        /// </summary>
        Task StopNotifyAsync();

        /// <summary>
        /// This method doesn't expect a reply so it is just a confirmation that value was received.
        /// </summary>
        Task ConfirmAsync();

        /// <summary>
        /// 128-bit characteristic UUID.
        /// </summary>
        string UUID {get;}

        /// <summary>
        /// Object path of the GATT service the characteristic belongs to.
        /// </summary>
        ObjectPath Service {get;}

        /// <summary>
        /// The cached value of the characteristic. This property gets updated only after a successful read request and  when a notification or indication is received, upon which a PropertiesChanged signal will be emitted.
        /// </summary>
        byte[] Value{get;}
		
        /// <summary>
        /// True, if this characteristic has been acquired by any client using AcquireWrite.
        /// For client properties is ommited in case 'write-without-response' flag is not set.
        /// For server the presence of this property indicates that AcquireWrite is supported.
        /// </summary>
		bool WriteAcquired {get;}

        /// <summary>
        /// True, if this characteristic has been acquired by any client using AcquireNotify.
	    /// For client this properties is ommited in case 'notify' flag is not set.
	    /// For server the presence of this property indicates that AcquireNotify is supported.
        /// </summary>
		bool NotifyAcquired {get;}

        /// <summary>
		/// True, if notifications or indications on this characteristic are currently enabled.
        /// </summary>
		bool Notifying {get;}

        // TODO: this should probably be an areay of enums, or a flags enum?
        /// <summary>
        /// Defines how the characteristic value can be used. See Core spec "Table 3.5: Characteristic Properties bit field", and "Table 3.8: Characteristic Extended Properties bit field". Allowed values:
        /// "broadcast"
        /// "read"
        /// "write-without-response"
        /// "write"
        /// "notify"
        /// "indicate"
        /// "authenticated-signed-writes"
        /// "reliable-write"
        /// "writable-auxiliaries"
        /// "encrypt-read"
        /// "encrypt-write"
        /// "encrypt-authenticated-read"
        /// "encrypt-authenticated-write"
        /// "secure-read" (Server only)
        /// "secure-write" (Server only)
        /// "authorize"
        /// </summary>
		string[] Flags {get;}
    }
}