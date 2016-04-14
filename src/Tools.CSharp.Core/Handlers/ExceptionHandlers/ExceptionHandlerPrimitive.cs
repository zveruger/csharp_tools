using System;
using System.ComponentModel;

namespace Tools.CSharp.Handlers
{
    public sealed class ExceptionHandlerPrimitive
    {
        #region private
        private readonly string _title;
        private readonly string _message;
        private readonly ExceptionHandlerLevel _handlerLevel;
        private readonly bool _defaultHandler;
        private readonly Exception _exception;
        #endregion
        public ExceptionHandlerPrimitive(Exception exception, string title, string message, ExceptionHandlerLevel handlerLevel = ExceptionHandlerLevel.Error)
            : this(exception, title, message, handlerLevel, false)
        { }
        internal ExceptionHandlerPrimitive(Exception exception, string title, string message, ExceptionHandlerLevel handlerLevel, bool defaultHandler)
        {
            if (exception == null)
            { throw new ArgumentNullException(nameof(exception)); }
            if (handlerLevel < ExceptionHandlerLevel.Error || handlerLevel > ExceptionHandlerLevel.Warning)
            { throw new InvalidEnumArgumentException(nameof(handlerLevel), (int)handlerLevel, typeof(ExceptionHandlerLevel)); }

            _exception = exception;
            _handlerLevel = handlerLevel;
            _title = title;
            _message = message;
            _defaultHandler = defaultHandler;
        }

        //---------------------------------------------------------------------
        public string Title
        {
            get { return _title; }
        }
        public string Message
        {
            get { return _message; }
        }
        public ExceptionHandlerLevel HandlerLevel
        {
            get { return _handlerLevel; }
        }
        //---------------------------------------------------------------------
        public bool DefaultHandler
        {
            get { return _defaultHandler; }
        }
        //---------------------------------------------------------------------
        public Exception Exception
        {
            get { return _exception; }
        }
        //---------------------------------------------------------------------
    }
}