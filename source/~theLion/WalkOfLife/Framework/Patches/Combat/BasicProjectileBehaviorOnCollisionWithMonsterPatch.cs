/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using StardewValley.Projectiles;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class BasicProjectileBehaviorOnCollisionWithMonsterPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal BasicProjectileBehaviorOnCollisionWithMonsterPatch()
		{
			Original = RequireMethod<BasicProjectile>(nameof(BasicProjectile.behaviorOnCollisionWithMonster));
			Prefix = new(GetType(), nameof(BasicProjectileBehaviorOnCollisionWithMonsterPrefix));
		}

		#region harmony patches

		/// <summary>Patch for Rascal slingshot damage increase with travel time + Desperado peirce shot.</summary>
		[HarmonyPrefix]
		private static bool BasicProjectileBehaviorOnCollisionWithMonsterPrefix(BasicProjectile __instance,
			ref NetBool ___damagesMonsters, NetCharacterRef ___theOneWhoFiredMe, int ___travelTime, ref NPC n,
			GameLocation location)
		{
			try
			{
				if (!___damagesMonsters.Value) return false; // don't run original logic

				if (!n.IsMonster) return true; // run original logic

				var firer = ___theOneWhoFiredMe.Get(location) is Farmer farmer ? farmer : Game1.player;
				if (!firer.HasProfession("Rascal")) return true; // run original logic

				if (Game1.random.NextDouble() < (Utility.Professions.GetDesperadoBulletPower() - 1) / 2)
					ModState.DidBulletPierceEnemy = true;
				else
					ModEntry.ModHelper.Reflection.GetMethod(__instance, "explosionAnimation")?.Invoke(location);

				var damageToMonster = (int) (__instance.damageToFarmer.Value *
				                             Utility.Professions.GetRascalBonusDamageForTravelTime(___travelTime));

				var knockbackModifier =
					firer.IsLocalPlayer && ModState.SuperModeIndex == Utility.Professions.IndexOf("Desperado")
						? Utility.Professions.GetDesperadoBulletPower()
						: 1f;
				location.damageMonster(n.GetBoundingBox(), damageToMonster, damageToMonster + 1, false,
					knockbackModifier, 0, 0f, 1f, false, firer);

				return false; // don't run original logic
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}