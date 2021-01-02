/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

using System.Collections.Generic;
using System.IO;

using BCC.Menus;
using BCC.Utilities;

using xTile.Tiles;
using xTile.Layers;

namespace BCC
{
    public class BCC : Mod
    {
        //Variables & Constants
        public GameLocation CommunityCenter;

        public Vector2 CCKitchen1 = new Vector2((int)5, (int)4);
        public Vector2 CCKitchen2 = new Vector2((int)6, (int)4);
        public Vector2 CCKitchen3 = new Vector2((int)7, (int)4);

        public Vector2 CCFridge1 = new Vector2((int)8, (int)4);

        public Vector2 CCPantry1 = new Vector2((int)13, (int)4);
        public Vector2 CCPantry2 = new Vector2((int)14, (int)4);
        public Vector2 CCPantry3 = new Vector2((int)15, (int)4);

        public Vector2 CCDonation1 = new Vector2((int)13, (int)10);

        public Vector2 CCDye1 = new Vector2((int)19, (int)19);
        public Vector2 CCDye2 = new Vector2((int)19, (int)20);

        public Vector2 CCTailor1 = new Vector2((int)9, (int)27);
        public Vector2 CCTailor2 = new Vector2((int)10, (int)27);

        public Vector2 CCVault1 = new Vector2((int)54, (int)5);
        public Vector2 CCVault2 = new Vector2((int)55, (int)5);
        public Vector2 CCVault3 = new Vector2((int)56, (int)5);
        public Vector2 CCVault4 = new Vector2((int)57, (int)5);

        public Vector2 CCBulletin1 = new Vector2((int)45, (int)12);
        public Vector2 CCBulletin2 = new Vector2((int)46, (int)12);
        public Vector2 CCBulletin3 = new Vector2((int)47, (int)12);

        public Vector2 CCBoiler1 = new Vector2((int)62, (int)13);
        public Vector2 CCBoiler2 = new Vector2((int)63, (int)13);

        public Vector2 CCBoilerCoal1 = new Vector2((int)64, (int)13);
        public Vector2 CCBoilerCoal2 = new Vector2((int)65, (int)13);
        public Vector2 CCBoilerCoal3 = new Vector2((int)66, (int)14);
        public Vector2 CCBoilerCoal4 = new Vector2((int)66, (int)15);
        public Vector2 CCBoilerCoal5 = new Vector2((int)64, (int)20);
        public Vector2 CCBoilerCoal6 = new Vector2((int)65, (int)20);

        public Vector2 OOBCCFridge1 = new Vector2((int)7400, (int)7400);
        public Vector2 OOBCCFridge2 = new Vector2((int)7401, (int)7400);
        public Vector2 OOBCCFridge3 = new Vector2((int)7402, (int)7400);
        public Vector2 OOBCCFridge4 = new Vector2((int)7403, (int)7400);
        public Vector2 OOBCCFridge5 = new Vector2((int)7404, (int)7400);
        public Vector2 OOBCCCoalChest = new Vector2((int)7405, (int)7400);

        public static Chest CCFridge1Chest;
        public static Chest CCFridge2Chest;
        public static Chest CCFridge3Chest;
        public static Chest CCFridge4Chest;
        public static Chest CCFridge5Chest;
        public static Chest CCCoalChest;
        public List<Chest> CCKitchenChests = new List<Chest>();

        public static string TodaysDonations;
        public string YesterdaysDonations;
        public string Season;
        public string PreviousSeason;
        public string ModFolderPath;

        public TileSheet customTileSheet;
        public string sheetPath;

        public Object itemFromNPC;

        public bool hasBeenAdded = false;
        public bool DonationsAddedForToday = false;
        public bool hasCPCCUpdate = false;

        public static IModHelper RequestableHelper; 

        //SMAPI Reference
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.GameLoop.SaveCreated += GameLoop_SaveCreated;
            RequestableHelper = helper;

            Util util = new Util(Helper, Monitor);

            sheetPath = Helper.Content.GetActualAssetKey("Assets/Textures/customTiles.png", ContentSource.ModFolder);
            ModFolderPath = Path.GetFullPath(Path.Combine(Helper.DirectoryPath, @"..\"));
            hasCPCCUpdate = helper.ModRegistry.IsLoaded("LemurKat.CommunityCenter.CP");
        }

        //Events
        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            if (CommunityCenter == null)
            {
                if (Game1.player.hasCompletedCommunityCenter() && Game1.player.eventsSeen.Contains(191393))
                {
                    CommunityCenter = Game1.getLocationFromName("CommunityCenter");
                    Helper.Events.Input.ButtonPressed += Input_ButtonPressed;
                    Helper.Events.Display.RenderingActiveMenu += Display_RenderingActiveMenu;
                    Helper.Events.GameLoop.DayEnding += GameLoop_DayEnding;
                    Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
                    Helper.Events.GameLoop.Saving += GameLoop_Saving;
                    Helper.Events.GameLoop.ReturnedToTitle += GameLoop_ReturnedToTitle;
                    Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked_Main;
                    DonationsAddedForToday = false;
                    if (Game1.Date.Season != Season)
                        PreviousSeason = Season;
                }
                else return;
            }
            else
                DonationsAddedForToday = false;
            Season = Game1.Date.Season;
            if (Util.Stored == null)
            {
                Util.Stored = Helper.Data.ReadSaveData<VaultGold>("MindMeltMax.BCC");
                if (Util.Stored == null)
                    Util.Stored = new VaultGold();
            }
            if(Requests.RequestList == null)
            {
                Requests.RequestList = new List<Request>();
                for(int i=0; i<3; i++)
                {
                    var ReaderData = Helper.Data.ReadSaveData<Request>("MindMeltMax.BCC" + i.ToString());
                    if (ReaderData == null)
                        break;
                    var newRequest = new Request(ReaderData.itemIndex, ReaderData.itemCount, ReaderData.totalPrice, ReaderData.CreationDate);
                    Requests.RequestList.Add(newRequest);
                }
                if (Requests.RequestList == null)
                    Requests.RequestList = new List<Request>();
            }
            Monitor.Log($"List : {BoilerData.dataList}", LogLevel.Debug);
            if (BoilerData.dataList == null)
            {
                BoilerData.dataList = new List<data>();
                for (int i = 0; i < 6; i++)
                {
                    var data = Helper.Data.ReadSaveData<data>("MindMeltMax.BCCX" + i.ToString());
                    if (data == null)
                        break;
                    var tempData = new data(data.ParentSheetIndex, data.Stack, data.HoldingComponentID);
                    BoilerData.dataList.Add(tempData);
                }
                if (BoilerData.dataList == null)
                    BoilerData.dataList = new List<data>();
            }

            #region Request
            int Days = Game1.Date.TotalDays;
            Util.removableRequests = new List<Request>();
            foreach(Request r in Requests.RequestList)
            {
                if (Days - r.CreationDate > 7)
                    return;
                else
                {
                    double r1 = Game1.random.NextDouble();
                    if (r1 < 0.2 + Game1.player.DailyLuck)
                    { 
                        Util.CompleteRequest(r.itemIndex);
                        Util.removableRequests.Add(r);
                        break;
                    }
                    else
                        return;
                }
            }
            #endregion Request
        }

        private void GameLoop_UpdateTicked_Main(object sender, UpdateTickedEventArgs e)
        {
            if(BoilerMenu.isSmelting)
            {
                Game1.currentLightSources.Add(new LightSource(LightSource.sconceLight, CCBoiler1, 2f));
                Game1.currentLightSources.Add(new LightSource(LightSource.sconceLight, CCBoiler2, 2f));
            }
        }

        private void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            ResetEverything();
        }

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.CanPlayerMove)
                return;
            if(Game1.currentLocation.Name == CommunityCenter.Name)
            {
                if (CommunityCenter.doesTileHaveProperty((int)CCDonation1.X, (int)CCDonation1.Y, "Action", "Buildings") == null)
                {
                    SetTilePropertiesCC();
                    setCustomTiles();
                    Helper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked;
                }
            }
        }

        private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
            YesterdaysDonations = TodaysDonations;
            Util.previousName = null;
        }

        private void Display_RenderingActiveMenu(object sender, RenderingActiveMenuEventArgs e)
        {
        }

        private void GameLoop_SaveCreated(object sender, SaveCreatedEventArgs e)
        {
            CCFridge1Chest = new Chest(true, OOBCCFridge1);
            CCFridge1Chest.fridge.Value = true;
            CCFridge1Chest.type.Value = "Crafting";
            CCFridge1Chest.Name = "Store1";
            CCKitchenChests.Add(CCFridge1Chest);

            CCFridge2Chest = new Chest(true, OOBCCFridge2);
            CCFridge2Chest.fridge.Value = true;
            CCFridge2Chest.Name = "Store2";
            CCKitchenChests.Add(CCFridge2Chest);

            CCFridge3Chest = new Chest(true, OOBCCFridge3);
            CCFridge3Chest.fridge.Value = true;
            CCFridge3Chest.Name = "Store3";
            CCKitchenChests.Add(CCFridge3Chest);

            CCFridge4Chest = new Chest(true, OOBCCFridge4);
            CCFridge4Chest.fridge.Value = true;
            CCFridge4Chest.Name = "Store4";
            CCKitchenChests.Add(CCFridge4Chest);

            CCFridge5Chest = new Chest(true, OOBCCFridge5);
            CCFridge5Chest.fridge.Value = true;
            CCFridge5Chest.Name = "Store5";
            CCKitchenChests.Add(CCFridge5Chest);

            CCCoalChest = new Chest(true, OOBCCCoalChest);
            CCCoalChest.name = "StoreCoal";
        }

        private void GameLoop_Saving(object sender, SavingEventArgs e)
        {
            if (!Game1.IsMasterGame)
                return;
            Helper.Data.WriteSaveData("MindMeltMax.BCC", Util.Stored);
            for (int i = 0; i < BoilerData.dataList.Count; i++)
            {
                Helper.Data.WriteSaveData("MindMeltMax.BCCX" + i.ToString(), BoilerData.dataList[i]);
            }
            for(int i = 0; i<Requests.RequestList.Count; i++)
            {
                Helper.Data.WriteSaveData("MindMeltMax.BCC" + i.ToString(), Requests.RequestList[i]);
            }
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.CanPlayerMove)
                return;
            GameLocation currentLocation = Game1.currentLocation;

            Vector2 posUnderMouse = e.Cursor.GrabTile;
            Vector2 CenterScreen = Utility.getTopLeftPositionForCenteringOnScreen(800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2);

            string property = currentLocation.doesTileHaveProperty((int)posUnderMouse.X, (int)posUnderMouse.Y, "Action", "Buildings");


            checkChestsInCC();
            addFridgesToListAfterCheck();
            if (CCKitchenChests.Count == 5)
                checkForDonations();

            if (e.Button != SButton.MouseRight)
                return;
            else
            {
                if (property == null)
                    return;

                #region Kitchen

                else if (property == "CCKitchen")
                {
                    if (TodaysDonations == null)
                        checkForDonations();
                    Game1.activeClickableMenu = (IClickableMenu)new CraftingPage((int)CenterScreen.X, (int)CenterScreen.Y, 800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, true, true, CCKitchenChests);
                }
                else if (property == "CCPantry")
                {
                    if (TodaysDonations == null)
                        checkForDonations();

                    Game1.activeClickableMenu = (IClickableMenu)new PantryMenu(CCKitchenChests, Helper, Monitor);
                }
                else if (property == "CCFridge")
                {
                    if (CCFridge1Chest != null)
                    {
                        if (TodaysDonations == null)
                            checkForDonations();
                        Game1.activeClickableMenu = (IClickableMenu)new DialogueBox("The fridge is stocked with items from the towns people");
                        /*Game1.activeClickableMenu = (IClickableMenu)new ItemGrabMenu((IList<Item>)CCFridge1Chest.items, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems),
                                                    new ItemGrabMenu.behaviorOnItemSelect(CCFridge1Chest.grabItemFromInventory), (string)null, new ItemGrabMenu.behaviorOnItemSelect(CCFridge1Chest.grabItemFromChest),
                                                    false, true, true, true, true, 1, sourceItem: ((bool)(NetFieldBase<bool, NetBool>)CCFridge1Chest.fridge ? (Item)null : (Item)CCFridge1Chest), context: ((object)CCFridge1Chest));*/
                        //Util.openFridge(Util.CurrentFridgeTab);
                        //Util.addFridgeSelectionButtons();
                        //Monitor.Log($"Fridge1 : {CCFridge1Chest.items.Count} | Fridge2 : {CCFridge2Chest.items.Count} | Fridge3 : {CCFridge3Chest.items.Count} | Fridge4 : {CCFridge4Chest.items.Count} | Fridge5 : {CCFridge5Chest.items.Count} |", LogLevel.Debug);
                    }
                }
                else if (property == "CCDonations")
                {
                    if (TodaysDonations == null)
                        checkForDonations();
                    if (Season == Game1.Date.Season && YesterdaysDonations == null || Season != Game1.Date.Season && YesterdaysDonations == null)
                        Game1.activeClickableMenu = new PantryDonationSheet($"{Game1.Date.Season} : {Game1.Date.DayOfMonth} ^" + TodaysDonations, "Pantry Donations", Monitor, Helper);
                    else if (Season == Game1.Date.Season && YesterdaysDonations != null)
                        Game1.activeClickableMenu = new PantryDonationSheet($"{Game1.Date.Season} : {Game1.Date.DayOfMonth} ^" + TodaysDonations + $"^{Game1.Date.Season} : {Game1.Date.DayOfMonth - 1} ^" + YesterdaysDonations, "Pantry Donations", Monitor, Helper);
                    else if (Season != Game1.Date.Season && YesterdaysDonations != null)
                        Game1.activeClickableMenu = new PantryDonationSheet($"{Game1.Date.Season} : {Game1.Date.DayOfMonth} ^" + TodaysDonations + $"^{PreviousSeason} : {28} ^" + YesterdaysDonations, "Pantry Donations", Monitor, Helper);
                }

                #endregion Kitchen


                #region Crafts

                else if (property == "CCDye")
                {
                    if (TodaysDonations == null)
                        checkForDonations();
                    Game1.activeClickableMenu = (IClickableMenu)new DyeMenu();
                }
                else if (property == "CCTailor")
                {
                    if (TodaysDonations == null)
                        checkForDonations();
                    Game1.activeClickableMenu = (IClickableMenu)new TailoringMenu();
                }

                #endregion Crafts


                #region Vault

                else if (property == "CCVault")
                {
                    if (TodaysDonations == null)
                        checkForDonations();
                    Util.openVault();
                }

                #endregion Vault


                #region Bulletin

                else if (property == "CCBulletin")
                {
                    if (TodaysDonations == null)
                        checkForDonations();
                    Game1.activeClickableMenu = (IClickableMenu)new ItemRequestBoard(Monitor);
                }
                else if (hasCPCCUpdate && property == "Message \"CCBulletinBoard."+Game1.Date.DayOfMonth.ToString()+"\"")
                {
                    Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked1;
                }

                #endregion Bulletin


                #region Boiler

                else if(property == "CCBoilerCoal")
                {
                    if (TodaysDonations == null)
                        checkForDonations();
                    Game1.activeClickableMenu = (IClickableMenu)new BoilerCoalMenu(CCCoalChest, Monitor, Helper);
                    //Game1.activeClickableMenu = (IClickableMenu)new DialogueBox("Under construction, please check back later");
                }

                else if(property == "CCBoiler")
                {
                    if (TodaysDonations == null)
                        checkForDonations();
                    //Game1.activeClickableMenu = (IClickableMenu)new DialogueBox("Under construction, please check back later");
                    Game1.activeClickableMenu = (IClickableMenu)new BoilerMenu(Monitor, Helper, CCCoalChest);
                    /*Game1.activeClickableMenu = (IClickableMenu)new ItemGrabMenu((IList<Item>)CCCoalChest.items, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems),
                                                    new ItemGrabMenu.behaviorOnItemSelect(CCCoalChest.grabItemFromInventory), (string)null, new ItemGrabMenu.behaviorOnItemSelect(CCCoalChest.grabItemFromChest),
                                                    false, true, true, true, true, 1, sourceItem: ((bool)(NetFieldBase<bool, NetBool>)CCCoalChest.fridge ? (Item)null : (Item)CCCoalChest), context: ((object)CCCoalChest));*/
                }

                #endregion Boiler
            }
        }

        private void GameLoop_UpdateTicked1(object sender, UpdateTickedEventArgs e)
        {
            if (Game1.activeClickableMenu == null)
            {
                Game1.activeClickableMenu = (IClickableMenu)new ItemRequestBoard(Monitor);
                Helper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked1;
            }
        }

        //Custom Functions
        private void ResetEverything()
        {
            //Smapi Events
            Helper.Events.Input.ButtonPressed -= Input_ButtonPressed;
            Helper.Events.Display.RenderingActiveMenu -= Display_RenderingActiveMenu;
            Helper.Events.GameLoop.DayEnding -= GameLoop_DayEnding;
            Helper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked;
            Helper.Events.GameLoop.Saving -= GameLoop_Saving;
            Helper.Events.GameLoop.DayStarted -= GameLoop_DayStarted;
            Helper.Events.GameLoop.SaveCreated -= GameLoop_SaveCreated;
            Helper.Events.GameLoop.ReturnedToTitle -= GameLoop_ReturnedToTitle;
            Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;

            CommunityCenter = null;
            customTileSheet = null;
            TodaysDonations = null;
            DonationsAddedForToday = false;
        }

        private void setCustomTiles()
        {
            CommunityCenter.map.LoadTileSheets(Game1.mapDisplayDevice);
            TileSheet sheet = CommunityCenter.map.GetTileSheet("indoors");

            int DonationsX = (int)CCDonation1.X;
            int DonationsY = (int)CCDonation1.Y;
            Layer DonationsLayer = CommunityCenter.map.GetLayer("Front");
            //DonationsLayer.Tiles[DonationsX, DonationsY] = new StaticTile(DonationsLayer, sheet, BlendMode.Alpha, 1);
            DonationsLayer.Tiles[DonationsX, DonationsY] = new StaticTile(DonationsLayer, sheet, BlendMode.Alpha, 1206);
        }

        private void SetTilePropertiesCC()
        {
            //Kitchen
            CommunityCenter.setTileProperty((int)CCKitchen1.X, (int)CCKitchen1.Y, "Buildings", "Action", "CCKitchen");
            CommunityCenter.setTileProperty((int)CCKitchen2.X, (int)CCKitchen2.Y, "Buildings", "Action", "CCKitchen");
            CommunityCenter.setTileProperty((int)CCKitchen3.X, (int)CCKitchen3.Y, "Buildings", "Action", "CCKitchen");

            //Fridge
            CommunityCenter.setTileProperty((int)CCFridge1.X, (int)CCFridge1.Y, "Buildings", "Action", "CCFridge");

            //Donations
            CommunityCenter.setTileProperty((int)CCDonation1.X, (int)CCDonation1.Y+1, "Buildings", "Action", "CCDonations");

            //Pantry
            CommunityCenter.setTileProperty((int)CCPantry1.X, (int)CCPantry1.Y, "Buildings", "Action", "CCPantry");
            CommunityCenter.setTileProperty((int)CCPantry2.X, (int)CCPantry2.Y, "Buildings", "Action", "CCPantry");
            CommunityCenter.setTileProperty((int)CCPantry3.X, (int)CCPantry3.Y, "Buildings", "Action", "CCPantry");

            //Dye
            CommunityCenter.setTileProperty((int)CCDye1.X, (int)CCDye1.Y, "Buildings", "Action", "CCDye");
            CommunityCenter.setTileProperty((int)CCDye2.X, (int)CCDye2.Y, "Buildings", "Action", "CCDye");

            //Tailor
            CommunityCenter.setTileProperty((int)CCTailor1.X, (int)CCTailor1.Y, "Buildings", "Action", "CCTailor");
            CommunityCenter.setTileProperty((int)CCTailor2.X, (int)CCTailor2.Y, "Buildings", "Action", "CCTailor");

            //Vault
            CommunityCenter.setTileProperty((int)CCVault1.X, (int)CCVault1.Y, "Buildings", "Action", "CCVault");
            CommunityCenter.setTileProperty((int)CCVault2.X, (int)CCVault2.Y, "Buildings", "Action", "CCVault");
            CommunityCenter.setTileProperty((int)CCVault3.X, (int)CCVault3.Y, "Buildings", "Action", "CCVault");
            CommunityCenter.setTileProperty((int)CCVault4.X, (int)CCVault4.Y, "Buildings", "Action", "CCVault");

            //Boiler
            CommunityCenter.setTileProperty((int)CCBoilerCoal1.X, (int)CCBoilerCoal1.Y, "Buildings", "Action", "CCBoilerCoal");
            CommunityCenter.setTileProperty((int)CCBoilerCoal2.X, (int)CCBoilerCoal2.Y, "Buildings", "Action", "CCBoilerCoal");
            CommunityCenter.setTileProperty((int)CCBoilerCoal3.X, (int)CCBoilerCoal3.Y, "Buildings", "Action", "CCBoilerCoal");
            CommunityCenter.setTileProperty((int)CCBoilerCoal4.X, (int)CCBoilerCoal4.Y, "Buildings", "Action", "CCBoilerCoal");
            CommunityCenter.setTileProperty((int)CCBoilerCoal5.X, (int)CCBoilerCoal5.Y, "Buildings", "Action", "CCBoilerCoal");
            CommunityCenter.setTileProperty((int)CCBoilerCoal6.X, (int)CCBoilerCoal6.Y, "Buildings", "Action", "CCBoilerCoal");
            CommunityCenter.setTileProperty((int)CCBoiler1.X, (int)CCBoiler1.Y, "Buildings", "Action", "CCBoiler");
            CommunityCenter.setTileProperty((int)CCBoiler2.X, (int)CCBoiler2.Y, "Buildings", "Action", "CCBoiler");

            if (!hasCPCCUpdate)
            {
                //Bulletin
                CommunityCenter.setTileProperty((int)CCBulletin1.X, (int)CCBulletin1.Y, "Buildings", "Action", "CCBulletin");
                CommunityCenter.setTileProperty((int)CCBulletin2.X, (int)CCBulletin2.Y, "Buildings", "Action", "CCBulletin");
                CommunityCenter.setTileProperty((int)CCBulletin3.X, (int)CCBulletin3.Y, "Buildings", "Action", "CCBulletin");
            }
        }
        private void checkChestsInCC()
        {
            if (CommunityCenter.objects.ContainsKey(OOBCCFridge1))
            {
                CCFridge1Chest = CommunityCenter.objects[OOBCCFridge1] as Chest ?? new Chest(true, OOBCCFridge1);
                CCFridge1Chest.fridge.Value = true;
            }
            else
                CommunityCenter.objects.Add(OOBCCFridge1, new Chest(true, OOBCCFridge1));

            if (CommunityCenter.objects.ContainsKey(OOBCCFridge2))
            {
                CCFridge2Chest = CommunityCenter.objects[OOBCCFridge2] as Chest ?? new Chest(true, OOBCCFridge2);
                CCFridge2Chest.fridge.Value = true;
            }
            else
                CommunityCenter.objects.Add(OOBCCFridge2, new Chest(true, OOBCCFridge2));

            if (CommunityCenter.objects.ContainsKey(OOBCCFridge3))
            {
                CCFridge3Chest = CommunityCenter.objects[OOBCCFridge3] as Chest ?? new Chest(true, OOBCCFridge3);
                CCFridge3Chest.fridge.Value = true;
            }
            else
                CommunityCenter.objects.Add(OOBCCFridge3, new Chest(true, OOBCCFridge3));

            if (CommunityCenter.objects.ContainsKey(OOBCCFridge4))
            {
                CCFridge4Chest = CommunityCenter.objects[OOBCCFridge4] as Chest ?? new Chest(true, OOBCCFridge4);
                CCFridge4Chest.fridge.Value = true;
            }
            else
                CommunityCenter.objects.Add(OOBCCFridge4, new Chest(true, OOBCCFridge4)); 

            if (CommunityCenter.objects.ContainsKey(OOBCCFridge5))
            {
                CCFridge5Chest = CommunityCenter.objects[OOBCCFridge5] as Chest ?? new Chest(true, OOBCCFridge5);
                CCFridge5Chest.fridge.Value = true;
            }
            else
                CommunityCenter.objects.Add(OOBCCFridge5, new Chest(true, OOBCCFridge5));

            if (CommunityCenter.objects.ContainsKey(OOBCCCoalChest))
                CCCoalChest = CommunityCenter.objects[OOBCCCoalChest] as Chest ?? new Chest(true, OOBCCCoalChest);
            else
                CommunityCenter.objects.Add(OOBCCCoalChest, new Chest(true, OOBCCCoalChest));
        }
        private void addFridgesToListAfterCheck()
        {
            if(CCFridge1Chest != null && CCFridge2Chest != null && CCFridge3Chest != null && CCFridge4Chest != null && CCFridge5Chest != null)
            {
                if (CCKitchenChests.Count == 0)
                {
                    CCKitchenChests.Add(CCFridge1Chest);
                    CCKitchenChests.Add(CCFridge2Chest);
                    CCKitchenChests.Add(CCFridge3Chest);
                    CCKitchenChests.Add(CCFridge4Chest);
                    CCKitchenChests.Add(CCFridge5Chest);
                }
                else
                    return;
            }
            else
                return;
        }
        private void checkForDonations()
        {
            if (!DonationsAddedForToday)
            {
                int RndmCount = Game1.random.Next(0, 4); //Max amount of donations
                Util.getNpcsForDonation(RndmCount);
                TodaysDonations = null; //Make sure Donations don't overlap
                itemFromNPC = null;
                if (Util.NPCnamesToday.Count != 0)
                {
                    foreach (string name in Util.NPCnamesToday)
                    {
                        int RndmCount2 = Game1.random.Next(1, 4);
                        foreach (NPC person in Util.allNPCS)
                        {
                            if (person.Name == name)
                            {
                                itemFromNPC = Util.getRandomItemForDonation(person);
                                if (itemFromNPC == null)
                                    itemFromNPC = Util.getRandomItemForDonation(person); //Try To Avoid Nulls
                                for (int f = 0; f < RndmCount2; f++)
                                {
                                    hasBeenAdded = false;
                                    foreach (Chest chest in CCKitchenChests)
                                    {
                                        if (!hasBeenAdded)
                                        {
                                            if (Chest.capacity + 1 != chest.items.Count || Util.DoesChestHaveItem(itemFromNPC))
                                            {
                                                var oneItem = itemFromNPC == null ? Util.getRandomItemForDonation(person).getOne() : itemFromNPC.getOne(); // Avoid Nulls After Inevitable Failure -_-
                                                chest.addItem(oneItem);
                                                hasBeenAdded = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        TodaysDonations += $"{name} : {itemFromNPC.Name} X {RndmCount2}, ^";
                    }
                }
                else
                    TodaysDonations += "No donations today, ^";
                DonationsAddedForToday = true;
            }
        }
    }
}