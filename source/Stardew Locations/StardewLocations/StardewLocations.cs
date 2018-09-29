using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using SFarmer = StardewValley.Farmer;

namespace StardewLocations
{
    internal class StardewLocations : Mod
    {
        public override void Entry(IModHelper helper)
        {
            
            LocationEvents.CurrentLocationChanged += LocationEvents_CurrentLocationChanged;
        }
        private void LocationEvents_CurrentLocationChanged(object sender, EventArgsCurrentLocationChanged e)
        {
            if (Game1.player == null)
                return;
            if (Game1.player.currentLocation != null)
                Game1.showGlobalMessage(getLocationNames(Game1.player.currentLocation.Name));
        }
        //Void to get Actual Location Names
        private string getLocationNames(string name)
        {
            var i18n = Helper.Translation;
            string outster = i18n.Get(name);
            if (outster.Contains("farm_name"))
            {
                outster = i18n.Get(name, new { farm_name = Game1.player.farmName });
            }                
            else if (outster.Contains("player_name"))
            {
                outster = i18n.Get(name, new { player_name = Game1.player.Name });
            }
            else
            {
                outster = i18n.Get(name);
            }            
            return "Current Location:\n\r"+ outster;
        }
    }
}
