/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class CharacterInitNetFieldsPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal CharacterInitNetFieldsPatch()
    {
        Target = RequireMethod<Character>("initNetFields");
    }

    #region harmony patches

    /// <summary>Patch to add custom net fields.</summary>
    [HarmonyPostfix]
    private static void CharacterInitNetFieldsPostfix(Character __instance)
    {
        if (__instance is not Farmer farmer) return;

        __instance.NetFields.AddFields(farmer.get_UltimateIndex());
        __instance.NetFields.AddFields(farmer.get_IsUltimateActive());
        __instance.NetFields.AddFields(farmer.get_IsFake());
    }

    #endregion harmony patches
}