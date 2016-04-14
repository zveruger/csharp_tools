namespace Tools.CSharp.Loggers
{
    public enum LoggerLevel
    {
        /// <summary>
        /// Не выводить сообщения логов.
        /// </summary>
        Off = 0,

        /// <summary>
        /// Выводить сообщения об ошибках.
        /// </summary>
        Error = 1,

        /// <summary>
        /// Выводить предупреждения и сообщения об ошибках.
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Выводить информационные сообщения, предупреждения и сообщения об ошибках.
        /// </summary>
        Info = 3,

        /// <summary>
        /// Выводить все сообщения.
        /// </summary>
        Verbose = 4
    }
}