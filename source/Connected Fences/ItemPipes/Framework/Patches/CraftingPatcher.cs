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
using Microsoft.Xna.Framework.Graphics;
using System.Data;

namespace ItemPipes.Framework.Patches
{
	public static class CraftingPatcher
    {
        public static void Apply(Harmony harmony)
        {
			try
			{
				harmony.Patch(
					original: typeof(LevelUpMenu).GetMethod(nameof(LevelUpMenu.draw), new Type[] { typeof(SpriteBatch) }),
					prefix: new HarmonyMethod(typeof(CraftingPatcher), nameof(CraftingPatcher.LevelUpMenu_draw_Prefix))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(CraftingPage), "layoutRecipes"),
					postfix: new HarmonyMethod(typeof(CraftingPatcher), nameof(CraftingPatcher.CraftingPage_layoutRecipes_Postfix))
				);
			}
			catch (Exception ex)
			{
				Printer.Error($"Failed to add crafting patches: {ex}");
			}
        }

		private static bool LevelUpMenu_draw_Prefix(LevelUpMenu __instance, SpriteBatch b)
		{
			List<CraftingRecipe> Recipes = ModEntry.helper.Reflection.GetField<List<CraftingRecipe>>(__instance, "newCraftingRecipes").GetValue();
			foreach(CraftingRecipe recipe in Recipes.ToList())
            {
				if(IsModdedRecipe(recipe.name))
                {
					Recipes.Remove(recipe);
					Recipes.Add(new CustomCraftingRecipe(recipe.name, false));
                }
            }
			ModEntry.helper.Reflection.GetField<List<CraftingRecipe>>(__instance, "newCraftingRecipes").SetValue(Recipes);
			return true;
		}

		private static void CraftingPage_layoutRecipes_Postfix(CraftingPage __instance)
		{
			foreach(Dictionary<ClickableTextureComponent, CraftingRecipe> page in __instance.pagesOfCraftingRecipes)
            {
				foreach(KeyValuePair<ClickableTextureComponent, CraftingRecipe> pair in page)
                {
					if(IsModdedRecipe(pair.Value.name))
					{
						pair.Key.texture = DataAccess.GetDataAccess().Sprites[Utilities.GetIDName(pair.Value.name) + "_item"];
						page[pair.Key] = new CustomCraftingRecipe(pair.Value.name, false);
					}
                }
			}
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

