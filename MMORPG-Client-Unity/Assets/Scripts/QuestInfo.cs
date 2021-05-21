using System;
using System.Collections;
using Newtonsoft.Json;
using Types.Quest;
using UnityEngine;
using UnityEngine.UI;

public class QuestInfo : MonoBehaviour {
    public Quest quest;

    public Text level;
    public Text exp;
    public Text reward;
    private Guid PlayerId => JsonConvert.DeserializeObject<Guid>(PlayerPrefs.GetString("PlayerId"));
    private EventsBroker broker;
    private void Start() {
        broker = FindObjectOfType<EventsBroker>();
        StartCoroutine(InstantiateQuest());
    }

    private IEnumerator InstantiateQuest() {
        yield return new WaitUntil(() => quest != null);
        var leftCons = 12;
        var rightCons = 15;
        var levelstr = "Level:";
        level.text = levelstr.PadRight(leftCons)  +quest.Level.ToString().PadLeft(rightCons);
        var expstr = "Experience:";
        exp.text = expstr.PadRight(leftCons) + quest.Experience.ToString().PadLeft(rightCons);
        var goldstr = "Gold:";
        reward.text = quest.RewardGold
            ? goldstr.PadRight(leftCons) + quest.Gold.ToString().PadLeft(rightCons)
            : quest.Item.Name.PadLeft(rightCons);
    }

    public async void DoQuest() {
        var player = await Quest.DoQuests(PlayerId,quest.Id);
        broker.Publish(new UpdatePlayerEvent(player));
    }
}