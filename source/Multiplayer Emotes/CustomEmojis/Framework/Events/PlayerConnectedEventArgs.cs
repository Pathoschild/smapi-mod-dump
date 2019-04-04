
using StardewValley;
using System;

namespace CustomEmojis.Framework.Events {
	public class PlayerConnectedEventArgs : EventArgs {
		public Farmer Player { get; set; }
	}
}
