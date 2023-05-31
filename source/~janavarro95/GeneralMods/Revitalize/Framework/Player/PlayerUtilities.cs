/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.SaveData.Player;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;
using StardewValley;

namespace Omegasis.Revitalize.Framework.Player
{
    public static class PlayerUtilities
    {

        /// <summary>
        /// Checks to see if the farmer team has completed a given special order.
        /// See Data/SpecialOrders and Strings/SpecialOrderStrings in the StardewValley Content folder for more info.
        /// </summary>
        /// <param name="SpecialOrderName">This is the name of the special order entry object, not the display name of the special order.</param>
        /// <returns></returns>
        public static bool HasCompletedSpecialOrder(string SpecialOrderName)
        {
            bool hasCompletedSpecialRequest = Game1.player.team.completedSpecialOrders.ContainsKey(SpecialOrderName);
            return Game1.player.team.completedSpecialOrders.ContainsKey(SpecialOrderName);
        }

        /// <summary>
        /// Checks to see if the special order for Robin's hardwood donation has been completed.
        /// </summary>
        /// <returns></returns>
        public static bool HasCompletedHardwoodDonationSpecialOrderForRobin()
        {
            return HasCompletedSpecialOrder("Robin");
        }

        /// <summary>
        /// Has the bottom of the hard mines been completed as Qi's special quest?
        /// </summary>
        /// <param name="who"></param>
        /// <returns></returns>
        public static bool HasReachedBottomOfHardMines(this Farmer who)
        {
            return who.hasOrWillReceiveMail("reachedBottomOfHardMines");
        }

        /// <summary>
        /// Adds an item to the player's inventory by a new slot, or stackable equivalent.
        /// </summary>
        /// <param name="Who"></param>
        /// <param name="I"></param>
        public static bool AddItemToInventory(this Farmer Who, Item I)
        {
            if (Who != null)
            {
                //Game1.showRedMessage("Attempt to add?");
                int emptyIndex = -1;
                for (int i = 0; i < Who.MaxItems; i++)
                {

                    //Find the first empty index in the player's inventory.
                    if (Who.Items[i] == null && emptyIndex == -1)
                    {
                        emptyIndex = i;
                        continue;
                    }
                    //Check to see if the items can stack. If they can simply add them together and then continue on.
                    if (Who.Items[i] != null && Who.Items[i].canStackWith(I))
                    {
                        Who.Items[i].Stack += I.Stack;
                        return true;
                    }
                }

                if (emptyIndex != -1)
                {
                    Who.Items[emptyIndex] = I;

                    //Set as active toolbar item.
                    if (emptyIndex < 12)
                    {
                        Who.CurrentToolIndex = emptyIndex;
                    }
                    return true;
                }
                return false;
            }
            return false;
        }

        public static bool InventoryContainsItem(this Farmer Who, Enums.SDVObject obj)
        {
            return InventoryContainsItem(Who, (int)obj);
        }
        public static bool InventoryContainsEnoughOfAnItem(this Farmer Who, Enums.SDVObject obj, int MinStackSize = 1)
        {
            return InventoryContainsEnoughOfAnItem(Who, (int)obj, MinStackSize);
        }

        public static bool ReduceInventoryItemIfEnoughFound(this Farmer Who, Enums.SDVObject obj, int MinStackSize)
        {
            if (InventoryContainsEnoughOfAnItem(Who, obj, MinStackSize))
            {
                return ReduceInventoryItemStackSize(Who, obj, MinStackSize);
            }
            return false;
        }

        /// <summary>
        /// Reduces a custom object in the inventory by a certain amount if found.
        /// </summary>
        /// <param name="Who"></param>
        /// <param name="BasicItemInfoId"></param>
        /// <param name="MinStackSize"></param>
        /// <returns></returns>
        public static bool ReduceInventoryItemIfEnoughFound(this Farmer Who, string BasicItemInfoId, int MinStackSize)
        {
            if (InventoryContainsEnoughOfAnItem(Who, BasicItemInfoId, MinStackSize))
            {
                return ReduceInventoryItemStackSize(Who, BasicItemInfoId, MinStackSize);
            }
            return false;
        }

        public static bool InventoryContainsItem(this Farmer Who, int ParentSheetIndex)
        {
            return InventoryContainsEnoughOfAnItem(Who, ParentSheetIndex);
        }

        public static bool ReduceInventoryItemStackSize(this Farmer Who, Enums.SDVObject ParentSheetIndex, int StackSize = 1)
        {
            return ReduceInventoryItemStackSize(Who, (int)ParentSheetIndex, StackSize);
        }

        /// <summary>
        /// Reduces the given stack size of a given vanilla item by a certain amount.
        /// </summary>
        /// <param name="Who"></param>
        /// <param name="ParentSheetIndex"></param>
        /// <param name="StackSizeToReduce"></param>
        /// <returns></returns>
        public static bool ReduceInventoryItemStackSize(this Farmer Who, int ParentSheetIndex, int StackSizeToReduce = 1)
        {
            if (Who != null)
            {

                if (Who.ActiveObject != null && Who.ActiveObject.ParentSheetIndex == ParentSheetIndex)
                {
                    Who.ActiveObject.Stack -= StackSizeToReduce;
                    if (Who.ActiveObject.Stack <= 0) Who.ActiveObject = null;
                    return true;
                }
                for (int i = 0; i < Who.MaxItems; i++)
                {

                    //Find the first empty index in the player's inventory.
                    if (Who.Items[i] == null)
                    {
                        continue;
                    }
                    //Check to see if the items can stack. If they can simply add them together and then continue on.
                    if (Who.Items[i].ParentSheetIndex == ParentSheetIndex)
                    {
                        if (Who.Items[i].Stack == StackSizeToReduce)
                        {
                            Who.Items[i] = null;
                            return true;
                        }
                        Who.Items[i].Stack -= StackSizeToReduce;

                        if (Who.Items[i].Stack <= 0) Who.Items[i] = null;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Reduces a <see cref="IBasicItemInfoProvider"/>'s stack size by a certain amount.
        /// </summary>
        /// <param name="Who"></param>
        /// <param name="ItemId"></param>
        /// <param name="StackSizeToReduce"></param>
        /// <returns></returns>
        public static bool ReduceInventoryItemStackSize(this Farmer Who, string ItemId, int StackSizeToReduce = 1)
        {
            if (Who != null)
            {
                if (Who.ActiveObject != null)
                {
                    Item item = Who.CurrentItem;

                    if (item is IBasicItemInfoProvider)
                    {
                        IBasicItemInfoProvider infoProvider = (IBasicItemInfoProvider)item;
                        //Check to see if the items can stack. If they can simply add them together and then continue on.
                        if (infoProvider.Id.Equals(ItemId))
                        {
                            item.Stack -= StackSizeToReduce;
                            if (Who.CurrentItem.Stack <= 0) Who.Items[Who.getIndexOfInventoryItem(Who.CurrentItem)] = null;
                            return true;
                        }
                    }
                }

                for (int i = 0; i < Who.MaxItems; i++)
                {
                    //Find the first empty index in the player's inventory.
                    if (Who.Items[i] == null)
                    {
                        continue;
                    }

                    Item item = Who.Items[i];
                    if (item is IBasicItemInfoProvider)
                    {
                        IBasicItemInfoProvider infoProvider = (IBasicItemInfoProvider)item;
                        //Check to see if the items can stack. If they can simply add them together and then continue on.
                        if (infoProvider.Id.Equals(ItemId))
                        {
                            if (Who.Items[i].Stack == StackSizeToReduce)
                            {
                                Who.Items[i] = null;
                                return true;
                            }

                            Who.Items[i].Stack -= StackSizeToReduce;

                            if (Who.Items[i].Stack <= 0) Who.Items[i] = null;
                            return true;
                        }
                    }
                    else
                    {
                        //This needs to be changed for Stardew Valley 1.6 to fully qualified item name.
                        string inventoryItemId = item.ParentSheetIndex.ToString();
                        if (inventoryItemId.Equals(ItemId))
                        {
                            if (Who.Items[i].Stack == StackSizeToReduce)
                            {
                                Who.Items[i] = null;
                                return true;
                            }

                            Who.Items[i].Stack -= StackSizeToReduce;

                            if (Who.Items[i].Stack <= 0) Who.Items[i] = null;
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Reduces a <see cref="Item"/>'s stack size by a certain amount.
        /// </summary>
        /// <param name="Who"></param>
        /// <param name="BasicItemInfoId"></param>
        /// <param name="StackSizeToReduce"></param>
        /// <returns></returns>
        public static bool ReduceInventoryItemStackSize(this Farmer Who, Item itemToReduce, int StackSizeToReduce = 1)
        {
            if (StackSizeToReduce == 0) return false;
            if (Who != null)
            {
                ReduceInventoryItemStackSize(Who.Items, itemToReduce, StackSizeToReduce, Who);
            }
            return false;
        }

        public static bool ReduceInventoryItemStackSize(IList<Item> ItemsToPullFrom, Item itemToReduce, int StackSizeToReduce = 1, Farmer who = null)
        {
            if (StackSizeToReduce == 0) return false;


            //Check for held object from the farmer.
            if (who != null)
            {
                if (who.ActiveObject != null)
                {
                    if ((who.ActiveObject == itemToReduce || who.ActiveObject.canStackWith(itemToReduce)))
                    {
                        if (itemToReduce.Stack == who.ActiveObject.Stack)
                        {
                            who.ActiveObject = null;
                            return true;
                        }
                        who.ActiveObject.Stack -= StackSizeToReduce;
                        return true;
                    }
                }
            }

            for (int i = 0; i < ItemsToPullFrom.Count; i++)
            {

                //Find the first empty index in the player's inventory.
                if (ItemsToPullFrom[i] == null)
                {
                    continue;
                }
                Item item = ItemsToPullFrom[i];

                if (item == itemToReduce || itemToReduce.canStackWith(item))
                {
                    if (ItemsToPullFrom[i].Stack == StackSizeToReduce)
                    {
                        ItemsToPullFrom[i] = null;
                        return true;
                    }

                    ItemsToPullFrom[i].Stack -= StackSizeToReduce;
                    if (ItemsToPullFrom[i].Stack <= 0) ItemsToPullFrom[i] = null; //This never runs technically since Stack always floors to a 1 value after reductions.
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Used to determine if a Player's inventory has enough of a given vanilla game item for Stardew Valley.
        /// </summary>
        /// <param name="Who"></param>
        /// <param name="ParentSheetIndex"></param>
        /// <param name="MinStackSize"></param>
        /// <returns></returns>
        public static bool InventoryContainsEnoughOfAnItem(this Farmer Who, int ParentSheetIndex, int MinStackSize = 1)
        {
            if (Who != null)
            {
                for (int i = 0; i < Who.MaxItems; i++)
                {

                    //Find the first empty index in the player's inventory.
                    if (Who.Items[i] == null)
                    {
                        continue;
                    }
                    //Check to see if the items can stack. If they can simply add them together and then continue on.
                    if (Who.Items[i].ParentSheetIndex == ParentSheetIndex && Who.Items[i].Stack >= MinStackSize)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool InventoryContainsTool<T>(this Farmer Who) where T : StardewValley.Tool
        {
            return GetToolsFromInventory<T>(Who).Count > 0;
        }

        /// <summary>
        /// Gets a list of all tools in the player's inventory that have the same base type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Who"></param>
        /// <returns></returns>
        public static List<T> GetToolsFromInventory<T>(this Farmer Who) where T : StardewValley.Tool
        {
            List<T> validTools = new List<T>();
            if (Who != null)
            {
                for (int i = 0; i < Who.MaxItems; i++)
                {

                    //Find the first empty index in the player's inventory.
                    if (Who.Items[i] == null)
                    {
                        continue;
                    }
                    //Check to see if the items can stack. If they can simply add them together and then continue on.
                    if ((Who.Items[i] is T))
                    {
                        validTools.Add((T)Who.Items[i]);
                    }
                }
            }
            return validTools;
        }

        /// <summary>
        /// Gets the current level of a tool if it is present, or -1 if the tool is not present.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int GetToolLevel<T>() where T : StardewValley.Tool
        {
            int toolLevel = 0;
            bool toolIsPresent = false;
            foreach (T axe in PlayerUtilities.GetToolsFromInventory<T>(Game1.player))
            {
                toolLevel = Math.Max(toolLevel, axe.UpgradeLevel);
                toolIsPresent = true;
            }
            if (toolIsPresent == false)
            {
                toolLevel = -1;
            }
            return toolLevel;
        }


        /// <summary>
        /// Check to see if a player's inventory contains enough of a given item.
        /// </summary>
        /// <param name="Who"></param>
        /// <param name="BasicItemInfoId"></param>
        /// <param name="MinStackSize"></param>
        /// <returns></returns>
        public static bool InventoryContainsEnoughOfAnItem(this Farmer Who, string BasicItemInfoId, int MinStackSize)
        {
            if (Who != null)
            {
                for (int i = 0; i < Who.MaxItems; i++)
                {

                    //Find the first empty index in the player's inventory.
                    if (Who.Items[i] == null)
                    {
                        continue;
                    }


                    Item item = Who.Items[i];

                    if (item is IBasicItemInfoProvider)
                    {
                        IBasicItemInfoProvider infoProvider = (IBasicItemInfoProvider)item;
                        //Check to see if the items can stack. If they can simply add them together and then continue on.
                        if (infoProvider.Id.Equals(BasicItemInfoId) && Who.Items[i].Stack >= MinStackSize)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks when items have been added to the player's inventory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnItemAddedToPlayersInventory(object sender, StardewModdingAPI.Events.InventoryChangedEventArgs e)
        {
            if (!e.Added.Any()) return;
            CheckForInventoryItem(e.Added);
        }

        public static void CheckForInventoryItem(IEnumerable<Item> items)
        {
            PlayerSaveData saveData = RevitalizeModCore.SaveDataManager.playerSaveData;
            foreach (Item addedItem in items)
            {
                ItemReference itemReference = new ItemReference(addedItem);

                if (!HasObtainedItem(itemReference.RegisteredObjectId))
                {
                    RevitalizeModCore.SaveDataManager.playerObtainedItems.obtainedItems.Add(itemReference.RegisteredObjectId);
                }

            }

        }

        /// <summary>
        /// Checks to see if the player has obtained an item with the specific item id.
        /// </summary>
        /// <param name="ItemId"></param>
        /// <returns></returns>
        public static bool HasObtainedItem(string ItemId)
        {
            if (RevitalizeModCore.SaveDataManager.playerObtainedItems == null) return false;

            return RevitalizeModCore.SaveDataManager.playerObtainedItems.obtainedItems.Contains(ItemId);
        }

        /// <summary>
        /// Checks to see if the player has has obtained an object.
        /// </summary>
        /// <param name="ItemId"></param>
        /// <returns></returns>
        public static bool HasObtainedItem(Enums.SDVObject ItemId)
        {
            return HasObtainedItem(RevitalizeModCore.ModContentManager.objectManager.createVanillaObjectId(ItemId));
        }
        /// <summary>
        /// Checks to see if the player has obtained a specific big craftable object before.
        /// </summary>
        /// <param name="ItemId"></param>
        /// <returns></returns>
        public static bool HasObtainedItem(Enums.SDVBigCraftable ItemId)
        {
            return HasObtainedItem(RevitalizeModCore.ModContentManager.objectManager.createVanillaBigCraftableId(ItemId));
        }

        /// <summary>
        /// Gets the number of golden walnuts found by all players.
        /// </summary>
        /// <returns></returns>
        public static int GetNumberOfGoldenWalnutsFound()
        {
            return Math.Max(0, Game1.netWorldState.Value.GoldenWalnutsFound.Value - 1);
        }

        /// <summary>
        /// Checks to see if the player knows a given crafting recipe added by the mod.
        /// </summary>
        /// <param name="CraftingBookName"></param>
        /// <param name="CraftingRecipeName"></param>
        /// <returns></returns>
        public static bool KnowsCraftingRecipe(string CraftingBookName, string CraftingRecipeName)
        {
            return RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingBookName, CraftingRecipeName);
        }

        /// <summary>
        /// Gets the type of footstep the player is making.
        /// </summary>
        /// <returns></returns>
        public static Enums.FootStepType GetFootStepType()
        {

            if (Game1.currentLocation.IsOutdoors || Game1.currentLocation.Name.ToLower().Contains("mine") || Game1.currentLocation.Name.ToLower().Contains("cave") || Game1.currentLocation.IsGreenhouse)
            {
                Vector2 tileLocationOfPlayer = Game1.player.getTileLocation();
                string stepType = Game1.currentLocation.doesTileHaveProperty((int)tileLocationOfPlayer.X, (int)tileLocationOfPlayer.Y, "Type", "Buildings");
                if (stepType == null || stepType.Length < 1)
                {
                    stepType = Game1.currentLocation.doesTileHaveProperty((int)tileLocationOfPlayer.X, (int)tileLocationOfPlayer.Y, "Type", "Back");
                }
                switch (stepType)
                {
                    case "Dirt":
                        return Enums.FootStepType.sandyStep;
                    case "Stone":
                        return Enums.FootStepType.stoneStep;
                    case "Grass":
                        return Enums.FootStepType.grassyStep;
                    case "Wood":
                        return Enums.FootStepType.woodyStep;
                    default: return Enums.FootStepType.thudStep;

                }
            }
            else
            {
                return Enums.FootStepType.thudStep;
            }
        }
    }
}
