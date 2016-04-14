using System.Drawing.Drawing2D;

namespace Tools.CSharp.Controls.Extensions
{
    public static class MatrixExtension
    {
        //---------------------------------------------------------------------
        public static void BeginTranslate(this Matrix matrix, float offsetX, float offsetY, GraphicsPath graphicsPath)
        {
            matrix.Translate(offsetX, offsetY);
            graphicsPath.Transform(matrix);
        }
        public static void EndTranslate(this Matrix matrix, GraphicsPath graphicsPath)
        {
            matrix.Invert();
            graphicsPath.Transform(matrix);
        }
        //---------------------------------------------------------------------
    }
}