namespace RollingOutTools.CmdLine
{
    /// <summary>
    /// Опции для получения ресурсов из CmdLineExtensions.
    /// </summary>
    public class ReadResourseOptions
    {
        /// <summary>
        /// Подсказка пользователю.
        /// </summary>
        public string Hint { get; set; }

        /// <summary>
        /// Указывает нужно ли сохранят ресурс в кеш для последующего автозаполнения.
        /// </summary>
        public bool SaveToCache { get; set; } = true;

        /// <summary>
        /// Если истина, то у пользователя не будет запрошен ввод, а он будет взят из кеша.
        /// </summary>
        public bool UseAutoread { get; set; } = false;
    }
}
