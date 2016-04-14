using System;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public sealed class TcpTypeByte : ITcpTypeValue<byte>, ITcpTypeConvertible, ITcpCloneable<TcpTypeByte>
    {
        #region private
        private byte _value;
        #endregion
        public TcpTypeByte()
        { }

        //---------------------------------------------------------------------
        public const byte MaxValue = byte.MaxValue;
        public const byte MinValue = byte.MinValue;
        //---------------------------------------------------------------------
        public static implicit operator byte(TcpTypeByte tcpType)
        {
            return tcpType.Value;
        }
        public static implicit operator TcpTypeByte(byte value)
        {
            return new TcpTypeByte { Value = value };
        }
        //---------------------------------------------------------------------
        public TypeCode GetTypeCode()
        {
            return TypeCode.Byte;
        }
        //---------------------------------------------------------------------
        public byte Value
        {
            get { return _value; }
            set { _value = value; }
        }
        public int DataLength
        {
            get { return 1; }
        }
        //---------------------------------------------------------------------
        public void Read(byte[] dataTarget, ref int startPositionInDataTarget)
        {
            if (dataTarget == null)
            { throw new ArgumentNullException(nameof(dataTarget)); }
            if (startPositionInDataTarget < 0 || startPositionInDataTarget > dataTarget.Length)
            { throw new ArgumentOutOfRangeException(nameof(startPositionInDataTarget)); }
            if (dataTarget.Length < (startPositionInDataTarget + DataLength))
            { throw new ArgumentException(string.Empty, nameof(startPositionInDataTarget)); }

            //-----------------------------------------------------------------
            dataTarget[startPositionInDataTarget] = _value;
            //-----------------------------------------------------------------
            startPositionInDataTarget += DataLength;
        }
        public void Read(ITcpTypeData dataTarget, ref int startPositionInDataTarget)
        {
            if (dataTarget == null)
            { throw new ArgumentNullException(nameof(dataTarget)); }
            if (startPositionInDataTarget < 0 || startPositionInDataTarget > dataTarget.Length)
            { throw new ArgumentOutOfRangeException(nameof(startPositionInDataTarget)); }
            if (dataTarget.Length < (startPositionInDataTarget + DataLength))
            { throw new ArgumentException(string.Empty, nameof(startPositionInDataTarget)); }

            //-----------------------------------------------------------------
            dataTarget[startPositionInDataTarget] = _value;
            //-----------------------------------------------------------------
            startPositionInDataTarget += DataLength;
        }
        //---------------------------------------------------------------------
        public void Write(byte[] dataSource, ref int startPositionInDataSource)
        {
            if (dataSource == null)
            { throw new ArgumentNullException(nameof(dataSource)); }
            if (startPositionInDataSource < 0 || startPositionInDataSource > dataSource.Length)
            { throw new ArgumentOutOfRangeException(nameof(startPositionInDataSource)); }
            if (dataSource.Length < (startPositionInDataSource + DataLength))
            { throw new ArgumentException(string.Empty, nameof(startPositionInDataSource)); }

            //-----------------------------------------------------------------
            _value = dataSource[startPositionInDataSource];
            //-----------------------------------------------------------------
            startPositionInDataSource += DataLength;
        }
        public void Write(ITcpTypeData dataSource, ref int startPositionInDataSource)
        {
            if (dataSource == null)
            { throw new ArgumentNullException(nameof(dataSource)); }
            if (startPositionInDataSource < 0 || startPositionInDataSource > dataSource.Length)
            { throw new ArgumentOutOfRangeException(nameof(startPositionInDataSource)); }
            if (dataSource.Length < (startPositionInDataSource + DataLength))
            { throw new ArgumentException(string.Empty, nameof(startPositionInDataSource)); }

            //-----------------------------------------------------------------
            _value = dataSource[startPositionInDataSource];
            //-----------------------------------------------------------------
            startPositionInDataSource += DataLength;
        }
        //---------------------------------------------------------------------
        object ICloneable.Clone()
        {
            return Clone();
        }
        public TcpTypeByte Clone()
        {
            return new TcpTypeByte { _value = _value };
        }
        //---------------------------------------------------------------------
        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(Value);
        }
        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(Value);
        }
        char IConvertible.ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(Value);
        }
        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(Value);
        }
        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(Value);
        }
        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(Value);
        }
        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(Value);
        }
        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(Value);
        }
        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(Value);
        }
        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(Value);
        }
        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(Value);
        }
        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(Value);
        }
        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(Value);
        }
        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(Value);
        }
        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return TcpConvert.DefaultToType(this, conversionType, provider);
        }
        //---------------------------------------------------------------------
        public override string ToString()
        {
            return Value.ToString();
        }
        public string ToString(IFormatProvider provider)
        {
            return Value.ToString(provider);
        }
        //--------------------------------------------------------------------
    }
}