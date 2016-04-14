using System;
using System.Text;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public abstract class BaseTcpTypeNullable<TTcpType, TTcpTypeNullable> : ITcpTypeValue<TTcpType>, ITcpEndianness
        where TTcpType : ITcpType
        where TTcpTypeNullable : BaseTcpTypeNullable<TTcpType, TTcpTypeNullable>
    {
        #region private
        private readonly TcpTypeBool _hasValue = new TcpTypeBool();
        //---------------------------------------------------------------------
        private readonly bool _bigEndian;
        private readonly Encoding _encoding;
        //---------------------------------------------------------------------
        private TTcpType _value;
        #endregion
        #region protected
        protected abstract TTcpType CreateValue(bool bigEndian, Encoding encoding);
        //---------------------------------------------------------------------
        protected abstract TTcpTypeNullable CreateObjectClone(bool bigEndian, Encoding encoding);
        //---------------------------------------------------------------------
        protected TTcpTypeNullable CreateClone()
        {
            var clone = CreateObjectClone(_bigEndian, _encoding);
            if (clone == null)
            { throw new InvalidOperationException(); }

            clone._hasValue.Value = _hasValue.Value;
            clone._value = (TTcpType)_value.Clone();

            return clone;
        }
        #endregion
        protected BaseTcpTypeNullable()
            : this(true, null)
        { }
        protected BaseTcpTypeNullable(bool bigEndian)
            : this(bigEndian, TcpTypeByteString.DefaultEncoding)
        { }
        protected BaseTcpTypeNullable(bool bigEndian, Encoding encoding)
        {
            if (typeof(TTcpType) == typeof(ITcpTypeValue<string>))
            {
                if (encoding == null)
                { throw new ArgumentNullException(nameof(encoding)); }
            }

            _bigEndian = bigEndian;
            _encoding = encoding;
            _hasValue.Value = false;
            _value = CreateValue(_bigEndian, _encoding);
        }

        //---------------------------------------------------------------------
        public bool HasValue
        {
            get { return _hasValue.Value; }
        }
        public TTcpType Value
        {
            get
            {
                if (HasValue)
                { return _value; }

                throw new InvalidOperationException();
            }
            set
            { throw new InvalidOperationException(); }
        }
        public int DataLength
        {
            get
            {
                var dataLength = _hasValue.DataLength;
                
                if (HasValue)
                { dataLength += _value.DataLength; }

                return dataLength;
            }
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
            _hasValue.Read(dataTarget, ref startPositionInDataTarget);

            if (_hasValue.Value)
            { _value.Read(dataTarget, ref startPositionInDataTarget); }
            //-----------------------------------------------------------------
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
            _hasValue.Read(dataTarget, ref startPositionInDataTarget);

            if (_hasValue.Value)
            { _value.Read(dataTarget, ref startPositionInDataTarget); }
            //-----------------------------------------------------------------
        }
        //---------------------------------------------------------------------
        public void Write(byte[] dataSource, ref int startPositionInDataSource)
        {
            if (dataSource == null)
            { throw new ArgumentNullException(nameof(dataSource)); }
            if (startPositionInDataSource < 0 || startPositionInDataSource > dataSource.Length)
            { throw new ArgumentOutOfRangeException(nameof(startPositionInDataSource)); }

            //-----------------------------------------------------------------
            _hasValue.Write(dataSource, ref startPositionInDataSource);

            if (HasValue)
            { _value.Write(dataSource, ref startPositionInDataSource); }
            //-----------------------------------------------------------------
        }
        public void Write(ITcpTypeData dataSource, ref int startPositionInDataSource)
        {
            if (dataSource == null)
            { throw new ArgumentNullException(nameof(dataSource)); }
            if (startPositionInDataSource < 0 || startPositionInDataSource > dataSource.Length)
            { throw new ArgumentOutOfRangeException(nameof(startPositionInDataSource)); }

            //-----------------------------------------------------------------
            _hasValue.Write(dataSource, ref startPositionInDataSource);

            if (HasValue)
            { _value.Write(dataSource, ref startPositionInDataSource); }
            //-----------------------------------------------------------------
        }
        //---------------------------------------------------------------------
        object ICloneable.Clone()
        {
            return CreateClone();
        }
        //---------------------------------------------------------------------
        public override string ToString()
        {
            return HasValue ? _value.ToString() : "";
        }
        //---------------------------------------------------------------------
    }
}