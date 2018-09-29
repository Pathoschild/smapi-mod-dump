
using CustomEmojis.Framework.Network;
using StardewValley;
using System;
using System.Collections.Generic;

namespace CustomEmojis.Framework.Events {
	public class PlayerDisconnectedEventArgs : EventArgs {
		public Farmer Player { get; set; }
	}
}
