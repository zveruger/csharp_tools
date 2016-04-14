namespace Tools.CSharp.Connectors
{
    public interface IServerConnectorConnectParams : IConnectorConnectParams
    {
        //---------------------------------------------------------------------
        string Address { get; }
        int Port { get; }
        //---------------------------------------------------------------------
    }

}