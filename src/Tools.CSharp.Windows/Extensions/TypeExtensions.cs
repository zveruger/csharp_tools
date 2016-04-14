using System;

namespace Tools.CSharp.Extensions
{
    public static class TypeExtensions
    {
        //---------------------------------------------------------------------
        public static Type FindGenericParent(this Type rootType, Type parentType)
        {
            if (rootType == null)
            { throw new ArgumentNullException(nameof(rootType)); }
            if (parentType == null)
            { throw new ArgumentNullException(nameof(parentType)); }

            if (parentType.IsGenericType)
            {
                var currentType = rootType;
                while(currentType != null && currentType != typeof(object))
                {
                    if (currentType.IsGenericType)
                    {
                        var genericType = currentType.GetGenericTypeDefinition();
                        if (genericType == parentType)
                        { return currentType; }
                    }

                    currentType = currentType.BaseType;
                }
            }
            return null;
        }
        //---------------------------------------------------------------------
    }
}