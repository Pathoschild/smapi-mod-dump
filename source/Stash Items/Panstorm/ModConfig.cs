/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zack-hill/stardew-valley-stash-items
**
*************************************************/

using StardewModdingAPI;

namespace Panstorm
{
    internal class ModConfig
    {
        public DisplayMode DisplayMode { get; set; } = DisplayMode.EnabledWhenHoldingPan;

        public int DisplayXOffset { get; set; } = 0;
        
        public int DisplayYOffset { get; set; } = 0;

        public bool PlayAudio { get; set; } = true;
        
        public float Volume { get; set; } = 2;
        
        public SButton StopPlayingButton { get; set; } = SButton.P;
    }
}
