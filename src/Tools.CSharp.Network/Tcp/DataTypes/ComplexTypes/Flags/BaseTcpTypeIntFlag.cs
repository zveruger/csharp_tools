namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public abstract class BaseTcpTypeIntFlag : BaseTcpTypeFlag<TcpTypeInt, int, BaseTcpTypeIntFlag>
    {
        #region protected
        protected override TcpTypeInt CreateFlagContent(bool bigEndian)
        {
            return new TcpTypeInt(bigEndian);
        }
        #endregion
        protected BaseTcpTypeIntFlag()
        { }
        protected BaseTcpTypeIntFlag(bool bigEndian)
            : base(bigEndian)
        { }
    }
}