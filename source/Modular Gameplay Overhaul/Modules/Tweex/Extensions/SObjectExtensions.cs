/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tweex.Extensions;

#region using directives

using DaLion.Shared.Extensions.Stardew;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
internal static class SObjectExtensions
{
    /// <summary>Gets an object quality value based on this <paramref name="object"/>'s age.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns>A <see cref="SObject"/> quality value.</returns>
    internal static int GetQualityFromAge(this SObject @object)
    {
        float skillFactor;
        int age;
        if (@object.IsBeeHouse())
        {
            skillFactor = 1f + (Game1.player.FarmingLevel * 0.1f);
            age = (int)(@object.Read<int>(DataKeys.Age) * skillFactor * TweexModule.Config.BeeHouseAgingFactor);
        }
        else if (@object.IsMushroomBox())
        {
            skillFactor = 1f + (Game1.player.ForagingLevel * 0.1f);
            age = (int)(@object.Read<int>(DataKeys.Age) * skillFactor * TweexModule.Config.MushroomBoxAgingFactor);
        }
        else
        {
            age = @object.Read<int>(DataKeys.Age);
        }

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
