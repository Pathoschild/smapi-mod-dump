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
using Netcode;
using StardewValley;
using StardewValley.Projectiles;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class BasicProjectileCtorPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal BasicProjectileCtorPatch()
		{
			Original = RequireConstructor<BasicProjectile>(typeof(int), typeof(int), typeof(int), typeof(int),
				typeof(float), typeof(float), typeof(float), typeof(Vector2), typeof(string), typeof(string),
				typeof(bool), typeof(bool), typeof(GameLocation), typeof(Character), typeof(bool),
				typeof(BasicProjectile.onCollisionBehavior));
			Postfix = new(GetType(), nameof(BasicProjectileCtorPostfix));
		}

		#region harmony patches

		/// <summary>Patch for all classes to eliminate travel grace period.</summary>
		[HarmonyPostfix]
		private static void BasicProjectileCtorPostfix(BasicProjectile __instance, ref NetInt ___bouncesLeft,
			bool damagesMonsters, Character firer)
		{
			if (!damagesMonsters || firer is not Farmer farmer) return;
			__instance.ignoreTravelGracePeriod.Value = true;

			if (!farmer.HasProfession("Rascal") || !ModEntry.Config.Modkey.IsDown()) return;
			++___bouncesLeft.Value;
			__instance.damageToFarmer.Value = (int) (__instance.damageToFarmer.Value * 0.6);
		}

		#endregion harmony patches
	}
}