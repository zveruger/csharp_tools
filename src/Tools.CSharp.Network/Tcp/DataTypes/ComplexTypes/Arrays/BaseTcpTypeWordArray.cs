using System.Text;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public abstract class BaseTcpTypeWordArray<TItem, TTcpTypeWordArray> : BaseTcpTypeArray<TcpTypeWord, TItem, TTcpTypeWordArray>
        where TItem : ITcpType
        where TTcpTypeWordArray : BaseTcpTypeWordArray<TItem, TTcpTypeWordArray>
    {
        #region protected
        protected override TcpTypeWord CreateCount(bool bigEndian)
        {
            return new TcpTypeWord(bigEndian);
        }
        #endregion
        protected BaseTcpTypeWordArray()
        { }
        protected BaseTcpTypeWordArray(bool bigEndian)
            : base(bigEndian)
        { }
        protected BaseTcpTypeWordArray(bool bigEndian, Encoding encoding)
            : base(bigEndian, encoding)
        { }
    }
}