using System.Text;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public sealed class TcpTypeWordString : BaseTcpTypeString<TcpTypeWord, TcpTypeWordString>, ITcpCloneable<TcpTypeWordString>
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
        protected override TcpTypeWord CreateHeader(bool bigEndian, int defaultValue)
        {
            return new TcpTypeWord(bigEndian) { Value = (ushort)defaultValue };
        }
        protected override int GetHeaderValue(TcpTypeWord header)
        {
            return header.Value;
        }
        //---------------------------------------------------------------------
        protected override TcpTypeWordString CreateObjectClone(bool headerBigEndian, Encoding encoding)
        {
            return new TcpTypeWordString(headerBigEndian, encoding);
        }
        #endregion
        public TcpTypeWordString()
        { }
        public TcpTypeWordString(bool headerBigEndian)
             : base(headerBigEndian)
        { }
        public TcpTypeWordString(bool headerBigEndian, Encoding encoding)
            : base(headerBigEndian, encoding)
        { }

        //---------------------------------------------------------------------
        public const int MaxLength = TcpTypeWord.MaxValue;
        internal const int MinLength = TcpTypeWord.MinValue;
        //---------------------------------------------------------------------
        public static implicit operator string(TcpTypeWordString tcpType)
        {
            return tcpType.Value;
        }
        //---------------------------------------------------------------------
        public TcpTypeWordString Clone()
        {
            return CreateClone();
        }
        //---------------------------------------------------------------------
    }
}