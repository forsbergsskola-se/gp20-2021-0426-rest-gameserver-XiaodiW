namespace MMORPG.Types.Player {

    public class ModifiedPlayer
    {
        public int Score { get; set; }
        public int Level { get; set; }
        public int Gold { get; set; }
        
        public int Experience { get; set; }

        public ModifiedPlayer() {
            Score = -1;
            Level = -1;
            Gold = -1;
            Experience = -1;
        }
    }

}