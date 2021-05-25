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
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class ObjectCtorPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Constructor(typeof(SObject), new[] { typeof(Vector2), typeof(int), typeof(string), typeof(bool), typeof(bool), typeof(bool), typeof(bool) }),
				postfix: new HarmonyMethod(GetType(), nameof(ObjectCtorPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch for Ecologist wild berry recovery.</summary>
		private static void ObjectCtorPostfix(ref SObject __instance)
		{
			try
			{
				var owner = Game1.getFarmer(__instance.owner.Value);
				if (Utility.IsWildBerry(__instance) && Utility.SpecificPlayerHasProfession("Ecologist", owner))
					__instance.Edibility = (int)(__instance.Edibility * 1.5f);
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(ObjectCtorPostfix)}:\n{ex}");
			}
		}

		#endregion harmony patches
	}
}