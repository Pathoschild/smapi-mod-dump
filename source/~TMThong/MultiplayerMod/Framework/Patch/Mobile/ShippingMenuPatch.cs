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
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerMod.Framework.Patch.Mobile
{
    internal class ShippingMenuPatch : IPatch
    {
        private readonly static Type TYPE_PATCH = typeof(ShippingMenu);

        private readonly static Dictionary<ShippingMenu, bool> ShippingMenu_activated = new Dictionary<ShippingMenu, bool>();


        public void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Constructor(TYPE_PATCH, new Type[] { typeof(IList<Item>) }), new HarmonyMethod(this.GetType(), nameof(postfix_ctor)));
            harmony.Patch(AccessTools.Method(TYPE_PATCH, "update", new Type[] { typeof(GameTime) }), prefix: new HarmonyMethod(this.GetType(), nameof(prefix_update)));
        }

        private static void postfix_ctor(IList<Item> items, ShippingMenu __instance)
        {
            ShippingMenu_activated[__instance] = false;
        }

        private static bool prefix_update(GameTime time, ShippingMenu __instance)
        {
            if (ShippingMenu_activated.ContainsKey(__instance))
            {
                ShippingMenu_activated.Clear();
                Game1.player.team.endOfNightStatus.UpdateState("shipment");
            }

            if (Game1.activeClickableMenu != __instance)
            {
                return true;
            }
            bool savedYet = ModUtilities.Helper.Reflection.GetField<bool>(__instance, "savedYet").GetValue();
            if (savedYet)
            {
                if (Game1.PollForEndOfNewDaySync())
                {
                    __instance.exitThisMenu(playSound: false);
                }

                return false;
            }
            return true;
        }
    }
}
