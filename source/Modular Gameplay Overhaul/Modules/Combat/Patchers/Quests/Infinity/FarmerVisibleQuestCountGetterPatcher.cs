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

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerVisibleQuestCountGetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerVisibleQuestCountGetterPatcher"/> class.</summary>
    internal FarmerVisibleQuestCountGetterPatcher()
    {
        this.Target = this.RequirePropertyGetter<Farmer>(nameof(Farmer.visibleQuestCount));
    }

    #region harmony patches

    /// <summary>Consider Virtues quest as visible.</summary>
    [HarmonyPostfix]
    private static void FarmerVisibleQuestCountGetterPostfix(Farmer __instance, ref int __result)
    {
        if (__instance.IsLocalPlayer && CombatModule.State.HeroQuest is not null)
        {
            __result++;
        }
    }

    #endregion harmony patches
}
