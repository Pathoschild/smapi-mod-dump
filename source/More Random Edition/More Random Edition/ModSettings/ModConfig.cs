/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

namespace Randomizer
{
	public class ModConfig
	{
		public bool CreateSpoilerLog { get; set; } = true;
		public bool SaveRandomizedImages { get; set; } = false;

		public CraftingRecipesConfig CraftingRecipes { get; set; } = new CraftingRecipesConfig();
		public WeaponsConfig Weapons { get; set; } = new WeaponsConfig();
		public BootsConfig Boots { get; set; } = new BootsConfig();
		public MonstersConfig Monsters { get; set; } = new MonstersConfig();
		public CropsConfig Crops { get; set; } = new CropsConfig();
		public FishConfig Fish { get; set; } = new FishConfig();
		public NPCsConfig NPCs { get; set; } = new NPCsConfig();
		public BundleConfig Bundles { get; set; } = new BundleConfig();
		public MusicConfig Music { get; set; } = new MusicConfig();
		public ShopsConfig Shops { get; set; } = new ShopsConfig();
		public AnimalsConfig Animals { get; set; } = new AnimalsConfig();

		public bool RandomizeForagables { get; set; } = true;
		public bool AddRandomArtifactItem { get; set; } = true;
		public bool RandomizeBuildingCosts { get; set; } = true;
		public bool RandomizeFruitTrees { get; set; } = true;
		public bool RandomizeAnimalSkins { get; set; } = true;
		public bool RandomizeMuseumRewards { get; set; } = true;
		public bool RandomizeIntroStory { get; set; } = true;
		public bool RandomizeQuests { get; set; } = true;
		public bool RandomizeRain { get; set; } = true;
	}

	public class CraftingRecipesConfig
	{
		public bool Randomize { get; set; } = true;
		public bool RandomizeLevels { get; set; } = true;

	}

	public class WeaponsConfig
	{
		public bool Randomize { get; set; } = true;
		public bool UseCustomImages { get; set; } = true;
	}

	public class BootsConfig
	{
		public bool Randomize { get; set; } = true;
		public bool UseCustomImages { get; set; } = true;
        public int HueShiftMax { get; set; } = 359;
    }

	public class MonstersConfig
	{
		public bool Randomize { get; set; } = true;
		public bool SwapUniqueDrops { get; set; } = true;
        public int HueShiftMax { get; set; } = 359;
    }

	public class CropsConfig
	{
		public bool Randomize { get; set; } = true;
		public bool UseCustomImages { get; set; } = true;
		public int HueShiftMax { get; set; } = 359;
	}

	public class FishConfig
	{
		public bool Randomize { get; set; } = true;
		public bool UseCustomImages { get; set; } = true;
        public int HueShiftMax { get; set; } = 359;
    }

	public class NPCsConfig
	{
		public bool RandomizeBirthdays { get; set; } = true;
		public bool RandomizeIndividualPreferences { get; set; } = true;
		public bool RandomizeUniversalPreferences { get; set; } = true;
	}

	public class BundleConfig
	{
		public bool Randomize { get; set; } = true;
		public bool ShowDescriptionsInBundleTooltips { get; set; } = true;
	}

	public class MusicConfig
	{
		public bool Randomize { get; set; } = true;
		public bool RandomSongEachTransition { get; set; } = false;
	}

	public class ShopsConfig
	{
		public bool AddSeedShopItemOfTheWeek { get; set; } = true;
        public bool AddJojaMartItemOfTheWeek { get; set; } = true;
        public bool AddClayToRobinsShop { get; set; } = true;
		public bool AddTapperCraftItemsToRobinsShop { get; set; } = true;
		public bool RandomizeBlackSmithShop {  get; set; } = true;
        public bool RandomizeSaloonShop { get; set; } = true;
		public bool AddFishingShopCatchOfTheDay {  get; set; } = true;
        public bool RandomizeOasisShop { get; set; } = true;
		public bool AddHatShopHatOfTheWeek { get; set; } = true;
		public bool RandomizerSewerShop { get; set; } = true;
		public bool RandomizeClubShop { get; set; } = true;
    }

	public class AnimalsConfig
	{
		public bool RandomizeHorses { get; set; } = true;
		public bool RandomizePets { get; set; } = true;
	}
}

