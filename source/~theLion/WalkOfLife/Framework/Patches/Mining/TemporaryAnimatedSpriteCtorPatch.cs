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

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class TemporaryAnimatedSpriteCtorPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal TemporaryAnimatedSpriteCtorPatch()
		{
			Original = typeof(TemporaryAnimatedSprite).Constructor(new[] { typeof(int), typeof(float), typeof(int), typeof(int), typeof(Vector2), typeof(bool), typeof(bool), typeof(GameLocation), typeof(Farmer) });
			Postfix = new HarmonyMethod(GetType(), nameof(TemporaryAnimatedSpriteCtorPostfix));
		}

		#region harmony patches

		/// <summary>Patch to increase Demolitionist bomb radius.</summary>
		[HarmonyPostfix]
		private static void TemporaryAnimatedSpriteCtorPostfix(ref TemporaryAnimatedSprite __instance, Farmer owner)
		{
			try
			{
				if (owner.HasProfession("Demolitionist")) ++__instance.bombRadius;
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}