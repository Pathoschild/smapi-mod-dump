/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/StardewValleyModding
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using xTile.Dimensions;

namespace CheaperBeachBridgeRepair
{
    public static class BeachPatches
    {
        private static ModConfig _config;
        private static IMonitor _monitor;

        public static void Initialize(ModConfig config, IMonitor monitor)
        {
            _config = config;
            _monitor = monitor;
        }

        public static bool AnswerDialogueAction_Prefix(StardewValley.Locations.Beach __instance, string questionAndAnswer, string[] questionParams, ref bool __result)
        {
            try
            {
                switch (questionAndAnswer)
                {
                    case "BeachBridge_Yes":
                        Game1.globalFadeToBlack(new Game1.afterFadeFunction(__instance.fadedForBridgeFix), 0.02f);
                        Game1.player.removeItemsFromInventory(388, _config.BridgeRepairPrice);
                        __result = true;
                        return false;
                    default:
                        return true;
                }
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AnswerDialogueAction_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }

        public static bool checkAction_Prefix(StardewValley.Locations.Beach __instance, Location tileLocation, Rectangle viewport, Farmer who, ref bool __result)
        {
            try
            {
                if (__instance.map.GetLayer("Buildings").Tiles[tileLocation] != null && __instance.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex == 284)
                {
                    if (who.hasItemInInventory(388, _config.BridgeRepairPrice, 0))
                    {
                        __instance.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Beach_FixBridge_Question").Replace("300", _config.BridgeRepairPrice.ToString()), __instance.createYesNoResponses(), "BeachBridge");
                    }
                    else
                    {
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Beach_FixBridge_Hint").Replace("300", _config.BridgeRepairPrice.ToString()));
                    }
                    __result = true;
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(checkAction_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }
    }
}
