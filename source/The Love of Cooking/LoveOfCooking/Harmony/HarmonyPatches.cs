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
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace LoveOfCooking.HarmonyPatches
{
	public static class HarmonyPatches
	{// TODO: harmony patch error messages
		public static void Patch(string id)
		{
			Harmony harmony = new Harmony(id);
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
				// Perform miscellaneous patches
				Type[] parameters;

				// Legacy: Upgrade cooking tool in any instance it's claimed by the player, including interactions with Clint's shop and mail delivery mods
				harmony.Patch(
					original: AccessTools.Method(typeof(StardewValley.Tool), "actionWhenClaimed"),
					prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Tool_ActionWhenClaimed_Prefix)));

				// Correctly assign display name field for CraftingRecipe instances in English locale
				parameters = new Type[] { typeof(string), typeof(bool) };
				harmony.Patch(
					original: AccessTools.Constructor(type: typeof(StardewValley.CraftingRecipe), parameters: parameters),
					postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CraftingRecipe_Constructor_Postfix)));

				// Correctly sort recipes by display name in base game cooking menu
				harmony.Patch(
					original: AccessTools.Method(type: typeof(StardewValley.Menus.CraftingPage), name: "layoutRecipes"),
					prefix: new HarmonyMethod(methodType: typeof(HarmonyPatches), methodName: nameof(HarmonyPatches.CraftingPage_LayoutRecipes_Prefix)));

				// Handle sale price bonus profession for Cooking skill by affecting object sale multipliers
				harmony.Patch(
					original: AccessTools.Method(typeof(StardewValley.Object), "getPriceAfterMultipliers"),
					postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Object_GetPriceAfterMultipliers_Postfix)));

				// Replace hold-up-item draw behaviour for Frying Pan cooking tool
				harmony.Patch(
					original: AccessTools.Method(typeof(StardewValley.Farmer), nameof(StardewValley.Farmer.showHoldingItem)),
					prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Utility_ShowHoldingItem_Prefix)));

				// Add Frying Pan cooking tool upgrades to Clint Upgrade stock
				harmony.Patch(
					original: AccessTools.Method(typeof(StardewValley.Utility), nameof(StardewValley.Utility.getBlacksmithUpgradeStock)),
					postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Utility_GetBlacksmithUpgradeStock_Postfix)));

				// Hide buffs in cooked foods not yet eaten
				if (ModEntry.HideBuffIconsOnItems)
				{
					parameters = new Type[]
					{
						typeof(SpriteBatch), typeof(System.Text.StringBuilder),
						typeof(SpriteFont), typeof(int), typeof(int), typeof(int),
						typeof(string), typeof(int), typeof(string[]), typeof(StardewValley.Item), typeof(int), typeof(int),
						typeof(int), typeof(int), typeof(int), typeof(float), typeof(StardewValley.CraftingRecipe),
						typeof(IList<StardewValley.Item>)
					};
					harmony.Patch(
						original: AccessTools.Method(typeof(StardewValley.Menus.IClickableMenu), "drawHoverText",
							parameters: parameters),
						prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(IClickableMenu_DrawHoverText_Prefix)));
				}
			}
			catch (Exception ex)
			{
				Log.E("" + ex);
			}
		}

		/// <summary>
		/// Draws the cooking tool sprite in place of default object draw logic when receiving an upgraded cooking tool.
		/// </summary>
		public static bool Utility_ShowHoldingItem_Prefix(
			Farmer who)
		{
			try
			{
				if (Objects.CookingTool.IsItemCookingTool(item: who.mostRecentlyGrabbedItem))
				{
					TemporaryAnimatedSprite sprite = new TemporaryAnimatedSprite(
						textureName: AssetManager.GameContentSpriteSheetPath,
						sourceRect: Objects.CookingTool.CookingToolSourceRectangle(upgradeLevel: (who.mostRecentlyGrabbedItem as StardewValley.Tool).UpgradeLevel),
						animationInterval: 2500f,
						animationLength: 1,
						numberOfLoops: 0,
						position: who.Position + (new Vector2(0, Game1.player.Sprite.SpriteHeight - 1) * -Game1.pixelZoom),
						flicker: false,
						flipped: false,
						layerDepth: 1f,
						alphaFade: 0f,
						color: Color.White,
						scale: Game1.pixelZoom,
						scaleChange: 0f,
						rotation: 0f,
						rotationChange: 0f)
					{
						motion = new Vector2(0f, -0.1f)
					};
					Game1.currentLocation.temporarySprites.Add(sprite);
					return false;
				}
				else
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				Log.E("" + ex);
				return true;
			}
		}

		/// <summary>
		/// Tries to add cooking tool to Blacksmith shop stock.
		/// </summary>
		public static void Utility_GetBlacksmithUpgradeStock_Postfix(
			Dictionary<ISalable, int[]> __result,
			Farmer who)
		{
			Objects.CookingTool.AddToShopStock(itemPriceAndStock: __result, who: who);
		}

		/// <summary>
		/// Raises flag to obscure buffs given by foods in their tooltip until recorded as having been eaten at least once.
		/// </summary>
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

		/// <summary>
		/// Legacy behaviour for non-specific GenericTool instances.
		/// </summary>
		public static void Tool_ActionWhenClaimed_Prefix(
			ref StardewValley.Tool __instance)
		{
			if (__instance is not Objects.CookingTool && Objects.CookingTool.IsItemCookingTool(item: __instance))
			{
				++ModEntry.Instance.States.Value.CookingToolLevel;
			}
		}

		/// <summary>
		/// Force recipe display names in English locale games.
		/// </summary>
		public static void CraftingRecipe_Constructor_Postfix(
			StardewValley.CraftingRecipe __instance)
		{
			bool isCooksAssistantContent = __instance.name.StartsWith(ModEntry.ObjectPrefix, StringComparison.OrdinalIgnoreCase);
			int displayNameIndex = __instance.isCookingRecipe ? 4 : 5;
			if (ModEntry.IsEnglishLocale && isCooksAssistantContent
				&& (StardewValley.CraftingRecipe.cookingRecipes.TryGetValue(__instance.name, out string data)
					|| StardewValley.CraftingRecipe.craftingRecipes.TryGetValue(__instance.name, out data))
				&& data.Split('/') is string[] split && split.Length >= displayNameIndex)
			{
				__instance.DisplayName = split.Last();
			}
		}

		/// <summary>
		/// Force cooking recipe sorting by display name in game menus.
		/// </summary>
		public static void CraftingPage_LayoutRecipes_Prefix(
			bool ___cooking,
			List<string> playerRecipes)
		{
			if (!___cooking)
				return;

			Dictionary<string, string> splitRecipes = playerRecipes.ToDictionary(
				keySelector: s => s,
				elementSelector: s => StardewValley.CraftingRecipe.cookingRecipes.TryGetValue(s, out string data)
					&& data.Split('/') is string[] split
					&& split.Length > 4 ? split.Last() : s);
			playerRecipes.Sort((a, b) => splitRecipes[a].CompareTo(splitRecipes[b]));
		}

		/// <summary>
		/// Apply custom sale price modifiers when calculating prices for any game objects.
		/// </summary>
		public static void Object_GetPriceAfterMultipliers_Postfix(
			StardewValley.Object __instance,
			ref float __result, 
			float startPrice,
			long specificPlayerID = -1L)
		{
			if (!ModEntry.CookingSkillApi.IsEnabled())
				return;
			
			float multiplier = 1f;
			foreach (StardewValley.Farmer player in Game1.getAllFarmers())
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
