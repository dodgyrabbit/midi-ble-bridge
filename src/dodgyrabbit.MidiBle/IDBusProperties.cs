namespace dodgyrabbit.MidiBle
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tmds.DBus;
    
    /// <summary>
    /// This class is probably redundant and here because of a lack of understanding.
    /// It seems like any time you implement a DBusINterface you are expected to have
    /// some of these methods (Like GetAll).
    /// </summary>
    [DBusInterface("org.freedesktop.DBus.Properties")]
    public interface IDBusProperties
    {
        Task<IDictionary<string, object>> GetAllAsync(string Interface);
    }
}