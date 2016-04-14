namespace Tools.CSharp.Loggers
{
    public class FileLogger : BaseLogger<FileLoggerWritable>
    {
        #region protected
        protected override ILogger CreateChildLogger(string loggerName, FileLoggerWritable loggerWritable)
        {
            return new FileLogger(loggerName, loggerWritable);
        }
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed && disposing)
                {
                    LoggerWritable?.Dispose();
                }
            }
            finally
            { base.Dispose(disposing); }
        }
        #endregion
        protected FileLogger(string loggerName, FileLoggerWritable loggerWritable)
            : base(loggerName, loggerWritable)
        { }
        public FileLogger(string loggerName, string companyName, string productName, string directoryName, string fileName)
            : base(loggerName, new FileLoggerWritable(companyName, productName, directoryName, fileName))
        { }

        //---------------------------------------------------------------------
        public string FileName
        {
            get { return LoggerWritable.FileName; }
        }
        //---------------------------------------------------------------------
    }
}