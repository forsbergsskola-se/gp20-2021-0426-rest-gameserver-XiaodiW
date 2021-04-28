using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace TinyBrowser {

    internal static class Program {
        private static void Main(string[] args) {
            const int port = 80;
            var version = "1.1";
            var exit = false;
            var localLink = string.Empty;
            var history = new List<string>();
            var historyPointer = 0;
            var newPage = true;
            var printResult = true;
            while(!exit) {
                var hostname = "acme.com";
                if(newPage) {
                    history.Add(localLink);
                    historyPointer = history.Count-1;
                }
                var targetLink = history[historyPointer];
                using var client = new TcpClient();
                var external = IsExternalLink(targetLink, out var newHost, out var newLink);
                if(external) {
                    hostname = newHost;
                    targetLink = newLink;
                }
                
                client.Connect(hostname, port);
                using var networkStream = client.GetStream();
                networkStream.ReadTimeout = 2000;
                using var writer = new StreamWriter(networkStream);
                var requestMeg = RequestLine(version, hostname, targetLink);

                var bytes = Encoding.UTF8.GetBytes(requestMeg);
                networkStream.Write(bytes, 0, bytes.Length);
                using var reader = new StreamReader(networkStream, Encoding.UTF8);
                var data = reader.ReadToEnd();
                // Console.WriteLine(data);
                client.Close();
                
                var title = ExtractHeadInfo(data, "Title");
                if(printResult)Console.WriteLine($"Webpage Title: {title}");
                if(printResult)Console.WriteLine("Links");
                var links = ExtractHyperLinks(data);
                for(int i = 0; i < links.Count; i++) 
                    if(printResult)Console.WriteLine($"{(i + ":").PadRight(3)} {links[i][0].TrimStart().TrimEnd()} ({links[i][1]})");
                Console.WriteLine("Type in 'q' to exit!");
                var s = Console.ReadLine();
                int linksIndex;
                bool isNumber = int.TryParse(s, out linksIndex);
                if(linksIndex > links.Count - 1) s = "r";
                newPage = isNumber;
                printResult = !Equals(s, "h");
                switch(s) {
                    default:
                        if(isNumber) localLink = links[linksIndex][1];
                        break;
                    case "h":
                        for(var i = 0; i < history.Count; i++) {
                            var pointer = historyPointer == i ? "==>" : "   ";
                            Console.WriteLine($"{pointer} History({i}): {history[i]}");
                        }
                        break;
                    case "b":
                        if(historyPointer > 0) historyPointer--;
                        break;
                    case "f":
                        if(history.Count - 1 > historyPointer) historyPointer++;
                        break;
                    case "r": break;
                }
                exit = Equals(s, "q") | Equals(s, "Q");
            }
        }

        static List<string[]> ExtractHyperLinks(string html) {
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
            var regex = new Regex("<a href=[\"|'](?<link>.*?)[\"|'].*?>(<b>|<img.*?>)?(?<name>.*?)(</b>)?</a>", 
                RegexOptions.None);
            if(regex.IsMatch(html))
                foreach(Match match in regex.Matches(html))
                    list.Add(new []{match.Groups["name"].Value, match.Groups["link"].Value});
            return list;
        }

        private static string ExtractHeadInfo(string data, string info) {
            return Regex.Match(data, $"<{info}>(?<Info>.*?)</{info}>", RegexOptions.IgnoreCase).Groups["Info"].Value;
        }

        private static string RequestLine(string v, string host, string localLink) {
            var requestMeg = $"GET /{localLink}";
            switch(v) {
                case "0.9":
                    requestMeg += "\r\n";
                    break;
                case "1.1":
                    requestMeg += $" HTTP/{v}" + 
                                  "\r\nAccept: text/html, charset=utf-8" + 
                                  "\r\nAccept-Language: en-US" +
                                  "\r\nUser-Agent: C# program" + 
                                  "\r\nConnection: close" + 
                                  $"\r\nHost: {host}" + "\r\n\r\n";
                    break;
            }
            return requestMeg;
        }

        private static bool IsExternalLink(string targetLink, out string host, out string localLink) {
            if(targetLink.StartsWith("//") | targetLink.StartsWith("http")) {
                host = Regex.Match(targetLink, ".*?//(?<Host>.*?)/",RegexOptions.IgnoreCase).Groups["Host"].Value;
                localLink = Regex.Match(targetLink, ".*?//.*?/(?<link>.*?)",RegexOptions.IgnoreCase).Groups["link"].Value;
                return true;
            }
            host = string.Empty;
            localLink = string.Empty;
            return false;
        }
    }

}
