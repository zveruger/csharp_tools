namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public abstract class BaseTcpTypeDWordFlag : BaseTcpTypeFlag<TcpTypeDWord, uint, BaseTcpTypeDWordFlag>
    {
        #region protected
        protected override TcpTypeDWord CreateFlagContent(bool bigEndian)
        {
            return new TcpTypeDWord(bigEndian);
        }
        #endregion
        protected BaseTcpTypeDWordFlag()
        { }
        protected BaseTcpTypeDWordFlag(bool bigEndian)
            : base(bigEndian)
        { }
    }
}