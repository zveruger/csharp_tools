using System;
using System.Text;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public sealed class TcpTypeData : ITcpTypeData, ITcpCloneable<TcpTypeData>
    {
        #region private
        private readonly byte[] _bytes;
        #endregion
        public TcpTypeData(byte[] bytes)
        {
            if (bytes == null)
            { throw new ArgumentNullException(nameof(bytes)); }

            _bytes = bytes;
        }
        public TcpTypeData(int size)
        {
            if (size < 0)
            { throw new ArgumentOutOfRangeException(nameof(size)); }

            _bytes = new byte[size];
        }

        //---------------------------------------------------------------------
        public static explicit operator TcpTypeData(byte[] bytes)
        {
            return new TcpTypeData(bytes);
        }
        //---------------------------------------------------------------------
        public byte this[int index]
        {
            get { return _bytes[index]; }
            set { _bytes[index] = value; }
        }
        public int Length
        {
            get { return _bytes.Length; }
        }
        //---------------------------------------------------------------------
        public byte[] ToArray()
        {
            var result = new byte[_bytes.Length];

            for (var i = 0; i < _bytes.Length; i++)
            { result[i] = _bytes[i]; }

            return result;
        }
        //---------------------------------------------------------------------
        public void CopyTo(byte[] dataTarget, int startPositionInDataTarger)
        {
            if (dataTarget == null)
            { throw new ArgumentNullException(nameof(dataTarget)); }
            if (startPositionInDataTarger < 0 || startPositionInDataTarger > dataTarget.Length)
            { throw new ArgumentOutOfRangeException(nameof(startPositionInDataTarger)); }
            if (dataTarget.Length < (startPositionInDataTarger + Length))
            { throw new ArgumentException(string.Empty, nameof(startPositionInDataTarger)); }

            //-----------------------------------------------------------------
            for (var i = 0; i < Length; i++)
            { dataTarget[startPositionInDataTarger + i] = _bytes[i]; }
            //-----------------------------------------------------------------
        }
        public void CopyTo(ITcpTypeData dataTarget, int startPositionInDataTarger)
        {
            if (dataTarget == null)
            { throw new ArgumentNullException(nameof(dataTarget)); }
            if (startPositionInDataTarger < 0 || startPositionInDataTarger > dataTarget.Length)
            { throw new ArgumentOutOfRangeException(nameof(startPositionInDataTarger)); }
            if (dataTarget.Length < (startPositionInDataTarger + Length))
            { throw new ArgumentException(string.Empty, nameof(startPositionInDataTarger)); }

            //-----------------------------------------------------------------
            for (var i = 0; i < Length; i++)
            { dataTarget[startPositionInDataTarger + i] = _bytes[i]; }
            //-----------------------------------------------------------------
        }
        //---------------------------------------------------------------------
        public string Dump()
        {
            var result = new StringBuilder(Length);

            foreach (var value in _bytes)
            { result.Append(value.ToString("X2")); }

            return result.ToString();
        }
        //---------------------------------------------------------------------
        object ICloneable.Clone()
        {
            return Clone();
        }
        public TcpTypeData Clone()
        {
            var clone = new TcpTypeData(_bytes.Length);
            _bytes.CopyTo(clone._bytes, 0);

            return clone;
        }
        //---------------------------------------------------------------------
        public override string ToString()
        {
            return Dump();
        }
        //---------------------------------------------------------------------
    }
}
