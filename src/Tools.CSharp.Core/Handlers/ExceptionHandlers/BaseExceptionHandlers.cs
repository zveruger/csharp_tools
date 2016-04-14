using System;
using System.Collections.Generic;
using Tools.CSharp.Extensions;
using Tools.CSharp.Loggers;

namespace Tools.CSharp.Handlers
{
    public abstract class BaseExceptionHandlers
    {
        #region private
        private readonly Dictionary<Type, Func<Exception, ExceptionHandlerPrimitive>> _actionExceptionHandlers = new Dictionary<Type, Func<Exception, ExceptionHandlerPrimitive>>();
        //---------------------------------------------------------------------
        private readonly ILogger _logger;
        #endregion
        #region protected
        protected ILogger Logger
        {
            get { return _logger; }
        }
        //---------------------------------------------------------------------
        protected void AddActionExceptionHandler<TInputException>(Func<Exception, ExceptionHandlerPrimitive> handler)
            where TInputException : Exception
        {
            if (handler == null)
            { throw new ArgumentNullException(nameof(handler)); }

            _actionExceptionHandlers.Add(typeof(TInputException), handler);
        }
        //---------------------------------------------------------------------
        protected ExceptionHandlerPrimitive GetHandlerPrimitive(Exception exception, bool findHandlerToInnerException)
        {
            if (exception == null)
            { throw new ArgumentNullException(nameof(exception)); }

            _logger.Log(LoggerLevel.Error, () => exception);

            Func<Exception, ExceptionHandlerPrimitive> actionExceptionHandler = null;
            var tmpException = exception;

            if (findHandlerToInnerException)
            {
                while (tmpException != null)
                {
                    if (_actionExceptionHandlers.TryGetValue(tmpException.GetType(), out actionExceptionHandler))
                    { break; }

                    tmpException = exception.InnerException;
                }
            }
            else
            { _actionExceptionHandlers.TryGetValue(tmpException.GetType(), out actionExceptionHandler); }

            var exceptionHandler = actionExceptionHandler != null
                ? actionExceptionHandler(exception)
                : new ExceptionHandlerPrimitive(exception, DefaultExceptionTitle(), DefaultExceptionMessage(exception), ExceptionHandlerLevel.Error, true);

            return exceptionHandler;

        }
        //---------------------------------------------------------------------
        protected abstract string DefaultExceptionTitle();
        protected abstract void ShowExceptionHandler(ExceptionHandlerPrimitive exceptionHandler);
        //---------------------------------------------------------------------
        protected virtual string DefaultExceptionMessage(Exception exception)
        {
            return exception?.ToString() ?? string.Empty;
        }
        #endregion
        protected BaseExceptionHandlers(ILogger logger = null)
        {
            _logger = logger;
        }

        //---------------------------------------------------------------------
        public void Handler(Exception exception, bool findHandlerToInnerException = true)
        {
            if (exception != null)
            {
                var exceptionHandler = GetHandlerPrimitive(exception, findHandlerToInnerException);
                ShowExceptionHandler(exceptionHandler);
            }
        }
        //---------------------------------------------------------------------
    }
}
