using Netcode;
using StardewValley;
using System;
using System.Linq;

namespace BattleRoyale
{
	class GameLocation_damageMonster_Patcher : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(GameLocation), "damageMonster",
			new Type[] { typeof(Microsoft.Xna.Framework.Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int), typeof(float), typeof(float), typeof(bool), typeof(Farmer) });

		public static void Prefix(Microsoft.Xna.Framework.Rectangle areaOfEffect,
										ref int minDamage,
										ref int maxDamage,
										bool isBomb,
										float knockBackModifier,
										int addedPrecision,
										float critChance,
										float critMultiplier,
										bool triggerMonsterInvincibleTimer,
										Farmer who,
										NetCollection<NPC> ___characters, GameLocation __instance)
		{
			//Console.WriteLine($"{who.Name} attacked with {minDamage} to {maxDamage} damage");
			
			var players = Game1.getOnlineFarmers().Where(x => x != who && x.currentLocation == who.currentLocation);

			foreach (Farmer player in players)
			{
				if (player.GetBoundingBox().Intersects(areaOfEffect))
				{
					if (maxDamage >= 0)
					{
						int damageAmount = Game1.random.Next(minDamage, maxDamage + 1);

						bool crit = false;
						if (who != null && Game1.random.NextDouble() < (double)(critChance + (float)who.LuckLevel * (critChance / 40f)))//Change?
						{
							crit = true;
							__instance.playSound("crit");
						}
						damageAmount = (crit ? ((int)((float)damageAmount * critMultiplier)) : damageAmount);
						damageAmount = Math.Max(1, damageAmount + ((who != null) ? (who.attack * 3) : 0));

						Console.WriteLine($"Calculated {damageAmount} damage to deal to {player.Name}");

						NetworkUtility.SendDamageToPlayer(damageAmount, player, Game1.player.UniqueMultiplayerID);
					}
				}
			}
		}
	}
}

