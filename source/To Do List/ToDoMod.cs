using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

using Microsoft.Xna.Framework.Input;

namespace ToDoMod
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /********
         ** Properties
         ********/
        /// <summary>
        /// The mod settings.
        /// </summary>
        private ModConfig Config;

        /// <summary>
        /// The saved task data.
        /// </summary>
        private ModData Data;

        /********
         ** Public methods
         ********/
        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// </summary>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>() ?? new ModConfig();
            ControlEvents.KeyPressed += this.ControlEvents_KeyPress;
            ControlEvents.ControllerButtonPressed += this.ControlEvents_ControllerButtonPressed;
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;

            /* Check if the set config key is valid i.e. won't close the menu when typing in the box! */
            /* Checks for single characters: any single letter, any single number, other likely typed characters */
            if (System.Text.RegularExpressions.Regex.IsMatch(this.Config.OpenListKey, @"^[a-zA-Z0-9-+=,./?*]$"))
            {
                this.Config.OpenListKey = Keys.F2.ToString();
                this.SaveConfig();
                this.Monitor.Log($"Set open menu key invalid, would prevent typing in textbox.", LogLevel.Warn);
                this.Monitor.Log($"Open menu key set to default of F2.", LogLevel.Warn);
            }
        }

        /********
         ** Private methods
         ********/
        /// <summary>
        /// Update the mod's config.json file from the current <see cref="Config"/>.
        /// </summary>
        private void SaveConfig()
        {
            this.Helper.WriteConfig(this.Config);
        }

        /// <summary>
        /// Update the save file's saved task list.
        /// </summary>
        private void SaveData()
        {
            this.Helper.WriteJsonFile($"data/{Constants.SaveFolderName}.json", this.Data);
        }

        /// <summary>
        /// Check for if the key pressed is the one bound to this menu.
        /// </summary>
        private void ControlEvents_KeyPress(object sender, EventArgsKeyPressed e)
        {
            /* Only open if the save file is loaded and the player can move. */
            if ((Context.IsWorldReady) && (Context.IsPlayerFree))
            {
                /* Check if the key pressed is the one bound in the config file. */
                if (e.KeyPressed.ToString() == this.Config.OpenListKey)
                {
                    /* If so open the to do list. */
                    this.OpenList();
                }
            }
        }

        /// <summary>
        /// Check for if the controller button pressed is the one bound to this menu.
        /// </summary>
        private void ControlEvents_ControllerButtonPressed(object sender, EventArgsControllerButtonPressed e)
        {
            if ((Context.IsWorldReady) && (Context.IsPlayerFree))
            {
                if (e.ButtonPressed.ToString() == this.Config.OpenListKey)
                    this.OpenList();
            }
           
        }

        /// <summary>The method called after the player loads their save.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            /* If the user wants the list to load at game start, do that */
            if (this.Config.OpenAtStartup)
            {
                this.OpenList();
            }
        }

        /// <summary>
        /// Open the to do list.
        /// </summary>
        private void OpenList()
        {
            /* Read in the specific saved task list for the opened save file */
            /* Or create one if it doesn't exist yet. */
            this.Data = this.Helper.ReadJsonFile<ModData>($"data/{Constants.SaveFolderName}.json") ?? new ModData();

            if (Game1.activeClickableMenu != null)
                Game1.exitActiveMenu();

            /* Open a to do list. */
            Game1.activeClickableMenu = new ToDoList(0, this.Config, this.SaveConfig, this.Data, this.SaveData);
        }

    }
}