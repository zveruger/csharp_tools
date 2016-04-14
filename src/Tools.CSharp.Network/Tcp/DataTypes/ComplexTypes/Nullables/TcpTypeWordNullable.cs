using System.Text;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public sealed class TcpTypeWordNullable : BaseTcpTypeNullable<TcpTypeWord, TcpTypeWordNullable>, ITcpCloneable<TcpTypeWordNullable>
    {
        #region protected
        protected override TcpTypeWord CreateValue(bool bigEndian, Encoding encoding)
        {
            return new TcpTypeWord(bigEndian);
        }
        protected override TcpTypeWordNullable CreateObjectClone(bool bigEndian, Encoding encoding)
        {
            return new TcpTypeWordNullable(bigEndian);
        }
        #endregion
        public TcpTypeWordNullable()
        { }
        public TcpTypeWordNullable(bool bigEndian)
            : base(bigEndian)
        { }

        //---------------------------------------------------------------------
        public TcpTypeWordNullable Clone()
        {
            return CreateClone();
        }
        //---------------------------------------------------------------------
    }
}