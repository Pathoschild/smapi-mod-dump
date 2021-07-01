/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

namespace SpaceCore.Overrides
{
    public class GameMenuTabNameHook
    {
        public static void Postfix(GameMenuTabNameHook __instance, string name, ref int __result)
        {
            foreach ( var tab in Menus.extraGameMenuTabs )
            {
                if (name == tab.Value)
                    __result = tab.Key;
            }
        }
    }
}
