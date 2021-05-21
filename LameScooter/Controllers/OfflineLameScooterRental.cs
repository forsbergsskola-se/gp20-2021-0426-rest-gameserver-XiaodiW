using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LameScooter.Interface;
using LameScooter.Type;

namespace LameScooter.Controllers {

    public class OfflineLameScooterRental : ILameScooterRental {
        private List<StationData> ReadOffineFile() {
            var fileName = "scooters.json";
            var path = Path.Combine(Environment.CurrentDirectory, @"OffineData", fileName);
            try {
                using var sr = new StreamReader(path);
                var data = sr.ReadToEnd();
                var result = JsonSerializer.Deserialize<List<StationData>>(data);
                return result;
            }
            catch(IOException e) {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public string Method { get; }

        public OfflineLameScooterRental() {
            Method = "Offline";
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
    }

}