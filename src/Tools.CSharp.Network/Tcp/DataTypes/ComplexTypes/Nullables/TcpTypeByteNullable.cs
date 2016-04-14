using System.Text;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public sealed class TcpTypeByteNullable : BaseTcpTypeNullable<TcpTypeByte, TcpTypeByteNullable>, ITcpCloneable<TcpTypeByteNullable>
    {
        #region protected
        protected override TcpTypeByte CreateValue(bool bigEndian, Encoding encoding)
        {
            return new TcpTypeByte();
        }
        protected override TcpTypeByteNullable CreateObjectClone(bool bigEndian, Encoding encoding)
        {
            return new TcpTypeByteNullable();
        }
        #endregion
        public TcpTypeByteNullable()
        { }

        //---------------------------------------------------------------------
        public TcpTypeByteNullable Clone()
        {
            return CreateClone();
        }
        //---------------------------------------------------------------------
    }
}