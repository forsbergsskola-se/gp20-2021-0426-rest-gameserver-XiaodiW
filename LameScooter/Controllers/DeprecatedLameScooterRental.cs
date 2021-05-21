using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LameScooter.Interface;
using LameScooter.Type;

namespace LameScooter.Controllers {

    public class DeprecatedLameScooterRental : ILameScooterRental {
        private List<StationData> ReadOffineFile() {
            var fileName = "scooters.txt";
            var path = Path.Combine(Environment.CurrentDirectory, @"OffineData", fileName);
            try {
                using var sr = new StreamReader(path);
                var data = sr.ReadToEnd();
                // var result = JsonSerializer.Deserialize<List<StationData>>(data);
                var result = DeprecatedDeserialized(data);
                return result;
            }
            catch(IOException e) {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public string Method { get; }

        public DeprecatedLameScooterRental() {
            Method = "Deprecated";
        }

        public Task<int> GetScooterCountInStation(string stationName) {
            try {
                var data = ReadOffineFile().Where(a => a.name == stationName);
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

        private static List<StationData> DeprecatedDeserialized(string data) {
            List<StationData> result = new();
            var regex = new Regex("(?<name>\\w*?)\\s*?:\\s*?(?<count>\\d*?)\\r\\n",RegexOptions.None);
            if(regex.IsMatch(data))
                foreach(Match match in regex.Matches(data)) {
                    StationData temp = new StationData();
                    temp.name = match.Groups["name"].Value;
                    temp.bikesAvailable = int.Parse(match.Groups["count"].Value);
                    result.Add(temp);
                }
            return result;
        }
    }

}