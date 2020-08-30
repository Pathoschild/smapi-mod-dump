// Deprecated, will be removed in future commit

//#define OLD_CODE

#if OLD_CODE

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Locations;
using StardewModdingAPI.Events;
using Netcode;


namespace ExpandedFridge
{
    /// <summary>
    /// This class represents a special fridge chest that acts as a hub for connected chests. 
    /// 
    /// We do this by extending the Chest class and removing the limit on how many items that can be inserted to its inventory.
    /// Then we fill its inventory with references to items from the chests connected to it and only display a hub for accessing those chests.
    /// 
    /// NOTE OF WARNING: Accessing the actuall inventory of this chest normally and inserting / grabbing items could 
    /// destroy the items or break the game. DO-NOT-DO-IT!
    /// 
    /// NOTE: 
    /// Code marked with OVERT usually is in same location as code that functions 
    /// more or less the same as some source code for StardewValley. We use OVERT
    /// to mark functions or parts of code that has differences from the source.
    /// 
    /// </summary>
    class ExpandedFridgeHub : StardewValley.Objects.Chest
    {
        /// Container for both a chest and its position.
        public class ChestAndPosition
        {
            public Chest c;
            public int x, y;
        }



        /// **************************************************************************************************************************************************
        /// VARIABLES and REFERENCES


        /// The normal fridge of the location.
        private Chest normalFridge;

        /// The current selected chest to display when accessing this chest.
        public Chest selectedChest { get; private set; }

        /// A list of special marked connected chests.
        public List<Chest> connectedChests { get; private set; }

        /// The location of the fridge, this is the Cabin or FarmHouse owned by the player.
        private FarmHouse location;

        /// For rendering the remote access button.
        private ClickableTextureComponent remoteButton;

        /// For rendering the remote access button.
        private ClickableTextureComponent remoteButtonTab;


        /// Are the references in the infinite storage set to the contents of the connected chests.
        /// NOTE: We should never setup references without clearing the references first.
        /// NOTE: Also its good to clear the references before we access the connected chests.
        public bool         referencesSetup { get; private set; }

        /// Is the chest opened remotely
        public bool         remoteOpen { get; private set; }

        /// Should we try to draw the remote button.
        private bool        remoteButtonDraw = false;

        /// Optional player check if remote button should be active.
        private bool        remoteButtonActive = true;


        /// OVERT from Chest: we use our own lid frame variable for more controll of chest opening and closing.
        private int         currentLidFrame = 0;

        /// The index of the current selected chest.
        private int         selectedChestIndex = 0;

        /// Current upgraded storage.
        private int         upgradeStorage = 0;

        /// Current upgraded modules.
        private string      upgradeModules = "";


        /// Upgrade codes for the frige.
        public const char   upgradeID = 'I';
        public const char   upgradeLim = 'L';
        public const char   upgradeWarp = 'W';
        public const char   upgradePortal = 'O';
        public const char   upgradeDimension = 'D';

        /// The special mark of chests that should be connected to this.
        public const string MAGIC = "FCHubM12";



        /// **************************************************************************************************************************************************
        /// ROUTINE METHODS


        /// The constructor initiates everything we need to get the fridge running.
        public ExpandedFridgeHub() : base(true)
        {
            // get the location of the fridge
            if (Game1.IsMasterGame)
                this.location = Game1.getLocationFromName("FarmHouse") as FarmHouse;
            else
            {
                foreach (GameLocation location in ModEntry.HelperInstance.Multiplayer.GetActiveLocations())
                {
                    if (location is Cabin && (location as Cabin).owner == Game1.player)
                        this.location = location as FarmHouse;
                }
            }
            
            // check if location can have a fridge
            if (this.location.upgradeLevel > 0)
            {
                this.remoteOpen = false;
                this.connectedChests = new List<Chest>();

                // save normal fridge
                this.normalFridge = this.location.fridge.Value;
                

                // replace normal fridge in location with this
                this.location.fridge.Value = this;

                // setup fridge values from saved data in normal fridge
                SetupUpgradesFromData(this.normalFridge.chestType.Value);

                // get marked chests in our location
                List<ChestAndPosition> markedChests = GetAllMarkedChestsInLocation();

                // determine how many marked chests there should be
                int targetChests = this.location.upgradeLevel * 2 + this.upgradeStorage + ModEntry.cheatStorage;
                if (targetChests > 12)
                    targetChests = 12;

                // ensure we have enough marked chests according to house level
                if (markedChests.Count < targetChests)
                {
                    for (int i = markedChests.Count; i < targetChests; i++)
                        CreateNewChestInLocation(i);

                    markedChests = GetAllMarkedChestsInLocation();
                }

                // set connected chests
                foreach (ChestAndPosition cap in markedChests)
                    connectedChests.Add(cap.c);

                // remove the marked chests from our location
                ClearChestsInLocation();

                // we make sure the order of the connected chests are in order of their chest type strings
                this.connectedChests.Sort((a, b) => (int.Parse( a.chestType.Value.Split(':')[1]).CompareTo( int.Parse(b.chestType.Value.Split(':')[1]) )));

                // set selected chest
                this.selectedChest = this.connectedChests[0];

                // setup button clickable texture
                const int fbscale = 2;
                this.remoteButton = new ClickableTextureComponent(
                    new Rectangle(0, 0, ModEntry.FridgeTexture.Width*fbscale, ModEntry.FridgeTexture.Height * fbscale),
                    ModEntry.FridgeTexture,
                    new Rectangle(0, 0, ModEntry.FridgeTexture.Width, ModEntry.FridgeTexture.Height),
                    fbscale,
                    false);

                // setup button clickable texture
                this.remoteButtonTab = new ClickableTextureComponent(
                    new Rectangle(0, 0, 64, 64),
                    Game1.mouseCursors,
                    new Rectangle(16, 368, 16, 16),
                    4f,
                    false);

                // setup references in infinite storage
                FillReferences();

                // setup callbacks for expanded fridge
                SetupEventCallbacks();
            }
        }

        /// Call this before we release the fridge object to garbage collection. The data returned should be saved somewhere and returned next fridge instance.
        public void CleanupForRelease()
        {
            if (this.location.upgradeLevel > 0)
            {
                FillChestsInLocation();
                ClearReferences();
                RemoveEventCallbacks();
                this.normalFridge.chestType.Value = ParseUpgradeDataToString();
                this.location.fridge.Value = this.normalFridge;
            }
        }


        /// Creates a menu that accesses the connected chests of this fridge. (only call this inside this class or from the expanded fridge menu)
        public IClickableMenu CreateMenu()
        {
            return new ExpandedFridgeMenu(this);
        }


        /// Setup event callbacks for the expanded fridge.
        private void SetupEventCallbacks()
        {
            ModEntry.HelperInstance.Events.GameLoop.OneSecondUpdateTicking += this.RemoteFridgeUpdate;
            ModEntry.HelperInstance.Events.Display.MenuChanged += this.RemoteFridgeOnMenuChanged;
            ModEntry.HelperInstance.Events.Display.RenderedActiveMenu += this.RemoteFridgeOnRenderedMenu;
            ModEntry.HelperInstance.Events.Input.ButtonPressed += this.RemoteFridgeOnButtonPressed;
        }

        /// Removes event callbacks for the expanded fridge.
        private void RemoveEventCallbacks()
        {
            ModEntry.HelperInstance.Events.GameLoop.OneSecondUpdateTicking -= this.RemoteFridgeUpdate;
            ModEntry.HelperInstance.Events.Display.MenuChanged -= this.RemoteFridgeOnMenuChanged;
            ModEntry.HelperInstance.Events.Display.RenderedActiveMenu -= this.RemoteFridgeOnRenderedMenu;
            ModEntry.HelperInstance.Events.Input.ButtonPressed -= this.RemoteFridgeOnButtonPressed;
        }


        /// Sets internal upgrade variables variables from data.
        private void SetupUpgradesFromData(string data)
        {
            this.upgradeModules = "";

            if (!data.Contains(":"))
                return;

            string[] s = data.Split(':');

            if (s.Length > 0)
            {
                this.upgradeStorage = int.Parse(s[0]);

                for (int i = 1; i < s.Length; i++)
                    this.upgradeModules += s[i];
            }
        }

        /// Parses the current upgrade variables and saves them to a string.
        private string ParseUpgradeDataToString()
        {
            // format of string is : "0:A:B:C:D" were first is number of storage upgrades following by upgrade codes in any order

            string data = this.upgradeStorage.ToString() + ":";

            foreach (char c in this.upgradeModules)
                data += c + ":";

            //Game1.showGlobalMessage(data);

            return data;
        }



        /// **************************************************************************************************************************************************
        /// PUBLIC METHODS


        /// Tries to set the selected chest by its index. Returns false if index was not accepted.
        public bool SetSelectedChest(int index)
        {
            if (index >= 0 && index < this.connectedChests.Count)
            {
                this.selectedChestIndex = index;
                this.selectedChest = this.connectedChests[index];
                return true;
            }
            return false;
        }

        /// Tries to 'scroll' through the connected chests. Returns false if end or start has been reached.
        public bool ScrollSelectedChest(bool next = true)
        {
            if (next)
            {
                if (selectedChestIndex + 1 < this.connectedChests.Count)
                {
                    this.selectedChestIndex++;
                    this.selectedChest = this.connectedChests[this.selectedChestIndex];
                    return true;
                }
            }
            else
            {
                if (selectedChestIndex - 1 >= 0)
                {
                    this.selectedChestIndex--;
                    this.selectedChest = this.connectedChests[this.selectedChestIndex];
                    return true;
                }
            }
            return false;
        }


        /// Try to access the fridge remotely based on upgrades.
        public void RemoteFridgeAccess()
        {
            if (this.location.upgradeLevel <= 0)
                return;

            if (ModEntry.cheatUpgrades)
            {
                this.RemoteOpen();
                return;
            }

            if (this.upgradeModules.Contains(upgradeDimension.ToString()))
            {
                this.RemoteOpen();
            }
            else if (this.upgradeModules.Contains(upgradePortal.ToString()))
            {
                if ( !(Game1.currentLocation is MineShaft) )
                {
                    this.RemoteOpen();
                }
            }
            else if (this.upgradeModules.Contains(upgradeWarp.ToString()))
            {
                if (Game1.currentLocation is FarmHouse || Game1.currentLocation == Game1.getLocationFromName("Farm"))
                {
                    this.RemoteOpen();
                }
            }
        }

        /// Emergency close the fridge from menu.
        public void RemoteEmergencyClose()
        {
            FillReferences();
            this.remoteOpen = false;
            this.mutex.ReleaseLock();
            //Game1.showGlobalMessage("Remote Emergency Close Release mutex");
        }

        /// Remotely close the fridge on menu closed.
        public void RemoteClose()
        {
            if (Game1.activeClickableMenu == null)
            {
                FillReferences();
                this.remoteOpen = false;
                this.mutex.ReleaseLock();
                //Game1.showGlobalMessage("Remote Close");
            }
        }



        /// **************************************************************************************************************************************************
        /// PRIVATE METHODS


        /// Clears items in connected chests that have been left with 0 stacks items.
        /// (This can happen when some items are merged in the filling of references into the infinite storage)
        private void ClearEmptyStacks()
        {
            foreach (Chest c in this.connectedChests)
            {
                for (int index = c.items.Count - 1; index >= 0; --index)
                {
                    if (c.items[index].Stack == 0)
                    {
                        c.items.RemoveAt(index);
                    }
                }
                c.clearNulls();
            }
        }

        /// Safely remove all references from the infinite storage to the connected chests.
        private void ClearReferences()
        {
            List<Item> items = new List<Item>();

            foreach (Item i in this.items)
                items.Add(i);

            foreach (Item i in items)
            {
                this.items.Remove(i);
                this.clearNulls();
            }

            // we should clear stacks that could have been merged in the infinite storage.
            ClearEmptyStacks();

            // these are removed when out of scope anyway but I'm used to c++ and managing my own memory.
            items.Clear();

            this.referencesSetup = false;
        }

        /// Fill infinite storage with references to items in connected chests.
        /// Will not execute unless current references has been cleared.
        private void FillReferences()
        {
            if (this.referencesSetup)
                return;

            foreach (Chest c in this.connectedChests)
                foreach (Item i in c.items)
                    this.addItem(i);

            this.referencesSetup = true;
        }


        /// Request to open the fridge remotely.
        private void RemoteOpen()
        {
            //Game1.showGlobalMessage("Req Mutex!");
            this.mutex.RequestLock((Action)(() =>
            {
                //Game1.showGlobalMessage("Mutex Acc!");
                ClearReferences();
                this.remoteOpen = true;
                Game1.activeClickableMenu = CreateMenu();
                Game1.player.Halt();
            }), (Action)null);
        }

        private bool CanRemoteOpenInCurrentLocation()
        {
            if (this.location.upgradeLevel <= 0)
                return false;

            if (ModEntry.cheatUpgrades || this.upgradeModules.Contains(upgradeDimension.ToString()))
                return true;
            else if (this.upgradeModules.Contains(upgradePortal.ToString()))
            {
                if (!(Game1.currentLocation is MineShaft))
                    return true;
            }
            else if (this.upgradeModules.Contains(upgradeWarp.ToString()))
            {
                if (Game1.currentLocation is FarmHouse || Game1.currentLocation == Game1.getLocationFromName("Farm"))
                    return true;
            }
            return false;
        }


        /// **************************************************************************************************************************************************
        /// CALLBACK METHODS


        /// Updates the fridge remotely if there are nobody in the location.
        private void RemoteFridgeUpdate(object sender, OneSecondUpdateTickingEventArgs e)
        {
            if (this.location.farmers.Count <= 0 && Game1.currentLocation != null)
                this.mutex.Update(Game1.currentLocation);
        }

        /// Activates drawing of remote button if menu is game menu.
        private void RemoteFridgeOnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!this.remoteButtonActive)
                return;

            if (e.NewMenu is GameMenu menu)
            {
                if (!this.remoteButtonDraw)
                {
                    this.remoteButtonTab.bounds.X = menu.xPositionOnScreen+64;
                    this.remoteButtonTab.bounds.Y = menu.yPositionOnScreen-40;
                    this.remoteButton.bounds.X = this.remoteButtonTab.bounds.X+16;
                    this.remoteButton.bounds.Y = this.remoteButtonTab.bounds.Y+16;

                    if (ModEntry.cheatUpgrades || this.upgradeModules.Contains(upgradeWarp.ToString()) || this.upgradeModules.Contains(upgradePortal.ToString()) || this.upgradeModules.Contains(upgradeDimension.ToString()))
                        this.remoteButtonDraw = true;
                }
            }
            else if (this.remoteButtonDraw)
                this.remoteButtonDraw = false;
        }

        /// Draws the remote access button if it should be drawn.
        private void RemoteFridgeOnRenderedMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (!this.remoteButtonActive)
                return;

            if (this.remoteButtonDraw)
            {
                if (Game1.activeClickableMenu is GameMenu menu && menu.currentTab != 3 && CanRemoteOpenInCurrentLocation())
                {
                    if (menu.currentTab == 0)
                    {
                        this.remoteButtonTab.bounds.Y = menu.yPositionOnScreen - 32;
                        this.remoteButton.bounds.Y = this.remoteButtonTab.bounds.Y + 20;
                    }
                    else
                    {
                        this.remoteButtonTab.bounds.Y = menu.yPositionOnScreen - 40;
                        this.remoteButton.bounds.Y = this.remoteButtonTab.bounds.Y + 20;
                    }

                    this.remoteButtonTab.draw(e.SpriteBatch);
                    this.remoteButton.draw(e.SpriteBatch);
                }
                    
            }
        }

        /// Accepts input for activating remote access to the fridge.
        private void RemoteFridgeOnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            //if (e.Button == ModEntry.RemoteButton)
            //{
            //    if (Game1.activeClickableMenu != null || Game1.eventUp || Game1.dialogueUp)
            //        return;
            //    RemoteFridgeAccess();
            //}

            if (!this.remoteButtonActive)
                return;

            if (this.remoteButtonDraw && e.Button == StardewModdingAPI.SButton.MouseLeft)
            {
                if (this.remoteButtonTab.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
                    RemoteFridgeAccess();
            }
        }



        /// **************************************************************************************************************************************************
        /// UPGRADE and INFO METHODS


        /// Initial access to the fridge 'menu'.
        private void LookAt()
        {
            List<Response> responseList = new List<Response>();
            responseList.Add(new Response("Phone", "(Pick up the phone)"));
            responseList.Add(new Response("Note", "(Look at the notepad)"));
            if (this.remoteButtonActive)
                responseList.Add(new Response("Remote", "(Deactivate remote button)"));
            else
                responseList.Add(new Response("Remote", "(Activate remote button)"));
            responseList.Add(new Response("Leave", "(Leave)"));
            Game1.currentLocation.createQuestionDialogue("There is a phone on the side of the fridge and a notepad...", responseList.ToArray(), this.LookAnswer, (NPC)null);
            Game1.player.Halt();
        }

        /// Callback for dialogue answers on initial 'menu'.
        private void LookAnswer(Farmer who, string answer)
        {
            if (answer.Equals("Phone"))
            {
                Game1.drawObjectDialogue("*CLICK* Hello, this is Expanded Fridge Services. Do you want to order an upgrade for your fridge?");
                Game1.afterDialogues = new Game1.afterFadeFunction(this.CallForUpgrade);
            }
            else if (answer.Equals("Note"))
            {
                Game1.drawObjectDialogue("*SHUFFLE* 'The text seem hastily written...'");
                Game1.afterDialogues = new Game1.afterFadeFunction(this.LookAtNote);
            }
            else if (answer.Equals("Remote"))
            {
                this.remoteButtonActive = !this.remoteButtonActive;
                Game1.drawObjectDialogue("*CLICK*");
            }
        }

        /// Access to the fridge instructions choice.
        private void LookAtNote()
        {
            List<Response> responseList = new List<Response>();
            responseList.Add(new Response("About", "(Read 'congratulations' tab)"));
            responseList.Add(new Response("Storage", "(Read 'storage' tab)"));
            responseList.Add(new Response("ID", "(Read 'ID security' tab)"));
            responseList.Add(new Response("Lim", "(Read 'limitless storage' tab)"));
            responseList.Add(new Response("Warp", "(Read 'warp upgrade' tab)"));
            responseList.Add(new Response("Portal", "(Read 'portal upgrade' tab)"));
            responseList.Add(new Response("Dimension", "(Read 'dimension upgrade' tab)"));
            responseList.Add(new Response("Leave", "(Leave)"));
            Game1.currentLocation.createQuestionDialogue("There are different tabs in the notepad...", responseList.ToArray(), this.LookAtNoteAnswer, (NPC)null);
        }

        /// Callback answer for dialogue choice on fridge instructions.
        private void LookAtNoteAnswer(Farmer who, string answer)
        {
            if (answer.Equals("About"))
            {
                Game1.drawObjectDialogue("Congratulations on receiving the 'I.W.O.D 1300l Fridge Model', brought to you by Expanded Fridge Inc. The fridge comes installed with a phone that lets you order upgrades. We hope that you will be satisfied with the product.");
            }
            else if (answer.Equals("Storage"))
            {
                Game1.drawObjectDialogue("Buying a storage upgrade will grant your fridge an extra inventory tab the next day. Every house upgrade will also grant you 2 extra tabs. The maximum amount of tabs you can have in your fridge is 12.");
            }
            else if (answer.Equals("ID"))
            {
                Game1.drawObjectDialogue("The ID safety upgrade will let you decide if other farmers can access the content of your fridge. This upgrade is still in development.");
            }
            else if (answer.Equals("Lim"))
            {
                Game1.drawObjectDialogue("The limitless storage upgrade will let you store more types of items in your fridge.");
            }
            else if (answer.Equals("Warp"))
            {
                Game1.drawObjectDialogue("The warp fridge lets you access your fridge from your farmland by the use of a button. The button will appear in your inventory after your purchase.");
            }
            else if (answer.Equals("Portal"))
            {
                Game1.drawObjectDialogue("The portal fridge lets you access your fridge from anywere in the world that isn't deep bellow ground.");
            }
            else if (answer.Equals("Dimension"))
            {
                Game1.drawObjectDialogue("The dimension fridge lets you access the fridge from anywere in the world.");
            }
            else if (answer.Equals("Leave"))
            {
                return;
            }

        }

        /// Access to the upgrade menu choice.
        private void CallForUpgrade()
        {
            List<Response> responseList = new List<Response>();

            // storage upgrade
            if (this.upgradeStorage < 6)
            {
                string[] prices = { "2,000", "5,000", "10,000", "20,000", "30,000", "50,000" };

                string price = prices[this.upgradeStorage];

                responseList.Add(new Response("Storage", "A storage upgrade (" + price + " g)"));
            }
            // id system upgrade
            if (!this.upgradeModules.Contains(upgradeID.ToString()))
            {
                responseList.Add(new Response("ID", "The ID security upgrade (5,000 g)")); // enable local player lock button (5k)
            }

            if (!this.upgradeModules.Contains(upgradeLim.ToString()))
            {
                responseList.Add(new Response("Lim", "The limitless storage upgrade (10,000 g)")); // enable any item in fridge (10k)
            }

            if (!this.upgradeModules.Contains(upgradeWarp.ToString()))
            {
                responseList.Add(new Response("Warp", "The warp fridge upgrade (20,000 g)")); // tranfer items into fridge from farm (20k)
            }
            else if (!this.upgradeModules.Contains(upgradePortal.ToString()))
            {
                responseList.Add(new Response("Portal", "The portal fridge upgrade (50,000 g)")); // access fridge from farm, transfer items anywere (50k)
            }
            else if (!this.upgradeModules.Contains(upgradeDimension.ToString()))
            {
                responseList.Add(new Response("Dimension", "The dimension fridge upgrade (150,000 g)")); // access fridge from anywere (150k)
            }

            responseList.Add(new Response("Leave", "Nothing right now (Leave)"));

            Game1.currentLocation.createQuestionDialogue("I want to order...", responseList.ToArray(), this.CallAnswer, (NPC)null);
            Game1.player.Halt();
        }

        /// Callback answer for dialogue choice on upgrade menu.
        private void CallAnswer(Farmer who, string answer)
        {
            bool buySuccess = false;
            if (answer.Equals("Leave"))
            {
                return;
            }
            else if (answer.Equals("Storage"))
            {
                int[] prices = { 2000, 5000, 10000, 20000, 30000, 50000 };
                if (ShopMenu.getPlayerCurrencyAmount(Game1.player, 0) >= prices[this.upgradeStorage])
                {
                    ShopMenu.chargePlayer(Game1.player, 0, prices[this.upgradeStorage]);
                    this.upgradeStorage++;
                    Game1.drawObjectDialogue("Thank you very much! Your upgrade will be installed by tomorrow.");
                    return;
                }
            }
            else if (answer.Equals("ID"))
            {
                Game1.drawObjectDialogue("We cannot provide you with this service at the moment.");
                return;
            }
            else if (answer.Equals("Lim"))
            {
                if (ShopMenu.getPlayerCurrencyAmount(Game1.player, 0) >= 10000)
                {
                    ShopMenu.chargePlayer(Game1.player, 0, 10000);
                    this.upgradeModules += upgradeLim;
                    buySuccess = true;
                }
            }
            else if (answer.Equals("Warp"))
            {
                if (ShopMenu.getPlayerCurrencyAmount(Game1.player, 0) >= 20000)
                {
                    ShopMenu.chargePlayer(Game1.player, 0, 20000);
                    this.upgradeModules += upgradeWarp;
                    buySuccess = true;
                }
            }
            else if (answer.Equals("Portal"))
            {
                if (ShopMenu.getPlayerCurrencyAmount(Game1.player, 0) >= 50000)
                {
                    ShopMenu.chargePlayer(Game1.player, 0, 50000);
                    this.upgradeModules += upgradePortal;
                    buySuccess = true;
                }
            }
            else if (answer.Equals("Dimension"))
            {
                if (ShopMenu.getPlayerCurrencyAmount(Game1.player, 0) >= 150000)
                {
                    ShopMenu.chargePlayer(Game1.player, 0, 150000);
                    this.upgradeModules += upgradeDimension;
                    buySuccess = true;
                }
            }

            if (buySuccess)
                Game1.drawObjectDialogue("Thank you very much! Your upgrade module should be active now.");
            else
                Game1.drawObjectDialogue("It seem like you dont have enough money...");
        }



        /// **************************************************************************************************************************************************
        /// MARKED CHEST MANIPULATION


        /// Get a free tile for chest placement in our location.
        /// NOTE: This can return a value outside the map bound of the house.
        private Point GetFreeTileInLocation()
        {
            // for the whole width of the map
            for (int w = 0; w <= this.location.map.Layers[0].LayerWidth; w++)
                // for the whole height of the map
                for (int h = 0; h <= this.location.map.Layers[0].LayerHeight; h++)
                    // check if tile in width and height is placeable and not on wall
                    if (this.location.isTileLocationTotallyClearAndPlaceable(w,h) && !this.location.isTileOnWall(w,h))
                        return new Point(w, h);

            // NOTE: if we arrive here, the location is a mess!. We want to ensure the chests are safe so we give an out of reach option
            // we get a tile out of the map, this will be saved but cannot be accessed normally if the mod breaks but items can still be rescued
            // with an updated version or other cheat mods that accesses chests or enables movement out of the map.
            
            int y = 0;
            int x = 0;

            // move in y direction untill no other potential offmap objects are there
            while (this.location.isObjectAtTile(x, y))
                y++;

            ModEntry.DebugLog(
                "A free tile could not be found in location, object might become placed out of bounds at tile x:" + x + ", y:" + y + " in location: " + location.Name, 
                StardewModdingAPI.LogLevel.Warn);

            // return that position
            return new Point(x, y);
        }


        /// Removes the marked chests from our location.
        private void ClearChestsInLocation()
        {
            foreach (ChestAndPosition cp in GetAllMarkedChestsInLocation())
                this.location.objects.Remove(new Vector2(cp.x, cp.y));
        }

        /// Places connected chests into our location.
        /// (the function we use to place these chests can fail if there are not enough space in the location, in that case players wont be able to access the chests normally if the mod breaks)
        private void FillChestsInLocation()
        {
            foreach (Chest c in this.connectedChests)
                this.location.objects.Add(GetFreeTileInLocation(), c);
        }


        /// Finds all chests and their location in the farm house with the magic mark.
        private List<ChestAndPosition> GetAllMarkedChestsInLocation()
        {
            // chest container for found chests
            List<ChestAndPosition> chests = new List<ChestAndPosition>();

            // find all chests in location of chest type magic mark
            foreach (var pair in this.location.objects.Pairs)
            {
                if (pair.Value is Chest)
                {
                    ChestAndPosition cap = new ChestAndPosition();
                    cap.c = pair.Value as Chest;
                    if (cap.c.chestType.Value.Contains(MAGIC))
                    {
                        cap.x = (int)pair.Key.X;
                        cap.y = (int)pair.Key.Y;
                        chests.Add(cap);
                    }
                }
            }

            return chests;
        }

        /// Creates a marked chest and add it to our location.
        /// The chest has the fridge bool active and a custom string in the chest type variable, this so we can recognize our connected chests.
        private void CreateNewChestInLocation(int markNumber)
        {
            // create the new chest
            Chest newChest = new Chest(true);
            newChest.chestType.Value = MAGIC + ":" + markNumber.ToString();
            newChest.fridge.Value = true;

            //ModEntry.MonitorInstance.Log("Created new chest in our location: " + newChest.chestType.Value);

            // find open tile for chest
            Vector2 freeTile = GetFreeTileInLocation();
            this.location.objects.Add(freeTile, newChest);
        }



        /// **************************************************************************************************************************************************
        /// OVERT or OVERRIDE METHODS


        /// OVERT from Chest: Same as Chest but works for our selected chest.
        public override void grabItemFromChest(Item item, Farmer who)
        {
            if (!who.couldInventoryAcceptThisItem(item))
                return;

            this.selectedChest.items.Remove(item);
            this.selectedChest.clearNulls();
            Game1.activeClickableMenu = CreateMenu(); // OVERT
        }

        /// OVERT from Chest: Same as Chest but without limit to how many items we can add.
        public override Item addItem(Item item)
        {
            item.resetState();
            this.clearNulls();
            for (int index = 0; index < this.items.Count; ++index)
            {
                if (this.items[index] != null && this.items[index].canStackWith(item))
                {
                    item.Stack = this.items[index].addToStack(item);
                    if (item.Stack <= 0)
                        return (Item)null;
                }
            }
            // OVERT: Removed limit here
            this.items.Add(item);
            return (Item)null;
        }

        /// OVERT from Chest: Same as Chest but works for our selected chest.
        public override void grabItemFromInventory(Item item, Farmer who)
        {
            if (item.Stack == 0)
                item.Stack = 1;
            Item obj = item;
            if (ModEntry.cheatUpgrades || this.upgradeModules.Contains(upgradeLim.ToString()) || (!(item is Tool) && !(item is Ring) && !(item is Boots) && !(item is Hat) && !(item.isPlaceable())) ) // OVERT
                obj = this.selectedChest.addItem(item); // OVERT
            if (obj == null)
                who.removeItemFromInventory(item);
            else
                obj = who.addItemToInventory(obj);
            this.selectedChest.clearNulls(); // OVERT
            int id = Game1.activeClickableMenu.currentlySnappedComponent != null ? Game1.activeClickableMenu.currentlySnappedComponent.myID : -1;
            Game1.activeClickableMenu = CreateMenu(); // OVERT
            (Game1.activeClickableMenu as ExpandedFridgeMenu).heldItem = obj; // OVERT
            if (id == -1)
                return;
            Game1.activeClickableMenu.currentlySnappedComponent = Game1.activeClickableMenu.getComponentWithID(id);
            Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
        }


        /// OVERT from Chest: Simply override to not accept any tool actions.
        public override bool performToolAction(Tool t, GameLocation location)
        {
            return false;
        }

        /// OVERT from Chest: We must prepare inventories when we want to access the connected chests to avoid strange duplicate behaviours. 
        /// Also we dont access unless we have a selected chest. And we have option for upgrading fridge.
        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            if (who.facingDirection.Value == 3) // <OVERT
            {
                this.LookAt();
                return true;
            } // OVERT>
            
            if (justCheckingForActivity || this.selectedChest == null) // OVERT
                return true;
            if ((bool)((NetFieldBase<bool, NetBool>)this.playerChest))
                this.mutex.RequestLock((Action)(() =>
                {
                    ClearReferences(); // OVERT
                    this.frameCounter.Value = 5;
                    Game1.playSound((bool)((NetFieldBase<bool, NetBool>)this.fridge) ? "doorCreak" : "openChest");
                    Game1.player.Halt();
                    Game1.player.freezePause = 1000;
                }), (Action)null);
            return true;
        }

        /// OVERT from Chest: We update like normally but prepare the infinite storage after we closed the chest.
        public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
        {
            this.fixLidFrame();
            this.mutex.Update(environment);
            if (this.shakeTimer > 0)
            {
                this.shakeTimer -= time.ElapsedGameTime.Milliseconds;
                if (this.shakeTimer <= 0)
                    this.health = 10;
            }
            if ((bool)((NetFieldBase<bool, NetBool>)this.playerChest))
            {
                if ((int)((NetFieldBase<int, NetInt>)this.frameCounter) > -1 && this.currentLidFrame < 136)
                {
                    --this.frameCounter.Value;
                    if ((int)((NetFieldBase<int, NetInt>)this.frameCounter) > 0 || !this.mutex.IsLockHeld())
                        return;
                    if (this.currentLidFrame == 135)
                    {
                        Game1.activeClickableMenu = CreateMenu(); // OVERT
                        this.frameCounter.Value = -1;
                    }
                    else
                    {
                        this.frameCounter.Value = 5;
                        ++this.currentLidFrame;
                    }
                }
                else
                {
                    if ((int)((NetFieldBase<int, NetInt>)this.frameCounter) != -1 || this.currentLidFrame <= 131 || (Game1.activeClickableMenu != null || !this.mutex.IsLockHeld()))
                        return;
                    FillReferences(); // OVERT
                    this.mutex.ReleaseLock();
                    this.currentLidFrame = 135;
                    this.frameCounter.Value = 2;
                    environment.localSound("doorCreakReverse");
                }
            }
        }


        /// OVERT from Chest: We just need to be able to reset our own lid frame like Chest.
        public new void resetLidFrame()
        {
            this.currentLidFrame = (int)((NetFieldBase<int, NetInt>)this.startingLidFrame);
        }

        /// OVERT from Chest: Same as Chest but with our own lid frame.
        public new void fixLidFrame()
        {
            if (this.currentLidFrame == 0)
                this.currentLidFrame = (int)((NetFieldBase<int, NetInt>)this.startingLidFrame);
            if ((bool)((NetFieldBase<bool, NetBool>)this.playerChest))
            {
                if (this.mutex.IsLocked() && !this.mutex.IsLockHeld())
                {
                    this.currentLidFrame = 135;
                }
                else
                {
                    if (this.mutex.IsLocked())
                        return;
                    this.currentLidFrame = (int)((NetFieldBase<int, NetInt>)this.startingLidFrame);
                }
            }
            else
            {
                if (this.currentLidFrame != 501 || !this.mutex.IsLocked() || this.mutex.IsLockHeld())
                    return;
                this.currentLidFrame = 503;
            }
        }

    }
}
#endif