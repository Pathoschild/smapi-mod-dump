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

namespace PetBed
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool IsBed { get; set; } = true;

        public int BedChance { get; set; } = 100;
        public string IndoorBedName { get; set; } = "Ped Bed";
        public string OutdoorBedName { get; set; } = "Ped Bed";
        public string IndoorBedOffset { get; set; } = "0,0";
        public string OutdoorBedOffset { get; set; } = "0,0";
    }
}
