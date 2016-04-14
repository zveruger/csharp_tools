using System;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public interface ITcpTypeData : ICloneable
    {
        //---------------------------------------------------------------------
        byte this[int index] { get; set; }
        int Length { get; }
        //---------------------------------------------------------------------
        byte[] ToArray();
        //---------------------------------------------------------------------
        void CopyTo(byte[] dataTarget, int startPositionInDataTarger);
        void CopyTo(ITcpTypeData dataTarget, int startPositionInDataTarger);
        //---------------------------------------------------------------------
        string Dump();
        //---------------------------------------------------------------------
    }
}
