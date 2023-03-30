/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley.Locations;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(AdventureGuild), nameof(AdventureGuild.willThisKillCompleteAMonsterSlayerQuest))]
    internal class AdventureGuildMonsterSlayerPatch
    {
        public static void Postfix(bool __result)
        {
            if (!ModEntry.ShouldPatch() || !__result)
                return;

            ModEntry.Instance.TaskManager?.OnMonsterSlayerQuestCompleted();
        }
    }
}
