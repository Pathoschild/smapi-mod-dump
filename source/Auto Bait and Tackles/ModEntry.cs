/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alison-Li/AutoBaitAndTacklesMod
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace AutoBaitAndTackles
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private AutoBaitAndTacklesConfig config;

        /*********
        ** Public methods
        *********/

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.config = helper.ReadConfig<AutoBaitAndTacklesConfig>();
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        /*********
        ** Private methods
        *********/

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == this.config.ActivationKey)
            {
                this.AttachBaitAndTackles();
            }
        }

        private void AttachBaitAndTackles()
        {
            if (Game1.player.CurrentTool is FishingRod rod)
            {
                IList<Item> items = Game1.player.Items;

                // Check the bait slot.
                // Case where there is already bait attached.
                // We stack the same type of bait onto the existing bait attached to the fishing rod.
                if (rod.attachments[0] != null && rod.attachments[0].Stack != rod.attachments[0].maximumStackSize()
                    && Game1.player.hasItemWithNameThatContains("Bait") != null)
                {
                    foreach (Item item in items)
                    {
                        // Category value for bait is -21.
                        // Source: https://github.com/veywrn/StardewValley/blob/master/StardewValley/Item.cs
                        if (item != null && item.Category == -21 && item.Name.Equals(rod.attachments[0].Name))
                        {
                            int stackAdd = Math.Min(rod.attachments[0].getRemainingStackSpace(), item.Stack);
                            rod.attachments[0].Stack += stackAdd;
                            item.Stack -= stackAdd;
                            
                            if (item.Stack == 0)
                            {
                                Game1.player.removeItemFromInventory(item);
                            }
                        }
                    }
                    // Game1.showGlobalMessage($"All stacks of {rod.attachments[0].Name} automatically attached");
                }
                // Case where there is no bait attached.
                // We simply attach the first instance of bait we see in the inventory onto the fishing rod.
                else if (rod.attachments[0] == null && Game1.player.hasItemWithNameThatContains("Bait") != null)
                {
                    foreach (Item item in items)
                    {
                        if (item != null && item.Category == -21)
                        {
                            rod.attachments[0] = (Object)item;
                            Game1.player.removeItemFromInventory(item);
                            // Game1.showGlobalMessage($"{item.Name} automatically attached");
                            break;
                        }
                    }
                }

                // Check the tackle slot.
                if (rod.attachments[1] == null && (Game1.player.hasItemWithNameThatContains("Bobber") != null
                    || Game1.player.hasItemWithNameThatContains("Spinner") != null
                    || Game1.player.hasItemWithNameThatContains("Hook") != null
                    || Game1.player.hasItemWithNameThatContains("Treasure Hunter") != null))
                {
                    foreach (Item item in items)
                    {
                        // Category value for tackle is -22.
                        // Source: https://github.com/veywrn/StardewValley/blob/master/StardewValley/Item.cs
                        if (item != null && item.Category == -22)
                        {
                            rod.attachments[1] = (Object) item;
                            Game1.player.removeItemFromInventory(item);
                            // Game1.showGlobalMessage($"{item.Name} automatically attached");
                            break;
                        }
                    }
                }
            }
        }
    }
}