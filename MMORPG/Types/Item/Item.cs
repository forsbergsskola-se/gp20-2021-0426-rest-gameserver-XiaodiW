using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.Forms;
using MMORPG.Validation;
using MongoDB.Bson.Serialization.Attributes;

namespace MMORPG.Types.Item {

    [BsonIgnoreExtraElements]
    [BsonNoId]
    public class Item {
        public Guid Id { get; set; }
        public string Name { get; set; }

        [EnumDataType(typeof(ItemType))] public ItemType Type { get; set; }
        [EnumDataType(typeof(ItemType))] public ItemRarity Rarity { get; set; }

        public int Price { get; set; }
        [Range(0, 99)] public int LevelRequired { get; set; }
        
        [Range(0, 99)] public int LevelBonus { get; set; }

        public bool IsDeleted { get; set; }
        
        public bool IsEquipped{ get; set; }

        [DateValidation] public DateTime CreationTime { get; set; }

        [JsonIgnore] [BsonIgnore] public Player.Player Player { get; set; }

        public Item() {
            Id = Guid.NewGuid();
            var rnd = new Random();
            var maxType = Enum.GetValues(typeof(ItemType)).Cast<int>().Max();
            Type = (ItemType)rnd.Next(0,maxType);
            var maxRarity = Enum.GetValues(typeof(ItemType)).Cast<int>().Max();
            Rarity = (ItemRarity)rnd.Next(0,maxRarity);
            Price = ((int) Rarity + 1) * 100 - rnd.Next(0, 100);
            LevelRequired = ((int)Rarity+1)*10-rnd.Next(0,10);
            LevelBonus = ((int) Rarity + 1) * rnd.Next(1, 3);
            Name = $"{LevelRequired}{Rarity}{Type}{LevelBonus}";
            IsDeleted = false;
            IsEquipped = false;
            CreationTime = DateTime.Now;
        }
    }

}