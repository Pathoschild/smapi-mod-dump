/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/BlueberryMushroomMachine
**
*************************************************/

using HarmonyLib; // el diavolo
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace BlueberryMushroomMachine
{
	internal class HarmonyPatches
	{
		public static void Apply(string uniqueID)
		{
			var harmony = new Harmony(uniqueID);

			harmony.Patch(
				original: AccessTools.Method(typeof(CraftingRecipe), "createItem"),
				postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CraftingRecipe_CreateItem_Postfix)));
			harmony.Patch(
				original: AccessTools.Method(typeof(CraftingPage), "performHoverAction"),
				postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CraftingPage_HoverAction_Postfix)));
			harmony.Patch(
				original: AccessTools.Method(typeof(CraftingPage), "clickCraftingRecipe"),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CraftingPage_ClickCraftingRecipe_Prefix)));
		}

		internal static void CraftingRecipe_CreateItem_Postfix(CraftingRecipe __instance, ref Item __result)
		{
			// Intercept machine crafts with a Propagator subclass,
			// rather than a generic nonfunctional craftable
			if (__instance.name == ModValues.PropagatorInternalName)
				__result = new Propagator(Game1.player.getTileLocation());
		}

		internal static void CraftingPage_HoverAction_Postfix(CraftingRecipe ___hoverRecipe)
		{
			// Add display name in crafting pages
			if (___hoverRecipe == null)
				return;
			if (___hoverRecipe.name.Equals(ModValues.PropagatorInternalName))
				___hoverRecipe.DisplayName = new Propagator().DisplayName;
		}

		internal static bool CraftingPage_ClickCraftingRecipe_Prefix(
			List<Dictionary<ClickableTextureComponent, CraftingRecipe>> ___pagesOfCraftingRecipes, int ___currentCraftingPage, ref Item ___heldItem,
			ClickableTextureComponent c, bool playSound = true)
		{
			try
			{
                var recipe = ___pagesOfCraftingRecipes[___currentCraftingPage][c];

                // Fetch an instance of any clicked-on craftable in the crafting menu
                var tempItem = recipe.createItem();

				// Fall through the prefix for any craftables other than the Propagator
				if (!tempItem.Name.Equals(ModValues.PropagatorInternalName))
					return true;

				// Behaviours as from base method
				if (___heldItem == null)
				{
					recipe.consumeIngredients(null);
					___heldItem = tempItem;
					if (playSound)
						Game1.playSound("coin");
				}
				if (Game1.player.craftingRecipes.TryGetValue( recipe.name, out int prevCount))
					Game1.player.craftingRecipes[recipe.name] = prevCount + recipe.numberProducedPerCraft;
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
				Log.E($"{ModValues.AuthorName}.{ModValues.PackageName} failed in {nameof(CraftingPage_ClickCraftingRecipe_Prefix)}"
					    + $"\n{e}");
				return true;
			}
		}
	}
}
