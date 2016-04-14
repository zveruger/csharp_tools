using System;

namespace Tools.CSharp.Connectors
{
    public class ServerConnectorConnectParams : BaseConnectorConnectParams, IServerConnectorConnectParams
    {
        public ServerConnectorConnectParams(string address, int port, int millisecondsTimeout = DefaultMillisecondsTimeout)
            : base(millisecondsTimeout)
        {
            if (string.IsNullOrWhiteSpace(address))
            { throw new ArgumentNullException(nameof(address)); }
            if (port < 0)
            { throw new ArgumentOutOfRangeException(nameof(port)); }

            Address = address;
            Port = port;
        }

        //---------------------------------------------------------------------
        public string Address { get; }
        public int Port { get; }
        //---------------------------------------------------------------------
        public override string ToString()
        {
            return $"{"Address"}={Address}, {"Port"}={Port.ToString()}";
        }
        //---------------------------------------------------------------------
    }
}