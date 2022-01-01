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
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;

namespace TheLion.Stardew.Professions.Framework.Patches;

[UsedImplicitly]
internal class FishPondGetFishPondDataPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondGetFishPondDataPatch()
    {
        Original = RequireMethod<FishPond>(nameof(FishPond.GetFishPondData));
    }

    #region harmony patches

    /// <summary>Patch to get fish pond data for legendary fish.</summary>
    [HarmonyPrefix]
    private static bool FishPondGetFishPondDataPrefix(ref FishPond __instance, ref FishPondData __result, ref FishPondData ____fishPondData)
    {
        if (__instance.fishType.Value <= 0) return true; // run original logic
			
        var fish_item = __instance.GetFishObject();
        if (!Utility.Objects.LegendaryFishNames.Contains(fish_item.Name)) return true; // run original logic
			
        ____fishPondData = new()
        {
            PopulationGates = null,
            ProducedItems = new()
            {
                new()
                {
                    Chance = 0.9f,
                    ItemID = 812, // roe
                    MinQuantity = 1,
                    MaxQuantity = 1
                }
            },
            RequiredTags = new(),
            SpawnTime = 7
        };
        __result = ____fishPondData;
        return false; // don't run original logic
    }

    #endregion harmony patches
}