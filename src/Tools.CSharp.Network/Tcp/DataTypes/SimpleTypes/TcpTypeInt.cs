using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public sealed class TcpTypeInt : ITcpTypeValue<int>, ITcpEndianness, ITcpTypeConvertible, ITcpCloneable<TcpTypeInt>
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
            public int Value;

            [FieldOffset(0)]
            public InternalData Data;
        }
        private struct InternalData
        {
            public byte Part0;
            public byte Part1;
            public byte Part2;
            public byte Part3;
        }
        #endregion
        public TcpTypeInt()
            : this(true)
        { }
        public TcpTypeInt(bool bigEndian)
        {
            _bigEndian = bigEndian;
        }

        //---------------------------------------------------------------------
        public const int MaxValue = int.MaxValue;
        public const int MinValue = int.MinValue;
        //---------------------------------------------------------------------
        public static implicit operator int(TcpTypeInt tcpType)
        {
            return tcpType.Value;
        }
        //---------------------------------------------------------------------
        public TypeCode GetTypeCode()
        {
            return TypeCode.Int32;
        }
        //---------------------------------------------------------------------
        public int Value
        {
            get { return _value.Value; }
            set { _value.Value = value; }
        }
        public int DataLength
        {
            get { return 4; }
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
                Value = _bigEndian ? IPAddress.HostToNetworkOrder(Value) : Value
            };
            //-----------------------------------------------------------------
            dataTarget[startPositionInDataTarget] = tmp.Data.Part0;
            dataTarget[startPositionInDataTarget + 1] = tmp.Data.Part1;
            dataTarget[startPositionInDataTarget + 2] = tmp.Data.Part2;
            dataTarget[startPositionInDataTarget + 3] = tmp.Data.Part3;
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
                Value = _bigEndian ? IPAddress.HostToNetworkOrder(Value) : Value
            };
            //-----------------------------------------------------------------
            dataTarget[startPositionInDataTarget] = tmp.Data.Part0;
            dataTarget[startPositionInDataTarget + 1] = tmp.Data.Part1;
            dataTarget[startPositionInDataTarget + 2] = tmp.Data.Part2;
            dataTarget[startPositionInDataTarget + 3] = tmp.Data.Part3;
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
                    Part1 = dataSource[startPositionInDataSource + 1],
                    Part2 = dataSource[startPositionInDataSource + 2],
                    Part3 = dataSource[startPositionInDataSource + 3]
                }
            };
            //-----------------------------------------------------------------
            Value = _bigEndian ? IPAddress.NetworkToHostOrder(tmp.Value) : tmp.Value;
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
                    Part1 = dataSource[startPositionInDataSource + 1],
                    Part2 = dataSource[startPositionInDataSource + 2],
                    Part3 = dataSource[startPositionInDataSource + 3]
                }
            };
            //-----------------------------------------------------------------
            Value = _bigEndian ? IPAddress.NetworkToHostOrder(tmp.Value) : tmp.Value;
            //-----------------------------------------------------------------
            startPositionInDataSource += DataLength;
        }
        //---------------------------------------------------------------------
        object ICloneable.Clone()
        {
            return Clone();
        }
        public TcpTypeInt Clone()
        {
            return new TcpTypeInt(_bigEndian) { Value = Value };
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