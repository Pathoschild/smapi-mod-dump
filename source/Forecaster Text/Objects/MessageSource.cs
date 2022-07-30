/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GStefanowich/SDV-Forecaster
**
*************************************************/

namespace ForecasterText.Objects {
    internal sealed class MessageSource {
        public string Source;
        public string Message;
        
        private MessageSource(string source, string message) {
            this.Source = source;
            this.Message = message;
        }
        
        public override string ToString() => $"{this.Source}: {this.Message}";
        
        public static MessageSource TV(string message = "") {
            if (message is null)
                return null;
            return new MessageSource("TV", message);
        }
        public static MessageSource Calendar(string message = "") {
            if (message is null)
                return null;
            return new MessageSource("Calendar", message);
        }
    }
}
