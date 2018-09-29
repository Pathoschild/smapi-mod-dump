using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
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
        private Keys ActionKey;
        private ChestManager ChestManager;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
            ControlEvents.KeyReleased += this.ControlEvents_KeyReleased;
        }


        /*********
        ** Private methods
        *********/
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            try
            {
                this.Config = this.Helper.ReadConfig<ModConfig>();
                this.ChestManager = new ChestManager(this.Monitor);

                if (!Enum.TryParse(this.Config.Keybind, true, out this.ActionKey))
                {
                    this.ActionKey = Keys.G;
                    this.Monitor.Log("Error parsing key binding. Defaulted to G");
                }

                ChestManager.ParseChests(this.Config.Chests);
            }
            catch (Exception ex)
            {
                this.Monitor.Log(ex.ToString(), LogLevel.Error);
            }
        }

        private void ControlEvents_KeyReleased(object sender, EventArgsKeyPressed e)
        {
            if (!Context.IsPlayerFree)
                return;

            if (e.KeyPressed == this.ActionKey)
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
