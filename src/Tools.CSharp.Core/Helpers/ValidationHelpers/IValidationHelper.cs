using System;

namespace Tools.CSharp.Helpers
{
    //-------------------------------------------------------------------------
    public interface IValidationHelper
    {
        //---------------------------------------------------------------------
        IValidationHelper<TError> CreateChildHelper<TError>() where TError : struct;
        //---------------------------------------------------------------------
        string CreateMessageInvalidity(int errorValue, object tag);
        string CreateMessageInvalidity(Enum errorEnumValue, object tag);
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    public interface IValidationHelper<in TError> : IValidationHelper
        where TError : struct
    {
        //---------------------------------------------------------------------
        string CreateMessageInvalidity(TError error, object tag);
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
}