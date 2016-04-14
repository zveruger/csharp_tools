using System;

namespace Tools.CSharp.Collections
{
    public class FastByteCollection : BaseFastCollection<byte>
    {
        #region protected
        protected override ulong CreateCount(byte startValue, byte endValue)
        {
            if (startValue > endValue)
            { throw new ArgumentException(string.Empty, nameof(startValue)); }

            var difference = (ulong)(endValue - startValue);
            return difference == 0 ? 0 : difference + 1;
        }
        protected override byte GetValue(byte startValue, ulong index)
        {
            return (byte)(startValue + index);
        }
        protected override bool IsValueRange(byte value, byte startValue, byte endValue)
        {
            return value >= startValue && value <= endValue;
        }
        #endregion
        public FastByteCollection(byte startValue, byte endValue)
            : base(startValue, endValue)
        { }
    }
}