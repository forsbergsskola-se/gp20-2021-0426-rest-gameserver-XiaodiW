using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Types.Player;
using UnityEngine;
using UnityEngine.UI;

public class Register : MonoBehaviour {
    public InputField input;
    public PlayerList playerList;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public void ShowRegisterPopup() {
        gameObject.SetActive(true);
    }

    public async void RegisterPlayer() {
        var newPlayer = await Player.CreatePlayer(input.text);
        var playerIds = new List<Guid>();
        if(PlayerPrefs.HasKey("IDs"))
            playerIds = JsonConvert.DeserializeObject<List<Guid>>(PlayerPrefs.GetString("IDs"));
        playerIds.Add(newPlayer.Id);
        var str = JsonConvert.SerializeObject(playerIds);
        PlayerPrefs.SetString("IDs", str);
        await playerList.GetComponent<PlayerList>().GeneratePlayerList();
        gameObject.SetActive(false);
    }
}