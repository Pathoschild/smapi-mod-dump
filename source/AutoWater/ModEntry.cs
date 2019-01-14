using System;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace AutoWater
{
    public class AutoWaterEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += this.WaterEverything;
        }

        private void WaterEverything(object sender, EventArgs e)
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
