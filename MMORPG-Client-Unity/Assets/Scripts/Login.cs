using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Types.Player;
using UnityEngine;

public class Login : MonoBehaviour {
    public GameObject playerPrefab;

    private async void Start() {
        await GeneratePlayerList();
    }

    public async Task GeneratePlayerList() {
        if(!PlayerPrefs.HasKey("IDs")) return;
        var playerIds = JsonConvert.DeserializeObject<List<Guid>>(PlayerPrefs.GetString("IDs"));
        foreach(Transform child in transform)
            if(child.gameObject.name != "Add Player")
                Destroy(child.gameObject);
        foreach(var playerId in playerIds) {
            var player = await Player.GetPlayer(playerId);
            var instance = Instantiate(playerPrefab, transform);
            instance.GetComponent<PlayerInfo>().player = player;
            instance.transform.SetAsFirstSibling();
        }
    }
}