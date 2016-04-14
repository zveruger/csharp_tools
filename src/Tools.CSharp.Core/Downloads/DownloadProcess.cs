using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.Downloads
{
    public sealed class DownloadProcess
    {
        #region private
        private const int _defaultReadBufferLength = 1024 * 75;
        private readonly int _readBufferLength;
        //-----------------------------------------------------------------------------------
        private void _downloadFile(string path, int readBufferLength)
        {
            const int maxProgressPercent = 100;

            using(var worker = new BackgroundWorker())
            {
                worker.WorkerReportsProgress = true;

                worker.DoWork += (o, e) =>
                {
                    double completeProgressPercent = maxProgressPercent;

                    if (!File.Exists(path))
                    {
                        worker.ReportProgress(0, new DownloadFileProgressEventArgs(string.Empty, completeProgressPercent));
                        return;
                    }

                    var readBuffer = new char[readBufferLength];
                    using(var stream = File.Open(path, FileMode.Open))
                    {
                        using(var streamReader = new StreamReader(stream, true))
                        {
                            var fileInfo = new FileInfo(path);
                            var fileLenght = fileInfo.Length;
                            long bytesRead = 0;

                            while(!streamReader.EndOfStream)
                            {
                                var readLength = readBufferLength;

                                if ((fileLenght - bytesRead) < readLength)
                                {
                                    readLength = (int)(fileLenght - bytesRead);
                                    completeProgressPercent = maxProgressPercent;
                                }
                                else
                                { completeProgressPercent = (bytesRead / (double)fileLenght) * 100; }

                                var readBlockLength = streamReader.Read(readBuffer, 0, readLength);
                                bytesRead += readBlockLength;

                                var currentLine = readBlockLength != 0 ? new string(readBuffer, 0, readBlockLength).Replace("\n", string.Empty) : string.Empty;
                                worker.ReportProgress(0, new DownloadFileProgressEventArgs(currentLine, completeProgressPercent));
                                Thread.Sleep(1000);
                            }
                        }
                    }
                };
                worker.ProgressChanged += (o, e) =>
                {
                    var downloadFileProgressEventArgs = (DownloadFileProgressEventArgs)e.UserState;
                    _downloadFileProgressRaise(downloadFileProgressEventArgs);
                };

                worker.RunWorkerAsync();
            }
        }

        //-----------------------------------------------------------------------------------
        private void _downloadFileProgressRaise(DownloadFileProgressEventArgs e)
        {
            e.Raise(this, ref DownloadFileProgress);
        }
        #endregion
        public DownloadProcess()
            : this(_defaultReadBufferLength)
        {
        }
        public DownloadProcess(int readBufferLength)
        {
            _readBufferLength = readBufferLength;
        }

        //---------------------------------------------------------------------
        public static int DefaultReadBufferLength
        {
            get { return _defaultReadBufferLength; }
        }
        //---------------------------------------------------------------------
        public void DownloadFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            _downloadFile(path, _readBufferLength);
        }
        //---------------------------------------------------------------------
        public event EventHandler<DownloadFileProgressEventArgs> DownloadFileProgress;
        //---------------------------------------------------------------------
    }
}