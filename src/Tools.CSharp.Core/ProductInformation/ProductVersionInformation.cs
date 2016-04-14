using System;
using System.Drawing;
using System.Net.Mail;

namespace Tools.CSharp.ProductInformation
{
    public class ProductVersionInformation : InformationBase, IProductVersionInformation
    {
        public ProductVersionInformation(string productName, Bitmap logo16, Bitmap logo32, Bitmap logo128, Uri productWebsite, ICompanyInformation company, Version version, MailAddress mailMessageFrom, MailAddress mailMessageTo)
            : base(productName, logo16, logo32, logo128, productWebsite)
        {
            if (company == null)
            { throw new ArgumentNullException(nameof(company)); }
            if (version == null)
            { throw new ArgumentNullException(nameof(version)); }

            Company = company;
            Version = version;
            MailMessageFrom = mailMessageFrom;
            MailMessageTo = mailMessageTo;
        }
        
        //---------------------------------------------------------------------
        public ICompanyInformation Company
        {
            get;
        }
        //---------------------------------------------------------------------
        public Version Version
        {
            get;
        }
        //---------------------------------------------------------------------
        public MailAddress MailMessageFrom
        {
            get;
        }
        public MailAddress MailMessageTo
        {
            get;
        }
        //---------------------------------------------------------------------
    }
}