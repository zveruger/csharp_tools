using System.Collections.Generic;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public interface ITcpTypeComposite : ITcpType, IEnumerable<ITcpType>
    {
        //---------------------------------------------------------------------
        void ContinueWrite(ITcpTypeData dataSource, ref int startPositionInDataSource);
        //---------------------------------------------------------------------
    }
}