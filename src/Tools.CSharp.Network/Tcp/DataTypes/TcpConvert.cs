using System;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public static class TcpConvert
    {
        #region private
        private static readonly bool _DefaultBooleanValue = Convert.ToBoolean(null);
        private static readonly byte _DefaultByteValue = Convert.ToByte(null);
        private static readonly short _DefaultInt16Value = Convert.ToInt16(null);
        private static readonly ushort _DefaultUInt16Value = Convert.ToUInt16(null);
        private static readonly int _DefaultInt32Value = Convert.ToInt32(null);
        private static readonly uint _DefaultUInt32Value = Convert.ToUInt32(null);
        #endregion
        //---------------------------------------------------------------------
        internal static object DefaultToType(IConvertible value, Type targetType, IFormatProvider provider)
        {
            throw new InvalidCastException();
        }
        //---------------------------------------------------------------------
        public static void ToValueTcpType(ITcpTypeConvertible tcpType, bool value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            var typeCode = tcpType.GetTypeCode();
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    { ToValueTcpType((TcpTypeBool)tcpType, value); }
                    break;
                case TypeCode.Byte:
                    { ToValueTcpType((TcpTypeByte)tcpType, value); }
                    break;
                case TypeCode.UInt16:
                    { ToValueTcpType((TcpTypeWord)tcpType, value); }
                    break;
                case TypeCode.Int32:
                    { ToValueTcpType((TcpTypeInt)tcpType, value); }
                    break;
                case TypeCode.UInt32:
                    { ToValueTcpType((TcpTypeDWord)tcpType, value); }
                    break;
                default:
                    { throw new InvalidCastException(); }
            }
        }
        public static void ToValueTcpType(ITcpTypeConvertible tcpType, byte value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            var typeCode = tcpType.GetTypeCode();
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    { ToValueTcpType((TcpTypeBool)tcpType, value); }
                    break;
                case TypeCode.Byte:
                    { ToValueTcpType((TcpTypeByte)tcpType, value); }
                    break;
                case TypeCode.UInt16:
                    { ToValueTcpType((TcpTypeWord)tcpType, value); }
                    break;
                case TypeCode.Int32:
                    { ToValueTcpType((TcpTypeInt)tcpType, value); }
                    break;
                case TypeCode.UInt32:
                    { ToValueTcpType((TcpTypeDWord)tcpType, value); }
                    break;
                default:
                    { throw new InvalidCastException(); }
            }
        }
        public static void ToValueTcpType(ITcpTypeConvertible tcpType, short value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            var typeCode = tcpType.GetTypeCode();
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    { ToValueTcpType((TcpTypeBool)tcpType, value); }
                    break;
                case TypeCode.Byte:
                    { ToValueTcpType((TcpTypeByte)tcpType, value); }
                    break;
                case TypeCode.UInt16:
                    { ToValueTcpType((TcpTypeWord)tcpType, value); }
                    break;
                case TypeCode.Int32:
                    { ToValueTcpType((TcpTypeInt)tcpType, value); }
                    break;
                case TypeCode.UInt32:
                    { ToValueTcpType((TcpTypeDWord)tcpType, value); }
                    break;
                default:
                    { throw new InvalidCastException(); }
            }
        }
        public static void ToValueTcpType(ITcpTypeConvertible tcpType, ushort value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            var typeCode = tcpType.GetTypeCode();
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    { ToValueTcpType((TcpTypeBool)tcpType, value); }
                    break;
                case TypeCode.Byte:
                    { ToValueTcpType((TcpTypeByte)tcpType, value); }
                    break;
                case TypeCode.UInt16:
                    { ToValueTcpType((TcpTypeWord)tcpType, value); }
                    break;
                case TypeCode.Int32:
                    { ToValueTcpType((TcpTypeInt)tcpType, value); }
                    break;
                case TypeCode.UInt32:
                    { ToValueTcpType((TcpTypeDWord)tcpType, value); }
                    break;
                default:
                    { throw new InvalidCastException(); }
            }
        }
        public static void ToValueTcpType(ITcpTypeConvertible tcpType, int value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            var typeCode = tcpType.GetTypeCode();
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    { ToValueTcpType((TcpTypeBool)tcpType, value); }
                    break;
                case TypeCode.Byte:
                    { ToValueTcpType((TcpTypeByte)tcpType, value); }
                    break;
                case TypeCode.UInt16:
                    { ToValueTcpType((TcpTypeWord)tcpType, value); }
                    break;
                case TypeCode.Int32:
                    { ToValueTcpType((TcpTypeInt)tcpType, value); }
                    break;
                case TypeCode.UInt32:
                    { ToValueTcpType((TcpTypeDWord)tcpType, value); }
                    break;
                default:
                    { throw new InvalidCastException(); }
            }
        }
        public static void ToValueTcpType(ITcpTypeConvertible tcpType, uint value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            var typeCode = tcpType.GetTypeCode();
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    { ToValueTcpType((TcpTypeBool)tcpType, value); }
                    break;
                case TypeCode.Byte:
                    { ToValueTcpType((TcpTypeByte)tcpType, value); }
                    break;
                case TypeCode.UInt16:
                    { ToValueTcpType((TcpTypeWord)tcpType, value); }
                    break;
                case TypeCode.Int32:
                    { ToValueTcpType((TcpTypeInt)tcpType, value); }
                    break;
                case TypeCode.UInt32:
                    { ToValueTcpType((TcpTypeDWord)tcpType, value); }
                    break;
                default:
                    { throw new InvalidCastException(); }
            }
        }
        //---------------------------------------------------------------------
        public static void ToValueTcpType(TcpTypeBool tcpType, object value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToBoolean(value);
        }
        public static void ToValueTcpType(TcpTypeBool tcpType, bool value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToBoolean(value);
        }
        public static void ToValueTcpType(TcpTypeBool tcpType, byte value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToBoolean(value);
        }
        public static void ToValueTcpType(TcpTypeBool tcpType, short value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToBoolean(value);
        }
        public static void ToValueTcpType(TcpTypeBool tcpType, ushort value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToBoolean(value);
        }
        public static void ToValueTcpType(TcpTypeBool tcpType, int value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToBoolean(value);
        }
        public static void ToValueTcpType(TcpTypeBool tcpType, uint value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToBoolean(value);
        }
        //---------------------------------------------------------------------
        public static void ToValueTcpType(TcpTypeByte tcpType, object value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToByte(value);
        }
        public static void ToValueTcpType(TcpTypeByte tcpType, bool value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToByte(value);
        }
        public static void ToValueTcpType(TcpTypeByte tcpType, byte value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToByte(value);
        }
        public static void ToValueTcpType(TcpTypeByte tcpType, short value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToByte(value);
        }
        public static void ToValueTcpType(TcpTypeByte tcpType, ushort value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToByte(value);
        }
        public static void ToValueTcpType(TcpTypeByte tcpType, int value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToByte(value);
        }
        public static void ToValueTcpType(TcpTypeByte tcpType, uint value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToByte(value);
        }
        //---------------------------------------------------------------------
        public static void ToValueTcpType(TcpTypeInt tcpType, object value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToInt32(value);
        }
        public static void ToValueTcpType(TcpTypeInt tcpType, bool value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToInt32(value);
        }
        public static void ToValueTcpType(TcpTypeInt tcpType, byte value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToInt32(value);
        }
        public static void ToValueTcpType(TcpTypeInt tcpType, short value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToInt32(value);
        }
        public static void ToValueTcpType(TcpTypeInt tcpType, ushort value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToInt32(value);
        }
        public static void ToValueTcpType(TcpTypeInt tcpType, int value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToInt32(value);
        }
        public static void ToValueTcpType(TcpTypeInt tcpType, uint value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToInt32(value);
        }
        //---------------------------------------------------------------------
        public static void ToValueTcpType(TcpTypeWord tcpType, object value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToUInt16(value);
        }
        public static void ToValueTcpType(TcpTypeWord tcpType, bool value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToUInt16(value);
        }
        public static void ToValueTcpType(TcpTypeWord tcpType, byte value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToUInt16(value);
        }
        public static void ToValueTcpType(TcpTypeWord tcpType, short value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToUInt16(value);
        }
        public static void ToValueTcpType(TcpTypeWord tcpType, ushort value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToUInt16(value);
        }
        public static void ToValueTcpType(TcpTypeWord tcpType, int value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToUInt16(value);
        }
        public static void ToValueTcpType(TcpTypeWord tcpType, uint value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToUInt16(value);
        }
        //---------------------------------------------------------------------
        public static void ToValueTcpType(TcpTypeDWord tcpType, object value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToUInt32(value);
        }
        public static void ToValueTcpType(TcpTypeDWord tcpType, bool value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToUInt32(value);
        }
        public static void ToValueTcpType(TcpTypeDWord tcpType, byte value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToUInt32(value);
        }
        public static void ToValueTcpType(TcpTypeDWord tcpType, short value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToUInt32(value);
        }
        public static void ToValueTcpType(TcpTypeDWord tcpType, ushort value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToUInt32(value);
        }
        public static void ToValueTcpType(TcpTypeDWord tcpType, int value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToUInt32(value);
        }
        public static void ToValueTcpType(TcpTypeDWord tcpType, uint value)
        {
            if (tcpType == null)
            { throw new ArgumentNullException(nameof(tcpType)); }

            tcpType.Value = Convert.ToUInt32(value);
        }
        //---------------------------------------------------------------------
        public static TcpTypeBool ToTcpTypeBool(object value)
        {
            return new TcpTypeBool { Value = Convert.ToBoolean(value) };
        }
        public static TcpTypeBool ToTcpTypeBool(bool value)
        {
            return new TcpTypeBool { Value = Convert.ToBoolean(value) };
        }
        public static TcpTypeBool ToTcpTypeBool(byte value)
        {
            return new TcpTypeBool { Value = Convert.ToBoolean(value) };
        }
        public static TcpTypeBool ToTcpTypeBool(short value)
        {
            return new TcpTypeBool { Value = Convert.ToBoolean(value) };
        }
        public static TcpTypeBool ToTcpTypeBool(ushort value)
        {
            return new TcpTypeBool { Value = Convert.ToBoolean(value) };
        }
        public static TcpTypeBool ToTcpTypeBool(int value)
        {
            return new TcpTypeBool { Value = Convert.ToBoolean(value) };
        }
        public static TcpTypeBool ToTcpTypeBool(uint value)
        {
            return new TcpTypeBool { Value = Convert.ToBoolean(value) };
        }
        //---------------------------------------------------------------------
        public static TcpTypeByte ToTcpTypeByte(object value)
        {
            return new TcpTypeByte { Value = Convert.ToByte(value) };
        }
        public static TcpTypeByte ToTcpTypeByte(bool value)
        {
            return new TcpTypeByte { Value = Convert.ToByte(value) };
        }
        public static TcpTypeByte ToTcpTypeByte(byte value)
        {
            return new TcpTypeByte { Value = Convert.ToByte(value) };
        }
        public static TcpTypeByte ToTcpTypeByte(short value)
        {
            return new TcpTypeByte { Value = Convert.ToByte(value) };
        }
        public static TcpTypeByte ToTcpTypeByte(ushort value)
        {
            return new TcpTypeByte { Value = Convert.ToByte(value) };
        }
        public static TcpTypeByte ToTcpTypeByte(int value)
        {
            return new TcpTypeByte { Value = Convert.ToByte(value) };
        }
        public static TcpTypeByte ToTcpTypeByte(uint value)
        {
            return new TcpTypeByte { Value = Convert.ToByte(value) };
        }
        //---------------------------------------------------------------------
        public static TcpTypeInt ToTcpTypeInt(bool bigEndian, object value)
        {
            return new TcpTypeInt(bigEndian) { Value = Convert.ToInt32(value) };
        }
        public static TcpTypeInt ToTcpTypeInt(bool bigEndian, bool value)
        {
            return new TcpTypeInt(bigEndian) { Value = Convert.ToInt32(value) };
        }
        public static TcpTypeInt ToTcpTypeInt(bool bigEndian, byte value)
        {
            return new TcpTypeInt(bigEndian) { Value = Convert.ToInt32(value) };
        }
        public static TcpTypeInt ToTcpTypeInt(bool bigEndian, short value)
        {
            return new TcpTypeInt(bigEndian) { Value = Convert.ToInt32(value) };
        }
        public static TcpTypeInt ToTcpTypeInt(bool bigEndian, ushort value)
        {
            return new TcpTypeInt(bigEndian) { Value = Convert.ToInt32(value) };
        }
        public static TcpTypeInt ToTcpTypeInt(bool bigEndian, int value)
        {
            return new TcpTypeInt(bigEndian) { Value = Convert.ToInt32(value) };
        }
        public static TcpTypeInt ToTcpTypeInt(bool bigEndian, uint value)
        {
            return new TcpTypeInt(bigEndian) { Value = Convert.ToInt32(value) };
        }
        //---------------------------------------------------------------------
        public static TcpTypeWord ToTcpTypeWord(bool bigEndian, object value)
        {
            return new TcpTypeWord(bigEndian) { Value = Convert.ToUInt16(value) };
        }
        public static TcpTypeWord ToTcpTypeWord(bool bigEndian, bool value)
        {
            return new TcpTypeWord(bigEndian) { Value = Convert.ToUInt16(value) };
        }
        public static TcpTypeWord ToTcpTypeWord(bool bigEndian, byte value)
        {
            return new TcpTypeWord(bigEndian) { Value = Convert.ToUInt16(value) };
        }
        public static TcpTypeWord ToTcpTypeWord(bool bigEndian, short value)
        {
            return new TcpTypeWord(bigEndian) { Value = Convert.ToUInt16(value) };
        }
        public static TcpTypeWord ToTcpTypeWord(bool bigEndian, ushort value)
        {
            return new TcpTypeWord(bigEndian) { Value = Convert.ToUInt16(value) };
        }
        public static TcpTypeWord ToTcpTypeWord(bool bigEndian, int value)
        {
            return new TcpTypeWord(bigEndian) { Value = Convert.ToUInt16(value) };
        }
        public static TcpTypeWord ToTcpTypeWord(bool bigEndian, uint value)
        {
            return new TcpTypeWord(bigEndian) { Value = Convert.ToUInt16(value) };
        }
        //---------------------------------------------------------------------
        public static TcpTypeDWord ToTcpTypeDWord(bool bigEndian, object value)
        {
            return new TcpTypeDWord(bigEndian) { Value = Convert.ToUInt32(value) };
        }
        public static TcpTypeDWord ToTcpTypeDWord(bool bigEndian, bool value)
        {
            return new TcpTypeDWord(bigEndian) { Value = Convert.ToUInt32(value) };
        }
        public static TcpTypeDWord ToTcpTypeDWord(bool bigEndian, byte value)
        {
            return new TcpTypeDWord(bigEndian) { Value = Convert.ToUInt32(value) };
        }
        public static TcpTypeDWord ToTcpTypeDWord(bool bigEndian, short value)
        {
            return new TcpTypeDWord(bigEndian) { Value = Convert.ToUInt32(value) };
        }
        public static TcpTypeDWord ToTcpTypeDWord(bool bigEndian, ushort value)
        {
            return new TcpTypeDWord(bigEndian) { Value = Convert.ToUInt32(value) };
        }
        public static TcpTypeDWord ToTcpTypeDWord(bool bigEndian, int value)
        {
            return new TcpTypeDWord(bigEndian) { Value = Convert.ToUInt32(value) };
        }
        public static TcpTypeDWord ToTcpTypeDWord(bool bigEndian, uint value)
        {
            return new TcpTypeDWord(bigEndian) { Value = Convert.ToUInt32(value) };
        }
        //---------------------------------------------------------------------
        public static bool ToBoolean(object value)
        {
            return value == null ? _DefaultBooleanValue : ((IConvertible)value).ToBoolean(null);
        }
        public static bool ToBoolean(TcpTypeBool value)
        {
            return value == null ? _DefaultBooleanValue : Convert.ToBoolean(value.Value);
        }
        public static bool ToBoolean(TcpTypeByte value)
        {
            return value == null ? _DefaultBooleanValue : Convert.ToBoolean(value.Value);
        }
        public static bool ToBoolean(TcpTypeInt value)
        {
            return value == null ? _DefaultBooleanValue : Convert.ToBoolean(value.Value);
        }
        public static bool ToBoolean(TcpTypeWord value)
        {
            return value == null ? _DefaultBooleanValue : Convert.ToBoolean(value.Value);
        }
        public static bool ToBoolean(TcpTypeDWord value)
        {
            return value == null ? _DefaultBooleanValue : Convert.ToBoolean(value.Value);
        }
        //---------------------------------------------------------------------
        public static byte ToByte(object value)
        {
            return value == null ? _DefaultByteValue : ((IConvertible)value).ToByte(null);
        }
        public static byte ToByte(TcpTypeBool value)
        {
            return value == null ? _DefaultByteValue : Convert.ToByte(value.Value);
        }
        public static byte ToByte(TcpTypeByte value)
        {
            return value == null ? _DefaultByteValue : Convert.ToByte(value.Value);
        }
        public static byte ToByte(TcpTypeInt value)
        {
            return value == null ? _DefaultByteValue : Convert.ToByte(value.Value);
        }
        public static byte ToByte(TcpTypeWord value)
        {
            return value == null ? _DefaultByteValue : Convert.ToByte(value.Value);
        }
        public static byte ToByte(TcpTypeDWord value)
        {
            return value == null ? _DefaultByteValue : Convert.ToByte(value.Value);
        }
        //---------------------------------------------------------------------
        public static short ToInt16(object value)
        {
            return value == null ? _DefaultInt16Value : ((IConvertible)value).ToInt16(null);
        }
        public static short ToInt16(TcpTypeBool value)
        {
            return value == null ? _DefaultInt16Value : Convert.ToInt16(value.Value);
        }
        public static short ToInt16(TcpTypeByte value)
        {
            return value == null ? _DefaultInt16Value : Convert.ToInt16(value.Value);
        }
        public static short ToInt16(TcpTypeInt value)
        {
            return value == null ? _DefaultInt16Value : Convert.ToInt16(value.Value);
        }
        public static short ToInt16(TcpTypeWord value)
        {
            return value == null ? _DefaultInt16Value : Convert.ToInt16(value.Value);
        }
        public static short ToInt16(TcpTypeDWord value)
        {
            return value == null ? _DefaultInt16Value : Convert.ToInt16(value.Value);
        }
        //---------------------------------------------------------------------
        public static ushort ToUInt16(object value)
        {
            return value == null ? _DefaultUInt16Value : ((IConvertible)value).ToUInt16(null);
        }
        public static ushort ToUInt16(TcpTypeBool value)
        {
            return value == null ? _DefaultUInt16Value : Convert.ToUInt16(value.Value);
        }
        public static ushort ToUInt16(TcpTypeByte value)
        {
            return value == null ? _DefaultUInt16Value : Convert.ToUInt16(value.Value);
        }
        public static ushort ToUInt16(TcpTypeInt value)
        {
            return value == null ? _DefaultUInt16Value : Convert.ToUInt16(value.Value);
        }
        public static ushort ToUInt16(TcpTypeWord value)
        {
            return value == null ? _DefaultUInt16Value : Convert.ToUInt16(value.Value);
        }
        public static ushort ToUInt16(TcpTypeDWord value)
        {
            return value == null ? _DefaultUInt16Value : Convert.ToUInt16(value.Value);
        }
        //---------------------------------------------------------------------
        public static int ToInt32(object value)
        {
            return value == null ? _DefaultInt32Value : ((IConvertible)value).ToInt32(null);
        }
        public static int ToInt32(TcpTypeBool value)
        {
            return value == null ? _DefaultInt32Value : Convert.ToInt32(value.Value);
        }
        public static int ToInt32(TcpTypeByte value)
        {
            return value == null ? _DefaultInt32Value : Convert.ToInt32(value.Value);
        }
        public static int ToInt32(TcpTypeInt value)
        {
            return value == null ? _DefaultInt32Value : Convert.ToInt32(value.Value);
        }
        public static int ToInt32(TcpTypeWord value)
        {
            return value == null ? _DefaultInt32Value : Convert.ToInt32(value.Value);
        }
        public static int ToInt32(TcpTypeDWord value)
        {
            return value == null ? _DefaultInt32Value : Convert.ToInt32(value.Value);
        }
        //---------------------------------------------------------------------
        public static uint ToUInt32(object value)
        {
            return value == null ? _DefaultUInt32Value : ((IConvertible)value).ToUInt32(null);
        }
        public static uint ToUInt32(TcpTypeBool value)
        {
            return value == null ? _DefaultUInt32Value : Convert.ToUInt32(value.Value);
        }
        public static uint ToUInt32(TcpTypeByte value)
        {
            return value == null ? _DefaultUInt32Value : Convert.ToUInt32(value.Value);
        }
        public static uint ToUInt32(TcpTypeInt value)
        {
            return value == null ? _DefaultUInt32Value : Convert.ToUInt32(value.Value);
        }
        public static uint ToUInt32(TcpTypeWord value)
        {
            return value == null ? _DefaultUInt32Value : Convert.ToUInt32(value.Value);
        }
        public static uint ToUInt32(TcpTypeDWord value)
        {
            return value == null ? _DefaultUInt32Value : Convert.ToUInt32(value.Value);
        }
        //---------------------------------------------------------------------
    }
}