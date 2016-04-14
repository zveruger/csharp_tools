using System.Text;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public sealed class TcpTypeIntString : BaseTcpTypeString<TcpTypeInt, TcpTypeIntString>, ITcpCloneable<TcpTypeIntString>
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
        protected override TcpTypeInt CreateHeader(bool bigEndian, int defaultValue)
        {
            return new TcpTypeInt { Value = defaultValue };
        }
        protected override int GetHeaderValue(TcpTypeInt header)
        {
            return header.Value;
        }
        //---------------------------------------------------------------------
        protected override TcpTypeIntString CreateObjectClone(bool headerBigEndian, Encoding encoding)
        {
            return new TcpTypeIntString(headerBigEndian, encoding);
        }
        #endregion
        public TcpTypeIntString()
        { }
        public TcpTypeIntString(bool headerBigEndian)
            : base(headerBigEndian)
        { }
        public TcpTypeIntString(bool headerBigEndian, Encoding encoding)
            : base(headerBigEndian, encoding)
        { }

        //---------------------------------------------------------------------
        public const int MaxLength = TcpTypeInt.MaxValue;
        internal const int MinLength = TcpTypeInt.MinValue;
        //---------------------------------------------------------------------
        public static implicit operator string(TcpTypeIntString tcpType)
        {
            return tcpType.Value;
        }
        //---------------------------------------------------------------------
        public TcpTypeIntString Clone()
        {
            return CreateClone();
        }
        //---------------------------------------------------------------------
    }
}