/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods
**
*************************************************/

namespace BetterTruffles; 

internal class Configuration {
    public bool Enabled { get; set; } = true;
    public bool PigsDigInGrass {get; set;} = true;
    public bool ShowBubbles { get; set; } = true;
    public bool RenderOnTop { get; set; } = false;
    public int OffsetY { get; set; } = 0;
    public int OffsetX { get; set; } = 0;
    public int OpacityPercent { get; set; } = 75;
    public int SizePercent { get; set; } = 75;
}