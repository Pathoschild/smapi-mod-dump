/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/


using StardewModdingAPI;

namespace FruitTreeTweaks
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool CropsBlock { get; set; } = false;
        public bool ObjectsBlock { get; set; } = false;
        public bool TreesBlock { get; set; } = false;
        public bool PlantAnywhere { get; set; } = false;
        public bool FruitAllSeasons { get; set; } = false;
        public int DaysUntilMature { get; set; } = 28;
        public int MaxFruitPerTree { get; set; } = 3;
        public int MinFruitPerDay { get; set; } = 1;
        public int MaxFruitPerDay { get; set; } = 1;
        public int ColorVariation { get; set; } = 50;
        public int SizeVariation { get; set; } = 20;
        public int FruitSpawnBufferX { get; set; } = 5;
        public int FruitSpawnBufferY { get; set; } = 40;
        public int DaysUntilSilverFruit { get; set; } = 112;
        public int DaysUntilGoldFruit { get; set; } = 224;
        public int DaysUntilIridiumFruit { get; set; } = 336;
    }
}
