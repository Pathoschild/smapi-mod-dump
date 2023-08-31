/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Menus;
using xTile.ObjectModel;
using xTile.Tiles;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewModdingAPI;
using NovoMundo.Farm1;
using NovoMundo.Farm2;
using Microsoft.Xna.Framework.Graphics;
using NovoMundo.Menus;
using System.Collections.Generic;
using xTile.Dimensions;

namespace NovoMundo.Managers
{
    public class Property_Manager
    {
        private Vector2 _playerPos;
        internal static GameLocation SourceLocation;
        public void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            
            if (!Context.CanPlayerMove)
                return;
            SourceLocation = null;
            _playerPos = Vector2.Zero;
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
            IPropertyCollection tileProperty = GetTileProperty(Game1.currentLocation, "Buildings", clickedTile);
            if (tileProperty == null)
                return;
            CheckTileProperty(tileProperty, e);
        
        }
        public void OnGameLoop(object sender, UpdateTickingEventArgs e)
        {
            if (SourceLocation != null && (Game1.locationRequest?.Name == "AnimalShop" || Game1.locationRequest?.Name == "WizardHouse" || Game1.locationRequest?.Name == "ScienceHouse"))
            {
                Game1.locationRequest.Location = SourceLocation;
                Game1.locationRequest.IsStructure = SourceLocation.isStructure.Value;
            }
        }
        public void OnDisplayMenuChanged(object sender, MenuChangedEventArgs e)
        {
            _playerPos = Vector2.Zero;
            if (e.NewMenu == null && _playerPos != Vector2.Zero)
            {
                Game1.player.position.Set(_playerPos);
            }
            if (e.OldMenu is CarpenterMenu || e.OldMenu is Farm1_Builder || e.OldMenu is Farm2_Builder || e.OldMenu is PurchaseAnimalsMenu)
            {
                if(e.NewMenu is DialogueBox)
                {
                    Game1.exitActiveMenu();
                    Game1.drawObjectDialogue(ModEntry.ModHelper.Translation.Get("NPCDataATMRequestSucess"));
                }
            }
        }
        public static IPropertyCollection GetTileProperty(GameLocation map, string layer, Vector2 tile)
        {
            if (map == null)
                return null;
            Tile checkTile = map.Map.GetLayer(layer).Tiles[(int)tile.X, (int)tile.Y];
            return checkTile?.Properties;
        }
        public void CheckTileProperty(IPropertyCollection tileProperty, ButtonPressedEventArgs e)
        {
            tileProperty.TryGetValue("nm_Action", out PropertyValue property);
            if (property != null)
            {
                var checking = (string)property;
                switch (checking)
                {
                    case "Carpenter_ATM":
                        {
                            ifAwayUseATM("nmATM", "Carpenter", true);
                            break;
                        }
                    case "Carpenter_Robin":
                        {
                            isNPCaway("Robin", out string npcchecked);
                            ifAwayUseATM(npcchecked, "Carpenter", false);
                            break;
                        }
                    case "NewJojaForm":
                        {
                            callMenu(checking);
                            break;
                        }
                        
                }             
            }
        }
        public void callMenu(string menu)
        {
            IClickableMenu whichMenu = WhichMenu(menu, out bool warpingShop);
            if (menu != null)
            {
                if (warpingShop)
                {
                    SourceLocation = Game1.currentLocation;
                    _playerPos = Game1.player.position.Get();
                }
                Game1.activeClickableMenu = whichMenu;
            }
        }
        public static void isNPCaway(string npc, out string npcchecked)
        {
            npcchecked = npc;
            if (Game1.currentLocation.getCharacterFromName(npc) is null)
            {
                npcchecked = null;
            }
        }
        public void ifAwayUseATM(string npcchecked, string caller, bool noload)
        {
            if (npcchecked != null)
            {
                createQuestionDialogue(caller, noload);
            }
            else
            {
                Game1.drawObjectDialogue(ModEntry.ModHelper.Translation.Get("NPCDataVendorAway"));
            }
        }
        public static bool getBasiShop(string npcchecked, Dictionary<ISalable, int[]> stock, int currency, out ShopMenu shop)
        {           
            shop = new ShopMenu(stock, currency, npcchecked);
            if (npcchecked is null)
            {
                shop.portraitPerson = Game1.getCharacterFromName("nmATM");
                shop.potraitPersonDialogue = Game1.parseText(ModEntry.ModHelper.Translation.Get("NPCDataATMQuote"), Game1.dialogueFont, 304);
            }          
            return true;
        }
        public static IClickableMenu WhichMenu(string menu, out bool warpingShop)
        {           
            warpingShop = false;
            Texture2D jojaFormTexture = ModEntry.ModHelper.ModContent.Load<Texture2D>(ModEntry.ModHelper.Translation.Get("ImagePathNewJojaFormTexture"));
            switch (menu)
            {
                case "CarpenterStock":
                    {
                        isNPCaway("Robin", out string npcchecked);
                        getBasiShop(npcchecked, Utility.getCarpenterStock(), 0, out ShopMenu shop);
                        return shop;
                    }                  
                case "CarpenterBuilderVanilla":
                    {
                        warpingShop = true;
                        return new CarpenterMenu();
                    }
                case "Farm1_Builder":
                    {
                        warpingShop = true;
                        return new Farm1_Builder();
                    }                 
                case "Farm2_Builder":
                    {
                        warpingShop = true;
                        return new Farm2_Builder();
                    }
                case "NewJojaForm":
                    {
                        return new NewJojaForm(jojaFormTexture);
                    }                   
            }
            return null;
        }
        
        public void createQuestionDialogue(string checking, bool noload)
        {            
            List<Response> options = new();
            switch (checking)
            {
                case "Carpenter":
                    {
                        isNPCaway("Robin", out string npcchecked);
                        options.Add(new Response("CarpenterStock", ModEntry.ModHelper.Translation.Get("QuestionDialoguesChoicesShop")));
                        options.Add(new Response("CarpenterBuilderVanilla", ModEntry.ModHelper.Translation.Get("QuestionDialoguesChoicesCarpenterBuilderVanilla")));
                        options.Add(new Response("Farm1Builder", ModEntry.ModHelper.Translation.Get("QuestionDialoguesChoicesFarm1_Builder")));
                        options.Add(new Response("Farm2Builder", ModEntry.ModHelper.Translation.Get("QuestionDialoguesChoicesFarm2_Builder")));
                        if (npcchecked !=null && noload is false)
                        {
                            options.Add(new Response("SpecialJobs", ModEntry.ModHelper.Translation.Get("QuestionDialoguesChoicesSpecialJobs")));
                        }
                        //if (Game1.IsMasterGame)
                        //{
                            //if (Game1.player.HouseUpgradeLevel < 3)
                            //{
                                //options.Add(new Response("Upgrade", ModEntry.ModHelper.Translation.Get("ImagePathNewJojaFormTexture")));
                            //}
                        //}
                        //else if (Game1.player.HouseUpgradeLevel < 3)
                        //{
                            //options.Add(new Response("Upgrade", ModEntry.ModHelper.Translation.Get("ImagePathNewJojaFormTexture")));
                        //}
                        //if (Game1.player.HouseUpgradeLevel >= 2)
                        //{
                            //if (Game1.IsMasterGame)
                            //{
                                //options.Add(new Response("Renovate", ModEntry.ModHelper.Translation.Get("ImagePathNewJojaFormTexture")));
                            //}
                            //else
                            //{
                                //options.Add(new Response("Renovate", ModEntry.ModHelper.Translation.Get("ImagePathNewJojaFormTexture")));
                            //}
                        //}
                        options.Add(new Response("Leave", ModEntry.ModHelper.Translation.Get("QuestionDialoguesNoChoice")));
                        Game1.currentLocation.createQuestionDialogue(ModEntry.ModHelper.Translation.Get("QuestionDialoguesTile"), options.ToArray(), new GameLocation.afterQuestionBehavior(CreateQuestionDialogue_output));
                        break;
                    }    
            }
        }
        public void CreateQuestionDialogue_output(Farmer farmer, string choice)
        {
            switch (choice)
            {
                case "CarpenterStock":
                    {
                        callMenu("CarpenterStock");
                        break;
                    }
                case "CarpenterBuilderVanilla":
                    {
                        if (Game1.player.daysUntilHouseUpgrade.Value < 0 && !Game1.getFarm().isThereABuildingUnderConstruction() && !getFarm1().isThereABuildingUnderConstruction() && !getFarm2().isThereABuildingUnderConstruction())
                        {
                            callMenu("CarpenterBuilderVanilla");
                        }
                        else
                        {
                            if (Game1.currentLocation.getCharacterFromName("Robin") != null)
                            {
                                Game1.drawDialogue(Game1.getCharacterFromName("Robin"), "Estou ocupada.");
                            }
                            else
                            {
                                Game1.drawObjectDialogue("Robin ocupada.");
                            }
                        }
                        break;
                    }
                case "Farm1Builder":
                    {
                        if (Game1.player.daysUntilHouseUpgrade.Value < 0 && !Game1.getFarm().isThereABuildingUnderConstruction() && !getFarm1().isThereABuildingUnderConstruction() && !getFarm2().isThereABuildingUnderConstruction())
                        {
                            callMenu("Farm1_Builder");
                        }
                        else
                        {
                            if (Game1.currentLocation.getCharacterFromName("Robin") != null)
                            {
                                Game1.drawDialogue(Game1.getCharacterFromName("Robin"), "Estou ocupada.");
                            }
                            else
                            {
                                Game1.drawObjectDialogue("Robin ocupada.");
                            }
                        }
                        break;
                    }
                case "Farm2Builder":
                    {
                        if (Game1.player.daysUntilHouseUpgrade.Value < 0 && !Game1.getFarm().isThereABuildingUnderConstruction() && !getFarm1().isThereABuildingUnderConstruction() && !getFarm2().isThereABuildingUnderConstruction())
                        {
                            callMenu("Farm2_Builder");
                        }
                        else
                        {
                            if (Game1.currentLocation.getCharacterFromName("Robin") != null)
                            {
                                Game1.drawDialogue(Game1.getCharacterFromName("Robin"), "Estou ocupada.");
                            }
                            else
                            {
                                Game1.drawObjectDialogue("Robin ocupada.");
                            }
                        }
                        break;
                    }
                case "SpecialJobs":
                    {
                        if (ModEntry.ModHelper.ModRegistry.IsLoaded("PeacefulEnd.AMouseWithAHat.Core") && Game1.MasterPlayer.mailReceived.Contains("hatter") is true && Game1.MasterPlayer.mailReceived.Contains("HatShopRepaired") is false)
                        {
                            Location location = new();
                            Game1.currentLocation.carpenters(location);
                        }
                        else
                        {
                            Game1.drawDialogue(Game1.getCharacterFromName("Robin"), ModEntry.ModHelper.Translation.Get("QuestionDialoguesNoSpecialJobs"));
                        }
                        break;
                    }
                case "Upgrade":
                    if (Game1.player.daysUntilHouseUpgrade.Value < 0 && !Game1.getFarm().isThereABuildingUnderConstruction() && !getFarm1().isThereABuildingUnderConstruction() && !getFarm2().isThereABuildingUnderConstruction())
                    {
                        switch (Game1.player.HouseUpgradeLevel)
                        {
                            case 0:
                                Game1.currentLocation.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse1")), Game1.currentLocation.createYesNoResponses(), "uphouse");
                                break;
                            case 1:
                                Game1.currentLocation.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse2")), Game1.currentLocation.createYesNoResponses(), "uphouse");
                                break;
                            case 2:
                                Game1.currentLocation.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse3")), Game1.currentLocation.createYesNoResponses(), "uphouse");
                                break;
                        }
                    }
                    else
                    {
                        if (Game1.currentLocation.getCharacterFromName("Robin") != null)
                        {
                            Game1.drawDialogue(Game1.getCharacterFromName("Robin"), "Estou ocupada.");
                        }
                        else
                        {
                            Game1.drawObjectDialogue("Robin ocupada.");
                        }
                    }
                    break;
            }
        }
        public static NMFarm1 getFarm1()
        {
            return Game1.getLocationFromName("NMFarm1") as NMFarm1;
        }
        public static NMFarm2 getFarm2()
        {
            return Game1.getLocationFromName("NMFarm2") as NMFarm2;
        }
    }
}
