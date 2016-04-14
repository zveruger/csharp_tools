using System;
using System.Collections.Generic;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public interface ITcpType : ICloneable
    {
        //---------------------------------------------------------------------
        int DataLength { get; }
        //---------------------------------------------------------------------
        void Read(byte[] dataTarget, ref int startPositionInDataTarget);
        void Read(ITcpTypeData dataTarget, ref int startPositionInDataTarget);
        //---------------------------------------------------------------------
        void Write(byte[] dataSource, ref int startPositionInDataSource);
        void Write(ITcpTypeData dataSource, ref int startPositionInDataSource);
        //---------------------------------------------------------------------
    }
}