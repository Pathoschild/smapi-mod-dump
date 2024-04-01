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
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;

namespace ContentPatcherEditor
{
    public partial class ModEntry
    {
        [HarmonyPatch(typeof(TitleMenu), nameof(TitleMenu.receiveKeyPress))]
        public class TitleMenu_receiveKeyPress_Patch
        {
            public static void Postfix(TitleMenu __instance, Keys key)
            {
                if (!Config.ModEnabled || TitleMenu.subMenu is null || !Game1.options.doesInputListContain(Game1.options.menuButton, key) || !TitleMenu.subMenu.readyToClose())
                    return;
                TitleMenu.subMenu.receiveKeyPress(key);
            }
        }
    }
}