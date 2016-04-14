using System.Text;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public abstract class BaseTcpTypeDWordArray<TItem, TTcpTypeDWordArray> : BaseTcpTypeArray<TcpTypeDWord, TItem, TTcpTypeDWordArray>
        where TItem : ITcpType
        where TTcpTypeDWordArray : BaseTcpTypeDWordArray<TItem, TTcpTypeDWordArray>
    {
        #region protected
        protected override TcpTypeDWord CreateCount(bool bigEndian)
        {
            return new TcpTypeDWord(bigEndian);
        }
        #endregion
        protected BaseTcpTypeDWordArray()
        { }
        protected BaseTcpTypeDWordArray(bool bigEndian)
            : base(bigEndian)
        { }
        protected BaseTcpTypeDWordArray(bool bigEndian, Encoding encoding)
            : base(bigEndian, encoding)
        { }
    }
}