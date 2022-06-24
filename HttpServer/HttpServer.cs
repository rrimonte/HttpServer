using System;
using System.Collections.Generic;
using System.Net;

namespace HttpServer
{
    public class HttpServer
    {
        private class EndpointHandler
        {
            public string method;
            public string path;
            public Action<HttpListenerRequest, HttpListenerResponse> callback;
        }

        private HttpListener _listener;
        private List<EndpointHandler> handlers = new List<EndpointHandler>();

        public HttpServer()
        {

        }

        public void Listen(string ip, int port)
        {
            if (_listener != null)
                throw new ArgumentException("Server is already listening...");

            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://{ip}:{port}/");

            _listener.Start();

            _listener.BeginGetContext(ReadContext, _listener);
        }

        public void Listen(int port)
        {
            Listen("0.0.0.0", port);
        }

        public void Close()
        {
            _listener.Stop();
            _listener.Close();
            _listener = null;
        }

        private void ReadContext(IAsyncResult result)
        {
            var listener = (HttpListener)result.AsyncState;
            var context = listener.EndGetContext(result);

            // Handle result here
            try
            {
                bool found = false;
                foreach (var endpoint in handlers)
                {
                    if (endpoint.method == context.Request.HttpMethod && endpoint.path == context.Request.Url.AbsolutePath)
                    {
                        endpoint.callback?.Invoke(context.Request, context.Response);
                        found = true;
                    }
                }
                if (!found)
                {
                    context.Response.StatusCode = 404;
                    context.Response.Close();
                }
            }
            finally
            {
                listener.BeginGetContext(ReadContext, listener);
            }

        }

        public void AddEndpoint(string method, string path, Action<HttpListenerRequest, HttpListenerResponse> callback)
        {
            handlers.Add(new EndpointHandler { method = method, path = path, callback = callback });
        }

        public void RemoveEndPoint(string method, string path, Action<HttpListenerRequest, HttpListenerResponse> callback)
        {
            handlers.RemoveAll(x => x.method == method && x.path == path && x.callback == callback);
        }
    }
}
