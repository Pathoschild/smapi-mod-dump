/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.TerrainFeatures;

namespace Leclair.Stardew.Hydrology {

	[Flags]
	public enum Neighbor {
		None = 0,
		FarTop = 1,
		TopLeft = 2,
		Top = 4,
		TopRight = 8,
		FarLeft = 16,
		Left = 32,
		FarRight = 64,
		Right = 128,
		FarBottomLeft = 256,
		BottomLeft = 512,
		FarBottom = 1024,
		Bottom = 2048,
		FarBottomRight = 4096,
		BottomRight = 8192
	};

	public class WaterFeature : TerrainFeature {

		public static Neighbor Far = Neighbor.FarTop | Neighbor.FarLeft | Neighbor.FarRight | Neighbor.FarBottom | Neighbor.FarBottomLeft | Neighbor.FarBottomRight;

		public static Neighbor AllLeft = Neighbor.TopLeft | Neighbor.Left | Neighbor.BottomLeft;
		public static Neighbor AllRight = Neighbor.TopRight | Neighbor.Right | Neighbor.BottomRight;

		public static Neighbor AllTop = Neighbor.TopLeft | Neighbor.Top | Neighbor.TopRight;
		public static Neighbor AllBottom = Neighbor.BottomLeft | Neighbor.Bottom | Neighbor.BottomRight;

		public static Neighbor Cardinal = Neighbor.Top | Neighbor.Left | Neighbor.Right | Neighbor.Bottom;

		#region Base Sprite Indexes

		public static int[] PlainShallows = new int[] {
			1246,
			1271,
			1274
		};

		public static int[] CommonShallows = new int[] {
			1249,
			1273,
			1299,
			1322,
			1324
		};

		public static int[] RareShallows = new int[] {
			1247,
			1248,
			1272,
			1297,
			1298,
			1323
		};

		public static int[] DeepSprites = new int[] {
			1227,
			1252,
			1277,
			1302
		};

		#endregion

		#region Lifecycle

		private int Sprite = -1;

		public WaterFeature() : base(false) {
			LoadTexture();
		}

		public void AddTile(GameLocation location, int x, int y) {
			if (location == null)
				location = Game1.currentLocation;

			int key = x + (y * location.Map.GetLayer("Back").LayerWidth);
			int group = (int) (50 * (1 + Math.Sin(100 * key)));
			int sprite;
			if (group > 90)
				sprite = RareShallows[key % RareShallows.Length];
			else if (group > 70)
				sprite = CommonShallows[key % CommonShallows.Length];
			else
				sprite = PlainShallows[key % PlainShallows.Length];

			ModEntry.instance.SetWaterTile(location, x, y, sprite);
			ModEntry.instance.SetWater(location, x, y, true);
			UpdateTile(location, x, y);
			UpdateNeighbors(location, x, y);
		}


		#endregion

		#region Player Interaction

		public override bool performToolAction(Tool t, int damage, Vector2 tileLocation, GameLocation location) {
			if (location == null)
				location = Game1.currentLocation;

			if (damage > 0)
				return false;

			int x = (int) tileLocation.X;
			int y = (int) tileLocation.Y;

			ModEntry.instance.SetWater(location, x, y, false);
			ModEntry.instance.SetWaterTile(location, x, y, 0);

			Sprite = -1;

			UpdateNeighbors(location, x, y, new Vector2(x, y));

			location.playSound("hoeHit");
			location.debris.Add(new Debris(new WaterFeatureObject(), tileLocation * Game1.tileSize + new Vector2(Game1.tileSize / 2, Game1.tileSize / 2)));

			return true;
		}

		#endregion

		#region Neighbors

		internal void UpdateNeighbors(GameLocation location, int x, int y, Vector2? ignore = null) {
			if (location == null)
				location = Game1.currentLocation;

			for (int i = -1; i <= 1; i++) {
				for (int j = -1; j <= 1; j++) {
					if (i == 0 && j == 0)
						continue;

					Vector2 pos = new Vector2(x + i, y + j);
					if (location.terrainFeatures.TryGetValue(pos, out TerrainFeature obj) && obj is WaterFeature water) {
						water.UpdateTile(location, x + i, y + j, ignore);
					}
				}
			}
		}

		internal void UpdateTile(GameLocation location, int x, int y, Vector2? ignore = null) {
			Neighbor neighbors = GetNeighbors(location, x, y, ignore);
			int sprite = PickTile(neighbors);

			Sprite = sprite;
		}

		private bool HasTile(GameLocation location, int x, int y, Vector2? ignore = null) {
			if (ignore.HasValue && ignore.Value.X == x && ignore.Value.Y == y)
				return false;

			return location.terrainFeatures.TryGetValue(new Vector2(x, y), out TerrainFeature obj) && obj is WaterFeature;
		}

		private Neighbor GetNeighbors(GameLocation location, int x, int y, Vector2? ignore = null) {
			Neighbor result = Neighbor.None;

			// Top
			if (HasTile(location, x, y - 1, ignore)) {
				result |= Neighbor.Top;

				// Far Top
				if (HasTile(location, x, y - 2, ignore))
					result |= Neighbor.FarTop;
			}

			// Left
			if (HasTile(location, x - 1, y, ignore)) {
				result |= Neighbor.Left;

				// Far Left
				if (HasTile(location, x - 2, y, ignore))
					result |= Neighbor.FarLeft;
			}

			// Right
			if (HasTile(location, x + 1, y, ignore)) {
				result |= Neighbor.Right;

				// Far Right
				if (HasTile(location, x + 2, y, ignore))
					result |= Neighbor.FarRight;
			}

			// Bottom
			if (HasTile(location, x, y + 1, ignore)) {
				result |= Neighbor.Bottom;

				// Far Bottom
				if (HasTile(location, x, y + 2, ignore))
					result |= Neighbor.FarBottom;
			}

			// Top Left
			if ((result & (Neighbor.Top | Neighbor.Left)) != 0)
				if (HasTile(location, x - 1, y - 1, ignore))
					result |= Neighbor.TopLeft;

			// Top Right
			if ((result & (Neighbor.Top | Neighbor.Right)) != 0)
				if (HasTile(location, x + 1, y - 1, ignore))
					result |= Neighbor.TopRight;

			// Bottom Left
			if ((result & (Neighbor.Bottom | Neighbor.Left)) != 0)
				if (HasTile(location, x - 1, y + 1, ignore)) {
					result |= Neighbor.BottomLeft;

					// Far Bottom Left
					if (HasTile(location, x - 1, y + 2, ignore))
						result |= Neighbor.FarBottomLeft;
				}

			// Bottom Right
			if ((result & (Neighbor.Bottom | Neighbor.Right)) != 0)
				if (HasTile(location, x + 1, y + 1, ignore)) {
					result |= Neighbor.BottomRight;

					// Far Bottom Right
					if (HasTile(location, x + 1, y + 2, ignore))
						result |= Neighbor.FarBottomRight;

				}

			return result;
		}

		private int PickTile(Neighbor neighbors) {
			// Top Right
			if (Matches(neighbors, Neighbor.Right | Neighbor.BottomRight | Neighbor.Bottom, Neighbor.TopRight | Neighbor.BottomLeft))
				return 183;

			if (Matches(neighbors, Neighbor.Left | Neighbor.Right | AllBottom, Neighbor.TopLeft | Neighbor.TopRight))
				return 184;

			if (Matches(neighbors, Neighbor.Left | Neighbor.BottomLeft | Neighbor.Bottom, Neighbor.TopLeft | Neighbor.BottomRight))
				return 185;

			if (Matches(neighbors, Neighbor.Top | Neighbor.FarTop | Neighbor.Bottom | AllRight, Neighbor.TopLeft | Neighbor.BottomLeft))
				return 233;

			if (Matches(neighbors, Neighbor.Top | Neighbor.Bottom | AllRight, Neighbor.TopLeft | Neighbor.BottomLeft))
				return 208;

			if (Matches(neighbors, Neighbor.Top | Neighbor.FarTop | Neighbor.Bottom | AllLeft, Neighbor.TopRight | Neighbor.BottomRight))
				return 235;

			if (Matches(neighbors, Neighbor.Top | Neighbor.Bottom | AllLeft, Neighbor.TopRight | Neighbor.BottomRight))
				return 210;

			if (Matches(neighbors, AllTop | Neighbor.Right))
				return 212;

			if (Matches(neighbors, Neighbor.Top | Neighbor.TopRight | Neighbor.Right, Neighbor.BottomRight | Neighbor.TopLeft))
				return 258;

			if (Matches(neighbors, AllTop | Neighbor.Left))
				return 213;

			if (Matches(neighbors, Neighbor.Top | Neighbor.TopLeft | Neighbor.Left, Neighbor.BottomLeft | Neighbor.TopRight))
				return 260;

			if (Matches(neighbors, AllTop | Neighbor.Left | Neighbor.Right, Neighbor.BottomLeft | Neighbor.BottomRight))
				return 259;

			if (Matches(neighbors, Neighbor.Top | AllRight | Neighbor.Bottom))
				return 237;

			if (Matches(neighbors, Neighbor.Top | AllLeft | Neighbor.Bottom))
				return 238;

			if (Matches(neighbors, Neighbor.Top | Neighbor.TopRight | Neighbor.Right | Neighbor.Left | Neighbor.BottomLeft | Neighbor.Bottom, Neighbor.BottomRight))
				return 283;

			if (Matches(neighbors, Neighbor.Top | Neighbor.Right | Neighbor.TopLeft | Neighbor.Left | Neighbor.Bottom | Neighbor.BottomRight, Neighbor.BottomLeft))
				return 284;

			if (Matches(neighbors, AllTop | Neighbor.Left | AllRight | Neighbor.Bottom))
				return 308;

			if (Matches(neighbors, AllTop | AllLeft | Neighbor.Right | Neighbor.Bottom))
				return 309;

			return -1;
		}

		private static bool Matches(Neighbor neighbor, Neighbor other, Neighbor? ignore = null) {
			Neighbor ign = ignore.HasValue ? (ignore.Value | Far) : Far;
			ign &= ~other;
			return (neighbor & ~ign) == other;
			//return (neigsdhbor & other) == other;
		}

		#endregion

		#region Rendering


		private void LoadTexture() {

		}

		public override bool seasonUpdate(bool onLoad) {
			LoadTexture();
			return true;
		}

		public override bool isPassable(Character c = null) {
			return true;
		}

		public override void draw(SpriteBatch b, Vector2 tileLocation) {
			Texture2D tex = ModEntry.instance.OutsideTexture.Value;
			if (tex == null) // || Sprite < 0)
				return;

			int x = (int) tileLocation.X;
			int y = (int) tileLocation.Y;

			Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize, y * Game1.tileSize));

			Neighbor neighbors = GetNeighbors(currentLocation, x, y);
			int sprite = PickTile(neighbors);

			// Draw Above

			if (sprite >= 0)
				b.Draw(
					tex,
					local,
					Game1.getSourceRectForStandardTileSheet(tex, sprite, 16, 16),
					Color.White,
					0f,
					Vector2.Zero,
					4f,
					SpriteEffects.None,
					(float) ((y * 64.0 + 2.0 + x / 10000.0) / 20000.0)
				);

			string label = ((short) neighbors).ToString();
			Vector2 size = Game1.tinyFont.MeasureString(label);

			if (Game1.oldKBState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F3))
				Utility.drawTextWithShadow(
					b,
					label,
					Game1.tinyFont,
					local + new Vector2((64 - size.X) / 2, (64 - size.Y) / 2),
					Color.Black,
					layerDepth: 1f
				);

			/*b.DrawString(
				Game1.smallFont,
				label,
				local + new Vector2(32,32),
				Color.White,
				0f,
				size / 2,
				1f,
				SpriteEffects.None,
				1f
			);*/

		}

		#endregion

	}
}
