using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tools.CSharp.Controls.Extensions
{
    public static class StringExtensions
    {
        //---------------------------------------------------------------------
        public static SizeF GetPossibleSize(this string value, Font font)
        {
            if (string.IsNullOrEmpty(value))
            { throw new ArgumentNullException(nameof(value)); }

            return TextRenderer.MeasureText(value, font);
        }
        //---------------------------------------------------------------------
    }
}
