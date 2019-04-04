
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

using System.IO;

namespace MapPings.Framework.Constants {

	internal static class Sprites {

		public static class Map {

			public static string AssetName = "LooseSprites/map";

			public static Texture2D Texture => Game1.content.Load<Texture2D>(AssetName);

			public static Rectangle SourceRectangle = new Rectangle(0, 0, 300, 180);

		}

		public static class PingArrow {

			public static string AssetName = Path.Combine(ModPaths.Assets.Folder, ModPaths.Assets.Ping);

			//public static Texture2D Texture => ModEntry.ModHelper.Content.Load<Texture2D>(AssetName, ContentSource.ModFolder);

			public static Texture2D Texture(IModHelper helper) {
				return helper.Content.Load<Texture2D>(AssetName, ContentSource.ModFolder);
			}

			public static Rectangle SourceRectangle = new Rectangle(0, 0, 9, 9);

		}

		public static class PingWave {

			public static string AssetName = "TileSheets/animations";

			//public static Texture2D Texture => ModEntry.ModHelper.Content.Load<Texture2D>(AssetName, ContentSource.ModFolder);

			public static Texture2D Texture(IModHelper helper) {
				return helper.Content.Load<Texture2D>(AssetName, ContentSource.ModFolder);
			}

			public static Rectangle SourceRectangle = new Rectangle(0, 0, 64, 64);

		}

		public static class VanillaPingArrow {

			public static string AssetName = "LooseSprites/Cursors";

			public static Texture2D Texture => Game1.content.Load<Texture2D>(AssetName);

			public static Rectangle SourceRectangle = new Rectangle(232, 346, 9, 9);

		}

	}

}
