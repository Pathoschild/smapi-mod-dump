/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/


using StardewModdingAPI;

namespace HarvestSeeds
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool RegrowableSeeds { get; set; } = false;
        public int SeedChance { get; set; } = 10;
        public int MinSeeds { get; set; } = 1;
        public int MaxSeeds { get; set; } = 2;

    }
}
