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
using EnhancedSlingshots.Framework.Enchantments;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;
using System.Linq;
using System.Reflection;
using SObject = StardewValley.Object;

namespace EnhancedSlingshots.Framework.Patch
{
	[HarmonyPatch(typeof(Slingshot))]
	public static class SlingshotPatchs
    {
        private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

		private static AccessTools.FieldRef<Slingshot, NetPoint> aimPos = AccessTools.FieldRefAccess<Slingshot, NetPoint>("aimPos");
		private static AccessTools.FieldRef<Slingshot, bool> canPlaySound = AccessTools.FieldRefAccess<Slingshot, bool>("canPlaySound");
		private static MethodInfo updateAimPosMethod = typeof(Slingshot).GetMethod("updateAimPos", BindingFlags.NonPublic | BindingFlags.Instance);

		[HarmonyPostfix]
		[HarmonyPatch(nameof(Slingshot.canThisBeAttached))]
		public static void canThisBeAttached_Postfix(ref bool __result, SObject o)
		{
			if (o == null || ModEntry.Instance.config.ItemsThatCanBeUsedAsAmmo.Contains(o.ParentSheetIndex))
				__result = true;
		}

		[HarmonyPostfix]
		[HarmonyPatch(nameof(Slingshot.GetAutoFireRate))]
		public static void GetAutoFireRate_Postfix(Slingshot __instance, ref float __result)
		{
			if (__instance.hasEnchantmentOfType<AutomatedEnchantment>())
				__result = ModEntry.Instance.config.SlingshotAutoFireRate;
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
			if (__instance.attachments[0] != null)
			{
				updateAimPosMethod.Invoke(__instance, null);
				Vector2 mousePos = aimPos(__instance).Value.ToVector2();				
				int backArmDistance = __instance.GetBackArmDistance(who);
				Vector2 shoot_origin = __instance.GetShootOrigin(who);
				Vector2 velocity = Utility.getVelocityTowardPoint(__instance.GetShootOrigin(who), __instance.AdjustForHeight(mousePos), (15 + Game1.random.Next(4, 6)) * (1f + who.weaponSpeedModifier));
				if (backArmDistance > 4 && !canPlaySound(__instance))
				{
					SObject ammunition = (SObject)__instance.attachments[0].getOne();
					__instance.attachments[0].Stack--;

					if (__instance.hasEnchantmentOfType<Enchantments.PreservingEnchantment>() && Game1.random.NextDouble() < ModEntry.Instance.config.PreciseEnchantment_Damage)
						__instance.attachments[0].Stack++;

					if (__instance.attachments[0].Stack <= 0)
						__instance.attachments[0] = null;
					
					BasicProjectile.onCollisionBehavior collisionBehavior = null;
					string collisionSound = "hammer";

					float damageMod = 1f;				
					if (__instance.InitialParentTileIndex == Slingshot.masterSlingshot)
						damageMod = 2f;
					else if (__instance.InitialParentTileIndex == Slingshot.galaxySlingshot)
						damageMod = 3f; //new galaxy slingshot damage						
					else if (__instance.InitialParentTileIndex == ModEntry.Instance.config.InfinitySlingshotId) //Infinity Sling
						damageMod = 4f;

					int damage = 1;
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
						case 909:
							damage = 75;
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
						velocity *= -1f;
					
					if (__instance.hasEnchantmentOfType<SwiftEnchantment>())
						velocity *= ModEntry.Instance.config.SwiftEnchantment_TimesFaster;					

					BasicProjectile projectile = new BasicProjectile(
						(int)(damageMod * (damage + Game1.random.Next(-(damage / 2), damage + 2)) * (1f + who.attackIncreaseModifier)),
						ammunition.ParentSheetIndex,
						0,
						0,
						(float)(Math.PI / (64f + Game1.random.Next(-63, 64))),
						0f - velocity.X,
						0f - velocity.Y,
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

		[HarmonyPostfix]
		[HarmonyPatch(nameof(Slingshot.PerformFire))]
		public static void PerformFire_Postfix(Slingshot __instance, GameLocation location, Farmer who)
        {			
            if (__instance.hasEnchantmentOfType<ExpertEnchantment>())
            {			
				if (__instance.attachments[0]?.Stack > 0 && __instance.attachments[0].Stack != __instance.attachments[0].maximumStackSize())
                {
					Item item = who.Items.FirstOrDefault(item => item?.ParentSheetIndex == __instance.attachments[0].ParentSheetIndex);
					if(item != null && item.Stack > 0)
                    {
						__instance.attachments[0].Stack++;
						item.Stack--;
						if (item.Stack <= 0) item = null;
						return;
					}
                }
                else
                {
					foreach(Item item in who.Items)
                    {
						if (item == null || item is not SObject) continue;

                        if (__instance.canThisBeAttached(item as SObject))
                        {
							__instance.attachments[0] = (SObject)item;
							who.Items[who.Items.IndexOf(item)] = null;
							return;
						}
					}                  
                }
            }                   
        }
    }
}
