/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Extensions;

#region using directives

using System.Linq;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;

#endregion using directives

/// <summary>Extensions for the <see cref="FishPond"/> class.</summary>
internal static class FishPondExtensions
{
    /// <summary>Determines whether the <paramref name="pond"/>'s population has been fully unlocked.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <returns><see langword="true"/> if the last unlocked population gate matches the last gate in the <see cref="FishPondData"/>, otherwise <see langword="false"/>.</returns>
    internal static bool HasUnlockedFinalPopulationGate(this FishPond pond)
    {
        var data = Reflector
            .GetUnboundFieldGetter<FishPond, FishPondData?>(pond, "_fishPondData")
            .Invoke(pond);
        return data?.PopulationGates is null ||
               pond.lastUnlockedPopulationGate.Value >= data.PopulationGates.Keys.Max();
    }

    /// <summary>Determines whether a legendary fish lives in this <paramref name="pond"/>.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="pond"/> houses a legendary fish species, otherwise <see langword="false"/>.</returns>
    internal static bool HasLegendaryFish(this FishPond pond)
    {
        return pond.GetFishObject().IsLegendaryFish();
    }
}
