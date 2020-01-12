using System.IO;

namespace SailorStyles_Clothing
{
	class Const
	{
		// files
		internal const string CatID = "zss_cat";
		internal const string ShopDialogueRoot = "catshop.text.";
		internal const string ImgExt = ".png";
		internal const string JsonExt = ".json";
		internal const string AssetsDir = "Assets";

		internal static readonly string CatDir = Path.Combine(AssetsDir, "Cat");
		internal static readonly string JAShirtsDir = Path.Combine(AssetsDir, "Shirts");
		internal static readonly string JAHatsDir = Path.Combine(AssetsDir, "Hats");

		internal static readonly string AnimDescs = Path.Combine("Data", "animationDescriptions");
		internal static readonly string CatSchedule = Path.Combine("Characters", "schedules", CatID);

		internal static readonly string CatTiles = Path.Combine(CatDir, CatID + "_tilesheet");
		internal static readonly string CatSprite = Path.Combine("Characters", CatID);
		internal static readonly string CatPortrait = Path.Combine("Portraits", CatID);
		internal static readonly string CatePortrait = Path.Combine("Portraits", CatID + "e");

		// keys
		internal const string LocationTarget = "Forest";

		// values
		internal const int CatShopQtyRatio = 5;
		internal const int CateRate = 100;
		internal const int ClothingCost = 50;

		internal const int CatX = 33;
		internal const int CatY = 96;
	}
}
