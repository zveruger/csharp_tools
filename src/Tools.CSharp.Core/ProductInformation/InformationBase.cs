using System;
using System.Drawing;

namespace Tools.CSharp.ProductInformation
{
    public class InformationBase : IInformationBase
    {
        #region private

        #endregion
        public InformationBase(string name, Bitmap logo16, Bitmap logo32, Bitmap logo128, Uri website)
        {
            Name = name;
            Logo16 = logo16;
            Logo32 = logo32;
            Logo128 = logo128;
            Website = website;
        }

        //---------------------------------------------------------------------
        public string Name { get; }
        //---------------------------------------------------------------------
        public Bitmap Logo16
        {
            get;
        }
        public Bitmap Logo32
        {
            get;
        }
        public Bitmap Logo128
        {
            get;
        }
        //---------------------------------------------------------------------
        public Uri Website
        {
            get;
        }
        //---------------------------------------------------------------------
    }
}