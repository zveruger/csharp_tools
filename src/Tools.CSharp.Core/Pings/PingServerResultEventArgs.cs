using System;
using System.ComponentModel;

namespace Tools.CSharp.Pings
{
    public sealed class PingServerResultEventArgs : EventArgs
    {
        #region private
        private readonly PingServerStateResult _value;
        #endregion
        internal PingServerResultEventArgs(PingServerStateResult value)
        {
            if (value < PingServerStateResult.Init || value > PingServerStateResult.ServerFailed)
            { throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(PingServerStateResult)); }
            
            _value = value;
        }

        //---------------------------------------------------------------------
        public PingServerStateResult Value
        {
            get { return _value; }
        }
        //---------------------------------------------------------------------
    }
}