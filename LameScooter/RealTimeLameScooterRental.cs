using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LameScooter {

    public class RealTimeLameScooterRental : ILameScooterRental {
        private async Task<List<StationData>> ReadOffineFile() {
            var url = new Uri("https://raw.githubusercontent.com/marczaku/GP20-2021-0426-Rest-Gameserver/main/assignments/scooters.json");
            try {
                HttpClient client = new();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.UserAgent.TryParseAdd("Xiaodi");
                var requestMeg = new HttpRequestMessage {RequestUri = url};
                requestMeg.Method = HttpMethod.Get;
                // Console.WriteLine(requestMeg.ToString());
                var received = client.SendAsync(requestMeg);
                Console.WriteLine($"Server response status: {received.Result.StatusCode.ToString()}");
                var stream = await received.Result.Content.ReadAsStreamAsync();
                var reader = new StreamReader(stream);
                string data = await reader.ReadToEndAsync();
                // Console.WriteLine(data);
                client.Dispose();
                var result = JsonSerializer.Deserialize<OnlineData>(data).stations;
                // var result = JsonSerializer.Deserialize<List<StationData>>(data);
                return result;
            }
            catch(IOException e) {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public string Method { get; }

        public RealTimeLameScooterRental() {
            Method = "RealTime";
        }

        public Task<int> GetScooterCountInStation(string stationName) {
            try {
                var data = ReadOffineFile().Result.Where(a => a.name == stationName);
                var station = data as StationData[] ?? data.ToArray();
                if(!station.Any()) throw new InvalidDataException($"{stationName} is not Valid");
                var result = station.Select(a => a.bikesAvailable).FirstOrDefault();
                return Task.FromResult(result);
            }
            catch(InvalidDataException e) {
                Console.WriteLine(e.ToString());
                throw;
            }
        }
    }

}