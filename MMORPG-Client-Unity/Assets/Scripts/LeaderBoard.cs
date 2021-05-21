using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Types;
using Types.Player;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard: MonoBehaviour {
        public Text[] byLevelName;
        public Text[] byLevelValue;
        public Text[] byGoldName;
        public Text[] byGoldValue;

        private void Start() {
            InvokeRepeating(nameof(UpdateLeaderBoardByLevel),1f,2f);
            InvokeRepeating(nameof(UpdateLeaderBoardByGold),2f,2f);
        }

        async Task<Player[]> GetLeaderBoard(LeaderBoardOrderBy orderBy) {
           return await Player.GetLeaderBoard(orderBy);
        }

        private async void UpdateLeaderBoardByLevel() {
            var players = await GetLeaderBoard(LeaderBoardOrderBy.Level);
            for(var i = 0; i < 10; i++) {
                byLevelName[i].text = players[i].Name;
                byLevelValue[i].text = players[i].Level.ToString();
            }
        }
        
        private async void UpdateLeaderBoardByGold() {
            var players = await GetLeaderBoard(LeaderBoardOrderBy.Gold);
            for(var i = 0; i < 10; i++) {
                byGoldName[i].text = players[i].Name;
                byGoldValue[i].text = players[i].Gold.ToString();
            }
        }
}