
using StardewValley;
using System;

namespace CustomEmojis.Framework.Events {
	public class PlayerDisconnectedEventArgs : EventArgs {
		public Farmer Player { get; set; }
	}
}
