/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Ponds.Patchers;

#region using directives

using System.IO;
using System.Linq;
using DaLion.Overhaul.Modules.Ponds.Extensions;
using DaLion.Overhaul.Modules.Professions;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondSpawnFishPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondSpawnFishPatcher"/> class.</summary>
    internal FishPondSpawnFishPatcher()
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.SpawnFish));
    }

    #region harmony patches

    /// <summary>Set the quality of newborn fishes.</summary>
    [HarmonyPostfix]
    private static void FishPondSpawnFishPostfix(FishPond __instance)
    {
        if (__instance.currentOccupants.Value >= __instance.maxOccupants.Value &&
            !__instance.hasSpawnedFish.Value)
        {
            return;
        }

        var r = new Random(Guid.NewGuid().GetHashCode());
        try
        {
            if (__instance.fishType.Value.IsAlgaeIndex())
            {
                var spawned = r.NextAlgae(__instance.fishType.Value);
                switch (spawned)
                {
                    case Constants.SeaweedIndex:
                        __instance.Increment(DataFields.SeaweedLivingHere);
                        break;
                    case Constants.GreenAlgaeIndex:
                        __instance.Increment(DataFields.GreenAlgaeLivingHere);
                        break;
                    case Constants.WhiteAlgaeIndex:
                        __instance.Increment(DataFields.WhiteAlgaeLivingHere);
                        break;
                }

                var total = __instance.Read<int>(DataFields.SeaweedLivingHere) +
                            __instance.Read<int>(DataFields.GreenAlgaeLivingHere) +
                            __instance.Read<int>(DataFields.WhiteAlgaeLivingHere);
                if (total != __instance.FishCount)
                {
                    ThrowHelper.ThrowInvalidDataException(
                        "Mismatch between algae population data and actual population.");
                }

                return;
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            __instance.Write(DataFields.SeaweedLivingHere, null);
            __instance.Write(DataFields.GreenAlgaeLivingHere, null);
            __instance.Write(DataFields.WhiteAlgaeLivingHere, null);
            var field = __instance.fishType.Value switch
            {
                Constants.SeaweedIndex => DataFields.SeaweedLivingHere,
                Constants.GreenAlgaeIndex => DataFields.GreenAlgaeLivingHere,
                Constants.WhiteAlgaeIndex => DataFields.WhiteAlgaeLivingHere,
                _ => string.Empty,
            };

            __instance.Write(field, __instance.FishCount.ToString());
        }

        try
        {
            var forFamily = false;
            var familyCount = 0;
            if (__instance.HasLegendaryFish())
            {
                familyCount = __instance.Read<int>(DataFields.FamilyLivingHere);
                if (familyCount < 0 || familyCount > __instance.FishCount)
                {
                    ThrowHelper.ThrowInvalidDataException(
                        "FamilyLivingHere data is negative or greater than actual population.");
                }

                if (familyCount > 0 &&
                    Game1.random.NextDouble() < (double)familyCount / (__instance.FishCount - 1)) // fish pond count has already been incremented at this point, so we consider -1
                {
                    forFamily = true;
                }
            }

            var @default = forFamily
                ? $"{familyCount},0,0,0"
                : $"{__instance.FishCount - familyCount - 1},0,0,0";
            var qualities = __instance
                .Read(forFamily ? DataFields.FamilyQualities : DataFields.FishQualities, @default)
                .ParseList<int>();
            if (qualities.Count != 4 ||
                qualities.Sum() != (forFamily ? familyCount : __instance.FishCount - familyCount - 1))
            {
                ThrowHelper.ThrowInvalidDataException("Mismatch between FishQualities data and actual population.");
            }

            if (qualities.Sum() == 0)
            {
                qualities[0]++;
                __instance.Write(forFamily ? DataFields.FamilyQualities : DataFields.FishQualities, string.Join(',', qualities));
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

            if (ProfessionsModule.IsEnabled && fishlingQuality < SObject.bestQuality && Game1.random.NextDouble() < 0.5 &&
                (__instance.GetOwner().HasProfession(Profession.Aquarist) ||
                 (ProfessionsModule.Config.LaxOwnershipRequirements &&
                  Game1.game1.DoesAnyPlayerHaveProfession(Profession.Aquarist, out _))))
            {
                fishlingQuality += fishlingQuality == SObject.highQuality ? 2 : 1;
            }

            qualities[fishlingQuality == SObject.bestQuality ? 3 : fishlingQuality]++;
            __instance.Write(forFamily ? DataFields.FamilyQualities : DataFields.FishQualities, string.Join(',', qualities));
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            __instance.Write(DataFields.FishQualities, $"{__instance.FishCount},0,0,0");
            __instance.Write(DataFields.FamilyQualities, null);
            __instance.Write(DataFields.FamilyLivingHere, null);
        }
    }

    #endregion harmony patches
}
