using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Tools.CSharp.Loggers
{
    public abstract class BaseLogger<TLoggerWritable> : BaseDisposable, ILogger
        where TLoggerWritable : BaseLoggerWritable
    {
        #region private
        private readonly Dictionary<LoggerLevel, LoggerPrimitive> _cacheLevelLoggerPrimitives = new Dictionary<LoggerLevel, LoggerPrimitive>();
        //---------------------------------------------------------------------
        private readonly TraceSwitch _traceSwitch;
        private readonly TLoggerWritable _loggerWritable;
        private readonly string _loggerName;
        #endregion
        #region protected
        protected TLoggerWritable LoggerWritable
        {
            get { return _loggerWritable; }
        }
        //---------------------------------------------------------------------
        protected abstract ILogger CreateChildLogger(string loggerName, TLoggerWritable loggerWritable);
        #endregion
        protected BaseLogger(string loggerName, TLoggerWritable loggerWritable)
        {
            if (string.IsNullOrWhiteSpace(loggerName))
            { throw new ArgumentNullException(nameof(loggerName)); }
            if (loggerWritable == null)
            { throw new ArgumentNullException(nameof(loggerWritable)); }

            _loggerName = loggerName;
            _loggerWritable = loggerWritable;
            _traceSwitch = new TraceSwitch(_loggerName, string.Empty);
        }

        //---------------------------------------------------------------------
        public bool IsLevelOn(LoggerLevel level)
        {
            return (int)_traceSwitch.Level >= (int)level;
        }
        //---------------------------------------------------------------------
        public void Log(LoggerLevel level, string message)
        {
            if (level < LoggerLevel.Off || level > LoggerLevel.Verbose)
            { throw new InvalidEnumArgumentException(nameof(level), (int)level, typeof(LoggerLevel)); }

            if (IsLevelOn(level))
            {
                if (!string.IsNullOrWhiteSpace(message))
                {  _loggerWritable.Write(_loggerName, level, message); }
            }
        }
        public void Log(LoggerLevel level, object obj)
        {
            if (level < LoggerLevel.Off || level > LoggerLevel.Verbose)
            { throw new InvalidEnumArgumentException(nameof(level), (int)level, typeof(LoggerLevel)); }

            if (IsLevelOn(level))
            {
                var messasge = obj?.ToString() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(messasge))
                { _loggerWritable.Write(_loggerName, level, messasge); }
            }
        }
        //---------------------------------------------------------------------
        public ILogger CreateChild(string loggerName)
        {
            if (string.IsNullOrWhiteSpace(loggerName))
            { throw new ArgumentNullException(nameof(loggerName)); }

            return CreateChildLogger(loggerName, _loggerWritable);
        }
        //---------------------------------------------------------------------
        public LoggerPrimitive GetErrorLoggerPrimitive
        {
            get { return GetLoggerPrimitive(LoggerLevel.Error); }
        }
        public LoggerPrimitive GetWarningLoggerPrimitive
        {
            get { return GetLoggerPrimitive(LoggerLevel.Warning); }
        }
        public LoggerPrimitive GetInfoLoggerPrimitive
        {
            get { return GetLoggerPrimitive(LoggerLevel.Info); }
        }
        public LoggerPrimitive GetVerboseLoggerPrimitive
        {
            get { return GetLoggerPrimitive(LoggerLevel.Verbose); }
        }
        //---------------------------------------------------------------------
        public LoggerPrimitive GetLoggerPrimitive(LoggerLevel level)
        {
            LoggerPrimitive loggerPrimitive;
            if (!_cacheLevelLoggerPrimitives.TryGetValue(level, out loggerPrimitive))
            {
                if (IsLevelOn(level))
                {
                    loggerPrimitive = new LoggerPrimitive(this, level);
                    _cacheLevelLoggerPrimitives.Add(level, loggerPrimitive);
                }
            }
            return loggerPrimitive;
        }
        //---------------------------------------------------------------------
    }
}