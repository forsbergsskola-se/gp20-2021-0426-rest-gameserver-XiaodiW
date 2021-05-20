using System;
using System.Collections;
using MMORPG.Types.Item;
using Newtonsoft.Json;
using Types.Item;
using Types.Player;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour {
    public Item item;
    public Text type;
    public Text rarity;
    public Text level;
    public Text price;
    public Text levelBonus;
    private Guid PlayerId => JsonConvert.DeserializeObject<Guid>(PlayerPrefs.GetString("PlayerId"));

    private void Start() {
        StartCoroutine(InstantiateItem());
    }

    private IEnumerator InstantiateItem() {
        yield return new WaitUntil(() => item != null);
        type.text = item.Type.ToString();
        rarity.text = item.Rarity.ToString();
        level.text = item.LevelRequired.ToString();
        price.text = item.Price.ToString();
        levelBonus.text = item.LevelBonus.ToString();
    }

    public async void SellItem() {
        await Item.HandleItem(PlayerId, item.Id, ItemActions.Sell);
        var player = await Player.GetPlayer(PlayerId);
        FindObjectOfType<EventsBroker>().Publish(new UpdatePlayerEvent(player));
    }
}