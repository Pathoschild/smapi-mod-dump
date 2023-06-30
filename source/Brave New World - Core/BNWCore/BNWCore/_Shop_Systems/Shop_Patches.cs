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
using StardewValley.Util;
using System;
using System.Collections.Generic;
using xTile.ObjectModel;
using xTile.Tiles;
using Microsoft.Xna.Framework;
using Object = StardewValley.Object;
using StardewModdingAPI.Events;
using StardewModdingAPI;

namespace BNWCore
{
    internal class Shop_Patches
    {
        private Vector2 _playerPos;
        public static string bnw_EradicationList { get; set; } = "bnw_EradicationList";
        public static string bnw_AdventureGuildChoices { get; set; } = "bnw_AdventureGuildChoices";
        public static string bnw_AnimalShopChoices { get; set; } = "bnw_AnimalShopChoices";
        public static string bnw_BlacksmithChoices { get; set; } = "bnw_BlacksmithChoices";
        public static string bnw_BlacksmithChoices_no_npc { get; set; } = "bnw_BlacksmithChoices_no_npc";
        public static string bnw_FishShopChoices { get; set; } = "bnw_FishShopChoices";
        public static string bnw_HospitalChoices { get; set; } = "bnw_HospitalChoices";
        public void Eradication_List()
        {
            List<Response> choices = new List<Response>()
            {
                new Response("Eradicator_List", ModEntry.ModHelper.Translation.Get("translator_dialog_box_choice_yes")),
                new Response("Leave", ModEntry.ModHelper.Translation.Get("translator_dialog_box_choice_no")),
            };
            Game1.currentLocation.createQuestionDialogue($"{ModEntry.ModHelper.Translation.Get("translator_dialog_box_choice_bnw_EradicationList")}", choices.ToArray(), new GameLocation.afterQuestionBehavior(List_Output));
        }
        public void Adventure_Guild_Choices()
        {
            List<Response> choices = new List<Response>()
            {
                new Response("bnw_MarlonShop_no_npc", ModEntry.ModHelper.Translation.Get("translator_dialog_box_buy_weapons_and_equipments")),
                new Response("bnw_AdventureRecovery_no_npc", ModEntry.ModHelper.Translation.Get("translator_dialog_box_choice_bnw_AdventureRecovery")),
                new Response("Eradicator_List", ModEntry.ModHelper.Translation.Get("translator_dialog_box_choice_bnw_EradicationList_option")),
                new Response("Leave", ModEntry.ModHelper.Translation.Get("translator_dialog_box_choice_close")),
            };
            Game1.currentLocation.createQuestionDialogue($"{ModEntry.ModHelper.Translation.Get("translator_dialog_box_services")}", choices.ToArray(), new GameLocation.afterQuestionBehavior(List_Output));
        }
        public void Animal_Shop_Choices()
        {
            List<Response> choices = new List<Response>()
            {
                new Response("bnw_MarnieShop_no_npc", ModEntry.ModHelper.Translation.Get("translator_dialog_box_choice_inventory")),
                new Response("bnw_MarnieAnimalShop", ModEntry.ModHelper.Translation.Get("translator_dialog_box_choice_animalshop")),
                new Response("Leave", ModEntry.ModHelper.Translation.Get("translator_dialog_box_choice_close")),
            };
            Game1.currentLocation.createQuestionDialogue($"{ModEntry.ModHelper.Translation.Get("translator_dialog_box_services")}", choices.ToArray(), new GameLocation.afterQuestionBehavior(List_Output));
        }
        public void Blacksmith_Choices()
        {
            List<Response> choices = new List<Response>()
            {
                new Response("bnw_ClintShop_no_npc", ModEntry.ModHelper.Translation.Get("translator_dialog_box_choice_inventory")),
                new Response("bnw_ClintGeodes", ModEntry.ModHelper.Translation.Get("translator_dialog_box_choice_break_geodes")),
                new Response("Leave", ModEntry.ModHelper.Translation.Get("translator_dialog_box_choice_close")),
            };
            Game1.currentLocation.createQuestionDialogue($"{ModEntry.ModHelper.Translation.Get("translator_dialog_box_services")}", choices.ToArray(), new GameLocation.afterQuestionBehavior(List_Output));
        }
        public void Blacksmith_Choices_no_npc()
        {
            List<Response> choices = new List<Response>()
            {
               new Response("bnw_ClintShop_no_npc", ModEntry.ModHelper.Translation.Get("translator_dialog_box_choice_inventory")),
                new Response("Cant_break_geodes", ModEntry.ModHelper.Translation.Get("translator_dialog_box_choice_break_geodes")),
                new Response("Leave", ModEntry.ModHelper.Translation.Get("translator_dialog_box_choice_close")),
            };
            Game1.currentLocation.createQuestionDialogue($"{ModEntry.ModHelper.Translation.Get("translator_dialog_box_services")}", choices.ToArray(), new GameLocation.afterQuestionBehavior(List_Output));
        }
        public void FishShop_Choices()
        {
            List<Response> choices = new List<Response>()
            {
                new Response("bnw_WillyShop_no_npc", ModEntry.ModHelper.Translation.Get("translator_dialog_box_choice_inventory")),
                new Response("Leave", ModEntry.ModHelper.Translation.Get("translator_dialog_box_choice_close")),
            };
            Game1.currentLocation.createQuestionDialogue($"{ModEntry.ModHelper.Translation.Get("translator_dialog_box_services")}", choices.ToArray(), new GameLocation.afterQuestionBehavior(List_Output));
        }
        public void Hospital_Choices()
        {
            List<Response> choices = new List<Response>()
            {
                new Response("bnw_HarveyShop", ModEntry.ModHelper.Translation.Get("translator_dialog_box_choice_inventory")),
                new Response("Leave", ModEntry.ModHelper.Translation.Get("translator_dialog_box_choice_close")),
            };
            Game1.currentLocation.createQuestionDialogue($"{ModEntry.ModHelper.Translation.Get("translator_dialog_box_services")}", choices.ToArray(), new GameLocation.afterQuestionBehavior(List_Output));
        }
        public static IPropertyCollection GetTileProperty(GameLocation map, string layer, Vector2 tile)
        {
            if (map == null)
                return null;
            Tile checkTile = map.Map.GetLayer(layer).Tiles[(int)tile.X, (int)tile.Y];
            return checkTile?.Properties;
        }
        
        public void CheckForShopToOpen(IPropertyCollection tileProperty, ButtonPressedEventArgs e)
        {
            tileProperty.TryGetValue("Shop", out PropertyValue shopProperty);
            if (shopProperty != null)
            {
                if (shopProperty == bnw_EradicationList)
                {
                    Eradication_List();
                }
                else if (shopProperty == bnw_AdventureGuildChoices)
                {
                    Adventure_Guild_Choices();
                }
                else if (shopProperty == bnw_AnimalShopChoices)
                {
                    Animal_Shop_Choices();
                }
                else if (shopProperty == bnw_BlacksmithChoices)
                {
                    Blacksmith_Choices();
                }
                else if (shopProperty == bnw_BlacksmithChoices_no_npc)
                {
                    Blacksmith_Choices_no_npc();
                }
                else if (shopProperty == bnw_FishShopChoices)
                {
                    FishShop_Choices();
                }
                else if (shopProperty == bnw_HospitalChoices)
                {
                    Hospital_Choices();
                }
                else
                {
                    IClickableMenu menu = CheckVanillaShop(shopProperty, out bool warpingShop);
                    if (menu != null)
                    {
                        if (warpingShop)
                        {
                            ModEntry.SourceLocation = Game1.currentLocation;
                            _playerPos = Game1.player.position.Get();
                        }
                        Game1.activeClickableMenu = menu;
                    }
                }
            }
        }
        public static IClickableMenu CheckVanillaShop(string shopProperty, out bool warpingShop)
        {
            warpingShop = false;
            switch (shopProperty)
            {
                case "bnw_closedShop":
                    return new DialogueBox(ModEntry.ModHelper.Translation.Get("translator_dialog_box_choice_closedShop"));
                case "bnw_PierreShop_no_npc":
                    {
                        var seedShop = new SeedShop();
                        return new ShopMenu(seedShop.shopStock(), 0, null, null, null, null);
                    }
                case "bnw_PierreShop":
                    {
                        var seedShop = new SeedShop();
                        return new ShopMenu(seedShop.shopStock(), 0, "Pierre");
                    }
                case "bnw_JojaShop":
                    return new ShopMenu(Utility.getJojaStock());
                case "bnw_RobinShop_no_npc":
                    return new ShopMenu(Utility.getCarpenterStock(), 0, null, null, null, null);
                case "bnw_RobinShop":
                    return new ShopMenu(Utility.getCarpenterStock(), 0, "Robin");
                case "bnw_RobinBuilder":
                    warpingShop = true;
                    if (Game1.player.daysUntilHouseUpgrade.Value < 0 && !Game1.getFarm().isThereABuildingUnderConstruction())
                        return new CarpenterMenu();
                    else
                        return new DialogueBox(ModEntry.ModHelper.Translation.Get("translator_dialog_box_robin_busy"));
                case "bnw_ClintShop_no_npc":
                    return new ShopMenu(Utility.getBlacksmithStock(), 0, null, null, null, null);
                case "bnw_ClintShop":
                    return new ShopMenu(Utility.getBlacksmithStock(), 0, "Clint");
                case "bnw_ClintToolUpgrades_no_npc":
                    return new ShopMenu(Utility.getBlacksmithUpgradeStock(Game1.player), 0, null);
                case "bnw_ClintToolUpgrades":
                    return new ShopMenu(Utility.getBlacksmithUpgradeStock(Game1.player), 0, "ClintUpgrade");
                case "bnw_ClintGeodes":
                    return new GeodeMenu();
                case "bnw_MarlonShop_no_npc":
                    return new ShopMenu(Utility.getAdventureShopStock(), 0, null, null, null, null);
                case "bnw_MarlonShop":
                    return new ShopMenu(Utility.getAdventureShopStock(), 0, "Marlon");
                case "bnw_AdventureRecovery_no_npc":
                    return new ShopMenu(Utility.getAdventureRecoveryStock(), 0, null);
                case "bnw_AdventureRecovery":
                    return new ShopMenu(Utility.getAdventureRecoveryStock(), 0, "Marlon_Recovery");
                case "bnw_MarnieShop_no_npc":
                    warpingShop = true;
                    return new ShopMenu(Utility.getAnimalShopStock(), 0, null, null, null, null);
                case "bnw_MarnieShop":
                    warpingShop = true;
                    return new ShopMenu(Utility.getAnimalShopStock(), 0, "Marnie");
                case "bnw_MarnieAnimalShop":
                    warpingShop = true;
                    return new PurchaseAnimalsMenu(Utility.getPurchaseAnimalStock());
                case "bnw_TravellingMerchant_no_npc":
                    return new ShopMenu(Utility.getTravelingMerchantStock((int)((long)Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed)), 0, null, Utility.onTravelingMerchantShopPurchase);
                case "bnw_TravellingMerchant":
                    return new ShopMenu(Utility.getTravelingMerchantStock((int)((long)Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed)), 0, "Traveler", Utility.onTravelingMerchantShopPurchase);
                case "bnw_HarveyShop":
                    return new ShopMenu(Utility.getHospitalStock());
                case "bnw_SandyShop_no_npc":
                    {
                        var SandyStock = ModEntry.ModHelper.Reflection.GetMethod(Game1.currentLocation, null).Invoke<Dictionary<ISalable, int[]>>();
                        return new ShopMenu(SandyStock, 0, null, onSandyShopPurchase);
                    }
                case "bnw_SandyShop":
                    {
                        var SandyStock = ModEntry.ModHelper.Reflection.GetMethod(Game1.currentLocation, null).Invoke<Dictionary<ISalable, int[]>>();
                        return new ShopMenu(SandyStock, 0, "Sandy", onSandyShopPurchase);
                    }
                case "bnw_DesertTrader_no_npc":
                    return new ShopMenu(Desert.getDesertMerchantTradeStock(Game1.player), 0, null, boughtTraderItem);
                case "bnw_DesertTrader":
                    return new ShopMenu(Desert.getDesertMerchantTradeStock(Game1.player), 0, "DesertTrade", boughtTraderItem);
                case "bnw_KrobusShop_no_npc":
                    {
                        var sewer = new Sewer();
                        return new ShopMenu(sewer.getShadowShopStock(), 0, null, sewer.onShopPurchase);
                    }
                case "bnw_KrobusShop":
                    {
                        var sewer = new Sewer();
                        return new ShopMenu(sewer.getShadowShopStock(), 0, "Krobus", sewer.onShopPurchase);
                    }
                case "bnw_DwarfShop_no_npc":
                    return new ShopMenu(Utility.getDwarfShopStock(), 0, null, null, null, null);
                case "bnw_DwarfShop":
                    return new ShopMenu(Utility.getDwarfShopStock(), 0, "Dwarf");
                case "bnw_GusShop_no_npc":
                    {
                        return new ShopMenu(Utility.getSaloonStock(), 0, null, (item, farmer, amount) =>
                        {
                            Game1.player.team.synchronizedShopStock.OnItemPurchased(SynchronizedShopStock.SynchedShop.Saloon, item, amount);
                            return false;
                        });
                    }
                case "bnw_GusShop":
                    {
                        return new ShopMenu(Utility.getSaloonStock(), 0, "Gus", (item, farmer, amount) =>
                        {
                            Game1.player.team.synchronizedShopStock.OnItemPurchased(SynchronizedShopStock.SynchedShop.Saloon, item, amount);
                            return false;
                        });
                    }
                case "bnw_WillyShop_no_npc":
                    return new ShopMenu(Utility.getFishShopStock(Game1.player), 0, null);
                case "bnw_WillyShop":
                    return new ShopMenu(Utility.getFishShopStock(Game1.player), 0, "Willy");
                case "bnw_WizardBuildings":
                    warpingShop = true;
                    return new CarpenterMenu(true);
                case "bnw_QiShop":
                    Game1.activeClickableMenu = new ShopMenu(Utility.getQiShopStock(), 2);
                    break;
                case "bnw_QiWallnutShop":
                    Game1.activeClickableMenu = new ShopMenu(Utility.GetQiChallengeRewardStock(Game1.player));
                    break;
                case "bnw_IceCreamStand":
                    return new ShopMenu(new Dictionary<ISalable, int[]>
                    {
                        {
                            new Object(233, 1), new[]{ 250, int.MaxValue }
                        }
                    });
            }
            return null;
        }
        private static bool boughtTraderItem(ISalable s, Farmer f, int i)
        {
            if (s.Name == "Magic Rock Candy")
                Desert.boughtMagicRockCandy = true;
            return false;
        }
        private static bool onSandyShopPurchase(ISalable item, Farmer who, int amount)
        {
            Game1.player.team.synchronizedShopStock.OnItemPurchased(SynchronizedShopStock.SynchedShop.Sandy, item, amount);
            return false;
        }
        public void GameLoop_UpdateTicking(UpdateTickingEventArgs e)
        {
            if (ModEntry.SourceLocation != null && (Game1.locationRequest?.Name == "AnimalShop" || Game1.locationRequest?.Name == "WizardHouse" || Game1.locationRequest?.Name == "ScienceHouse"))
            {
                Game1.locationRequest.Location = ModEntry.SourceLocation;
                Game1.locationRequest.IsStructure = ModEntry.SourceLocation.isStructure.Value;
            }
        }
        public void Display_MenuChanged(MenuChangedEventArgs e)
        {
            _playerPos = Vector2.Zero;
            if (e.OldMenu is CarpenterMenu && e.NewMenu is DialogueBox)
            {
                Game1.exitActiveMenu();
                Game1.activeClickableMenu = new DialogueBox(ModEntry.ModHelper.Translation.Get("translator_dialog_box_building_requested"));
            }
            if (e.NewMenu == null && _playerPos != Vector2.Zero)
            {
                Game1.player.position.Set(_playerPos);
            }
            if (e.OldMenu is PurchaseAnimalsMenu && e.NewMenu is DialogueBox)
            {
                Game1.exitActiveMenu();
                Game1.activeClickableMenu = new DialogueBox(ModEntry.ModHelper.Translation.Get("translator_dialog_box_animal_delivered"));
            }
            if (e.OldMenu is ShopMenu && e.NewMenu is DialogueBox)
                if(Game1.player.currentLocation == Game1.getLocationFromName("Blacksmith"))
                {
                    Game1.exitActiveMenu();
                    Game1.activeClickableMenu = new DialogueBox(ModEntry.ModHelper.Translation.Get("translator_dialog_box_tool_upgraded"));
                }
                else
                {
                    Game1.exitActiveMenu();
                    Game1.activeClickableMenu = new DialogueBox(ModEntry.ModHelper.Translation.Get("translator_dialog_box_request_fulfilled"));
                }
            
        }
        public void Input_ButtonPressed(ButtonPressedEventArgs e)
        {
            if (!Context.CanPlayerMove)
                return;
            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                if (e.Button != SButton.MouseLeft)
                    return;
                if (e.Cursor.GrabTile != e.Cursor.Tile)
                    return;
            }
            else if (!e.Button.IsActionButton())
                return;
            Vector2 clickedTile = ModEntry.ModHelper.Input.GetCursorPosition().GrabTile;
            IPropertyCollection tileProperty = Shop_Patches.GetTileProperty(Game1.currentLocation, "Buildings", clickedTile);
            if (tileProperty == null)
                return;
            CheckForShopToOpen(tileProperty, e);         
        }
        public void List_Output(Farmer who, string dialogue_id)
        {
            if (dialogue_id == "UpgradeOrRenovate")
            {            
                if (Game1.player.daysUntilHouseUpgrade.Value < 0 && !Game1.getFarm().isThereABuildingUnderConstruction())
                {
                    List<Response> options = new List<Response>();
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
                    Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString($"Strings\\Locations:ScienceHouse_CarpenterMenu"), options.ToArray(), "carpenter");
                }
                else
                {
                    Game1.activeClickableMenu = new DialogueBox(ModEntry.ModHelper.Translation.Get("translator_dialog_box_robin_busy"));
                }
            }
            else if (dialogue_id == "Eradicator_List")
            {
                var adventureguild = new AdventureGuild();
                adventureguild.showMonsterKillList();
            }
            else if (dialogue_id == "Cant_break_geodes")
            {
                Game1.activeClickableMenu = new DialogueBox(ModEntry.ModHelper.Translation.Get("translator_dialog_box_choice_cant_break_geodes"));
            }
            else
            {
                IClickableMenu menu = CheckVanillaShop(dialogue_id, out bool warpingShop);
                if (menu != null)
                {
                    if (warpingShop)
                    {
                        ModEntry.SourceLocation = Game1.currentLocation;
                        _playerPos = Game1.player.position.Get();
                    }
                    Game1.activeClickableMenu = menu;
                }
            }
        }
  
        public void OnUpdateTicked(UpdateTickedEventArgs e)
        {
            if (!e.IsMultipleOf(8))
                return;
            Farmer farmer = Game1.player;
            Item item;
            try
            {
                item = farmer.Items[farmer.CurrentToolIndex];
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }
            if (Game1.player.daysLeftForToolUpgrade.Value > 0)
            {
                if (Game1.player.toolBeingUpgraded.Value is GenericTool genericTool)
                {
                    genericTool.actionWhenClaimed();
                }
                else
                {
                    Game1.player.addItemToInventory(Game1.player.toolBeingUpgraded.Value);
                }
                Game1.player.toolBeingUpgraded.Value = null;
                Game1.player.daysLeftForToolUpgrade.Value = 0;
            }

        }
    }
}
