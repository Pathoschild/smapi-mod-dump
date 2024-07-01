/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using System.IO;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.GameData.FishPonds;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondAddFishToPondPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondAddFishToPondPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FishPondAddFishToPondPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FishPond>("addFishToPond");
    }

    #region harmony patches

    /// <summary>Distinguish extended family pairs + increment total Fish Pond quality ratings.</summary>
    [HarmonyPostfix]
    private static void FishPondAddFishToPondPostfix(FishPond __instance, FishPondData? ____fishPondData, SObject fish)
    {
        try
        {
            if (fish.ItemId == __instance.fishType.Value || !Lookups.FamilyPairs.ContainsKey(fish.QualifiedItemId))
            {
                return;
            }

            Data.Increment(__instance, DataKeys.FamilyLivingHere);

            // enable reproduction if angler or ms. angler
            if (fish.QualifiedItemId is not (QualifiedObjectIds.Angler or QualifiedObjectIds.MsAngler) || ____fishPondData is null)
            {
                return;
            }

            var familyLivingHere = Data.ReadAs<int>(__instance, DataKeys.FamilyLivingHere);
            var mates = Math.Min(__instance.FishCount - familyLivingHere, familyLivingHere);
            ____fishPondData.SpawnTime = 18 / mates;
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            Data.Write(__instance, DataKeys.FamilyLivingHere, null);
        }
    }

    #endregion harmony patches
}
