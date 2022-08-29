/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Shockah.CommonModCode;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace Shockah.FlexibleSprinklers
{
	internal class GameLocationMap : IMap.WithKnownSize
	{
		private readonly GameLocation Location;
		private readonly IEnumerable<Func<GameLocation, Vector2, bool?>> CustomWaterableTileProviders;

		public int Width
			=> Location.Map.DisplayWidth / Game1.tileSize;

		public int Height
			=> Location.Map.DisplayHeight / Game1.tileSize;

		internal GameLocationMap(GameLocation location, IEnumerable<Func<GameLocation, Vector2, bool?>> customWaterableTileProviders)
		{
			this.Location = location;
			this.CustomWaterableTileProviders = customWaterableTileProviders;
		}

		public override bool Equals(object? obj)
			=> obj is IMap other && Equals(other);

		public bool Equals(IMap? other)
			=> other is GameLocationMap map && (ReferenceEquals(Location, map.Location) || Location == map.Location);

		public override int GetHashCode()
			=> Location.GetHashCode();

		public SoilType this[IntPoint point]
		{
			get
			{
				var tileVector = new Vector2(point.X, point.Y);

				if (Location.Objects.TryGetValue(tileVector, out SObject @object))
				{
					if (@object.IsSprinkler())
						return SoilType.Sprinkler;
					if (FlexibleSprinklers.Instance.Config.WaterGardenPots && @object is IndoorPot)
						return SoilType.Waterable;
				}

				foreach (var provider in CustomWaterableTileProviders)
				{
					bool? result = provider(Location, tileVector);
					if (result.HasValue)
						return result.Value ? SoilType.Waterable : SoilType.NonWaterable;
				}

				if (!Location.terrainFeatures.TryGetValue(tileVector, out TerrainFeature feature) || feature is not HoeDirt)
					return SoilType.NonWaterable;
				if (Location.doesTileHaveProperty(point.X, point.Y, "NoSprinklers", "Back")?.StartsWith("T", StringComparison.InvariantCultureIgnoreCase) == true)
					return SoilType.NonWaterable;
				return SoilType.Waterable;
			}
		}

		public void WaterTile(IntPoint point)
		{
			var can = new WateringCan();
			var tileVector = new Vector2(point.X, point.Y);

			if (Location.terrainFeatures.TryGetValue(tileVector, out TerrainFeature feature))
				feature.performToolAction(can, 0, tileVector, Location);
			if (Location.Objects.TryGetValue(tileVector, out SObject @object))
				@object.performToolAction(can, Location);
			Location.performToolAction(can, point.X, point.Y);

			// TODO: add animation, if needed
		}

		public IEnumerable<(IntPoint position, SprinklerInfo info)> GetAllSprinklers()
		{
			return Location.Objects.Values
				.Where(o => o.IsSprinkler())
				.Select(s => (position: new IntPoint((int)s.TileLocation.X, (int)s.TileLocation.Y), info: FlexibleSprinklers.Instance.GetSprinklerInfo(s)));
		}
	}
}