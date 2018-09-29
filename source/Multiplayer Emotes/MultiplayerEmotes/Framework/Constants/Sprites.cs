
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.IO;

namespace MultiplayerEmotes.Framework.Constants {

    internal static class Sprites {

		public static class Emotes {

			public static string AssetName = "TileSheets\\emotes";

			public static Texture2D Texture => Game1.content.Load<Texture2D>(AssetName);

		}

		public static class Menu {

			public static string PrototypeAssetName = Path.Combine("assets", "emoteBoxPrototype.png");

			public static string AssetName = Path.Combine("assets", "emoteBox.png");

			public static readonly Rectangle EmoteBox = new Rectangle(0, 0, 228, 300);

			public static readonly Rectangle TopArrow = new Rectangle(228, 0, 28, 28 - 8);

			public static readonly Rectangle DownArrow = new Rectangle(228, 28 + 8, 28, 28 - 8);

			public static readonly Rectangle LeftArrow = new Rectangle(228, 56, 28 - 8, 28);

			public static readonly Rectangle RightArrow = new Rectangle(228 + 8, 84, 28 - 8, 28);

		}

		public static class ChatBox {

			public static string AssetName = @"LooseSprites\\chatBox";

			public static Texture2D Texture => Game1.content.Load<Texture2D>(AssetName);

			public static Rectangle UpArrow = new Rectangle(256, 20, 32, 20);

			public static Rectangle DownArrow = new Rectangle(256, 200, 32, 20);

		}

	}

}
