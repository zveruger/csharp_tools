using System.Text;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public abstract class BaseTcpTypeIntArray<TItem, TTcpTypeIntArray> : BaseTcpTypeArray<TcpTypeInt, TItem, TTcpTypeIntArray>
        where TItem : ITcpType
        where TTcpTypeIntArray : BaseTcpTypeIntArray<TItem, TTcpTypeIntArray>
    {
        #region protected
        protected override TcpTypeInt CreateCount(bool bigEndian)
        {
            return new TcpTypeInt(bigEndian);
        }
        #endregion
        protected BaseTcpTypeIntArray()
        { }
        protected BaseTcpTypeIntArray(bool bigEndian)
            : base(bigEndian)
        { }
        protected BaseTcpTypeIntArray(bool bigEndian, Encoding encoding)
            : base(bigEndian, encoding)
        { }
    }
}