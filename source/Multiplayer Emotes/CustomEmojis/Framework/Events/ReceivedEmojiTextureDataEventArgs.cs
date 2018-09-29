
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
