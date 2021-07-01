/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pepoluan/StackSplitRedux
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Menus;

using SChest = StardewValley.Objects.Chest;

namespace StackSplitRedux.MenuHandlers
    {
    public class CraftingPageHandler : GameMenuPageHandler<CraftingPage>
        {
        /// <summary>Store the click location so it's the same after the user submits the split menu even if they moved their mouse.</summary>
        private Point ClickItemLocation;

        /// <summary>Used to track if the inventory or crafting menu recieved input last.</summary>
        private bool WasInventoryClicked = false;

        /// <summary>Maximum possible amount for crafting/cooking item selected</summary>
        private int MaxAmount;

        /// <summary>Null constructor that currently only invokes the base null constructor</summary>
        public CraftingPageHandler()
            : base() {
            }

        /// <summary>Initializes the inventory using the most common variable names.</summary>
        public override void InitInventory() {
            // We need to do this explicitly because the crafting page uses a different variable name for hover item.
            var inventoryMenu = this.MenuPage.inventory;
            var hoveredItemField = Mod.Reflection.GetField<Item>(this.MenuPage, "hoverItem");

            this.InventoryHandler.Init(inventoryMenu, hoveredItemField);
            }

        /// <summary>Tells the handler that the inventory was shift-clicked.</summary>
        /// <param name="stackAmount">The default stack amount to display in the split menu.</param>
        /// <returns>If the input was handled or consumed. Generally returns not handled if an invalid item was selected.</returns>
        public override EInputHandled InventoryClicked(out int stackAmount) {
            this.WasInventoryClicked = true;
            return base.InventoryClicked(out stackAmount);
            }

        /// <summary>Tells the handler that the interface recieved the hotkey input.</summary>
        /// <param name="stackAmount">The default stack amount to display in the split menu.</param>
        /// <returns>If the input was handled or consumed. Generally returns not handled if an invalid item was selected.</returns>
        public override EInputHandled OpenSplitMenu(out int stackAmount) {
            stackAmount = Mod.Config.DefaultCraftingAmount;
            this.WasInventoryClicked = false;

            var hoverRecipe = Mod.Reflection.GetField<CraftingRecipe>(this.MenuPage, "hoverRecipe").GetValue();
            var hoveredItem = hoverRecipe?.createItem();
            var heldItem = Mod.Reflection.GetField<Item>(this.MenuPage, "heldItem").GetValue();

            // If we're holding an item already then it must stack with the item we want to craft.
            if (hoveredItem == null || (heldItem != null && heldItem.Name != hoveredItem.Name))
                return EInputHandled.NotHandled;

            // We might need to put in some mutex-wrangling here if there' a problem with MultiPlayer desyncs...

            // Grab ingredients in additional places (if any)
            List<Item> extraItems = null;
            if (this.MenuPage._materialContainers is not null) {
                extraItems = new();
                // Not using .SelectMany() but iterating manually so we can trigger TraceIfD (optionally if Config = Release)
                foreach (SChest chest in this.MenuPage._materialContainers) {
                    Log.TraceIfD($"Engrabbening {chest} @ {chest.TileLocation} (fridge = {chest.fridge.Value})");
                    extraItems.AddRange(chest.items);
                    }
                }

            // Only allow items that can actually stack
            if (!hoveredItem.canStackWith(hoveredItem) || !hoverRecipe.doesFarmerHaveIngredientsInInventory(extraItems))
                return EInputHandled.NotHandled;

            this.MaxAmount = hoverRecipe.getCraftableCount(extraItems);
            this.ClickItemLocation = new Point(Game1.getOldMouseX(true), Game1.getOldMouseY(true));
            return EInputHandled.Consumed;
            }

        /// <summary>Lets the handler run the logic for doing the split after the amount has been input.</summary>
        /// <param name="amount">The stack size the user requested.</param>
        public override void OnStackAmountEntered(int amount) {
            // Run regular inventory logic if that's what was clicked.
            if (this.WasInventoryClicked) {
                base.OnStackAmountEntered(amount);
                return;
                }

            int count = Math.Min(amount, this.MaxAmount);
            ISoundBank origSoundBank = Game1.soundBank;
            for (int i = 0; i < count; ++i) {
                // Only play sound for the very first RightClick, or else the sound will mix together and sounds horrible
                if (i > 0) Game1.soundBank = null;
                this.MenuPage.receiveRightClick(this.ClickItemLocation.X, this.ClickItemLocation.Y, playSound: i == 0);
                Game1.soundBank = origSoundBank;
                // NOTE: This nullify-then-restore tactic is needed because as of SDV 1.5.4, CraftingPage.receiveRightClick actually
                //       *ignores* the playSound parameter; it's supposed to pass that parameter to CraftingPage.clickCraftingRecipe,
                //       but it doesn't. So the same sound gets layered one atop another with a slight shift, resulting in an overly
                //       loud and very distorted blip.
                //       If this oversight is fixed in a future patch, we can remove the nullify-and-restore lines.
                }
            }
        }
    }
