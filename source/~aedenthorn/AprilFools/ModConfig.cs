/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/


using StardewModdingAPI;

namespace AprilFools
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool BeesEnabled { get; set; } = true;
        public bool BackwardsEnabled { get; set; } = true;
        public bool AsciiEnabled { get; set; } = true;
        public bool PixelateEnabled { get; set; } = true;
        public bool TreeScreamEnabled { get; set; } = true;
        public bool InventoryEnabled { get; set; } = true;
        public bool RavenEnabled { get; set; } = true;
        public bool SlimeEnabled { get; set; } = true;
        public bool GiantEnabled { get; set; } = true;
        public bool BuildingsEnabled { get; set; } = true;
    }
}
