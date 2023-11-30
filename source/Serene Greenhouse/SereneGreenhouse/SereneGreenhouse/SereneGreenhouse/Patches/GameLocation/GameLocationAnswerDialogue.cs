/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SereneGreenhouse
**
*************************************************/

using HarmonyLib;
using StardewValley;
using System.Reflection;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System.Linq;
using xTile.Dimensions;
using xTile.Tiles;
using System.Collections.Generic;

namespace SereneGreenhouse.Patches.GameLocation
{
    [HarmonyPatch]
    public class GameLocationAnswerDialogue
    {
        private static IMonitor monitor = ModEntry.monitor;

        internal static MethodInfo TargetMethod()
        {
            return AccessTools.Method(typeof(StardewValley.GameLocation), nameof(StardewValley.GameLocation.answerDialogue));
        }

        internal static bool Prefix(StardewValley.GameLocation __instance, ref bool __result, Response answer)
        {
            if (__instance.Name != "Greenhouse")
            {
                return true;
            }

            switch (answer.responseKey)
            {
                case "Offering_Yes":
                    int offeringsCount = 0;
                    if (!int.TryParse(Game1.MasterPlayer.modData[ModEntry.offeringsStoredInWaterHutKey], out offeringsCount))
                    {
                        monitor.Log($"Issue parsing ModData key [{ModEntry.offeringsStoredInWaterHutKey}]'s value to int", LogLevel.Trace);
                    }

                    Game1.MasterPlayer.modData[ModEntry.offeringsStoredInWaterHutKey] = (offeringsCount + Game1.player.ActiveObject.Stack).ToString();
                    ModEntry.AcceptOffering(Game1.player, "Yay, yay! Your offerings have pleased us!", Game1.player.ActiveObject.Stack);

                    __result = true;
                    return false;
                case "Offering_No":
                    __result = true;
                    return false;
            }

            return true;
        }
    }
}
