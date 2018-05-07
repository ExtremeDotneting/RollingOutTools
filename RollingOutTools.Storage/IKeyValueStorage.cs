using System.Threading.Tasks;

namespace RollingOutTools.Storage
{
    public interface IKeyValueStorage
    {
        Task<T> Get<T>(string key);
        Task Set(string key, object value);
    }
}