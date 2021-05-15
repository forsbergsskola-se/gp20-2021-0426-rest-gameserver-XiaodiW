using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace MMORPG.Types.Quest {

    [BsonIgnoreExtraElements]
    [BsonNoId]
    public class Quest {
        public const int GetQuestInterval = 60;
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public Item.Item Item { get; set; }
        public int Gold { get; set; }
        public bool RewardGold { get; set; }
        public DateTime CreateTime { get; set; }
        public int ExpiredTime { get; set; }
        [EnumDataType(typeof(QuestStatus))]  public QuestStatus Status { get; set; }

        public Quest(int playerLevel) {
            Id = Guid.NewGuid();
            var minLevel = playerLevel - 3 < 0 ? 0 : playerLevel - 3;
            var maxLevel = playerLevel + 2;
            var rnd = new Random(DateTime.Now.Millisecond);
            Level = rnd.Next(minLevel, maxLevel);
            Experience = rnd.Next(1, 20) * Math.Max(1,Level);
            RewardGold = rnd.NextDouble() >= 0.5;
            Gold = RewardGold? rnd.Next(1, 5) * Math.Max(1,Level): 0;
            Item = !RewardGold? new Item.Item(true): null;
            CreateTime = DateTime.Now;
            ExpiredTime = rnd.Next(60, 2000) * Math.Max(1,Level);
            Status = QuestStatus.New;
            var str = RewardGold ? $"Gold{Gold}" : $"Item{Item.Name}";
            Name = $"Level{Level}Experience{Experience}Reward{str}";
        }

        public static List<Quest> GetQuests(Player.Player player) {
            //GEtQuest can only be trigger after a const interval;
            var lastGetElapse = (DateTime.Now - player.LastGetQuests).TotalSeconds;
            if(lastGetElapse < Quest.GetQuestInterval) return player.Quests;
            var questsList = player.Quests.ToList();
            var removeList = questsList.Where(quest => quest.Status != QuestStatus.Done).ToList();
            foreach(var quest in removeList) questsList.Remove(quest);

            var rnd = new Random(DateTime.Now.Millisecond);
            var count = rnd.Next(1, 5);
            while(count >0) {
                count--;
                questsList.Add(new Quest(player.Level));
            }
            return questsList;
        }
    }

}