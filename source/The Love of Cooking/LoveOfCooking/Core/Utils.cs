/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CooksAssistant
**
*************************************************/

using LoveOfCooking.Objects;
using Microsoft.Xna.Framework;
using SpaceCore;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LoveOfCooking
{
	public static class Utils
	{
		/// <summary>
		/// Checks whether the player has agency during gameplay, cutscenes, and input sessions.
		/// </summary>
		public static bool PlayerAgencyLostCheck()
		{
			// HOUSE RULES
			return !Context.IsWorldReady || !Context.CanPlayerMove
					|| Game1.game1 is null || Game1.currentLocation is null || Game1.player is null // No unplayable games
					|| !Game1.game1.IsActive // No alt-tabbed game state
					|| (Game1.eventUp && Game1.currentLocation.currentEvent is not null && !Game1.currentLocation.currentEvent.playerControlSequence) // No event cutscenes
					|| Game1.nameSelectUp || Game1.IsChatting || Game1.dialogueTyping || Game1.dialogueUp
					|| (Game1.keyboardDispatcher is not null && Game1.keyboardDispatcher.Subscriber is not null) // No text inputs
					|| Game1.player.UsingTool || Game1.pickingTool || Game1.numberOfSelectedItems != -1 // No tools in use
					|| Game1.fadeToBlack; // None of that
		}

		public static void CleanUpSaveFiles()
		{
			Utils.CleanUpMail();
			Utils.PopulateMissingRecipes();
		}

		internal static void CleanUpMail()
		{
			Game1.player.mailReceived.Remove(ModEntry.MailFryingPanWhoops);
		}

		internal static bool IsCookingMenu(IClickableMenu menu)
		{
			return menu is not null && ModEntry.Instance.Helper.Reflection.GetField<bool>(obj: menu, name: "cooking", required: false)?.GetValue() is true;
		}

		internal static void PopulateMissingRecipes()
		{
			// Add any missing starting recipes
			foreach (string recipe in CookingSkill.StartingRecipes)
			{
				if (!Game1.player.cookingRecipes.ContainsKey(recipe))
				{
					Log.D($"Added missing starting recipe {recipe}",
						ModEntry.Config.DebugMode);
					Game1.player.cookingRecipes.Add(recipe, 0);
				}
			}

			// Ensure vanilla campfire recipe is added
			if (!Game1.player.craftingRecipes.ContainsKey("Campfire"))
			{
				// Re-add campfire to the player's recipe list if it's otherwise missing
				Game1.player.craftingRecipes["Campfire"] = 0;
			}

			if (ModEntry.Config.AddCookingSkillAndRecipes)
			{
				// Clear daily cooking to free up Cooking experience gains
				ModEntry.Instance.States.Value.FoodCookedToday.Clear();

				// Add any missing recipes from the level-up recipe table
				int level = ModEntry.CookingSkillApi.GetLevel();
				IReadOnlyDictionary<int, IList<string>> recipes = ModEntry.CookingSkillApi.GetAllLevelUpRecipes();
				IEnumerable<string> missingRecipes = recipes
					// Take all recipe lists up to the current level
					.TakeWhile(pair => pair.Key < level)
					.SelectMany(pair => pair.Value) // Flatten recipe lists into their recipes
					.Select(r => ModEntry.ObjectPrefix + r) // Add item prefixes
					.Where(r => !Game1.player.cookingRecipes.ContainsKey(r)); // Take recipes not known by the player
				foreach (string recipe in missingRecipes)
				{
					Log.D($"Added missing recipe {recipe}",
						ModEntry.Config.DebugMode);
					Game1.player.cookingRecipes.Add(recipe, 0);
				}
			}
		}

		internal static void CalculateFoodRegenModifiers()
		{
			// Calculate food regeneration rate from skill levels
			float[] scalingCurrent = new float[ModEntry.ItemDefinitions["RegenSkillModifiers"].Count];
			float[] scalingMax = new float[ModEntry.ItemDefinitions["RegenSkillModifiers"].Count];
			for (int i = 0; i < ModEntry.ItemDefinitions["RegenSkillModifiers"].Count; ++i)
			{
				string[] split = ModEntry.ItemDefinitions["RegenSkillModifiers"][i].Split(':');
				string name = split[0];
				bool isDefined = Enum.TryParse(name, out ModEntry.SkillIndex skillIndex);
				int level = isDefined
					? Game1.player.GetSkillLevel((int)Enum.Parse(typeof(ModEntry.SkillIndex), name))
					: SpaceCore.Skills.GetSkill(name) is not null
						? Game1.player.GetCustomSkillLevel(name)
						: -1;
				float value = float.Parse(split[1]);
				if (level < 0)
					continue;
				scalingCurrent[i] = level * value;
				scalingMax[i] = 10 * value;
			}
			ModEntry.Instance.States.Value.RegenSkillModifier = scalingCurrent.Sum() / scalingMax.Sum();
		}

		/// <summary>
		/// I keep forgetting the method name
		/// </summary>
		public static void CreateInspectDialogue(string dialogue)
		{
			Game1.drawDialogueNoTyping(dialogue);
		}

		public static void AddOrDropItem(Item item)
		{
			if (Game1.player.couldInventoryAcceptThisItem(item))
				Game1.player.addItemToInventory(item);
			else
				Game1.createItemDebris(item, Game1.player.Position, -1);
		}

		internal static void ReplaceCraftingMenu(IClickableMenu lastMenu)
		{
			lastMenu.exitThisMenuNoSound();
			Game1.delayedActions.Add(new DelayedAction(timeUntilAction: 0, behavior: delegate
			{
				Utils.OpenNewCookingMenu(lastMenu: lastMenu);
			}));
		}

		internal static void OpenNewCookingMenu(IClickableMenu lastMenu = null, bool forceOpen = false)
		{
			if (ModEntry.Config.AddCookingMenu || forceOpen)
			{
				Game1.activeClickableMenu = lastMenu is not null && lastMenu is CraftingPage craftingPage
					? new CookingMenu(craftingPage: craftingPage)
					: new CookingMenu();
			}
			else
			{
				Game1.activeClickableMenu?.exitThisMenuNoSound();
				Utils.CreateInspectDialogue(ModEntry.Instance.i18n.Get("menu.cooking_station.no_cookbook"));
			}
		}

		/// <summary>
		/// Returns the base health/stamina regeneration rate for some food object.
		/// </summary>
		public static float GetFoodRegenRate(StardewValley.Object food)
		{
			// Health regenerates faster when...

			// drinking drinks
			float rate = ModEntry.Instance.States.Value.LastFoodWasDrink ? 0.12f : 0.075f;
			// consuming quality foods
			rate += food.Quality * 0.0085f;
			// under the 'tipsy' debuff
			if (Game1.player.hasBuff(17))
				rate *= 1.3f;
			// cooking skill professions are unlocked
			if (ModEntry.CookingSkillApi.HasProfession(ICookingSkillAPI.Profession.Restoration))
				rate += rate / CookingSkill.RestorationValue;
			// sitting or lying down
			if (Game1.player.IsSitting() || Game1.player.isInBed.Value)
				rate *= 1.4f;

			return rate;
		}

		public static void AddAndDisplayNewRecipesOnLevelUp(SpaceCore.Interface.SkillLevelUpMenu menu)
		{
			// Add cooking recipes
			string skill = ModEntry.Instance.Helper.Reflection
				.GetField<string>(menu, "currentSkill")
				.GetValue();
			if (skill != CookingSkill.InternalName)
				return;
			
			int level = ModEntry.Instance.Helper.Reflection
				.GetField<int>(menu, "currentLevel")
				.GetValue();
			List<CraftingRecipe> cookingRecipes = ModEntry.CookingSkillApi
				.GetCookingRecipesForLevel(level)
				.ToList()
				.ConvertAll(name => new CraftingRecipe(ModEntry.ObjectPrefix + name, true))
				.Where(recipe => !Game1.player.knowsRecipe(recipe.name))
				.ToList();
			if (cookingRecipes is not null && cookingRecipes.Count > 0)
			{
				UpdateEnglishRecipeDisplayNames(ref cookingRecipes);
				foreach (CraftingRecipe recipe in cookingRecipes.Where(r => !Game1.player.cookingRecipes.ContainsKey(r.name)))
				{
					Game1.player.cookingRecipes[recipe.name] = 0;
				}
			}

			// Add crafting recipes
			var craftingRecipes = new List<CraftingRecipe>();
			// No new crafting recipes currently.

			// Apply new recipes
			List<CraftingRecipe> combinedRecipes = craftingRecipes
				.Concat(cookingRecipes)
				.ToList();
			ModEntry.Instance.Helper.Reflection
				.GetField<List<CraftingRecipe>>(menu, "newCraftingRecipes")
				.SetValue(combinedRecipes);
			Log.D(combinedRecipes.Aggregate($"New recipes for level {level}:", (total, cur) => $"{total}{Environment.NewLine}{cur.name} ({cur.createItem().ParentSheetIndex})"),
				ModEntry.Config.DebugMode);

			// Adjust menu to fit if necessary
			const int defaultMenuHeightInRecipes = 4;
			int menuHeightInRecipes = combinedRecipes.Count + combinedRecipes.Count(recipe => recipe.bigCraftable);
			if (menuHeightInRecipes >= defaultMenuHeightInRecipes)
			{
				menu.height += (menuHeightInRecipes - defaultMenuHeightInRecipes) * StardewValley.Object.spriteSheetTileSize * Game1.pixelZoom;
			}
		}

		public static List<CraftingRecipe> TakeRecipesFromCraftingPage(CraftingPage cm, bool cookingOnly = true)
		{
			if (Utils.IsCookingMenu(cm) || !cookingOnly)
			{
				cm.exitThisMenuNoSound();
				return cm.pagesOfCraftingRecipes
					.SelectMany(page => page.Values)
					.ToList();
			}
			return null;
		}

		public static bool IsFridgeOrMinifridge(StardewValley.Object o)
		{
			return o is not null && o.bigCraftable.Value && o is Chest c && c.fridge.Value;
		}

		public static bool IsMinifridge(StardewValley.Object o)
		{
			return Utils.IsFridgeOrMinifridge(o) && o.ParentSheetIndex != 130;
		}

		public static bool IsItemFoodAndNotYetEaten(StardewValley.Item item)
		{
			return item is StardewValley.Object o && o is not null
				&& !o.bigCraftable.Value && o.Category == ModEntry.CookingCategory
				&& !ModEntry.Instance.States.Value.FoodsEaten.Contains(o.Name);
		}

		public static bool AreNewCropsActive()
        {
			return ModEntry.Config.AddNewCropsAndStuff && !Interface.Interfaces.UsingPPJACrops;
        }

		public static bool AreNettlesActive()
		{
			return !Interface.Interfaces.UsingNettlesCrops;
		}

		public static void TrySpawnNettles()
		{
			// Only master player should make changes to the world
			if (Game1.MasterPlayer.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
				return;

			// Attempt to place a wild nettle as forage around other weeds
			bool spawnNettlesToday = (Game1.dayOfMonth % 3 == 1 && (Game1.currentSeason == "spring" || Game1.currentSeason == "fall"))
				|| Game1.currentSeason == "summer";
			if (Utils.AreNettlesActive() && spawnNettlesToday)
			{
				Utils.SpawnNettles();
			}
		}

		public static void SpawnNettles(bool force = false)
		{
			foreach (string l in ModEntry.ItemDefinitions["NettlesLocations"])
			{
				if (!force && Game1.random.NextDouble() > float.Parse(ModEntry.ItemDefinitions["NettlesDailyChancePerLocation"][0]))
				{
					// Skip the location if we didn't succeed the roll to add nettles
					Log.D($"Did not add nettles to {l}.",
						ModEntry.Config.DebugMode);
					continue;
				}

				// Spawn a random number of nettles between some upper and lower bounds, reduced by the number of nettles already in this location
				GameLocation location = Game1.getLocationFromName(l);
				int nettlesToAdd = Game1.random.Next(int.Parse(ModEntry.ItemDefinitions["NettlesAddedRange"][0]), int.Parse(ModEntry.ItemDefinitions["NettlesAddedRange"][1]));
				int nettlesAlreadyInLocation = force 
					? 0
					: location.largeTerrainFeatures.Count(
						ltf => ltf is CustomBush cb && (string)cb.Variety == ModEntry.BushNameNettle);
				nettlesToAdd -= nettlesAlreadyInLocation;
				int nettlesAdded = 0;

				List<Vector2> shuffledWeedsTiles = location.Objects.Keys
					.Where(tile => location.Objects.TryGetValue(tile, out StardewValley.Object o) && o.Name == "Weeds")
					.ToList();
				Utility.Shuffle(Game1.random, shuffledWeedsTiles);
				foreach (Vector2 tile in shuffledWeedsTiles)
				{
					if (nettlesAdded >= nettlesToAdd)
					{
						// Move to the next location if this location's quota is met
						break;
					}
					Vector2 nearbyTile = Utility.getRandomAdjacentOpenTile(tile, location);
					if (nearbyTile == Vector2.Zero)
					{
						// Skip weeds without any free spaces to spawn nettles upon
						continue;
					}
					// Spawn nettles around other weeds
					CustomBush nettleBush = new CustomBush(
						tile: nearbyTile,
						location: location,
						variety: ModEntry.BushNameNettle);
					location.largeTerrainFeatures.Add(nettleBush);
					++nettlesAdded;
					Log.D($"Adding to {nearbyTile}...",
						force || ModEntry.Config.DebugMode);
				}

				Log.D($"Added {nettlesAdded} nettles to {l}.",
					force || ModEntry.Config.DebugMode);
			}
		}

		public static void ShakeNettles(CustomBush bush)
		{
			string variety = (string)bush.Variety;
			if (variety == ModEntry.BushNameNettle)
			{
				DelayedAction.playSoundAfterDelay("leafrustle", 100);
				Game1.player.takeDamage(
					damage: Math.Max(1, int.Parse(ModEntry.ItemDefinitions["NettlesDamage"][0]) - Game1.player.resilience),
					overrideParry: true,
					damager: null);
				if (Game1.player.health < 1)
					Game1.player.health = 1;
				Buff existingBuff = Game1.buffsDisplay.otherBuffs
					.FirstOrDefault(b => b.source == variety);
				if (existingBuff is null)
				{
					Game1.buffsDisplay.addOtherBuff(new Buff(
						0, 0, 0, 0, 0, 0, 0,
						0, 0, speed: -1, 0, 0,
						minutesDuration: 10,
						source: variety,
						displaySource: ModEntry.Instance.Helper.Translation.Get("buff.nettles.inspect")));
				}
				else
				{
					existingBuff.millisecondsDuration = 6000;
				}
			}
		}

		/// <summary>
		/// Checks for if the player meets conditions to open the new cooking menu.
		/// Always true if using the default cooking menu.
		/// </summary>
		public static bool CanUseKitchens()
		{
			return !ModEntry.Config.AddCookingMenu || Game1.player.hasOrWillReceiveMail(ModEntry.MailCookbookUnlocked);
		}

		public static void AddToShopAtItemIndex(ShopMenu menu, StardewValley.Object o, string targetItemName = "", int price = -1, int stock = -1)
		{
			if (stock < 1)
				stock = int.MaxValue;
			if (price < 0)
				price = o.salePrice();
			price = (int)(price * Game1.MasterPlayer.difficultyModifier);

			// Add sale info
			menu.itemPriceAndStock.Add(o, new[] { price, stock });

			// Add shop entry
			int index = menu.forSale.FindIndex(i => i.Name == targetItemName);
			if (index >= 0 && index < menu.forSale.Count)
				menu.forSale.Insert(index, o);
			else
				menu.forSale.Add(o);
		}

		/// <summary>
		/// Bunches groups of common items together in the seed shop.
		/// Json Assets appends new stock to the bottom, and we don't want that very much at all.
		/// </summary>
		public static void SortSeedShopStock(ref ShopMenu menu)
		{
			// Pair a suffix grouping some common items together with the name of the lowest-index (first-found) item in the group
			List<ISalable> itemList = menu.forSale;
			var suffixes = new Dictionary<string, string>
				{{"seeds", null}, {"bulb", null}, {"starter", null}, {"shoot", null}, {"sapling", null}};

			for (int i = 0; i < itemList.Count; ++i)
			{
				// Ignore items without one of our group suffixes
				string suffix = suffixes.Keys
					.FirstOrDefault(s => itemList[i].Name.ToLower().EndsWith(s));
				if (suffix is null)
					continue;
				// Set the move-to-this-item name to be the first-found item in the group
				suffixes[suffix] ??= itemList[i].Name;
				if (suffixes[suffix] == itemList[i].Name)
					continue;
				// Move newly-found items of a group up to the first item in the group, and change the move-to name to this item
				ISalable item = itemList[i];
				int index = 1 + itemList
					.FindIndex(i => i.Name == suffixes[suffix]);
				itemList.RemoveAt(i);
				itemList.Insert(index, item);
				suffixes[suffix] = itemList[index].Name;
			}
			menu.forSale = itemList;
		}

		/// <summary>
		/// Update display names for all new cooking recipe objects
		/// With English locale, recipes' display names default to the internal name, so we have to replace it
		/// </summary>
		internal static void UpdateEnglishRecipeDisplayNames(ref List<CraftingRecipe> recipes)
		{
			if (ModEntry.IsEnglishLocale)
			{
				foreach (CraftingRecipe recipe in recipes.Where(r => r.DisplayName.StartsWith(ModEntry.ObjectPrefix)))
				{
					string displayName = Game1.objectInformation[Interface.Interfaces.JsonAssets.GetObjectId(recipe.name)].Split('/')[4];
					recipe.DisplayName = displayName;
				}
			}
		}

		/// <summary>
		/// Updates multi-field entries separated by some delimiter, appending or replacing select fields.
		/// </summary>
		/// <returns>The old entry, with fields added from the new entry, reformed into a string of the delimited fields.</returns>
		public static string UpdateEntry(string oldEntry, string[] newEntry, bool append = false, bool replace = false,
			int startIndex = 0, char delimiter = '/')
		{
			string[] fields = oldEntry.Split(delimiter);
			if (replace)
				fields = newEntry;
			else for (int i = 0; i < newEntry.Length; ++i)
				if (newEntry[i] is not null)
					fields[startIndex + i] = append ? $"{fields[startIndex + i]} {newEntry[i]}" : newEntry[i];
			return string.Join(delimiter.ToString(), fields);
		}
	}
}
