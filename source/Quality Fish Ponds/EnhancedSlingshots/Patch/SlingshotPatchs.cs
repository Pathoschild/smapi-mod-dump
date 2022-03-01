/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/YTSC/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using EnhancedSlingshots.Enchantments;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EnhancedSlingshots.Patch
{
	[HarmonyPatch(typeof(Slingshot))]
	public static class SlingshotPatchs
    {
        private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

		[HarmonyPostfix]
		[HarmonyPatch(nameof(Slingshot.GetAutoFireRate))]
		public static void GetAutoFireRate_Postfix(Slingshot __instance, ref float __result)
		{
			if (__instance.hasEnchantmentOfType<AutomatedEnchantment>())
				__result = 0.5f;
		}

		[HarmonyPostfix]
		[HarmonyPatch(nameof(Slingshot.CanAutoFire))]
		public static void CanAutoFire_Postfix(Slingshot __instance, ref bool __result)
        {			
			if (__instance.hasEnchantmentOfType<AutomatedEnchantment>())
				__result = true;
		}

		[HarmonyPrefix]
		[HarmonyPatch(nameof(Slingshot.PerformFire))]
		public static bool PerformFire_Prefix(Slingshot __instance, ref GameLocation location, ref Farmer who)
        {	
			if (__instance != null &&
			    (__instance.hasEnchantmentOfType<Enchantments.PreservingEnchantment>() ||				
				 __instance.hasEnchantmentOfType<SwiftEnchantment>()))
            {
                AccessTools.FieldRef<Slingshot, NetPoint> aimPos = AccessTools.FieldRefAccess<Slingshot, NetPoint>("aimPos");
                AccessTools.FieldRef<Slingshot, bool> canPlaySound = AccessTools.FieldRefAccess<Slingshot, bool>("canPlaySound");
				var updateAimPosMethod = typeof(Slingshot).GetMethod("updateAimPos", BindingFlags.NonPublic | BindingFlags.Instance);
				
				if (__instance.attachments[0] != null)
				{
					updateAimPosMethod.Invoke(__instance, null);
					int mouseX = aimPos(__instance).X;
					int mouseY = aimPos(__instance).Y;
					int backArmDistance = __instance.GetBackArmDistance(who);
                    Vector2 shoot_origin = __instance.GetShootOrigin(who);
                    Vector2 v = Utility.getVelocityTowardPoint(__instance.GetShootOrigin(who), __instance.AdjustForHeight(new Vector2(mouseX, mouseY)), (15 + Game1.random.Next(4, 6)) * (1f + who.weaponSpeedModifier));
					if (backArmDistance > 4 && !canPlaySound(__instance))
					{
                        StardewValley.Object ammunition = (StardewValley.Object)__instance.attachments[0].getOne();
						if (__instance.hasEnchantmentOfType<Enchantments.PreservingEnchantment>() && Game1.random.NextDouble() < 0.5)
							__instance.attachments[0].Stack--;

						if (__instance.attachments[0].Stack <= 0)						
							__instance.attachments[0] = null;
						
						int damage = 1;
                        BasicProjectile.onCollisionBehavior collisionBehavior = null;
						string collisionSound = "hammer";

						float damageMod = 1f;
						if (__instance.InitialParentTileIndex == 33)						
							damageMod = 2f;						
						else if (__instance.InitialParentTileIndex == 34)
							damageMod = 3f; //new damage						
						else if (__instance.InitialParentTileIndex == ModEntry.Instance.config.InfinitySlingshotId) //Infinity Sling
                        	damageMod = 4f;
						
						switch (ammunition.ParentSheetIndex)
						{
							case 388:
								damage = 2;
								ammunition.ParentSheetIndex++;
								break;
							case 390:
								damage = 5;
								ammunition.ParentSheetIndex++;
								break;
							case 378:
								damage = 10;
								ammunition.ParentSheetIndex++;
								break;
							case 380:
								damage = 20;
								ammunition.ParentSheetIndex++;
								break;
							case 384:
								damage = 30;
								ammunition.ParentSheetIndex++;
								break;
							case 382:
								damage = 15;
								ammunition.ParentSheetIndex++;
								break;
							case 386:
								damage = 50;
								ammunition.ParentSheetIndex++;
								break;
							case 441:
								damage = 20;
								collisionBehavior = BasicProjectile.explodeOnImpact;								
								collisionSound = "explosion";
								break;
						}

						if (ammunition.Category == -5)						
							collisionSound = "slimedead";
						
						if (!Game1.options.useLegacySlingshotFiring)
						{
							v.X *= -1f;
							v.Y *= -1f;
						}

						if (__instance.hasEnchantmentOfType<SwiftEnchantment>())
						{
							v.X *= 2;
							v.Y *= 2;
						}

                        BasicProjectile projectile = new BasicProjectile(
							(int)(damageMod * (damage + Game1.random.Next(-(damage / 2), damage + 2)) * (1f + who.attackIncreaseModifier)),
							ammunition.ParentSheetIndex,
							0,
							0,
							(float)(Math.PI / (64f + Game1.random.Next(-63, 64))),
							0f - v.X,
							0f - v.Y,
							shoot_origin - new Vector2(32f, 32f),
							collisionSound,
							"",
							explode: false,
							damagesMonsters: true,
							location,
							who,
							spriteFromObjectSheet: true,
							collisionBehavior)
						{
							IgnoreLocationCollision = (Game1.currentLocation.currentEvent != null || Game1.currentMinigame != null)
						};

						location.projectiles.Add(projectile);
					}
				}
				else
				{
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Slingshot.cs.14254"));
				}
				canPlaySound(__instance) = true;
				return false;
            }
            else			
				return true;
			
        }

		[HarmonyPostfix]
		[HarmonyPatch(nameof(Slingshot.PerformFire))]
		public static void PerformFire_Postfix(Slingshot __instance, GameLocation location, Farmer who)
        {			
            if (__instance.hasEnchantmentOfType<ExpertEnchantment>())
            {			
				if (__instance.attachments[0]?.Stack > 0)
                {				
					for (int x = 0; x < who.Items?.Count; x++)
                    {						
						if (who.Items[x]?.ParentSheetIndex == __instance.attachments[0]?.ParentSheetIndex &&
							__instance.attachments[0]?.Stack != __instance.attachments[0]?.maximumStackSize())
                        {						
							__instance.attachments[0].Stack++;
                            who.Items[x].Stack--;

                            if (who.Items[x].Stack <= 0)
                                who.Items[x] = null;

							break;
                        }
                    }
                }
                else
                {					
					bool exit = false;
					foreach(Item item in who.Items)
                    {
						switch (item?.ParentSheetIndex)
						{
							case 388:
							case 390:
							case 378:
							case 380:
							case 384:
							case 382:
							case 386:
							case 441:								
								__instance.attachments[0] = (StardewValley.Object)item;
								who.Items[who.Items.IndexOf(item)] = null;
								exit = true;
								break;							
						}
						if (exit) break;
					}					
					                    
                }
            }                   
        }
    }
}
