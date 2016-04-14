using System;

namespace Tools.CSharp.Downloads
{
    public sealed class DownloadFileProgressEventArgs : EventArgs
    {
        #region private
        private readonly string _text;
        private readonly double _progressPercent;
        #endregion
        internal DownloadFileProgressEventArgs(string text, double progressPercent)
        {
            _text = text;
            _progressPercent = progressPercent;
        }

        //---------------------------------------------------------------------
        public string Text
        {
            get { return _text; }
        }
        //---------------------------------------------------------------------
        public double ProgressPercent
        {
            get { return _progressPercent; }
        }
        //---------------------------------------------------------------------
    }
}