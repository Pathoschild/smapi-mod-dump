/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Tweex.Extensions;

#region using directives

using Common.Data;
using StardewValley;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Extensions for the <see cref="Tree"/> class.</summary>
public static class TreeExtensions
{
    /// <summary>Whether a given tree can hold a Tapper.</summary>
    public static bool CanBeTapped(this Tree tree) =>
        tree.treeType.Value is Tree.bushyTree or Tree.leafyTree or Tree.pineTree or Tree.mushroomTree
            or Tree.mahoganyTree;

    /// <summary>Get a string representation of a given tree's species.</summary>
    public static string NameFromType(this Tree tree) =>
        tree.treeType.Value switch
        {
            Tree.bushyTree => "Oak Tree",
            Tree.leafyTree => "Mahogany Tree",
            Tree.pineTree => "Pine Tree",
            Tree.winterTree1 => "Winter Tree",
            Tree.winterTree2 => "Winter Tree 2",
            Tree.palmTree => "Palm Tree",
            Tree.mushroomTree => "Mushroom Tree",
            Tree.mahoganyTree => "Mahogany Tree",
            Tree.palmTree2 => "Palm Tree 2",
            _ => "Unknown Tree"
        };

    /// <summary>Get an object quality value based on this tree's age.</summary>
    public static int GetQualityFromAge(this Tree tree)
    {
        var skillFactor = 1f + Game1.player.ForagingLevel * 0.1f;
        if (ModEntry.ProfessionsAPI is not null && Game1.player.professions.Contains(Farmer.lumberjack)) ++skillFactor;

        var age = (int)(ModDataIO.ReadFrom<int>(tree, "Age") * skillFactor * ModEntry.Config.AgeImproveQualityFactor);
        if (ModEntry.Config.DeterministicAgeQuality)
        {
            return age switch
            {
                >= 336 => SObject.bestQuality,
                >= 224 => SObject.highQuality,
                >= 112 => SObject.medQuality,
                _ => SObject.lowQuality
            };
        }

        return Game1.random.Next(age) switch
        {
            >= 336 => SObject.bestQuality,
            >= 224 => SObject.highQuality,
            >= 112 => SObject.medQuality,
            _ => SObject.lowQuality
        };
    }
}