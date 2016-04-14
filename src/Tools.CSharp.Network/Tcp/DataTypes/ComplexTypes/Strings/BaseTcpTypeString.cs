using System;
using System.Collections.Generic;
using System.Text;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public abstract class BaseTcpTypeString<TTcpTypeHeader, TTcpTypeString> : ITcpTypeValue<string>, ITcpEndianness
        where TTcpTypeHeader : ITcpType, new()
        where TTcpTypeString : BaseTcpTypeString<TTcpTypeHeader, TTcpTypeString>
    {
        #region private
        private static readonly byte[] _EmptyValueBytes = new byte[0];
        //---------------------------------------------------------------------
        private readonly List<byte> _valueBytes = new List<byte>();
        private readonly Encoding _encoding;
        private readonly bool _headerBigEndian;
        private readonly int _headerDataLength;
        //---------------------------------------------------------------------
        private string _value = string.Empty;
        #endregion
        #region protected
        protected abstract int GetMaxLength { get; }
        protected abstract int GetMinLength { get; }

        protected abstract TTcpTypeHeader CreateHeader(bool bigEndian, int defaultValue);
        protected abstract int GetHeaderValue(TTcpTypeHeader header);
        //---------------------------------------------------------------------
        protected abstract TTcpTypeString CreateObjectClone(bool headerBigEndian, Encoding encoding);
        //---------------------------------------------------------------------
        protected TTcpTypeString CreateClone()
        {
            var clone = CreateObjectClone(_headerBigEndian, _encoding);
            if (clone == null)
            { throw new InvalidOperationException(); }

            clone._valueBytes.AddRange(_valueBytes);
            clone._value = _value;

            return clone;
        }
        #endregion
        protected BaseTcpTypeString()
            : this(true)
        { }
        protected BaseTcpTypeString(bool headerBigEndian)
            : this(headerBigEndian, DefaultEncoding)
        { }
        protected BaseTcpTypeString(bool headerBigEndian, Encoding encoding)
        {
            if (encoding == null)
            { throw new ArgumentNullException(nameof(encoding)); }

            _headerBigEndian = headerBigEndian;
            _encoding = encoding;
            _headerDataLength = new TTcpTypeHeader().DataLength;
        }

        //---------------------------------------------------------------------
        /// <summary>
        // Кодировка по умолчанию - ASCII.
        /// </summary>
        public static Encoding DefaultEncoding = Encoding.ASCII;
        //---------------------------------------------------------------------
        public Encoding Encoding
        {
            get { return _encoding; }
        }
        public string Value
        {
            get { return _value; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (value.Length < GetMinLength || value.Length > GetMaxLength)
                    { throw new ArgumentException(nameof(value)); }
                }

                _value = value;
                _valueBytes.AddRange(string.IsNullOrEmpty(_value) ? _EmptyValueBytes : _encoding.GetBytes(_value));
            }
        }
        public int DataLength
        {
            get { return _headerDataLength + _valueBytes.Count; }
        }
        //---------------------------------------------------------------------
        public bool IsBigEndian
        {
            get { return _headerBigEndian; }
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
            var length = CreateHeader(_headerBigEndian, _valueBytes.Count);
            length.Read(dataTarget, ref startPositionInDataTarget);
            var dataLength = GetHeaderValue(length);
            //-----------------------------------------------------------------
            if (_valueBytes.Count < dataLength)
            { throw new InvalidOperationException(); }

            //-----------------------------------------------------------------
            for (var i = 0; i < dataLength; i++)
            { dataTarget[startPositionInDataTarget + i] = _valueBytes[i]; }
            //-----------------------------------------------------------------
            startPositionInDataTarget += dataLength;
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
            var length = CreateHeader(_headerBigEndian, _valueBytes.Count);
            length.Read(dataTarget, ref startPositionInDataTarget);
            var dataLength = GetHeaderValue(length);
            //-----------------------------------------------------------------
            if (_valueBytes.Count < dataLength)
            { throw new InvalidOperationException(); }

            //-----------------------------------------------------------------
            for (var i = 0; i < dataLength; i++)
            { dataTarget[startPositionInDataTarget + i] = _valueBytes[i]; }
            //-----------------------------------------------------------------
            startPositionInDataTarget += dataLength;
        }
        //---------------------------------------------------------------------
        public void Write(byte[] dataSource, ref int startPositionInDataSource)
        {
            if (dataSource == null)
            { throw new ArgumentNullException(nameof(dataSource)); }
            if (startPositionInDataSource < 0 || startPositionInDataSource > dataSource.Length)
            { throw new ArgumentOutOfRangeException(nameof(startPositionInDataSource)); }

            //-----------------------------------------------------------------
            var length = CreateHeader(_headerBigEndian, 0);
            length.Write(dataSource, ref startPositionInDataSource);
            var dataLength = GetHeaderValue(length);
            //-----------------------------------------------------------------
            if (dataSource.Length < (startPositionInDataSource + dataLength))
            { throw new ArgumentException(string.Empty, nameof(dataSource)); }

            //-----------------------------------------------------------------
            _valueBytes.Clear();
            //-----------------------------------------------------------------
            for (var i = 0; i < dataLength; i++)
            { _valueBytes.Add(dataSource[startPositionInDataSource + i]); }
            //-----------------------------------------------------------------
            _value = _encoding.GetString(_valueBytes.ToArray());
            //-----------------------------------------------------------------
            startPositionInDataSource += dataLength;
        }
        public void Write(ITcpTypeData dataSource, ref int startPositionInDataSource)
        {
            if (dataSource == null)
            { throw new ArgumentNullException(nameof(dataSource)); }
            if (startPositionInDataSource < 0 || startPositionInDataSource > dataSource.Length)
            { throw new ArgumentOutOfRangeException(nameof(startPositionInDataSource)); }

            //-----------------------------------------------------------------
            var length = CreateHeader(_headerBigEndian, 0);
            length.Write(dataSource, ref startPositionInDataSource);
            var dataLength = GetHeaderValue(length);
            //-----------------------------------------------------------------
            if (dataSource.Length < (startPositionInDataSource + dataLength))
            { throw new ArgumentException(string.Empty, nameof(dataSource)); }

            //-----------------------------------------------------------------
            _valueBytes.Clear();
            //-----------------------------------------------------------------
            for (var i = 0; i < dataLength; i++)
            { _valueBytes.Add(dataSource[startPositionInDataSource + i]); }
            //-----------------------------------------------------------------
            _value = _encoding.GetString(_valueBytes.ToArray());
            //-----------------------------------------------------------------
            startPositionInDataSource += dataLength;
        }
        //---------------------------------------------------------------------
        object ICloneable.Clone()
        {
            return CreateClone();
        }
        //---------------------------------------------------------------------
        public override string ToString()
        {
            return Value;
        }
        //---------------------------------------------------------------------
    }
}