/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Ponds.Patchers;

#region using directives

using System.IO;
using System.Linq;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondAddFishToPondPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondAddFishToPondPatcher"/> class.</summary>
    internal FishPondAddFishToPondPatcher()
    {
        this.Target = this.RequireMethod<FishPond>("addFishToPond");
    }

    #region harmony patches

    /// <summary>Distinguish extended family pairs + increment total Fish Pond quality ratings.</summary>
    [HarmonyPostfix]
    private static void FishPondAddFishToPondPostfix(FishPond __instance, FishPondData ____fishPondData, SObject fish)
    {
        try
        {
            if (fish.HasContextTag("fish_legendary"))
            {
                if (fish.ParentSheetIndex != __instance.fishType.Value)
                {
                    var familyQualities = __instance
                        .Read(DataKeys.FamilyQualities, $"{__instance.Read<int>(DataKeys.FamilyLivingHere)},0,0,0")
                        .ParseList<int>();
                    if (familyQualities.Count != 4 ||
                        familyQualities.Sum() != __instance.Read<int>(DataKeys.FamilyLivingHere))
                    {
                        ThrowHelper.ThrowInvalidDataException("FamilyQualities data had incorrect number of values.");
                    }

                    familyQualities[fish.Quality == 4 ? 3 : fish.Quality]++;
                    __instance.Increment(DataKeys.FamilyLivingHere);
                    __instance.Write(DataKeys.FamilyQualities, string.Join(',', familyQualities));
                }
                else
                {
                    var fishQualities = __instance.Read(DataKeys.FishQualities, $"{__instance.FishCount - __instance.Read<int>(DataKeys.FamilyLivingHere) - 1},0,0,0") // already added at this point, so consider - 1
                        .ParseList<int>();
                    if (fishQualities.Count != 4 || fishQualities.Any(q => q < 0 || q > __instance.FishCount - 1))
                    {
                        ThrowHelper.ThrowInvalidDataException("FishQualities data had incorrect number of values.");
                    }

                    fishQualities[fish.Quality == 4 ? 3 : fish.Quality]++;
                    __instance.Write(DataKeys.FishQualities, string.Join(',', fishQualities));
                }

                // enable reproduction if angler or ms. angler
                if (fish.ParentSheetIndex is not (160 or 899) ||
                    __instance.Read<int>(DataKeys.FamilyLivingHere) is not ({ } familyCount and > 0))
                {
                    return;
                }

                var mates = Math.Min(__instance.FishCount - familyCount, familyCount);
                ____fishPondData.SpawnTime = 12 / mates;
            }
            else if (fish.IsAlgae())
            {
                switch (fish.ParentSheetIndex)
                {
                    case ObjectIds.Seaweed:
                        __instance.Increment(DataKeys.SeaweedLivingHere);
                        break;
                    case ObjectIds.GreenAlgae:
                        __instance.Increment(DataKeys.GreenAlgaeLivingHere);
                        break;
                    case ObjectIds.WhiteAlgae:
                        __instance.Increment(DataKeys.WhiteAlgaeLivingHere);
                        break;
                }
            }
            else
            {
                var fishQualities = __instance.Read(DataKeys.FishQualities, $"{__instance.FishCount - __instance.Read<int>(DataKeys.FamilyLivingHere) - 1},0,0,0") // already added at this point, so consider - 1
                    .ParseList<int>();
                if (fishQualities.Count != 4 || fishQualities.Any(q => q < 0 || q > __instance.FishCount - 1))
                {
                    ThrowHelper.ThrowInvalidDataException("FishQualities data had incorrect number of values.");
                }

                fishQualities[fish.Quality == 4 ? 3 : fish.Quality]++;
                __instance.Write(DataKeys.FishQualities, string.Join(',', fishQualities));
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"[PNDS]: {ex}\nThe data will be reset.");
            __instance.Write(DataKeys.FishQualities, $"{__instance.FishCount},0,0,0");
            __instance.Write(DataKeys.FamilyQualities, null);
            __instance.Write(DataKeys.FamilyLivingHere, null);
        }
    }

    #endregion harmony patches
}
