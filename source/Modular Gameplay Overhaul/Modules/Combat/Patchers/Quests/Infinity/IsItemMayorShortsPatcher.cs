/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Patchers.Quests.Infinity;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class IsItemMayorShortsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="IsItemMayorShortsPatcher"/> class.</summary>
    internal IsItemMayorShortsPatcher()
    {
        this.Target = this.RequireMethod<Event>(nameof(Event.IsItemMayorShorts));
    }

    #region harmony patches

    /// <summary>Record shorts betrayal.</summary>
    [HarmonyPostfix]
    private static void IsItemMayorShortsPostfix(bool __result)
    {
        if (__result)
        {
            Game1.MasterPlayer.Write(DataKeys.HasUsedMayorShorts, true.ToString());
        }
    }

    #endregion harmony patches
}
