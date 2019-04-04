
using StardewValley;
using System;

namespace MapPings.Framework.Events {
	public class PlayerDisconnectedEventArgs : EventArgs {
		public Farmer Player { get; set; }
	}
}
