using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using Netcode;


//bugs while playing:
/*
    - adding an item to a chest that already has a partial in the open chest and in the remote chest, might duplicate. 
    Need to add a pre-check for the open chest to give it priority. * probably fixed, nope, seems that the isn't full check broke being able to remote add when
    theres a full stack
    - weirdness when item exists in multiple places, possibly only when stacks are mostly full


    - need to come up with a system to properly identify if an item should stay in it's current chest
    if the item matched in the chest is literally the same item, then it should start the move check
    if it's not, abort
*/

namespace ChestPooling
{
    /// <summary>The mod entry point.</summary>
    public class ChestPoolingMainClass : Mod
    {

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            StardewModdingAPI.Events.PlayerEvents.InventoryChanged += this.Event_InventoryChanged;
            //this.helper = helper;
            //long uniqueID = this.Helper.Multiplayer.GetNewID();
        }


        /*********
        ** Private methods
        *********/
        private void DebugLog(string theString)
        {
#if DEBUG
            this.Monitor.Log(theString);
#endif
        }

        private void DebugThing(object data, string descriptor = "")
        {
            this.Helper.WriteJsonFile("debug.json", data);
            string result = File.ReadAllText(Path.Combine(this.Helper.DirectoryPath, "debug.json"));
            this.Monitor.Log($"{descriptor}\n{result}");
        }

        private List<Chest> GetChests()
        {
            if (!Game1.hasLoadedGame || Game1.currentLocation == null)
                return null;

            List<Chest> chestList = new List<Chest>();

            //this.Helper.Multiplayer.GetActiveLocations();
            //get chests from normal buildings
            foreach (GameLocation location in Game1.locations)
            {
                if(location == null) { break; }
                //get chests
                chestList.AddRange(location.Objects.Values.OfType<Chest>());

                //get fridge
                FarmHouse house = location as FarmHouse;
                
                if (house != null)
                {
                    Chest fridge = house.fridge.Value;
                    if (fridge != null)
                    {
                        chestList.Add(fridge);
                    }
                }  
            }

            //get stuff inside build buildings
            Farm farm = Game1.getFarm();
            if (farm != null)
            {
                foreach (StardewValley.Buildings.Building building in farm.buildings)
                {
                    GameLocation indoors = building.indoors.Value;
                    if (indoors != null)
                        chestList.AddRange(indoors.Objects.Values.OfType<Chest>());
                }
            }

            chestList.RemoveAll(IsIgnored);


            return chestList;
        }

        //chest filter predicate
        private bool IsIgnored(Chest chest)
        {
            return chest.Name == "IGNORED";
        }

        private Chest GetOpenChest()
        {
            if (Game1.activeClickableMenu == null)
                return null;

            if (Game1.activeClickableMenu is ItemGrabMenu)
            {
                //DebugLog("it's an item grab");
                ItemGrabMenu menu = (ItemGrabMenu)Game1.activeClickableMenu;
                Chest chest = menu.behaviorOnItemGrab?.Target as Chest;
                if (chest != null)
                    return chest;
            }
            return null;
        }

        private bool isExactItemInChest(Item sourceItem, NetObjectList<Item> items)
        {
            return items.Any(item => item == sourceItem);
        }

        private Item matchingItemInChest(Item sourceItem, NetObjectList<Item> items)
        {
            foreach (Item item in items)
            {
                //weirdly, this is an equals check
                //if (sourceItem.canStackWith(item) && (item.Stack - stackSizeOffset) < item.maximumStackSize() && item.Stack - stackSizeOffset > 0)
                if (sourceItem.canStackWith(item) && item.Stack < item.maximumStackSize() && item != sourceItem)
                    return item;
            }
            return null;
        }

        //method is poorly named
        private Chest QueryChests(List<Chest> chestList, Item itemRemoved)
        {
            //Log.Info("queryStarted");
            Chest openChest = this.GetOpenChest();
            Chest chestWithStack = null;
            Item itemToAddTo = null;
            bool hasFoundCurrentChest = false;

            //likely in some other menu
            if (openChest == null)
                return null;

            //foreach(StardewValley.Item item in openChest.items)
            //{

            //}

            //Log.Info("openChest isn't null");
            //the place where it went is fine
            if (!isExactItemInChest(itemRemoved, openChest.items))
            {
                //Log.Info("item in open chest, aborting");
                return null;
            }
            // Log.Info("isn't in the current chest");

            foreach (Chest chest in chestList)
            {
                if (chest.items.Equals(openChest.items))
                {
                    hasFoundCurrentChest = true;
                    continue;
                }

                //found something, don't bother going any further
                //consider adding another check that completely bails if both the open and "withStack" chest is found
                if (chestWithStack != null)
                    continue;

                Item item = matchingItemInChest(itemRemoved, chest.items);
                if (item != null)
                {
                    chestWithStack = chest;
                    itemToAddTo = item;
                }
            }

            //user probably just threw away the item
            //could probably remove this check as a "cheat" to allow remote deposit...
            if (!hasFoundCurrentChest)
                return null;
            //Log.Info("current chest was found");

            if (chestWithStack != null)
            {
                //Log.Info("chestWithStack isn't null");
                if (openChest.items.Count > 0 && chestWithStack.items.Count > 0)
                {
                    //Log.Info("open chest first item: " + openChest.items.First().Name);
                    //Log.Info("target chest first item: " + chestWithStack.items.First().Name);
                }

                int newStackSize = itemToAddTo.Stack + itemRemoved.Stack;

                //resize it in the chest it was placed in
                if (newStackSize > itemRemoved.maximumStackSize())
                {
                    //Log.Info("stack maxed");
                    this.DebugLog("stack maxed for " + itemToAddTo.Name);
                    itemRemoved.Stack = newStackSize - itemRemoved.maximumStackSize();
                    itemToAddTo.Stack = itemToAddTo.maximumStackSize();
                }
                //actually do things
                else
                {
                    itemToAddTo.addToStack(itemRemoved.Stack);
                    this.DebugLog(itemToAddTo.Name + " new size: " + newStackSize);
                    openChest.items.Remove(itemRemoved);
                    openChest.clearNulls();
                    Game1.activeClickableMenu = new ItemGrabMenu(openChest.items, false, true, InventoryMenu.highlightAllItems, openChest.grabItemFromInventory, null, openChest.grabItemFromChest, false, true, true, true, true, 1, openChest);
                    //openChest.grabItemFromChest(itemRemoved, StardewModdingAPI.Entities.SPlayer.CurrentFarmer);
                }
            }

            return null;
        }

        //e is a thing that contains "Inventory", "Added" and "Removed" properties, not yet sure what object that corresponds to
        private void Event_InventoryChanged(object sender, EventArgs e)
        {
            if (!Game1.hasLoadedGame || Game1.currentLocation == null)
                return;

            //the real event, might be necessary to determine what item was placed where
            StardewModdingAPI.Events.EventArgsInventoryChanged inventoryEvent = (StardewModdingAPI.Events.EventArgsInventoryChanged)e;
            
            if (inventoryEvent.Removed.Count == 0)
                return;

            List<Chest> chestList = this.GetChests();
            if (chestList == null)
                return;

            QueryChests(chestList, inventoryEvent.Removed.First().Item);
        }
    }
}
