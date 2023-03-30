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
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.blacksmith))]
    internal class GameLocationBlacksmithPatch
    {
        public static bool Prefix(out Tool? __state)
        {
            __state = null;
            if (!ModEntry.ShouldPatch())
                return true;

            __state = Game1.player.toolBeingUpgraded.Value;
            return true;
        }

        public static void Postfix(Tool? __state)
        {
            if (!ModEntry.ShouldPatch() || __state is null)
                return;

            if (Game1.player.toolBeingUpgraded.Value is null)
                ModEntry.Instance.TaskManager?.OnToolUpgrade(__state);
        }
    }
}
