/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using System;
using StardewModdingAPI;
using SObject = StardewValley.Object;
using HarmonyLib;

namespace CategoriesInRecipes
{
	public class ModEntry : Mod
	{
		/*********
		** Public methods
		*********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			RecipePatches.Initialize(Monitor, helper.Translation);

			var harmony = new Harmony(this.ModManifest.UniqueID);

			harmony.Patch(
				original: AccessTools.Method(typeof(StardewValley.CraftingRecipe), nameof(StardewValley.CraftingRecipe.getNameFromIndex))
					?? throw new InvalidOperationException("Can't find CraftingRecipe.getNameFromIndex to patch"),
				prefix: new HarmonyMethod(typeof(RecipePatches), nameof(RecipePatches.GetNameFromIndex_Prefix))
			);

			Monitor.Log("Harmony patched a prefix for CraftingRecipe.getNameFromIndex");

			harmony.Patch(
				original: AccessTools.Method(typeof(StardewValley.CraftingRecipe), nameof(StardewValley.CraftingRecipe.getSpriteIndexFromRawIndex))
					?? throw new InvalidOperationException("Can't find CraftingRecipe.getSpriteIndexFromRawIndex to patch"),
				prefix: new HarmonyMethod(typeof(RecipePatches), nameof(RecipePatches.GetSpriteIndexFromRawIndex_Prefix))
			);

			Monitor.Log("Harmony patched a prefix for CraftingRecipe.getSpriteIndexFromRawIndex");
		}
	}

	public class RecipePatches
	{
		private static IMonitor Monitor = null!;
		private static ITranslationHelper Translator = null!;


		public static void Initialize(IMonitor monitor, ITranslationHelper translation)
		{
			Monitor = monitor;
			Translator = translation;
		}


		public static bool GetNameFromIndex_Prefix(ref int index, ref string __result)
		{
			try
			{
				switch (index)
				{
					case SObject.VegetableCategory:
						index = -3;
						return true;
					case SObject.FruitsCategory:
						__result = Translator.Get("category.fruit");
						return false;
					case SObject.GreensCategory:
						index = -1;
						return true;
					default:
						return true;
				}
			}
			catch (Exception e)
			{
				Monitor.Log("Mod failed at prefixing CraftingRecipe.getNameFromIndex", LogLevel.Error);
				Monitor.Log(e.ToString(), LogLevel.Error);
				return true;
			}
		}


		public static bool GetSpriteIndexFromRawIndex_Prefix(ref int index, ref int __result)
		{
			try
			{
				switch (index)
				{
					case SObject.VegetableCategory:
						index = -3;
						return true;
					case SObject.FruitsCategory:
						// Use Apple's parent sheet index
						__result = 613;
						return false;
					case SObject.GreensCategory:
						index = -1;
						return true;
					default:
						return true;
				}
			}
			catch (Exception e)
			{
				Monitor.Log("Mod failed at prefixing CraftingRecipe.getSpriteIndexFromRawIndex", LogLevel.Error);
				Monitor.Log(e.ToString(), LogLevel.Error);
				return true;
			}
		}
	}
}
