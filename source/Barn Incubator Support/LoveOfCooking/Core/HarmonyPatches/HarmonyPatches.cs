/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using Harmony; // el diavolo
using StardewValley;
using System;

namespace LoveOfCooking.Core.HarmonyPatches
{
	public static class HarmonyPatches
	{
		public static string Id => ModEntry.Instance.Helper.ModRegistry.ModID;

		public static void Patch()
		{
			var harmony = HarmonyInstance.Create(Id);
			try
			{
				BushPatches.Patch(harmony);
			}
			catch (Exception ex)
			{
				Log.E("" + ex);
			}
			try
			{
				CommunityCentrePatches.Patch(harmony);
			}
			catch (Exception ex)
			{
				Log.E("" + ex);
			}
			try
			{
				CraftingPagePatches.Patch(harmony);
			}
			catch (Exception ex)
			{
				Log.E("" + ex);
			}
			try
			{
				// Perform other miscellaneous patches

				// Upgrade cooking tool in any instance it's claimed by the player, including through interactions with Clint's shop and mail delivery
				harmony.Patch(
					original: AccessTools.Method(typeof(StardewValley.Tools.GenericTool), "actionWhenClaimed"),
					prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(GenericTool_ActionWhenClaimed_Prefix)));
				// Handle sale price bonus profession for Cooking skill by affecting object sale multipliers
				harmony.Patch(
					original: AccessTools.Method(typeof(StardewValley.Object), "getPriceAfterMultipliers"),
					postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Object_GetPriceAfterMultipliers_Postfix)));
			}
			catch (Exception ex)
			{
				Log.E("" + ex);
			}
		}

		public static void GenericTool_ActionWhenClaimed_Prefix(ref StardewValley.Tools.GenericTool __instance)
		{
			if (Tools.IsThisCookingTool(__instance))
			{
				Log.D($"Collected {__instance?.Name ?? "null cooking tool"} (index {__instance.IndexOfMenuItemView})",
					ModEntry.Instance.Config.DebugMode);
				++ModEntry.Instance.States.Value.CookingToolLevel;
			}
		}

		public static void Object_GetPriceAfterMultipliers_Postfix(StardewValley.Object __instance, ref float __result, 
			float startPrice, long specificPlayerID = -1L)
		{
			if (ModEntry.CookingSkillApi.IsEnabled())
			{
				var multiplier = 1f;
				foreach (Farmer player in Game1.getAllFarmers())
				{
					if (Game1.player.useSeparateWallets)
					{
						if (specificPlayerID == -1)
						{
							if (player.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID || !player.isActive())
							{
								continue;
							}
						}
						else if (player.UniqueMultiplayerID != specificPlayerID)
						{
							continue;
						}
					}
					else if (!player.isActive())
					{
						continue;
					}

					// Add bonus price for having the sale value Cooking skill profession
					bool hasSaleProfession = ModEntry.CookingSkillApi.HasProfession(GameObjects.ICookingSkillAPI.Profession.SalePrice, player.UniqueMultiplayerID);
					if (hasSaleProfession && __instance.Category == ModEntry.CookingCategory)
					{
						multiplier *= GameObjects.CookingSkill.SalePriceModifier;
					}
				}
				__result *= multiplier;
			}
		}
	}
}
