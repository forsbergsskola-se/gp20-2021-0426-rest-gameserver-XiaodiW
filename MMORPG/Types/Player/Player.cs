using System;
using System.Collections.Generic;
using MMORPG.Validation;
using MongoDB.Bson.Serialization.Attributes;

namespace MMORPG.Types {
    [BsonIgnoreExtraElements]
    [BsonNoId]
    public class Player
    {

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; }
        public int Gold { get; set; }
        public bool IsDeleted { get; set; }
        
        [DateValidation]
        public DateTime CreationTime { get; set; }
        public List<Item> Items { get; set; }
        
        public List<string> Tag { get; set; }
        
        public Player(string name) {
            Id = Guid.NewGuid();
            Name = name;
            Score = 0;
            Level = 0;
            Gold = 0;
            IsDeleted = false;
            CreationTime = DateTime.Now;
            Items = new List<Item>();
        }

        public Player() : this(string.Empty) {
            Id = Guid.Empty;
            CreationTime = DateTime.UnixEpoch;
        }
    }

}