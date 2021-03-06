using System;
using System.Collections.Generic;
using MMORPG.Validation;
using MongoDB.Bson.Serialization.Attributes;


namespace MMORPG.Types.Player {
    [BsonIgnoreExtraElements]
    [BsonNoId]
    public class Player
    {

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public int Score { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; }
        public int Gold { get; set; }
        public bool IsDeleted { get; set; }
        
        [DateValidation]
        public DateTime CreationTime { get; private set; }
        public List<Item.Item> Items { get; set; }
        
        public List<string> Tag { get; set; }
        public List<Quest.Quest> Quests { get; set; }
        public DateTime LastGetQuests { get; set; }
        
        public Player(string name) {
            Id = Guid.NewGuid();
            Name = name;
            Score = 0;
            Level = 0;
            Gold = 0;
            IsDeleted = false;
            CreationTime = DateTime.UtcNow;
            Items = new List<Item.Item>();
            Quests = new List<Quest.Quest>();
        }

        public Player() : this(string.Empty) {
            Id = Guid.Empty;
            CreationTime = DateTime.UnixEpoch;
        }
    }

}