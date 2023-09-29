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

using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Quests;

#endregion using directives

[UsedImplicitly]
internal sealed class QuestQuestCompletePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="QuestQuestCompletePatcher"/> class.</summary>
    internal QuestQuestCompletePatcher()
    {
        this.Target = this.RequireMethod<Quest>(nameof(Quest.questComplete));
    }

    #region harmony patches

    /// <summary>Record shorts honor.</summary>
    [HarmonyPostfix]
    private static void QuestQuestCompletePostfix(Quest __instance)
    {
        if (__instance is not LostItemQuest || __instance.id.Value != 102 ||
            Game1.MasterPlayer.Read<bool>(DataKeys.HasUsedMayorShorts))
        {
            return;
        }

        foreach (var farmer in Game1.getAllFarmers())
        {
            farmer.Increment(Virtue.Honor.Name, 2);
        }
    }

    #endregion harmony patches
}
