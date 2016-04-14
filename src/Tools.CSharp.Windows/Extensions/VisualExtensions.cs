using System;
using System.Windows.Media;

namespace Tools.CSharp.Extensions
{
    public static class VisualExtensions
    {
        //---------------------------------------------------------------------
        public static TOutput GetVisualChild<TOutput>(this Visual parent) where TOutput : Visual{
            if (parent == null)
            { throw new ArgumentNullException(nameof(parent)); }

            var child = default(TOutput);

            var visualChildrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for(var childIndex = 0; childIndex < visualChildrenCount; childIndex++)
            {
                var visualChildren = VisualTreeHelper.GetChild(parent, childIndex) as Visual;
                if (visualChildren != null)
                {
                    child = visualChildren as TOutput ?? GetVisualChild<TOutput>(visualChildren);

                    if (child != null)
                    { break; }
                }
            }
            return child;
        }
        //---------------------------------------------------------------------
    }
}
