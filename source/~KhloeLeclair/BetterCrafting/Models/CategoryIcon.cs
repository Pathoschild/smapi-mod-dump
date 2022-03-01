/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/


using Leclair.Stardew.Common.Enums;

using Microsoft.Xna.Framework;

namespace Leclair.Stardew.BetterCrafting.Models {
	public class CategoryIcon {

		public enum IconType {
			Item,
			Texture
		}

		public IconType Type { get; set; }

		// Item
		public string RecipeName { get; set; }


		// Texture
		public GameTexture? Source { get; set; } = null;
		public string Path { get; set; }

		public Rectangle? Rect { get; set; }
		public float Scale { get; set; } = 1;
	}
}
