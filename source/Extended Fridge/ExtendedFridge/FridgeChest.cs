using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Menus;

namespace ExtendedFridge
{
    internal class FridgeChest
    {
        public readonly int MAX_CAPACITY = ITEMS_PER_PAGE * MAX_ITEM_PAGE;
        public const int ITEMS_PER_PAGE = 36;
        public const int MAX_ITEM_PAGE = 6;
        public int currentpage = 0;
        public Item lastAddedItem;
        private bool _autoSwitchPageOnGrab = false;
        
        public List<Item> items = new List<Item>();

        public FridgeChest(bool bSwitchPage)
        {
            _autoSwitchPageOnGrab = bSwitchPage;
        }

        //public void DisplayCurrentPage()
        //{
        //    List<Item> newItems = GetCurrentPageItems();

        //    Game1.activeClickableMenu = (IClickableMenu)new ItemGrabMenu(newItems, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromInventory), GetPageString(), new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromChest), false, true, true, true, true, 1);
        //}

        public bool PageHasItems(int pagenumber)
        {
            return ( items.Count >= (pagenumber * 36) );
        }

        public void MovePageToNext()
        {
            if ( (this.currentpage + 1) > (MAX_ITEM_PAGE- 1 ) || !PageHasItems(currentpage +1 ) ) { return; }

            currentpage += 1;
            ShowCurrentPage();
        }

        public void ShowCurrentPage()
        {
            List<Item> newItems = GetCurrentPageItems();

            bool bShowNextPage = false;
            bool bShowPrevPage = false;

            int nextpage = currentpage + 1;
            int prevpage = currentpage - 1;            

            bShowPrevPage = ( (prevpage >= 0) && PageHasItems(prevpage) );
            bShowNextPage = ( ( nextpage <= (MAX_ITEM_PAGE - 1) ) && PageHasItems(nextpage) );         

            //Game1.activeClickableMenu = (IClickableMenu)new FridgeGrabMenu(newItems, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new FridgeGrabMenu.behaviorOnItemSelect(this.grabItemFromInventory), GetPageString(), new FridgeGrabMenu.behaviorOnItemSelect(this.grabItemFromChest), false, true, true, true, true, 1, new FridgeGrabMenu.behaviorOnPageCtlClick(this.MovePageToNext), new FridgeGrabMenu.behaviorOnPageCtlClick(this.MovePageToPrevious), bShowPrevPage, bShowNextPage, new FridgeGrabMenu.behaviorOnOrganizeItems(this.OrganizeItems));
            Game1.activeClickableMenu = (IClickableMenu)new FridgeGrabMenu(newItems, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new FridgeGrabMenu.behaviorOnItemSelect(this.grabItemFromInventory), GetPageString(), new FridgeGrabMenu.behaviorOnItemSelect(this.grabItemFromChest), false, true, true, true, true, 1, null, -1, null, new FridgeGrabMenu.behaviorOnPageCtlClick(this.MovePageToNext), new FridgeGrabMenu.behaviorOnPageCtlClick(this.MovePageToPrevious), bShowPrevPage, bShowNextPage, new FridgeGrabMenu.behaviorOnOrganizeItems(this.OrganizeItems));
        }

        public void MovePageToPrevious()
        {
            if (this.currentpage <= 0) { return; }
            if (!PageHasItems(currentpage - 1)) { return; }

            currentpage -= 1;
            ShowCurrentPage();
        }

        private int GetPageForIndex(int itemindex)
        {
            if (itemindex % 36 == 0)
            {
                return ((int)itemindex / 36) - 1;
            }
            else
            {
                return (int)itemindex / 36;
            }
        }

        private List<Item> GetCurrentPageItems()
        {
            List<Item> curPageItems = new List<Item>();

            int low_limit = currentpage * 36;
            int high_limit = low_limit + 35;

            for (int i = low_limit; i <= high_limit; i++)
            {
                if (i < items.Count)
                { 
                    curPageItems.Add(items[i]); 
                }
                else
                { 
                    break; 
                }
            }            

            return curPageItems;
        }

        //behaviorOnItemGrab
        public void grabItemFromChest(Item item, StardewValley.Farmer who)
        {
            if (!who.couldInventoryAcceptThisItem(item))
                return;
            this.items.Remove(item);
            this.clearNulls();

            StardewValley.Locations.FarmHouse h = (StardewValley.Locations.FarmHouse)Game1.currentLocation;
            h.fridge.items = items;

            //TODO: implement page change

            //List<Item> newItems = GetCurrentPageItems();

            this.ShowCurrentPage();
        }

        //behaviorOnItemSelectFunction
        public void grabItemFromInventory(Item item, StardewValley.Farmer who)
        {
            if (item.Stack == 0)
                item.Stack = 1;
            Item obj = this.addItem(item);
            if (obj == null)
                who.removeItemFromInventory(item);
            else
                who.addItemToInventory(obj);
            this.clearNulls();

            //TODO: implement page change
            //List<Item> newItems = GetCurrentPageItems();
            this.ShowCurrentPage();
        }

        public Item addItem(Item item)
        {

            lastAddedItem = null;

            for (int index = 0; index < this.items.Count<Item>(); ++index)
            {
                if (this.items[index] != null && this.items[index].canStackWith(item))
                {
                    item.Stack = this.items[index].addToStack(item.Stack);
                    if (item.Stack <= 0) { return (Item)null; }

                    if (_autoSwitchPageOnGrab) { currentpage = GetPageForIndex(index); }                    
                }
            }
            if (this.items.Count<Item>() >= MAX_CAPACITY) { return item; }

            this.items.Add(item);
            if (_autoSwitchPageOnGrab) { currentpage = GetPageForIndex(this.items.Count); }
            

            lastAddedItem = item;

            StardewValley.Locations.FarmHouse h = (StardewValley.Locations.FarmHouse)Game1.currentLocation;
            h.fridge.items = items;

            return (Item)null;
        }

        public void clearNulls()
        {
            for (int index = this.items.Count<Item>() - 1; index >= 0; --index)
            {
                if (this.items[index] == null)
                {
                    this.items.RemoveAt(index);
                    //if (_autoSwitchPageOnGrab) { currentpage = GetPageForIndex(index); }
                }
            }

            StardewValley.Locations.FarmHouse h = (StardewValley.Locations.FarmHouse)Game1.currentLocation;
            h.fridge.items = items;
        }

        private string GetPageString()
        {
            return String.Format("Extended Fridge {0} | Current Page: {1} | {2} items in fridge", M007_ExtendedFridge_Mod.Version, (currentpage + 1), this.items.Count);
        }


        //organize items behaviour
        public void OrganizeItems()
        {
            items.Sort();
            items.Reverse();

            StardewValley.Locations.FarmHouse h = (StardewValley.Locations.FarmHouse)Game1.currentLocation;
            h.fridge.items = items;

            currentpage = 0;
            ShowCurrentPage();
        }
    }
}