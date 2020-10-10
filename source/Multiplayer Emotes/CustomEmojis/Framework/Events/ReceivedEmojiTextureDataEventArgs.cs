/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/


using CustomEmojis.Framework.Network;
using StardewValley;
using System;
using System.Collections.Generic;

namespace CustomEmojis.Framework.Events {
	public class ReceivedEmojiTextureDataEventArgs : EventArgs {
		public Farmer SourceFarmer { get; set; }
		public List<TextureData> TextureDataList { get; set; }
	}
}
