/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GuiNoya/SVMods
**
*************************************************/

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