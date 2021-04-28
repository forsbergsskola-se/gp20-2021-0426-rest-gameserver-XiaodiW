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

            const string hostname = "acme.com";
            const int port = 80;
            string version = "1.1";
            client.Connect(hostname, port);

            using var networkStream = client.GetStream();
            networkStream.ReadTimeout = 2000;

            using var writer = new StreamWriter(networkStream);

            var requestMeg = $"GET /";
            switch(version) {
                case "0.9": requestMeg += "\r\n";
                    break;
                case "1.1":
                    requestMeg += $" HTTP/{version}" 
                                  + "\r\nAccept: text/html, charset=utf-8" 
                                  + "\r\nAccept-Language: en-US" 
                                  + "\r\nUser-Agent: C# program" 
                                  + "\r\nConnection: close" 
                                  + $"\r\nHost: {hostname}" + "\r\n\r\n";
                    break;

            }

            var bytes = Encoding.UTF8.GetBytes(requestMeg);
            networkStream.Write(bytes, 0, bytes.Length);
            using var reader = new StreamReader(networkStream, Encoding.UTF8);
            var data = reader.ReadToEnd();
            // Console.WriteLine(data);
            var title = Regex.Match(data, "<title>(?<Title>.*?)</title>",
                RegexOptions.IgnoreCase).Groups["Title"].Value;
            Console.WriteLine($"Webpage Title: {title}");

            Console.WriteLine("Links");
            var links = ExtractHyperLinks(data);
            for(int i = 0; i < links.Count; i++) {
                Console.WriteLine($"{(i + ":").PadRight(3)} {links[i][0].TrimStart().TrimEnd()} ({links[i][1]})");
            }
            
            List<string[]> ExtractHyperLinks(string html) {
                //Remove all new Lines and carriage return characters
                var charsToRemove = new [] { "\n", "\r"};
                foreach (var c in charsToRemove)
                    html = html.Replace(c, string.Empty);
                
                List<string[]> list = new ();
                //[\"|'] => a single character is " or '
                //() Select in () as indexed Group, or give name to each group (?<GroupName> exp);
                //(?<link>.*?) Named Group
                //.*? => any character (without \n) 0 or more times (Lazy mode), 
                //[\\w|\\s|\\x2E]*? => accept word or space or "." 0 or more times. \\x2E = ASCII '.'
                // \\s* ignore 0 or more white-space
                //(<b>|<img.*?>)? ignore <b> or <img .... > 0 or 1 time;
                //(</b>)? ignore </b> 0 or 1 time;
                var regex = new Regex("<a href=[\"|'][/]*(?<link>.*?)[\"|'].*?>(<b>|<img.*?>)?(?<name>.*?)(</b>)?</a>", 
                    RegexOptions.None);
                if(regex.IsMatch(html))
                    foreach(Match match in regex.Matches(html))
                        list.Add(new []{match.Groups["name"].Value, match.Groups["link"].Value});
                return list;
            }
        }
        
    }

}
