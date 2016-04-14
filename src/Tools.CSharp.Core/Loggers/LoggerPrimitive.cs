using System;

namespace Tools.CSharp.Loggers
{
    public sealed class LoggerPrimitive
    {
        #region private
        private readonly ILogger _owner;
        private readonly LoggerLevel _level;
        #endregion
        public LoggerPrimitive(ILogger owner, LoggerLevel level)
        {
            if (owner == null)
            { throw new ArgumentNullException(nameof(owner)); }

            _owner = owner;
            _level = level;
        }

        //---------------------------------------------------------------------
        public ILogger Owner
        {
            get { return _owner; }
        }
        //---------------------------------------------------------------------
        public bool LogError
        {
            get { return _level >= LoggerLevel.Error; }
        }
        public bool LogWarning
        {
            get { return _level >= LoggerLevel.Warning; }
        }
        public bool LogInfo
        {
            get { return _level >= LoggerLevel.Info; }
        }
        public bool LogVerbose
        {
            get { return _level == LoggerLevel.Verbose; }
        }
        //---------------------------------------------------------------------
        public void Log(string message)
        {
            _owner.Log(_level, message);
        }
        public void Log(object obj)
        {
            _owner.Log(_level, obj);
        }
        //---------------------------------------------------------------------
    }
}