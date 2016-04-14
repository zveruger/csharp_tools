using System;
using System.Collections.Generic;
using System.Threading;

namespace Tools.CSharp.ProgressProcesses
{
    public sealed class ProgressProcessSheduler
    {
        #region private
        private readonly List<ProgressProcess> _processes = new List<ProgressProcess>();
        private readonly int _maxRunProcessesCount;
        //---------------------------------------------------------------------
        private int _runProcessesCount;
        //---------------------------------------------------------------------
        private int _findProcessIndex(ProgressProcess process)
        {
            var processIndex = -1;
            if (process != null)
            {
                for (var i = _processes.Count - 1; i >= 0; i--)
                {
                    var tmpProcess = _processes[i];
                    if (ReferenceEquals(tmpProcess, process))
                    {
                        processIndex = i;
                        break;
                    }
                }
            }
            return processIndex;
        }
        private ProgressProcess _getNextProcess(int startIndex)
        {
            ProgressProcess nextProcess = null;
            for (var i = startIndex; (i >= 0) && (i < _processes.Count); i--)
            {
                var process = _processes[i];
                if (process.State == ProgressProcessState.Waiting)
                {
                    nextProcess = process;
                    break;
                }
            }
            return nextProcess;
        }
        //---------------------------------------------------------------------
        private void _resetRunProcessCount()
        {
            Interlocked.Exchange(ref _runProcessesCount, 0);
        }
        #endregion
        public ProgressProcessSheduler(bool isProcessorCount = false)
            : this(isProcessorCount ? Environment.ProcessorCount : 1)
        { }
        public ProgressProcessSheduler(int maxRunProcessesCount)
        {
            if (maxRunProcessesCount < 1)
            { throw new ArgumentOutOfRangeException(nameof(maxRunProcessesCount), maxRunProcessesCount, string.Empty); }

            _maxRunProcessesCount = maxRunProcessesCount;
        }

        //---------------------------------------------------------------------
        public bool IncrementRunProcessCount()
        {
            if (Interlocked.CompareExchange(ref _runProcessesCount, _maxRunProcessesCount, _maxRunProcessesCount) != _maxRunProcessesCount)
            { return Interlocked.Increment(ref _runProcessesCount) <= _maxRunProcessesCount; }

            return false;
        }
        //---------------------------------------------------------------------
        public void AddProcess(ProgressProcess process)
        {
            if (process == null)
            { throw new ArgumentNullException(nameof(process)); }

            _processes.Insert(0, process);
        }
        public void RemoveProcess(ProgressProcess process)
        {
            _processes.Remove(process);
        }
        //---------------------------------------------------------------------
        public ProgressProcess GetNextProcessAndDecrementRunProcessCount(ProgressProcess process)
        {
            var processIndex = _findProcessIndex(process);
            if (processIndex == -1)
            { return null; }

            _processes.RemoveAt(processIndex);

            var nextProcess = _getNextProcess(processIndex - 1);

            if (_processes.Count == 0)
            { _resetRunProcessCount(); }

            return nextProcess;
        }
        //---------------------------------------------------------------------
    }
}