using System;
using Newtonsoft.Json;
using Types.Quest;
using UnityEngine;

public class QuestList : MonoBehaviour {
    public GameObject questPrefab;
    public GameObject questEmpty;
    private Guid PlayerId => JsonConvert.DeserializeObject<Guid>(PlayerPrefs.GetString("PlayerId"));

    private EventsBroker broker;

    private void Start() {
        broker = FindObjectOfType<EventsBroker>();
        broker.SubscribeTo<UpdatePlayerEvent>(GenerateQuestList);
    }

    private void GenerateQuestList(UpdatePlayerEvent e) {
        var questList = e.Player.Quests;
        foreach(Transform child in transform) Destroy(child.gameObject);
        foreach(var quest in questList) {
            if(quest.Status == QuestStatus.Done) continue;
            var instance = Instantiate(questPrefab, transform);
            instance.GetComponent<QuestInfo>().quest = quest;
        }
        for(var i = 0; i < 5 - questList.Count; i++) Instantiate(questEmpty, transform);
    }

    public async void GetQuest() {
        var player = await Quest.GetQuests(PlayerId);
        broker.Publish(new UpdatePlayerEvent(player));
    }
}