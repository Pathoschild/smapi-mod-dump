/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using StardewValley.Objects;
using StardewValley.Characters;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace PetWaterBowl
{
    public class PetBowl : Mod
    {
        private Config config;
        public bool EnableMod;
        public bool EnableSnowWatering;
        //private bool WaterBowl;

        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<Config>();
            EnableMod = this.config.EnableMod;
            EnableSnowWatering = this.config.EnableSnowWatering;
            //WaterBowl = false;
            //Evants
            ControlEvents.KeyPressed += KeyPressed;
            SaveEvents.AfterSave += AfterSave;
            SaveEvents.AfterLoad += AfterLoad;
        }
        //Private methods
        private void KeyPressed(object sender, EventArgsKeyPressed e)
        {
            if (!Context.IsWorldReady)
                return;
            if(e.KeyPressed == Keys.F5)
            {
                config = this.Helper.ReadConfig<Config>();
                EnableMod = this.config.EnableMod;
                EnableSnowWatering = this.config.EnableSnowWatering;
                this.Monitor.Log("Config Reloaded", LogLevel.Warn);
            }
        }
        private void AfterSave(object sender, EventArgs e)
        {
            if (!EnableMod)
                return;
            Farm farm = Game1.getFarm();
            if (Game1.isRaining || Game1.isLightning || (Game1.isSnowing && EnableSnowWatering))
            {
                farm.setMapTileIndex(54, 7, 1939, "Buildings", 0);
            }
            else
                farm.setMapTileIndex(54, 7, 1938, "Buildings", 0);
        }
        private void AfterLoad(object sender, EventArgs e)
        {
            if (!EnableMod)
                return;
            Farm farm = Game1.getFarm();
            if (Game1.isRaining || Game1.isLightning || (Game1.isSnowing && EnableSnowWatering))
            {
                farm.setMapTileIndex(54, 7, 1939, "Buildings", 0);
            }
            else
                farm.setMapTileIndex(54, 7, 1938, "Buildings", 0);
        }
    }
}
