namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public abstract class BaseTcpTypeByteFlag<TTcpTypeByteFlag> : BaseTcpTypeFlag<TcpTypeByte, byte, TTcpTypeByteFlag>
        where TTcpTypeByteFlag : BaseTcpTypeByteFlag<TTcpTypeByteFlag>
    {
        #region protected
        protected override TcpTypeByte CreateFlagContent(bool bigEndian)
        {
            return new TcpTypeByte();
        }
        #endregion
        protected BaseTcpTypeByteFlag()
        { }
    }
}