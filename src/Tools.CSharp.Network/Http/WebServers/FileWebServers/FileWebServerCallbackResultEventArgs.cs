using System;
using System.Net;

namespace Tools.CSharp.Network.Http
{
    public sealed class FileWebServerCallbackResultEventArgs : EventArgs
    {
        #region private
        private readonly Exception _exception;
        #endregion
        internal FileWebServerCallbackResultEventArgs(HttpListenerRequest request, string filePath, FileWebServerCallbackResultType resultType, Exception exception)
        {
            Request = request;
            FilePath = filePath;
            ResultType = resultType;
            _exception = exception;
        }

        //---------------------------------------------------------------------
        public HttpListenerRequest Request { get; }
        public FileWebServerCallbackResultType ResultType { get; }
        public string FilePath { get; }
        public Exception Exception
        {
            get
            {
                if(ResultType == FileWebServerCallbackResultType.Exception)
                { return _exception; }

                throw new InvalidOperationException();
            }
        }
        //---------------------------------------------------------------------
    }
}