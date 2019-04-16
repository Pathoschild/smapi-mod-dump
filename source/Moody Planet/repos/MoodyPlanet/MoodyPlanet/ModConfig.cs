using System;

namespace MoodyPlanet
{
    public class ModConfig
    {
        public int seed { get; set; } = (int)(DateTime.Now.Ticks & 0x0000FFFF);
    }
}