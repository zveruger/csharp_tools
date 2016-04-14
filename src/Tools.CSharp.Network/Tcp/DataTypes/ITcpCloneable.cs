namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public interface ITcpCloneable<out TOutputTcpType>
        where TOutputTcpType : ITcpCloneable<TOutputTcpType>
    {
        //---------------------------------------------------------------------
        TOutputTcpType Clone();
        //---------------------------------------------------------------------
    }
}