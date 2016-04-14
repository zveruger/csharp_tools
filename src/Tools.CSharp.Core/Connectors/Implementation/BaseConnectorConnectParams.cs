using System;
using System.Threading;

namespace Tools.CSharp.Connectors
{
    public abstract class BaseConnectorConnectParams : IConnectorConnectParams
    {
        #region private
        private readonly int _millisecondsTimeout;
        #endregion
        protected BaseConnectorConnectParams()
            : this(DefaultMillisecondsTimeout)
        { }
        protected BaseConnectorConnectParams(int millisecondsTimeout)
        {
            if (millisecondsTimeout < InfinityMillisecondsTimeout)
            { throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout)); }

            _millisecondsTimeout = millisecondsTimeout;
        }

        //---------------------------------------------------------------------
        public const int InfinityMillisecondsTimeout = Timeout.Infinite;
        //---------------------------------------------------------------------
        public const int DefaultMillisecondsTimeout = 5000;
        //---------------------------------------------------------------------
        public int MillisecondsTimeout
        {
            get { return _millisecondsTimeout; }
        }
        //---------------------------------------------------------------------
        public override string ToString()
        {
            return $"Timeout: {_millisecondsTimeout.ToString()}";
        }
        //---------------------------------------------------------------------
    }
}