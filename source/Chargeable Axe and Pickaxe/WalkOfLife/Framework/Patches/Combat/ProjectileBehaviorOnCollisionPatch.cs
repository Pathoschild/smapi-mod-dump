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
using Netcode;
using StardewValley;
using StardewValley.Projectiles;
using StardewValley.Network;

namespace TheLion.AwesomeProfessions
{
	internal class ProjectileBehaviorOnCollisionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal ProjectileBehaviorOnCollisionPatch() { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Projectile), name: "behaviorOnCollision"),
				postfix: new HarmonyMethod(GetType(), nameof(ProjectileBehaviorOnCollisionPostfix))
			);
		}

		#region harmony patches
		/// <summary>Patch for Rascal chance to recover ammunition.</summary>
		protected static void ProjectileBehaviorOnCollisionPostfix(ref Projectile __instance, ref NetInt ___currentTileSheetIndex, ref NetPosition ___position, ref NetCharacterRef ___theOneWhoFiredMe, GameLocation location)
		{
			if (!(__instance is BasicProjectile)) return;

			Farmer who = ___theOneWhoFiredMe.Get(location) is Farmer ? ___theOneWhoFiredMe.Get(location) as Farmer : Game1.player;
			if (!Utility.SpecificPlayerHasProfession("rascal", who)) return;

			if (Game1.random.NextDouble() < 0.6 && Utility.IsMineralAmmunition(___currentTileSheetIndex.Value))
				location.debris.Add(new Debris(___currentTileSheetIndex.Value - 1, new Vector2((int)___position.X, (int)___position.Y), who.getStandingPosition()));
		}
		#endregion harmony patches
	}
}
