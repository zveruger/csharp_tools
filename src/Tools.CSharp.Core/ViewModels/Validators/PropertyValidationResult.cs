using System;
using Tools.CSharp.Helpers;

namespace Tools.CSharp.ViewModels
{
    //-------------------------------------------------------------------------
    public class PropertyValidationResult
    {
        #region private
        private readonly string _propertyName;
        private readonly bool _isValid;
        private readonly string _messageInvalidity;
        private readonly int _errorValue;
        private readonly Enum _errorEnumValue;
        private readonly object _tag;
        #endregion
        #region protected
        protected const int DefaultErrorValue = -1;
        protected const string DefaultMessageInvalidity = "";
        //---------------------------------------------------------------------
        protected void ThrowInvalidOperationExceptionWhenValid()
        {
            if (_isValid)
            { throw new InvalidOperationException(); }
        }
        //---------------------------------------------------------------------
        protected PropertyValidationResult(string propertyName, bool isValid, string messageInvalidity, int errorValue, Enum errorEnumValue, object tag)
        {
            if (string.IsNullOrEmpty(propertyName))
            { throw new ArgumentNullException(nameof(propertyName)); }

            _propertyName = propertyName;
            _isValid = isValid;
            _tag = tag;

            if (_isValid)
            {
                _messageInvalidity = DefaultMessageInvalidity;
                _errorValue = DefaultErrorValue;
                _errorEnumValue = null;
            }
            else
            {
                _messageInvalidity = messageInvalidity;
                _errorValue = errorValue;
                _errorEnumValue = errorEnumValue;
            }
        }
        protected PropertyValidationResult(string propertyName, bool isValid, string messageInvalidity, Enum errorEnumValue, object tag)
        {
            if (string.IsNullOrEmpty(propertyName))
            { throw new ArgumentNullException(nameof(propertyName)); }

            _propertyName = propertyName;
            _isValid = isValid;
            _tag = tag;

            if (_isValid)
            {
                _messageInvalidity = DefaultMessageInvalidity;
                _errorValue = DefaultErrorValue;
                _errorEnumValue = null;
            }
            else
            {
                _messageInvalidity = messageInvalidity;
                _errorValue = errorEnumValue == null ? DefaultErrorValue : Convert.ToInt32(errorEnumValue);
                _errorEnumValue = errorEnumValue;
            }
        }
        #endregion
        public PropertyValidationResult(string propertyName)
            : this(propertyName, true, DefaultMessageInvalidity, DefaultErrorValue, null, null)
        { }
        //---------------------------------------------------------------------
        public PropertyValidationResult(string propertyName, string messageInvalidity)
            : this(propertyName, false, messageInvalidity, DefaultErrorValue, null, null)
        { }
        public PropertyValidationResult(string propertyName, string messageInvalidity, int errorValue)
            : this(propertyName, false, messageInvalidity, errorValue, null, null)
        { }
        public PropertyValidationResult(string propertyName, string messageInvalidity, Enum errorEnumValue)
            : this(propertyName, false, messageInvalidity, errorEnumValue, null)
        { }
        public PropertyValidationResult(string propertyName, string messageInvalidity, object tag)
            : this(propertyName, false, messageInvalidity, DefaultErrorValue, null, tag)
        { }
        public PropertyValidationResult(string propertyName, int errorValue)
           : this(propertyName, false, DefaultMessageInvalidity, errorValue, null, null)
        { }
        public PropertyValidationResult(string propertyName, int errorValue, object tag)
           : this(propertyName, false, DefaultMessageInvalidity, errorValue, null, tag)
        { }
        public PropertyValidationResult(string propertyName, Enum errorEnumValue)
            : this(propertyName, false, DefaultMessageInvalidity, errorEnumValue, null)
        {
        }
        public PropertyValidationResult(string propertyName, Enum errorEnumValue, object tag)
            : this(propertyName, false, DefaultMessageInvalidity, errorEnumValue, tag)
        {
        }
        //---------------------------------------------------------------------
        public PropertyValidationResult(string propertyName, bool isValid, string messageInvalidity)
            : this(propertyName, isValid, messageInvalidity, DefaultErrorValue, null, null)
        { }
        public PropertyValidationResult(string propertyName, bool isValid, string messageInvalidity, int errorValue)
            : this(propertyName, isValid, messageInvalidity, errorValue, null, null)
        { }
        public PropertyValidationResult(string propertyName, bool isValid, string messageInvalidity, Enum errorEnumValue)
            : this(propertyName, isValid, messageInvalidity, errorEnumValue, null)
        { }
        public PropertyValidationResult(string propertyName, bool isValid, string messageInvalidity, object tag)
            : this(propertyName, isValid, messageInvalidity, DefaultErrorValue, null, tag)
        { }
        public PropertyValidationResult(string propertyName, bool isValid, int errorValue)
           : this(propertyName, isValid, DefaultMessageInvalidity, errorValue, null, null)
        { }
        public PropertyValidationResult(string propertyName, bool isValid, int errorValue, object tag)
           : this(propertyName, isValid, DefaultMessageInvalidity, errorValue, null, tag)
        { }
        public PropertyValidationResult(string propertyName, bool isValid, Enum errorEnumValue)
            : this(propertyName, isValid, DefaultMessageInvalidity, errorEnumValue, null)
        {
        }
        public PropertyValidationResult(string propertyName, bool isValid, Enum errorEnumValue, object tag)
            : this(propertyName, isValid, DefaultMessageInvalidity, errorEnumValue, tag)
        {
        }

        //---------------------------------------------------------------------
        public string PropertyName
        {
            get { return _propertyName; }
        }
        //---------------------------------------------------------------------
        public bool IsValid
        {
            get { return _isValid; }
        }
        //---------------------------------------------------------------------
        public int ErrorValue
        {
            get
            {
                ThrowInvalidOperationExceptionWhenValid();
                return _errorValue;
            }
        }
        public Enum ErrorEnumValue
        {
            get
            {
                ThrowInvalidOperationExceptionWhenValid();
                return _errorEnumValue;
            }
        }
        //---------------------------------------------------------------------
        public string MessageInvalidity
        {
            get
            {
                ThrowInvalidOperationExceptionWhenValid();
                return _messageInvalidity;
            }
        }
        //---------------------------------------------------------------------
        public object Tag
        {
            get { return _tag; }
        }
        //---------------------------------------------------------------------
        public string CreateMessageInvalidity(IValidationHelper validationHelper)
        {
            return CreateMessageInvalidity(validationHelper, _tag);
        }
        public string CreateMessageInvalidity(IValidationHelper validationHelper, object tag)
        {
            if (IsValid || validationHelper == null)
            { return DefaultMessageInvalidity; }

            return _errorEnumValue == null
                ? validationHelper.CreateMessageInvalidity(_errorValue, null)
                : validationHelper.CreateMessageInvalidity(_errorEnumValue, tag);
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    public class PropertyValidationResult<TError> : PropertyValidationResult
        where TError : struct
    {
        #region private
        private readonly TError _error;
        //---------------------------------------------------------------------
        #endregion
        #region protected
        protected static readonly TError DefaultErroTyperValue = default(TError);
        //---------------------------------------------------------------------
        protected PropertyValidationResult(string propertyName, bool isValid, string messageInvalidity, TError error, object tag)
            : base(propertyName, isValid, messageInvalidity, error as Enum, tag)
        {
            _error = isValid ? DefaultErroTyperValue : error;
        }
        #endregion
        public PropertyValidationResult(string propertyName)
            : this(propertyName, true, DefaultMessageInvalidity, DefaultErroTyperValue, null)
        { }
        //---------------------------------------------------------------------
        public PropertyValidationResult(string propertyName, string messageInvalidity)
            : this(propertyName, false, messageInvalidity, DefaultErroTyperValue, null)
        { }
        public PropertyValidationResult(string propertyName, string messageInvalidity, TError error)
            : this(propertyName, false, messageInvalidity, error, null)
        { }
        public PropertyValidationResult(string propertyName, string messageInvalidity, object tag)
            : this(propertyName, false, messageInvalidity, DefaultErroTyperValue, tag)
        { }
        public PropertyValidationResult(string propertyName, TError error)
            : this(propertyName, false, DefaultMessageInvalidity, error, null)
        {
        }
        public PropertyValidationResult(string propertyName, TError error, object tag)
            : this(propertyName, false, DefaultMessageInvalidity, error, tag)
        {
        }
        //---------------------------------------------------------------------
        public PropertyValidationResult(string propertyName, bool isValid, string messageInvalidity)
            : this(propertyName, isValid, messageInvalidity, DefaultErroTyperValue, null)
        { }
        public PropertyValidationResult(string propertyName, bool isValid, string messageInvalidity, TError error)
            : this(propertyName, isValid, messageInvalidity, error, null)
        { }
        public PropertyValidationResult(string propertyName, bool isValid, string messageInvalidity, object tag)
            : this(propertyName, isValid, messageInvalidity, DefaultErroTyperValue, tag)
        { }
        public PropertyValidationResult(string propertyName, bool isValid, TError error)
            : this(propertyName, isValid, DefaultMessageInvalidity, error, null)
        { }
        public PropertyValidationResult(string propertyName, bool isValid, TError error, object tag)
            : this(propertyName, isValid, DefaultMessageInvalidity, error, tag)
        { }

        //---------------------------------------------------------------------
        public TError Error
        {
            get
            {
                ThrowInvalidOperationExceptionWhenValid();
                return _error;
            }
        }
        //---------------------------------------------------------------------
        public string CreateMessageInvalidity(IValidationHelper<TError> validationHelper)
        {
            return CreateMessageInvalidity(validationHelper, Tag);
        }
        public string CreateMessageInvalidity(IValidationHelper<TError> validationHelper, object tag)
        {
            if (IsValid || validationHelper == null)
            { return DefaultMessageInvalidity; }

            return validationHelper.CreateMessageInvalidity(_error, tag);
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
}