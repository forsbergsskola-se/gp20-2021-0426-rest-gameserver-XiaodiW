using System;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace MMORPG.Types {
    [BsonIgnoreExtraElements]
    [BsonNoId]
    public class Item {
        
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationTime { get; set; }
        
        [JsonIgnore]
        [BsonIgnore] 
        public Player Player { get; set; }
        public Item(Player player,string name) {
            Id = Guid.NewGuid();
            Name = name;
            Level = 0;
            IsDeleted = false;
            CreationTime = DateTime.Now;
            Player = player;
        }
    }

}