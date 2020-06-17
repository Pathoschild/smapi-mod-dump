using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System.Linq;
using SObject = StardewValley.Object;

namespace FlowerBombs
{
	public partial class FlowerBomb
	{
		private static readonly string[] FertileTypes = new string[]
			{ "Grass", "Dirt" };

		public static bool IsSuitableTile (GameLocation location,
			Vector2 tileVector, SObject toPlace = null, bool clearGrass = false)
		{
			Point tile = Utility.Vector2ToPoint (tileVector);

			// Must be placed outdoors and not on the beach.
			if (!location.IsOutdoors || location.Name.StartsWith ("Beach"))
				return false;

			// Must be a valid tile.
			if (!location.isTileOnMap (tileVector))
				return false;

			// Must be a grass or dirt tile.
			if (!FertileTypes.Contains (location.doesTileHaveProperty (tile.X,
					tile.Y, "Type", "Back")))
				return false;

			// Must be a tile that allows placement and spawning.
			if (!location.isTilePlaceable (tileVector) ||
					location.doesTileHaveProperty (tile.X, tile.Y,
						"NoSpawn", "Back") != null)
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
			return IsSuitableTile (location, tile, this);
		}

		public override bool placementAction (GameLocation location, int x,
			int y, Farmer who = null)
		{
			bool result = base.placementAction (location, x, y, who);
			if (result)
			{
				// Clear any grass on the tile.
				IsSuitableTile (location, new Vector2 (x / 64, y / 64),
					clearGrass: true);
			}
			return result;
		}

		public override bool performToolAction (Tool tool, GameLocation location)
		{
			if (!location.objects.TryGetValue (TileLocation, out SObject occupant) ||
					!this.Equals (occupant) || isTemporarilyInvisible)
				return false;

			if (tool is MeleeWeapon || (tool != null && !tool.isHeavyHitter ()))
				return false;

			location.playSound ("hammer");
			location.objects.Remove (TileLocation);
			location.debris.Add (new Debris (this, TileLocation * 64f +
				new Vector2 (32f, 32f)));

			return false;
		}
	}
}
