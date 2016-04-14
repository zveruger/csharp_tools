using System;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public abstract class BaseTcpTypeEnum<TTcpTypeValue, TValue, TEnum, TTcpTypeEnum> : ITcpTypeValue<TEnum>, ITcpEndianness
        where TTcpTypeValue : ITcpTypeValue<TValue>
        where TEnum : struct
        where TTcpTypeEnum : BaseTcpTypeEnum<TTcpTypeValue, TValue, TEnum, TTcpTypeEnum>
    {
        #region private
        private readonly TTcpTypeValue _enumContent;
        private readonly bool _bigEndian;
        //---------------------------------------------------------------------
        private TEnum _value;
        //---------------------------------------------------------------------
        private void _updateValue()
        {
            _value = (TEnum)Enum.ToObject(typeof(TEnum), _enumContent.Value);
        }
        #endregion
        #region protected
        protected abstract TTcpTypeValue CreateEnumContent(bool bigEndian);
        protected abstract void UpdateEnumContentValue(TTcpTypeValue enumContent, TEnum value);
        //---------------------------------------------------------------------
        protected abstract TTcpTypeEnum CreateObjectClone(bool bigEndian);
        //---------------------------------------------------------------------
        protected TTcpTypeEnum CreateClone()
        {
            var clone = CreateObjectClone(_bigEndian);
            if (clone == null)
            { throw new InvalidOperationException(); }

            clone.UpdateEnumContentValue(clone._enumContent, _value);
            clone._value = _value;

            return clone;
        }
        #endregion
        protected BaseTcpTypeEnum()
            : this(true)
        { }
        protected BaseTcpTypeEnum(bool bigEndian)
        {
            _bigEndian = bigEndian;
            _enumContent = CreateEnumContent(_bigEndian);

            Value = default(TEnum);
        }

        //---------------------------------------------------------------------
        public TEnum Value
        {
            get { return _value; }
            set
            {
                UpdateEnumContentValue(_enumContent, value);
                _updateValue();
            }
        }
        public int DataLength
        {
            get { return _enumContent.DataLength; }
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
            _enumContent.Read(dataTarget, ref startPositionInDataTarget);
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
            _enumContent.Read(dataTarget, ref startPositionInDataTarget);
            //-----------------------------------------------------------------
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
            _enumContent.Write(dataSource, ref startPositionInDataSource);
            _updateValue();
            //-----------------------------------------------------------------
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
            _enumContent.Write(dataSource, ref startPositionInDataSource);
            _updateValue();
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
            return Value.ToString();
        }
        //---------------------------------------------------------------------
    }
}