using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace TinyBrowser {

    internal static class Program {
        private static void Main(string[] args) {
            using var client = new TcpClient();

            const string hostname = "marc-zaku.de";
            const int port = 80;
            const string version = "1.1";
            client.Connect(hostname, port);

            using var networkStream = client.GetStream();
            networkStream.ReadTimeout = 2000;

            using var writer = new StreamWriter(networkStream);
            
            var requestMeg = $"GET / HTTP/{version}" 
                             + "\r\nAccept: text/html, charset=utf-8" 
                             + "\r\nAccept-Language: en-US" 
                             + "\r\nUser-Agent: C# program" 
                             + "\r\nConnection: close" 
                             + $"\r\nHost: {hostname}" + "\r\n\r\n";
            
            var bytes = Encoding.UTF8.GetBytes(requestMeg);
            networkStream.Write(bytes, 0, bytes.Length);
            using var reader = new StreamReader(networkStream, Encoding.UTF8);
            var data = reader.ReadToEnd();
            // Console.WriteLine(data);
            var title = Regex.Match(data, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
                RegexOptions.IgnoreCase).Groups["Title"].Value;
            Console.WriteLine($"Webpage Title: {title}");
            // var hyperLinks = Regex.Match(data, @"/(?:ht|f)tps?\:\/\/[a-zA-Z0-9\-.]+\.[a-zA-Z]{2,3}(\/\S*)?/g",
            //     RegexOptions.IgnoreCase).Groups["Title"].Value;
            Console.WriteLine("Links");
            foreach(var link in ExtractHyperLinks(data)) Console.WriteLine(link);

            List<string> ExtractHyperLinks(string html) {
                var charsToRemove = new [] { "\n", "\r"};
                foreach (var c in charsToRemove)
                    html = html.Replace(c, string.Empty);
                
                List<string> list = new List<string>();

                var regex = new Regex("<a href=[\"|']([\\w].*?)[\"|'].*?([\\w|\\s|\\x2E]*?)</a>", RegexOptions.None);
                if(regex.IsMatch(html))
                    foreach(Match match in regex.Matches(html))
                        list.Add(match.Groups[2].Value.TrimStart(' ').TrimEnd(' ').PadRight(21) + "   " + match.Groups[1].Value);

                return list;
            }
        }
        
    }

}
