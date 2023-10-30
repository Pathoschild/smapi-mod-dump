/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Extensions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;

#endregion using directives

/// <summary>Extensions for the <see cref="GameLocation"/> class.</summary>
internal static class GameLocationExtensions
{
    /// <summary>
    ///     Determines whether any <see cref="Farmer"/> in this <paramref name="location"/> has the specified
    ///     <paramref name="profession"/>.
    /// </summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if the <paramref name="location"/> has at least one <see cref="Farmer"/> with the specified <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool DoesAnyPlayerHereHaveProfession(this GameLocation location, IProfession profession, bool prestiged = false)
    {
        if (!Context.IsMultiplayer && location.Equals(Game1.currentLocation))
        {
            return Game1.player.HasProfession(profession, prestiged);
        }

        return location.farmers.Any(farmer => farmer.HasProfession(profession, prestiged));
    }

    /// <summary>
    ///     Determines whether any <see cref="Farmer"/> in this <paramref name="location"/> has the specified
    ///     <paramref name="profession"/> and gets a <see cref="List{T}"/> of those <see cref="Farmer"/>s.
    /// </summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    /// <param name="farmers">All the farmer instances in the location with the given profession.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if the <paramref name="location"/> has at least one <see cref="Farmer"/> with the specified <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool DoesAnyPlayerHereHaveProfession(
        this GameLocation location, IProfession profession, out List<Farmer> farmers, bool prestiged = false)
    {
        farmers = new List<Farmer>();
        if (!Context.IsMultiplayer && location.Equals(Game1.player.currentLocation) &&
            Game1.player.HasProfession(profession, prestiged))
        {
            farmers.Add(Game1.player);
        }
        else
        {
            foreach (var farmer in location.farmers)
            {
                if (farmer.HasProfession(profession, prestiged))
                {
                    farmers.Add(farmer);
                }
            }
        }

        return farmers.Count > 0;
    }

    /// <summary>Determines whether this <paramref name="location"/> is a dungeon.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="location"/> is a <see cref="MineShaft"/> or one of several recognized dungeon locations, otherwise <see langword="false"/>.</returns>
    /// <remarks>Includes locations from Stardew Valley Expanded, Ridgeside Village and Moon Misadventures.</remarks>
    internal static bool IsDungeon(this GameLocation location)
    {
        return location is MineShaft or BugLand or VolcanoDungeon ||
               location.NameOrUniqueName.ContainsAnyOf(
                   "CrimsonBadlands",
                   "DeepWoods",
                   "Highlands",
                   "RidgeForest",
                   "SpiritRealm",
                   "AsteroidsDungeon");
    }

    /// <summary>Determines whether a <paramref name="tile"/> on a map is valid for spawning diggable treasure.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="tile">The tile to check.</param>
    /// <returns><see langword="true"/> if the <paramref name="tile"/> is completely clear of any <see cref="SObject"/>, <see cref="TerrainFeature"/> or other map property that would make it inaccessible, otherwise <see langword="false"/>.</returns>
    internal static bool IsTileValidForTreasure(this GameLocation location, Vector2 tile)
    {
        return (!location.objects.TryGetValue(tile, out var @object) || @object == null) &&
               location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Spawnable", "Back") is not null &&
               !location.doesEitherTileOrTileIndexPropertyEqual((int)tile.X, (int)tile.Y, "Spawnable", "Back", "F") &&
               location.isTileLocationTotallyClearAndPlaceable(tile) &&
               location.getTileIndexAt((int)tile.X, (int)tile.Y, "AlwaysFront") == -1 &&
               location.getTileIndexAt((int)tile.X, (int)tile.Y, "Front") == -1 && !location.isBehindBush(tile) &&
               !location.isBehindTree(tile);
    }

    /// <summary>Determines whether a <paramref name="tile"/> is clear of <see cref="Debris"/>.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="tile">The tile to check.</param>
    /// <returns><see langword="true"/> if the <paramref name="tile"/> is clear of <see cref="Debris"/>, otherwise <see langword="false"/>.</returns>
    internal static bool IsTileClearOfDebris(this GameLocation location, Vector2 tile)
    {
        return (from debris in location.debris
            where debris.item is not null && debris.Chunks.Count > 0
            select new Vector2(
                (int)(debris.Chunks[0].position.X / Game1.tileSize) + 1,
                (int)(debris.Chunks[0].position.Y / Game1.tileSize) + 1))
            .All(debrisTile => debrisTile != tile);
    }

    /// <summary>Forces a <paramref name="tile"/> to be susceptible to a <see cref="StardewValley.Tools.Hoe"/>.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="tile">The tile to change.</param>
    /// <returns><see langword="true"/> if the <paramref name="tile"/>'s "Diggable" property was changed, otherwise <see langword="false"/>.</returns>
    internal static bool MakeTileDiggable(this GameLocation location, Vector2 tile)
    {
        var (x, y) = tile;
        if (location.doesTileHaveProperty((int)x, (int)y, "Diggable", "Back") is not null)
        {
            return true;
        }

        var digSpot = new Location((int)x * Game1.tileSize, (int)y * Game1.tileSize);
        location.Map.GetLayer("Back").PickTile(digSpot, Game1.viewport.Size).Properties["Diggable"] = true;
        return false;
    }
}
