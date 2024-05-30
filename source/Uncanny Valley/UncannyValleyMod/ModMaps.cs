/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jasisco5/UncannyValleyMod
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley.Tools;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Force.DeepCloner;
using Microsoft.Xna.Framework;
using xTile;

namespace UncannyValleyMod
{
    internal class ModMaps
    {
        public ModSaveData oldSaveModel { get; private set; }
        public ModSaveData saveModel { get; private set; }
        public Dictionary<string, Token> tokens { get; set; }
        private IModHelper helper;
        private IMonitor Monitor;

        private bool wasRaining;
        private bool wasDebrisWeather;
        private bool wasLightning;
        private bool wasSnowing;

        public void SetSaveModel(ModSaveData _saveModel)
        {
            this.oldSaveModel = _saveModel.DeepClone();
            this.saveModel = _saveModel;
        }

        public ModMaps(IModHelper helper, IMonitor monitor)
        {
            this.helper = helper;
            this.Monitor = monitor;
            helper.Events.Player.Warped += this.OnWarped;
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            // Player is in the FarmHouse
            if (Game1.getLocationFromName("FarmHouse") == e.NewLocation)
            {
                //this.Monitor.Log($"{e.Player.Name} is in FarmHouse", LogLevel.Debug);

                return;
            }
            // Player is in the Town
            //if (!USEPATCHER && Game1.getLocationFromName("Town") == e.NewLocation)
            //{
            //    Warp mansionWarp = new Warp(112, 76, "Custom_Mansion_Exterior", 1, 49, false);
            //    e.NewLocation.warps.Add(mansionWarp);
            //    return;
            //}
            // Player enters the mansion exterior from town
            if (Game1.getLocationFromName("Custom_Mansion_Exterior") == e.NewLocation && Game1.getLocationFromName("Town") == e.OldLocation)
            {
                // Apply rain
                //this.Monitor.Log("Entering Uncanny Valley location, applying rain after this message     | wasRaining = " + wasRaining + " | wasLightning = " + wasLightning + " | wasDebrisWeather = " + wasDebrisWeather + " | wasSnowing = " + wasSnowing, LogLevel.Info);
                Game1.isRaining = true;
                Game1.isLightning = true;
                Game1.isDebrisWeather = false;
                Game1.isSnowing = false;
                Game1.performTenMinuteClockUpdate();
                Game1.timeOfDay -= 10;
                if (Game1.timeOfDay % 100 > 59)
                {
                    Game1.timeOfDay -= 40;
                }
                //this.Monitor.Log("Entering Uncanny Valley location, rain applied                         | wasRaining = " + wasRaining + " | wasLightning = " + wasLightning + " | wasDebrisWeather = " + wasDebrisWeather + " | wasSnowing = " + wasSnowing, LogLevel.Info);

                // Door warp
                if (!oldSaveModel.weaponObtained && saveModel.weaponObtained)
                {
                    // Bit of a dirty check, could probably be done OnAssetRequested
                    e.NewLocation.setTileProperty(25, 26, "Buildings", "Action", "Warp 44 47 Custom_Mansion_Interior");
                    //Warp mansionDoor = new Warp(25, 26, "Custom_Mansion_Interior", 44, 47, false);
                    //e.NewLocation.warps.Add(mansionDoor);
                }
                return;
            }
            // Player leaves the mansion exterior going to town
            else if (Game1.getLocationFromName("Town") == e.NewLocation && Game1.getLocationFromName("Custom_Mansion_Exterior") == e.OldLocation)
            {
                // Revert weather changes
                //this.Monitor.Log("Leaving Uncanny Valley location, undoing weather changes after this message        | wasRaining = " + wasRaining + " | wasLightning = " + wasLightning + " | wasDebrisWeather = " + wasDebrisWeather + " | wasSnowing = " + wasSnowing, LogLevel.Info);
                Game1.isRaining = wasRaining;
                Game1.isLightning = wasLightning;
                Game1.isDebrisWeather = wasDebrisWeather;
                Game1.isSnowing = wasSnowing;
                Game1.performTenMinuteClockUpdate();
                Game1.timeOfDay -= 10;
                if (Game1.timeOfDay % 100 > 59)
                {
                    Game1.timeOfDay -= 40;
                }
                //this.Monitor.Log("Leaving Uncanny Valley location, weather changes undone.                           | wasRaining = " + wasRaining + " | wasLightning = " + wasLightning + " | wasDebrisWeather = " + wasDebrisWeather + " | wasSnowing = " + wasSnowing, LogLevel.Info);
            }
            // Player is not entering or leaving the mod's areas
            else if (
                Game1.getLocationFromName("Custom_Mansion_Exterior")        != e.NewLocation &&
                Game1.getLocationFromName("Custom_Mansion_Interior")        != e.NewLocation &&
                Game1.getLocationFromName("Custom_Mansion_Basement")        != e.NewLocation &&
                Game1.getLocationFromName("Custom_Mansion_Basement_Chase")  != e.NewLocation)
            {
                //this.Monitor.Log("Not interacting with a UV area, updating wasWeather variables after this message   | wasRaining = " + wasRaining + " | wasLightning = " + wasLightning + " | wasDebrisWeather = " + wasDebrisWeather + " | wasSnowing = " + wasSnowing, LogLevel.Info);
                wasRaining = Game1.isRaining;
                wasLightning = Game1.isLightning;
                wasDebrisWeather = Game1.isDebrisWeather;
                wasSnowing = Game1.isSnowing;
                //this.Monitor.Log("Not interacting with a UV area, updated wasWeather variables                       | wasRaining = " + wasRaining + " | wasLightning = " + wasLightning + " | wasDebrisWeather = " + wasDebrisWeather + " | wasSnowing = " + wasSnowing, LogLevel.Info);
            }
        }
    }
}
