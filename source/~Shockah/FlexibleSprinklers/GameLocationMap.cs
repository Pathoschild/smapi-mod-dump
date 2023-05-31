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
using Shockah.Kokoro;
using Shockah.Kokoro.Map;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace Shockah.FlexibleSprinklers
{
	internal class GameLocationMap : IMap<SoilType>.WithKnownSize
	{
		private readonly GameLocation Location;
		private readonly IEnumerable<Func<GameLocation, IntPoint, bool?>> CustomWaterableTileProviders;

		public IntRectangle Bounds
			=> new(IntPoint.Zero, Location.Map.DisplayWidth / Game1.tileSize, Location.Map.DisplayHeight / Game1.tileSize);

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
					bool? result = provider(Location, point);
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

		internal GameLocationMap(GameLocation location, IEnumerable<Func<GameLocation, IntPoint, bool?>> customWaterableTileProviders)
		{
			this.Location = location;
			this.CustomWaterableTileProviders = customWaterableTileProviders;
		}

		public override bool Equals(object? obj)
			=> obj is GameLocationMap map && (ReferenceEquals(Location, map.Location) || Location == map.Location);

		public override int GetHashCode()
			=> Location.GetHashCode();
	}
}