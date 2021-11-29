/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class ObjectCtorPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal ObjectCtorPatch()
		{
			Original = RequireConstructor<SObject>(typeof(Vector2), typeof(int), typeof(string), typeof(bool),
				typeof(bool), typeof(bool), typeof(bool));
			Postfix = new(GetType(), nameof(ObjectCtorPostfix));
		}

		#region harmony patches

		/// <summary>Patch for Ecologist wild berry recovery.</summary>
		[HarmonyPostfix]
		private static void ObjectCtorPostfix(ref SObject __instance)
		{
			var owner = Game1.getFarmer(__instance.owner.Value);
			if (__instance.IsWildBerry() && owner.HasProfession("Ecologist"))
				__instance.Edibility = (int) (__instance.Edibility * 1.5f);
		}

		#endregion harmony patches
	}
}