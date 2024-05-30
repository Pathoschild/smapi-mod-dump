/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aceynk/AdvancedFishingTreasure
**
*************************************************/


namespace AdvancedFishingTreasure;
public sealed class ModConfig
{
     public bool ModEnabled { get; set; } = true;
     public int ChestChance { get; set; } = 15;
     public int VanillaMultiplier { get; set; } = 1;
     public int ModdedMultiplier { get; set; } = 0;
     public bool EnableBlacklist { get; set; } = true;
     public int MoneyPrize { get; set; } = 0;
     public bool VanillaFluctuation { get; set; } = true;
     public int PriceMax { get; set; } = -1;
     public int PriceMin { get; set; } = 0;

     public bool AllowSilver { get; set; } = false;
     public int SilverProb { get; set; } = 0;
     public bool AllowGold { get; set; } = false;
     public int GoldProb { get; set; } = 0;
     public bool AllowIridium { get; set; } = false;
     public int IridiumProb { get; set; } = 0;

     public string AllowedContext { get; set; } = "";
     public int ContextProb { get; set; } = 0;
     public bool IncludeModded { get; set; } = false; //Overrides other Include configs
     public bool IncludeExpBooks { get; set; } = false;
     public int ExpBooksChance { get; set; } = 5;
     public bool IncludeSkillBooks { get; set; } = false;
     public int SkillBooksChance { get; set; } = 5;
     public bool IncludeRings { get; set; } = false;
     public int RingsChance { get; set; } = 10;
     public bool IncludeGreens { get; set; } = false;
     public int GreensChance { get; set; } = 20;
     public bool IncludeFlowers { get; set; } = false;
     public int FlowersChance { get; set; } = 20;
     public bool IncludeFruit { get; set; } = false;
     public int FruitChance { get; set; } = 15;
     public bool IncludeVegetables { get; set; } = false;
     public int VegetablesChance { get; set; } = 15;
     public bool IncludeSeeds { get; set; } = false;
     public int SeedsChance { get; set; } = 20;
     public bool IncludeMonsterLoot { get; set; } = false;
     public int MonsterLootChance { get; set; } = 10;
     public bool IncludeTreeGoods { get; set; } = false;
     public int TreeGoodsChance { get; set; } = 10;
     public bool IncludeArtisanGoods { get; set; } = false;
     public int ArtisanGoodsChance { get; set; } = 10;
     public bool IncludeFloors { get; set; } = false;
     public int FloorsChance { get; set; } = 15;
     public bool IncludeFishingItems { get; set; } = false;
     public int FishingItemsChance { get; set; } = 10;
     public bool IncludeTackle { get; set; } = false;
     public int TackleChance { get; set; } = 5;
     public bool IncludeBait { get; set; } = false;
     public int BaitChance { get; set; } = 20;
     public bool IncludeTrash { get; set; } = false;
     public int TrashChance { get; set; } = 30;
     public bool IncludeFarmBoosts { get; set; } = false;
     public int FarmBoostsChance { get; set; } = 10;
     public bool IncludeAnimalGoods { get; set; } = false;
     public int AnimalGoodsChance { get; set; } = 10;
     public bool IncludeSpecialFarming { get; set; } = false;
     public int SpecialFarmingChance { get; set; } = 5;
     public bool IncludeComponents { get; set; } = false;
     public int ComponentsChance { get; set; } = 20;
     public bool IncludeOresBars { get; set; } = false;
     public int OresBarsChance { get; set; } = 10;
     public bool IncludeStones { get; set; } = false;
     public int StonesChance { get; set; } = 15;
     public bool IncludeSmallCraft { get; set; } = false;
     public int SmallCraftChance { get; set; } = 10;
     public bool IncludeFood { get; set; } = false;
     public int FoodChance { get; set; } = 15;
     public bool IncludeMilk { get; set; } = false;
     public int MilkChance { get; set; } = 10;
     public bool IncludeEggs { get; set; } = false;
     public int EggsChance { get; set; } = 10;
     public bool IncludeFish { get; set; } = false;
     public int FishChance { get; set; } = 20;
     public bool IncludeGems { get; set; } = false;
     public int GemsChance { get; set; } = 5;
     public bool IncludeSpecial { get; set; } = false;
     public int SpecialChance { get; set; } = 1;

     public bool IncludeBigCraft { get; set; } = false;
     public int BigCraftChance { get; set; } = 5;
     public bool IncludeTools { get; set; } = false;
     public int ToolsChance { get; set; } = 5;
     public bool IncludeWeapons { get; set; } = false;
     public int WeaponsChance { get; set; } = 5;
     public bool IncludePants { get; set; } = false;
     public int PantsChance { get; set; } = 5;
     public bool IncludeShirts { get; set; } = false;
     public int ShirtsChance { get; set; } = 5;
     
     
     public string ExcludeVanilla { get; set; } = "";
     public string ExcludeModded { get; set; } = "";
}