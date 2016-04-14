using System;

namespace Tools.CSharp.Extensions
{
    public static class ExceptionExtensions
    {
        //---------------------------------------------------------------------
        public static TOutputException Get<TOutputException>(this Exception exception)
            where TOutputException : Exception
        {
            TOutputException outputException = null;

            var tmpException = exception;
            while (tmpException != null)
            {
                outputException = tmpException as TOutputException;
                if (outputException != null)
                { break; }

                tmpException = tmpException.InnerException;
            }

            return outputException;
        }
        //---------------------------------------------------------------------
    }
}