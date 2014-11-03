using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class JsonRequestHandler
    {
        private readonly string rootURL;

        public JsonRequestHandler(string rootURL)
        {
            this.rootURL = rootURL.TrimEnd('/');
        }

        public JObject Request(string url, RequestMethods method, JObject data)
        {
            return Request(url, method, data.ToString());
        }
        public JObject Request(string url, RequestMethods method, string data)
        {
            byte[] response = getReponse(rootURL + url, method, data);
            var json = JObject.Parse(Encoding.UTF8.GetString(response));

            JToken successObj = json["success"] as JValue;
            if (successObj == null)
                throw new ApplicationException("No success property in returned json.");

            var success = successObj.Value<bool>();
            if (!success)
                throw new ApplicationException("Request error: " + json["message"]);

            return json;
        }
        public JObject Request(string url, RequestMethods method)
        {
            return Request(url, method, (string)null);
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

            using (var client = new System.Net.WebClient())
            {
                switch (method)
                {
                    case RequestMethods.GET:
                        responseBuffer = client.DownloadData(url);
                        break;

                    case RequestMethods.PUT:
                    case RequestMethods.POST:
                    case RequestMethods.DELETE:
                        client.Headers.Add("Content-Type", "application/json");
                        responseBuffer = client.UploadData(url, getMethodString(method), buffer);
                        break;
                }
            }

            return responseBuffer;
        }
    }
}
