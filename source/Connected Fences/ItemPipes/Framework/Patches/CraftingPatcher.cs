/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley;
using StardewValley.Objects;
using StardewModdingAPI;
using ItemPipes.Framework.Model;
using ItemPipes.Framework.Recipes;
using ItemPipes.Framework.Util;
using Netcode;
using StardewValley.Menus;
using ItemPipes.Framework.Nodes.ObjectNodes;
using Microsoft.Xna.Framework;
using System.Reflection.Emit;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Data;

namespace ItemPipes.Framework.Patches
{
    [HarmonyPatch(typeof(CraftingPage))]
    public static class CraftingPatcher
    {

        public static void Apply(Harmony harmony)
        {
			try
			{
				harmony.Patch(
					original: AccessTools.Method(typeof(CraftingPage), "layoutRecipes"),
					prefix: new HarmonyMethod(typeof(CraftingPatcher), nameof(CraftingPatcher.CraftingPage_layoutRecipes_Prefix))
				);

				harmony.Patch(
					original: typeof(LevelUpMenu).GetMethod(nameof(LevelUpMenu.draw), new Type[] { typeof(SpriteBatch) }),
					prefix: new HarmonyMethod(typeof(CraftingPatcher), nameof(CraftingPatcher.LevelUpMenu_draw_Prefix))
				);
			}
			catch (Exception ex)
			{
				Printer.Info($"Failed to add crafting patches: {ex}");
			}
        }

		private static bool LevelUpMenu_draw_Prefix(LevelUpMenu __instance, SpriteBatch b)
		{
			List<CraftingRecipe> Recipes = Helper.GetHelper().Reflection.GetField<List<CraftingRecipe>>(__instance, "newCraftingRecipes").GetValue();
			foreach(CraftingRecipe recipe in Recipes.ToList())
            {
				if(IsModdedRecipe(recipe.name))
                {
					Recipes.Remove(recipe);
					Recipes.Add(new CustomCraftingRecipe(recipe.name, false));
                }
            }
			Helper.GetHelper().Reflection.GetField<List<CraftingRecipe>>(__instance, "newCraftingRecipes").SetValue(Recipes);
			return true;
		}

		private static bool CraftingPage_layoutRecipes_Prefix(CraftingPage __instance, List<string> playerRecipes)
        {
			int craftingPageX = __instance.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth - 16;
			int spaceBetweenCraftingIcons = 8;
			Dictionary<ClickableTextureComponent, CraftingRecipe> currentPage = Helper.GetHelper().Reflection.GetMethod(__instance, "createNewPage").Invoke<Dictionary<ClickableTextureComponent, CraftingRecipe>>();
			int x = 0;
			int y = 0;
			int i = 0;
			ClickableTextureComponent[,] pageLayout = Helper.GetHelper().Reflection.GetMethod(__instance, "createNewPageLayout").Invoke<ClickableTextureComponent[,]>();
			List<ClickableTextureComponent[,]> pageLayouts = new List<ClickableTextureComponent[,]>();
			pageLayouts.Add(pageLayout);
			foreach (string playerRecipe in playerRecipes)
			{
				i++;
				CraftingRecipe recipe;
				if (IsModdedRecipe(playerRecipe))
				{
					recipe = new CustomCraftingRecipe(playerRecipe, false);
				}
				else
                {
					recipe = new CraftingRecipe(playerRecipe, false);
				}
				while (Helper.GetHelper().Reflection.GetMethod(__instance, "spaceOccupied").Invoke<bool>(pageLayout, x, y, recipe))
					{
						x++;
					if (x >= 10)
					{
						x = 0;
						y++;
						if (y >= 4)
						{
							currentPage = Helper.GetHelper().Reflection.GetMethod(__instance,
								"createNewPage").Invoke<Dictionary<ClickableTextureComponent, CraftingRecipe>>();
							pageLayout = Helper.GetHelper().Reflection.GetMethod(__instance,
								"createNewPageLayout").Invoke<ClickableTextureComponent[,]>();
							pageLayouts.Add(pageLayout);
							x = 0;
							y = 0;
						}
					}
				}
				int id = 200 + i;
				ClickableTextureComponent component;
				if (IsModdedRecipe(playerRecipe))
				{
					recipe = new CustomCraftingRecipe(playerRecipe, false);
					component = new ClickableTextureComponent
					("",
					new Rectangle(craftingPageX + x * (64 + spaceBetweenCraftingIcons), Helper.GetHelper().Reflection.GetMethod(__instance, "craftingPageY").Invoke<int>() + y * 72, 64, recipe.bigCraftable ? 128 : 64),
					null,
					(false && !Game1.player.cookingRecipes.ContainsKey(recipe.name)) ? "ghosted" : "",
					DataAccess.GetDataAccess().Sprites[Utilities.GetIDName(playerRecipe)+"_Item"],
					recipe.bigCraftable ? new Rectangle(0, 0, 16, 32) : new Rectangle(0, 0, 16, 16),
					4f)
					{
						myID = id,
						rightNeighborID = -99998,
						leftNeighborID = -99998,
						upNeighborID = -99998,
						downNeighborID = -99998,
						fullyImmutable = true,
						region = 8000
					};
				}
				else
				{
					component = new ClickableTextureComponent
					("",
					new Rectangle(craftingPageX + x * (64 + spaceBetweenCraftingIcons), Helper.GetHelper().Reflection.GetMethod(__instance, "craftingPageY").Invoke<int>() + y * 72, 64, recipe.bigCraftable ? 128 : 64),
					null,
					(false && !Game1.player.cookingRecipes.ContainsKey(recipe.name)) ? "ghosted" : "",
					recipe.bigCraftable ? Game1.bigCraftableSpriteSheet : Game1.objectSpriteSheet,
					recipe.bigCraftable ? Game1.getArbitrarySourceRect(Game1.bigCraftableSpriteSheet, 16, 32, recipe.getIndexOfMenuView()) : Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, recipe.getIndexOfMenuView(), 16, 16),
					4f)
					{
						myID = id,
						rightNeighborID = -99998,
						leftNeighborID = -99998,
						upNeighborID = -99998,
						downNeighborID = -99998,
						fullyImmutable = true,
						region = 8000
					};
				}
				currentPage.Add(component, recipe);
				pageLayout[x, y] = component;
				if (recipe.bigCraftable)
				{
					pageLayout[x, y + 1] = component;
				}
			}
			return false;
		}

		public static bool IsModdedRecipe(string playerRecipe)
        {
			DataAccess DataAccess = DataAccess.GetDataAccess();
			if (DataAccess.Recipes.Keys.Contains(playerRecipe))
            {
				return true;
			}
			return false;
        }

	}
}

