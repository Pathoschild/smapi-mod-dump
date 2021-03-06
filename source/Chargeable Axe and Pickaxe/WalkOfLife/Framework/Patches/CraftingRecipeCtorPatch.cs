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
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class CraftingRecipeCtorPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal CraftingRecipeCtorPatch(ModConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Constructor(typeof(CraftingRecipe), new Type[] { typeof(string), typeof(bool) }),
				postfix: new HarmonyMethod(GetType(), nameof(CrafingRecipeCtorPostfix))
			);
		}

		/// <summary>Patch for cheaper tapper recipe for Tapper.</summary>
		protected static void CrafingRecipeCtorPostfix(ref CraftingRecipe __instance)
		{
			if (__instance.name.Equals("Tapper") && Utils.PlayerHasProfession("tapper"))
			{
				__instance.recipeList = new Dictionary<int, int>
				{
					{ 388, 25 },
					{ 334, 1 }
				};
			}
		}
	}
}
