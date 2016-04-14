using System;
using System.Collections.Generic;
using Tools.CSharp.Helpers;
using Tools.CSharp.Loggers;

namespace Tools.CSharp.Handlers
{
    public abstract class BaseExceptionHandlersByValidationHepler : BaseExceptionHandlers, IValidationHelper
    {
        #region private
        private readonly Dictionary<Type, Func<Enum, object, string>> _actionPropertyErrorEnumHandlers = new Dictionary<Type, Func<Enum, object, string>>();
        private readonly Dictionary<Type, IValidationHelper> _childHelpers = new Dictionary<Type, IValidationHelper>();
        //---------------------------------------------------------------------
        private Func<int, object, string> _actionPropertyErrorValueHandler;
        //---------------------------------------------------------------------
        private sealed class ValidationHelperError<TError> : IValidationHelper<TError>
            where TError : struct 
        {
            #region private
            private readonly BaseExceptionHandlersByValidationHepler _owner;
            #endregion
            public ValidationHelperError(BaseExceptionHandlersByValidationHepler owner)
            {
                _owner = owner;
            }

            //-----------------------------------------------------------------
            public IValidationHelper<TError1> CreateChildHelper<TError1>() 
                where TError1 : struct
            {
                return _owner.CreateChildHelper<TError1>();
            }
            //-----------------------------------------------------------------
            public string CreateMessageInvalidity(TError error, object tag)
            {
                return _owner.CreateMessageInvalidity(error as Enum, tag);
            }
            public string CreateMessageInvalidity(int errorValue, object tag)
            {
                return _owner.CreateMessageInvalidity(errorValue, tag);
            }
            public string CreateMessageInvalidity(Enum errorEnumValue, object tag)
            {
                return _owner.CreateMessageInvalidity(errorEnumValue, tag);
            }
            //-----------------------------------------------------------------
        }
        #endregion
        #region protected
        protected const string DefaultMessageInvalid = "";
        //---------------------------------------------------------------------
        protected void AddActionPropertyHandler<TEnum>(Func<Enum, object, string> handler)
            where TEnum : struct
        {
            if (handler == null)
            { throw new ArgumentNullException(nameof(handler)); }

            _actionPropertyErrorEnumHandlers.Add(typeof(TEnum), handler);
        }
        protected void AddActionPropertyHandler(Func<int, object, string> handler)
        {
            if (handler == null)
            { throw new ArgumentNullException(nameof(handler)); }
            if (_actionPropertyErrorValueHandler != null)
            { throw new ArgumentException(string.Empty, nameof(handler)); }

            _actionPropertyErrorValueHandler = handler;
        }
        #endregion
        protected BaseExceptionHandlersByValidationHepler(ILogger logger = null)
            : base(logger)
        { }

        //---------------------------------------------------------------------
        public IValidationHelper<TError> CreateChildHelper<TError>()
            where TError : struct 
        {
            var validationHelderKey = typeof(TError);
            IValidationHelper validationHelper;

            if (!_childHelpers.TryGetValue(validationHelderKey, out validationHelper))
            {
                validationHelper = new ValidationHelperError<TError>(this);
                _childHelpers.Add(validationHelderKey, validationHelper);
            }

            return (IValidationHelper<TError>)validationHelper;
        }
        //---------------------------------------------------------------------
        public string CreateMessageInvalidity(int errorValue, object tag)
        {
            return _actionPropertyErrorValueHandler != null ? _actionPropertyErrorValueHandler(errorValue, tag) : DefaultMessageInvalid;
        }
        public string CreateMessageInvalidity(Enum errorEnumValue, object tag)
        {
            if (errorEnumValue == null)
            { throw new ArgumentNullException(nameof(errorEnumValue)); }

            Func<Enum, object, string> actionPropertyHandler;
            if (_actionPropertyErrorEnumHandlers.TryGetValue(errorEnumValue.GetType(), out actionPropertyHandler))
            { return actionPropertyHandler(errorEnumValue, tag); }

            throw new ValidationHeplerException(errorEnumValue);
        }
        //---------------------------------------------------------------------
    }
}