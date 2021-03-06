/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/MultipleMiniObelisks
**
*************************************************/

using Harmony;
using StardewValley;
using System.Reflection;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System.Linq;
using System.Collections.Generic;
using MultipleMiniObelisks.UI;
using StardewValley.Locations;
using StardewValley.Buildings;
using MultipleMiniObelisks.Objects;
using Newtonsoft.Json;

namespace MultipleMiniObelisks.Patches
{
    [HarmonyPatch]
    public class ObjectCheckForActionPatch
    {
        private static IMonitor monitor = ModEntry.monitor;

        internal static MethodInfo TargetMethod()
        {
            return AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.checkForAction));
        }

        internal static bool Prefix(Object __instance, ref bool __result, Farmer who, bool justCheckingForActivity = false)
        {
            // 238 is Mini-Obelisks
            if (__instance.ParentSheetIndex == 238)
            {
                if (justCheckingForActivity)
                {
                    return false;
                }

                List<MiniObelisk> miniObelisks = JsonConvert.DeserializeObject<List<MiniObelisk>>(Game1.MasterPlayer.modData[ModEntry.ObeliskLocationsKey]);
                Game1.activeClickableMenu = new TeleportMenu(__instance, miniObelisks);

                __result = true;
                return false;
            }

            return true;
        }
    }
}
