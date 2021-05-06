using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LameScooter {
    [BsonIgnoreExtraElements]
    [BsonNoId]
    public class StationData {
        public string id { get; set; }
        public string name { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public int bikesAvailable { get; set; }
        public int spacesAvailable { get; set; }
        public int capacity { get; set; }
        public bool allowDropoff { get; set; }
        public bool allowOverloading { get; set; }
        public bool isFloatingBike { get; set; }
        public bool isCarStation { get; set; }
        public string state { get; set; }
        public string[] networks { get; set; }
        public bool realTimeData { get; set; }

        public override string ToString() {
            return $"Station {name} has valid bikes of {bikesAvailable}.";
        }
    }

}