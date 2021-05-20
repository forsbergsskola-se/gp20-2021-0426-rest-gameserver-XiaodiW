using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Types.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    
    private Guid PlayerId => JsonConvert.DeserializeObject<Guid>(PlayerPrefs.GetString("PlayerId"));
    public Text Name;
    public Text Level;
    public Text Gold;
    public Text Exp;

    public UnityAction UpdatePlayerProfile;

    private void Start() {
        UpdatePlayerProfile += UpdatePlayer;
        UpdatePlayerProfile.Invoke();
    }

    private async void UpdatePlayer() {
        var player = await Player.GetPlayer(PlayerId);
        Name.text = player.Name;
        Level.text = player.Level.ToString();
        Gold.text = player.Gold.ToString();
        Exp.text = player.Experience.ToString();
    }
}
