/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/flowerbombs
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace FlowerBombs
{
	public partial class FlowerBomb
	{
		public static bool IsSuitableTile (GameLocation location,
			Vector2 tileVector, SObject toPlace = null, bool clearGrass = false)
		{
			Point tile = Utility.Vector2ToPoint (tileVector);

			// Must be outdoors.
			if (!location.IsOutdoors && !location.treatAsOutdoors.Value)
				return false;

			// Must be a valid tile.
			if (!location.isTileOnMap (tileVector))
				return false;

			// Must be a grass or dirt tile.
			var type = location.doesTileHaveProperty (tile.X, tile.Y, "Type", "Back");
			if (type != "Grass" && type != "Dirt")
				return false;

			// Must be a tile that allows placement and spawning.
			var noSpawn = location.doesTileHaveProperty (tile.X, tile.Y, "NoSpawn", "Back");
			if (!location.isTilePlaceable (tileVector) || noSpawn != null)
				return false;

			// Remove any grass on the tile if requested.
			if (clearGrass && location.terrainFeatures.ContainsKey (tileVector) &&
					location.terrainFeatures[tileVector] is Grass)
				location.terrainFeatures.Remove (tileVector);

			// Must be an unoccupied, passable tile.
			if (location.isTileOccupiedForPlacement (tileVector, toPlace))
				return false;

			return true;
		}

		public override bool canBePlacedHere (GameLocation location,
			Vector2 tile)
		{
			return IsSuitableTile (location, tile, Base);
		}

		public override bool placementAction (GameLocation location, int x,
			int y, Farmer who = null)
		{
			// Replicate relevant logic from SObject.placementAction. There is
			// no working way to call it here.
			Vector2 placementTile = new (x / 64, y / 64);
			Helper.Reflection.GetField<int> (Base, "health").SetValue (10);
			Base.owner.Value = who?.UniqueMultiplayerID ?? Game1.player.UniqueMultiplayerID;
			SObject placed = (SObject) getOne ();
			placed.TileLocation = placementTile;
			placed.performDropDownAction (who);
			if (location.objects.ContainsKey (placementTile))
			{
				if (location.objects[placementTile].ParentSheetIndex != TileIndex)
				{
					Game1.createItemDebris (location.objects[placementTile],
						placementTile * 64f, Game1.random.Next (4));
					location.objects[placementTile] = placed;
				}
			}
			else
			{
				location.objects.Add (placementTile, placed);
			}

			// Clear any grass on the tile.
			IsSuitableTile (location, new Vector2 (x / 64, y / 64),
				clearGrass: true);

			location.playSound ("dirtyHit");
			return true;
		}

		public override bool performToolAction (Tool tool, GameLocation location)
		{
			if (!location.objects.TryGetValue (Base.TileLocation, out SObject occupant) ||
					!Base.Equals (occupant) || isTemporarilyInvisible)
				return false;

			if (tool is WateringCan can && can.WaterLeft > 0)
			{
				int health = Base.getHealth () - 1;
				if (health <= 0)
					germinateLive (location, Base.TileLocation);
				else
					Base.setHealth (health);
				return false;
			}

			if (tool is not MeleeWeapon && (tool?.isHeavyHitter () ?? true))
			{
				location.playSound ("hammer");
				location.objects.Remove (Base.TileLocation);
				location.debris.Add (new Debris (Base, Base.TileLocation * 64f +
					new Vector2 (32f, 32f)));
				return false;
			}

			return false;
		}
	}
}
