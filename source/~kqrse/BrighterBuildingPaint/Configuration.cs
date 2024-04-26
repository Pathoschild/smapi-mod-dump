/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods
**
*************************************************/

namespace BrighterBuildingPaint; 

internal class Configuration {
    public bool Enabled { get; set; } = true;
    public int MaxBrightness { get; set; } = 50;
    public int MinBrightness { get; set; } = -50;
    public int MaxSaturation { get; set; } = 100;
}