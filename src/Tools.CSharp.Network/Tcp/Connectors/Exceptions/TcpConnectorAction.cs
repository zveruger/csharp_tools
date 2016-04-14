namespace Tools.CSharp.Network.Tcp.Connectors
{
    public enum TcpConnectorAction
    {
        /// <summary>
        /// Установление связи с устройством.
        /// </summary>
        Connecting,

        /// <summary>
        /// Отправка сообщения.
        /// </summary>
        Send,

        /// <summary>
        /// Прием сообщения.
        /// </summary>
        Receive
    }
}