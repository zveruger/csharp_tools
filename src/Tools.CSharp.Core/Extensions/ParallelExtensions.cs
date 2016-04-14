using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tools.CSharp.Extensions
{
    public static class ParallelExtensions
    {
        #region private
        private static readonly ParallelOptions _DefaultParallelOptions = new ParallelOptions();
        private static readonly Func<bool> _DefaultCondition = () => true;

        private sealed class InfinitePartitioner : Partitioner<bool>
        {
            #region private
            private static IEnumerator<bool> _InfiniteEnumerator()
            {
                while(true)
                { yield return true; }
            }

            private sealed class InfiniteEnumerators : IEnumerable<bool>
            {
                //-------------------------------------------------------------
                public IEnumerator<bool> GetEnumerator()
                {
                    return _InfiniteEnumerator();
                }
                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }
                //-------------------------------------------------------------
            }
            #endregion
            //-----------------------------------------------------------------
            public override bool SupportsDynamicPartitions
            {
                get { return true; }
            }
            //-----------------------------------------------------------------
            public override IList<IEnumerator<bool>> GetPartitions(int partitionCount)
            {
                if (partitionCount < 1)
                { throw new ArgumentOutOfRangeException(nameof(partitionCount)); }

                return (from i in Enumerable.Range(0, partitionCount) select _InfiniteEnumerator()).ToArray();
            }
            public override IEnumerable<bool> GetDynamicPartitions()
            {
                return new InfiniteEnumerators();
            }
            //-----------------------------------------------------------------
        }

        private static ParallelLoopResult _While(ParallelOptions parallelOptions, Func<bool> condition, Action body, Action<ParallelLoopState> bodyWithState)
        {
            if (parallelOptions.CancellationToken.IsCancellationRequested)
            { throw new OperationCanceledException(parallelOptions.CancellationToken); }

            var infinityPartitioner = new InfinitePartitioner();

            if (body != null)
            { return Parallel.ForEach(infinityPartitioner, parallelOptions, i => { if (condition()) { body(); } }); }

            return Parallel.ForEach(infinityPartitioner, parallelOptions, (i, loopState) =>
            {
                if (condition())
                { bodyWithState(loopState); }
                else
                { loopState.Stop(); }
            });
        }
        #endregion
        //---------------------------------------------------------------------
        public static ParallelLoopResult While(Action body)
        {
            return While(body, _DefaultCondition);
        }
        public static ParallelLoopResult While(Action body, Func<bool> condition)
        {
            return While(body, condition, _DefaultParallelOptions);
        }
        public static ParallelLoopResult While(Action body, Func<bool> condition, ParallelOptions parallelOptions)
        {
            if (body == null)
            { throw new ArgumentNullException(nameof(body)); }

            if (condition == null)
            { throw new ArgumentNullException(nameof(condition)); }

            if (parallelOptions == null)
            { throw new ArgumentNullException(nameof(parallelOptions)); }

            return _While(parallelOptions, condition, body, null);
        }
        public static ParallelLoopResult While(Action<ParallelLoopState> body)
        {
            return While(body, _DefaultCondition);
        }
        public static ParallelLoopResult While(Action<ParallelLoopState> body, Func<bool> condition)
        {
            return While(body, condition, _DefaultParallelOptions);
        }
        public static ParallelLoopResult While(Action<ParallelLoopState> body, Func<bool> condition, ParallelOptions parallelOptions)
        {
            if (body == null)
            { throw new ArgumentNullException(nameof(body)); }

            if (condition == null)
            { throw new ArgumentNullException(nameof(condition)); }

            if (parallelOptions == null)
            { throw new ArgumentNullException(nameof(parallelOptions)); }

            return _While(parallelOptions, condition, null, body);
        }
        //---------------------------------------------------------------------
    }
}