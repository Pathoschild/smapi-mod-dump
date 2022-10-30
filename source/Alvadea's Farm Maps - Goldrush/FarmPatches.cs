/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jaksha6472/MiningShack
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace AlvadeasMiningShack
{
    class FarmPatches
    {
        private static IModHelper Helper => AlvadeasMiningShack.Instance.Helper;
        private static IMonitor Monitor => AlvadeasMiningShack.Instance.Monitor;

        public static void Apply(Harmony harmony)
        {
            // Overwriting the checkAction to check for new action triggers
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm),
                    nameof(Farm.checkAction)),
                prefix: new HarmonyMethod(typeof(FarmPatches),
                    nameof(FarmPatches.farmCheckAction_Prefix))
            );
        }

        public static void fixShack()
        {
            GameLocation location = Game1.getLocationFromName("Farm");
            Layer layer = location.map.GetLayer("Back2");
            TileSheet tilesheet = location.Map.GetTileSheet("z_AlvadeasTileSheet_spring");

            layer.Tiles[19, 56] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 720);
            layer.Tiles[20, 56] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 721);
            layer.Tiles[21, 56] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 722);
            layer.Tiles[22, 56] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 723);
            layer.Tiles[23, 56] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 724);
            layer.Tiles[24, 56] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 725);
            layer.Tiles[25, 56] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 726);
            layer.Tiles[26, 56] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 727);

            // Remove raven shadow
            layer.Tiles[12, 57] = null;
            layer.Tiles[13, 57] = null;
            layer.Tiles[12, 58] = null;
            layer.Tiles[13, 58] = null;

            // Add water tanke
            layer.Tiles[14, 56] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 126);
            layer.Tiles[14, 57] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 156);

            layer = location.map.GetLayer("Buildings");

            layer.Tiles[19, 55] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 690);
            layer.Tiles[20, 55] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 691);
            layer.Tiles[21, 55] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 692);
            layer.Tiles[22, 55] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 693);
            layer.Tiles[23, 55] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 694);
            layer.Tiles[24, 55] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 695);
            layer.Tiles[25, 55] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 696);
            layer.Tiles[26, 55] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 697);

            layer.Tiles[19, 54] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 660);
            layer.Tiles[20, 54] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 661);
            layer.Tiles[21, 54] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 662);
            layer.Tiles[22, 54] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 663);
            layer.Tiles[23, 54] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 664);
            layer.Tiles[24, 54] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 665);
            layer.Tiles[25, 54] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 666);
            layer.Tiles[26, 54] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 667);

            layer.Tiles[19, 53] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 630);
            layer.Tiles[20, 53] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 631);
            layer.Tiles[21, 53] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 632);
            layer.Tiles[22, 53] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 633);
            layer.Tiles[23, 53] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 634);
            layer.Tiles[24, 53] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 635);
            layer.Tiles[25, 53] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 636);
            layer.Tiles[26, 53] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 637);

            // Light up the latern at the door
            location.map.Properties["DayTiles"] = location.map.Properties["DayTiles"].ToString() + " " + "Buildings 22 54 663";
            location.map.Properties["NightTiles"] = location.map.Properties["NightTiles"].ToString() + " " + "Buildings 22 54 517";
            location.map.Properties["Light"] = location.map.Properties["Light"].ToString() + " " + "22 54 4";

            location.setTileProperty(23, 55, "Buildings", "Action", "Warp 14 15 Custom_MiningShack");
            location.setTileProperty(12, 57, "Back", "WaterSource", "T");
            location.setTileProperty(13, 57, "Back", "WaterSource", "T");

            // Remove the camp fire
            layer.Tiles[24, 60] = null;

            // Remove raven and set water tank
            layer.Tiles[12, 57] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 154);
            layer.Tiles[13, 57] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 155);

            layer = location.map.GetLayer("Buildings2");

            // Remove raven
            layer.Tiles[12, 57] = null;

            layer = location.map.GetLayer("Front");

            layer.Tiles[19, 52] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 600);
            layer.Tiles[20, 52] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 601);
            layer.Tiles[21, 52] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 602);
            layer.Tiles[22, 52] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 603);
            layer.Tiles[23, 52] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 604);
            layer.Tiles[24, 52] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 605);
            layer.Tiles[25, 52] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 606);
            layer.Tiles[26, 52] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 607);

            layer.Tiles[19, 51] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 570);
            layer.Tiles[20, 51] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 571);
            layer.Tiles[21, 51] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 572);
            layer.Tiles[22, 51] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 573);
            layer.Tiles[23, 51] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 574);
            layer.Tiles[24, 51] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 575);
            layer.Tiles[25, 51] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 576);
            layer.Tiles[26, 51] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 577);

            layer.Tiles[19, 50] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 540);
            layer.Tiles[20, 50] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 541);
            layer.Tiles[21, 50] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 542);
            layer.Tiles[22, 50] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 543);
            layer.Tiles[23, 50] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 544);
            layer.Tiles[24, 50] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 545);
            layer.Tiles[25, 50] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 546);
            layer.Tiles[26, 50] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 547);

            // Add water tank
            layer.Tiles[12, 56] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 124);
            layer.Tiles[13, 56] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 125);
        }

        // This is mostly stolen from the original game code and reverses the freezes
        private static void doneWithShackFix()
        {
            Game1.globalFadeToClear();
            Game1.viewportFreeze = false;
            Game1.freezeControls = false;
            fixShack();
        }

        // This is mostly stolen from the original game code and does the fade to black animation
        private static void fadedForShackFix()
        {
            Game1.freezeControls = true;
            DelayedAction.playSoundAfterDelay("crafting", 1000);
            DelayedAction.playSoundAfterDelay("crafting", 1500);
            DelayedAction.playSoundAfterDelay("crafting", 2000);
            DelayedAction.playSoundAfterDelay("crafting", 2500);
            DelayedAction.playSoundAfterDelay("axchop", 3000);
            DelayedAction.playSoundAfterDelay("Ship", 3200);
            Game1.viewportFreeze = true;
            Game1.viewport.X = -10000;
            Game1.pauseThenDoFunction(4000, doneWithShackFix);
        }

        public static bool farmCheckAction_Prefix(Location tileLocation, Rectangle viewport, Farmer who, Farm __instance, ref bool __result)
        {
            try
            {
                // This should only be doable on the mining farm
                if (Game1.whichFarm != 3)
                    return true;

                if (NetWorldState.checkAnywhereForWorldStateID("miningShackRepaired"))
                    return true;

                int tileLocationCurrent = __instance.map.GetLayer("Buildings").Tiles[tileLocation] != null ? __instance.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex : -1;

                List<Response> answers = new();

                switch (__instance.map.GetLayer("Buildings").Tiles[tileLocation] != null ? __instance.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex : -1)
                {
                    case 0:
                        if (__instance.map.GetLayer("Buildings").Tiles[tileLocation].TileSheet.Id != "z_ShackEntry")
                            return true;

                        // Get all the items in the player's inventory
                        List<Item> InventoryItems = Game1.player.Items.ToList();

                        bool hasEnoughWoodFloor = hasEnoughResources(InventoryItems, 328, 100);
                        bool hasEnoughStone = hasEnoughResources(InventoryItems, 390, 200);
                        bool hasEnoughCopperBars = hasEnoughResources(InventoryItems, 334, 25);

                        answers = new List<Response>();
                        if (hasEnoughWoodFloor && hasEnoughStone && hasEnoughCopperBars)
                        {
                            answers.Add(new Response("Repair", Helper.Translation.Get("miningshack.repair")));
                            answers.Add(new Response("Leave", Helper.Translation.Get("miningshack.leave")));
                        }
                        else
                        {
                            answers.Add(new Response("NotEnough", Helper.Translation.Get("miningshack.notEnough")));
                        }

                        GameLocation.afterQuestionBehavior afterQuestion = (Farmer, choice) =>
                        {
                            if (choice == "Repair")
                            {
                                // Remove all necessary resources from the player inventory
                                removeResources(InventoryItems, 328, 100);
                                removeResources(InventoryItems, 390, 200);
                                removeResources(InventoryItems, 334, 25);

                                if (!NetWorldState.checkAnywhereForWorldStateID("miningShackRepaired"))
                                {
                                    NetWorldState.addWorldStateIDEverywhere("miningShackRepaired");
                                }

                                //Fix the Shack
                                Game1.globalFadeToBlack(fadedForShackFix);
                            }
                        };
                        Game1.currentLocation.createQuestionDialogue(Helper.Translation.Get("miningshack.question"), answers.ToArray(), afterQuestion);

                        return true;
                    default:
                        return true;
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(farmCheckAction_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // Run original code
            }
        }

        // Check if the player has the right amount of resources
        private static bool hasEnoughResources(List<Item> inventory, int itemIndex, int itemAmount)
        {
            foreach (Item item in inventory)
            {
                if (item == null)
                    return false;

                int key = item.ParentSheetIndex;
                int amount = item.Stack;

                if (key == itemIndex && amount >= itemAmount)
                    return true;
            }

            return false;
        }

        private static bool removeResources(List<Item> inventory, int itemIndex, int itemAmount)
        {
            foreach (Item item in inventory)
            {
                if (item == null)
                    return false;

                int key = item.ParentSheetIndex;

                if (key == itemIndex)
                    item.Stack -= itemAmount;
            }

            return false;
        }
    }
}
