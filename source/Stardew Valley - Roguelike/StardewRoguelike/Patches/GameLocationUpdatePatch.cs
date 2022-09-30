/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewRoguelike.VirtualProperties;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(GameLocation), "updateEvenIfFarmerIsntHere")]
    internal class GameLocationUpdatePatch
    {
        public static void Postfix(GameLocation __instance)
        {
            __instance.get_DebuffPlayerEvent().Poll();
        }
    }
}
