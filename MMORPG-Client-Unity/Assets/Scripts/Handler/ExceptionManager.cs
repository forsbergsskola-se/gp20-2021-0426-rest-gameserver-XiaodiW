using Events;
using UnityEngine;
using UnityEngine.UI;

namespace Handler {

    public class ExceptionManager : MonoBehaviour {
        private EventsBroker broker;
        private void Awake() {
            broker = GetComponent<EventsBroker>();
            Application.logMessageReceived += HandleException;
            DontDestroyOnLoad(gameObject);
        }

        private void HandleException(string logString, string stackTrace, LogType type) {
            if(type == LogType.Exception) broker.Publish(new GameLogicExceptionEvent(logString));
        }
    }

}