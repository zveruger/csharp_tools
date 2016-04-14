using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Security;
using System.Text;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.Network.Ftp
{
    public class FtpClient
    {
        #region private
        /// <summary>
        /// ftp://account:password@serverName/
        /// </summary>
        private const string _ConnectFormatStr = "ftp:{0}{0}{1}:{2}@{3}{0}";
        private const char _DirectorySeparatorChar = '/';
        private const int _DefaultResponseTimeOut = 5000;
        private const int _DefaultBufferLength = 2048;
        //---------------------------------------------------------------------
        private readonly string _serverName;
        private readonly string _connectionStr;
        private readonly NetworkCredential _credential;
        //---------------------------------------------------------------------
        private int _responseTimeOut = _DefaultResponseTimeOut;
        //---------------------------------------------------------------------
        private static string _CreateConnectionStr(string serverName, string userName, string password)
        {
            return string.Format(_ConnectFormatStr, DirectorySeparatorChar, userName, password, serverName);
        }
        //---------------------------------------------------------------------
        private void _coreDownloadFile(string path, string pathToLocalMachine, int responseTimeOut, int bufferLength)
        {
            var webRequest = CreateWebRequest(path, WebRequestMethods.Ftp.DownloadFile);
            var asyncResult = webRequest.BeginGetResponse(null, null);

            if (asyncResult.AsyncWaitHandle.WaitOne(responseTimeOut, false))
            {
                FtpWebResponse webResponse = null;
                try
                {
                    webResponse = (FtpWebResponse)webRequest.EndGetResponse(asyncResult);
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        using (var fileStream = LocalMachineCreateFileStream(pathToLocalMachine))
                        { CopyStreams(responseStream, fileStream, bufferLength); }
                    }
                }
                finally
                { webResponse?.Close(); }
            }
            else
            {
                webRequest.Abort();
                throw new WebException(string.Empty, WebExceptionStatus.Timeout);
            }
        }
        private void _coreUploadFile(string pathToLocalMachine, string path, int responseTimeOut, int bufferLength)
        {
            var webRequest = CreateWebRequest(path, WebRequestMethods.Ftp.UploadFile);
            var asyncResult = webRequest.BeginGetRequestStream(null, null);

            if (asyncResult.AsyncWaitHandle.WaitOne(responseTimeOut, false))
            {
                using (var response = webRequest.EndGetRequestStream(asyncResult))
                {
                    using(var fileStream = LocalMachineOpenFileStream(pathToLocalMachine, false))
                    { CopyStreams(fileStream, response, bufferLength); }
                }
            }
            else
            {
                webRequest.Abort();
                throw new WebException(string.Empty, WebExceptionStatus.Timeout);
            }
        }
        private void _coreCreateFullPathDirectory(string path, int responseTimeOut)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                const char separator = _DirectorySeparatorChar;
                var directories = path.GetFilePathUnderDirectory(1, separator).Remove(0, ConnectionStr.Length).Split(separator);

                for (var i = directories.Length; i > 0; i--)
                {
                    var directory = path.GetFilePathUnderDirectory(i, separator);
                    if (!string.IsNullOrWhiteSpace(directory))
                    {
                        if (!_coreDirectoryExists(directory, responseTimeOut))
                        { _coreCreateDirectory(directory, responseTimeOut); }
                    }
                }
            }
        }
        private void _coreCreateDirectory(string path, int responseTimeOut)
        {
            var request = CreateWebRequest(path, WebRequestMethods.Ftp.MakeDirectory);
            var asyncResult = request.BeginGetResponse(null, null);

            if (asyncResult.AsyncWaitHandle.WaitOne(responseTimeOut, false))
            {
                FtpWebResponse response = null;
                try
                {
                    response = (FtpWebResponse)request.EndGetResponse(asyncResult);
                }
                finally
                { response?.Close(); }
            }
            else
            {
                request.Abort();
                throw new WebException(string.Empty, WebExceptionStatus.Timeout);
            }
        }
        private bool _coreDirectoryExists(string path, int responseTimeOut)
        {
            var request = CreateWebRequest(path, WebRequestMethods.Ftp.PrintWorkingDirectory);
            var asyncResult = request.BeginGetResponse(null, null);
            var result = false;

            if (asyncResult.AsyncWaitHandle.WaitOne(responseTimeOut, false))
            {
                FtpWebResponse response = null;
                try
                {
                    response = (FtpWebResponse)request.EndGetResponse(asyncResult);
                    result = true;
                }
                catch (WebException)
                { }
                finally
                { response?.Close(); }
            }
            else
            {
                request.Abort();
                throw new WebException(string.Empty, WebExceptionStatus.Timeout);
            }

            return result;
        }
        private bool _coreFileExists(string path, int responseTimeOut)
        {
            var request = CreateWebRequest(path, WebRequestMethods.Ftp.GetFileSize);
            var asyncResult = request.BeginGetResponse(null, null);
            var result = false;
            
            if (asyncResult.AsyncWaitHandle.WaitOne(responseTimeOut, false))
            {
                FtpWebResponse response = null;
                try
                {
                    response = (FtpWebResponse)request.EndGetResponse(asyncResult);
                    result = true;
                }
                catch (WebException)
                { }
                finally
                { response?.Close(); }
            }
            else
            {
                request.Abort();
                throw new WebException(string.Empty, WebExceptionStatus.Timeout);
            }

            return result;
        }
        private void _coreDeleteFile(string path, int responseTimeOut)
        {
            var request = CreateWebRequest(path, WebRequestMethods.Ftp.DeleteFile);
            var asyncResult = request.BeginGetResponse(null, null);

            if (asyncResult.AsyncWaitHandle.WaitOne(responseTimeOut, false))
            {
                FtpWebResponse response = null;
                try
                {
                    response = (FtpWebResponse)request.EndGetResponse(asyncResult);
                }
                catch (WebException)
                { throw new FileNotFoundException(nameof(path)); }
                finally
                { response?.Close(); }
            }
            else
            {
                request.Abort();
                throw new WebException(string.Empty, WebExceptionStatus.Timeout);
            }
        }
        #endregion
        #region protected
        protected WebClient CreateWebClient()
        {
            var webClient = new WebClient
            {
                Credentials = _credential,
                CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache),
                Proxy = null
            };

            return webClient;
        }
        //---------------------------------------------------------------------
        protected FtpWebRequest CreateWebRequest(string requestUriString, string method)
        {
            if (string.IsNullOrWhiteSpace(requestUriString))
            { throw new ArgumentNullException(nameof(requestUriString)); }
            if (string.IsNullOrWhiteSpace(method))
            { throw new ArgumentNullException(nameof(method)); }

            var requestUri = new Uri(requestUriString);
            if (requestUri.Scheme != Uri.UriSchemeFtp)
            { throw new ArgumentException(string.Empty, nameof(requestUriString)); }

            var request = (FtpWebRequest)WebRequest.Create(requestUri);
            request.Credentials = _credential;
            request.Proxy = null;
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            request.KeepAlive = false;
            request.Method = method;

            return request;
        }
        //---------------------------------------------------------------------
        protected static FileStream LocalMachineCreateFileStream(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            { return null; }

            var underDirectory = path.GetFilePathUnderDirectory(1, Path.DirectorySeparatorChar);
            Directory.CreateDirectory(underDirectory);

            return new FileStream(path, FileMode.Create);
        }
        protected static FileStream LocalMachineOpenFileStream(string path, bool openWrite)
        {
            return string.IsNullOrWhiteSpace(path) || !File.Exists(path) ? null : (openWrite ? File.OpenWrite(path) : File.OpenRead(path));
        }
        protected static void LocalMachineRemoveFile(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            { File.Delete(path); }
        }
        //---------------------------------------------------------------------
        protected static void CopyStreams(Stream sourceStream, Stream targetStream, int bufferLength = _DefaultBufferLength)
        {
            if (bufferLength < 0)
            { throw new ArgumentOutOfRangeException(nameof(bufferLength)); }

            if (sourceStream != null && targetStream != null)
            {
                var buffer = new byte[bufferLength];
                var readCount = sourceStream.Read(buffer, 0, bufferLength);

                while (readCount > 0)
                {
                    targetStream.Write(buffer, 0, readCount);
                    readCount = sourceStream.Read(buffer, 0, bufferLength);
                }
            }
        }
        #endregion
        public FtpClient(string serverName, string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(serverName))
            { throw new ArgumentNullException(nameof(serverName)); }
            if (string.IsNullOrWhiteSpace(userName))
            { throw new ArgumentNullException(nameof(userName)); }
            if (string.IsNullOrWhiteSpace(password))
            { throw new ArgumentNullException(nameof(password)); }

            _serverName = serverName;
            _credential = new NetworkCredential(userName, password);
            _connectionStr = _CreateConnectionStr(_serverName, userName, password);
        }

        //---------------------------------------------------------------------
        public const int DefaultResponseTimeOut = _DefaultResponseTimeOut;
        public const char DirectorySeparatorChar = _DirectorySeparatorChar;
        public const int DefaultBufferLength = _DefaultBufferLength;
        //---------------------------------------------------------------------
        public int ResponseTimeOut
        {
            get { return _responseTimeOut; }
            set
            {
                if (value < 0)
                { throw new ArgumentOutOfRangeException(nameof(value)); }

                _responseTimeOut = value;
            }
        }
        //---------------------------------------------------------------------
        public string ConnectionStr
        {
            get { return _connectionStr; }
        }
        public string ServerName
        {
            get { return _serverName; }
        }
        public string UserName
        {
            get { return _credential.UserName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                { throw new ArgumentNullException(nameof(value)); }

                _credential.UserName = value;
            }
        }
        public string Password
        {
            get { return _credential.Password; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                { throw new ArgumentNullException(nameof(value)); }

                _credential.Password = value;
            }
        }
        public SecureString SecurePassword
        {
            get { return _credential.SecurePassword; }
            set
            {
                if (value == null)
                { throw new ArgumentNullException(nameof(value)); }
                if (value.Length == 0)
                { throw new ArgumentException(string.Empty, nameof(value)); }

                _credential.SecurePassword = value;
            }
        }
        //---------------------------------------------------------------------
        public static string CreateConnectStr(string serverName, string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(serverName))
            { throw new ArgumentNullException(nameof(serverName)); }
            if (string.IsNullOrWhiteSpace(userName))
            { throw new ArgumentNullException(nameof(userName)); }
            if (string.IsNullOrWhiteSpace(password))
            { throw new ArgumentNullException(nameof(password)); }

            return _CreateConnectionStr(serverName, userName, password);
        }
        //---------------------------------------------------------------------
        public string CreatePath(string partPath)
        {
            if (string.IsNullOrWhiteSpace(partPath))
            { throw new ArgumentNullException(nameof(partPath)); }

            return $"{_connectionStr}{partPath}";
        }
        public string CreatePath(params string[] partPaths)
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append(_connectionStr);

            if (partPaths.Length != 0)
            {
                const char directorySeparatorChar = DirectorySeparatorChar;
                var isFirstPartPathAppended = false;

                foreach (var partPath in partPaths.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    if (isFirstPartPathAppended)
                    { strBuilder.AppendFormat("{0}{1}", directorySeparatorChar, partPath); }
                    else
                    {
                        isFirstPartPathAppended = true;
                        strBuilder.Append(partPath);
                    }
                }
            }

            return strBuilder.ToString();
        }
        //---------------------------------------------------------------------
        public void DownloadFile(string path, string pathToLocalMachine, int bufferLength = DefaultBufferLength)
        {
            if (string.IsNullOrWhiteSpace(path))
            { throw new ArgumentNullException(nameof(path)); }
            if (string.IsNullOrWhiteSpace(pathToLocalMachine))
            { throw new ArgumentNullException(nameof(pathToLocalMachine)); }
            if (bufferLength < 0)
            { throw new ArgumentOutOfRangeException(nameof(bufferLength)); }

            _coreDownloadFile(path, pathToLocalMachine, _responseTimeOut, bufferLength);
        }
        public void UploadFile(string pathToLocalMachine, string path, int bufferLength = DefaultBufferLength)
        {
            if (string.IsNullOrWhiteSpace(pathToLocalMachine))
            { throw new ArgumentNullException(nameof(pathToLocalMachine)); }
            if (!File.Exists(pathToLocalMachine))
            { throw new FileNotFoundException(string.Empty, nameof(pathToLocalMachine)); }
            if (string.IsNullOrWhiteSpace(path))
            { throw new ArgumentNullException(nameof(path)); }
            if (bufferLength < 0)
            { throw new ArgumentOutOfRangeException(nameof(bufferLength)); }

            _coreCreateFullPathDirectory(path, _responseTimeOut);
            _coreUploadFile(pathToLocalMachine, path, _responseTimeOut, bufferLength);
        }
        public void CreateDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            { throw new ArgumentNullException(nameof(path)); }

            _coreCreateFullPathDirectory(path, _responseTimeOut);
        }
        public bool DirectoryExists(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            { throw new ArgumentNullException(nameof(path)); }

            return _coreDirectoryExists(path, _responseTimeOut);
        }
        public bool FileExists(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            { throw new ArgumentNullException(nameof(path)); }

            return _coreFileExists(path, _responseTimeOut);
        }
        public void DeleteFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            { throw new ArgumentNullException(nameof(path)); }

            _coreDeleteFile(path, _responseTimeOut);
        }
        //---------------------------------------------------------------------
    }
}