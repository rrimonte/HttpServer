using System;
using System.IO;
using System.Net;
using System.Text;

namespace HttpServer
{
    public static class HttpExt
    {
        public static string GetBody(this HttpListenerRequest req)
        {
            if (!req.HasEntityBody)
                return null;

            using (var str = new StreamReader(req.InputStream))
            {
                return str.ReadToEnd();
            }
        }

        public static void WriteBody(this HttpListenerResponse res, string input, Encoding encoding)
        {
            var bytes = encoding.GetBytes(input);
            res.OutputStream.Write(bytes, 0, bytes.Length);
        }

        public static void WriteBody(this HttpListenerResponse res, string input)
        {
            WriteBody(res, input, Encoding.UTF8);
        }
    }
}
