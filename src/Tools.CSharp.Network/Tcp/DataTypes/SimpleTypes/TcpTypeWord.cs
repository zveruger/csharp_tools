using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public sealed class TcpTypeWord : ITcpTypeValue<ushort>, ITcpEndianness, ITcpTypeConvertible, ITcpCloneable<TcpTypeWord>
    {
        #region private
        private readonly bool _bigEndian;
        //---------------------------------------------------------------------
        private UnionValue _value;
        //---------------------------------------------------------------------
        [StructLayout(LayoutKind.Explicit)]
        private struct UnionValue
        {
            [FieldOffset(0)]
            public ushort Value;

            [FieldOffset(0)]
            public InternalData Data;
        }
        private struct InternalData
        {
            public byte Part0;
            public byte Part1;
        }
        #endregion
        public TcpTypeWord()
            : this(true)
        { }
        public TcpTypeWord(bool bigEndian)
        {
            _bigEndian = bigEndian;
        }

        //---------------------------------------------------------------------
        public const ushort MaxValue = ushort.MaxValue;
        public const ushort MinValue = ushort.MinValue;
        //---------------------------------------------------------------------
        public static implicit operator ushort(TcpTypeWord tcpType)
        {
            return tcpType.Value;
        }
        //---------------------------------------------------------------------
        public TypeCode GetTypeCode()
        {
            return TypeCode.UInt16;
        }
        //---------------------------------------------------------------------
        public ushort Value
        {
            get { return _value.Value; }
            set { _value.Value = value; }
        }
        public int DataLength
        {
            get { return 2; }
        }
        //---------------------------------------------------------------------
        public bool IsBigEndian
        {
            get { return _bigEndian; }
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
            var tmp = new UnionValue
            {
                Value = _bigEndian ? (ushort)IPAddress.HostToNetworkOrder(Value) : Value
            };
            //-----------------------------------------------------------------
            dataTarget[startPositionInDataTarget] = tmp.Data.Part0;
            dataTarget[startPositionInDataTarget + 1] = tmp.Data.Part1;
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
            var tmp = new UnionValue
            {
                Value = _bigEndian ? (ushort)IPAddress.HostToNetworkOrder(Value) : Value
            };
            //-----------------------------------------------------------------
            dataTarget[startPositionInDataTarget] = tmp.Data.Part0;
            dataTarget[startPositionInDataTarget + 1] = tmp.Data.Part1;
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
            var tmp = new UnionValue
            {
                Data =
                {
                    Part0 = dataSource[startPositionInDataSource],
                    Part1 = dataSource[startPositionInDataSource + 1]
                }
            };
            //-----------------------------------------------------------------
            Value = _bigEndian ? (ushort)IPAddress.NetworkToHostOrder(tmp.Value) : tmp.Value;
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
            var tmp = new UnionValue
            {
                Data =
                {
                    Part0 = dataSource[startPositionInDataSource],
                    Part1 = dataSource[startPositionInDataSource + 1]
                }
            };
            //-----------------------------------------------------------------
            Value = _bigEndian ? (ushort)IPAddress.NetworkToHostOrder(tmp.Value) : tmp.Value;
            //-----------------------------------------------------------------
            startPositionInDataSource += DataLength;
        }
        //---------------------------------------------------------------------
        object ICloneable.Clone()
        {
            return Clone();
        }
        public TcpTypeWord Clone()
        {
            return new TcpTypeWord(_bigEndian) { Value = Value };
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