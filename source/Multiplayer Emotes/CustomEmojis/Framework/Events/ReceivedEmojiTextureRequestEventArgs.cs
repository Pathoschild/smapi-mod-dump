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

namespace CustomEmojis.Framework.Events {
	public class ReceivedEmojiTextureRequestEventArgs : EventArgs {
		public Farmer SourceFarmer { get; set; }
	}
}
