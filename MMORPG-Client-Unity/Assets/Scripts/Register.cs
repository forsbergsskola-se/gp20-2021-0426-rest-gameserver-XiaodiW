using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Types.Player;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace {

    public class Register : MonoBehaviour {
        public InputField input;
        public Login login;
        private void Awake() 
        {
            this.gameObject.SetActive(false);
        }

        public void ShowRegisterPopup() {
            this.gameObject.SetActive(true);
        }
        
        public async void RegisterPlayer() {
            var newPlayer = await Player.CreatePlayer(input.text);
            var list = new List<Guid>();
            if(PlayerPrefs.HasKey("IDs")) {
                var playerIds = JsonConvert.DeserializeObject<Guid[]>(PlayerPrefs.GetString("IDs"));
                list = playerIds.ToList();
            }
            list.Add(newPlayer.Id);
            var str = JsonConvert.SerializeObject(list.ToArray());
            PlayerPrefs.SetString("IDs",str);
            await login.GetComponent<Login>().GeneratePlayers();
            this.gameObject.SetActive(false);
        }
    }

}