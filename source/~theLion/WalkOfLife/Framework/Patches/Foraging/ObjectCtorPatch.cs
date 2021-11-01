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
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class ObjectCtorPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal ObjectCtorPatch()
		{
			Original = typeof(SObject).Constructor(new[]
				{typeof(Vector2), typeof(int), typeof(string), typeof(bool), typeof(bool), typeof(bool), typeof(bool)});
			Postfix = new(GetType(), nameof(ObjectCtorPostfix));
		}

		#region harmony patches

		/// <summary>Patch for Ecologist wild berry recovery.</summary>
		[HarmonyPostfix]
		private static void ObjectCtorPostfix(ref SObject __instance)
		{
			try
			{
				var owner = Game1.getFarmer(__instance.owner.Value);
				if (__instance.IsWildBerry() && owner.HasProfession("Ecologist"))
					__instance.Edibility = (int)(__instance.Edibility * 1.5f);
			}
			catch (Exception ex)
			{
				Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}