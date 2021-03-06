/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/HandyHeadphones
**
*************************************************/

using Harmony;
using StardewValley;
using System.Reflection;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System.Linq;
using StardewValley.Objects;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace HandyHeadphones.Patches
{
    [HarmonyPatch]
    class GameLocationPatch
    {
        private static IMonitor monitor = ModEntry.monitor;
        private static IModHelper helper = ModEntry.modHelper;

        internal static MethodInfo TargetMethod()
        {
            return AccessTools.Method(typeof(StardewValley.GameLocation), nameof(StardewValley.GameLocation.IsMiniJukeboxPlaying));
        }

        internal static bool Prefix(GameLocation __instance, ref bool __result)
        {
            Hat playerHat = Game1.player.hat;
            if (playerHat != null && (playerHat.Name == "Headphones" || playerHat.Name == "Earbuds" || playerHat.Name == "Studio Headphones"))
            {
                __result = true;
                return false;
            }

            return true;
        }
    }
}
