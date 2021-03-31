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
using System;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class ObjectGetPriceAfterMultipliersPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal ObjectGetPriceAfterMultipliersPatch() { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(SObject), name: "getPriceAfterMultipliers"),
				prefix: new HarmonyMethod(GetType(), nameof(ObjectGetPriceAfterMultipliersPrefix))
			);
		}

		#region harmony patches
		/// <summary>Patch to modify price multipliers for various modded professions.</summary>
		protected static bool ObjectGetPriceAfterMultipliersPrefix(ref SObject __instance, ref float __result, float startPrice, long specificPlayerID)
		{
			float saleMultiplier = 1f;

			foreach (Farmer player in Game1.getAllFarmers())
			{
				if (Game1.player.useSeparateWallets)
				{
					if (specificPlayerID == -1)
					{
						if (player.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID || !player.isActive()) continue;
					}
					else if (player.UniqueMultiplayerID != specificPlayerID) continue;
				}
				else if (!player.isActive()) continue;

				float multiplier = 1f;

				// professions
				if (player.IsLocalPlayer && Utility.LocalPlayerHasProfession("oenologist") && Utility.IsWineOrBeverage(__instance))
					multiplier *= 1f + Utility.GetOenologistPriceBonus();
				else if (Utility.SpecificPlayerHasProfession("producer", player) && Utility.IsAnimalProduct(__instance))
					multiplier *= Utility.GetProducerPriceMultiplier(player);
				else if (Utility.SpecificPlayerHasProfession("angler", player) && Utility.IsReeledFish(__instance))
					multiplier *= Utility.GetAnglerPriceMultiplier(player);

				// events
				else if (player.eventsSeen.Contains(2120303) && Utility.IsWildBerry(__instance))
					multiplier *= 3f;
				else if (player.eventsSeen.Contains(3910979) && Utility.IsSpringOnion(__instance))
					multiplier *= 5f;

				if (Utility.LocalPlayerHasProfession("conservationist"))
					multiplier *= 1f + Data.ConservationistTaxBonusThisSeason;

				saleMultiplier = Math.Max(saleMultiplier, multiplier);
			}

			__result = startPrice * saleMultiplier;
			return false; // don't run original logic
		}
		#endregion harmony patches
	}
}
