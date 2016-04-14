using System;
using System.IO;
using System.Net;
using System.Text;

namespace Tools.CSharp.Network.Http
{
    public sealed class FileWebServer : WebServer
    {
        #region private
        private readonly string _directoryFiles;
        #endregion
        #region protected
        protected override void DefaultCallback(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var responseOutput = response.OutputStream;
            //-----------------------------------------------------------------
            var resultType = FileWebServerCallbackResultType.FileNotFound;
            Exception exception = null;
            FileStream fileStream = null;
            //-----------------------------------------------------------------
            var filePath = _directoryFiles + request.RawUrl.Replace('/', DirectorySeparator);

            try
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        fileStream = File.OpenRead(filePath);
                        fileStream.CopyTo(responseOutput);

                        resultType = FileWebServerCallbackResultType.Success;
                    }
                    catch (Exception exc)
                    {
                        resultType = FileWebServerCallbackResultType.Exception;
                        exception = exc;
                    }
                }
                else
                {
                    var errorMessage = new FileNotFoundException(string.Empty, request.Url.ToString()).ToString();
                    var buf = Encoding.Default.GetBytes(errorMessage);
                    response.ContentLength64 = buf.Length;
                    responseOutput.Write(buf, 0, buf.Length);
                }
            }
            finally
            {
                fileStream?.Close();
                responseOutput.Close();

                Result?.Invoke(this, new FileWebServerCallbackResultEventArgs(request, filePath, resultType, exception));
            }
        }
        #endregion
        public FileWebServer()
            : this(DefaultDirectoryPath, DefaultLocalPrefix)
        { }
        public FileWebServer(string directoryFiles)
            : this(directoryFiles, DefaultLocalPrefix)
        { }
        public FileWebServer(params string[] prefixes)
            : this(DefaultDirectoryPath, prefixes)
        { }
        public FileWebServer(string directoryFiles, params string[] prefixes)
            : base(prefixes)
        {
            if(!Directory.Exists(directoryFiles))
            { throw new DirectoryNotFoundException(nameof(directoryFiles)); }

            _directoryFiles = (directoryFiles[directoryFiles.Length - 1] == DirectorySeparator)
                ? directoryFiles
                : $"{directoryFiles}{DirectorySeparator.ToString()}";
        }
        
        //---------------------------------------------------------------------
        public static readonly string DefaultDirectoryPath = Environment.CurrentDirectory;
        public static readonly char DirectorySeparator = Path.DirectorySeparatorChar;
        //---------------------------------------------------------------------
        public event EventHandler<FileWebServerCallbackResultEventArgs> Result;
        //---------------------------------------------------------------------
    }
}