using System;
using System.ComponentModel;

namespace Tools.CSharp.Controls.Extensions
{
    public static class ObjectExtensions
    {
        //---------------------------------------------------------------------
        public static PropertyDescriptor GetPropertyName(this object component, string propertyName)
        {
            if (component == null)
            { throw new ArgumentNullException(nameof(component)); }

            return TypeDescriptor.GetProperties(component).Find(propertyName, true);
        }
        //---------------------------------------------------------------------
    }
}