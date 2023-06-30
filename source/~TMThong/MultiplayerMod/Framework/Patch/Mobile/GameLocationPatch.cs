/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerMod.Framework.Patch.Mobile
{
    internal class GameLocationPatch : IPatch
    {
        private static readonly Type PATCH_TYPE = typeof(GameLocation);
        public void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.PropertyGetter(PATCH_TYPE, "tapToMove"), postfix: new HarmonyMethod(this.GetType(), nameof(postfix_get_tapToMove)));
        }

        private static void postfix_get_tapToMove(GameLocation __instance, ref object __result)
        {
            if (__result == null)
            {
                object TapToMove = typeof(IClickableMenu).Assembly.GetType("StardewValley.Mobile.TapToMove").CreateInstance<object>(new object[] { __instance });
                ModEntry.tapToMoveProperty.SetValue(__instance, TapToMove);
                __result = TapToMove;
            }
        }
    }
}
