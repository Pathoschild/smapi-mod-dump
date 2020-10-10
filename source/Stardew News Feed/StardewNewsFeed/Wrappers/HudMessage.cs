/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mikesnorth/StardewNewsFeed
**
*************************************************/

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
