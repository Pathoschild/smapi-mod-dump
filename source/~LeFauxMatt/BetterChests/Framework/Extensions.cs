/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework;

using Microsoft.Xna.Framework;
using StardewMods.Common.Services.Integrations.BetterChests.Enums;

/// <summary>Extension methods for Better Chests.</summary>
internal static class Extensions
{
    /// <summary>Tests whether the player is within range of the location.</summary>
    /// <param name="range">The range.</param>
    /// <param name="distance">The distance in tiles to the player.</param>
    /// <param name="parent">The context where the source object is contained.</param>
    /// <param name="position">The coordinates.</param>
    /// <returns>true if the location is within range; otherwise, false.</returns>
    public static bool WithinRange(this RangeOption range, int distance, object parent, Vector2 position) =>
        range switch
        {
            RangeOption.World => true,
            RangeOption.Inventory when parent is Farmer farmer && farmer.Equals(Game1.player) => true,
            RangeOption.Default or RangeOption.Disabled or RangeOption.Inventory => false,
            RangeOption.Location when parent is GameLocation location && !location.Equals(Game1.currentLocation) =>
                false,
            RangeOption.Location when distance == -1 => true,
            RangeOption.Location when Math.Abs(position.X - Game1.player.Tile.X)
                + Math.Abs(position.Y - Game1.player.Tile.Y)
                <= distance => true,
            _ => false,
        };
}