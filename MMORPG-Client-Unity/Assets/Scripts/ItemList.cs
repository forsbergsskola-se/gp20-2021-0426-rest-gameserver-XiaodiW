using UnityEngine;

public class ItemList : MonoBehaviour {
    public GameObject ItemPrefab;

    private void Start() {
        FindObjectOfType<EventsBroker>().SubscribeTo<UpdatePlayerEvent>(GenerateItemList);
    }

    private void GenerateItemList(UpdatePlayerEvent e) {
        var itemList = e.Player.Items;
        if(itemList == null) return;
        foreach(Transform child in transform) Destroy(child.gameObject);
        foreach(var item in itemList) {
            if(item == null || item.IsDeleted) continue;
            var instance = Instantiate(ItemPrefab, transform);
            instance.GetComponent<ItemInfo>().item = item;
        }
    }
}