/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/


using StardewValley;
using System;

namespace MapPings.Framework.Events {
	public class PlayerConnectedEventArgs : EventArgs {
		public Farmer Player { get; set; }
	}
}
