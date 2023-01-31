/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

#if DEBUG
namespace DaLion.Overhaul.Modules.Ponds.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondGetFishPondDataPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondGetFishPondDataPatcher"/> class.</summary>
    internal FishPondGetFishPondDataPatcher()
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.GetFishPondData));
    }

    #region harmony patches

    /// <summary>Replace single production with multi-yield production.</summary>
    [HarmonyPrefix]
    // ReSharper disable once RedundantAssignment
    private static bool FishPondGetFishPondDataPrefix(FishPond __instance, ref FishPondData? __result)
    {
        if (__instance.fishType.Value <= 0)
        {
            __result = null;
            return false;
        }

        var list = Game1.content.Load<List<FishPondData>>("Data\\FishPondData");
        var fish = __instance.GetFishObject();
        for (var i = 0; i < list.Count; i++)
        {
            var entry = list[i];
            if (entry.RequiredTags.Any(required => !fish.HasContextTag(required)))
            {
                continue;
            }

            if (entry.SpawnTime == -1)
            {
                entry.SpawnTime = fish.Price switch
                {
                    <= 30 => 1,
                    <= 80 => 2,
                    <= 120 => 3,
                    <= 250 => 4,
                    _ => 5,
                };
            }

            __instance.GetType().RequireField("_fishPondData").SetValue(__instance, entry);
            __result = entry;
            return false;
        }

        __result = null;
        return false;
    }

    #endregion harmony patches
}

#endif
