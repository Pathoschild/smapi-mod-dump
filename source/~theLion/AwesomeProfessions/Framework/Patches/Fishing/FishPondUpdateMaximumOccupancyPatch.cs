/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches;

[UsedImplicitly]
internal class FishPondUpdateMaximumOccupancyPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondUpdateMaximumOccupancyPatch()
    {
        Original = RequireMethod<FishPond>(nameof(FishPond.UpdateMaximumOccupancy));
    }

    #region harmony patches

    /// <summary>Patch for Aquarist increased max fish pond capacity.</summary>
    [HarmonyPostfix]
    private static void FishPondUpdateMaximumOccupancyPostfix(ref FishPond __instance,
        FishPondData ____fishPondData)
    {
        if (__instance is null || ____fishPondData is null) return;

        var owner = Game1.getFarmerMaybeOffline(__instance.owner.Value) ?? Game1.MasterPlayer;
        if (owner.HasProfession("Aquarist") && (____fishPondData.PopulationGates is null ||
                                                __instance.lastUnlockedPopulationGate.Value >=
                                                ____fishPondData.PopulationGates.Keys.Max()))
            __instance.maxOccupants.Set(12);
    }

    #endregion harmony patches
}