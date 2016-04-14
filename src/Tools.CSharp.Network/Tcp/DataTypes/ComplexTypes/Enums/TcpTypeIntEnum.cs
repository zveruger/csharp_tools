using System;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public sealed class TcpTypeIntEnum<TEnum> : BaseTcpTypeEnum<TcpTypeInt, int, TEnum, TcpTypeIntEnum<TEnum>>, ITcpCloneable<TcpTypeIntEnum<TEnum>>
        where TEnum : struct
    {
        #region protected
        protected override TcpTypeInt CreateEnumContent(bool bigEndian)
        {
            return new TcpTypeInt(bigEndian);
        }
        protected override void UpdateEnumContentValue(TcpTypeInt enumContent, TEnum value)
        {
            enumContent.Value = Convert.ToInt32(value);
        }
        //---------------------------------------------------------------------
        protected override TcpTypeIntEnum<TEnum> CreateObjectClone(bool bigEndian)
        {
            return new TcpTypeIntEnum<TEnum>(bigEndian);
        }
        #endregion
        public TcpTypeIntEnum()
        { }
        public TcpTypeIntEnum(bool bigEndian)
            : base(bigEndian)
        { }

        //---------------------------------------------------------------------
        public TcpTypeIntEnum<TEnum> Clone()
        {
            return CreateClone();
        }
        //---------------------------------------------------------------------
    }
}