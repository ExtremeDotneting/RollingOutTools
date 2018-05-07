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

        public static Task<T> Get<T>(string key) => Handler.Get<T>(key);
        public static Task Set(string key, object value) => Handler.Set(key, value);


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
