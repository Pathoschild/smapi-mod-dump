/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jdusbabek/stardewvalley
**
*************************************************/

using System;
using System.Collections.Generic;
using SortMyStuff.Framework;
using StardewLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;


namespace SortMyStuff
{
    public class SortMyStuff : Mod
    {
        /*********
        ** Properties
        *********/
        private ModConfig Config;
        private SButton ActionKey;
        private ChestManager ChestManager;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player loads a save slot.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, EventArgs e)
        {
            try
            {
                this.Config = this.Helper.ReadConfig<ModConfig>();
                this.ChestManager = new ChestManager(this.Monitor);

                if (!Enum.TryParse(this.Config.Keybind, true, out this.ActionKey))
                {
                    this.ActionKey = SButton.G;
                    this.Monitor.Log($"Error parsing key binding; defaulted to {this.ActionKey}.");
                }

                ChestManager.ParseChests(this.Config.Chests);
            }
            catch (Exception ex)
            {
                this.Monitor.Log(ex.ToString(), LogLevel.Error);
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            if (e.Button == this.ActionKey)
            {
                this.Monitor.Log("Logging key stroke G!!!", LogLevel.Trace);
                List<ItemContainer> ic = new List<ItemContainer>();
                try
                {
                    foreach (Item item in Game1.player.Items)
                    {
                        if (item != null)
                        {
                            this.Monitor.Log($"{item.Name}/{item.ParentSheetIndex}", LogLevel.Trace);
                            Object c = ChestManager.GetChest(item.ParentSheetIndex);
                            if (c is Chest)
                                ic.Add(new ItemContainer((Chest)c, item));
                            else
                            {
                                c = (Chest)ChestManager.GetChest(item.Category);
                                if (c is Chest)
                                    ic.Add(new ItemContainer((Chest)c, item));
                            }
                        }
                    }

                    foreach (ItemContainer i in ic)
                    {
                        Item o = i.Chest.addItem(i.Item);
                        if (o == null)
                            Game1.player.removeItemFromInventory(i.Item);
                        //else
                        //    Game1.player.addItemToInventory(i.item);
                    }


                    SortedDictionary<int, ChestDef> bestGuessChests = ChestManager.ParseAllChests();
                    string message = "";
                    foreach (KeyValuePair<int, ChestDef> chestDefs in bestGuessChests)
                    {
                        message += chestDefs.Key + ": " + chestDefs.Value + "\n";
                    }
                    this.Monitor.Log(message);
                }
                catch (Exception ex)
                {
                    this.Monitor.Log(ex.ToString(), LogLevel.Error);
                }
            }
        }
    }
}
