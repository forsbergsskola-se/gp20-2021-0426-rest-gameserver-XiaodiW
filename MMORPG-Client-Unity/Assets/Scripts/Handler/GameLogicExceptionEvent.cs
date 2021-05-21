using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Handler {

    public class GameLogicExceptionEvent {
        public string Message { get; }

        public GameLogicExceptionEvent(string message) {
            var reg = new Regex("{(.+?)}");
            var str = reg.Match(message).Value;
            var data = JsonConvert.DeserializeObject<ApiMessage>(str);
            Message = data.Message;
        }
    }

}