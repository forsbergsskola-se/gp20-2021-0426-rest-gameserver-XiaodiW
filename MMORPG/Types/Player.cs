using System;

namespace MMORPG.Types {

    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public int Level { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationTime { get; set; }

        public Player(string name) {
            Id = Guid.NewGuid();
            Name = name;
            Score = 0;
            Level = 0;
            IsDeleted = false;
            CreationTime = DateTime.Now;
        }
    }

}