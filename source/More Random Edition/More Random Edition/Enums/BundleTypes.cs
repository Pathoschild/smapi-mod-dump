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
	/// <summary>
	/// The types of bundles
	/// The prefix on the name matters!
	/// </summary>
	public enum BundleTypes
	{
		None, // Default

		// Can be used by any room
		AllRandom,
		AllLetter,

		// Crafting room
		CraftingResource,
		CraftingHappyCrops,
		CraftingTree,
		CraftingTotems,
		CraftingBindle,
		CraftingSpringForaging,
		CraftingSummerForaging,
		CraftingFallForaging,
		CraftingWinterForaging,
		CraftingColorYellow,
		CraftingColorOrange,

		// Pantry
		PantryAnimal,
		PantryQualityCrops,
		PantryQualityForagables,
		PantryCooked,
		PantryFlower,
		PantrySpringCrops,
		PantrySummerCrops,
		PantryFallCrops,
		PantryEgg,
		PantryRareFoods,
		PantryDesert,
		PantryDessert,
		PantryMexicanFood,
		PantryColorGreen,
		PantryColorBrown,

		// Fish tank
		FishTankSpringFish,
		FishTankSummerFish,
		FishTankFallFish,
		FishTankWinterFish,
		FishTankOceanFood,
		FishTankRandom,
		FishTankLocation,
		FishTankRainFish,
		FishTankNightFish,
		FishTankQualityFish,
		FishTankBeachForagables,
		FishTankFishingTools,
		FishTankUnique,
		FishTankColorBlue,
		FishTankColorPurple,

		// Boiler room
		BoilerArtifacts,
		BoilerMinerals,
		BoilerGeode,
		BoilerGemstone,
		BoilerMetal,
		BoilerExplosive,
		BoilerRing,
		BoilerSpoopy,
		BoilerMonster,
		BoilerColorBlack,
		BoilerColorRed,
		BoilerColorGray,

		// Vault
		Vault2500,
		Vault5000,
		Vault10000,
		Vault25000,

		// Bulletin Board
		BulletinNews,
		BulletinCleanup,
		BulletinHated,
		BulletinLoved,
		BulletinAbigail,
		BulletinAlex,
		BulletinCaroline,
		BulletinClint,
		BulletinDemetrius,
		BulletinDwarf,
		BulletinElliott,
		BulletinEmily,
		BulletinEvelyn,
		BulletinGeorge,
		BulletinGus,
		BulletinHaley,
		BulletinHarvey,
		BulletinJas,
		BulletinJodi,
		BulletinKent,
		BulletinKrobus,
		BulletinLeah,
		BulletinLewis,
		BulletinLinus,
		BulletinMarnie,
		BulletinMaru,
		BulletinPam,
		BulletinPenny,
		BulletinPierre,
		BulletinRobin,
		BulletinSam,
		BulletinSandy,
		BulletinSebastian,
		BulletinShane,
		BulletinVincent,
		BulletinWilly,
		BulletinWizard,
		BulletinColorPink,
		BulletinColorWhite,

		// Joja
		JojaMissing
	}
}
