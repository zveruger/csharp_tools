using System;

namespace Tools.CSharp.Collections
{
    public class FastUInt32Collection : BaseFastCollection<uint>
    {
        #region protected
        protected override ulong CreateCount(uint startValue, uint endValue)
        {
            if (startValue > endValue)
            { throw new ArgumentException(string.Empty, nameof(startValue)); }

            var difference = (ulong)(endValue - startValue);
            return difference == 0 ? 0 : difference + 1;
        }
        protected override uint GetValue(uint startValue, ulong index)
        {
            return startValue + (uint)index;
        }
        protected override bool IsValueRange(uint value, uint startValue, uint endValue)
        {
            return value >= startValue && value <= endValue;
        }
        #endregion
        public FastUInt32Collection(uint startValue, uint endValue)
            : base(startValue, endValue)
        { }
    }
}