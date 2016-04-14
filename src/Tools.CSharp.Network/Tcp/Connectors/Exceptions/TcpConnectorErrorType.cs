namespace Tools.CSharp.Network.Tcp.Connectors
{
    public enum TcpConnectorErrorType
    {
        /// <summary>
        /// Неопределенная ошибка.
        /// </summary>
        Unknown,

        /// <summary>
        /// Устройство не найден.
        /// </summary>
        DeviceNotFound,

        /// <summary>
        /// Обрыв соединения с устройством.
        /// </summary>
        ConnectionLost,

        /// <summary>
        /// Неправильный пакет.
        /// </summary>
        WrongPacket
    }
}