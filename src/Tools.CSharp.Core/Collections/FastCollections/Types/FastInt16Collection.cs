using System;

namespace Tools.CSharp.Collections
{
    public class FastInt16Collection : BaseFastCollection<short>
    {
        #region protected
        protected override ulong CreateCount(short startValue, short endValue)
        {
            if (startValue > endValue)
            { throw new ArgumentException(string.Empty, nameof(startValue)); }

            var difference = (ulong)(endValue - startValue);
            return difference == 0 ? 0 : difference + 1;
        }
        protected override short GetValue(short startValue, ulong index)
        {
            return (short)((ulong)startValue + index);
        }
        protected override bool IsValueRange(short value, short startValue, short endValue)
        {
            return value >= startValue && value <= endValue;
        }
        #endregion
        public FastInt16Collection(short startValue, short endValue)
            : base(startValue, endValue)
        { }
    }
}