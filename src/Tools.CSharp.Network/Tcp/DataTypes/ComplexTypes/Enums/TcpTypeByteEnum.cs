using System;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public sealed class TcpTypeByteEnum<TEnum> : BaseTcpTypeEnum<TcpTypeByte, byte, TEnum, TcpTypeByteEnum<TEnum>>, ITcpCloneable<TcpTypeByteEnum<TEnum>>
        where TEnum : struct 
    {
        #region protected
        protected override TcpTypeByte CreateEnumContent(bool bigEndian)
        {
            return new TcpTypeByte();
        }
        protected override void UpdateEnumContentValue(TcpTypeByte enumContent, TEnum value)
        {
            enumContent.Value = Convert.ToByte(value);
        }
        //---------------------------------------------------------------------
        protected override TcpTypeByteEnum<TEnum> CreateObjectClone(bool bigEndian)
        {
            return new TcpTypeByteEnum<TEnum>();
        }
        #endregion
        public TcpTypeByteEnum()
        { }

        //---------------------------------------------------------------------
        public TcpTypeByteEnum<TEnum> Clone()
        {
            return CreateClone();
        }
        //---------------------------------------------------------------------
    }
}