using System.Collections.Generic;
using System.IO;

namespace SailorStyles_Clothing
{
	internal class Const
	{
		/* files */
		internal const string CatId = "zss_cat";
		internal const string ShopDialogueRoot = "catshop.text.";
		internal const string ImgExt = ".png";
		internal const string JsonExt = ".json";
		
		/* content */
		internal const string CatDir = "Cat";
		internal const string HatsDir = "Hats";
		internal const string ClothingDir = "Clothing";

		internal const string ContentPackPrefix = "blueberry.SailorStyles.Clothing.";

		internal static readonly List<string> HatPacks = new List<string> {
			"Hats And Pieces"
		};

		internal static readonly List<string> ClothingPacks = new List<string> {
			"Sakura Kimono",
			"Skirts n' Stuff",
			"Everyday Heroes",
			"Uniform Operation",
			"Sailor Suits"
		};

		internal static readonly Dictionary<string, int> PackCosts = new Dictionary<string, int> {
			{ "Hats And Pieces", 100 },
			{ "Sakura Kimono", 375 },
			{ "Skirts n' Stuff", 250 },
			{ "Everyday Heroes", 50 },
			{ "Uniform Operation", 100 },
			{ "Sailor Suits", 100 }
		};

		/* assets */
		internal static readonly string AnimDescs = Path.Combine("Data", "animationDescriptions");
		internal static readonly string CatSchedule = Path.Combine("Characters", "schedules", CatId);

		internal static readonly string CatSprite = Path.Combine("Characters", CatId);
		internal static readonly string CatPortrait = Path.Combine("Portraits", CatId);

		/* values */
		internal const string LocationTarget = "Forest";

		internal const int CatShopQtyRatio = 5;

		internal const int CatX = 33;
		internal const int CatY = 96;

		internal const int DummyTileIndex = 140;
	}
}
