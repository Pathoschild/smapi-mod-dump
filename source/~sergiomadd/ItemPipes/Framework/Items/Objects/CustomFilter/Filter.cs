/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StardewValley;
using StardewValley.Menus;
using Netcode;
using SObject = StardewValley.Object;
using ItemPipes.Framework.Items.Objects;
using ItemPipes.Framework.Nodes.ObjectNodes;
using MaddUtil;


namespace ItemPipes.Framework.Items.CustomFilter
{
    public class Filter
	{
        public NetObjectList<Item> items { get; set; }
        public string message { get; set; }
        public int Cols { get; set; }
        public int Rows { get; set; }
        public int Capacity { get; set; }
        public Dictionary<string, string> Options { get; set; }
        public FilterPipeItem FilterPipe { get; set; }
        public Filter()
        {

        }
        public Filter(int capacity, FilterPipeItem filterPipe)
        {
            items = new NetObjectList<Item>();
            message = "Filter";
            Capacity = capacity;
            //Generate cols and rows based on capacity
            Cols = 9;
            Rows = 1;
            FilterPipe = filterPipe;
            Options = new Dictionary<string, string>();
            PopulateOptions();
        }

        public void PopulateOptions()
        {
            Options.Add("quality", "False");
        }

        public void UpdateOption(string option, string value)
        {
            if(FilterPipe.GetNode() != null)
            {
                (FilterPipe.GetNode() as FilterPipeNode).Filter.UpdateOption(option, value);
            }
        }

        public void ShowMenu()
        {
			Game1.activeClickableMenu = new FilterItemGrabMenu(this, items, Capacity, reverseGrab: false, showReceivingMenu: true, InventoryMenu.highlightAllItems,
                grabItemFromInventory, message, grabItemFromChest, snapToBottom: true, canBeExitedWithKey: true, playRightClickSound: true,
                allowRightClick: false, showOrganizeButton: false, 1, null, -80, this);
		}

        public void grabItemFromInventory(Item item, Farmer who)
        {
            if(CanAddItem(item))
            {
                addItem(item);
                clearNulls();
            }
        }

        public bool CanAddItem(Item item)
        {
            DataAccess dataAccess = DataAccess.GetDataAccess();
            bool can = true;
            string category = Utilities.GetItemCategoryTag(item);
            if (category.Equals("tool"))
            {
                can = false;
                Utilities.ShowInGameMessage(dataAccess.Warnings["filterError4"].Replace("itemName", item.DisplayName).Replace("itemType", item.getCategoryName()), "error");
                Printer.Debug($"Attempted to place a tool [{item.Name}] in a filter pipe. Tools are not allowed in filter pipes!");
            }
            else if (!dataAccess.IsVanillaItem(item))
            {
                can = false;
                Utilities.ShowInGameMessage(dataAccess.Warnings["filterError3"].Replace("itemName", item.DisplayName), "error");
                Printer.Debug($"Attempted to place a non vanilla item [{item.Name}] in a filter pipe. Non vanilla items are not allowed in filter pipes!");
            }
            else if(category.Equals("weapon"))
            {
                can = false;
                Utilities.ShowInGameMessage(dataAccess.Warnings["filterError4"].Replace("itemName", item.DisplayName).Replace("itemType", item.getCategoryName()), "error");
                Printer.Debug($"Attempted to place a weapon [{item.Name}] in a filter pipe. Weapons are not allowed in filter pipes!");
            }
            else if(category.Equals("cooking") || category.Equals("crafting"))
            {
                can = false;
                Utilities.ShowInGameMessage(dataAccess.Warnings["filterError4"].Replace("itemName", item.DisplayName).Replace("itemType", item.getCategoryName()), "error");
                Printer.Debug($"Attempted to place a recipe [{item.Name}] in a filter pipe. Recipes are not allowed in filter pipes!");
            }
            else
            {
                can = false;
                if(items.Count < Capacity)
                {
                    if (Utilities.ToBool(Options["quality"]))
                    {
                        if(item is SObject)
                        {
                            if (!items.Any(i => i.Name.Equals(item.Name) && (i as SObject).Quality.Equals((item as SObject).Quality)))
                            {
                                can = true;
                            }
                            else
                            {
                                if((item as SObject).Quality > 0)
                                {
                                    Utilities.ShowInGameMessage(dataAccess.Warnings["filterError2"].Replace("itemName", item.DisplayName), "error");
                                    Printer.Debug($"Attempted to place {item.Name} in a filter pipe. {item.Name} of that quality is already in the filter!!");
                                }
                                else
                                {
                                    Utilities.ShowInGameMessage(dataAccess.Warnings["filterError1"].Replace("itemName", item.DisplayName), "error");
                                    Printer.Debug($"Attempted to place {item.Name} in a filter pipe. {item.Name} of that quality is already in the filter!!");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!items.Any(i => i.Name.Equals(item.Name)))
                        {
                            can = true;
                        }
                        else
                        {
                            Utilities.ShowInGameMessage(dataAccess.Warnings["filterError1"].Replace("itemName", item.DisplayName), "error");
                            Printer.Debug($"Attempted to place {item.Name} in a filter pipe. {item.Name} is already in the filter!!");
                        }
                    }
                }
            }
            return can;
        }

        public void grabItemFromChest(Item item, Farmer who)
        {
            items.Remove(item);
            clearNulls();
            ShowMenu();
        }

        public Item addItem(Item item)
        {
            item.resetState();
            this.clearNulls();
            items.Add(item.getOne());
            return item;
        }

        public void clearNulls()
        {
            for (int j = this.items.Count - 1; j >= 0; j--)
            {
                if (this.items[j] == null)
                {
                    this.items.RemoveAt(j);
                }
            }
        }

        public int GetActualCapacity()
        {
            return items.Capacity;
        }
    }
}
