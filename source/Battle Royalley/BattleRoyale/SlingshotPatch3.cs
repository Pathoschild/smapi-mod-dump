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
	class SlingshotPatch3 : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Projectile), "isColliding");
		
		public static bool Postfix(bool __result, Projectile __instance, GameLocation location)
		{
			bool damagesMonsters = ModEntry.BRGame.ModHelper.Reflection.GetField<NetBool>(__instance, "damagesMonsters").GetValue().Value;

			if (!__result && (!(__instance is BasicProjectile bp) || bp.damageToFarmer.Value < 20))
			{
				Rectangle r = __instance.getBoundingBox();
				foreach (Farmer farmer in location.farmers.Where(x => x != null))
				{
					var bounds = SlingshotPatch5.GetFarmerBounds(farmer);


					if (bounds.Intersects(r))
					{
						Console.WriteLine("Collide with farmer");
						//__result = true;
						return true;
					}
				}
			}

			if (!__result)
			{
				// 	x / Game1.tileSize, y / Game1.tileSize
				var center = __instance.getBoundingBox().Center;
				var centerTile = new Vector2((int)(center.X / Game1.tileSize), (int)(center.Y / Game1.tileSize));
				if (location.objects.TryGetValue(centerTile, out StardewValley.Object obj) && obj.Name.ToLower().Contains("fence"))
				{
					//__result = true;
					return true;
				}
			}

			return __result;
		}

		}
	}
