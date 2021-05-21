using System.Collections;
using System.Text.RegularExpressions;
using Events;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Handler {

    public class ExceptionPopup : MonoBehaviour {
        public Text text;
        private EventsBroker broker;

        private void Awake() {
            broker = FindObjectOfType<EventsBroker>();
            gameObject.SetActive(false);
            broker.SubscribeTo<GameLogicExceptionEvent>(ShowPopup);
        }

        private void ShowPopup(GameLogicExceptionEvent e) {
            text.text = e.Message;
            gameObject.SetActive(true);
            StartCoroutine(nameof(FadeOut));
        }

        private IEnumerator FadeOut() {
            var aTime = 5f;
            var color = GetComponent<Image>().color;
            for(var t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {
                color.a = Mathf.Lerp(1, 0, t);
                GetComponent<Image>().color = color;
                yield return null;
            }
            gameObject.SetActive(false);
        }
    }

}