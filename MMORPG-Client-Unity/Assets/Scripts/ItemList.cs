using System.Linq;
using Events;
using UnityEngine;

public class ItemList : MonoBehaviour {
    public GameObject ItemPrefab;

    private void Start() {
        FindObjectOfType<EventsBroker>().SubscribeTo<UpdatePlayerEvent>(GenerateItemList);
    }

    private void GenerateItemList(UpdatePlayerEvent e) {
        if(e.Player.Items == null) return;
        var itemList = e.Player.Items.OrderByDescending(i=>i.IsEquipped).ThenBy(i=>i.LevelRequired).ToList();
        foreach(Transform child in transform) Destroy(child.gameObject);
        foreach(var item in itemList) {
            if(item.IsDeleted) continue;
            var instance = Instantiate(ItemPrefab, transform);
            instance.GetComponent<ItemInfo>().item = item;
        }
    }
}