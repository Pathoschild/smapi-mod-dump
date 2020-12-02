/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/SailorStyles
**
*************************************************/

using System.Collections.Generic;
using System.IO;

namespace SailorStyles
{
	internal class ModConsts
	{
		/* Files */
		internal const string CatId = "zss_cat";
		internal const string ShopDialogueRoot = "catshop.text.";
		
		/* Content */
		internal const string CatDir = "Cat";
		internal const string HatsDir = "Hats";
		internal const string ClothingDir = "Clothing";
		internal const string HairstylesDir = "Hairstyles";

		internal const string ContentPackPrefix = "blueberry.SailorStyles.Clothing.";

		internal static readonly List<string> HatPacks = new List<string>
		{
			"Hats And Pieces"
		};

		internal static readonly List<string> ClothingPacks = new List<string>
		{
			"Sakura Kimono",
			"Skirts n' Stuff",
			"Everyday Heroes",
			"Uniform Operation",
			"Sailor Suits"
		};

		internal const int DefaultClothingCost = 280;
		internal static readonly Dictionary<string, int> ClothingPackCosts = new Dictionary<string, int>
		{
			{ "Hats And Pieces", 100 },
			{ "Sakura Kimono", 375 },
			{ "Skirts n' Stuff", 250 },
			{ "Everyday Heroes", 50 },
			{ "Uniform Operation", 100 },
			{ "Sailor Suits", 100 }
		};

		/* Assets */
		internal static readonly string AnimDescs = Path.Combine("Data", "animationDescriptions");

		internal static readonly string CatSchedule = Path.Combine("Characters", "schedules", CatId);
		internal static readonly string CatSpritesheet = Path.Combine("Characters", CatId);
		internal static readonly string CatPortrait = Path.Combine("Portraits", CatId);

		internal static readonly string HairstylesSpritesheet = Path.Combine(HairstylesDir, "hairstyles");

		/* Values */
		internal const string CatLocation = "Forest";
		internal const int CatX = 33;
		internal const int CatY = 96;
		internal const int DummyTileIndex = 140;
		internal const int CatShopQtyRatio = 5;

		/* API */
		internal static int HairstylesInitialIndex = -1;
	}
}
