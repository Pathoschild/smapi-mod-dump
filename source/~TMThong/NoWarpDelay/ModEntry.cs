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
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZeroDelayMap
{
    public class ModEntry : Mod
    {
        private Harmony Harmony = new Harmony("zerodelaymap");

        public override void Entry(IModHelper helper)
        {
            Harmony.Patch(AccessTools.Method(typeof(Game1), "warpFarmer", new Type[] { typeof(LocationRequest), typeof(int), typeof(int), typeof(int) }), postfix: new HarmonyMethod(this.GetType(), nameof(postfix_warpFarmer)));
            Harmony.Patch(AccessTools.Method(typeof(Game1), "fadeScreenToBlack", new Type[] { }), prefix: new HarmonyMethod(this.GetType(), nameof(prefix_fadeScreenToBlack)));
        }


        public static bool IsReadyToSkipWarp
        {
            get
            {
                return Game1.isWarping && Context.IsPlayerFree && Game1.timeOfDay < 2600 && Game1.player.stamina >= -13 && Game1.player.health > 0;
            }
        }

        private static bool prefix_fadeScreenToBlack()
        {
            if (IsReadyToSkipWarp)
            {
                return false;
            }
            return true;
        }


        private static void postfix_warpFarmer()
        {

            if (!IsReadyToSkipWarp)
            {
                return;
            }
            var method = AccessTools.Method(typeof(Game1), "onFadeToBlackComplete");
            method.Invoke(Game1.game1, null);
        }
    }
}
