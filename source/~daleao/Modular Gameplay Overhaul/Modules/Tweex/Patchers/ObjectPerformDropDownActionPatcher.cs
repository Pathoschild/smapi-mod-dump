/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tweex.Patchers;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPerformDropDownActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectPerformDropDownActionPatcher"/> class.</summary>
    internal ObjectPerformDropDownActionPatcher()
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.performDropDownAction));
    }

    #region harmony patches

    /// <summary>Clear the age of bee houses and mushroom boxes.</summary>
    [HarmonyPostfix]
    private static void ObjectPerformDropDownActionPostfix(SObject __instance)
    {
        if (__instance.IsBeeHouse() || __instance.IsMushroomBox())
        {
            __instance.Write(DataFields.Age, null);
        }
    }

    #endregion harmony patches
}
