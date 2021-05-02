/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods/-/tree/master/WalkOfLife
**
*************************************************/

using Harmony;
using StardewValley.TerrainFeatures;
using System;

namespace TheLion.AwesomeProfessions
{
	internal class FruitTreeDayUpdatePatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(FruitTree), nameof(FruitTree.dayUpdate)),
				postfix: new HarmonyMethod(GetType(), nameof(FruitTreeDayUpdatePostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch to increase Abrorist fruit tree growth speed.</summary>
		private static void FruitTreeDayUpdatePostfix(ref FruitTree __instance)
		{
			try
			{
				if (Utility.AnyPlayerHasProfession("Arborist", out _) && __instance.daysUntilMature.Value % 4 == 0)
					--__instance.daysUntilMature.Value;
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(FruitTreeDayUpdatePostfix)}:\n{ex}");
			}
		}

		#endregion harmony patches
	}
}