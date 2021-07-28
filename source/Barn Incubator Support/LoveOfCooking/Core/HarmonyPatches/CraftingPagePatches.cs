/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using Harmony;
using StardewValley;
using StardewValley.Menus;

namespace LoveOfCooking.Core.HarmonyPatches
{
	public static class CraftingPagePatches
	{
		private static uint _itemsCooked;

		public static void Patch(HarmonyInstance harmony)
		{
			Log.D($"Applying patches to CraftingPage.clickCraftingRecipe",
				ModEntry.Instance.Config.DebugMode);
			harmony.Patch(
				original: AccessTools.Method(typeof(CraftingPage), "clickCraftingRecipe"),
				prefix: new HarmonyMethod(typeof(CraftingPagePatches), nameof(CraftItem_Prefix)));
			harmony.Patch(
				original: AccessTools.Method(typeof(CraftingPage), "clickCraftingRecipe"),
				postfix: new HarmonyMethod(typeof(CraftingPagePatches), nameof(CraftItem_Postfix)));
		}
		
		public static void CraftItem_Prefix()
		{
			_itemsCooked = Game1.stats.ItemsCooked;
		}

		public static void CraftItem_Postfix(CraftingPage __instance, ClickableTextureComponent c)
		{
			if (Game1.stats.ItemsCooked <= _itemsCooked)
			{
				return;
			}

			var item = ModEntry.Instance.Helper.Reflection.GetField<Item>(__instance, "lastCookingHover").GetValue();
			var recipe = new CraftingRecipe(item.Name, isCookingRecipe: true);

			// Apply burn chance to destroy cooked food at random
			/*
			if (GameObjects.CookingMenu.GetBurnChance(recipe) > Game1.random.NextDouble())
			{
				// Add burnt food to inventory if possible
				var burntItem = new StardewValley.Object(ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "burntfood"), 1);
				ModEntry.AddOrDropItem(burntItem);

				// Hunt down the usual food to be incinerated
				if (Game1.player.hasItemInInventoryNamed(item.Name))
				{
					var items = Game1.player.Items.Where(i => i.Name == item.Name).ToArray();
					var index = Game1.player.Items.IndexOf(items[Game1.random.Next(items.Length)]);
					--Game1.player.Items[index].Stack;
					if (Game1.player.Items[index].Stack < 1)
						Game1.player.removeItemFromInventory(index);
				}
				else if (Game1.player.isInventoryFull())
				{
					var items = Game1.currentLocation.debris.Where(d => d.item.Name == item.Name).ToArray();
					// . . .
				}
			}
			*/

			if (ModEntry.Instance.Config.AddCookingSkillAndRecipes)
			{
				// TODO: SYSTEM: Finish integrating cooking skill profession bonuses into cooking minus new menu

				// Apply extra portion bonuses to the amount cooked
				if (ModEntry.CookingSkillApi.HasProfession(GameObjects.ICookingSkillAPI.Profession.ExtraPortion) && ModEntry.CookingSkillApi.RollForExtraPortion())
				{
					//qualityStacks[0] += numPerCraft;
				}

				// Update tracked stats
				if (!ModEntry.Instance.States.Value.FoodCookedToday.ContainsKey(item.Name))
					ModEntry.Instance.States.Value.FoodCookedToday[item.Name] = 0;
				ModEntry.Instance.States.Value.FoodCookedToday[item.Name] += item.Stack;

				// Add cooking skill experience
				ModEntry.CookingSkillApi.CalculateExperienceGainedFromCookingItem(
					item, numIngredients: recipe.getNumberOfIngredients(), item.Stack, applyExperience: true);
			}
		}
	}
}
