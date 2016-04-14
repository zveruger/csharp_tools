using System;
using System.Drawing;

namespace Tools.CSharp.ProductInformation
{
    public class CompanyInformation : InformationBase, ICompanyInformation
    {
        public CompanyInformation(string companyName, Bitmap logo16, Bitmap logo32, Bitmap logo128, Uri companyWebsite, string copyright)
            : base(companyName, logo16, logo32, logo128, companyWebsite)
        {
            Copyright = copyright;
        }
       
        //---------------------------------------------------------------------
        public string Copyright
        {
            get;
        }
        //---------------------------------------------------------------------
    }
}