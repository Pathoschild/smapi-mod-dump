/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kakashigr/stardew-radioactivetools
**
*************************************************/

namespace RadioactiveTools.Framework {
    public class ModConfig {
        public bool UseSprinklersAsScarecrows { get; set; } = true;
        public bool UseSprinklersAsLamps { get; set; } = true;
        public int SprinklerRange { get; set; } = 5;
        public int RadioactiveToolLength { get; set; } = 7;
        public int RadioactiveToolWidth { get; set; } = 2;
        public int RadioactiveToolCost { get; set; } = 200000;
    }
}
