namespace Tools.CSharp.Extensions
{
    public static class ArrayExtensions
    {
        //---------------------------------------------------------------------
        //Copyright (c) 2008-2013 Hafthor Stefansson
        //Distributed under the MIT/X11 software license
        //Ref: http://www.opensource.org/licenses/mit-license.php.
        //
        //Сравнительное тестирование пяти способов сравнить байтовые массивы в C# 
        //(https://rsdn.ru/article/submit/comparative_test/comparative_testing.xml).
        public static unsafe bool FastEqual(this byte[] array, byte[] compareArray)
        {
            if (array == compareArray)
            { return true; }

            if (array == null || compareArray == null)
            {  return false; }

            if (array.Length != compareArray.Length)
            { return false;}

            fixed (byte* tmpPointArray = array, tmpPointCompareArray = compareArray)
            {
                var pointArray = tmpPointArray;
                var pointCompareArray = tmpPointCompareArray;
                var arrayLength = array.Length;

                for (var i = 0; i < arrayLength / 8; i++, pointArray += 8, pointCompareArray += 8)
                {
                    if (*((long*)pointArray) != *((long*)pointCompareArray))
                    { return false;}
                }

                if ((arrayLength & 4) != 0)
                {
                    if (*((int*)pointArray) != *((int*)pointCompareArray))
                    { return false; }

                    pointArray += 4;
                    pointCompareArray += 4;
                }

                if ((arrayLength & 2) != 0)
                {
                    if (*((short*)pointArray) != *((short*)pointCompareArray))
                    { return false; }

                    pointArray += 2;
                    pointCompareArray += 2;
                }

                if ((arrayLength & 1) != 0)
                {
                    if (*((byte*)pointArray) != *((byte*)pointCompareArray))
                    { return false; }
                }

                return true;
            }
        }
        //---------------------------------------------------------------------
    }
}