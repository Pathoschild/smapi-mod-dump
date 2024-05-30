/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Caua-Oliveira/StardewValley-AutomateToolSwap
**
*************************************************/

using StardewModdingAPI.Utilities;
namespace AutomateToolSwap
{
    internal class ModConfig
    {
        /****
        ** Main Page Options
        ****/
        public bool Enabled { get; set; } = true;
        public KeybindList ToggleKey { get; set; } = KeybindList.Parse("CapsLock");
        public bool UseDifferentSwapKey { get; set; } = false;
        public KeybindList SwapKey { get; set; } = KeybindList.Parse("MouseLeft");
        public KeybindList LastToolKey { get; set; } = KeybindList.Parse("MouseMiddle");
        public string DetectionMethod { get; set; } = "Cursor";
        public bool AutoReturnToLastTool { get; set; } = false;


        /****
        ** Weapons Page Options
        ****/
        public bool WeaponOnMonsters { get; set; } = true;
        public bool AlternativeWeaponOnMonsters { get; set; } = false;
        public int MonsterRangeDetection { get; set; } = 3;
        public bool WeaponForMineBarrels { get; set; } = true;
        public bool IgnoreCrabs { get; set; } = true;
        /****
        ** Pickaxe Page Options
        ****/
        public bool PickaxeForStoneAndOres { get; set; } = true;
        public bool PickaxeForBoulders { get; set; } = true;
        public bool PickaxeOverWateringCan { get; set; } = false;


        /****
        ** Axe Page Options
        ****/
        public bool AxeForTwigs { get; set; } = true;
        public bool AxeForTrees { get; set; } = true;
        public bool AxeForFruitTrees { get; set; } = false;
        public bool AxeForStumpsAndLogs { get; set; } = true;
        public bool AxeForGiantCrops { get; set; } = true;
        public bool IgnoreGrowingTrees { get; set; } = false;


        /****
        ** Hoe Page Options
        ****/
        public bool HoeForArtifactSpots { get; set; } = true;
        public bool HoeForGingerCrop { get; set; } = true;
        public bool HoeForDiggableSoil { get; set; } = true;

        /****
        ** Scythe Page Options
        ****/
        public bool ScytheForWeeds { get; set; } = true;
        public bool ScytheForBushes { get; set; } = true;
        public bool ScytheForMossOnTrees { get; set; } = true;
        public bool ScytheForCrops { get; set; } = true;
        public bool ScytheForGrass { get; set; } = false;
        public bool ScytheForForage { get; set; } = false;

        /****
        ** WateringCan Page Options
        ****/
        public bool WateringCanForGardenPot { get; set; } = true;
        public bool WateringCanForUnwateredCrop { get; set; } = true;
        public bool WateringCanForPetBowl { get; set; } = true;
        public bool WateringCanForWater { get; set; } = true;

        /****
        ** Machines Page Options
        ****/
        public bool OresForFurnaces { get; set; } = true;
        public bool MilkForCheesePress { get; set; } = true;
        public bool EggsForMayoMachine { get; set; } = true;
        public bool TrashForRecycling { get; set; } = true;
        public bool BoneForBoneMill { get; set; } = true;
        public bool WoolForLoom { get; set; } = true;
        public bool FishForSmoker { get; set; } = true;
        public bool FishForBaitMaker { get; set; } = true;
        public bool MineralsForCrystalarium { get; set; } = true;
        public bool SwapForSeedMaker { get; set; } = false;
        public bool BaitForCrabPot { get; set; } = true;
        public bool OresForFurnace { get; set; } = true;
        public string SwapForKegs { get; set; } = "None";
        public string SwapForPreservesJar { get; set; } = "None";

        /****
        ** Random Options
        ****/
        public bool AnyToolForWeeds { get; set; } = false;
        public bool FishingRodOnWater { get; set; } = true;
        public bool SeedForTilledDirt { get; set; } = false;
        public bool DisableTractorSwap { get; set; } = false;
        public bool AnyToolForSupplyCrates { get; set; } = true;
        public bool PanForPanningSpots { get; set; } = true;
        public bool HayForFeedingBench { get; set; } = true;
        public bool MilkPailForCowsAndGoats { get; set; } = true;
        public bool ShearsForSheeps { get; set; } = true;
        public bool FertilizerForCrops { get; set; } = false;


    }


}