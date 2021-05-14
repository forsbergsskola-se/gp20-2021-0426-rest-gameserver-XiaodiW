using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace MMORPG.Types.Quest {
    [BsonIgnoreExtraElements]
    [BsonNoId]
    public class Quest {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public Item Item { get; set; }
        public int Gold { get; set; }
        
        public bool RewardGold { get; set; }
        public DateTime CreateTime { get; set; }
        public int ExpiredTime { get; set; }

        public Quest(int playerLevel) {
            var minLevel = playerLevel - 3 < 0 ? 0 : playerLevel - 3;
            var maxLevel = playerLevel + 2;
            var rnd = new Random(DateTime.Now.Millisecond);
            Level = rnd.Next(minLevel, maxLevel);
            Experience = rnd.Next(1, 20) * Level;
            RewardGold = rnd.NextDouble() >= 0.5;
            Gold = RewardGold? rnd.Next(1, 5) * Level: 0;
            Item = !RewardGold? new Item(true): null;
            CreateTime = DateTime.Now;
            ExpiredTime = rnd.Next(5, 20) * Level;
        }

        public static Quest[] CreateQuests(int playerLevel) {
            var rnd = new Random(DateTime.Now.Millisecond);
            var count = rnd.Next(1, 5);
            var temp = new List<Quest>();
            while(count >0) {
                count--;
                temp.Add(new Quest(playerLevel));
            }
            var result = temp.ToArray();
            return result;
        }
    }

}