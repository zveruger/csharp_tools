using System;
using System.Linq.Expressions;
using Tools.CSharp.Loggers;

namespace Tools.CSharp.Extensions
{
    public static class LoggerExtensions
    {
        //---------------------------------------------------------------------
        public static void Log(this ILogger logger, LoggerLevel level, Expression<Func<string>> expression)
        {
            if (expression == null)
            { throw new ArgumentNullException(nameof(expression)); }

            logger?.GetLoggerPrimitive(level)?.Log(expression.Compile().Invoke());
        }
        public static void Log(this ILogger logger, LoggerLevel level, Expression<Func<object>> expression)
        {
            if (expression == null)
            { throw new ArgumentNullException(nameof(expression)); }

            logger?.GetLoggerPrimitive(level)?.Log(expression.Compile().Invoke());
        }
        //---------------------------------------------------------------------
    }
}