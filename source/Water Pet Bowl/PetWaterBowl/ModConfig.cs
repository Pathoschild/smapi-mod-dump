using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PetWaterBowl
{
    internal class Config
    {
        public bool EnableMod { get; set; } = true;

        public bool EnableSprinklerWatering { get; set; } = true;

        public bool EnableSnowWatering { get; set; } = true;

        public Vector2 WaterBowlLocation { get; set; } = new Vector2(54, 7);
    }
}
