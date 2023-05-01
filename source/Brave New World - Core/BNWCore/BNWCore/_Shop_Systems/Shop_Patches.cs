/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;

namespace BNWCore
{
    internal class Shop_Patches
    {
        public static bool GameLocation_performAction_Prefix(GameLocation __instance, string action, ref bool __result)
        {
            if (ModEntry.ModHelper.ModRegistry.IsLoaded("DiogoAlbano.BNWChapter1"))
            {
                string[] actionParams = action.Split(' ');
                string text = actionParams[0];
                List<Response> options = new List<Response>();
                switch (text)
                {
                    case "Buy":
                        __instance.openShopMenu(actionParams[1]);
                        __result = true;
                        return false;
                    case "Carpenter":
                        if (Game1.player.daysUntilHouseUpgrade.Value < 0 && !Game1.getFarm().isThereABuildingUnderConstruction())
                        {
                            if (Game1.IsMasterGame)
                            {
                                if (Game1.player.HouseUpgradeLevel < 3)
                                {
                                    options.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeHouse")));
                                }
                            }
                            else if (Game1.player.HouseUpgradeLevel < 3)
                            {
                                options.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeCabin")));
                            }
                            if (Game1.player.HouseUpgradeLevel >= 2)
                            {
                                if (Game1.IsMasterGame)
                                {
                                    options.Add(new Response("Renovate", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateHouse")));
                                }
                                else
                                {
                                    options.Add(new Response("Renovate", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateCabin")));
                                }
                            }
                            options.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave")));
                            __instance.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu"), options.ToArray(), "carpenter");
                        }
                        else
                        {
                            Game1.activeClickableMenu = new DialogueBox(ModEntry.ModHelper.Translation.Get("translator_dialog_box_robin_busy"));
                        }
                        __result = true;
                        return false;
                    case "Blacksmith":
                        if (Game1.player.toolBeingUpgraded.Value != null && Game1.player.daysLeftForToolUpgrade.Value <= 0)
                        {
                            if (Game1.player.freeSpotsInInventory() > 0 || Game1.player.toolBeingUpgraded.Value is GenericTool)
                            {
                                Tool tool = Game1.player.toolBeingUpgraded.Value;
                                Game1.player.toolBeingUpgraded.Value = null;
                                Game1.player.hasReceivedToolUpgradeMessageYet = false;
                                Game1.player.holdUpItemThenMessage(tool, true);
                                if (tool is GenericTool)
                                {
                                    tool.actionWhenClaimed();
                                }
                                else
                                {
                                    Game1.player.addItemToInventoryBool(tool, false);
                                }
                                if (Game1.player.team.useSeparateWallets.Value && tool.UpgradeLevel == 4)
                                {
                                    AccessTools.FieldRefAccess<Game1, Multiplayer>(null, "multiplayer").globalChatInfoMessage("IridiumToolUpgrade", new string[]
                                    {
                                    Game1.player.Name,
                                    tool.DisplayName
                                    });
                                }
                            }
                            else
                            {
                                Game1.drawDialogue(Game1.getCharacterFromName("Clint"), Game1.content.LoadString("Data\\ExtraDialogue:Clint_NoInventorySpace"));
                            }
                        }
                        else
                        {
                            Response[] responses;
                            if (Game1.player.hasItemInInventory(535, 1, 0) || Game1.player.hasItemInInventory(536, 1, 0) || Game1.player.hasItemInInventory(537, 1, 0) || Game1.player.hasItemInInventory(749, 1, 0) || Game1.player.hasItemInInventory(275, 1, 0) || Game1.player.hasItemInInventory(791, 1, 0))
                            {
                                responses = new Response[]
                                {
                                new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Upgrade")),
                                new Response("Process", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Geodes")),
                                new Response("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave"))
                                };
                            }
                            else
                            {
                                responses = new Response[]
                                {
                                new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Upgrade")),
                                new Response("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave"))
                                };
                            }
                            __instance.createQuestionDialogue("", responses, "Blacksmith");
                        }
                        __result = true;
                        return false;
                    case "AnimalShop":
                        options = new List<Response>
                    {
                        new Response("Purchase", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Animals")),
                        new Response("Leave", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Leave"))
                    };
                        __instance.createQuestionDialogue("", options.ToArray(), "Marnie");
                        __result = true;
                        return false;
                }
            }
            return true;
        }
        public static bool GameLocation_openShopMenu_Prefix(GameLocation __instance, string which, ref bool __result)
        {
            if (ModEntry.ModHelper.ModRegistry.IsLoaded("DiogoAlbano.BNWChapter1"))
            {
                string name = null;
                switch (which)
                {
                    case "Fish":
                        foreach (var c in __instance.characters)
                        {
                            if (c.Name == "Willy")
                            {
                                name = "Willy";
                                break;
                            }
                        }
                        Game1.activeClickableMenu = new ShopMenu(Utility.getFishShopStock(Game1.player), 0, name, null, null, null);
                        __result = true;
                        return false;
                    case "General":
                        foreach (var c in __instance.characters)
                        {
                            if (c.Name == "Pierre")
                            {
                                name = "Pierre";
                                break;
                            }
                        }
                        Game1.activeClickableMenu = new ShopMenu((__instance as SeedShop).shopStock(), 0, name, null, null, null);
                        __result = true;
                        return false;
                    case "SandyShop":
                        foreach (var c in __instance.characters)
                        {
                            if (c.Name == "Sandy")
                            {
                                name = "Sandy";
                                break;
                            }
                        }
                        Game1.activeClickableMenu = new ShopMenu((Dictionary<ISalable, int[]>)AccessTools.Method(typeof(GameLocation), "sandyShopStock").Invoke(__instance, new object[] { }), 0, name, (Func<ISalable, Farmer, int, bool>)Delegate.CreateDelegate(typeof(Func<ISalable, Farmer, int, bool>), __instance, AccessTools.Method(typeof(GameLocation), "onSandyShopPurchase")), null, null);
                        __result = true;
                        return false;
                }
            }
            return true;
        }
    }
}
