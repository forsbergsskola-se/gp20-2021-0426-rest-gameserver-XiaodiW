using System;
using System.IO;
using System.Text.Json;

namespace GitHubExplorer.Security {

    public class Secrets {
        public string Token { get; set; }
        private static readonly string FileName = Path.Combine(Environment.CurrentDirectory, "secrets.json");

        public Secrets(string token) {
            Token = token;
        }

        private static Secrets LoadAndValidateSectets() {
            Secrets secrets;
            if(!File.Exists(FileName)) {
                var token = Console.ReadLine();
                secrets = new Secrets(token);
                File.WriteAllText(FileName, JsonSerializer.Serialize(secrets));
            } else { secrets = JsonSerializer.Deserialize<Secrets>(File.ReadAllText(FileName)); }
            if(string.IsNullOrEmpty(secrets.Token)) {
                Console.WriteLine("Error:");
                throw new Exception($"Error: Need to define a Token in file {FileName}.");
            }
            return secrets;
        }
    }

}