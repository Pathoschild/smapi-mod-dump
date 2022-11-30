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
using StardewValley.Minigames;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(MineCart), "restartLevel")]
    internal class MineCartUpdateScore
    {
        public static bool Prefix(MineCart __instance)
        {
            int score = (int)__instance.GetType().GetField("score", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(__instance)!;
            Minigames.JunimoKartScore += score;

            return true;
        }
    }
}
