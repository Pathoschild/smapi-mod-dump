/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

namespace BetterJunimosForestry {
    internal class ModConfig {
        internal readonly string[] WildTreePatternChoices = { "tight", "loose", "fruity-tight", "fruity-loose" };
        internal readonly string[] FruitTreePatternChoices = { "rows", "diagonal", "tight" };
        public string WildTreePattern { get; set; } = "loose";
        public string FruitTreePattern { get; set; } = "rows";
        public int PlantWildTreesSize { get; set; }
        public int PlantFruitTreesSize { get; set; }
        public bool SustainableWildTreeHarvesting { get; set; } = true;
        public bool InfiniteJunimoInventory { get; set; } = false;
        public bool HarvestGrassEnabled { get; set; } = true;
    }
}
