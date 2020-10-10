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

using System.IO;

namespace CustomEmojis.Framework.Constants {

	internal static class Sprites {

		public static class VanillaEmojis {

			public static string AssetName = "LooseSprites\\emojis";

			public static Texture2D Texture => Game1.content.Load<Texture2D>(AssetName);

		}

		public static class CustomEmojis {

            public static string AssetName = Path.Combine(ModPaths.Assets.Folder, ModPaths.Assets.OutputFile);

		}

	}

}
