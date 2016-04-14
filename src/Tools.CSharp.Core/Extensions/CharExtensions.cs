using System;

namespace Tools.CSharp.Extensions
{
    public static class CharExtensions
    {
        //---------------------------------------------------------------------
        public static bool EqualsIgnoreCase(this char ch, char obj, bool ignoreCase)
        {
            if (ignoreCase)
            { return ch.ToIgnoreCase() == obj.ToIgnoreCase(); }

            return ch.Equals(obj);
        }
        //---------------------------------------------------------------------
        public static char ToIgnoreCase(this char ch)
        {
            return Char.ToLower(ch);
        }
        //---------------------------------------------------------------------
    }
}