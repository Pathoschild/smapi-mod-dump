/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/ExpFromMonsterKillsOnFarm
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using StardewValley.Monsters;

namespace ExpFromMonsterKillsOnFarm;

/// <summary>
/// Patches on the GameLocation class.
/// </summary>
[HarmonyPatch(typeof(GameLocation))]
internal class GameLocationPatches
{
    /// <summary>
    /// Appends EXP gain to monsterDrop.
    /// </summary>
    /// <param name="__instance">Game location.</param>
    /// <param name="__0">Monster killed.</param>
    /// <param name="__1">X location of monster killed.</param>
    /// <param name="__2">Y location of monster killed.</param>
    /// <param name="__3">Farmer who killed monster.</param>
    /// <remarks>This function is always called when a monster dies.</remarks>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(GameLocation.monsterDrop))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    public static void AppendMonsterDrop(GameLocation __instance, Monster __0, int __1, int __2, Farmer __3)
    {
        try
        {
            if (__3 is null || !__instance.IsFarm)
            {
                return;
            }
            if (ModEntry.Config.GainExp)
            {
                __3.gainExperience(Farmer.combatSkill, __0.ExperienceGained);
                ModEntry.ModMonitor.Log($"Granting {__3.Name} {__0.ExperienceGained} combat XP for monster kill on farm");
            }
            if (ModEntry.Config.QuestCompletion)
            {
                __3.checkForQuestComplete(null, 1, 1, null, __0.Name, 4);
                ModEntry.ModMonitor.Log($"Granting {__3.Name} one kill of {__0.Name} towards billboard.");
            }
            if (ModEntry.Config.SpecialOrderCompletion && Game1.player.team.specialOrders is not null)
            {
                foreach (SpecialOrder order in Game1.player.team.specialOrders)
                {
                    if (order.onMonsterSlain is not null)
                    {
                        order.onMonsterSlain(Game1.player, __0);
                        ModEntry.ModMonitor.Log($"Granting {__3.Name} one kill of {__0.Name} towards special order {order.questKey}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in granting combat xp on farm\n\n{ex}", LogLevel.Error);
        }
    }
}