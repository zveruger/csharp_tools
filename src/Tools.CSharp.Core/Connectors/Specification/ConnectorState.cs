namespace Tools.CSharp.Connectors
{
    public enum ConnectorState
    {
        /// <summary>
        /// Подключение к объекту.
        /// </summary>
        Connecting,

        /// <summary>
        /// Подключен к объекту.
        /// </summary>
        Connected,

        /// <summary>
        /// Отключение от объекта.
        /// </summary>
        Disconnecting,

        /// <summary>
        /// Отключен от объекта.
        /// </summary>
        Disconnected
    }
}
