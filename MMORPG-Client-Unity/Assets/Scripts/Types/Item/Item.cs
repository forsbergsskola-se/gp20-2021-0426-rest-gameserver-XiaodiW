using System;
using Newtonsoft.Json;

namespace Types.Item {

    public class Item {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ItemType Type { get; set; }
        public ItemRarity Rarity { get; set; }

        public int Price { get; set; }
        public int LevelRequired { get; set; }

        public int LevelBonus { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsEquipped { get; set; }

        public DateTime CreationTime { get; set; }
        
        [JsonIgnore]
        public Player.Player Player { get; set; }
    }

}