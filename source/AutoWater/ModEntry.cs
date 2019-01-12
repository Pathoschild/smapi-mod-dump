using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace AutoWater
{
    public class AutoWaterEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += this.waterEverything;
        }

        private void waterEverything(object sender, EventArgs e)
        {

            foreach (HoeDirt dirt in Game1.getLocationFromName("Farm").terrainFeatures.Values.OfType<HoeDirt>())
            {

                dirt.state.Value = HoeDirt.watered;

            }

            foreach (HoeDirt dirt in Game1.getLocationFromName("Greenhouse").terrainFeatures.Values.OfType<HoeDirt>())
            {

                dirt.state.Value = HoeDirt.watered;

            }
        }
    }
}
