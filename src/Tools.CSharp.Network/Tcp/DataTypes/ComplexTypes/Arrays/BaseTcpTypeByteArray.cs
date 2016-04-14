using System.Text;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public abstract class BaseTcpTypeByteArray<TItem, TTcpTypeByteArray> : BaseTcpTypeArray<TcpTypeByte, TItem, TTcpTypeByteArray>
        where TItem : ITcpType
        where TTcpTypeByteArray : BaseTcpTypeByteArray<TItem, TTcpTypeByteArray>
    {
        #region protected
        protected override TcpTypeByte CreateCount(bool bigEndian)
        {
            return new TcpTypeByte();
        }
        #endregion
        protected BaseTcpTypeByteArray()
        { }
        protected BaseTcpTypeByteArray(bool bigEndian)
            : base(bigEndian)
        { }
        protected BaseTcpTypeByteArray(bool bigEndian, Encoding encoding)
            : base(bigEndian, encoding)
        { }
    }
}