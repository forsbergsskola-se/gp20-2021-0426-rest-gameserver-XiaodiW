using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Types.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public partial class GameManager : MonoBehaviour {
    private EventsBroker broker;
    private Guid PlayerId => JsonConvert.DeserializeObject<Guid>(PlayerPrefs.GetString("PlayerId"));
    public Text Name;
    public Text Level;
    public Text Gold;
    public Text Exp;


    private async void Start() {
        broker = GetComponent<EventsBroker>();
        broker.SubscribeTo<UpdatePlayerEvent>(UpdatePlayerUI);
        await UpdatePlayer();
    }
    
    public async Task UpdatePlayer() {
        var player = await Player.GetPlayer(PlayerId);
        broker.Publish(new UpdatePlayerEvent(player));
    }

    private void UpdatePlayerUI(UpdatePlayerEvent e) {
        var player = e.Player;
        Name.text = player.Name;
        Level.text = "Level: " + player.Level;
        Gold.text = "Gold: " + player.Gold;
        Exp.text = "Exp: " + player.Experience;
    }
}
