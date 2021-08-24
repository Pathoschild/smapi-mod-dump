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
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using StardewValley.Projectiles;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class ProjectileBehaviorOnCollisionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal ProjectileBehaviorOnCollisionPatch()
		{
			Original = typeof(Projectile).MethodNamed(name: "behaviorOnCollision");
			Postfix = new HarmonyMethod(GetType(), nameof(ProjectileBehaviorOnCollisionPostfix));
		}

		#region harmony patches

		/// <summary>Patch for Rascal chance to recover ammunition.</summary>
		[HarmonyPostfix]
		private static void ProjectileBehaviorOnCollisionPostfix(Projectile __instance, NetInt ___currentTileSheetIndex, NetPosition ___position, NetCharacterRef ___theOneWhoFiredMe, GameLocation location)
		{
			try
			{
				if (__instance is not BasicProjectile || ModEntry.IsSuperModeActive) return;

				var firer = ___theOneWhoFiredMe.Get(location) is Farmer farmer ? farmer : Game1.player;
				if (!firer.HasProfession("Rascal")) return;

				if (Util.Objects.IsMineralAmmunition(___currentTileSheetIndex.Value) && Game1.random.NextDouble() < 0.6
				|| ___currentTileSheetIndex.Value == SObject.wood + 1 && Game1.random.NextDouble() < 0.3)
					location.debris.Add(new Debris(___currentTileSheetIndex.Value - 1, new Vector2((int)___position.X, (int)___position.Y), firer.getStandingPosition()));
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}