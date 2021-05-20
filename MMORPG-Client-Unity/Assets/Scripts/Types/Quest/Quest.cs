using System;

namespace Types.Quest {

    public class Quest {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public Item.Item Item { get; set; }
        public int Gold { get; set; }
        public bool RewardGold { get; set; }
        public DateTime CreateTime { get; set; }
        public int ExpiredTime { get; set; }
        public QuestStatus Status { get; set; }
    }

}