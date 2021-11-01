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
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using StardewValley.Projectiles;
using TheLion.Stardew.Common.Extensions;
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
			Original = typeof(Projectile).MethodNamed("behaviorOnCollision");
			Postfix = new(GetType(), nameof(ProjectileBehaviorOnCollisionPostfix));
		}

		#region harmony patches

		/// <summary>Patch for Rascal chance to recover ammunition.</summary>
		[HarmonyPostfix]
		private static void ProjectileBehaviorOnCollisionPostfix(Projectile __instance, NetInt ___currentTileSheetIndex,
			NetPosition ___position, NetCharacterRef ___theOneWhoFiredMe, GameLocation location)
		{
			try
			{
				if (__instance is not BasicProjectile || ModEntry.IsSuperModeActive) return;

				var firer = ___theOneWhoFiredMe.Get(location) is Farmer farmer ? farmer : Game1.player;
				if (!firer.HasProfession("Rascal")) return;

				if ((___currentTileSheetIndex.Value - 1).AnyOf(SObject.copper, SObject.iron, SObject.gold,
					    SObject.iridium, SObject.stone) && Game1.random.NextDouble() < 0.6
				    || ___currentTileSheetIndex.Value == SObject.wood + 1 && Game1.random.NextDouble() < 0.3)
					location.debris.Add(new(___currentTileSheetIndex.Value - 1,
						new((int) ___position.X, (int) ___position.Y), firer.getStandingPosition()));
			}
			catch (Exception ex)
			{
				Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}