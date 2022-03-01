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

using System;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Buildings;

using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class FishPondAddFishToPondPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondAddFishToPondPatch()
    {
        Original = RequireMethod<FishPond>("addFishToPond");
    }

    #region harmony patches

    /// <summary>Patch to increment total Fish Pond quality rating.</summary>
    [HarmonyPostfix]
    private static void FishPondOnFisTypeChangedPostfix(FishPond __instance, SObject fish)
    {
        if (!ModEntry.Config.EnableFishPondRebalance) return;

        var qualityRating = __instance.ReadDataAs<int>("QualityRating");
        qualityRating += (int) Math.Pow(16, fish.Quality == SObject.bestQuality ? fish.Quality - 1 : fish.Quality);
        __instance.WriteData("QualityRating", qualityRating.ToString());
    }

    #endregion harmony patches
}