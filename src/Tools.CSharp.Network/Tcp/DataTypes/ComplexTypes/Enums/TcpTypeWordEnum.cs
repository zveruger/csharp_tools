using System;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public sealed class TcpTypeWordEnum<TEnum> : BaseTcpTypeEnum<TcpTypeWord, ushort, TEnum, TcpTypeWordEnum<TEnum>>, ITcpCloneable<TcpTypeWordEnum<TEnum>>
        where TEnum : struct 
    {
        #region protected
        protected override TcpTypeWord CreateEnumContent(bool bigEndian)
        {
            return new TcpTypeWord(bigEndian);
        }
        protected override void UpdateEnumContentValue(TcpTypeWord enumContent, TEnum value)
        {
            enumContent.Value = Convert.ToUInt16(value);
        }
        //---------------------------------------------------------------------
        protected override TcpTypeWordEnum<TEnum> CreateObjectClone(bool bigEndian)
        {
            return new TcpTypeWordEnum<TEnum>(bigEndian);
        }
        #endregion
        public TcpTypeWordEnum()
        { }
        public TcpTypeWordEnum(bool bigEndian)
            : base(bigEndian)
        { }

        //---------------------------------------------------------------------
        public TcpTypeWordEnum<TEnum> Clone()
        {
            return CreateClone();
        }
        //---------------------------------------------------------------------        
    }
}