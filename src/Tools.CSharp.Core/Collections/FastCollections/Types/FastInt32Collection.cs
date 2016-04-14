using System;

namespace Tools.CSharp.Collections
{
    public class FastInt32Collection : BaseFastCollection<int>
    {
        #region protected
        protected override ulong CreateCount(int startValue, int endValue)
        {
            if (startValue > endValue)
            { throw new ArgumentException(string.Empty, nameof(startValue)); }

            var difference = (ulong)(endValue - startValue);
            return difference == 0 ? 0 : difference + 1;
        }
        protected override int GetValue(int startValue, ulong index)
        {
            return startValue + (int)index;
        }
        protected override bool IsValueRange(int value, int startValue, int endValue)
        {
            return value >= startValue && value <= endValue;
        }
        #endregion
        public FastInt32Collection(int startValue, int endValue)
            : base(startValue, endValue)
        { }
    }
}