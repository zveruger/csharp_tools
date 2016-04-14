using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Tools.CSharp.Helpers
{
    [Serializable]
    public class ValidationHeplerException : Exception
    {
        #region private
        private readonly Enum _errorEnumValue;
        #endregion
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected ValidationHeplerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _errorEnumValue = (Enum)info.GetValue(nameof(ErrorEnumValue), typeof(Enum));
        }
        public ValidationHeplerException(Enum errorEnumValue)
        {
            if (errorEnumValue == null)
            { throw new ArgumentNullException(nameof(errorEnumValue)); }

            _errorEnumValue = errorEnumValue;
        }

        //---------------------------------------------------------------------
        public Enum ErrorEnumValue
        {
            get { return _errorEnumValue; }
        }
        //---------------------------------------------------------------------
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(ErrorEnumValue), ErrorEnumValue, typeof(Enum));;
        }
        //---------------------------------------------------------------------
        public override string ToString()
        {
            return $"{GetType().Name}: {_errorEnumValue}";
        }
        //---------------------------------------------------------------------
    }
}