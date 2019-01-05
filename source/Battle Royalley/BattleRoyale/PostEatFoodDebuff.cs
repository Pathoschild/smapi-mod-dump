using StardewValley;
using System;

namespace BattleRoyale
{
	class PostEatFoodDebuff1 : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Farmer), "doneEating");

		internal static DateTime? timeSinceLastEat = null;
		internal static int debuffMilliseconds = 3500;
		internal static float speedMultiplier = 0.6f;
		private static readonly int minimumHealthBonusForDebuff = 50;

		public static void Prefix(Farmer __instance, Item ___mostRecentlyGrabbedItem, Item ___itemToEat)
		{
			if (___mostRecentlyGrabbedItem != null && ___itemToEat is StardewValley.Object consumed)
			{
				int staminaToHeal = (int)Math.Ceiling((double)consumed.Edibility * 2.5) + (int)consumed.quality * consumed.Edibility;
				int healthBonus = ((consumed.Edibility >= 0) ? ((int)((float)staminaToHeal * 0.45f)) : 0);

				if (healthBonus >= minimumHealthBonusForDebuff)
				{
					timeSinceLastEat = DateTime.Now;
				}
			}
		}
	}

	class PostEatFoodDebuff2 : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Farmer), "getMovementSpeed");

		public static void Postfix(ref float __result)
		{
			if (PostEatFoodDebuff1.timeSinceLastEat.HasValue)
			{
				if ((DateTime.Now - PostEatFoodDebuff1.timeSinceLastEat.Value).TotalMilliseconds < PostEatFoodDebuff1.debuffMilliseconds)
				{
					__result *= PostEatFoodDebuff1.speedMultiplier;
				}				
			}
		}
	}
}
