/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using static StardewValley.Minigames.AbigailGame;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(CowboyMonster), "takeDamage")]
    internal class JOTPKTakeDamagePatch
    {
        public static void Postfix(bool __result)
        {
            if (__result)
                Minigames.PrairieKingKills++;
        }
    }
}
