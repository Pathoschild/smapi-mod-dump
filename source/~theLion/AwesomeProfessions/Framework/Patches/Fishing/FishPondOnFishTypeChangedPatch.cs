/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Buildings;

using Stardew.Common.Extensions;

#endregion using directives

[UsedImplicitly]
internal class FishPondOnFishTypeChangedPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondOnFishTypeChangedPatch()
    {
        Original = RequireMethod<FishPond>(nameof(FishPond.OnFishTypeChanged));
    }

    #region harmony patches

    /// <summary>Patch to reset total Fish Pond quality rating.</summary>
    [HarmonyPostfix]
    private static void FishPondOnFishTypeChangedPostfix(FishPond __instance)
    {
        if (!ModEntry.Config.EnableFishPondRebalance) return;

        var who = Game1.getFarmerMaybeOffline(__instance.owner.Value) ?? Game1.MasterPlayer;
        var qualityRatingByFishPond =
            ModData.Read(DataField.QualityRatingByFishPond, who).ToDictionary<int, int>(",", ";");
        var thisFishPond = __instance.GetCenterTile().ToString().GetDeterministicHashCode();
        qualityRatingByFishPond.Remove(thisFishPond);
        ModData.Write(DataField.QualityRatingByFishPond, qualityRatingByFishPond.ToString(",", ";"), who);
    }

    #endregion harmony patches
}