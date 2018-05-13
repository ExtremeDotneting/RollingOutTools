using RollingOutTools.SimpleIoc;
using System.Threading.Tasks;

namespace RollingOutTools.Storage
{
    /// <summary>
    /// Класс для хранилища ключ-значение (с записью на диск).
    /// Вы можете подставить отдельную реализацию на каждой платформе.
    /// Статический класс - просто обертка для простоты использования.
    /// </summary>
    public static class Storage_HardDrive
    {
        static IKeyValueStorage _handler;

        /// <summary>
        /// Реализация.
        /// </summary>
        static IKeyValueStorage Handler
        {
            get
            {
                if (_handler == null)
                {

                    InitDependencies();
                }
                return _handler;
            }
        }

        /// <summary>
        /// Automatically synchronized with Set. 
        /// If key not exists - will return null for reference type and default value for value types.
        /// </summary>
        public static Task<T> Get<T>(string key) => Handler.Get<T>(key);

        /// <summary>
        /// Automatically synchronized with Get. If 'null' - will remove value.
        /// If you not closing application, recommended not await task.
        /// </summary>
        public static Task Set(string key, object value) => Handler.Set(key, value);

        /// <summary>
        /// Remember, that null value is equals that it not exists. 
        /// So, if before you set 'key_name' value to null, then ContainsKey will return false;
        /// </summary>
        public static Task<bool> ContainsKey(string key) => Handler.ContainsKey(key);

        /// <summary>
        /// Manual set handler. Use for fast initialization.
        /// </summary>
        public static void InitDependencies(IKeyValueStorage handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Automaticaly find handler.
        /// </summary>
        public static void InitDependencies()
        {
            _handler = IocGlob.CreateTypeInstance<IKeyValueStorage>(
                IocGlob.FindAssignable<IKeyValueStorage>(
                    new string[] { "RollingOutTools.Storage.JsonFileStorage" }
                    )
                );
        }
    }
}
