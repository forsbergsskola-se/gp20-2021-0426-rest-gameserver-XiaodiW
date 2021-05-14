using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MMORPG.Validation;
using MongoDB.Bson.Serialization.Attributes;

namespace MMORPG.Types.Item {

    [BsonIgnoreExtraElements]
    [BsonNoId]
    public class Item {
        public Guid Id { get; set; }
        public string Name { get; set; }

        [EnumDataType(typeof(ItemType))] public ItemType Type { get; set; }

        [Range(0, 99)] public int Level { get; set; }
        public bool IsDeleted { get; set; }

        [DateValidation] public DateTime CreationTime { get; set; }

        [JsonIgnore] [BsonIgnore] public Player.Player Player { get; set; }

        public Item(string name, ItemType type) {
            Id = Guid.NewGuid();
            Name = name;
            Type = type;
            Level = 0;
            IsDeleted = false;
            CreationTime = DateTime.Now;
        }

        public Item() : this(string.Empty, ItemType.Potion) {
            Id = Guid.Empty;
            CreationTime = DateTime.UnixEpoch;
        }

        public Item(bool ramdom) : this(string.Empty, ItemType.Potion) {}
    }

}