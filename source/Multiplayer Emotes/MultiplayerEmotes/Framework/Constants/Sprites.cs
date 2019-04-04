
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace MultiplayerEmotes.Framework.Constants {

	internal static class Sprites {

		public static class Emotes {

			public static string AssetName => "TileSheets/emotes";

			public static Texture2D Texture => ModEntry.ModHelper.Content.Load<Texture2D>(AssetName, ContentSource.GameContent);

			public static int Size = 16;

			public static int AnimationFrames = 4;

		}

		public static class MenuButton {

			public static string AssetName => "LooseSprites/Cursors";

			public static Texture2D Texture => ModEntry.ModHelper.Content.Load<Texture2D>(AssetName, ContentSource.GameContent);

			public static Rectangle SourceRectangle = new Rectangle(301, 288, 15, 15);

		}

		public static class MenuBox {

			public static string AssetName => "assets/emoteBox.png";

			public static Texture2D Texture => ModEntry.ModHelper.Content.Load<Texture2D>(AssetName, ContentSource.ModFolder);

			public static Rectangle SourceRectangle = new Rectangle(0, 0, 228, 300);

			public static int Width = 300;

			public static int Height = 250;

			//TODO make enum of top, down, left, right arrows
			public static class TopArrow {

				public static Rectangle SourceRectangle = new Rectangle(228, 0, 28, 28 - 8);

			}

			public static class DownArrow {

				public static Rectangle SourceRectangle = new Rectangle(228, 28 + 8, 28, 28 - 8);

			}

			public static class LeftArrow {

				public static Rectangle SourceRectangle = new Rectangle(228, 56, 28 - 8, 28);

			}

			public static class RightArrow {

				public static Rectangle SourceRectangle = new Rectangle(228 + 8, 84, 28 - 8, 28);

			}

		}

		public static class MenuArrow {

			public static string AssetName => "LooseSprites/chatBox";

			public static Texture2D Texture => ModEntry.ModHelper.Content.Load<Texture2D>(AssetName, ContentSource.GameContent);

			//TODO make enum of top, down, left, right arrows
			public static Rectangle UpSourceRectangle = new Rectangle(156, 300, 32, 20); //new Rectangle(256, 20, 32, 20);

			public static Rectangle DownSourceRectangle = new Rectangle(192, 304, 32, 20);//new Rectangle(256, 200, 32, 20);

		}

	}

}
