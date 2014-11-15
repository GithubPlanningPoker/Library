using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace Library
{
    public class JsonRequestHandler
    {
        private readonly string rootURL;

        public JsonRequestHandler(string rootURL)
        {
            this.rootURL = rootURL.TrimEnd('/');
        }

        public JObject Request(string url, RequestMethods method, JObject data, bool ignoreError = false)
        {
            return Request(url, method, data.ToString(), ignoreError);
        }
        public JObject Request(string url, RequestMethods method, string data, bool ignoreError = false)
        {
            byte[] response = getReponse(rootURL + url, method, data);

            if (response.Length != 0)
                return JObject.Parse(Encoding.UTF8.GetString(response));
            else return null;
        }
        public JObject Request(string url, RequestMethods method, bool ignoreError = false)
        {
            return Request(url, method, (string)null, ignoreError);
        }

        private string getMethodString(RequestMethods method)
        {
            switch (method)
            {
                case RequestMethods.GET: return "GET";
                case RequestMethods.PUT: return "PUT";
                case RequestMethods.POST: return "POST";
                case RequestMethods.DELETE: return "DELETE";
                default:
                    throw new ArgumentException("Unknown request method.");
            }
        }
        private byte[] getReponse(string url, RequestMethods method, string data)
        {
            byte[] buffer = data == null ? new byte[0] : Encoding.UTF8.GetBytes(data);
            byte[] responseBuffer = new byte[0];

            HttpWebRequest client = System.Net.HttpWebRequest.CreateHttp(url);
            switch (method)
            {
                case RequestMethods.GET:
                    responseBuffer = HandleWebResponse(client);
                    break;

                case RequestMethods.PUT:
                case RequestMethods.POST:
                case RequestMethods.DELETE:
                    client.ContentType = "application/json";
                    client.Method = getMethodString(method);

                    var g = client.GetRequestStream();
                    g.Write(buffer, 0, buffer.Length);

                    HttpWebResponse response = client.GetResponse() as HttpWebResponse;

                    responseBuffer = HandleWebResponse(client);

                    break;
            }

            return responseBuffer;
        }

        private static byte[] HandleWebResponse(HttpWebRequest client)
        {
            byte[] responseBuffer = new byte[0];
            HttpWebResponse r = client.GetResponse() as HttpWebResponse;
            var g = r.StatusCode;

            if (g == HttpStatusCode.OK || g == HttpStatusCode.Created)
            {
                MemoryStream ms = new MemoryStream();
                r.GetResponseStream().CopyTo(ms);
                responseBuffer = ms.ToArray();
            }
            else
            {
                throw new WebException((r as HttpWebResponse).StatusDescription);
            }
            return responseBuffer;
        }
    }
}
