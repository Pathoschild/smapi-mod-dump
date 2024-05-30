/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Freaksaus/Tileman-Redux
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using xTile.Dimensions;

namespace TilemanRedux;

internal static class Extensions
{
	public static bool isTileLocationTotallyClearAndPlaceable(this GameLocation location, int x, int y)
	{
		return location.isTileLocationTotallyClearAndPlaceable(new Vector2(x, y));
	}

	public static bool isTileLocationTotallyClearAndPlaceable(this GameLocation location, Vector2 v)
	{
		Vector2 pixel = new((v.X * Game1.tileSize) + Game1.tileSize / 2, (v.Y * Game1.tileSize) + Game1.tileSize / 2);
		foreach (Furniture f in location.furniture)
		{
			if (f.furniture_type.Value != Furniture.rug && !f.isPassable() && f.GetBoundingBox().Contains((int)pixel.X, (int)pixel.Y) && !f.AllowPlacementOnThisTile((int)v.X, (int)v.Y))
				return false;
		}

		return location.isTileOnMap(v) && !location.isTileOccupied(v) && location.isTilePassable(new Location((int)v.X, (int)v.Y), Game1.viewport) && location.isTilePlaceable(v);
	}

	public static bool isTileOccupied(this GameLocation location, Vector2 tileLocation, bool ignoreAllCharacters = false)
	{
		CollisionMask mask = ignoreAllCharacters ? CollisionMask.All & ~CollisionMask.Characters & ~CollisionMask.Farmers : CollisionMask.All;
		return location.IsTileOccupiedBy(tileLocation, mask);
	}
}
