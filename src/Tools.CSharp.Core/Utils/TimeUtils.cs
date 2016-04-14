using System;

namespace Tools.CSharp.Utils
{
    public static class TimeUtils
    {
        #region private
        private static long _RealPeriodFromStartTillNow(int start)
        {
            var now = Environment.TickCount;

            long realPeriod = now - start;
            if (realPeriod < 0)
            { realPeriod = (int.MaxValue - start) + (now - int.MinValue); }

            return realPeriod;
        }
        #endregion
        //---------------------------------------------------------------------
        public static bool IsPeriodElapsed(int start, int period)
        {
            var realPeriod = _RealPeriodFromStartTillNow(start);
            return realPeriod >= period;
        }
        //---------------------------------------------------------------------
    }
}