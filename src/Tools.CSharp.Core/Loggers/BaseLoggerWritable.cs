namespace Tools.CSharp.Loggers
{
    public abstract class BaseLoggerWritable : BaseDisposable
    {
        #region protected
        protected abstract void WriteToLog(string loggerName, LoggerLevel level, string message);
        #endregion
        protected BaseLoggerWritable()
        { }

        //---------------------------------------------------------------------
        public void Write(string loggerName, LoggerLevel level, string message)
        {
            WriteToLog(loggerName, level, message);
        }
        //---------------------------------------------------------------------
    }
}