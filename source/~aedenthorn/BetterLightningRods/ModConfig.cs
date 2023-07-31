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

namespace BetterLightningRods
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool UniqueCheck { get; set; } = true;
        public bool OnlyCheckEmpty { get; set; } = false;
        public int RodsToCheck { get; set; } = 2;
        public float LightningChance { get; set; } = 13f;
        public bool Astraphobia { get; set; } = false;
    }
}
