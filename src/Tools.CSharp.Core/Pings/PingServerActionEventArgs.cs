using System;

namespace Tools.CSharp.Pings
{
    public abstract class PingServerActionEventArgs : EventArgs
    {
        public abstract void SetServerAddress(string address);
    }
}