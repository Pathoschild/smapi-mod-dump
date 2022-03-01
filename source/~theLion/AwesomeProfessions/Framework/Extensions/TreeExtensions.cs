/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

using Common.Extensions;

#endregion using directives

internal static class TreeExtensions
{
    /// <summary>Whether a given common tree satisfies all conditions to advance a stage.</summary>
    public static bool CanGrow(this Tree tree)
    {
        var tileLocation = tree.currentTileLocation;
        var environment = tree.currentLocation;
        if (Game1.GetSeasonForLocation(tree.currentLocation) == "winter" &&
            !tree.treeType.Value.IsAnyOf(Tree.palmTree, Tree.palmTree2) &&
            !environment.CanPlantTreesHere(-1, (int) tileLocation.X, (int) tileLocation.Y) &&
            !tree.fertilized.Value)
            return false;

        var s = environment.doesTileHaveProperty((int) tileLocation.X, (int) tileLocation.Y, "NoSpawn", "Back");
        if (s is not null && s.IsAnyOf("All", "Tree", "True")) return false;

        var growthRect = new Rectangle((int) ((tileLocation.X - 1f) * 64f), (int) ((tileLocation.Y - 1f) * 64f),
            192, 192);
        switch (tree.growthStage.Value)
        {
            case 4:
            {
                foreach (var pair in environment.terrainFeatures.Pairs)
                    if (pair.Value is Tree otherTree && !otherTree.Equals(tree) &&
                        otherTree.growthStage.Value >= 5 &&
                        otherTree.getBoundingBox(pair.Key).Intersects(growthRect))
                        return false;
                break;
            }
            case 0 when environment.objects.ContainsKey(tileLocation):
                return false;
        }

        return true;
    }
}