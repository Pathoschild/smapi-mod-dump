
using MapPings.Framework.Network;
using StardewValley;
using System;
using System.Collections.Generic;

namespace MapPings.Framework.Events {
	public class PlayerDisconnectedEventArgs : EventArgs {
		public Farmer Player { get; set; }
	}
}
