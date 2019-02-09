namespace TehPers.Core.Multiplayer {
    public class MessageHandler {
        public string Channel { get; }
        internal MessageReader OnMessageRecevied { get; }

        internal MessageHandler(string channel, MessageReader onMessageRecevied) {
            this.Channel = channel;
            this.OnMessageRecevied = onMessageRecevied;
        }

        /// <summary>Stops this handler from receiving any future messages.</summary>
        /// <returns>True if successfully removed, false if not registered.</returns>
        public bool Unregister() {
            return MessageDelegator.UnregisterMessageHandler(this);
        }
    }
}