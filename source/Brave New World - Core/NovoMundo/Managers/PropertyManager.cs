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
            if (e.OldMenu is CarpenterMenu || e.OldMenu is Carpenter_Builder || e.OldMenu is PurchaseAnimalsMenu)
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
                    case "JojaATM":
                        {
                            bool noload = false;
                            if (Utility.doesAnyFarmerHaveOrWillReceiveMail("nmQuarry") is true && Utility.doesAnyFarmerHaveOrWillReceiveMail("nmCinema") is true && Utility.doesAnyFarmerHaveOrWillReceiveMail("nmFarm") is true && Utility.doesAnyFarmerHaveOrWillReceiveMail("nmLake") is true)
                            {
                                noload = true;
                            }
                            createQuestionDialogue(checking, noload);
                            break;
                        }
                    case "JojaNPC":
                        {
                            isNPCaway("Claire", out string npcchecked);
                            if(npcchecked!= null) 
                            {
                                createQuestionDialogue(checking, false);
                                break;
                            }
                            isNPCaway("Martin", out string npcchecked2);
                            if (npcchecked2 != null)
                            {
                                createQuestionDialogue(checking, false);
                                break;
                            }
                            Game1.drawObjectDialogue(ModEntry.ModHelper.Translation.Get("NPCDataVendorAway"));
                            break;
                        }
                    case "CarpenterATM":
                        {
                            //isNPCaway("Robin", out string npcchecked);
                            //if (npcchecked != null)
                            //{
                            //createQuestionDialogue(checking, false);
                            //break;
                            //}
                            //Game1.drawObjectDialogue(ModEntry.ModHelper.Translation.Get("NPCDataVendorAway"));
                            //break;
                             createQuestionDialogue(checking, false);
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
        public static IClickableMenu WhichMenu(string menu, out bool warpingShop)
        {           
            warpingShop = false;
            Texture2D jojaATMmenutexture = ModEntry.ModHelper.ModContent.Load<Texture2D>(ModEntry.ModHelper.Translation.Get("ImagePathNewjojaATMMenuTexture"));
            Texture2D carpenterATMmaintexture = ModEntry.ModHelper.ModContent.Load<Texture2D>(ModEntry.ModHelper.Translation.Get("ImagePathNewCarpenterATMMainTexture"));
            Texture2D carpenterATMselecttypetexture = ModEntry.ModHelper.ModContent.Load<Texture2D>(ModEntry.ModHelper.Translation.Get("ImagePathNewCarpenterATMSelectTypeTexture"));
            Texture2D carpenterATMselectwheretexture = ModEntry.ModHelper.ModContent.Load<Texture2D>(ModEntry.ModHelper.Translation.Get("ImagePathNewCarpenterATMSelectWhereTexture"));
            Texture2D carpenterATMimprovtexture = ModEntry.ModHelper.ModContent.Load<Texture2D>(ModEntry.ModHelper.Translation.Get("ImagePathNewCarpenterATMImprovementsLocked0Texture"));//upgrade 1
            if (!Utility.doesAnyFarmerHaveOrWillReceiveMail("nmQuarry"))
            {
                jojaATMmenutexture = ModEntry.ModHelper.ModContent.Load<Texture2D>(ModEntry.ModHelper.Translation.Get("ImagePathNewjojaATMMenuLockedTexture"));

            }
            if (Game1.player.HouseUpgradeLevel == 1)
            {
                carpenterATMimprovtexture = ModEntry.ModHelper.ModContent.Load<Texture2D>(ModEntry.ModHelper.Translation.Get("ImagePathNewCarpenterATMImprovementsLocked1Texture"));//upgrade 2
            }
            else if (Game1.player.HouseUpgradeLevel == 2)
            {
                carpenterATMimprovtexture = ModEntry.ModHelper.ModContent.Load<Texture2D>(ModEntry.ModHelper.Translation.Get("ImagePathNewCarpenterATMImprovementsUnlockedTexture"));//cellar
            }
            else if (Game1.player.HouseUpgradeLevel == 3)
            {
                carpenterATMimprovtexture = ModEntry.ModHelper.ModContent.Load<Texture2D>(ModEntry.ModHelper.Translation.Get("ImagePathNewCarpenterATMImprovementsCompletedTexture"));//completo
            }
            switch (menu)
            {                
                case "JojaNPCCashier":
                    {
                        string npc = null;
                        isNPCaway("Claire", out string npcchecked);
                        if(npcchecked != null)
                        {
                            npc = npcchecked;
                        }
                        isNPCaway("Martin", out string npcchecked2);
                        if(npcchecked2 !=null)
                        {
                            npc= npcchecked2;
                        }
                        if(npcchecked2 != null && npcchecked != null)
                        {
                            npc = "Martin";
                        }
                        var shop = new ShopMenu(Utility.getJojaStock())
                        {
                            portraitPerson = Game1.getCharacterFromName(npc),
                            potraitPersonDialogue = Game1.parseText(ModEntry.ModHelper.Translation.Get("NPCDataJojaNPCQuote"), Game1.dialogueFont, 304)
                        };
                        return shop;                        
                    }
                case "JojaFormATM":
                    {
                        return new Form_Builder(jojaATMmenutexture, 1, 1, true, 4);
                    }
                case "JojaStock":
                    {
                        var shop = new ShopMenu(Utility.getJojaStock(), 0)
                        {
                            portraitPerson = Game1.getCharacterFromName("nmATMJoja"),
                            potraitPersonDialogue = Game1.parseText(ModEntry.ModHelper.Translation.Get("NPCDataATMQuote"), Game1.dialogueFont, 304)

                        };
                        return shop;
                    }
                case "CarpenterFormATM":
                    {
                        return new Form_Builder(carpenterATMmaintexture, 0, 0, false, 4);
                    }
                case "CarpenterFormATMSelectType":
                    {
                        return new Form_Builder(carpenterATMselecttypetexture, 0, 2, false, 2);
                    }
                case "CarpenterFormATMSelectWhere":
                    {
                        return new Form_Builder(carpenterATMselectwheretexture, 0, 3, false, 4);
                    }
                case "CarpenterFormATMImprovements0":
                    {
                        return new Form_Builder(carpenterATMimprovtexture, 0, 4, true, 4);
                    }
                case "CarpenterFormATMImprovements1":
                    {
                        return new Form_Builder(carpenterATMimprovtexture, 0, 5, true, 4);
                    }
                case "CarpenterFormATMImprovements2":
                    {
                        return new Form_Builder(carpenterATMimprovtexture, 0, 6, true, 4);
                    }
                case "CarpenterShopATM":
                    {
                        var shop = new ShopMenu(Utility.getCarpenterStock(), 0)
                        {
                            portraitPerson = Game1.getCharacterFromName("nmATM"),
                            potraitPersonDialogue = Game1.parseText(ModEntry.ModHelper.Translation.Get("NPCDataATMQuote"), Game1.dialogueFont, 304)

                        };
                        return shop;
                    }
                case "CarpenterShopRobin":
                    {
                        return new ShopMenu(Utility.getCarpenterStock(), 0, "Robin");
                    }
                case "CarpenterMenuVanilla":
                    {
                        warpingShop = true;
                        return new CarpenterMenu();
                    }
                case "CarpenterMenuMainFarm":
                    {
                        warpingShop = true;
                        return new Carpenter_Builder(0);
                    }
                case "CarpenterMenuQuarry":
                    {
                        warpingShop = true;
                        return new Carpenter_Builder(1);
                    }
                case "CarpenterMenuPlantation":
                    {
                        warpingShop = true;
                        return new Carpenter_Builder(2);
                    }
                case "CarpenterMenuLakeLand":
                    {
                        warpingShop = true;
                        return new Carpenter_Builder(3);
                    }
            }
            return null;
        }
        
        public void createQuestionDialogue(string checking, bool noload)
        {            
            List<Response> options = new();
            switch (checking)
            {
                case "JojaATM":
                    {
                        options.Add(new Response("JojaStock", ModEntry.ModHelper.Translation.Get("QuestionDialoguesChoicesShop")));
                        if (noload is false)
                        {
                            options.Add(new Response("JojaFormATM", ModEntry.ModHelper.Translation.Get("QuestionDialoguesChoicesJojaATMFormMenu")));
                        }                       
                        options.Add(new Response("Leave", ModEntry.ModHelper.Translation.Get("QuestionDialoguesNoChoice")));
                        Game1.currentLocation.createQuestionDialogue(ModEntry.ModHelper.Translation.Get("QuestionDialoguesTile"), options.ToArray(), new GameLocation.afterQuestionBehavior(CreateQuestionDialogue_output));
                        break;
                    }
                case "JojaNPC":
                    {
                        options.Add(new Response("JojaNPCCashier", ModEntry.ModHelper.Translation.Get("QuestionDialoguesChoicesShop")));
                        options.Add(new Response("Leave", ModEntry.ModHelper.Translation.Get("QuestionDialoguesNoChoice")));
                        Game1.currentLocation.createQuestionDialogue(ModEntry.ModHelper.Translation.Get("QuestionDialoguesTile"), options.ToArray(), new GameLocation.afterQuestionBehavior(CreateQuestionDialogue_output));
                        break;
                    }
                case "CarpenterATM":
                    {
                        options.Add(new Response("CarpenterShopATM", ModEntry.ModHelper.Translation.Get("QuestionDialoguesChoicesShop")));
                        options.Add(new Response("CarpenterFormATM", ModEntry.ModHelper.Translation.Get("QuestionDialoguesBuilderForm")));
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
                case "JojaStock":
                    {
                        callMenu("JojaStock");
                        break;
                    }
                case "JojaFormATM":
                    {
                        callMenu("JojaFormATM");
                        break;
                    }
                case "JojaNPCCashier":
                    {
                        callMenu("JojaNPCCashier");
                        break;
                    }
                case "CarpenterFormATM":
                    {
                        callMenu("CarpenterFormATM");
                        break;
                    }
                case "CarpenterShopATM":
                    {
                        callMenu("CarpenterShopATM");
                        break;
                    }
            }
        }
        public static NMFarm1 QuarryLand()
        {
            return Game1.getLocationFromName("NMFarm1") as NMFarm1;
        }
        public static NMFarm2 PlantationLand()
        {
            return Game1.getLocationFromName("NMFarm2") as NMFarm2;
        }
        public static NMFarm2 LakeLand()
        {
            return Game1.getLocationFromName("NMFarm3") as NMFarm2;
        }
    }
}
