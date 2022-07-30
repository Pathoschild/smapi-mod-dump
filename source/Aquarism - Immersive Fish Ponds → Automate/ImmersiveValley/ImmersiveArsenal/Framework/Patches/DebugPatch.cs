/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Menus;

namespace DaLion.Stardew.Arsenal.Framework.Patches;

[UsedImplicitly]
internal sealed class DebugPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal DebugPatch()
    {
        //Target = RequireMethod<ForgeMenu>("_ValidateCraft");
    }

    #region harmony patches

    /// <summary>Debug patch.</summary>
    [HarmonyPrefix]
    private static bool DebugPrefix(ForgeMenu __instance)
    {
        return false;
        //switch (__instance.type.Value)
        //{
        //    case MeleeWeapon.stabbingSword:
        //        __instance.addedAreaOfEffect.Value += 50;
        //        break;
        //    case MeleeWeapon.dagger:
        //        __instance.addedAreaOfEffect.Value += 50;
        //        break;
        //    case MeleeWeapon.club:
        //        __instance.addedAreaOfEffect.Value += 50;
        //        break;
        //    case MeleeWeapon.defenseSword:
        //        __instance.addedAreaOfEffect.Value += 50;
        //        break;
        //}
    }

    #endregion harmony patches
}