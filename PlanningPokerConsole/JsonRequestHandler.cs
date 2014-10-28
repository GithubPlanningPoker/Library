using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanningPokerConsole
{
    public static class JsonRequestHandler
    {
        public static JObject Request(string url, RequestMethods method, JObject data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data.ToString());
            byte[] responseBuffer;

            using (var client = new System.Net.WebClient())
                responseBuffer = client.UploadData(url, getMethodString(method), buffer);

            var json = JObject.Parse(Encoding.UTF8.GetString(responseBuffer));

            JToken successObj = json["success"] as JValue;
            if (successObj == null)
                throw new ApplicationException("No success property in returned json.");

            var success = successObj.Value<bool>();
            if (!success)
                throw new ApplicationException("Request error: " + json["message"]);

            return json;
        }

        private static string getMethodString(RequestMethods method)
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
    }
}
