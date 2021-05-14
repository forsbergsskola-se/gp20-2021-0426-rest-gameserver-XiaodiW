using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GitHubExplorer.Comm {

    public abstract class RestApi {
        protected Uri Url;
        
        protected static async Task<string> ApiRequest(Uri url, HttpMethod method, object obj) {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("MMORPG/CLient/v1");
            var requestMeg = new HttpRequestMessage {RequestUri = url};
            requestMeg.Method = method;
            HttpResponseMessage httpResponse = null;
            if(method == HttpMethod.Post) {
                var requestContent =
                    new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
                httpResponse = await client.PostAsync(url, requestContent);
            }
            if(method == HttpMethod.Get) httpResponse = await client.SendAsync(requestMeg);
            // Console.WriteLine(requestMeg.ToString());
            
            // Console.WriteLine($"Server response status: {httpResponse.StatusCode.ToString()}");
            var stream = await httpResponse.Content.ReadAsStreamAsync();
            var reader = new StreamReader(stream);
            string data = await reader.ReadToEndAsync();
            // Console.WriteLine(data);
            client.Dispose();
            return data;
        }
    }

}