/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Tweex.Extensions;

#region using directives

using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Extensions for the <see cref="Tree"/> class.</summary>
public static class FruitTreeExtensions
{
    /// <summary>Get an object quality value based on this fruit tree's age.</summary>
    public static int GetQualityFromAge(this FruitTree tree)
    {
        var skillFactor = 1f + Game1.player.FarmingLevel * 0.1f;
        if (ModEntry.ProfessionsApi is not null && Game1.player.professions.Contains(Farmer.lumberjack)) ++skillFactor;

        var age = tree.daysUntilMature.Value < 0 ? tree.daysUntilMature.Value * -1 : 0;
        age = (int)(age * skillFactor * ModEntry.Config.AgeImproveQualityFactor);
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