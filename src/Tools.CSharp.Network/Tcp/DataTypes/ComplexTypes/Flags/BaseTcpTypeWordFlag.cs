namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public abstract class BaseTcpTypeWordFlag : BaseTcpTypeFlag<TcpTypeWord, ushort, BaseTcpTypeWordFlag>
    {
        #region protected
        protected override TcpTypeWord CreateFlagContent(bool bigEndian)
        {
            return new TcpTypeWord(bigEndian);
        }
        #endregion
        protected BaseTcpTypeWordFlag()
        { }
        protected BaseTcpTypeWordFlag(bool bigEndian)
            : base(bigEndian)
        { }
    }
}