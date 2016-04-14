using System.Drawing;

namespace Tools.CSharp.Controls
{
    public interface ILoadingCircle
    {
        //---------------------------------------------------------------------
        int OuterCircleRadius { get; set; }
        int InnerCircleRadius { get; set; }
        //---------------------------------------------------------------------
        int NumberSpoke { get; set; }
        int SpokeThickness { get; set; }
        Color SpokeColor { get; set; }
        //---------------------------------------------------------------------
        int RotationSpeed { get; set; }
        //---------------------------------------------------------------------
        bool Active { get; set; }
        //---------------------------------------------------------------------
    }
}