/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tweex.Extensions;

#region using directives

using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Extensions for the <see cref="Tree"/> class.</summary>
internal static class BushExtensions
{
    /// <summary>Gets an object quality value based on this <paramref name="bush"/> age.</summary>
    /// <param name="bush">The <see cref="Bush"/>.</param>
    /// <returns>A <see cref="SObject"/> quality value.</returns>
    internal static int GetQualityFromAge(this Bush bush)
    {
        var skillFactor = 1f + (Game1.player.FarmingLevel * 0.1f);
        if (ProfessionsModule.ShouldEnable && Game1.player.professions.Contains(Farmer.botanist))
        {
            skillFactor++;
        }

        var age = (int)((bush.getAge() - 20) * skillFactor * TweexModule.Config.TreeAgingFactor);
        if (TweexModule.Config.DeterministicAgeQuality)
        {
            return age switch
            {
                >= 336 => SObject.bestQuality,
                >= 224 => SObject.highQuality,
                >= 112 => SObject.medQuality,
                _ => SObject.lowQuality,
            };
        }

        return Game1.random.Next(age) switch
        {
            >= 336 => SObject.bestQuality,
            >= 224 => SObject.highQuality,
            >= 112 => SObject.medQuality,
            _ => SObject.lowQuality,
        };
    }
}
