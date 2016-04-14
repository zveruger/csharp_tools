using System;

namespace Tools.CSharp.DataTable.CompositeObjects
{
    [Serializable]
    public sealed class ContainedObjectDestroyedException : ContainedObjectException
    {
        internal ContainedObjectDestroyedException(IContainedObject containedObject)
            : base(containedObject)
        { }
        internal ContainedObjectDestroyedException(string objectName)
            : base(objectName)
        { }
    }
}