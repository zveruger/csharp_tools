using System.Text;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public sealed class TcpTypeDWordNullable : BaseTcpTypeNullable<TcpTypeDWord, TcpTypeDWordNullable>, ITcpCloneable<TcpTypeDWordNullable>
    {
        #region protected
        protected override TcpTypeDWord CreateValue(bool bigEndian, Encoding encoding)
        {
            return new TcpTypeDWord(bigEndian);
        }
        protected override TcpTypeDWordNullable CreateObjectClone(bool bigEndian, Encoding encoding)
        {
            return new TcpTypeDWordNullable(bigEndian);
        }
        #endregion
        public TcpTypeDWordNullable()
        { }
        public TcpTypeDWordNullable(bool bigEndian)
            : base(bigEndian)
        { }

        //---------------------------------------------------------------------
        public TcpTypeDWordNullable Clone()
        {
            return CreateClone();
        }
        //---------------------------------------------------------------------
    }
}