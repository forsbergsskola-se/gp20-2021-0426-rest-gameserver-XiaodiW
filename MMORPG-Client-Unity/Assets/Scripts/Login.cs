using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace;
using Newtonsoft.Json;
using Types.Player;
using UnityEngine;

public class Login : MonoBehaviour {
    public GameObject playerPrefab;
    private async void Start() {
        await GeneratePlayers();
    }

    public async Task GeneratePlayers() {
        if(!PlayerPrefs.HasKey("IDs")) return;
        var playerIds = JsonConvert.DeserializeObject<Guid[]>(PlayerPrefs.GetString("IDs"));
        foreach(var playerId in playerIds) {
            var player = await Player.GetPlayer(playerId);
            var instance = Instantiate(this.playerPrefab, transform);
            instance.GetComponent<PlayerInfo>().player = player;
            instance.transform.SetAsFirstSibling();
        }
    }
}
