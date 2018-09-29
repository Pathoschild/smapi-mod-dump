using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace InfertileSoil
{
    class GardenPlanter : IndoorPot
    {
        
        public readonly HoeDirt[] planterDirt = new HoeDirt[2];
        public readonly bool horizontal;

        public GardenPlanter() { }

        public GardenPlanter(Vector2 tileLocation, bool horizontal = false) : base(tileLocation)
        {
            this.horizontal = horizontal;

            if (!isBeingRainedOn(Game1.currentLocation))
                return;

            foreach (HoeDirt dirt in this.planterDirt)
            {
                dirt.state.Value = 1;
            }
        }

        public override void DayUpdate(GameLocation location)
        {
            base.DayUpdate(location);
            bool raining = isBeingRainedOn(location);
            for (int i = 0; i < planterDirt.Length; i++)
            {
                int xTileOffset = i, yTileOffset = i;
                if (this.horizontal)
                    xTileOffset = 2 * i - 1;
                else
                    yTileOffset = 2 * i - 1;

                planterDirt[i].dayUpdate(location, new Vector2(this.TileLocation.X + xTileOffset, this.TileLocation.Y + yTileOffset));
                if (raining)
                    planterDirt[i].state.Value = 1;
            }
        }

        private static bool isBeingRainedOn(GameLocation location)
        {
            return (Game1.isRaining && location.IsOutdoors);
        }
    }
}
