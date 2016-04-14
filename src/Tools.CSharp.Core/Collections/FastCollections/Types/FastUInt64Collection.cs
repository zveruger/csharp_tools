using System;

namespace Tools.CSharp.Collections
{
    public class FastUInt64Collection : BaseFastCollection<ulong>
    {
        #region protected
        protected override ulong CreateCount(ulong startValue, ulong endValue)
        {
            if (startValue > endValue)
            { throw new ArgumentException(string.Empty, nameof(startValue)); }

            var difference = endValue - startValue;
            return difference == 0 ? 0 : difference + 1;
        }
        protected override ulong GetValue(ulong startValue, ulong index)
        {
            return startValue + index;
        }
        protected override bool IsValueRange(ulong value, ulong startValue, ulong endValue)
        {
            return value >= startValue && value <= endValue;
        }
        #endregion
        public FastUInt64Collection(ulong startValue, ulong endValue)
            : base(startValue, endValue)
        { }
    }
}