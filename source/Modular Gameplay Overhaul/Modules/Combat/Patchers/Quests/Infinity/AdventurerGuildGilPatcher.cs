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
using Microsoft.Xna.Framework;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class AdventurerGuildGilPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="AdventurerGuildGilPatcher"/> class.</summary>
    internal AdventurerGuildGilPatcher()
    {
        this.Target = this.RequireMethod<AdventureGuild>("gil");
    }

    #region harmony patches

    /// <summary>Update virtue progress.</summary>
    [HarmonyPostfix]
    private static void AdventurerGuildGilPostfix()
    {
        var player = Game1.player;
        var delta = player.NumMonsterSlayerQuestsCompleted() -
                    player.Read<int>(DataKeys.NumCompletedSlayerQuests);
        if (delta <= 0)
        {
            return;
        }

        player.Increment(DataKeys.NumCompletedSlayerQuests, delta);
        player.Increment(Virtue.Valor.Name, delta);
        Game1.chatBox.addMessage(I18n.Virtues_Recognize_Gil(), Color.Green);
        CombatModule.State.HeroQuest?.UpdateTrialProgress(Virtue.Valor);
    }

    #endregion harmony patches
}
