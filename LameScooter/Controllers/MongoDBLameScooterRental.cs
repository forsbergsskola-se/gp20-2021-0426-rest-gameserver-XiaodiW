using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LameScooter.Interface;
using LameScooter.Type;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace LameScooter.Controllers {

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
            catch(MongoException e) {
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
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }

}