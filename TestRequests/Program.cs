using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestRequests
{
    internal class Program
    {
        private enum RequestType
        {
            POST,
            GET,
            PATCH
        }

        private static string ClientID = "";
        private static string OAuth = "";

        static void Main(string[] args)
        {
            Test().GetAwaiter().GetResult();
        }

        private async static Task Test()
        {
            string reply = await SendWebRequest("https://api.twitch.tv/helix/users", ClientID, OAuth, RequestType.GET);
            Console.WriteLine(reply);
        }

        private async static Task<string> SendWebRequest(string URL, string clientID, string OAuth, RequestType requestType, string body = null, bool ignoreSocket = false)
        {
            string finalResponse = string.Empty;

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = requestType.ToString();
                request.Headers.Add("Client-Id", clientID);
                request.Headers.Add("Authorization", $"Bearer {OAuth}");

                if (body != null)
                {
                    request.ContentType = "application/json";
                    using (var streamWriter = new StreamWriter(await request.GetRequestStreamAsync()))
                    {
                        streamWriter.Write(body);
                    }
                }

                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    finalResponse = reader.ReadToEnd()?.Trim();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex); }

            return finalResponse ?? string.Empty;
        }
    }
}
