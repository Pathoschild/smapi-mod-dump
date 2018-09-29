
using StardewValley;
using System;

namespace CustomEmojis.Framework.Events {
	public class ReceivedEmojiTextureRequestEventArgs : EventArgs {
		public Farmer SourceFarmer { get; set; }
	}
}
