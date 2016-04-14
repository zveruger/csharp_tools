using System.Drawing;
using Tools.CSharp.Loggers;

namespace Tools.CSharp.Utils
{
    public interface IBaseApplication
    {
        //---------------------------------------------------------------------
        Bitmap GetCompanyLogoImage16 { get; }
        Bitmap GetCompanyLogoImage32 { get; }
        Bitmap GetCompanyLogoImage128 { get; }
        //---------------------------------------------------------------------
        Bitmap GetProductLogoImage16 { get; }
        Bitmap GetProductLogoImage32 { get; }
        Bitmap GetProductLogoImage128 { get; }
        //---------------------------------------------------------------------
        ILogger GetLogger { get; }
        //---------------------------------------------------------------------
    }
}