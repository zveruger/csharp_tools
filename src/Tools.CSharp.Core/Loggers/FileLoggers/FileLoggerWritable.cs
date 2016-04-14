using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Tools.CSharp.Loggers
{
    public class FileLoggerWritable : BaseLoggerWritable
    {
        #region private
        private readonly TraceListener _listener;
        private readonly string _logFileName;
        //---------------------------------------------------------------------
        private static string _UserFolderPath(string companyName, string productName, string directoryName)
        {
            var userFolderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                companyName,
                productName,
                directoryName
            );

            if (!Directory.Exists(userFolderPath))
            { Directory.CreateDirectory(userFolderPath); }

            return userFolderPath;
        }
        #endregion
        #region protected
        protected virtual string CreateLogMessage(string loggerName, LoggerLevel level, string message)
        {
            return string.Format(Thread.CurrentThread.CurrentCulture, "{0}: {1}: {2}: {3}",
                DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss", Thread.CurrentThread.CurrentCulture),
                loggerName,
                level.ToString().Substring(0, 1),
                message
            );
        }
        //---------------------------------------------------------------------
        protected override void WriteToLog(string loggerName, LoggerLevel level, string message)
        {
            var logMessage = CreateLogMessage(loggerName, level, message);
            if (!string.IsNullOrWhiteSpace(logMessage))
            {
                _listener.WriteLine(logMessage);
                _listener.Flush();
            }
        }
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed && disposing)
                {
                    _listener?.Dispose();
                }
            }
            finally
            { base.Dispose(disposing); }
        }
        #endregion
        public FileLoggerWritable(string companyName, string productName, string directoryName, string fileName)
        {
            if (string.IsNullOrWhiteSpace(companyName))
            { throw new ArgumentNullException(nameof(companyName)); }
            if (string.IsNullOrWhiteSpace(productName))
            { throw new ArgumentNullException(nameof(productName)); }
            if (string.IsNullOrWhiteSpace(directoryName))
            { throw new ArgumentNullException(nameof(directoryName)); }
            if (string.IsNullOrWhiteSpace(fileName))
            { throw new ArgumentNullException(nameof(fileName)); }

            _logFileName = Path.Combine(_UserFolderPath(companyName, productName, directoryName), fileName);
            _listener = new TextWriterTraceListener(_logFileName);
        }

        //---------------------------------------------------------------------
        public string FileName
        {
            get { return _logFileName; }
        }
        //---------------------------------------------------------------------
    }
}