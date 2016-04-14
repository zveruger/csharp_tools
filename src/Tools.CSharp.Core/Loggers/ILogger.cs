namespace Tools.CSharp.Loggers
{
    public interface ILogger
    {
        //---------------------------------------------------------------------
        bool IsLevelOn(LoggerLevel level);
        //---------------------------------------------------------------------
        void Log(LoggerLevel level, string message);
        void Log(LoggerLevel level, object obj);
        //---------------------------------------------------------------------
        ILogger CreateChild(string loggerName);
        //---------------------------------------------------------------------
        LoggerPrimitive GetErrorLoggerPrimitive { get; }
        LoggerPrimitive GetWarningLoggerPrimitive { get; }
        LoggerPrimitive GetInfoLoggerPrimitive { get; }
        LoggerPrimitive GetVerboseLoggerPrimitive { get; }
        //---------------------------------------------------------------------
        LoggerPrimitive GetLoggerPrimitive(LoggerLevel level);
        //---------------------------------------------------------------------
    }
}
