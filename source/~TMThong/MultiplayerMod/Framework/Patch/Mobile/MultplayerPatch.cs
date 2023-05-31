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
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerMod.Framework.Patch.Mobile
{
    internal class MultplayerPatch : IPatch
    {
        private static Type PATCH_TYPE = typeof(Multiplayer);

        public void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(PATCH_TYPE, "readActiveLocation", new Type[] { typeof(IncomingMessage) }), postfix: new HarmonyMethod(this.GetType(), nameof(prefix_readActiveLocation)));
        }
        private static void prefix_readActiveLocation(IncomingMessage msg)
        {
            var property = Game1.currentLocation.GetType().GetProperty("tapToMove");
            object TapToMove = typeof(IClickableMenu).Assembly.GetType("StardewValley.Mobile.TapToMove").CreateInstance<object>(new object[] { Game1.currentLocation });
            property.SetValue(Game1.currentLocation, TapToMove);
        }
    }
}
