using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public sealed class TcpTypeByteString : BaseTcpTypeString<TcpTypeByte, TcpTypeByteString>, ITcpCloneable<TcpTypeByteString>
    {
        #region protected
        protected override int GetMaxLength
        {
            get { return MaxLength; }
        }
        protected override int GetMinLength
        {
            get { return MinLength; }
        }
        //---------------------------------------------------------------------
        protected override TcpTypeByte CreateHeader(bool bigEndian, int defaultValue)
        {
            return new TcpTypeByte { Value = (byte)defaultValue };
        }
        protected override int GetHeaderValue(TcpTypeByte header)
        {
            return header.Value;
        }
        //---------------------------------------------------------------------
        protected override TcpTypeByteString CreateObjectClone(bool headerBigEndian, Encoding encoding)
        {
            return new TcpTypeByteString(encoding);
        }
        #endregion
        public TcpTypeByteString()
        { }
        public TcpTypeByteString(Encoding encoding)
            : base(true, encoding)
        { }

        //---------------------------------------------------------------------
        public const int MaxLength = TcpTypeByte.MaxValue;
        internal const int MinLength = TcpTypeByte.MinValue;
        //---------------------------------------------------------------------
        public static implicit operator string(TcpTypeByteString tcpType)
        {
            return tcpType.Value;
        }
        //---------------------------------------------------------------------
        public TcpTypeByteString Clone()
        {
            return CreateClone();
        }
        //---------------------------------------------------------------------
    }
}