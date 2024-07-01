/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chiccenFL/StardewValleyMods
**
*************************************************/

namespace WildTreeTweaks
{
    public class ModConfig
    {

        public bool EnableMod { get; set; } = true; // enable the mod
        public bool OnlyOnFarm { get; set; } = false; // only overwrite farm trees (done)
        public bool GrowInWinter { get; set; } = false; // just like FTT, grow in winter -- patched in Tree.TryGetData
        public bool GrowNearTrees { get; set; } = false; // true = growth is NOT blocked by adjacent trees -- patched by Tree.IsGrowthBlockedByNearbyTree
        public bool MossFromMature { get; set; } = true; // whether they can grow moss from maturity (growthStage 4) or default (growthStage 14)
        public float Health { get; set; } = 10f; // start health -- patched by Tree constructor
        public float WoodMultiplier { get; set; } = 1f; // multiply wood output -- patched by Tree.tickUpdate && ModEntry.performTreeFall (in methods)
        public float MysteryBoxChance { get; set; } = 0.005f; // mystery box roll chance -- patched by Tree.performToolAction
        public bool BookChanceBool { get; set; } = false; // does user want to change the logic
        public float BookChance { get; set; } = 0.0005f; // chance to drop woodcutting book -- patched by Tree.performToolAction
        public float GrowthChance { get; set; } = 0.2f; // DONE? -- patched by Tree.TryGetData
        public float SeedChance { get; set; } = 0.05f; // chance for daily seed from shake -- patched by Tree.TryGetData
        public float SeedSpreadChance { get; set; } = 0.15f; // chance for seed spread overnight -- patched by Tree.TryGetData
        public bool Debug { get; set; } = false;

    }
}
