using System;
using System.Collections.Generic;
using Harmony;
using StardewValley;
using StardewValley.Menus;

namespace BlueberryMushroomMachine
{
	internal class HarmonyPatches
	{
		public static void Apply()
		{
			var harmony = HarmonyInstance.Create($"{ModValues.AuthorName}.{ModValues.PackageName}");

			harmony.Patch(
				original: AccessTools.Method(typeof(CraftingPage), "performHoverAction"),
				postfix: new HarmonyMethod(
					typeof(CraftingPage_HoverAction),
					nameof(CraftingPage_HoverAction.Postfix)));
			harmony.Patch(
				original: AccessTools.Method(typeof(CraftingPage), "clickCraftingRecipe"),
				prefix: new HarmonyMethod(
					typeof(CraftingPage_ClickCraftingRecipe),
					nameof(CraftingPage_ClickCraftingRecipe.Prefix)));
			harmony.Patch(
				original: AccessTools.Method(typeof(CraftingRecipe), "createItem"),
				postfix: new HarmonyMethod(
					typeof(CraftingRecipe_CreateItem),
					nameof(CraftingRecipe_CreateItem.Postfix)));
		}

		public class CraftingPage_HoverAction
		{
			internal static void Postfix(CraftingRecipe ___hoverRecipe)
			{
				if (___hoverRecipe == null)
					return;
				if (___hoverRecipe.name.Equals(ModValues.PropagatorInternalName))
					___hoverRecipe.DisplayName = new Propagator().DisplayName;
			}
		}

		public class CraftingPage_ClickCraftingRecipe
		{
			internal static bool Prefix(
				List<Dictionary<ClickableTextureComponent, CraftingRecipe>> ___pagesOfCraftingRecipes,
				int ___currentCraftingPage, Item ___heldItem,
				ClickableTextureComponent c, bool playSound = true)
			{
				try
				{
					// Fetch an instance of any clicked-on craftable in the crafting menu
					var tempItem = ___pagesOfCraftingRecipes[___currentCraftingPage][c]
						.createItem();

					// Fall through the prefix for any craftables other than the Propagator
					if (!tempItem.Name.Equals(ModValues.PropagatorInternalName))
						return true;

					// Behaviours as from base method
					if (___heldItem == null)
					{
						___pagesOfCraftingRecipes[___currentCraftingPage][c]
							.consumeIngredients(null);
						___heldItem = tempItem;
						if (playSound)
							Game1.playSound("coin");
					}
					if (Game1.player.craftingRecipes.ContainsKey(___pagesOfCraftingRecipes[___currentCraftingPage][c].name))
						Game1.player.craftingRecipes[___pagesOfCraftingRecipes[___currentCraftingPage][c].name]
							+= ___pagesOfCraftingRecipes[___currentCraftingPage][c].numberProducedPerCraft;
					if (___heldItem == null || !Game1.player.couldInventoryAcceptThisItem(___heldItem))
						return false;

					// Add the machine to the user's inventory
					var prop = new Propagator(Game1.player.getTileLocation());
					Game1.player.addItemToInventoryBool(prop);
					___heldItem = null;
					return false;
				}
				catch (Exception e)
				{
					Log.E($"{ModValues.AuthorName}.{ModValues.PackageName} failed in"
					      + $" {nameof(CraftingPage_ClickCraftingRecipe)}.{nameof(Prefix)}"
					      + $"\n{e}");
					return true;
				}
			}
		}

		public class CraftingRecipe_CreateItem
		{
			internal static void Postfix(CraftingRecipe __instance, Item __result)
			{
				// Intercept machine crafts with a Propagator subclass,
				// rather than a generic nonfunctional craftable
				if (__instance.name == ModValues.PropagatorInternalName)
					__result = new Propagator(Game1.player.getTileLocation());
			}
		}
	}
}
