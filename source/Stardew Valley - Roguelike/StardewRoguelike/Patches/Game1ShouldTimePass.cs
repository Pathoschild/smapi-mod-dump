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
using StardewValley.Menus;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(Game1), "shouldTimePass")]
    internal class Game1ShouldTimePass
    {
        public static bool Prefix(ref bool __result, bool ignore_multiplayer = false)
        {
            if (Game1.IsMultiplayer && !ignore_multiplayer)
            {
                __result = !Game1.netWorldState.Value.IsTimePaused;
                return false;
            }
            if (Game1.eventUp)
            {
                __result = false;
                return false;
            }
            if (Game1.activeClickableMenu is DialogueBox || Game1.player.isEating)
            {
                __result = false;
                return false;
            }

            __result = true;
            return false;
        }
    }
}
