// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace SelfServiceShop
{
    internal class ModConfig
    {
        public bool Pierre { get; set; } = true;
        public bool Ranch { get; set; } = true;
        public bool Carpenter { get; set; } = true;
        public bool FishShop { get; set; } = true;
        public bool Blacksmith { get; set; } = true;
        public bool IceCreamStand { get; set; } = true;
        public bool IceCreamInAllSeasons { get; set; } = false;
        public bool ShopsAlwaysOpen { get; set; } = false;
    }
}