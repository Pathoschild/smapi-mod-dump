/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Tweaks.Extensions;

#region using directives

using StardewValley;
using StardewValley.TerrainFeatures;

using Common.Extensions;
using Common.Extensions.Stardew;

using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Extensions for the <see cref="Tree"/> class.</summary>
public static class FruitTreeExtensions
{
    /// <summary>Get an object quality value based on this tree's age.</summary>
    public static int GetQualityFromAge(this FruitTree tree)
    {
        var skillFactor = 1f + Game1.player.FarmingLevel * 0.1f;
        var age = tree.daysUntilMature.Value < 0 ? tree.daysUntilMature.Value * -1 : 0;
        age = (int) (age * skillFactor * ModEntry.Config.AgeImproveQualityFactor);
        if (ModEntry.HasProfessionsMod && Game1.player.professions.Contains(Farmer.lumberjack)) age *= 2;

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