/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jaksha6472/WitchTower
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using HarmonyLib;
using System.Collections.Generic;
using System;
using xTile.Dimensions;
using StardewValley.Network;
using System.Linq;
using xTile.Layers;
using xTile.Tiles;

namespace AlvadeasWitchTower
{
    internal class CustomQuests
    {
        private static IModHelper Helper => AlvadeasWitchTower.Instance.Helper;
        private static IMonitor Monitor => AlvadeasWitchTower.Instance.Monitor;

        public static void Apply(Harmony harmony)
        {
            // Overwriting the checkAction to check for new action triggers
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm),
                    nameof(Farm.checkAction)),
                prefix: new HarmonyMethod(typeof(CustomQuests),
                    nameof(CustomQuests.farmCheckAction_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation),
                    nameof(GameLocation.answerDialogueAction)),
                prefix: new HarmonyMethod(typeof(CustomQuests),
                    nameof(CustomQuests.answerDialogueAction_Prefix))
            );
        }

        public static void fixTeleportCircle()
        {
            GameLocation location = Game1.getLocationFromName("Farm");
            TileSheet tilesheet = location.Map.GetTileSheet("z_SwampTiles");

            //Remove raven from old position
            Layer layer = location.map.GetLayer("Back3");

            layer.Tiles[83, 47] = null;
            layer.Tiles[84, 47] = null;
            layer.Tiles[83, 48] = null;
            layer.Tiles[84, 48] = null;

            layer = location.map.GetLayer("Buildings");

            layer.Tiles[83, 47] = null;
            layer.Tiles[84, 47] = null;
            layer.Tiles[83, 48] = null;
            layer.Tiles[84, 48] = null;
            
            //Change teleportation circle design
            layer = location.map.GetLayer("Back2");
            tilesheet = location.Map.GetTileSheet("z_AlvadeasTileSheet");

            layer.Tiles[83, 47] = null;
            layer.Tiles[84, 47] = null;
            layer.Tiles[83, 48] = null;
            layer.Tiles[84, 48] = null;

            layer.Tiles[4, 31] = null;
            layer.Tiles[5, 31] = null;
            layer.Tiles[4, 32] = null;
            layer.Tiles[5, 32] = null;

            layer.Tiles[83, 47] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 680);
            layer.Tiles[84, 47] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 681);
            layer.Tiles[83, 48] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 710);
            layer.Tiles[84, 48] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 711);

            layer.Tiles[4, 31] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 680);
            layer.Tiles[5, 31] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 681);
            layer.Tiles[4, 32] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 710);
            layer.Tiles[5, 32] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tileIndex: 711);

            //Add warp
            location.setTileProperty(84, 47, "Back", "TouchAction", "MagicWarp Farm 5 32");
            location.setTileProperty(84, 48, "Back", "TouchAction", "MagicWarp Farm 5 32");

            location.setTileProperty(4, 31, "Back", "TouchAction", "MagicWarp Farm 83 48");
            location.setTileProperty(4, 32, "Back", "TouchAction", "MagicWarp Farm 83 48");
            location.setTileProperty(5, 31, "Back", "TouchAction", "MagicWarp Farm 83 48");

            location.map.Properties["Light"] = location.map.Properties["Light"].ToString() + " " + "5 32 5";
            location.map.Properties["Light"] = location.map.Properties["Light"].ToString() + " " + "83 48 5";

            if (!NetWorldState.checkAnywhereForWorldStateID("walpurgis.fixedTeleportationCircle"))
            {
                NetWorldState.addWorldStateIDEverywhere("walpurgis.fixedTeleportationCircle");
            }
        }

        // This is mostly stolen from the original game code and reverses the freezes
        private static void doneWithCreatingTeleportCircle()
        {
            Game1.globalFadeToClear();
            Game1.viewportFreeze = false;
            Game1.freezeControls = false;
            fixTeleportCircle();
        }

        // This is mostly stolen from the original game code and does the fade to black animation
        private static void fadedForCreatingTeleportCircle()
        {
            Game1.freezeControls = true;
            DelayedAction.playSoundAfterDelay("healSound", 1000);
            DelayedAction.playSoundAfterDelay("healSound", 2000);
            DelayedAction.playSoundAfterDelay("healSound", 3000);
            DelayedAction.playSoundAfterDelay("crow", 4000);
            DelayedAction.playSoundAfterDelay("batFlap", 5000);
            DelayedAction.playSoundAfterDelay("batFlap", 5300);
            DelayedAction.playSoundAfterDelay("batFlap", 5600);
            DelayedAction.playSoundAfterDelay("batFlap", 5900);
            DelayedAction.playSoundAfterDelay("batFlap", 6200);
            DelayedAction.playSoundAfterDelay("batFlap", 6500);
            DelayedAction.playSoundAfterDelay("batFlap", 6800);
            DelayedAction.playSoundAfterDelay("batFlap", 7100);
            DelayedAction.playSoundAfterDelay("batFlap", 7400);
            Game1.viewportFreeze = true;
            Game1.viewport.X = -10000;
            Game1.pauseThenDoFunction(8000, doneWithCreatingTeleportCircle);
        }

        public static bool answerDialogueAction_Prefix(string questionAndAnswer, string[] questionParams)
        {
            List<Response> answers = new();
            List<string> textList = new();
            string playerName = Game1.player.displayName;
            string text = "";

            switch (questionAndAnswer)
            {
                case "Raven_interested":
                    textList = new List<string>();
                    text = Helper.Translation.Get("witchtower.raven_happyFirstRavenRequest");
                    text = text.Replace("@", playerName);
                    textList.AddRange(text.Split("|"));

                    answers = new List<Response>
                    {
                        new Response("acceptFirstRequest", Helper.Translation.Get("witchtower.farmer_acceptFirstRavenRequest")),
                        new Response("complainFirstRequest", Helper.Translation.Get("witchtower.farmer_complainFirstRavenRequest"))
                    };

                    Game1.afterDialogues = () =>
                    {
                        Game1.afterDialogues = null;
                        Game1.currentLocation.createQuestionDialogue(Helper.Translation.Get("witchtower.raven_firstRavenRequestQuestion").ToString().Replace("@", playerName), answers.ToArray(), "Raven");
                    };

                    Game1.multipleDialogues(textList.ToArray());
                    return true;
                case "Raven_whatRequest":
                    textList = new List<string>();
                    text = Helper.Translation.Get("witchtower.raven_firstRavenRequest");
                    text = text.Replace("@", playerName);
                    textList.AddRange(text.Split("|"));

                    answers = new List<Response>
                    {
                        new Response("acceptFirstRequest", Helper.Translation.Get("witchtower.farmer_acceptFirstRavenRequest")),
                        new Response("complainFirstRequest", Helper.Translation.Get("witchtower.farmer_complainFirstRavenRequest"))
                    };

                    Game1.afterDialogues = () =>
                    {
                        Game1.afterDialogues = null;
                        Game1.currentLocation.createQuestionDialogue(Helper.Translation.Get("witchtower.raven_firstRavenRequestQuestion").ToString().Replace("@", playerName), answers.ToArray(), "Raven");
                    };

                    Game1.multipleDialogues(textList.ToArray());
                    return true;
                case "Raven_notInterested":
                    textList = new List<string>();
                    text = Helper.Translation.Get("witchtower.raven_answerNotInterested");
                    text = text.Replace("@", playerName);
                    textList.AddRange(text.Split("|"));

                    Game1.multipleDialogues(textList.ToArray());
                    if (!NetWorldState.checkAnywhereForWorldStateID("walpurgis.deniedRavenOnce"))
                    {
                        NetWorldState.addWorldStateIDEverywhere("walpurgis.deniedRavenOnce");
                    }
                    return true;
                case "Raven_acceptFirstRequest":
                    textList = new List<string>();
                    text = Helper.Translation.Get("witchtower.raven_acceptFirstRavenRequest");
                    text = text.Replace("@", playerName);
                    textList.AddRange(text.Split("|"));

                    Game1.multipleDialogues(textList.ToArray());
                    Game1.player.addQuest(15926001);
                    if (!NetWorldState.checkAnywhereForWorldStateID("walpurgis.gotFirstRavenRequest"))
                    {
                        NetWorldState.addWorldStateIDEverywhere("walpurgis.gotFirstRavenRequest");
                    }
                    return true;
                case "Raven_complainFirstRequest":
                    textList = new List<string>();
                    text = Helper.Translation.Get("witchtower.raven_complainFirstRavenRequest");
                    text = text.Replace("@", playerName);
                    textList.AddRange(text.Split("|"));

                    Game1.multipleDialogues(textList.ToArray());
                    Game1.player.addQuest(15926001);
                    if (!NetWorldState.checkAnywhereForWorldStateID("walpurgis.gotFirstRavenRequest"))
                    {
                        NetWorldState.addWorldStateIDEverywhere("walpurgis.gotFirstRavenRequest");
                    }
                    return true;
                case "Raven_acceptSecondRequest":
                    textList = new List<string>();
                    text = Helper.Translation.Get("witchtower.raven_acceptSecondRavenRequest");
                    text = text.Replace("@", playerName);
                    textList.AddRange(text.Split("|"));

                    Game1.multipleDialogues(textList.ToArray());
                    Game1.player.addQuest(15926002);
                    if (!NetWorldState.checkAnywhereForWorldStateID("walpurgis.gotSecondRavenRequest"))
                    {
                        NetWorldState.addWorldStateIDEverywhere("walpurgis.gotSecondRavenRequest");
                    }
                    return true;
                case "Raven_complainSecondRequest":
                    textList = new List<string>();
                    text = Helper.Translation.Get("witchtower.raven_complainSecondRavenRequest");
                    text = text.Replace("@", playerName);
                    textList.AddRange(text.Split("|"));

                    Game1.multipleDialogues(textList.ToArray());
                    Game1.player.addQuest(15926002);
                    if (!NetWorldState.checkAnywhereForWorldStateID("walpurgis.gotSecondRavenRequest"))
                    {
                        NetWorldState.addWorldStateIDEverywhere("walpurgis.gotSecondRavenRequest");
                    }
                    return true;
                case "Raven_acceptThirdRequest":
                    textList = new List<string>();
                    text = Helper.Translation.Get("witchtower.raven_acceptThirdRavenRequest");
                    text = text.Replace("@", playerName);
                    textList.AddRange(text.Split("|"));

                    Game1.multipleDialogues(textList.ToArray());
                    Game1.player.addQuest(15926003);
                    if (!NetWorldState.checkAnywhereForWorldStateID("walpurgis.gotThirdRavenRequest"))
                    {
                        NetWorldState.addWorldStateIDEverywhere("walpurgis.gotThirdRavenRequest");
                    }
                    return true;
                case "Raven_complainThirdRequest":
                    textList = new List<string>();
                    text = Helper.Translation.Get("witchtower.raven_complainThirdRavenRequest");
                    text = text.Replace("@", playerName);
                    textList.AddRange(text.Split("|"));

                    Game1.multipleDialogues(textList.ToArray());
                    Game1.player.addQuest(15926003);
                    if (!NetWorldState.checkAnywhereForWorldStateID("walpurgis.gotThirdRavenRequest"))
                    {
                        NetWorldState.addWorldStateIDEverywhere("walpurgis.gotThirdRavenRequest");
                    }
                    return true;
                case "Raven_askAboutRequest":
                    textList = new List<string>();
                    text = Helper.Translation.Get("witchtower.raven_askMore");
                    text = text.Replace("@", playerName);
                    textList.AddRange(text.Split("|"));

                    answers = new List<Response>
                    {
                        new Response("complainMore", Helper.Translation.Get("witchtower.farmer_complainMore"))
                    };

                    Game1.afterDialogues = () =>
                    {
                        Game1.afterDialogues = null;
                        Game1.currentLocation.createQuestionDialogue(Helper.Translation.Get("witchtower.raven_tryRequest").ToString().Replace("@", playerName), answers.ToArray(), "Raven");
                    };

                    Game1.multipleDialogues(textList.ToArray());
                    return true;
                case "Raven_complainFourthRequest":
                    answers = new List<Response>
                    {
                        new Response("complainMore", Helper.Translation.Get("witchtower.farmer_complainMore"))
                    };
                    Game1.currentLocation.createQuestionDialogue(Helper.Translation.Get("witchtower.raven_tryRequestNicer").ToString().Replace("@", playerName), answers.ToArray(), "Raven");
                    return true;
                case "Raven_complainMore":
                    textList = new List<string>();
                    text = Helper.Translation.Get("witchtower.raven_unhappyAboutComplains");
                    text = text.Replace("@", playerName);
                    textList.AddRange(text.Split("|"));

                    answers = new List<Response>
                    {
                        new Response("acceptFourthRequest", Helper.Translation.Get("witchtower.farmer_acceptFourthRavenRequest")),
                        new Response("complainEvenMore", Helper.Translation.Get("witchtower.farmer_denyFourthRavenRequest"))
                    };

                    Game1.afterDialogues = () =>
                    {
                        Game1.afterDialogues = null;
                        Game1.currentLocation.createQuestionDialogue(Helper.Translation.Get("witchtower.raven_fourthRavenRequestQuestion").ToString().Replace("@", playerName), answers.ToArray(), "Raven");
                    };

                    Game1.multipleDialogues(textList.ToArray());
                    return true;
                case "Raven_acceptFourthRequest":
                    textList = new List<string>();
                    text = Helper.Translation.Get("witchtower.raven_acceptFourthRavenRequest");
                    text = text.Replace("@", playerName);
                    textList.AddRange(text.Split("|"));

                    Game1.multipleDialogues(textList.ToArray());
                    Game1.player.addQuest(15926004);
                    if (!NetWorldState.checkAnywhereForWorldStateID("walpurgis.raven_acceptFourthRavenRequest"))
                    {
                        NetWorldState.addWorldStateIDEverywhere("walpurgis.gotFourthRavenRequest");
                    }
                    return true;
                case "Raven_complainEvenMore":
                    textList = new List<string>();
                    text = Helper.Translation.Get("witchtower.raven_denyFourthRavenRequest");
                    text = text.Replace("@", playerName);
                    textList.AddRange(text.Split("|"));

                    Game1.multipleDialogues(textList.ToArray());
                    Game1.afterDialogues = () =>
                    {
                        Game1.globalFadeToBlack(fadedForCreatingTeleportCircle);
                    };
                    
                    return true;
                default:
                    return true;
            }
        }

        public static bool farmCheckAction_Prefix(Location tileLocation, Rectangle viewport, Farmer who, Farm __instance, ref bool __result)
        {
            try
            {
                if (Game1.whichFarm != 4)
                    return true;

                int tileLocationCurrent = __instance.map.GetLayer("Back2").Tiles[tileLocation] != null ? __instance.map.GetLayer("Back2").Tiles[tileLocation].TileIndex : -1;

                string playerName = Game1.player.displayName;
                string text = "";
                List<Response> answers = new();
                List<string> textList = new();
                List<Item> InventoryItems = Game1.player.Items.ToList();

                switch (__instance.map.GetLayer("Back2").Tiles[tileLocation] != null ? __instance.map.GetLayer("Back2").Tiles[tileLocation].TileIndex : -1)
                {
                    case 52:
                        if (NetWorldState.checkAnywhereForWorldStateID("walpurgis.fixedTeleportationCircle"))
                        {
                            fixTeleportCircle();
                            return true;
                        }

                        if (__instance.map.GetLayer("Back2").Tiles[tileLocation].TileSheet.Id != "z_SwampTiles")
                            return true;

                        if (NetWorldState.checkAnywhereForWorldStateID("walpurgis.gotFourthRavenRequest"))
                        {
                            //Check if the player has 1 small glow ring
                            bool hasEnoughRings = hasEnoughResources(InventoryItems, 516, 1);

                            if (hasEnoughRings)
                            {
                                removeResources(InventoryItems, 516, 1);
                                Game1.player.completeQuest(15926004);

                                textList = new List<string>();
                                text = Helper.Translation.Get("witchtower.raven_finishQuestLine");
                                text = text.Replace("@", playerName);
                                textList.AddRange(text.Split("|"));
                                Game1.multipleDialogues(textList.ToArray());

                                Game1.afterDialogues = () =>
                                {
                                    Game1.globalFadeToBlack(fadedForCreatingTeleportCircle);
                                };

                                return true;
                            }

                            textList = new List<string>();
                            text = Helper.Translation.Get("witchtower.raven_waitForItem");
                            text = text.Replace("@", playerName);
                            textList.AddRange(text.Split("|"));

                            Game1.multipleDialogues(textList.ToArray());
                            return true;
                        }

                        if (NetWorldState.checkAnywhereForWorldStateID("walpurgis.gotThirdRavenRequest"))
                        {
                            //Check if the player has 3 midnight carp
                            bool hasEnoughCarp = hasEnoughResources(InventoryItems, 269, 3);

                            if (hasEnoughCarp)
                            {
                                removeResources(InventoryItems, 269, 3);
                                Game1.player.completeQuest(15926003);

                                answers = new List<Response>
                                {
                                    new Response("complainFourthRequest", Helper.Translation.Get("witchtower.farmer_complainFourthRavenRequest")),
                                    new Response("askAboutRequest", Helper.Translation.Get("witchtower.farmer_askMore"))
                                };

                                Game1.currentLocation.createQuestionDialogue(Helper.Translation.Get("witchtower.raven_fourthRequestPreludeFirst").ToString().Replace("@", playerName), answers.ToArray(), "Raven");
                                return true;
                            }

                            textList = new List<string>();
                            text = Helper.Translation.Get("witchtower.raven_waitForItem");
                            text = text.Replace("@", playerName);
                            textList.AddRange(text.Split("|"));

                            Game1.multipleDialogues(textList.ToArray());
                            return true;
                        }

                        if (NetWorldState.checkAnywhereForWorldStateID("walpurgis.gotSecondRavenRequest"))
                        {
                            //Check if the player has 5 fairy roses
                            bool hasEnoughFairyRoses = hasEnoughResources(InventoryItems, 595, 5);

                            if (hasEnoughFairyRoses)
                            {
                                removeResources(InventoryItems, 595, 5);
                                Game1.player.completeQuest(15926002);

                                text = Helper.Translation.Get("witchtower.raven_thirdRavenRequest");
                                text = text.Replace("@", playerName);
                                textList = new List<string>();
                                textList.AddRange(text.Split("|"));

                                answers = new List<Response>
                                {
                                    new Response("acceptThirdRequest", Helper.Translation.Get("witchtower.farmer_acceptThirdRavenRequest")),
                                    new Response("complainThirdRequest", Helper.Translation.Get("witchtower.farmer_complainThirdRavenRequest"))
                                };

                                Game1.afterDialogues = () =>
                                {
                                    Game1.afterDialogues = null;
                                    Game1.currentLocation.createQuestionDialogue(Helper.Translation.Get("witchtower.raven_thirdRavenRequestQuestion").ToString().Replace("@", playerName), answers.ToArray(), "Raven");
                                };

                                Game1.multipleDialogues(textList.ToArray());
                                return true;
                            }

                            textList = new List<string>();
                            text = Helper.Translation.Get("witchtower.raven_waitForItem");
                            text = text.Replace("@", playerName);
                            textList.AddRange(text.Split("|"));

                            Game1.multipleDialogues(textList.ToArray());
                            return true;
                        }

                        if (NetWorldState.checkAnywhereForWorldStateID("walpurgis.gotFirstRavenRequest"))
                        {
                            //Check if the player has 2 life elixirs
                            bool hasEnoughLifeExlirs = hasEnoughResources(InventoryItems, 773, 2);

                            if (hasEnoughLifeExlirs)
                            {
                                removeResources(InventoryItems, 773, 2);
                                Game1.player.completeQuest(15926001);

                                text = Helper.Translation.Get("witchtower.raven_secondRavenRequest");
                                text = text.Replace("@", playerName);
                                textList = new List<string>();
                                textList.AddRange(text.Split("|"));

                                answers = new List<Response>
                                {
                                    new Response("acceptSecondRequest", Helper.Translation.Get("witchtower.farmer_acceptSecondRavenRequest")),
                                    new Response("complainSecondRequest", Helper.Translation.Get("witchtower.farmer_complainSecondRavenRequest"))
                                };

                                Game1.afterDialogues = () =>
                                {
                                    Game1.afterDialogues = null;
                                    Game1.currentLocation.createQuestionDialogue(Helper.Translation.Get("witchtower.raven_secondRavenRequestQuestion").ToString().Replace("@", playerName), answers.ToArray(), "Raven");
                                };

                                Game1.multipleDialogues(textList.ToArray());
                                return true;
                            }

                            textList = new List<string>();
                            text = Helper.Translation.Get("witchtower.raven_waitForItem");
                            text = text.Replace("@", playerName);
                            textList.AddRange(text.Split("|"));

                            Game1.multipleDialogues(textList.ToArray());
                            return true;
                        }

                        if (NetWorldState.checkAnywhereForWorldStateID("walpurgis.deniedRavenOnce"))
                        {
                            answers = new List<Response>
                            {
                                new Response("interested", Helper.Translation.Get("witchtower.farmer_interested")),
                                new Response("whatRequest", Helper.Translation.Get("witchtower.farmer_whatRequest"))
                            };

                            Game1.currentLocation.createQuestionDialogue(Helper.Translation.Get("witchtower.raven_deniedRavenOnce").ToString().Replace("@", playerName), answers.ToArray(), "Raven");
                            return true;
                        }

                        if (Game1.player.eventsSeen.Contains(112))
                        {
                            text = Helper.Translation.Get("witchtower.raven_introduction");
                            text = text.Replace("@", playerName);
                            textList = new List<string>();
                            textList.AddRange(text.Split("|"));

                            answers = new List<Response>
                            {
                                new Response("interested", Helper.Translation.Get("witchtower.farmer_interested")),
                                new Response("whatRequest", Helper.Translation.Get("witchtower.farmer_whatRequest")),
                                new Response("notInterested", Helper.Translation.Get("witchtower.farmer_notInterested"))
                            };

                            Game1.afterDialogues = () =>
                            {
                                Game1.afterDialogues = null;
                                Game1.currentLocation.createQuestionDialogue(Helper.Translation.Get("witchtower.raven_introductionQuestion"), answers.ToArray(), "Raven");
                            };

                            Game1.multipleDialogues(textList.ToArray());
                            return true;
                        }
                        else
                        {
                            text = Helper.Translation.Get("witchtower.raven_notWorthy");
                            text = text.Replace("@", playerName);
                            textList = new List<string>();
                            textList.AddRange(text.Split("|"));

                            Game1.multipleDialogues(textList.ToArray());
                            return true;
                        }
                    case 53:
                        if (!NetWorldState.checkAnywhereForWorldStateID("walpurgis.fixedTeleportationCircle"))
                            return true;

                        if (__instance.map.GetLayer("Back2").Tiles[tileLocation].TileSheet.Id != "z_SwampTiles")
                            return true;

                        if (NetWorldState.checkAnywhereForWorldStateID("walpurgis.gotRavenReward"))
                        {
                            textList = new List<string>();
                            text = Helper.Translation.Get("witchtower.raven_afterReward");
                            text = text.Replace("@", playerName);
                            textList.AddRange(text.Split("|"));

                            Game1.multipleDialogues(textList.ToArray());

                            return true;
                        }

                        if (NetWorldState.checkAnywhereForWorldStateID("walpurgis.gotFourthRavenRequest"))
                        {
                            if (hasEnoughSpaceInInventory(InventoryItems))
                            {
                                textList = new List<string>();
                                text = Helper.Translation.Get("witchtower.raven_afterQuestLine");
                                text = text.Replace("@", playerName);
                                textList.AddRange(text.Split("|"));

                                Game1.multipleDialogues(textList.ToArray());

                                Game1.afterDialogues = () =>
                                {
                                    Item hat = new Hat(70);
                                    Game1.player.holdUpItemThenMessage(hat, false);
                                    Game1.player.addItemToInventory(hat);

                                    if (!NetWorldState.checkAnywhereForWorldStateID("walpurgis.gotRavenReward"))
                                    {
                                        NetWorldState.addWorldStateIDEverywhere("walpurgis.gotRavenReward");
                                    }
                                };

                                return true;
                            }
                            textList = new List<string>();
                            text = Helper.Translation.Get("witchtower.raven_afterQuestLineNotEnoughSpace");
                            text = text.Replace("@", playerName);
                            textList.AddRange(text.Split("|"));

                            Game1.multipleDialogues(textList.ToArray());
                            return true;

                        }
                        return true;
                    default:
                        return true;
                    
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(farmCheckAction_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }

        private static bool hasEnoughResources(List<Item> inventory, int itemIndex, int itemAmount)
        {
            foreach (Item item in inventory)
            {
                if (item == null)
                    continue;

                int key = item.ParentSheetIndex;
                int amount = item.Stack;

                if (key == itemIndex && amount >= itemAmount)
                    return true;
            }

            return false;
        }

        private static bool hasEnoughSpaceInInventory(List<Item> inventory)
        {
            foreach (Item item in inventory)
            {
                if (item == null)
                    return true;
            }

            return false;
        }

        private static bool removeResources(List<Item> inventory, int itemIndex, int itemAmount)
        {
            foreach (Item item in inventory)
            {
                if (item == null)
                    continue;

                int key = item.ParentSheetIndex;

                if (key == itemIndex)
                    if (item.Stack == itemAmount)
                        Game1.player.removeItemFromInventory(item);
                    else
                        item.Stack -= itemAmount;
            }

            return false;
        }
    }
}
