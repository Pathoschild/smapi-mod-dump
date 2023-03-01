/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FricativeMelon/GroupableChests
**
*************************************************/

using System;
using System.Linq;
using System.Net.Mail;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using static StardewValley.Objects.Chest;

namespace GroupableChests
{

class ModConfig
{
}

    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {

        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        private ModConfig Config;

        private Chest chest;

        private int slot = -1;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            //this.Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private void behaviorOnItemSelectFromChest(Item item, Farmer who)
        {
            if (who.couldInventoryAcceptThisItem(item))
            {
                chest.GetItemsForPlayer(Game1.player.UniqueMultiplayerID).Remove(item);
                slot = -1;
                chest.ShowMenu();
            }
        }

        private void moveItemInChest(int slotTo)
        {
            NetObjectList<Item> item_list = chest.items;
            if (chest.SpecialChestType == SpecialChestTypes.MiniShippingBin || chest.SpecialChestType == SpecialChestTypes.JunimoChest)
            {
                item_list = chest.GetItemsForPlayer(Game1.player.UniqueMultiplayerID);
            }
            if (slot < 0 || slot > item_list.Count) return;
            Item item = item_list[slot];
            if (item == null)
            {
                return;
            }
            item.resetState();
            while (slotTo >= item_list.Count)
            {
                item_list.Add(null);
            }
            if (item_list[slotTo] == null)
            {
                item_list[slot] = null;
                slot = slotTo;
                item_list[slotTo] = item;
            }
            else if (item_list[slotTo] != null && item_list[slotTo].canStackWith(item))
            {
                item_list[slot] = null;
                slot = slotTo;
                item.Stack = item_list[slotTo].addToStack(item);
                if (item.Stack <= 0)
                {
                    return;
                }
            }
            return;
        }

        private Item addItemToChest(Item item)
        {
            item.resetState();
            bool LC = Helper.Input.IsDown(SButton.LeftControl);
            NetObjectList<Item> item_list = chest.items;
            if (chest.SpecialChestType == SpecialChestTypes.MiniShippingBin || chest.SpecialChestType == SpecialChestTypes.JunimoChest)
            {
                item_list = chest.GetItemsForPlayer(Game1.player.UniqueMultiplayerID);
            }
            int simSlotLeft = -1;
            int simSlotRight = -1;
            int simSlotNone = -1;
            //Fill up whatever stacks you can
            for (int i = 0; i < item_list.Count; i++)
            {
                if (item_list[i] != null)
                {
                    if (item_list[i].Name.Equals(item.Name))
                    {
                        if (simSlotRight == -1 && i+1 < chest.GetActualCapacity() && (i+1 >= item_list.Count || item_list[i+1] == null))
                        {
                            simSlotRight = i + 1;
                        }
                        else if (simSlotLeft == -1 && i - 1 >= 0 && item_list[i-1] == null)
                        {
                            simSlotLeft = i - 1;
                        }
                        if (item_list[i].canStackWith(item))
                        {
                            slot = i;
                            item.Stack = item_list[i].addToStack(item);
                            if (item.Stack <= 0)
                            {
                                return null;
                            }
                        }
                    }

                }
                else if (simSlotNone == -1)
                {
                    simSlotNone = i;
                }
            }
            if (simSlotRight > -1)
            {
                slot = simSlotRight;
            }
            else if (simSlotLeft > -1)
            {
                slot = simSlotLeft;
            }
            else if (simSlotNone > -1)
            {
                slot = simSlotNone;
            }
            else if (item_list.Count >= chest.GetActualCapacity())
            {
                // no valid spots
                return item;
            }
            else
            {
                // in this case, we should be appending at the end
                slot = item_list.Count;
            }
            if (slot >= item_list.Count)
            {
                item_list.Add(item);
            }
            else
            {
                item_list[slot] = item;
            }
            return null;
        }

        private void behaviorOnItemSelectFromInventory(Item item, Farmer who)
        {
            if (item.Stack == 0)
            {
                item.Stack = 1;
            }
            Item tmp = addItemToChest(item);
            if (tmp == null)
            {
                who.removeItemFromInventory(item);
            }
            else
            {
                tmp = who.addItemToInventory(tmp);
            }
            int oldID = (Game1.activeClickableMenu.currentlySnappedComponent != null) ? Game1.activeClickableMenu.currentlySnappedComponent.myID : (-1);
            chest.ShowMenu();
            (Game1.activeClickableMenu as ItemGrabMenu).heldItem = tmp;
            if (oldID != -1)
            {
                Game1.activeClickableMenu.currentlySnappedComponent = Game1.activeClickableMenu.getComponentWithID(oldID);
                Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
            }
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (Game1.activeClickableMenu is ItemGrabMenu igm)
            {
                Item i = Helper.Reflection.GetField<Item>(igm, "sourceItem").GetValue();
                if (i is Chest c)
                {
                    chest = c;
                    igm.behaviorOnItemGrab = behaviorOnItemSelectFromChest;
                    Helper.Reflection.GetField<ItemGrabMenu.behaviorOnItemSelect>(igm, "behaviorFunction").SetValue(behaviorOnItemSelectFromInventory);
                }
            }
        }

        private bool buttonEqual(InputButton[] inputs, SButton button)
        {
            return inputs.Any((InputButton p) => p.ToSButton() == button);
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (chest != null && slot >= 0 && Game1.activeClickableMenu is ItemGrabMenu igm)
            {
                int cap = igm.ItemsToGrabMenu.capacity;
                int rows = igm.ItemsToGrabMenu.rows;
                int cols = cap / rows;
                if (buttonEqual(Game1.options.moveLeftButton, e.Button))
                {
                    moveItemInChest(slot % cols == 0 ? slot + cols - 1 : slot - 1);
                }
                else if (buttonEqual(Game1.options.moveRightButton, e.Button))
                {
                    moveItemInChest(slot % cols == cols-1 ? slot - cols + 1 : slot + 1);
                }
                else if (buttonEqual(Game1.options.moveUpButton, e.Button))
                {
                    moveItemInChest((slot + cols*(rows-1)) % cap);
                }
                else if (buttonEqual(Game1.options.moveDownButton, e.Button))
                {
                    moveItemInChest((slot + cols) % cap);
                }
            }
        }
    }
}