/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using DaLion.Common;
using HarmonyLib;
using StardewValley.Menus;
using System;
using System.Reflection;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuGetProfessionNamePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal LevelUpMenuGetProfessionNamePatch()
    {
        Target = RequireMethod<LevelUpMenu>("getProfessionName");
    }

    #region harmony patches

    /// <summary>Patch to apply modded profession names.</summary>
    [HarmonyPrefix]
    private static bool LevelUpMenuGetProfessionNamePrefix(ref string __result, int whichProfession)
    {
        try
        {
            if (!Profession.TryFromValue(whichProfession, out var profession)) return true; // run original logic

            __result = profession.Name;
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}