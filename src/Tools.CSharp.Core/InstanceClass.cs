using System.Threading;

namespace Tools.CSharp
{
    public class InstanceClass<TOutput>
        where TOutput : InstanceClass<TOutput>, new()
    {
        #region private
        private static TOutput _Instance;
        #endregion
        //---------------------------------------------------------------------
        public static TOutput Instance
        {
            get { return LazyInitializer.EnsureInitialized(ref _Instance, () => new TOutput()); }
        }
        //---------------------------------------------------------------------
    }
}
