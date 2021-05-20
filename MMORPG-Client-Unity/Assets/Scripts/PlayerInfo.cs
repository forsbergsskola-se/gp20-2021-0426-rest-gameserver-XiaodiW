using System.Collections;
using Types.Player;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour {
    public Player player;
    public Text Name;
    public Text Level;
    public Text Gold;
    public Text Exp;
    public Text Item;

    private void Start() {
        StartCoroutine(InstantiatePlayer());
    }

    private IEnumerator InstantiatePlayer() {
        yield return new WaitUntil(() => player != null);
        Name.text = player.Name;
        Level.text = player.Level.ToString();
        Gold.text = player.Gold.ToString();
        Exp.text = player.Experience.ToString();
        Item.text = player.Items.Count.ToString();
    }
}