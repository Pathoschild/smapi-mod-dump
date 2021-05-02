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
using StardewValley;
using System;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class ObjectGetMinutesForCrystalariumPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(SObject), name: "getMinutesForCrystalarium"),
				postfix: new HarmonyMethod(GetType(), nameof(ObjectGetMinutesForCrystalariumPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch to speed up crystalarium processing time for each Gemologist.</summary>
		private static void ObjectGetMinutesForCrystalariumPostfix(ref SObject __instance, ref int __result)
		{
			try
			{
				var owner = Game1.getFarmer(__instance.owner.Value);
				if (Utility.SpecificPlayerHasProfession("Gemologist", owner)) __result = (int)(__result * 0.75);
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(ObjectGetMinutesForCrystalariumPostfix)}:\n{ex}");
			}
		}

		#endregion harmony patches
	}
}