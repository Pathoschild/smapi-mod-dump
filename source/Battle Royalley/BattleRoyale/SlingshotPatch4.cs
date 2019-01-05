using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Network;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale
{
	class SlingshotPatch4 : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(BasicProjectile), "behaviorOnCollisionWithPlayer");

		public static bool Prefix(BasicProjectile __instance, GameLocation location, Farmer player)
		{
			bool damagesMonsters = ModEntry.BRGame.ModHelper.Reflection.GetField<NetBool>(__instance, "damagesMonsters").GetValue().Value;

			Console.WriteLine($"damage, m = {damagesMonsters}");
			if (!damagesMonsters || true && SlingshotPatch5.GetFarmerBounds(player).Intersects(__instance.getBoundingBox()))//TODO: remove?
			{
				//TODO: modify slingshot damage here?
				int damage = __instance.damageToFarmer.Value;
				damage = Math.Min(damage, 10);

				if (player == Game1.player)
				{
					Console.WriteLine("sending slingshot damage to self");
					ModEntry.BRGame.TakeDamage(damage);
				}
				else
				{
					Console.WriteLine("sending slingshot damage to other player");
					NetworkUtility.SendDamageToPlayer(damage, player, Game1.player.UniqueMultiplayerID);
				}

				try
				{
					if (!HitShaker.IsPlayerFlashing(player.UniqueMultiplayerID))
					{
						var r = ModEntry.BRGame.ModHelper.Reflection;
						r.GetMethod(__instance, "explosionAnimation", false)?.Invoke(location);
					}
				}
				catch (Exception) { }
			}
			return false;
		}
	}
}
