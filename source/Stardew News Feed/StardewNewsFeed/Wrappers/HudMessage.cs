using StardewNewsFeed.Enums;

namespace StardewNewsFeed.Wrappers {
    public class HudMessage : IHudMessage {

        private readonly string _message;
        private readonly HudMessageType _messageType;

        public HudMessage(string message, HudMessageType messageType) {
            _message = message;
            _messageType = messageType;
        }

        public string GetMessageText() {
            return _message;
        }

        public HudMessageType GetMessageType() {
            return _messageType;
        }
    }
}
