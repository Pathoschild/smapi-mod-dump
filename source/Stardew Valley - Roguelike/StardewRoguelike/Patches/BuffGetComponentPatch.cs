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
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(Buff), "getClickableComponents")]
    internal class BuffGetComponentPatch
    {
        public static bool Prefix(Buff __instance, ref List<ClickableTextureComponent> __result)
        {
            if (__instance is Curse)
            {
                Curse curse = __instance as Curse;
                __result = new() { new("", Rectangle.Empty, null, curse.Description, Game1.buffsIcons, Game1.getSourceRectForStandardTileSheet(Game1.buffsIcons, Curse.IconId, 16, 16), 4f) };
                return false;
            }

            return true;
        }
    }
}
