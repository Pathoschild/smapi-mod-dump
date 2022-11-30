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
using StardewRoguelike.UI;
using StardewValley;
using StardewValley.Menus;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(Game1), "UpdateControlInput")]
    internal class Game1UpdateControlPatch
    {
        public static void Postfix()
        {
            if (Game1.activeClickableMenu is QuestLog)
                Game1.activeClickableMenu = new PerkDisplayMenu();
        }
    }
}
