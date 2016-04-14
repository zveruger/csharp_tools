using Tools.CSharp.Network.Tcp.DataTypes;

namespace Tools.CSharp.Network.Tcp.Connectors
{
    public interface ITcpConnectorSendAndReceive
    {
        //---------------------------------------------------------------------
        void Send(ITcpTypeData data);
        //---------------------------------------------------------------------
        bool Receive(out ITcpTypeData receivedData);
        bool Receive(int millisecondsTimeout, out ITcpTypeData receivedData);
        //---------------------------------------------------------------------
    }
}