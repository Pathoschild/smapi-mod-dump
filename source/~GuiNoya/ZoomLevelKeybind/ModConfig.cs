using StardewModdingAPI;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace ZoomLevelKeybind
{
    internal class ModConfig
    {
        public SButton IncreaseZoomKey { get; set; } = SButton.OemPeriod;
        public SButton DecreaseZoomKey { get; set; } = SButton.OemComma;
        public SButton IncreaseZoomButton { get; set; } = SButton.RightStick;
        public SButton DecreaseZoomButton { get; set; } = SButton.LeftStick;
        public bool SuppressControllerButton { get; set; } = true;
        public bool MoreZoom { get; set; } = true;
        public bool UnlimitedZoom { get; set; } = false;
    }
}