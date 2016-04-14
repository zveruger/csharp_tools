using System;
using System.Drawing;

namespace Tools.CSharp.Extensions
{
    public static class IconExtensions
    {
        //---------------------------------------------------------------------
        /// <summary>
        /// �������� ����� ������ ������, ������� ������������� ���������� ������� � 
        /// ����������� �� � GDI+ System.Drawing.Bitmap.
        /// </summary>
        public static Bitmap ToBitmap(this Icon icon, int width, int heigth)
        {
            if (icon == null)
            { throw new ArgumentNullException(nameof(icon)); }

            Bitmap bitmap;

            using (var resultIcon = new Icon(icon, width, heigth))
            {
                bitmap = resultIcon.ToBitmap();
            }

            return bitmap;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// �������� ����� ������ ������, ������� ������������� ���������� ������� � 
        /// ����������� �� � GDI+ System.Drawing.Bitmap.
        /// </summary>
        public static Bitmap ToBitmap(this Icon icon, Size size)
        {
            if (icon == null)
            { throw new ArgumentNullException(nameof(icon)); }

            Bitmap bitmap;

            using (var resultIcon = new Icon(icon, size))
            {
                bitmap = resultIcon.ToBitmap();
            }

            return bitmap;
        }
        //---------------------------------------------------------------------
    }
}