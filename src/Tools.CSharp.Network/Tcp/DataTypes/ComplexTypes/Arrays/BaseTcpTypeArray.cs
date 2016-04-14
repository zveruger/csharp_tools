using System;
using System.Text;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public abstract class BaseTcpTypeArray<TCount, TItem, TTcpTypeArray> : BaseTcpTypeComposite<TTcpTypeArray>, ITcpEndianness
        where TCount : ITcpType, ITcpTypeConvertible, new()
        where TItem : ITcpType
        where TTcpTypeArray : BaseTcpTypeArray<TCount, TItem, TTcpTypeArray>
    {
        #region private
        private readonly int _headerDataLength;
        //---------------------------------------------------------------------
        private readonly bool _bigEndian;
        private readonly Encoding _encoding;
        #endregion
        #region protected
        protected abstract TCount CreateCount(bool bigEndian);
        protected abstract TItem CreateItem(bool bigEndian, Encoding encoding);
        #endregion
        protected BaseTcpTypeArray()
            : this(true, null)
        { }
        protected BaseTcpTypeArray(bool bigEndian)
            : this(bigEndian, TcpTypeByteString.DefaultEncoding)
        { }
        protected BaseTcpTypeArray(bool bigEndian, Encoding encoding)
        {
            if (typeof (TItem) == typeof (ITcpTypeValue<string>))
            {
                if (encoding == null)
                { throw new ArgumentNullException(nameof(encoding)); }
            }

            _bigEndian = bigEndian;
            _encoding = encoding;
            _headerDataLength = new TCount().DataLength;
        }

        //---------------------------------------------------------------------
        public TItem this[int index]
        {
            get { return (TItem)GetItem(index); }
        }
        public int Length
        {
            get { return GetCount; }
        }
        //---------------------------------------------------------------------
        public override int DataLength
        {
            get { return _headerDataLength + base.DataLength; }
        }
        //---------------------------------------------------------------------
        public bool IsBigEndian
        {
            get { return _bigEndian; }
        }
        //---------------------------------------------------------------------
        public void Add(TItem value)
        {
            if (value == null)
            { throw new ArgumentNullException(nameof(value)); }

            AddChild(value);
        }
        //---------------------------------------------------------------------
        public override void Read(byte[] dataTarget, ref int startPositionInDataTarget)
        {
            if (dataTarget == null)
            { throw new ArgumentNullException(nameof(dataTarget)); }
            if (startPositionInDataTarget < 0 || startPositionInDataTarget > dataTarget.Length)
            { throw new ArgumentOutOfRangeException(nameof(startPositionInDataTarget)); }
            if (dataTarget.Length < (startPositionInDataTarget + DataLength))
            { throw new ArgumentException(string.Empty, nameof(startPositionInDataTarget)); }

            //-----------------------------------------------------------------
            var count = CreateCount(_bigEndian);
            TcpConvert.ToValueTcpType(count, GetCount);
            count.Read(dataTarget, ref startPositionInDataTarget);
            //-----------------------------------------------------------------
            base.Read(dataTarget, ref startPositionInDataTarget);
        }
        public override void Read(ITcpTypeData dataTarget, ref int startPositionInDataTarget)
        {
            if (dataTarget == null)
            { throw new ArgumentNullException(nameof(dataTarget)); }
            if (startPositionInDataTarget < 0 || startPositionInDataTarget > dataTarget.Length)
            { throw new ArgumentOutOfRangeException(nameof(startPositionInDataTarget)); }
            if (dataTarget.Length < (startPositionInDataTarget + DataLength))
            { throw new ArgumentException(string.Empty, nameof(startPositionInDataTarget)); }

            //-----------------------------------------------------------------
            var count = CreateCount(_bigEndian);
            TcpConvert.ToValueTcpType(count, GetCount);
            count.Read(dataTarget, ref startPositionInDataTarget);
            //-----------------------------------------------------------------
            base.Read(dataTarget, ref startPositionInDataTarget);
        }
        //---------------------------------------------------------------------
        public override void Write(byte[] dataSource, ref int startPositionInDataSource)
        {
            if (dataSource == null)
            { throw new ArgumentNullException(nameof(dataSource)); }
            if (startPositionInDataSource < 0 || startPositionInDataSource > dataSource.Length)
            { throw new ArgumentOutOfRangeException(nameof(startPositionInDataSource)); }

            //-----------------------------------------------------------------
            var count = CreateCount(_bigEndian);
            count.Write(dataSource, ref startPositionInDataSource);
            var dataLength = TcpConvert.ToInt32(count);
            //-----------------------------------------------------------------
            if (dataSource.Length < (startPositionInDataSource + dataLength))
            { throw new ArgumentException(string.Empty, nameof(dataSource)); }

            //-----------------------------------------------------------------
            ClearChilds();
            //-----------------------------------------------------------------
            for (var i = 0; i < dataLength; i++)
            {
                var item = CreateItem(_bigEndian, _encoding);
                item.Write(dataSource, ref startPositionInDataSource);

                AddChild(item);
            }
            //-----------------------------------------------------------------
        }
        public override void Write(ITcpTypeData dataSource, ref int startPositionInDataSource)
        {
            if (dataSource == null)
            { throw new ArgumentNullException(nameof(dataSource)); }
            if (startPositionInDataSource < 0 || startPositionInDataSource > dataSource.Length)
            { throw new ArgumentOutOfRangeException(nameof(startPositionInDataSource)); }

            //-----------------------------------------------------------------
            var count = CreateCount(_bigEndian);
            count.Write(dataSource, ref startPositionInDataSource);
            var dataLength = TcpConvert.ToInt32(count);
            //-----------------------------------------------------------------
            if (dataSource.Length < (startPositionInDataSource + dataLength))
            { throw new ArgumentException(string.Empty, nameof(dataSource)); }

            //-----------------------------------------------------------------
            ClearChilds();
            //-----------------------------------------------------------------
            for (var i = 0; i < dataLength; i++)
            {
                var item = CreateItem(_bigEndian, _encoding);
                item.Write(dataSource, ref startPositionInDataSource);

                AddChild(item);
            }
            //-----------------------------------------------------------------
        }
        //---------------------------------------------------------------------
        public override void ContinueWrite(ITcpTypeData dataSource, ref int startPositionInDataSource)
        {
            Write(dataSource, ref startPositionInDataSource);
        }
        //---------------------------------------------------------------------
    }
}