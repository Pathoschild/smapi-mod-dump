/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/RaisedGardenBeds
**
*************************************************/

using HarmonyLib; // el diavolo nuevo
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RaisedGardenBeds
{
	public static class HarmonyPatches
	{
		internal class PatchTemplate
		{
			public readonly HarmonyPatchType type;
			public readonly MethodInfo original;
			public readonly string patch;
			public readonly HarmonyMethod method;

			public PatchTemplate(HarmonyPatchType type, MethodInfo original, string patch = null, HarmonyMethod method = null)
			{
				this.type = type;
				this.original = original;
				this.patch = patch ?? method.methodName;
				this.method = method ?? new HarmonyMethod(
					methodType: typeof(HarmonyPatches),
					methodName: patch);
			}
		}

		internal static void Patch(string id)
		{
			Harmony harmony = new Harmony(id);

			List<PatchTemplate> patches = new List<PatchTemplate>
			{
				// Utility
				new PatchTemplate(
					type: HarmonyPatchType.Prefix,
					original: AccessTools.Method(typeof(StardewValley.Utility), "isThereAnObjectHereWhichAcceptsThisItem"),
					patch: nameof(HarmonyPatches.Utility_IsThereAnObjectHereWhichAcceptsThisItem_Prefix)),
				new PatchTemplate(
					type: HarmonyPatchType.Prefix,
					original: AccessTools.Method(typeof(StardewValley.Utility), "isViableSeedSpot"),
					patch: nameof(HarmonyPatches.Utility_IsViableSeedSpot_Prefix)),
				
				// Object
				new PatchTemplate(
					type: HarmonyPatchType.Prefix,
					original: AccessTools.Method(typeof(StardewValley.Object), "ApplySprinkler"),
					patch: nameof(HarmonyPatches.Object_ApplySprinkler_Prefix)),
				
				// GameLocation
				new PatchTemplate(
					type: HarmonyPatchType.Postfix,
					original: AccessTools.Method(typeof(StardewValley.GameLocation), "isTileOccupiedForPlacement"),
					patch: nameof(HarmonyPatches.GameLocation_IsTileOccupiedForPlacement_Postfix)),
				
				// CraftingPage
				new PatchTemplate(
					type: HarmonyPatchType.Postfix,
					original: AccessTools.Method(typeof(StardewValley.Menus.CraftingPage), "layoutRecipes"),
					patch: nameof(HarmonyPatches.CraftingPage_LayoutRecipes_Postfix)),
				new PatchTemplate(
					type: HarmonyPatchType.Prefix,
					original: AccessTools.Method(typeof(StardewValley.Menus.CraftingPage), "clickCraftingRecipe"),
					patch: nameof(HarmonyPatches.CraftingPage_ClickCraftingRecipe_Prefix)),
			};

			Log.T(patches.Aggregate("Applying Harmony patches:", (str, p) => $"{str}{Environment.NewLine}{p.patch}"));

			foreach (PatchTemplate patch in patches)
			{
				harmony.Patch(
					original: patch.original,
					prefix: patch.type == HarmonyPatchType.Prefix ? patch.method : null,
					postfix: patch.type == HarmonyPatchType.Postfix ? patch.method : null,
					transpiler: patch.type == HarmonyPatchType.Transpiler ? patch.method : null,
					finalizer: patch.type == HarmonyPatchType.Finalizer ? patch.method : null);
			}
		}

		private static void ErrorHandler(Exception e)
		{
			Log.E($"{ModEntry.Instance.ModManifest.UniqueID} failed in harmony patch method.{Environment.NewLine}{e}");
		}
		
		/// <summary>
		/// Replace logic determining item drop-in actions on garden bed objects.
		/// </summary>
		public static bool Utility_IsThereAnObjectHereWhichAcceptsThisItem_Prefix(
			ref bool __result,
			GameLocation location,
			Item item,
			int x,
			int y)
		{
			try
			{
				Vector2 tileLocation = new Vector2(x / Game1.tileSize, y / Game1.tileSize);
				if (location.Objects.TryGetValue(tileLocation, out StardewValley.Object o) && o != null && o is OutdoorPot op)
				{
					if (!OutdoorPot.CanAcceptItemOrSeed(item: item) && OutdoorPot.CanAcceptAnything(op: op))
					{
						__result = op.performObjectDropInAction(dropInItem: (StardewValley.Object)item, probe: true, who: Game1.player);
					}
					else
					{
						__result = false;
					}
					return false;
				}
			}
			catch (Exception e)
			{
				HarmonyPatches.ErrorHandler(e);
			}
			return true;
		}

		/// <summary>
		/// Add logic to consider new conditions for planting seeds in garden bed objects.
		/// </summary>
		public static bool Utility_IsViableSeedSpot_Prefix(
			GameLocation location,
			Vector2 tileLocation,
			Item item)
		{
			try
			{
				if (location.Objects.TryGetValue(tileLocation, out StardewValley.Object o) && o != null && o is OutdoorPot op)
				{
					return OutdoorPot.CanAcceptItemOrSeed(item) && OutdoorPot.CanAcceptSeed(item: item, op: op) && OutdoorPot.CanAcceptAnything(op: op);
				}
			}
			catch (Exception e)
			{
				HarmonyPatches.ErrorHandler(e);
			}
			return true;
		}

		/// <summary>
		/// Replace logic for garden bed objects being watered by sprinklers.
		/// </summary>
		public static bool Object_ApplySprinkler_Prefix(
			GameLocation location,
			Vector2 tile)
		{
			try
			{
				if (ModEntry.Config.SprinklersEnabled
					&& location.Objects.TryGetValue(tile, out StardewValley.Object o) && o != null && o is OutdoorPot op)
				{
					if (OutdoorPot.CanAcceptAnything(op: op, ignoreCrops: true))
					{
						op.Water();
					}
					return false;
				}
			}
			catch (Exception e)
			{
				HarmonyPatches.ErrorHandler(e);
			}
			return true;
		}

		/// <summary>
		/// Replace logic for choosing whether objects can be placed into a custom garden bed.
		/// </summary>
		public static void GameLocation_IsTileOccupiedForPlacement_Postfix(
			GameLocation __instance,
			ref bool __result,
			Vector2 tileLocation,
			StardewValley.Object toPlace)
		{
			if (__instance.Objects.TryGetValue(tileLocation, out StardewValley.Object o) && o != null && o is OutdoorPot op)
			{
				bool isPlantable = OutdoorPot.CanAcceptItemOrSeed(toPlace)
					&& op.hoeDirt.Value.canPlantThisSeedHere(toPlace.ParentSheetIndex, (int)tileLocation.X, (int)tileLocation.Y, toPlace.Category == -19);
				if (OutdoorPot.CanAcceptAnything(op: op) && isPlantable)
				{
					__result = false;
				}
			}
		}

		/// <summary>
		/// Required to draw correct object sprites and strings in crafting menu.
		/// Event handlers on StardewModdingAPI.Events.Display.MenuChanged were inconsistent.
		/// </summary>
		public static void CraftingPage_LayoutRecipes_Postfix(
			CraftingPage __instance)
		{
			int unlockedCount = Game1.player.craftingRecipes.Keys.Count(c => c.StartsWith(OutdoorPot.GenericName));
			int[] matchesPerDict = new int[__instance.pagesOfCraftingRecipes.Count];
			int i = 0;
			foreach (Dictionary<ClickableTextureComponent, CraftingRecipe> dict in __instance.pagesOfCraftingRecipes)
			{
				List<KeyValuePair<ClickableTextureComponent, CraftingRecipe>> matches = dict
					.Where(pair => pair.Value.name.StartsWith(OutdoorPot.GenericName))
					.ToList();
				matches.ForEach(pair =>
				{
					string variantKey = OutdoorPot.GetVariantKeyFromName(name: pair.Value.name);

					// Sprite
					pair.Key.texture = ModEntry.Sprites[ModEntry.ItemDefinitions[variantKey].SpriteKey];
					pair.Key.sourceRect = OutdoorPot.GetSpriteSourceRectangle(spriteIndex: ModEntry.ItemDefinitions[variantKey].SpriteIndex);

					// Strings
					pair.Value.DisplayName = OutdoorPot.GetDisplayNameFromName(pair.Value.name);
					pair.Value.description = OutdoorPot.GetRawDescription();
				});
				matchesPerDict[i++] = matches.Count();
			}
			Log.T($"Found {string.Join("/", matchesPerDict)} garden beds in crafting menu pages ({unlockedCount} unlocked).");
		}

		/// <summary>
		/// Replace logic for crafting objects in base game crafting menu to create the appropriate garden bed for the crafting recipe variant.
		/// </summary>
		public static bool CraftingPage_ClickCraftingRecipe_Prefix(
			CraftingPage __instance,
			int ___currentCraftingPage,
			ref Item ___heldItem,
			ClickableTextureComponent c,
			bool playSound = true)
		{
			try
			{
				// Fetch an instance of any clicked-on craftable in the crafting menu
				CraftingRecipe recipe = __instance.pagesOfCraftingRecipes[___currentCraftingPage][c];

				// Fall through to default method for any other craftables
				if (!recipe.name.StartsWith(OutdoorPot.GenericName))
					return true;

				OutdoorPot item = new OutdoorPot(
					variantKey: OutdoorPot.GetVariantKeyFromName(recipe.name),
					tileLocation: Vector2.Zero);

				// Behaviours as from base method
				recipe.consumeIngredients(null);
				if (playSound)
				{
					Game1.playSound("coin");
				}
				if (___heldItem == null)
				{
					___heldItem = item;
				}
				else if (___heldItem.canStackWith(item))
				{
					___heldItem.addToStack(item);
				}
				if (Game1.player.craftingRecipes.ContainsKey(recipe.name))
				{
					Game1.player.craftingRecipes[recipe.name] += recipe.numberProducedPerCraft;
				}
				Game1.stats.checkForCraftingAchievements();
				if (Game1.options.gamepadControls && ___heldItem != null && Game1.player.couldInventoryAcceptThisItem(___heldItem))
				{
					Game1.player.addItemToInventoryBool(___heldItem);
					___heldItem = null;
				}

				return false;
			}
			catch (Exception e)
			{
				HarmonyPatches.ErrorHandler(e);
			}
			return true;
		}
	}
}
