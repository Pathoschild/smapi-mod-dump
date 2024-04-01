/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Menus;

namespace OKNightCheck
{
    public partial class ModEntry
    {
        [HarmonyPatch(typeof(Game1), nameof(Game1.showEndOfNightStuff))]
        public class Game1_showEndOfNightStuff_Patch
        {
            public static void Postfix()
            {
                if (!Config.ModEnabled)
                    return;
                if(Game1.endOfNightMenus.Count == 0 && Game1.activeClickableMenu is SaveGameMenu)
                {
                    SMonitor.Log("Showing pause menu");
                    Game1.activeClickableMenu = new OKMenu();
                }
            }
        }
    }
}