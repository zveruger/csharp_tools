using Tools.CSharp.StateMachines;

namespace Tools.CSharp.Connectors
{
    public interface IConnector<TConnectorConnectParams> : IStateble<ConnectorState>
        where TConnectorConnectParams : IConnectorConnectParams
    {
        //---------------------------------------------------------------------
        bool IsConnected { get; }
        //---------------------------------------------------------------------
        TConnectorConnectParams ConnectParamsWhereConnect { get; }
        //---------------------------------------------------------------------
        void Connect(TConnectorConnectParams connectParams);
        void Disconnect();
        //---------------------------------------------------------------------
    }
}