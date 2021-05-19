using System;
using System.Collections;
using System.Collections.Generic;
using Types.Player;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace {

    public class PlayerInfo : MonoBehaviour {

        [HideInInspector] public Player player;
        public Text Name;
        public Text Level;
        public Text Gold;
        public Text Exp;
        public Text Item;

        private void Start() {
            StartCoroutine(InstantiatePlayer());
        }

        IEnumerator InstantiatePlayer() {
            yield return new WaitUntil(() => player != null);
            Name.text = player.Name;
            Level.text = player.Level.ToString();
            Gold.text = player.Gold.ToString();
            Exp.text = player.Experience.ToString();
            Item.text = player.Items.Count.ToString();

        }
    }

}