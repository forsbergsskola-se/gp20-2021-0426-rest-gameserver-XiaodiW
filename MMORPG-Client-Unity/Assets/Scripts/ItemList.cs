using UnityEngine;

public class ItemList : MonoBehaviour {
    public GameObject playerPrefab;

    private void Start() {
        FindObjectOfType<EventsBroker>().SubscribeTo<UpdatePlayerEvent>(GenerateItemList);
    }

    private void GenerateItemList(UpdatePlayerEvent e) {
        var itemList = e.Player.Items;
        foreach(Transform child in transform) Destroy(child.gameObject);
        foreach(var item in itemList) {
            if(item.IsDeleted) continue;
            var instance = Instantiate(playerPrefab, transform);
            instance.GetComponent<ItemInfo>().item = item;
        }
    }
}