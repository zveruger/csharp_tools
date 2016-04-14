using System;

namespace Tools.CSharp.Extensions
{
    public static class WeakReferenceExpression
    {
        //---------------------------------------------------------------------
        public static T GetTarget<T>(this WeakReference weakReference, Func<T> createTarget)
            where T : class
        {
            if (weakReference == null)
            { throw new ArgumentNullException(nameof(weakReference)); }

            if (weakReference.Target == null)
            { weakReference.Target = createTarget(); }

            return weakReference.Target as T;
        }
        //---------------------------------------------------------------------
    }
}