
using StardewValley;
using System;

namespace MapPings.Framework.Events {
	public class PlayerConnectedEventArgs : EventArgs {
		public Farmer Player { get; set; }
	}
}
