using System;
using System.Drawing;

namespace Tools.CSharp.ProductInformation
{
    public interface IInformationBase
    {
        //---------------------------------------------------------------------
        string Name { get; }
        //---------------------------------------------------------------------
        Bitmap Logo16 { get; }
        Bitmap Logo32 { get; }
        Bitmap Logo128 { get; }
        //---------------------------------------------------------------------
        Uri Website { get; }
        //---------------------------------------------------------------------
    }
}