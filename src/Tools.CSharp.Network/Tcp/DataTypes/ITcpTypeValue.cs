namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public interface ITcpTypeValue<TValue> : ITcpType
    {
        //---------------------------------------------------------------------
        TValue Value { get; set; }
        //---------------------------------------------------------------------
    }
}