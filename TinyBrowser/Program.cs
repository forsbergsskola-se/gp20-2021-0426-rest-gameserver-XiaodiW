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
            var hostname = "acme.com";
            var url = $"http://{hostname}/";
            var history = new List<string>();
            var historyPointer = 0;
            var newPage = true;
            var isPrintResults = true;
            
            while(!exit) {
                if(newPage) {
                    history.Add(url);
                    historyPointer = history.Count-1;
                }
                var targetUrl = history[historyPointer];

                var isExternalUrl = IsExternalLink(targetUrl, out var newHost, out var newUrl);
                if(isExternalUrl) {
                    hostname = newHost;
                    targetUrl = newUrl;
                }
                
                using var client = new TcpClient();
                client.ReceiveTimeout = 2000;
                client.Connect(hostname, port);
                using var networkStream = client.GetStream();
                networkStream.ReadTimeout = 2000;
                using var writer = new StreamWriter(networkStream);
                
                var requestMeg = RequestLine(version, hostname, targetUrl);

                var bytes = Encoding.UTF8.GetBytes(requestMeg);
                networkStream.Write(bytes, 0, bytes.Length);
                using var reader = new StreamReader(networkStream, Encoding.UTF8);
                var data = reader.ReadToEnd();
                // Console.WriteLine(data);
                client.Close();
                
                var title = ExtractHeading(data, "Title");
                if(isPrintResults)Console.WriteLine($"Webpage Title: {title}");
                if(isPrintResults)Console.WriteLine("Links");
                var urls = ExtractHyperLinks(data);
                
                for(int i = 0; i < urls.Count; i++) {
                    if(isPrintResults)
                        Console.WriteLine(
                            $"{(i + ":").PadRight(3)} {PrettifyPrint(urls[i][0].TrimStart().TrimEnd())} ({urls[i][1]})");
                    urls[i][1] = NormalizeUrl(urls[i][1], hostname);
                }                
                Console.WriteLine("What would you want to continue? line number(Goto the link in the list) b?(Backward) f?(Forward) h?(History) g?(Goto any Link) q?(Quit)");
                var s = Console.ReadLine();
                //Check if the user type in a number.
                bool isNumber = int.TryParse(s, out var linksIndex);
                //Refresh the page if user typed number is out of boundary. 
                if(isNumber && linksIndex > urls.Count - 1) s = "r";
                //If User chosen option is a number, then a newpage will be added in history
                newPage = isNumber;
                //if user chosen option is not "History", need to print all Results
                isPrintResults = !Equals(s, "h");
                switch(s) {
                    default:
                        if(isNumber) url = urls[linksIndex][1];
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
                    case "g": Console.WriteLine("Please type in a URL. should be like 'http://www.google.com/'");
                        url = Console.ReadLine();
                        newPage = true;
                        break;
                    case "r": 
                        break;
                    case "q": exit = true;
                        break;
                }
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

        private static string ExtractHeading(string data, string info) {
            return Regex.Match(data, $"<{info}>(?<Info>.*?)</{info}>", RegexOptions.IgnoreCase).Groups["Info"].Value;
        }

        private static string RequestLine(string version, string host, string url) {
            var requestMeg = $"GET {url}";
            switch(version) {
                case "0.9":
                    requestMeg += "\r\n";
                    break;
                case "1.1":
                    requestMeg += $" HTTP/{version}" + 
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
            host = string.Empty;
            localLink = targetLink;
            if(targetLink.Contains("//")) {
                host = Regex.Match(targetLink, "//(www.|WWW.)?(?<host>.*?)/",RegexOptions.IgnoreCase).Groups["host"].Value;
                if(targetLink.StartsWith("//")) 
                    localLink = localLink.Remove(0, 2);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Convert all the returned urls into form like http://Hostname/subUrl or //www.hostname/subUrl.
        /// </summary>
        /// <param name="targetLink"></param>
        /// <param name="hostName"></param>
        /// <returns></returns>
        private static string NormalizeUrl(string targetLink, string hostName) {
            var tar = targetLink.StartsWith("/") ? targetLink : "/" + targetLink;
            if(!(targetLink.StartsWith("//") | targetLink.StartsWith("http"))) 
                return $"http://{hostName}{tar}";
            return targetLink;
        }

        private static string PrettifyPrint(string s) {
            if(s.Length > 15)
                s = string.Concat(s.Substring(0, 6), "...", s.Substring(s.Length - 7, 6));
            else s = s.PadRight(15);
            return s;
        }
    }

}
