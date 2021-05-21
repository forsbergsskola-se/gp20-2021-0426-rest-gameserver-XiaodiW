using Types.Player;

namespace Events {

    public class UpdatePlayerEvent {
        public Player Player;

        public UpdatePlayerEvent(Player player) {
            Player = player;
        }
        
    }

}
