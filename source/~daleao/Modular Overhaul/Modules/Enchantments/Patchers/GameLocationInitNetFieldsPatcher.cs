/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Enchantments.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Enchantments.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationInitNetFieldsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationInitNetFieldsPatcher"/> class.</summary>
    internal GameLocationInitNetFieldsPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>("initNetFields");
    }

    #region harmony patches

    /// <summary>Patch to add custom net fields.</summary>
    [HarmonyPostfix]
    private static void GameLocationInitNetFieldsPostfix(GameLocation __instance)
    {
        __instance.NetFields.AddFields(__instance.Get_Animals());
    }

    #endregion harmony patches
}
