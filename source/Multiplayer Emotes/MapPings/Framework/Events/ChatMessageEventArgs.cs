
using MapPings.Framework.Constants;
using StardewValley;
using System;

namespace MapPings.Framework.Events {
	public class ChatMessageEventArgs : EventArgs {
		public Farmer SourcePlayer { get; set; }
		public ChatMessageKind ChatKind { get; set; }
		public LocalizedContentManager.LanguageCode Language { get; set; }
		public string Message { get; set; }
	}
}
