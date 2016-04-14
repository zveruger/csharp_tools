using System.Text;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public sealed class TcpTypeIntNullable : BaseTcpTypeNullable<TcpTypeInt, TcpTypeIntNullable>, ITcpCloneable<TcpTypeIntNullable>
    {
        #region protected
        protected override TcpTypeInt CreateValue(bool bigEndian, Encoding encoding)
        {
            return new TcpTypeInt(bigEndian);
        }
        protected override TcpTypeIntNullable CreateObjectClone(bool bigEndian, Encoding encoding)
        {
            return new TcpTypeIntNullable(bigEndian);
        }
        #endregion
        public TcpTypeIntNullable()
        { }
        public TcpTypeIntNullable(bool bigEndian)
            : base(bigEndian)
        { }

        //---------------------------------------------------------------------
        public TcpTypeIntNullable Clone()
        {
            return CreateClone();
        }
        //---------------------------------------------------------------------
    }
}