using System;
using System.Net.Mail;

namespace Tools.CSharp.ProductInformation
{
    public interface IProductVersionInformation : IInformationBase
    {
        //---------------------------------------------------------------------
        ICompanyInformation Company { get; }
        //---------------------------------------------------------------------
        Version Version { get; }
        //---------------------------------------------------------------------
        MailAddress MailMessageFrom { get; }
        MailAddress MailMessageTo { get; }
        //---------------------------------------------------------------------
    }
}