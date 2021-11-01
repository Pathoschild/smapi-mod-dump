/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StephenKairos/Teban100-StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using InventoryCycle.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace InventoryCycle
{
    /// <summary>The mod entry point.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        private SButton FrontKey = SButton.E;
        private SButton BackKey = SButton.Q;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;

            ModConfig config = helper.ReadConfig<ModConfig>();

            this.FrontKey = config.FrontCycleKey;
            this.BackKey = config.BackCycleKey;

            this.Monitor.Log($"Loaded Cycle Keys as: \n For cycling forward: {this.FrontKey} \n For cycling backward {this.BackKey}");
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == FrontKey)
            {
                Item[] oldInventory = Game1.player.Items.ToArray();
                List<Item> newInventory = new List<Item>();
                for (int i = 12; i < oldInventory.Length; i++)
                {
                    newInventory.Add(oldInventory[i]);
                }
                for (int i = 0; i < 12; i++)
                {
                    newInventory.Add(oldInventory[i]);
                }

                Game1.player.setInventory(newInventory);
                if (Game1.activeClickableMenu is GameMenu)
                {
                    Game1.activeClickableMenu = new GameMenu();
                }
            }
            else if (e.Button == BackKey)
            {
                Item[] oldInventory = Game1.player.Items.ToArray();
                List<Item> newInventory = new List<Item>();
                for (int i = 24; i < oldInventory.Length; i++)
                {
                    newInventory.Add(oldInventory[i]);
                }
                for (int i = 0; i < 24; i++)
                {
                    newInventory.Add(oldInventory[i]);
                }

                Game1.player.setInventory(newInventory);
                if (Game1.activeClickableMenu is GameMenu)
                {
                    Game1.activeClickableMenu = new GameMenu();
                }
            }
        }
    }
}
