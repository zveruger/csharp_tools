using System;

namespace Tools.CSharp.Collections
{
    public class FastInt64Collection : BaseFastCollection<long>
    {
        #region protected
        protected override ulong CreateCount(long startValue, long endValue)
        {
            if (startValue > endValue)
            { throw new ArgumentException(string.Empty, nameof(startValue)); }

            var difference = (ulong)(endValue - startValue);
            return difference == 0 ? 0 : difference + 1;
        }
        protected override long GetValue(long startValue, ulong index)
        {
            return startValue + (long)index;
        }
        protected override bool IsValueRange(long value, long startValue, long endValue)
        {
            return value >= startValue && value <= endValue;
        }
        #endregion
        public FastInt64Collection(long startValue, long endValue)
            : base(startValue, endValue)
        { }
    }
}