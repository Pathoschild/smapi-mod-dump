/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/FlipBuildings
**
*************************************************/

using System.Reflection;
using Microsoft.Xna.Framework;
using xTile.Layers;
using StardewValley;

namespace FlipBuildings.Utilities
{
	internal class FarmHouseHelper
	{
		internal static void Load()
		{
			Farm farm = Game1.getFarm();
			if (!farm.modData.ContainsKey(ModDataKeys.FLIPPED))
				return;
			Flip();
		}

		internal static void Flip()
		{
			Farm farm = Game1.getFarm();
			Point farmHouseEntry = farm.GetMainFarmHouseEntry();
			int tileX = farmHouseEntry.X - (farm.modData.ContainsKey(ModDataKeys.FLIPPED) ? 3 : 5);
			int tileY = farmHouseEntry.Y - 4;
			int tilesWide = 9;
			int tilesHigh = 6;
			FlipLayer(tileX, tileY, tilesWide, tilesHigh, "AlwaysFront");
			FlipLayer(tileX, tileY, tilesWide, tilesHigh, "Front");
			FlipLayer(tileX, tileY, tilesWide, tilesHigh, "Paths");
			FlipLayer(tileX, tileY, tilesWide, tilesHigh, "Buildings");
			FlipLayer(tileX, tileY, tilesWide, tilesHigh, "Back");
			FlipTileFrontMainFarmHouseEntry();
		}

		private static void FlipLayer(int tileX, int tileY, int tilesWide, int tilesHigh, string layerId)
		{
			Layer layer = Game1.getFarm().Map.GetLayer(layerId);
			for (int x = tileX; x < (tileX + tileX + tilesWide ) / 2; x++)
			{
				for (int y = tileY; y < tileY + tilesHigh; y++)
				{
					(layer.Tiles[x, y], layer.Tiles[tileX + tilesWide - 1 - (x - tileX), y]) = (layer.Tiles[tileX + tilesWide - 1 - (x - tileX), y], layer.Tiles[x, y]);
				}
			}
			typeof(Layer).GetField("m_tileArray", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Game1.getFarm().Map.GetLayer(layerId), layer.Tiles);
		}

		private static void FlipTileFrontMainFarmHouseEntry()
		{
			Farm farm = Game1.getFarm();
			Point left = new(62, 17);
			Point right = new(64, 17);
			const string layer = "Back";
			const string noFurniture = "NoFurniture";
			const string spawnable = "Spawnable";

			if (farm.doesTileHaveProperty(left.X, left.Y, noFurniture, layer) != null && farm.doesTileHaveProperty(left.X, left.Y, spawnable, layer) != null)
			{
				farm.removeTileProperty(left.X, left.Y, layer, noFurniture);
				farm.removeTileProperty(left.X, left.Y, layer, spawnable);
				farm.setTileProperty(right.X, right.Y, layer, noFurniture, "T");
				farm.setTileProperty(right.X, right.Y, layer, spawnable, "F");
				return;
			}
			if (farm.doesTileHaveProperty(right.X, right.Y, noFurniture, layer) != null && farm.doesTileHaveProperty(right.X, right.Y, spawnable, layer) != null)
			{
				farm.removeTileProperty(right.X, right.Y, layer, noFurniture);
				farm.removeTileProperty(right.X, right.Y, layer, spawnable);
				farm.setTileProperty(left.X, left.Y, layer, noFurniture, "T");
				farm.setTileProperty(left.X, left.Y, layer, spawnable, "F");
				return;
			}
		}
	}
}
