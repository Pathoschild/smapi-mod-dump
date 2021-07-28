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
using Netcode;
using SpaceCore;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
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
			return Game1.game1 == null || Game1.currentLocation == null || Game1.player == null // No unplayable games
					|| !Game1.game1.IsActive // No alt-tabbed game state
					|| (Game1.eventUp && Game1.currentLocation.currentEvent != null && !Game1.currentLocation.currentEvent.playerControlSequence) // No event cutscenes
					|| Game1.nameSelectUp || Game1.IsChatting || Game1.dialogueTyping || Game1.dialogueUp
					|| (Game1.keyboardDispatcher != null && Game1.keyboardDispatcher.Subscriber != null) // No text inputs
					|| Game1.player.UsingTool || Game1.pickingTool || Game1.numberOfSelectedItems != -1 // No tools in use
					|| Game1.fadeToBlack; // None of that
		}

		public static void CheckTileAction(Vector2 position, GameLocation location)
		{
			string property = location.doesTileHaveProperty(
				(int)position.X, (int)position.Y, "Action", "Buildings");
			if (property == null)
				return;
			string[] action = property.Split(' ');
			switch (action[0])
			{
				case ModEntry.ActionRange:
					// A new cooking range in the Saloon acts as a cooking station
					//if (Config.AddCookingQuestline && Game1.player.getFriendshipHeartLevelForNPC("Gus") < 2)
					if (false)
					{
						CreateInspectDialogue(ModEntry.Instance.i18n.Get("world.range_gus.inspect"));
						break;
					}
					OpenNewCookingMenu(null);
					break;

				case ModEntry.ActionDockCrate:
					// Interact with the new crates at the secret beach pier to loot items for quests
					if (Interface.Interfaces.JsonAssets != null)
					{
						Game1.currentLocation.playSoundAt("ship", position);
						double roll = Game1.random.NextDouble();
						StardewValley.Object o = null;
						if (roll < 0.2f && Game1.player.eventsSeen.Contains(0))
						{
							o = new StardewValley.Object(832, 1); // Pineapple
							if (roll < 0.05f && Game1.player.eventsSeen.Contains(1))
								o = new StardewValley.Object(Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.Instance.ChocolateName), 1);
						}
						if (o != null)
							Game1.player.addItemByMenuIfNecessary(o.getOne());
					}
					break;
			}
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
				Dictionary<int, List<string>> recipes = ModEntry.CookingSkillApi.GetAllLevelUpRecipes();
				IEnumerable<string> missingRecipes = recipes.TakeWhile(pair => pair.Key < level)
						// Take all recipe lists up to the current level
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
					: SpaceCore.Skills.GetSkill(name) != null
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

		internal static void OpenNewCookingMenu(List<CraftingRecipe> recipes = null)
		{
			void CreateCookingMenu(NetRef<Chest> fridge, List<Chest> miniFridges)
			{
				var list = new List<Chest>();
				if (fridge.Value != null)
				{
					list.Add(fridge);
				}
				list.AddRange(miniFridges);

				Vector2 topLeftPositionForCenteringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(
					800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2);

				CraftingPage craftingMenu = new CraftingPage(
					(int)topLeftPositionForCenteringOnScreen.X, (int)topLeftPositionForCenteringOnScreen.Y,
					800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2,
					cooking: true, standalone_menu: true, material_containers: list);

				if (ModEntry.Config.AddCookingMenu)
				{
					if (!(Game1.activeClickableMenu is CookingMenu)
						|| (Game1.activeClickableMenu is CookingMenu menu && menu.PopMenuStack(true, true)))
					{
						Game1.activeClickableMenu = new CookingMenu(recipes ?? TakeRecipesFromCraftingPage(craftingMenu));
					}
				}
				else
				{
					Game1.activeClickableMenu = craftingMenu;
				}
			}

			if (CanUseKitchens())
			{
				Chest ccFridge = Game1.currentLocation is CommunityCenter ? Bundles.GetCommunityCentreFridge() : null;
				var fridge = new NetRef<Chest>();
				var muticies = new List<NetMutex>();
				var miniFridges = new List<Chest>();

				fridge.Set(Game1.currentLocation is FarmHouse farmHouse && GetFarmhouseKitchenLevel(farmHouse) > 0
					? farmHouse.fridge
					: Game1.currentLocation is IslandFarmHouse islandFarmHouse
						? islandFarmHouse.fridge
						: ccFridge != null ? new NetRef<Chest>(ccFridge) : null);

				foreach (StardewValley.Object obj in Game1.currentLocation.Objects.Values.Where(
					o => o != null && o.bigCraftable.Value && o is Chest && o.ParentSheetIndex == 216))
				{
					miniFridges.Add(obj as Chest);
					muticies.Add((obj as Chest).mutex);
				}
				if (fridge.Value != null && fridge.Value.mutex.IsLocked())
				{
					Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Kitchen_InUse"));
				}
				else if (fridge.Value == null)
				{
					CreateCookingMenu(fridge, miniFridges);
				}
				else
				{
					MultipleMutexRequest multiple_mutex_request = null;
					multiple_mutex_request = new MultipleMutexRequest(muticies, delegate
					{
						fridge.Value.mutex.RequestLock(delegate
						{
							CreateCookingMenu(fridge, miniFridges);
							Game1.activeClickableMenu.exitFunction = delegate
							{
								fridge.Value.mutex.ReleaseLock();
								multiple_mutex_request.ReleaseLocks();
							};
						}, delegate
						{
							Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Kitchen_InUse"));
							multiple_mutex_request.ReleaseLocks();
						});
					}, delegate
					{
						Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Kitchen_InUse"));
					});
				}
			}
			else
			{
				Game1.activeClickableMenu?.exitThisMenuNoSound();
				CreateInspectDialogue(ModEntry.Instance.i18n.Get("menu.cooking_station.no_cookbook"));
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
			string skill = ModEntry.Instance.Helper.Reflection.GetField<string>(menu, "currentSkill").GetValue();
			if (skill != CookingSkill.InternalName)
			{
				return;
			}
			int level = ModEntry.Instance.Helper.Reflection.GetField<int>(menu, "currentLevel").GetValue();
			List<CraftingRecipe> cookingRecipes = ModEntry.CookingSkillApi
				.GetCookingRecipesForLevel(level)
				.ConvertAll(name => new CraftingRecipe(ModEntry.ObjectPrefix + name, true))
				.Where(recipe => !Game1.player.knowsRecipe(recipe.name))
				.ToList();
			if (cookingRecipes != null && cookingRecipes.Count > 0)
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
			List<CraftingRecipe> combinedRecipes = craftingRecipes.Concat(cookingRecipes).ToList();
			ModEntry.Instance.Helper.Reflection.GetField<List<CraftingRecipe>>(menu, "newCraftingRecipes").SetValue(combinedRecipes);
			Log.D(combinedRecipes.Aggregate($"New recipes for level {level}:", (total, cur) => $"{total}\n{cur.name} ({cur.createItem().ParentSheetIndex})"),
				ModEntry.Config.DebugMode);

			// Adjust menu to fit if necessary
			const int defaultMenuHeightInRecipes = 4;
			int menuHeightInRecipes = combinedRecipes.Count + combinedRecipes.Count(recipe => recipe.bigCraftable);
			if (menuHeightInRecipes >= defaultMenuHeightInRecipes)
			{
				menu.height += (menuHeightInRecipes - defaultMenuHeightInRecipes) * 64;
			}
		}

		public static List<CraftingRecipe> TakeRecipesFromCraftingPage(CraftingPage cm, bool cookingOnly = true)
		{
			bool cooking = ModEntry.Instance.Helper.Reflection.GetField<bool>(cm, "cooking").GetValue();
			if (cooking || !cookingOnly)
			{
				var recipePages = ModEntry.Instance.Helper.Reflection.GetField
					<List<Dictionary<ClickableTextureComponent, CraftingRecipe>>>
					(cm, "pagesOfCraftingRecipes")
					.GetValue();
				cm.exitThisMenuNoSound();
				return recipePages.SelectMany(page => page.Values).ToList();
			}
			return null;
		}

		public static bool IsMinifridge(StardewValley.Object o)
		{
			return o != null && o.bigCraftable.Value && o is Chest && o.ParentSheetIndex == 216;
		}

		public static bool AreNettlesActive()
		{
			return ModEntry.NettlesEnabled && !Interface.Interfaces.UsingNettlesCrops;
		}

		/// <summary>
		/// Checks for if the player meets conditions to open the new cooking menu.
		/// Always true if using the default cooking menu.
		/// </summary>
		public static bool CanUseKitchens()
		{
			return !ModEntry.Config.AddCookingMenu || Game1.player.hasOrWillReceiveMail(ModEntry.MailCookbookUnlocked);
		}

		/// <summary>
		/// Identifies the level of the best cooking station within the player's use range.
		/// A cooking station's level influences the number of ingredients slots available to the player.
		/// </summary>
		/// <returns>Level of the best cooking station in range, defaults to 0.</returns>
		public static int GetNearbyCookingStationLevel()
		{
			int radius = int.Parse(ModEntry.ItemDefinitions["CookingUseRange"][0]);
			int cookingStationLevel = 0;

			// If indoors, use the farmhouse or cabin level as a base for cooking levels
			if (!Game1.currentLocation.IsOutdoors)
			{
				xTile.Layers.Layer layer = Game1.currentLocation.Map.GetLayer("Buildings");
				int xLimit = Game1.player.getTileX() + radius;
				int yLimit = Game1.player.getTileY() + radius;
				for (int x = Game1.player.getTileX() - radius; x < xLimit && cookingStationLevel == 0; ++x)
					for (int y = Game1.player.getTileY() - radius; y < yLimit && cookingStationLevel == 0; ++y)
					{
						xTile.Tiles.Tile tile = layer.Tiles[x, y];
						if (tile == null
							|| (Game1.currentLocation.doesTileHaveProperty(x, y, "Action", "Buildings") != "kitchen"
								&& !ModEntry.IndoorsTileIndexesThatActAsCookingStations.Contains(tile.TileIndex)))
							continue;

						switch (Game1.currentLocation)
						{
							case FarmHouse farmHouse:
								// FarmHouses use their upgrade level as a baseline after Robin installs a kitchen
								cookingStationLevel = GetFarmhouseKitchenLevel(farmHouse);
								break;
							default:
								// NPC kitchens (other than the Saloon) use the Farmer's ingredients limits only
								cookingStationLevel = GetFarmersMaxUsableIngredients();
								break;
						}

						Log.D($"Cooking station: {Game1.currentLocation.Name}: Kitchen (level {cookingStationLevel})",
							ModEntry.Config.DebugMode);
					}
			}
			else
			{
				int xLimit = Game1.player.getTileX() + radius;
				int yLimit = Game1.player.getTileY() + radius;
				for (int x = Game1.player.getTileX() - radius; x < xLimit && cookingStationLevel == 0; ++x)
					for (int y = Game1.player.getTileY() - radius; y < yLimit && cookingStationLevel == 0; ++y)
					{
						Game1.currentLocation.Objects.TryGetValue(new Vector2(x, y), out var o);
						if (o == null || (o.Name != "Campfire" && o.Name != ModEntry.CookingCraftableName))
							continue;
						cookingStationLevel = GetFarmersMaxUsableIngredients();
						Log.D($"Cooking station: {cookingStationLevel}",
							ModEntry.Config.DebugMode);
					}
			}
			Log.D("Cooking station search finished",
				ModEntry.Config.DebugMode);
			return cookingStationLevel;
		}

		/// <summary>
		/// Fetches the cooking station level for the farmhouse based on its upgrade/kitchen level,
		/// accounting for mods that would provide the kitchen at level 0.
		/// </summary>
		public static int GetFarmhouseKitchenLevel(FarmHouse farmHouse)
		{
			// A basic (modded) farmhouse has a maximum of 1 slot,
			// and a farmhouse with a kitchen has a minimum of 2+ slots
			int level = Math.Max(farmHouse.upgradeLevel, GetFarmersMaxUsableIngredients());
			if (farmHouse.upgradeLevel == 0 && Interface.Interfaces.UsingFarmhouseKitchenStart)
			{
				level = 1;
			}
			return level;
		}

		public static int GetFarmersMaxUsableIngredients()
		{
			return (ModEntry.Config.AddCookingToolProgression && ModEntry.Instance.States.Value.CookingToolLevel < 4)
				? 1 + ModEntry.Instance.States.Value.CookingToolLevel
				: 6;
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
			int debugCount = 0;
			for (int i = 0; i < itemList.Count; ++i)
			{
				// Ignore items without one of our group suffixes
				string suffix = suffixes.Keys.FirstOrDefault(s => itemList[i].Name.ToLower().EndsWith(s));
				if (suffix == null)
					continue;
				// Set the move-to-this-item name to be the first-found item in the group
				suffixes[suffix] ??= itemList[i].Name;
				if (suffixes[suffix] == itemList[i].Name)
					continue;
				// Move newly-found items of a group up to the first item in the group, and change the move-to name to this item
				ISalable item = itemList[i];
				int index = 1 + itemList.FindIndex(i => i.Name == suffixes[suffix]);
				itemList.RemoveAt(i);
				itemList.Insert(index, item);
				suffixes[suffix] = itemList[index].Name;
				++debugCount;
			}
			menu.forSale = itemList;
		}

		/// <summary>
		/// Update display names for all new cooking recipe objects
		/// With English locale, recipes' display names default to the internal name, so we have to replace it
		/// </summary>
		internal static void UpdateEnglishRecipeDisplayNames(ref List<CraftingRecipe> recipes)
		{
			if (LocalizedContentManager.CurrentLanguageCode.ToString() == "en")
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
					if (newEntry[i] != null)
						fields[startIndex + i] = append ? $"{fields[startIndex + i]} {newEntry[i]}" : newEntry[i];
			return string.Join(delimiter.ToString(), fields);
		}
	}
}
