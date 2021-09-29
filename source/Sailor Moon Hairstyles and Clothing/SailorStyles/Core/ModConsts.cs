/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/SailorStyles
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace SailorStyles
{
	internal static class ModConsts
	{
		/* Files */
		internal const string CatId = "zss_cat";
		internal const string ShopDialogueRoot = "catshop.text.";
		internal const string HairstylesSheetId = "sailorstyles";
		
		/* Content */
		internal const string CatDir = "Cat";
		internal const string HatsDir = "Hats";
		internal const string ClothingDir = "Clothing";
		internal const string HairstylesDir = "Hairstyles";
		internal const string ContentPackPrefix = "blueberry.SailorStyles.";
		internal const string ClothingPackPrefix = "Clothing.";
		internal const string HatPackPrefix = "Hats.";
		internal static readonly List<string> HatPacks = new List<string>
		{
			"Hats And Pieces"
			// "Tuxedo Top Hats"	<= Not enabled by default, content is added contextually
		};
		internal static readonly List<string> ClothingPacks = new List<string>
		{
			"Man About Town",
			"Stylish Rogue",
			"Sakura Kimono",
			"Skirts n' Stuff",
			"Everyday Heroes",
			"Uniform Operation",
			"Sailor Suits"
		};
		internal static readonly Dictionary<string, int> ClothingPackCosts = new Dictionary<string, int>
		{
			{ "Hats And Pieces", 100 },
			{ "Tuxedo Top Hats", 100 },
			{ "Man About Town", 50 },
			{ "Stylish Rogue", 375 },
			{ "Sakura Kimono", 375 },
			{ "Skirts n' Stuff", 250 },
			{ "Everyday Heroes", 50 },
			{ "Uniform Operation", 100 },
			{ "Sailor Suits", 100 }
		};
		internal const int DefaultClothingCost = 100;

		/* Assets */
		internal static readonly string GameContentAnimationsPath = Path.Combine("Data", "animationDescriptions");
		internal static readonly string LocalAnimationsPath = Path.Combine("assets", CatDir, "animations");

		internal static readonly string GameContentCatSchedulePath = Path.Combine("Characters", "schedules", CatId);
		internal static readonly string GameContentCatSpritesPath = Path.Combine("Characters", CatId);
		internal static readonly string GameContentCatPortraitPath = Path.Combine("Portraits", CatId);
		internal static readonly string LocalCatSchedulePath = Path.Combine("assets", CatDir, "schedule");
		internal static readonly string LocalCatSpritesPath = Path.Combine("assets", CatDir, "character");
		internal static readonly string LocalCatPortraitPath = Path.Combine("assets", CatDir, "portrait");

		internal static readonly string GameContentHairstyleDataPath = Path.Combine("Data", "HairData");
		internal static readonly string GameContentHairstyleImagePath = Path.Combine("Characters", "Farmer", HairstylesSheetId);
		internal static readonly string LocalHairstylesSpritesPath = Path.Combine("assets", HairstylesDir, "hairstyles");

		/* Values */
		internal const string CatLocation = "Forest";
		internal static readonly Point CatPosition = new Point(33, 96);
		internal const int DummyTileIndex = 140;
		internal const int CatShopQtyRatio = 5;
		internal const string CatMutexKey = "shoplock";

		/* API */
		internal static int HairstylesInitialIndex = -1;
	}
}
