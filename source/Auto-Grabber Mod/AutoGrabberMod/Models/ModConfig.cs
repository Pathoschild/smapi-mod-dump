using System;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;

namespace AutoGrabberMod.Models
{
    public class ModConfig
    {
        public SButton OpenMenuKey { get; set; } = SButton.Z;

        public int MaxRange { get; set; } = 8;

        public bool AllowAutoHarvest { get; set; } = true;
        public bool AllowAutoSeed { get; set; } = true;
        public bool AllowAutoFertilize { get; set; } = true;
        public bool AllowAutoWater { get; set; } = true;
        public bool AllowAutoForage { get; set; } = true;
        public bool AllowAutoPet { get; set; } = true;
        public bool AllowAutoHoe { get; set; } = true;
        public bool AllowAutoDig { get; set; } = true;
        public bool ShowAllRangeGrids { get; set; } = false;
        public bool ShowRangeGridMouseOver { get; set; } = false;
    }
}
