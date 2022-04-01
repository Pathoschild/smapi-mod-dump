/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;


namespace Leclair.Stardew.BetterCrafting {
	public static class Sprites {

		public static class Buttons {
			public static Texture2D Texture;

			public readonly static Rectangle UNIFORM_OFF = new(0, 0, 16, 16);
			public readonly static Rectangle UNIFORM_ON = new(0, 16, 16, 16);

			public readonly static Rectangle SEASONING_ON = new(16, 0, 16, 16);
			public readonly static Rectangle SEASONING_OFF = new(16, 16, 16, 16);
			public readonly static Rectangle SEASONING_LOCAL = new(48, 16, 16, 16);

			public readonly static Rectangle FAVORITE_ON = new(32, 0, 16, 16);
			public readonly static Rectangle FAVORITE_OFF = new(32, 16, 16, 16);

			public readonly static Rectangle WRENCH = new(48, 0, 16, 16);
			public readonly static Rectangle SETTINGS = new(64, 0, 16, 16);

			public readonly static Rectangle SEARCH_OFF = new(64, 32, 16, 16);
			public readonly static Rectangle SEARCH_ON = new(64, 16, 16, 16);

			public readonly static Rectangle QUALITY_0 = new(0, 32, 16, 16);
			public readonly static Rectangle QUALITY_1 = new(16, 32, 16, 16);
			public readonly static Rectangle QUALITY_2 = new(32, 32, 16, 16);
			public readonly static Rectangle QUALITY_3 = new(48, 32, 16, 16);
		}

	}
}
