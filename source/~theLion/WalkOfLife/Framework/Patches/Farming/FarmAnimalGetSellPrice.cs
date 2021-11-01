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
using StardewModdingAPI;
using StardewValley;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class FarmAnimalGetSellPricePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal FarmAnimalGetSellPricePatch()
		{
			Original = typeof(FarmAnimal).MethodNamed(nameof(FarmAnimal.getSellPrice));
			Prefix = new(GetType(), nameof(FarmAnimalGetSellPricePrefix));
		}

		#region harmony patches

		/// <summary>Patch to adjust Breeder animal sell price.</summary>
		[HarmonyPrefix]
		private static bool FarmAnimalGetSellPricePrefix(FarmAnimal __instance, ref int __result)
		{
			double adjustedFriendship;
			try
			{
				var owner = Game1.getFarmer(__instance.ownerID.Value);
				if (!owner.HasProfession("Breeder")) return true; // run original logic

				adjustedFriendship = Util.Professions.GetProducerAdjustedFriendship(__instance);
			}
			catch (Exception ex)
			{
				Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
				return true; // default to original logic
			}

			__result = (int) (__instance.price.Value * adjustedFriendship);
			return false; // don't run original logic
		}

		#endregion harmony patches
	}
}