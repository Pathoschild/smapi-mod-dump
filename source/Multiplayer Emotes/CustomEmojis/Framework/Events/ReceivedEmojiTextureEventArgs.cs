
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
