/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using Common.Extensions.Stardew;
using Extensions;
using HarmonyLib;
using System;

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
        if (__instance.IsBeeHouse() && ModEntry.Config.AgeImprovesBeeHouses)
        {
            __instance.Increment("Age");
        }
        else if (__instance.IsMushroomBox() && ModEntry.Config.AgeImprovesMushroomBoxes)
        {
            __instance.Increment("Age");
            if (__instance.heldObject.Value is null) return;

            __instance.heldObject.Value.Quality = ModEntry.ProfessionsApi is null
                ? Game1.player.professions.Contains(Farmer.botanist)
                    ? SObject.bestQuality
                    : __instance.GetQualityFromAge()
                : Math.Max(ModEntry.ProfessionsApi.GetEcologistForageQuality(Game1.player),
                    __instance.GetQualityFromAge());
        }
    }

    #endregion harmony patches
}