/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;

namespace TheLion.AwesomeProfessions
{
	internal class CraftingRecipeCtorPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal CraftingRecipeCtorPatch() { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Constructor(typeof(CraftingRecipe), new Type[] { typeof(string), typeof(bool) }),
				postfix: new HarmonyMethod(GetType(), nameof(CraftingRecipeCtorPostfix))
			);
		}

		#region harmony patches
		/// <summary>Patch for cheaper crafting recipes for Blaster and Tapper.</summary>
		protected static void CraftingRecipeCtorPostfix(ref CraftingRecipe __instance)
		{
			if (__instance.name.Equals("Tapper") && Utility.LocalPlayerHasProfession("tapper"))
			{
				__instance.recipeList = new Dictionary<int, int>
				{
					{ 388, 25 },	// wood
					{ 334, 1 }		// copper bar
				};
			}
			else if (__instance.name.Contains("Bomb") && Utility.LocalPlayerHasProfession("blaster"))
			{
				__instance.recipeList = __instance.name switch
				{
					"Cherry Bomb" => new Dictionary<int, int>
					{
						{ 378, 2 },	// copper ore
						{ 382, 1 }	// coal
					},
					"Bomb" => new Dictionary<int, int>
					{
						{ 380, 2 },	// iron ore
						{ 382, 1 }	// coal
					},
					"Mega Bomb" => new Dictionary<int, int>
					{
						{ 384, 2 },	// gold ore
						{ 382, 1 }	// coal
					}
				};
			}
		}
		#endregion harmony patches
	}
}
