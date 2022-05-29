/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;

using Extensions;

#endregion using directives

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
    private static void FishPondUpdateMaximumOccupancyPostfix(FishPond __instance,
        FishPondData ____fishPondData)
    {
        if (__instance is null) return;

        if (__instance.IsLegendaryPond())
        {
            __instance.maxOccupants.Set(6);
        }
        else if (____fishPondData is not null)
        {
            var owner = Game1.getFarmerMaybeOffline(__instance.owner.Value) ?? Game1.MasterPlayer;
            if (owner.HasProfession(Profession.Aquarist) && __instance.HasUnlockedFinalPopulationGate())
                __instance.maxOccupants.Set(12);
        }
    }

    #endregion harmony patches
}