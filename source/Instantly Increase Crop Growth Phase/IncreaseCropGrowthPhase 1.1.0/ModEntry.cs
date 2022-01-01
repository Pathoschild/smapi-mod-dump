/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/RyanJesky/IncreaseCropGrowthPhase
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.TerrainFeatures;
using StardewValley.Locations;
using xTile.Dimensions;
using IncreaseCropGrowthPhase.Framework;
using StardewValley;

namespace IncreaseCropGrowthPhase
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {

        /// <summary>The mod settings.</summary>
        private KeyBindConfig Config;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            
            //read config.json file for keybind
            this.Config = helper.ReadConfig<KeyBindConfig>();


        }

        private void GrowCrop()
        {

            //instantiate instance of Location as location that is assigned the current cursor tile X and Y coordinates
            Location location = new Location((int)Game1.currentCursorTile.X, (int)Game1.currentCursorTile.Y);

            //instantiate an instance of GameLocation as instance
            GameLocation instance = Game1.currentLocation;

            //if current cursor tile is a crop tile then enter the if statement
            if (Game1.currentLocation.isCropAtTile(location.X, location.Y))
            {

                //if crop regrows and is fully grown enter if statement
                if ((instance.terrainFeatures[new Vector2(location.X, location.Y)] as HoeDirt).crop.regrowAfterHarvest.Get() > -1 & ((instance.terrainFeatures[new Vector2(location.X, location.Y)] as HoeDirt).crop.fullyGrown == true))
                {
                    //grow crop completely thus setting regrowable crop back to harvest phase
                    (instance.terrainFeatures[new Vector2(location.X, location.Y)] as HoeDirt).crop.growCompletely();
                }

                //if crop's current phase is less than the count of phase days - 1 enter if statement
                if ((instance.terrainFeatures[new Vector2(location.X, location.Y)] as HoeDirt).crop.currentPhase.Get() < (instance.terrainFeatures[new Vector2(location.X, location.Y)] as HoeDirt).crop.phaseDays.Count - 1)
                {

                    //increase crop's current phase by 1 and update draw math to display current crop phase without changing locations
                    (instance.terrainFeatures[new Vector2(location.X, location.Y)] as HoeDirt).crop.currentPhase.Set((instance.terrainFeatures[new Vector2(location.X, location.Y)] as HoeDirt).crop.currentPhase.Get() + 1);
                    (instance.terrainFeatures[new Vector2(location.X, location.Y)] as HoeDirt).crop.updateDrawMath(new Vector2(location.X, location.Y));
                }
            }

        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            //if Sbutton equals config.json keybind then enter loop
            if (e.Button.ToString().Contains(Config.GrowCropsKey.ToString()))
            {
                //Reference the GrowPlant method
                GrowCrop();
            }
        }
    }
}
