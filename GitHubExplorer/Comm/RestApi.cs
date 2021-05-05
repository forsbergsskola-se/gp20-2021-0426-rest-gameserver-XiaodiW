using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using GitHubExplorer.Security;

namespace GitHubExplorer.Comm {

    public abstract class RestApi {
        private static string Token => MicroSoftSecretsManager.LoadSecret("github-token");
        protected Uri Url;
        
        protected static async Task<string> DoComm(Uri url, HttpMethod method, object obj) {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("Github Explorer");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", Token);
            var requestMeg = new HttpRequestMessage {RequestUri = url};
            requestMeg.Method = method;
            if(obj != null) requestMeg.Content = new StringContent(JsonSerializer.Serialize(obj));
            // Console.WriteLine(requestMeg.ToString());
            var received = client.SendAsync(requestMeg);
            
            Console.WriteLine($"Server response status: {received.Result.StatusCode.ToString()}");
            var stream = await received.Result.Content.ReadAsStreamAsync();
            var reader = new StreamReader(stream);
            string data = await reader.ReadToEndAsync();
            // Console.WriteLine(data);
            client.Dispose();
            return data;
        }
    }

}