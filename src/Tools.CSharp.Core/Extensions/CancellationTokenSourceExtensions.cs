using System.Threading;

namespace Tools.CSharp.Extensions
{
    public static class CancellationTokenSourceExtensions
    {
        //---------------------------------------------------------------------
        public static CancellationTokenSource CreateNoCancellationRequested(this CancellationTokenSource source)
        {
            if (source == null || source.IsCancellationRequested)
            {
                source?.Dispose();
                return new CancellationTokenSource();
            }
            return source;
        }
        //---------------------------------------------------------------------
    }
}
