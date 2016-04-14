using System.Drawing;
using System.Drawing.Drawing2D;

namespace Tools.CSharp.Controls.Extensions
{
    public static class GraphicsExtension
    {
        //---------------------------------------------------------------------
        public static void FillPathByTransform(this Graphics graphics, Brush brush, GraphicsPath graphicsPath, float offsetX, float offsetY)
        {
            using (var matrix = new Matrix())
            {
                matrix.BeginTranslate(offsetX, offsetY, graphicsPath);
                graphics.FillPath(brush, graphicsPath);
                matrix.EndTranslate(graphicsPath);
            }
        }
        public static void FillPathRegionByTransform(this Graphics graphics, Brush brush, GraphicsPath graphicsPath, float offsetX, float offsetY)
        {
            using (var matrix = new Matrix())
            {
                matrix.BeginTranslate(offsetX, offsetY, graphicsPath);
                using (var region = new Region(graphicsPath))
                {
                    var oldRegion = graphics.Clip;
                    graphics.Clip = region;
                    graphics.FillPath(brush, graphicsPath);
                    graphics.Clip = oldRegion;
                }
                matrix.EndTranslate(graphicsPath);
            }
        }

        public static void DrawPathByTransform(this Graphics graphics, Pen pen, GraphicsPath graphicsPath, float offsetX, float offsetY)
        {
            using (var matrix = new Matrix())
            {
                matrix.BeginTranslate(offsetX, offsetY, graphicsPath);
                graphics.DrawPath(pen, graphicsPath);
                matrix.EndTranslate(graphicsPath);
            }
        }
        //---------------------------------------------------------------------
    }
}