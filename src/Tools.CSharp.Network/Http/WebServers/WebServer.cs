using System;
using System.Net;
using System.Threading;

namespace Tools.CSharp.Network.Http
{
    //-------------------------------------------------------------------------
    //https://codehosting.net/blog/BlogEngine/post/Simple-C-Web-Server.aspx
    public class WebServer
    {
        #region private
        private readonly HttpListener _listener = new HttpListener();
        private readonly WebServerCallback _callback;
        //---------------------------------------------------------------------
        private WebServer(string[] prefixes, WebServerCallback callback)
        {
            if (!HttpListener.IsSupported)
            { throw new NotSupportedException("Needs Windows XP SP2, Server 2003 or later."); }

            if (prefixes == null || prefixes.Length == 0)
            { throw new ArgumentException(nameof(prefixes)); }

            foreach (var prefix in prefixes)
            { _listener.Prefixes.Add(prefix); }

            _callback = callback;
        }
        #endregion
        #region protected
        protected virtual void DefaultCallback(HttpListenerContext context)
        { }
        #endregion
        protected WebServer(params string[] prefixes)
            : this(prefixes, null)
        { }
        public WebServer(WebServerCallback callback)
            : this(new[] { DefaultLocalPrefix }, callback)
        {
            if (callback == null)
            { throw new ArgumentException(nameof(callback)); }
        }
        public WebServer(WebServerCallback callback, params string[] prefixes)
            : this(prefixes, callback)
        {
            if (callback == null)
            { throw new ArgumentException(nameof(callback)); }
        }

        //---------------------------------------------------------------------
        public const string DefaultLocalPrefix = "http://localhost:8080/";
        //---------------------------------------------------------------------
        public bool IsStarted
        {
            get { return _listener.IsListening; }
        }
        //---------------------------------------------------------------------
        public void Start()
        {
            if (_listener.IsListening)
            { throw new InvalidOperationException(); }

            _listener.Start();

            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem(callBack =>
                            {
                                var context = (HttpListenerContext)callBack;
                                if (_callback == null)
                                { DefaultCallback(context); }
                                else
                                { _callback(context); }
                            }, _listener.GetContext()
                        );
                    }
                }
                catch { }
            });
        }
        //---------------------------------------------------------------------
        public void Stop()
        {
            if (_listener.IsListening)
            {
                _listener.Stop();
                _listener.Close();
            }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    public delegate void WebServerCallback(HttpListenerContext context);
    //-------------------------------------------------------------------------
}
