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
using StardewValley;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(Farmer), nameof(Farmer.changeFriendship))]
    internal class FarmerChangeFriendshipPatch
    {
        public static bool Prefix(ref int amount)
        {
            if (!ModEntry.ShouldPatch())
                return true;

            if (EventManager.EventIsActive(EventType.MoodSwings))
                amount *= 2;

            return true;
        }

        public static void Postfix(Farmer __instance, int amount, NPC n)
        {
            if (!ModEntry.ShouldPatch())
                return;

            ModEntry.Instance.TaskManager?.OnChangeFriendship(n, amount);
        }
    }
}
