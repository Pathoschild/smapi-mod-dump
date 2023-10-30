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
using StardewValley.Monsters;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(Bug), nameof(Bug.takeDamage))]
    internal class NoArmoredBugPatch
    {
        public static bool Prefix(Bug __instance)
        {
            __instance.isArmoredBug.Value = false;
            return true;
        }
    }
}
