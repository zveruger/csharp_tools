namespace Tools.CSharp.Extensions
{
    public static class Int32Extensions
    {
        //---------------------------------------------------------------------
        public static int GetValue(this int value, int minValue, int maxValue)
        {
            if (value < minValue)
            { return minValue; }

            return value > maxValue ? maxValue : value;
        }
        //---------------------------------------------------------------------
    }
}