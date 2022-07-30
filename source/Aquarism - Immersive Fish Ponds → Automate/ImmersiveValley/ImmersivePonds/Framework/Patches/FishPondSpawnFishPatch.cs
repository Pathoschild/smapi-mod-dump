/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using Common;
using Common.Data;
using Common.Extensions;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.IO;
using System.Linq;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondSpawnFishPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondSpawnFishPatch()
    {
        Target = RequireMethod<FishPond>(nameof(FishPond.SpawnFish));
    }

    #region harmony patches

    /// <summary>Set the quality of newborn fishes.</summary>
    [HarmonyPostfix]
    private static void FishPondSpawnFishPostfix(FishPond __instance)
    {
        if (__instance.currentOccupants.Value >= __instance.maxOccupants.Value &&
            !__instance.hasSpawnedFish.Value) return;

        var r = new Random(Guid.NewGuid().GetHashCode());
        if (__instance.fishType.Value.IsAlgae())
        {
            var spawned = Utils.ChooseAlgae(__instance.fishType.Value, r);
            switch (spawned)
            {
                case Constants.SEAWEED_INDEX_I:
                    ModDataIO.Increment<int>(__instance, "SeaweedLivingHere");
                    break;
                case Constants.GREEN_ALGAE_INDEX_I:
                    ModDataIO.Increment<int>(__instance, "GreenAlgaeLivingHere");
                    break;
                case Constants.WHITE_ALGAE_INDEX_I:
                    ModDataIO.Increment<int>(__instance, "WhiteAlgaeLivingHere");
                    break;
            }

            return;
        }

        try
        {
            var forFamily = false;
            var familyCount = 0;
            if (__instance.HasLegendaryFish())
            {
                familyCount = ModDataIO.ReadFrom<int>(__instance, "FamilyLivingHere");
                if (0 > familyCount || familyCount > __instance.FishCount)
                    throw new InvalidDataException("FamilyLivingHere data is invalid.");

                if (familyCount > 0 &&
                    Game1.random.NextDouble() <
                    (double)familyCount /
                    (__instance.FishCount -
                     1)) // fish pond count has already been incremented at this point, so we consider -1;
                    forFamily = true;
            }

            var @default = forFamily
                ? $"{familyCount},0,0,0"
                : $"{__instance.FishCount - familyCount - 1},0,0,0";
            var qualities = ModDataIO.ReadFrom(__instance, forFamily ? "FamilyQualities" : "FishQualities", @default)
                .ParseList<int>()!;
            if (qualities.Count != 4 ||
                qualities.Sum() != (forFamily ? familyCount : __instance.FishCount - familyCount - 1))
                throw new InvalidDataException("FishQualities data had incorrect number of values.");

            if (qualities.Sum() == 0)
            {
                ++qualities[0];
                ModDataIO.WriteTo(__instance, forFamily ? "FamilyQualities" : "FishQualities",
                    string.Join(',', qualities));
                return;
            }

            var roll = r.Next(forFamily ? familyCount : __instance.FishCount - familyCount - 1);
            var fishlingQuality = roll < qualities[3]
                ? SObject.bestQuality
                : roll < qualities[3] + qualities[2]
                    ? SObject.highQuality
                    : roll < qualities[3] + qualities[2] + qualities[1]
                        ? SObject.medQuality
                        : SObject.lowQuality;

            ++qualities[fishlingQuality == 4 ? 3 : fishlingQuality];
            ModDataIO.WriteTo(__instance, forFamily ? "FamilyQualities" : "FishQualities", string.Join(',', qualities));
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            ModDataIO.WriteTo(__instance, "FishQualities", $"{__instance.FishCount},0,0,0");
            ModDataIO.WriteTo(__instance, "FamilyQualities", null);
            ModDataIO.WriteTo(__instance, "FamilyLivingHere", null);
        }
    }

    #endregion harmony patches
}