/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/


using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;

namespace CustomEmojis.Framework.Events {
	public class ReceivedEmojiTextureEventArgs : EventArgs {
		public Farmer SourceFarmer { get; set; }
		public int NumberEmojis { get; set; }
		public Texture2D EmojiTexture { get; set; }
	}
}
