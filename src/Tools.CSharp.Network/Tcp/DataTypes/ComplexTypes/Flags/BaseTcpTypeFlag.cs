using System;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public abstract class BaseTcpTypeFlag<TTcpTypeValue, TValue, TTcpTypeFlag> : ITcpTypeValue<TValue>, ITcpEndianness
        where TTcpTypeValue : ITcpTypeValue<TValue>
        where TValue : struct
        where TTcpTypeFlag : BaseTcpTypeFlag<TTcpTypeValue, TValue, TTcpTypeFlag>
    {
        #region private
        private readonly TTcpTypeValue _content;
        private readonly bool _bigEndian;
        #endregion
        #region protected
        protected TValue Value
        {
            get { return _content.Value; }
            set { _content.Value = value; }
        }
        //---------------------------------------------------------------------
        protected abstract TTcpTypeValue CreateFlagContent(bool bigEndian);
        //---------------------------------------------------------------------
        protected abstract TTcpTypeFlag CreateObjectClone(bool bigEndian);
        //---------------------------------------------------------------------
        protected TTcpTypeFlag CreateClone()
        {
            var clone = CreateObjectClone(_bigEndian);
            if (clone == null)
            { throw new InvalidOperationException(); }

            clone.Value = Value;

            return clone;
        }
        #endregion
        protected BaseTcpTypeFlag()
            : this(true)
        { }
        protected BaseTcpTypeFlag(bool bigEndian)
        {
            _bigEndian = bigEndian;
            _content = CreateFlagContent(_bigEndian);
        }

        //---------------------------------------------------------------------
        TValue ITcpTypeValue<TValue>.Value
        {
            get { return Value; }
            set { Value = value; }
        }
        public int DataLength
        {
            get { return _content.DataLength; }
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
            _content.Read(dataTarget, ref startPositionInDataTarget);
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
            _content.Read(dataTarget, ref startPositionInDataTarget);
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
            _content.Write(dataSource, ref startPositionInDataSource);
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
            _content.Write(dataSource, ref startPositionInDataSource);
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