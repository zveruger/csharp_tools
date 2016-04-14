using System;

namespace Tools.CSharp.Collections
{
    public class FastUInt16Collection : BaseFastCollection<ushort>
    {
        #region protected
        protected override ulong CreateCount(ushort startValue, ushort endValue)
        {
            if (startValue > endValue)
            { throw new ArgumentException(string.Empty, nameof(startValue)); }

            var difference = (ulong)(endValue - startValue);
            return difference == 0 ? 0 : difference + 1;
        }
        protected override ushort GetValue(ushort startValue, ulong index)
        {
            return (ushort)(startValue + index);
        }
        protected override bool IsValueRange(ushort value, ushort startValue, ushort endValue)
        {
            return value >= startValue && value <= endValue;
        }
        #endregion
        public FastUInt16Collection(ushort startValue, ushort endValue)
            : base(startValue, endValue)
        { }
    }
}