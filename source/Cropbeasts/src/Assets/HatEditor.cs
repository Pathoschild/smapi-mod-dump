using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Linq;

namespace Cropbeasts.Assets
{
	internal static class HatEditor
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ModConfig Config => ModConfig.Instance;
		private static JsonAssets.IApi JsonAssets => ModEntry.Instance.jsonAssets;

		public static readonly string Name = "Beast Mask";
		public static int Which { get; private set; } = -1;

		public static void PrepareSave ()
		{
			Which = JsonAssets?.GetHatId (Name) ?? -1;
		}

		public static void FixShopMenu (ShopMenu menu)
		{
			// Remove the invalid -1 hat from the inventory.
			RemoveHatFromMenu (menu, -1);

			// If the achievement has not been attained, remove the actual hat.
			if (Which != -1 && !AchievementEditor.HasAchievement ())
				RemoveHatFromMenu (menu, Which);
		}

		private static void RemoveHatFromMenu (ShopMenu menu, int which)
		{
			for (int i = menu.forSale.Count - 1; i >= 0; --i)
			{
				if (menu.forSale[i] is Hat hat && hat.which.Value == which)
					menu.forSale.RemoveAt (i);
			}

			for (int i = menu.itemPriceAndStock.Count - 1; i >= 0; --i)
			{
				ISalable key = menu.itemPriceAndStock.Keys.ElementAt (i);
				if (key is Hat hat && hat.which.Value == which)
					menu.itemPriceAndStock.Remove (key);
			}
		}
	}
}
