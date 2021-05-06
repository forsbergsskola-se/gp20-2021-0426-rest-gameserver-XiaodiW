using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace LameScooter {

    public class MongoDBLameScooterRental : ILameScooterRental {
        private async Task<List<StationData>> ReadOffineFile() {
            try {
                var client = new MongoClient("mongodb://localhost:27017");
                var database = client.GetDatabase("lamescooters");
                var collection = database.GetCollection<BsonDocument>("lamescooters");
                var documents = await collection.Find(new BsonDocument()).ToListAsync();
                var result =  documents.Select(bsonDocument => BsonSerializer.Deserialize<StationData>(bsonDocument)).ToList();
                return result;
            }
            catch(IOException e) {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public string Method { get; }

        public MongoDBLameScooterRental() {
            Method = "MongoDB";
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