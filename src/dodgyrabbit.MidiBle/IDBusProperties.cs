using System.Collections.Generic;
using System.Threading.Tasks;
using Tmds.DBus;

namespace dodgyrabbit.MidiBle
{

    [DBusInterface("org.freedesktop.DBus.Properties")]
    public interface IDBusProperties
    {
        Task<IDictionary<string, object>> GetAllAsync(string Interface);
    }
}