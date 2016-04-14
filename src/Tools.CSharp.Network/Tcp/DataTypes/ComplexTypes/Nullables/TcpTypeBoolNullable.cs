using System.Text;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public sealed class TcpTypeBoolNullable : BaseTcpTypeNullable<TcpTypeBool, TcpTypeBoolNullable>, ITcpCloneable<TcpTypeBoolNullable>
    {
        #region protected
        protected override TcpTypeBool CreateValue(bool bigEndian, Encoding encoding)
        {
            return new TcpTypeBool();
        }
        protected override TcpTypeBoolNullable CreateObjectClone(bool bigEndian, Encoding encoding)
        {
            return new TcpTypeBoolNullable();
        }
        #endregion
        public TcpTypeBoolNullable()
        { }

        //---------------------------------------------------------------------
        public TcpTypeBoolNullable Clone()
        {
            return CreateClone();
        }
        //---------------------------------------------------------------------
    }
}