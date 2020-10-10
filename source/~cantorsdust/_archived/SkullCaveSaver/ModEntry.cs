/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cantorsdust/StardewMods
**
*************************************************/

using System;
using System.IO;
using SkullCaveSaver.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace SkullCaveSaver
{
    /// <summary>The entry class called by SMAPI.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration.</summary>
        private ModConfig Config;

        /// <summary>The mod configuration file path.</summary>
        private string ConfigPath;

        /// <summary>Whether the mod is warping to the saved mine level.</summary>
        private bool WarpingToSavedLevel;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            SaveEvents.AfterLoad += this.ReceiveAfterLoad;
            GameEvents.HalfSecondTick += this.ReceiveHalfSecondTick;
        }


        /*********
        ** Private methods
        *********/
        /****
        ** Event handlers
        ****/
        /// <summary>The method called after the player loads a saved game.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ReceiveAfterLoad(object sender, EventArgs e)
        {
            // read per-save config
            this.ConfigPath = Path.Combine("config", $"{Constants.SaveFolderName}.json");
            this.Config = this.Helper.ReadJsonFile<ModConfig>(this.ConfigPath) ?? new ModConfig();
            this.Helper.WriteJsonFile(this.ConfigPath, this.Config);
        }

        /// <summary>The method called roughly twice per second.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ReceiveHalfSecondTick(object sender, EventArgs e)
        {
            if (Game1.currentLocation is MineShaft mine)
            {
                // ignore if currently warping
                if (this.WarpingToSavedLevel)
                {
                    if (mine.mineLevel >= this.Config.LastMineLevel)
                        this.WarpingToSavedLevel = false;
                }

                // warp to warp level
                else if (mine.mineLevel > 120 && mine.mineLevel < this.Config.LastMineLevel && !this.WarpingToSavedLevel)
                {
                    Game1.enterMine(this.Config.LastMineLevel);
                    this.WarpingToSavedLevel = true;
                }

                // save mine level
                else if (mine.mineLevel > this.Config.LastMineLevel && (mine.mineLevel - this.Config.LastMineLevel) >= this.Config.SaveLevelEveryXFloors)
                {
                    this.Config.LastMineLevel = mine.mineLevel - (mine.mineLevel % this.Config.SaveLevelEveryXFloors);
                    this.Helper.WriteJsonFile(this.ConfigPath, this.Config);
                }
            }
        }
    }
}
