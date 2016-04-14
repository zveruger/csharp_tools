using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Tools.CSharp.Network.Tcp.Connectors
{
    [Serializable]
    public class TcpConnectorException : Exception
    {
        #region private
        private readonly TcpConnectorAction _action;
        private readonly TcpConnectorErrorType _errorType;
        //---------------------------------------------------------------------
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        private TcpConnectorException(string message, Exception innerException, TcpConnectorAction operation, TcpConnectorErrorType errorType)
            : base(message, innerException)
        {
            if (operation < TcpConnectorAction.Connecting || operation > TcpConnectorAction.Receive)
            { throw new InvalidEnumArgumentException(nameof(operation), (int)operation, typeof(TcpConnectorAction)); }
            if (errorType < TcpConnectorErrorType.Unknown || errorType > TcpConnectorErrorType.WrongPacket)
            { throw new InvalidEnumArgumentException(nameof(errorType), (int)errorType, typeof(TcpConnectorErrorType)); }

            _action = operation;
            _errorType = errorType;
        }
        #endregion
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected TcpConnectorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _action = (TcpConnectorAction)info.GetInt32(nameof(Action));
            _errorType = (TcpConnectorErrorType)info.GetInt32(nameof(ErrorType));
        }

        public TcpConnectorException(TcpConnectorAction operation, TcpConnectorErrorType errorType)
            : this(string.Empty, null, operation, errorType)
        { }
        public TcpConnectorException(TcpConnectorAction operation, TcpConnectorErrorType errorType, string message)
            : this(message, null, operation, errorType)
        { }
        public TcpConnectorException(TcpConnectorAction operation, TcpConnectorErrorType errorType, string message, Exception innerException)
            : this(message, innerException, operation, errorType)
        { }

        //---------------------------------------------------------------------
        public TcpConnectorAction Action
        {
            get { return _action; }
        }
        public TcpConnectorErrorType ErrorType
        {
            get { return _errorType; }
        }
        //---------------------------------------------------------------------
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(Action), (int)Action);
            info.AddValue(nameof(ErrorType), (int)ErrorType);
        }
        //---------------------------------------------------------------------

        public override string ToString()
        {
            return $"{GetType().Name}: {_action.ToString()}, {_errorType.ToString()}";
        }
        //---------------------------------------------------------------------
    }
}