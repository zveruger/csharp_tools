using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Tools.CSharp.DataTable.CompositeObjects
{
    [Serializable]
    public class ContainedObjectException : InvalidOperationException
    {
        #region private
        private readonly string _objectName;
        #endregion
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected ContainedObjectException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _objectName = info.GetString(nameof(ObjectName));
        }
        internal ContainedObjectException(IContainedObject containedObject)
            : this(containedObject.GetType().Name)
        { }
        internal ContainedObjectException(string objectName)
        {
            _objectName = objectName;
        }

        //---------------------------------------------------------------------
        public string ObjectName
        {
            get { return _objectName; }
        }
        //---------------------------------------------------------------------
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(ObjectName), _objectName, typeof(string));
        }
        //---------------------------------------------------------------------
        public override string ToString()
        {
            return string.IsNullOrEmpty(_objectName) ? base.ToString() : $"{base.ToString()}: {nameof(ObjectName)}={_objectName}";
        }
        //---------------------------------------------------------------------
    }
}