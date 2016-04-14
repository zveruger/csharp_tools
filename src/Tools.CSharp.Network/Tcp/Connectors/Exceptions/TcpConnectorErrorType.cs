namespace Tools.CSharp.Network.Tcp.Connectors
{
    public enum TcpConnectorErrorType
    {
        /// <summary>
        /// �������������� ������.
        /// </summary>
        Unknown,

        /// <summary>
        /// ���������� �� ������.
        /// </summary>
        DeviceNotFound,

        /// <summary>
        /// ����� ���������� � �����������.
        /// </summary>
        ConnectionLost,

        /// <summary>
        /// ������������ �����.
        /// </summary>
        WrongPacket
    }
}