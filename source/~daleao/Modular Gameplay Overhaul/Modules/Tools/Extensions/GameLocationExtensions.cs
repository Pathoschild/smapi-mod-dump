/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Extensions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Extensions for the <see cref="GameLocation"/> class.</summary>
internal static class GameLocationExtensions
{
    /// <summary>Gets the <see cref="ResourceClump"/> in the <paramref name="location"/>.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of the <see cref="ResourceClump"/>.</returns>
    internal static IEnumerable<ResourceClump> GetNormalResourceClumps(this GameLocation location)
    {
        var clumps = location.resourceClumps.AsEnumerable();
        clumps = location switch
        {
            Forest { log: { } } forest => clumps.Concat(new[] { forest.log }),
            Woods woods when woods.stumps.Count > 0 => clumps.Concat(woods.stumps),
            _ => clumps,
        };

        return clumps;
    }

    /// <summary>Gets the <see cref="ResourceClump"/> which covers the given <paramref name="tile"/>, if any.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="tile">The tile to check.</param>
    /// <param name="who">The current player.</param>
    /// <param name="applyTool">Applies a tool to the resource clump.</param>
    /// <returns>The <see cref="ResourceClump"/> located at <paramref name="tile"/> if any, otherwise <see langword="null"/>.</returns>
    internal static ResourceClump? GetResourceClumpCoveringTile(
        this GameLocation location, Vector2 tile, Farmer who, out Func<Tool, bool>? applyTool)
    {
        var tileArea = tile.GetAbsoluteTileArea();

        // normal resource clumps
        foreach (var clump in location.GetNormalResourceClumps())
        {
            if (!clump.getBoundingBox(clump.tile.Value).Intersects(tileArea))
            {
                continue;
            }

            applyTool = tool => tool.UseOnTile(tile, location, who);
            return clump;
        }

        // FarmTypeManager resource clumps
        if (ModHelper.ModRegistry.IsLoaded("Esca.FarmTypeManager"))
        {
            for (var i = 0; i < location.largeTerrainFeatures.Count; i++)
            {
                var feature = location.largeTerrainFeatures[i];
                if (feature.GetType().FullName != "FarmTypeManager.LargeResourceClump" ||
                    !feature.getBoundingBox(feature.tilePosition.Value).Intersects(tileArea))
                {
                    continue;
                }

                var clump = ModHelper.Reflection
                    .GetField<NetRef<ResourceClump>>(feature, "Clump").GetValue().Value;
                applyTool = tool => feature.performToolAction(tool, 0, tile, location);
                return clump;
            }
        }

        applyTool = null;
        return null;
    }
}
