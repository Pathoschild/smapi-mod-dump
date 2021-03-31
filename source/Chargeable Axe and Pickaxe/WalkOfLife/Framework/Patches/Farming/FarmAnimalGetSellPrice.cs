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
using StardewValley;

namespace TheLion.AwesomeProfessions
{
	internal class FarmAnimalGetSellPricePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal FarmAnimalGetSellPricePatch() { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.getSellPrice)),
				prefix: new HarmonyMethod(GetType(), nameof(FarmAnimalGetSellPricePrefix))
			);
		}

		#region harmony patches
		/// <summary>Patch to adjust Breeder animal sell price.</summary>
		protected static bool FarmAnimalGetSellPricePrefix(ref FarmAnimal __instance, ref int __result)
		{
			Farmer who = Game1.getFarmer(__instance.ownerID.Value);
			if (Utility.SpecificPlayerHasProfession("breeder", who))
			{
				double adjustedFriendship = Utility.GetProducerAdjustedFriendship(__instance);
				__result = (int)(__instance.price.Value * adjustedFriendship);
				return false; // don't run original logic
			}

			return true; // run original logic
		}
		#endregion harmony patches
	}
}
