/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CooksAssistant
**
*************************************************/

using HarmonyLib; // el diavolo nuevo
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;

namespace LoveOfCooking.Core.HarmonyPatches
{
	public static class HarmonyPatches
	{
		public static string Id => ModEntry.Instance.Helper.ModRegistry.ModID;


		public static void Patch()
		{
			Harmony harmony = new Harmony(Id);
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
				CraftingPagePatches.Patch(harmony);
			}
			catch (Exception ex)
			{
				Log.E("" + ex);
			}
			try
			{
				// Perform other miscellaneous patches
				Type[] types;

				// Upgrade cooking tool in any instance it's claimed by the player, including interactions with Clint's shop and mail delivery mods
				harmony.Patch(
					original: AccessTools.Method(typeof(StardewValley.Tool), "actionWhenClaimed"),
					prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Tool_ActionWhenClaimed_Prefix)));

				// Handle sale price bonus profession for Cooking skill by affecting object sale multipliers
				harmony.Patch(
					original: AccessTools.Method(typeof(StardewValley.Object), "getPriceAfterMultipliers"),
					postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Object_GetPriceAfterMultipliers_Postfix)));

				// Hide buffs in cooked foods not yet eaten
				if (ModEntry.HideBuffIconsOnItems)
				{
					types = new Type[]
					{
						typeof(SpriteBatch), typeof(System.Text.StringBuilder),
						typeof(SpriteFont), typeof(int), typeof(int), typeof(int),
						typeof(string), typeof(int), typeof(string[]), typeof(Item), typeof(int), typeof(int),
						typeof(int), typeof(int), typeof(int), typeof(float), typeof(CraftingRecipe),
						typeof(IList<Item>)
					};
					harmony.Patch(
						original: AccessTools.Method(typeof(StardewValley.Menus.IClickableMenu), "drawHoverText", parameters: types),
						prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(IClickableMenu_DrawHoverText_Prefix)));
				}
			}
			catch (Exception ex)
			{
				Log.E("" + ex);
			}
		}

		public static void IClickableMenu_DrawHoverText_Prefix(
			ref string[] buffIconsToDisplay,
			StardewValley.Item hoveredItem)
		{
			if (!Utils.IsItemFoodAndNotYetEaten(hoveredItem))
				return;

			string[] dummyBuffIcons = new string[AssetManager.DummyIndexForHidingBuffs + 1];
			for (int i = 0; i < dummyBuffIcons.Length; ++i)
			{
				dummyBuffIcons[i] = "0";
			}
			dummyBuffIcons[AssetManager.DummyIndexForHidingBuffs] = "1";
			buffIconsToDisplay = dummyBuffIcons;
			AssetManager.IsCurrentHoveredItemHidingBuffs = true;
		}

		public static void Tool_ActionWhenClaimed_Prefix(
			ref StardewValley.Tool __instance)
		{
			if (Tools.IsThisCookingTool(__instance))
			{
				Log.D($"Collected {__instance?.Name ?? "null cooking tool"} (index {__instance.IndexOfMenuItemView})",
					ModEntry.Config.DebugMode);
				++ModEntry.Instance.States.Value.CookingToolLevel;
			}
		}

		public static void Object_GetPriceAfterMultipliers_Postfix(
			StardewValley.Object __instance,
			ref float __result, 
			float startPrice,
			long specificPlayerID = -1L)
		{
			if (ModEntry.CookingSkillApi.IsEnabled())
			{
				float multiplier = 1f;
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
					bool hasSaleProfession = ModEntry.CookingSkillApi.HasProfession(Objects.ICookingSkillAPI.Profession.SalePrice, player.UniqueMultiplayerID);
					if (hasSaleProfession && __instance.Category == ModEntry.CookingCategory)
					{
						multiplier *= Objects.CookingSkill.SalePriceModifier;
					}
				}
				__result *= multiplier;
			}
		}
	}
}
