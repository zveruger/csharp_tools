using System;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public sealed class TcpTypeDWordEnum<TEnum> : BaseTcpTypeEnum<TcpTypeDWord, uint, TEnum, TcpTypeDWordEnum<TEnum>>, ITcpCloneable<TcpTypeDWordEnum<TEnum>>
        where TEnum : struct 
    {
        #region protected
        protected override TcpTypeDWord CreateEnumContent(bool bigEndian)
        {
            return new TcpTypeDWord(bigEndian);
        }
        protected override void UpdateEnumContentValue(TcpTypeDWord enumContent, TEnum value)
        {
            enumContent.Value = Convert.ToUInt16(value);
        }
        //---------------------------------------------------------------------
        protected override TcpTypeDWordEnum<TEnum> CreateObjectClone(bool bigEndian)
        {
            return new TcpTypeDWordEnum<TEnum>(bigEndian);
        }
        #endregion
        public TcpTypeDWordEnum()
        { }
        public TcpTypeDWordEnum(bool bigEndian)
            : base(bigEndian)
        { }

        //---------------------------------------------------------------------
        public TcpTypeDWordEnum<TEnum> Clone()
        {
            return CreateClone();
        }
        //---------------------------------------------------------------------
    }
}