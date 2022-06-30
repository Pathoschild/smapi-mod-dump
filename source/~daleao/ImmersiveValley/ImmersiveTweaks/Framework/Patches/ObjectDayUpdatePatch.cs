/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using Common.Data;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using System;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectDayUpdatePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectDayUpdatePatch()
    {
        Target = RequireMethod<SObject>(nameof(SObject.DayUpdate));
        Postfix!.priority = Priority.LowerThanNormal;
    }

    #region harmony patches

    /// <summary>Age bee houses and mushroom boxes.</summary>
    [HarmonyPostfix]
    [HarmonyPriority(Priority.LowerThanNormal)]
    private static void ObjectDayUpdatePostfix(SObject __instance)
    {
        if (__instance.IsBeeHouse() && ModEntry.Config.AgeBeeHouses)
        {
            ModDataIO.IncrementData<int>(__instance, "Age");
        }
        else if (__instance.IsMushroomBox())
        {
            if (ModEntry.Config.AgeMushroomBoxes) ModDataIO.IncrementData<int>(__instance, "Age");

            if (__instance.heldObject.Value is null) return;

            __instance.heldObject.Value.Quality = ModEntry.ProfessionsAPI is null
                ? __instance.GetQualityFromAge()
                : Math.Max(ModEntry.ProfessionsAPI.GetEcologistForageQuality(Game1.player),
                    __instance.heldObject.Value.Quality);
        }
    }

    #endregion harmony patches
}