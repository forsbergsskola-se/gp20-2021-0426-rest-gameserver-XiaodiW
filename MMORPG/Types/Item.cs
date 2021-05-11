using System;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace MMORPG.Types {

    [BsonIgnoreExtraElements]
    [BsonNoId]
    public class Item {
        
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ItemType Type{ get; set; }
        public int Level { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationTime { get; set; }

        [JsonIgnore]
        [BsonIgnore] 
        public Player Player { get; set; }
        public Item(string name, ItemType type) {
            Id = Guid.NewGuid();
            Name = name;
            Type = type;
            Level = 0;
            IsDeleted = false;
            CreationTime = DateTime.Now;
        }
    }

}