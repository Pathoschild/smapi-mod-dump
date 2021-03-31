/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CooksAssistant
**
*************************************************/

using LoveOfCooking.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using PyTK.Extensions;
using SpaceCore;
using SpaceCore.Events;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Rectangle = Microsoft.Xna.Framework.Rectangle;


// TO DO THE MOST

//		// TO VERY MUCH DO

//				// ABSOLUTELY TO DO


//						// TODO: RELEASE: Copy assets folder from E:\Dev\Projects\SDV\Projects\CooksAssistant\temp_py_output to C:\SteamSSD\steamapps\common\Stardew Valley\Mods\LoveOfCooking


//			// DO THAT

// DON'T FORGET


// TODO: 1.0.13: Icon-based rewrite of ingredients dropIn slots
// TODO: FIX: Duplicating items when inventory full and cooking menu closes (ingredients brought from fridge into ingredients dropIn)

// TODO: FIX: CC Kitchen star doesn't show up on board for host when CC completed; empty star shows for peers (https://i.imgur.com/UZXTopu.png)
// TODO: FIX: Left-right inventory bundle menu navigation with/without kitchen and other bundles
// TODO: FIX: Bundle is unloaded overnight, CC is completed when all other areas are finished?

// TODO: UPDATE: Cooked food has a chance (scaling with Cooking level) of taking the quality of its ingredients,
//		Final quality is decided by random choice from list of qualities of each ingredient
// TODO: UPDATE: Finish the cookbook sprite bottom + spine and have the cookbook + inventory space outwards vertically on large resolutions
// TODO: UPDATE: Quests, events, and scripts
// TODO: UPDATE: Hot chocolate at the ice festival

// TODO: COMPATIBILITY: Remote Fridge Storage (https://www.nexusmods.com/stardewvalley/mods/2545)
//		Will need new checks for adding chests to the minifridge list, and display chest icon if true
// TODO: COMPATIBILITY: Limited Campfire Cooking (https://www.nexusmods.com/stardewvalley/mods/4971)
//		In DisplayMenuChanged intercept for CraftingPage, OpenCookingMenu is delayed a tick for mutex request on fridges
//		Campfires have their menu intercepted correctly, but no longer have the limited crafting recipe list passed on
// TODO: COMPATIBILITY: Skill Prestige (https://www.nexusmods.com/stardewvalley/mods/569)
// TODO: COMPATIBILITY: Level Extender (https://www.nexusmods.com/stardewvalley/mods/1471)
//		No current errors or issues, but doesn't interact, either
// TODO: COMPATIBILITY: Convenient Chests (https://www.nexusmods.com/stardewvalley/mods/2196)
// TODO: COMPATIBILITY: Tool Upgrade Delivery (https://www.nexusmods.com/stardewvalley/mods/5421)
// TODO: COMPATIBILITY: Expanded Fridge (https://forums.stardewvalley.net/threads/unofficial-mod-updates.2096/page-6#post-20884)
// TODO: COMPATIBILITY: Expanded Storage (https://www.nexusmods.com/stardewvalley/mods/7431)
// TODO: COMPATIBILITY: Food Buff Stacking (https://www.nexusmods.com/stardewvalley/mods/4321)


namespace LoveOfCooking
{
	public class ModEntry : Mod
	{
		internal static ModEntry Instance;
		internal Config Config;

		internal ITranslationHelper i18n => Helper.Translation;
		internal static IJsonAssetsApi JsonAssets;
		internal static ILevelExtenderAPI LevelExtender;
		internal static Texture2D SpriteSheet;

		public static ICookingSkillAPI CookingSkillApi;

		internal const string SaveDataKey = "SaveData";
		internal const string AssetPrefix = "blueberry.LoveOfCooking.";
		internal const string ObjectPrefix = "blueberry.cac.";
		internal const string MailPrefix = "blueberry.cac.mail.";
		internal const int NexusId = 6830;

		// Assets
		// Game content paths: asset keys sent as requests to Game1.content.Load<T>()
		// These can be intercepted and modified by AssetLoaders/Editors, eg. Content Patcher.
		internal static readonly string GameContentSpriteSheetPath = AssetPrefix + "Assets\\Sprites";
		internal static readonly string GameContentBundleDataPath = AssetPrefix + "Assets\\Bundles";
		internal static readonly string GameContentIngredientBuffDataPath = AssetPrefix + "Assets\\IngredientBuffChart";
		internal static readonly string GameContentDefinitionsPath = AssetPrefix + "Assets\\ItemDefinitions";
		internal static readonly string GameContentSkillValuesPath = AssetPrefix + "Assets\\CookingSkillValues";
		internal static readonly string GameContentSkillRecipeTablePath = AssetPrefix + "Assets\\CookingSkillLevelUpRecipes";
		// Local paths: filepaths without extension passed to Helper.Content.Load<T>()
		// These are the paths for our default data files bundled with the mod in our assets folder.
		internal static readonly string LocalSpriteSheetPath = Path.Combine("assets", "sprites");
		internal static readonly string LocalBundleDataPath = Path.Combine("assets", "bundles");
		internal static readonly string LocalIngredientBuffDataPath = Path.Combine("assets", "ingredientBuffChart");
		internal static readonly string LocalDefinitionsPath = Path.Combine("assets", "itemDefinitions");
		internal static readonly string LocalSkillValuesPath = Path.Combine("assets", "cookingSkillValues");
		internal static readonly string LocalSkillRecipeTablePath = Path.Combine("assets", "cookingSkillLevelUpRecipes");
		// Content pack paths: filepaths without extension passed to JsonAssets.LoadAssets()
		// These are again bundled with the mod, but aren't intended to be intercepted or changed.
		internal static readonly string BasicObjectsPack = Path.Combine("assets", "BasicObjectsPack");
		internal static readonly string NewRecipesPackPath = Path.Combine("assets", "NewRecipesPack");
		internal static readonly string NewCropsPackPath = Path.Combine("assets", "NewCropsPack");
		internal static readonly string NettlesPackPath = Path.Combine("assets", "NettlesPack");

		// Player session state
		public class State
		{
			// Persistent player data
			public int CookingToolLevel = 0;
			public bool IsUsingRecipeGridView = false;
			public List<string> FoodsEaten = new List<string>();
			public List<string> FavouriteRecipes = new List<string>();

			// Add Cooking Menu
			public CookingMenu.Filter LastFilterThisSession = CookingMenu.Filter.None;
			public bool LastFilterReversed;

			// Add Cooking Skill
			public readonly Dictionary<string, int> FoodCookedToday = new Dictionary<string, int>();

			// Food Healing Takes Time
			public float RegenSkillModifier;
			public int HealthOnLastTick;
			public float StaminaOnLastTick;
			public int HealthRegeneration, StaminaRegeneration;
			public uint RegenTicksCurr;
			public readonly Queue<uint> RegenTicksDiff = new Queue<uint>();
			public StardewValley.Object LastFoodEaten;
			public bool LastFoodWasDrink;
			// debug
			public float DebugRegenRate;
		}
		public readonly PerScreen<State> States = new PerScreen<State>(createNewState: () => new State());

		// Add Cooking Questline
		internal const string ActionDockCrate = AssetPrefix + "DockCrate";
		internal const string ActionRange = AssetPrefix + "Range";

		// Item Definitions
		internal static Dictionary<string, string> IngredientBuffChart;
		internal static Dictionary<string, List<string>> ItemDefinitions;

		// Others:
		// base game reference
		internal const int CookingCategory = -7;
		internal enum SkillIndex
		{
			Farming,
			Fishing,
			Foraging,
			Mining,
			Combat,
			Luck
		}
		// safe item names
		internal static string ChocolateName { get { return UsingPPJACrops ? "Chocolate Bar" : ObjectPrefix + "chocolate"; } }
		internal static string CabbageName { get { return UsingPPJACrops ? "Cabbage" : ObjectPrefix + "cabbage"; } }
		internal static string OnionName { get { return UsingPPJACrops ? "Onion" : ObjectPrefix + "onion"; } }
		internal static string CarrotName { get { return UsingPPJACrops ? "Carrot" : ObjectPrefix + "carrot"; } }
		internal static readonly string CookingCraftableName = ObjectPrefix + "cookingcraftable";
		// cook at kitchens
		internal static Dictionary<string, string> NpcHomeLocations;
		// kebab
		private const string KebabBuffSource = AssetPrefix + "kebab";
		private const int KebabBonusDuration = 220;
		private const int KebabMalusDuration = 140;
		private const int KebabCombatBonus = 3;
		private const int KebabNonCombatBonus = 2;

		// Mail titles
		internal static readonly string MailKitchenCompleted = $"cc{Bundles.CommunityCentreAreaName}";
		internal static readonly string MailCookbookUnlocked = MailPrefix + "cookbook_unlocked";
		internal static readonly string MailKitchenCompletedFollowup = MailPrefix + "kitchen_completed_followup";
		internal static readonly string MailKitchenLastBundleCompleteRewardDelivery = MailPrefix + "kitchen_reward_guarantee";
		internal static readonly string MailFryingPanWhoops = MailPrefix + "im_sorry_lol_pan";

		// Loaded mods
		internal static bool UsingPPJACrops { get => Instance.Helper.ModRegistry.IsLoaded("PPJA.FruitsAndVeggies"); }
		internal static bool UsingLevelExtender { get => Instance.Helper.ModRegistry.IsLoaded("Devin_Lematty.Level_Extender"); }
		internal static bool UsingFarmhouseKitchenStart
		{
			get
			{
				// thanks lenne!
				return new [] { "Allayna.Kitchen", "Froststar11.CustomFarmhouse", "burakmese.products", "minervamaga.FR.BiggerFarmhouses" }
					.Any(id => Instance.Helper.ModRegistry.IsLoaded(id));
			}
		}
		private double _totalSecondsOnLoaded;

		internal static readonly bool CiderEnabled = true;
		internal static readonly bool PerryEnabled = false;
		internal static readonly bool MarmaladeEnabled = false;
		internal static readonly bool NettlesEnabled = false;
		internal static readonly bool RedberriesEnabled = false;
		internal static readonly bool SendBundleFollowupMail = false;
		internal static readonly bool PrintRename = false;

		private void PrintConfig()
		{
			try
			{
				Log.D("\n== CONFIG SUMMARY ==\n"
					  + $"\nNew Cooking Menu:   {Config.AddCookingMenu}"
					  + $"\nNew CC Bundles:     {Config.AddCookingCommunityCentreBundles}"
					  + $"\nNew Cooking Skill:  {Config.AddCookingSkillAndRecipes}"
					  + $"\nNew Cooking Tool:   {Config.AddCookingToolProgression}"
					  + $"\nNew Crops & Stuff:  {Config.AddNewCropsAndStuff}"
					  + $"\nNew Recipe Scaling: {Config.AddRecipeRebalancing}"
					  + $"\nNew Buff Assigning: {Config.AddBuffReassigning}"
					  + $"\nCooking Animation:  {Config.PlayCookingAnimation}"
					  + $"\nHealing Takes Time: {Config.FoodHealingTakesTime}"
					  + $"\nHide Food Buffs:    {Config.HideFoodBuffsUntilEaten}"
					  + $"\nFood Can Burn:      {Config.FoodCanBurn}"
					  + $"\n-------------"
					  + $"\nDebugging:      {Config.DebugMode}"
					  + $"\nRegen tracker:  {Config.DebugRegenTracker}"
					  + $"\nCommand prefix: {Config.ConsoleCommandPrefix}"
					  + $"\nResize Korean:  {Config.ResizeKoreanFonts}\n",
					Config.DebugMode);
			}
			catch (Exception e)
			{
				Log.E($"Error in printing mod configuration.\n{e}");
			}
		}

		private void PrintModData()
		{
			try
			{
				Log.D("\n== LOCAL DATA ==\n"
					+ $"\nRecipeGridView:   {States.Value.IsUsingRecipeGridView}"
					+ $"\nCookingToolLevel: {States.Value.CookingToolLevel}"
					+ $"\nCanUpgradeTool:   {Tools.CanFarmerUpgradeCookingEquipment()}"
					+ $"\nFoodsEaten:       {States.Value.FoodsEaten.Aggregate("", (s, cur) => $"{s} ({cur})")}"
					+ $"\nFavouriteRecipes: {States.Value.FavouriteRecipes.Aggregate("", (s, cur) => $"{s} ({cur})")}\n"
					+ "-- OTHERS --"
					+ $"\nLanguage:         {LocalizedContentManager.CurrentLanguageCode.ToString().ToUpper()}"
					+ $"\nFarmHouseLevel:   {GetFarmhouseKitchenLevel(Game1.getLocationFromName("FarmHouse") as FarmHouse)}"
					+ $"\nNumberOfCabins:   {Bundles.GetNumberOfCabinsBuilt()}"
					+ $"\nMaxIngredients:   {GetFarmersMaxUsableIngredients()}"
					+ $"\nCookbookUnlockedMail: {Game1.player.mailReceived.Contains(MailCookbookUnlocked)}"
					+ $"\nBundleCompleteMail:   {Game1.player.mailReceived.Contains(MailKitchenCompleted)}"
					+ $"\nBundleFollowupMail:   {Game1.player.mailReceived.Contains(MailKitchenCompletedFollowup)}"
					+ $"\nFryingPanWhoopsMail:  {Game1.player.mailReceived.Contains(MailFryingPanWhoops)}\n",
					Config.DebugMode);
			}
			catch (Exception e)
			{
				Log.E($"Error in printing mod save data.\n{e}");
			}
		}

		private void PrintCookingSkill()
		{
			if (!Config.AddCookingSkillAndRecipes)
			{
				Log.D("Cooking skill is disabled in mod config.",
					Config.DebugMode);
			}
			else if (CookingSkillApi.GetSkill() == null)
			{
				Log.D("Cooking skill is enabled, but skill is not loaded.",
					Config.DebugMode);
			}
			else
			{
				try
				{
					var level = CookingSkillApi.GetLevel();
					var current = CookingSkillApi.GetTotalCurrentExperience();
					var total = CookingSkillApi.GetTotalExperienceRequiredForLevel(level + 1);
					var remaining = CookingSkillApi.GetExperienceRemainingUntilLevel(level + 1);
					var required = CookingSkillApi.GetExperienceRequiredForLevel(level + 1);
					Log.D("\n== COOKING SKILL ==\n"
						+ $"\nID: {CookingSkillApi.GetSkill().GetName()}"
						+ $"\nCooking level: {level}"
						+ $"\nExperience until next level: ({required - remaining}/{required})"
						+ $"\nTotal experience: ({current}/{total})\n",
						Config.DebugMode);
				}
				catch (Exception e)
				{
					Log.E($"Error in printing custom skill data.\n{e}");
				}
			}
		}


		public override void Entry(IModHelper helper)
		{
			Instance = this;
			Config = helper.ReadConfig<Config>();
			PrintConfig();

			try
			{
				// Apply harmony patches
				Core.HarmonyPatches.HarmonyPatches.Patch();
			}
			catch (Exception e)
			{
				Log.E("Error in applying Harmony patches:\n" + e);
			}

			// Asset editors
			var assetManager = new AssetManager();
			Helper.Content.AssetLoaders.Add(assetManager);
			Helper.Content.AssetEditors.Add(assetManager);

			// Game events
			Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
			Helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
			Helper.Events.GameLoop.Saving += GameLoop_Saving;
			Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
			Helper.Events.GameLoop.ReturnedToTitle += GameLoop_ReturnedToTitle;
			Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
			Helper.Events.Player.InventoryChanged += Player_InventoryChanged;
			Helper.Events.Input.ButtonPressed += Input_ButtonPressed;
			Helper.Events.Display.MenuChanged += Display_MenuChanged;
			Helper.Events.Multiplayer.PeerContextReceived += Multiplayer_PeerContextReceived;
			Helper.Events.Multiplayer.PeerConnected += Multiplayer_PeerConnected;
			Bundles.RegisterEvents();
			Tools.RegisterEvents();

			if (Config.DebugMode && Config.DebugRegenTracker)
			{
				Helper.Events.Display.RenderedHud += Event_DrawDebugRegenTracker;
			}
			SpaceEvents.OnItemEaten += SpaceEvents_ItemEaten;
			SpaceEvents.BeforeGiftGiven += SpaceEvents_BeforeGiftGiven;

			AddConsoleCommands();
		}

		private void LoadJsonAssetsObjects()
		{
			JsonAssets = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
			if (JsonAssets == null)
			{
				Log.E("Can't access the Json Assets API. Is the mod installed correctly?");
				return;
			}

			if (Config.DebugMode)
				Log.W("Loading Basic Objects Pack.");
			JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, BasicObjectsPack));

			if (!Config.AddCookingSkillAndRecipes)
			{
				Log.W("Did not add new recipes: Recipe additions are disabled in config file.");
			}
			else
			{
				if (Config.DebugMode)
					Log.W("Loading New Recipes Pack.");
				JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, NewRecipesPackPath));
			}

			if (!Config.AddNewCropsAndStuff)
			{
				Log.W("Did not add new objects: New stuff is disabled in config file.");
				return;
			}
			else if (UsingPPJACrops)
			{
				Log.I("Did not add new crops: [PPJA] Fruits and Veggies already adds these objects.");
			}
			else
			{
				if (Config.DebugMode)
					Log.W("Loading New Crops Pack.");
				JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, NewCropsPackPath));
			}

			if (Helper.ModRegistry.IsLoaded("uberkwefty.wintercrops"))
			{
				Log.I("Did not add nettles: Winter Crops is enabled.");
			}
			else
			{
				if (Config.DebugMode)
					Log.W("Loading Nettles Pack.");
				JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, NettlesPackPath));
			}
		}

		private void LoadModConfigMenuElements()
		{
			var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
			if (api == null)
			{
				return;
			}

			api.RegisterModConfig(ModManifest, () => Config = new Config(), () => Helper.WriteConfig(Config));

			var entries = new []
			{
				"features",

				"AddCookingMenu",
				"AddCookingCommunityCentreBundles",
				"AddCookingSkillAndRecipes",
				"AddCookingToolProgression",
				//"AddCookingQuestline",
				"AddNewCropsAndStuff",

				"changes",

				"PlayCookingAnimation",
				"AddRecipeRebalancing",
				"AddBuffReassigning",
				"HideFoodBuffsUntilEaten",
				"FoodHealingTakesTime",
				"FoodCanBurn",

				"others",

				"ResizeKoreanFonts",
			};
			foreach (var entry in entries)
			{
				var flags = BindingFlags.Public | BindingFlags.Instance;
				var property = typeof(Config).GetProperty(entry, flags);
				if (property != null)
				{
					var i18nKey = $"config.option.{entry.ToLower()}_";
					api.RegisterSimpleOption(ModManifest, i18n.Get(i18nKey + "name"), i18n.Get(i18nKey + "description"),
						() => (bool)property.GetValue(Config), (bool value) => property.SetValue(Config, value));
				}
				else
				{
					var i18nKey = $"config.{entry}_";
					api.RegisterLabel(ModManifest, i18n.Get(i18nKey + "label"), null);
				}
			}
		}

		private void LoadLevelExtenderApi()
		{
			if (UsingLevelExtender)
			{
				try
				{
					LevelExtender = Helper.ModRegistry.GetApi<ILevelExtenderAPI>("Devin_Lematty.Level_Extender");
				}
				catch (Exception e)
				{
					Log.T("Encountered exception in reading ILevelExtenderAPI from LEApi:");
					Log.T("" + e);
				}
				finally
				{
					if (LevelExtender == null)
					{
						Log.W("Level Extender is loaded, but the API was inaccessible.");
					}
				}
			}
		}

		private void RegisterSkillsWithLevelExtender()
		{
			LevelExtender.initializeSkill(
				name: CookingSkill.InternalName,
				xp: CookingSkillApi.GetTotalCurrentExperience(),
				xp_mod: CookingSkill.GlobalExperienceRate,
				xp_table: CookingSkillApi.GetSkill().ExperienceCurve.ToList(), cats: null);
		}

		private void AddConsoleCommands()
		{
			var cmd = Config.ConsoleCommandPrefix;

			Bundles.AddConsoleCommands(cmd);
			Tools.AddConsoleCommands(cmd);

			Helper.ConsoleCommands.Add(cmd + "menu", "Open cooking menu.", (s, args) =>
			{
				if (!PlayerAgencyLostCheck())
					OpenNewCookingMenu(null);
			});
			Helper.ConsoleCommands.Add(cmd + "lvl", "Set cooking level.", (s, args) =>
			{
				if (!Config.AddCookingSkillAndRecipes)
				{
					Log.D("Cooking skill is not enabled.");
					return;
				}
				if (args.Length < 1)
					return;

				// Update experience
				if (args.Length > 1)
				{
					var exp = Helper.Reflection.GetField<Dictionary<long, Dictionary<string, int>>>(typeof(Skills), "exp");
					var dict = exp.GetValue();
					if (args[1].ToLower() == "r")
					{
						Log.D("Resetting level.");
						dict[Game1.player.UniqueMultiplayerID][CookingSkill.InternalName] = 0;
					}
					exp.SetValue(dict);
				}

				// Reset recipes
				if (args.Length > 1)
				{
					var recipes = CookingSkill.CookingSkillLevelUpRecipes.Values.Aggregate(new List<string>(), (list, cur) => list.Concat(cur).ToList());
					foreach (var recipe in recipes)
					{
						if (Game1.player.cookingRecipes.Keys.Contains(ObjectPrefix + recipe))
						{
							Game1.player.cookingRecipes.Remove(ObjectPrefix + recipe);
						}
					}
				}

				// Add to current level
				var level = CookingSkillApi.GetLevel();
				var target = Math.Min(10, level + int.Parse(args[0]));
				CookingSkillApi.AddExperienceDirectly(CookingSkillApi.GetTotalExperienceRequiredForLevel(target) - CookingSkillApi.GetTotalCurrentExperience());

				// Update professions
				foreach (var profession in CookingSkillApi.GetSkill().Professions)
					if (Game1.player.professions.Contains(profession.GetVanillaId()))
						Game1.player.professions.Remove(profession.GetVanillaId());

				Log.D($"Set Cooking skill to {CookingSkillApi.GetLevel()}");
			});
			Helper.ConsoleCommands.Add(cmd + "tool", "Set cooking tool level.", (s, args) =>
			{
				if (!Config.AddCookingToolProgression)
				{
					Log.D("Cooking tool is not enabled.");
					return;
				}
				if (args.Length < 1)
					return;

				States.Value.CookingToolLevel = int.Parse(args[0]);
				Log.D($"Set Cooking tool to {States.Value.CookingToolLevel}");
			});
			Helper.ConsoleCommands.Add(cmd + "lvlmenu", "Show cooking level menu.", (s, args) =>
			{
				if (!Config.AddCookingSkillAndRecipes)
				{
					Log.D("Cooking skill is not enabled.");
					return;
				}
				Helper.Reflection.GetMethod(typeof(CookingSkill), "showLevelMenu").Invoke(
					null, new EventArgsShowNightEndMenus());
				Log.D("Bumped Cooking skill levelup menu.");
			});
			Helper.ConsoleCommands.Add(cmd + "tired", "Reduce health and stamina. Pass zero, one, or two values.", (s, args) =>
			{
				if (args.Length < 1)
				{
					Game1.player.health = Game1.player.maxHealth / 10;
					Game1.player.Stamina = Game1.player.MaxStamina / 10;
				}
				else
				{
					Game1.player.health = int.Parse(args[0]);
					Game1.player.Stamina = args.Length < 2 ? Game1.player.health * 2.5f : int.Parse(args[1]);
				}
				Log.D($"Set HP: {Game1.player.health}, EP: {Game1.player.Stamina}");
			});
			Helper.ConsoleCommands.Add(cmd + "recipes", "Show all unlocked player recipes.", (s, args) =>
			{
				var message = Game1.player.cookingRecipes.Keys.OrderBy(str => str).Aggregate("Cooking recipes:", (cur, str) => $"{cur}\n{str}");
				Log.D(message);
			});
			Helper.ConsoleCommands.Add(cmd + "unlearn", "Remove all unlocked player recipes.", (s, args) =>
			{
				var recipes = CookingSkill.CookingSkillLevelUpRecipes.Values.Aggregate(new List<string>(), (list, cur) => list.Concat(cur).ToList());
				foreach (var recipe in recipes)
				{
					if (Game1.player.cookingRecipes.Keys.Contains(ObjectPrefix + recipe))
					{
						Game1.player.cookingRecipes.Remove(ObjectPrefix + recipe);
						Log.D($"Removed {recipe}.");
					}
				}

				var message = Game1.player.cookingRecipes.Keys.OrderBy(str => str).Aggregate("Cooking recipes:", (cur, str) => $"{cur}\n{str}");
				Log.D(message);
			});
			Helper.ConsoleCommands.Add(cmd + "purgerecipes", "Remove all invalid player recipes.", (s, args) =>
			{
				var validRecipes = Game1.content.Load<Dictionary<string, string>>("Data/CookingRecipes");
				var invalidRecipes = Game1.player.cookingRecipes.Keys.Where(key => !validRecipes.ContainsKey(key)).ToList();
				foreach (var recipe in invalidRecipes)
				{
					Game1.player.cookingRecipes.Remove(recipe);
					Log.D($"Removed {recipe}.");
				}

				var message = Game1.player.cookingRecipes.Keys.OrderBy(str => str).Aggregate("Cooking recipes:", (cur, str) => $"{cur}\n{str}");
				Log.D(message);
			});
			Helper.ConsoleCommands.Add(cmd + "anim", "Animate for generic or specific food.", (s, args) =>
			{
				CookingMenu.AnimateForRecipe(recipe: new CraftingRecipe(args.Length > 0 ? args[0] : "Fried Egg", true),
					quantity: 1, burntCount: 0, containsFish: false);
			});
			Helper.ConsoleCommands.Add(cmd + "book", "Flag cookbook mail as read, allowing kitchens to be used.", (s, args) =>
			{
				if (!Game1.player.hasOrWillReceiveMail(MailCookbookUnlocked))
				{
					Game1.player.mailReceived.Add(MailCookbookUnlocked);
				}
				Log.D($"Added cookbook: {Game1.player.hasOrWillReceiveMail(MailCookbookUnlocked)}");
			});
			Helper.ConsoleCommands.Add(cmd + "clearinbox", "Remove all notifications from your inbox.", (s, p) =>
			{
				var count = NotificationMenu.PendingNotifications.Count;
				NotificationMenu.PendingNotifications.Clear();
				Log.D($"Cleared {count} notifications: {NotificationMenu.PendingNotifications.Count <= 0}");
			});
			Helper.ConsoleCommands.Add(cmd + "inbox", "Bring up the Notification Menu.", (s, p) =>
			{
				Game1.activeClickableMenu = new NotificationMenu();
			});
			Helper.ConsoleCommands.Add(cmd + "unstuck", "Unlocks player movement if stuck in animations.", (s, args) =>
			{
				if (Game1.activeClickableMenu is CookingMenu || Game1.activeClickableMenu is NotificationMenu)
				{
					Game1.activeClickableMenu.emergencyShutDown();
				}
				Game1.player.Halt();
				Game1.player.completelyStopAnimatingOrDoingAction();
				Game1.player.faceDirection(2);
				Game1.player.Position = Utility.recursiveFindOpenTileForCharacter(Game1.player, Game1.currentLocation, Game1.player.getTileLocation(), 10) * 64;
				Game1.freezeControls = false;
			});
			Helper.ConsoleCommands.Add(cmd + "printconfig", "Print config state.", (s, args) =>
			{
				PrintConfig();
			});
			Helper.ConsoleCommands.Add(cmd + "printsave", "Print save data state.", (s, args) =>
			{
				PrintModData();
			});
			Helper.ConsoleCommands.Add(cmd + "printskill", "Print skill state.", (s, args) =>
			{
				PrintCookingSkill();
			});
		}

		private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
		{
			// Load assets after mods and asset editors have been registered to allow for patches, correct load orders
			Helper.Events.GameLoop.OneSecondUpdateTicked += Event_LoadAssetsLate;
		}

		private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			SaveLoadedBehaviours();
		}

		private void GameLoop_Saving(object sender, SavingEventArgs e)
		{
			// Save persistent player data to player
			Game1.player.modData[AssetPrefix + "grid_view"] = States.Value.IsUsingRecipeGridView.ToString();
			Game1.player.modData[AssetPrefix + "tool_level"] = States.Value.CookingToolLevel.ToString();
			Game1.player.modData[AssetPrefix + "foods_eaten"] = string.Join(",", States.Value.FoodsEaten);
			Game1.player.modData[AssetPrefix + "favourite_recipes"] = string.Join(",", States.Value.FavouriteRecipes);

			// Save local (and/or persistent) community centre data
			Log.D("Unloading world bundle data at end of day.",
				Config.DebugMode);
			Bundles.SaveAndUnloadBundleData();
		}

		private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
		{
			// Perform OnSaveLoaded behaviours when starting a new game
			var isNewGame = Game1.dayOfMonth == 1 && Game1.currentSeason == "spring" && Game1.year == 1;
			if (isNewGame)
			{
				SaveLoadedBehaviours();
			}

			// Add any missing starting recipes
			foreach (var recipe in CookingSkill.StartingRecipes)
			{
				if (!Game1.player.cookingRecipes.ContainsKey(recipe))
				{
					Log.D($"Added missing starting recipe {recipe}",
						Config.DebugMode);
					Game1.player.cookingRecipes.Add(recipe, 0);
				}
			}

			// Add or remove vanilla campfire recipe
			if (CookingSkillApi.IsEnabled())
			{
				if (CookingSkillApi.GetLevel() < CookingSkill.CraftCampfireLevel)
				{
					// Campfire is added on level-up for cooking skill users
					Game1.player.craftingRecipes.Remove("Campfire");
				}
				else if (!Game1.player.craftingRecipes.ContainsKey("Campfire"))
				{
					// Re-add campfire to the player's recipe list if it's otherwise missing
					Game1.player.craftingRecipes["Campfire"] = 0;
				}
			}

			if (Config.AddCookingSkillAndRecipes)
			{
				// Clear daily cooking to free up Cooking experience gains
				States.Value.FoodCookedToday.Clear();

				// Add any missing recipes from the level-up recipe table
				var level = CookingSkillApi.GetLevel();
				var recipes = CookingSkillApi.GetAllLevelUpRecipes();
				var missingRecipes = recipes.TakeWhile(pair => pair.Key < level) // Take all recipe lists up to the current level
					.SelectMany(pair => pair.Value) // Flatten recipe lists into their recipes
					.Where(r => !Game1.player.cookingRecipes.ContainsKey(r)); // Take recipes not known by the player
				foreach (var recipe in missingRecipes)
				{
					Log.D($"Added missing recipe {recipe}",
						Config.DebugMode);
					Game1.player.cookingRecipes.Add(recipe, 0);
				}
			}

			// Add the cookbook for the player once they've reached the unlock date
			// Internally day and month are zero-indexed, but are one-indexed in data file for consistency with year
			var day = int.Parse(ItemDefinitions["CookbookMailDate"][0]) - 1;
			var month = int.Parse(ItemDefinitions["CookbookMailDate"][1]) - 1;
			var year = int.Parse(ItemDefinitions["CookbookMailDate"][2]);
			var gameMonth = Utility.getSeasonNumber(Game1.currentSeason);
			var reachedNextYear = (Game1.year > year);
			var reachedNextMonth = (Game1.year == year && gameMonth > month);
			var reachedMailDate = (Game1.year == year && gameMonth == month && Game1.dayOfMonth >= day);
			if (Config.AddCookingMenu
				&& !Game1.player.hasOrWillReceiveMail(MailCookbookUnlocked)
				&& (reachedNextYear || reachedNextMonth || reachedMailDate))
			{
				Game1.player.mailbox.Add(MailCookbookUnlocked);
			}

			// Attempt to place a wild nettle as forage around other weeds
			if (NettlesEnabled && (Game1.currentSeason == "summer" || ((Game1.currentSeason == "spring" || Game1.currentSeason == "fall") && Game1.dayOfMonth % 2 == 0)))
			{
				foreach (var l in ItemDefinitions["NettlesLocations"])
				{
					var location = Game1.getLocationFromName(l);
					var tile = location.getRandomTile();
					location.Objects.TryGetValue(tile, out var o);
					tile = Utility.getRandomAdjacentOpenTile(tile, location);
					if (tile == Vector2.Zero || o == null || o.ParentSheetIndex < 312 || o.ParentSheetIndex > 322)
						continue;
					location.terrainFeatures.Add(tile, new CustomBush(tile, location, CustomBush.BushVariety.Nettle));
				}
			}

			// Calculate food regeneration rate from skill levels
			var scalingCurrent = new float[ItemDefinitions["RegenSkillModifiers"].Count];
			var scalingMax = new float[ItemDefinitions["RegenSkillModifiers"].Count];
			for (var i = 0; i < ItemDefinitions["RegenSkillModifiers"].Count; ++i)
			{
				var split = ItemDefinitions["RegenSkillModifiers"][i].Split(':');
				var name = split[0];
				var isDefined = Enum.TryParse(name, out SkillIndex skillIndex);
				var level = isDefined
					? Game1.player.GetSkillLevel((int)Enum.Parse(typeof(SkillIndex), name))
					: SpaceCore.Skills.GetSkill(name) != null
						? Game1.player.GetCustomSkillLevel(name)
						: -1;
				var value = float.Parse(split[1]);
				if (level < 0)
					continue;
				scalingCurrent[i] = level * value;
				scalingMax[i] = 10 * value;
			}
			States.Value.RegenSkillModifier = scalingCurrent.Sum() / scalingMax.Sum();
		}

		private void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
		{
			// Reset session state
			States.Value = new State();
		}

		private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (!Config.FoodHealingTakesTime)
			{
				return;
			}

			// Track player HP/EP to use in reverting instant food healing
			if (Game1.player != null && Context.IsWorldReady)
			{
				States.Value.HealthOnLastTick = Game1.player.health;
				States.Value.StaminaOnLastTick = Game1.player.Stamina;
			}

			// Game not paused:
			if ((!Game1.IsMultiplayer && !Game1.game1.IsActive) || (Game1.activeClickableMenu != null && !Game1.shouldTimePass()))
				return;

			// Check to regenerate HP/EP for player over time
			if (States.Value.HealthRegeneration < 1 && States.Value.StaminaRegeneration < 1)
				return;

			if (Game1.player.health < 1)
			{
				States.Value.HealthRegeneration = 0;
				States.Value.StaminaRegeneration = 0;
				States.Value.HealthOnLastTick = 1;
				States.Value.StaminaOnLastTick = 1;
				return;
			}

			var cookingLevel = CookingSkillApi.GetLevel();
			var panicMultiplier = (Game1.player.health * 3f + Game1.player.Stamina) / (Game1.player.maxHealth * 3f + Game1.player.MaxStamina);
			var foodMultiplier = GetFoodRegenRate(States.Value.LastFoodEaten);
			var baseRate = int.Parse(ItemDefinitions["RegenBaseRate"][0]);
			var rate = (baseRate - baseRate * States.Value.RegenSkillModifier) * foodMultiplier * 100d;
			rate = Math.Floor(Math.Max(36 - cookingLevel * 1.75f, rate * panicMultiplier));

			States.Value.DebugRegenRate = (float)rate;
			++States.Value.RegenTicksCurr;

			if (States.Value.RegenTicksCurr < rate)
				return;

			States.Value.RegenTicksDiff.Enqueue(States.Value.RegenTicksCurr);
			if (States.Value.RegenTicksDiff.Count > 5)
				States.Value.RegenTicksDiff.Dequeue();
			States.Value.RegenTicksCurr = 0;

			if (States.Value.HealthRegeneration > 0)
			{
				if (Game1.player.health < Game1.player.maxHealth)
					++Game1.player.health;
				--States.Value.HealthRegeneration;
			}

			if (States.Value.StaminaRegeneration > 0)
			{
				if (Game1.player.Stamina < Game1.player.MaxStamina)
					++Game1.player.Stamina;
				--States.Value.StaminaRegeneration;
			}
		}

		private void Event_LoadAssetsLate(object sender, OneSecondUpdateTickedEventArgs e)
		{
			Helper.Events.GameLoop.OneSecondUpdateTicked -= Event_LoadAssetsLate;

			IngredientBuffChart = Game1.content.Load<Dictionary<string, string>>(GameContentIngredientBuffDataPath);
			ItemDefinitions = Game1.content.Load<Dictionary<string, List<string>>>(GameContentDefinitionsPath);
			SpriteSheet = Game1.content.Load<Texture2D>(GameContentSpriteSheetPath);
			CookingSkillApi = new CookingSkillAPI(Helper.Reflection);
			if (Config.AddCookingSkillAndRecipes)
			{
				SpaceCore.Skills.RegisterSkill(new CookingSkill());
			}

			// Load custom objects now that mod-provided APIs are available
			LoadJsonAssetsObjects();
			// Load GMCM menu elements
			LoadModConfigMenuElements();
			// Load Level Extender if enabled
			LoadLevelExtenderApi();
		}

		private void Event_RegisterLevelExtenderLate(object sender, OneSecondUpdateTickedEventArgs e)
		{
			// LevelExtender/LEModApi.cs:
			// Please [initialise skill] ONCE in the Save Loaded event (to be safe, PLEASE ADD A 5 SECOND DELAY BEFORE initialization)
			if (Game1.currentGameTime.TotalGameTime.TotalSeconds - _totalSecondsOnLoaded >= 5)
			{
				Helper.Events.GameLoop.OneSecondUpdateTicked -= this.Event_RegisterLevelExtenderLate;
				RegisterSkillsWithLevelExtender();
			}
		}

		private void Event_ReplaceCraftingMenu(object sender, UpdateTickedEventArgs e)
		{
			Helper.Events.GameLoop.UpdateTicked -= Event_ReplaceCraftingMenu;
			OpenNewCookingMenu();
		}

		private void Event_DrawDebugRegenTracker(object sender, RenderedHudEventArgs e)
		{
			for (var i = 0; i < States.Value.RegenTicksDiff.Count; ++i)
			{
				e.SpriteBatch.DrawString(
					Game1.smallFont,
					$"{(i == 0 ? "DIFF" : "      ")}   {States.Value.RegenTicksDiff.ToArray()[States.Value.RegenTicksDiff.Count - 1 - i]}",
					new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width - 222, Game1.graphics.GraphicsDevice.Viewport.Height - 144 - i * 24),
					Color.White * ((States.Value.RegenTicksDiff.Count - 1 - i + 1f) / (States.Value.RegenTicksDiff.Count / 2f)));
			}
			e.SpriteBatch.DrawString(
				Game1.smallFont,
				$"CUR  {States.Value.RegenTicksCurr}",
				new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width - 222, Game1.graphics.GraphicsDevice.Viewport.Height - 120),
				Color.White);
			e.SpriteBatch.DrawString(
				Game1.smallFont,
				$"RATE {States.Value.DebugRegenRate}",
				new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width - 222, Game1.graphics.GraphicsDevice.Viewport.Height - 96),
				Color.White);
			e.SpriteBatch.DrawString(
				Game1.smallFont,
				$"HP+   {States.Value.HealthRegeneration}",
				new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width - 222, Game1.graphics.GraphicsDevice.Viewport.Height - 72),
				Color.White);
			e.SpriteBatch.DrawString(
				Game1.smallFont,
				$"EP+   {States.Value.StaminaRegeneration}",
				new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width - 222, Game1.graphics.GraphicsDevice.Viewport.Height - 48),
				Color.White);
		}

		private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			if (!Game1.game1.IsActive || Game1.currentLocation == null)
			{
				return;
			}

			// World interactions
			if (PlayerAgencyLostCheck())
				return;

			// Debug hotkeys
			if (Config.DebugMode)
			{
				switch (e.Button)
				{
					case SButton.G:
						Game1.player.warpFarmer(Game1.currentLocation is CommunityCenter
							? new Warp(0, 0, "FarmHouse", 0, 0, false)
							: new Warp(0, 0, "CommunityCenter", 7, 7, false));
						return;
					case SButton.H:
						if (Game1.activeClickableMenu is CookingMenu cookingMenu)
							cookingMenu.exitThisMenu();
						else
							OpenNewCookingMenu(null);
						return;
					case SButton.F5:
						Game1.currentLocation.largeTerrainFeatures.Add(
							new Bush(e.Cursor.GrabTile, 1, Game1.currentLocation));
						return;
					case SButton.F6:
						Game1.currentLocation.terrainFeatures.Add(e.Cursor.GrabTile,
							new CustomBush(e.Cursor.GrabTile, Game1.currentLocation, CustomBush.BushVariety.Nettle));
						return;
					case SButton.F7:
						Game1.currentLocation.largeTerrainFeatures.Add(
							new CustomBush(e.Cursor.GrabTile, Game1.currentLocation, CustomBush.BushVariety.Redberry));
						return;
				}
			}

			// World interactions:
			if (Game1.currentBillboard != 0 || Game1.activeClickableMenu != null || Game1.menuUp // No menus
			    || !Game1.player.CanMove) // Player agency enabled
				return;

			if (Game1.currentLocation != null && !Game1.currentLocation.IsOutdoors && Game1.player.ActiveObject?.Name == CookingCraftableName
				&& (e.Button.IsActionButton() || e.Button.IsUseToolButton()))
			{
				// Block the portable grill from being placed indoors
				Game1.playSound("cancel");
				Game1.showRedMessage(i18n.Get("world.cooking_craftable.rejected_indoors"));
				Helper.Input.Suppress(e.Button);
			}

			if (e.Button.IsActionButton())
			{
				// Tile actions
				var tile = Game1.currentLocation.Map.GetLayer("Buildings")
					.Tiles[(int)e.Cursor.GrabTile.X, (int)e.Cursor.GrabTile.Y];
				if (tile != null)
				{
					// Try to open a cooking menu when nearby to cooking stations (ie. kitchen, range)
					if (tile.Properties.Any(p => p.Key == "Action") && tile.Properties.FirstOrDefault(p => p.Key == "Action").Value == "kitchen")
					{
						OpenNewCookingMenu();
						Helper.Input.Suppress(e.Button);
					}
					else if (!Game1.currentLocation.IsOutdoors && ItemDefinitions["IndoorsTileIndexesThatActAsCookingStations"].Contains(tile.TileIndex.ToString()))
					{
						if (NpcHomeLocations.Any(pair => pair.Value == Game1.currentLocation.Name
								&& Game1.player.getFriendshipHeartLevelForNPC(pair.Key) >= int.Parse(ItemDefinitions["NpcKitchenFriendshipRequired"][0]))
							|| NpcHomeLocations.All(pair => pair.Value != Game1.currentLocation.Name))
						{
							Log.D($"Clicked the kitchen at {Game1.currentLocation.Name}",
								Config.DebugMode);
							if (Game1.player.team.specialOrders.Any(order => order != null && order.objectives.Any(
								obj => obj is DonateObjective dobj && dobj.dropBox.Value.EndsWith("Kitchen"))))
							{
								// Avoid blocking the player from submitting items to special order dropboxes
								return;
							}
							OpenNewCookingMenu();
							Helper.Input.Suppress(e.Button);
						}
						else
						{
							var name = NpcHomeLocations.FirstOrDefault(pair => pair.Value == Game1.currentLocation.Name).Key;
							Game1.showRedMessage(i18n.Get("world.range_npc.rejected",
								new { name = Game1.getCharacterFromName(name).displayName }));
						}
					}
				}

				if (Game1.currentLocation.Objects.ContainsKey(e.Cursor.GrabTile)
					&& Game1.currentLocation.Objects[e.Cursor.GrabTile].Name == CookingCraftableName)
				{
					Game1.playSound("bigSelect");
					OpenNewCookingMenu();
					Helper.Input.Suppress(e.Button);
				}

				// Use tile actions in maps
				CheckTileAction(e.Cursor.GrabTile, Game1.currentLocation);
			}
			else if (e.Button.IsUseToolButton())
			{
				// Ignore Nettles used on Kegs to make Nettle Tea when Cooking skill level is too low
				if ((!Config.AddCookingSkillAndRecipes || CookingSkillApi.GetLevel() < int.Parse(ItemDefinitions["NettlesUsableLevel"][0]))
					&& Game1.player.ActiveObject != null
					&& Game1.player.ActiveObject.Name.ToLower().EndsWith("nettles")
					&& ItemDefinitions["NettlesUsableMachine"].Contains(Game1.currentLocation.Objects[e.Cursor.GrabTile]?.Name))
				{
					Helper.Input.Suppress(e.Button);
					Game1.playSound("cancel");
				}
			}
		}

		private void Display_Rendered(object sender, RenderedEventArgs e)
		{
			// Draw cooking animation sprites
			if (Config.PlayCookingAnimation)
			{
				Game1.currentLocation.getTemporarySpriteByID(CookingMenu.SpriteId)?.draw(e.SpriteBatch, localPosition: false, xOffset: 0, yOffset: 0, extraAlpha: 1f);
			}

			// Render the correct English display name in crafting pages over the top of the incorrect display name.
			// Default game draw logic uses internal name for recipes in English locales, ignoring display name altogether.
			// 
			// Also applies to NewCraftingPage instances, found in spacechase0.CookingSkill:
			// https://github.com/spacechase0/CookingSkill/blob/master/NewCraftingPage.cs
			if (Game1.activeClickableMenu == null)
			{
				return;
			}
			var isEnglish = LocalizedContentManager.CurrentLanguageCode.ToString() == "en";
			var isCraftingMenu = Game1.activeClickableMenu is CraftingPage || Game1.activeClickableMenu.GetType().Name == "NewCraftingPage";
			var isCookingMenu = false;
			IReflectedField<bool> isCookingFlag;
			if (isCraftingMenu)
			{
				isCookingFlag = Helper.Reflection.GetField<bool>(Game1.activeClickableMenu, "cooking");
				isCookingMenu = isCookingFlag != null && isCookingFlag.GetValue();
			}
			if (!isCraftingMenu || !isCookingMenu || !isEnglish)
			{
				return;
			}

			var craftingMenu = Game1.activeClickableMenu;
			var heldItem = Helper.Reflection.GetField<Item>(craftingMenu, "heldItem").GetValue();
			var hoverRecipe = Helper.Reflection.GetField<CraftingRecipe>(craftingMenu, "hoverRecipe").GetValue();
			var lastCookingHover = Helper.Reflection.GetField<Item>(craftingMenu, "lastCookingHover").GetValue();
			if (lastCookingHover != null && hoverRecipe != null && hoverRecipe.name.StartsWith(ObjectPrefix))
			{
				var displayName = lastCookingHover.DisplayName + ((hoverRecipe.numberProducedPerCraft > 1)
					? (" x" + hoverRecipe.numberProducedPerCraft)
					: "");
				var originalWidthHeight = Game1.dialogueFont.MeasureString(hoverRecipe.name);
				var coverupWidth = (int)Math.Max(384f - 8f, originalWidthHeight.X + 4);
				var x = Game1.getOldMouseX() + 44 + (heldItem != null ? 48 : 0);

				// Check to show advanced crafting info
				var displayNameWidthHeight = Game1.dialogueFont.MeasureString(displayName);
				var craftableCount = hoverRecipe.getCraftableCount(Helper.Reflection.GetMethod(craftingMenu, "getContainerContents").Invoke<IList<Item>>());
				var craftableCountText = " (" + craftableCount + ")";
				var craftableCountTextWidthHeight = Game1.smallFont.MeasureString(craftableCountText);
				var areaWidth = (int)Math.Max(420f, displayNameWidthHeight.X + craftableCountTextWidthHeight.X + 12f);
				if (Game1.options.showAdvancedCraftingInformation && craftableCount > 0)
				{
					coverupWidth += (int)craftableCountTextWidthHeight.X;
				}

				// Calculate height of the full tooltip to determine Y position
				var healAmountToDisplay = -1;
				var buffIconsToDisplay = Game1.objectInformation[(lastCookingHover as StardewValley.Object).ParentSheetIndex].Split('/').Length > 7
					? Game1.objectInformation[(lastCookingHover as StardewValley.Object).ParentSheetIndex].Split('/')[7].Split(' ')
					: null;
				var addedHeight = Math.Max(20 * 3,
					(int)Game1.smallFont.MeasureString(displayName).Y + 32 + 8
					+ (int)(Game1.dialogueFont.MeasureString(displayName).Y + 16));

				// Buffs
				if (buffIconsToDisplay != null)
				{
					addedHeight += 4 + 34 * buffIconsToDisplay.Count(str => str != "0");
				}
				/*
				if (Game1.options.showAdvancedCraftingInformation && hoverRecipe.getCraftCountText() != null)
				{
					addedHeight += (int)Game1.smallFont.MeasureString("T").Y;
				}
				*/
				// Category
				addedHeight += 68 * lastCookingHover.attachmentSlots();
				/*if (lastCookingHover.getCategoryName().Length > 0)
				{
					addedHeight += (int)Game1.smallFont.MeasureString("T").Y / 2;
				}*/
				// Description
				var descriptionHeight = hoverRecipe.getDescriptionHeight(areaWidth) + ((healAmountToDisplay == -1) ? (-32) : 0);
				addedHeight += descriptionHeight - 4;
				// Healing
				if (lastCookingHover is StardewValley.Object o && o.Edibility != -300)
				{
					addedHeight = (healAmountToDisplay == -1)
						? (addedHeight + 40)
						: (addedHeight + 40 * ((healAmountToDisplay <= 0) ? 1 : 2));
				}

				// Keep the stupid junk within screen bounds
				var y = Game1.getOldMouseY() + 50 + (heldItem != null ? 48 : 0);
				var coverupHeight = (int)originalWidthHeight.Y + 1;
				var squeezeWidth = (int)Math.Max(384f - 8f, coverupWidth);
				if (x + squeezeWidth > Utility.getSafeArea().Right + 4)
				{
					x = Utility.getSafeArea().Right - squeezeWidth + 4;
					y += 16;
				}
				if (y + addedHeight + coverupHeight > Utility.getSafeArea().Bottom)
				{
					x += 16;
					if (x + squeezeWidth > Utility.getSafeArea().Right + 4)
					{
						x = Utility.getSafeArea().Right - squeezeWidth + 4;
					}
					y = Utility.getSafeArea().Bottom - addedHeight - coverupHeight;
				}

				// Draw dumb solution over original broken name
				e.SpriteBatch.Draw(Game1.menuTexture,
					destinationRectangle: new Rectangle(x, y, coverupWidth, coverupHeight),
					sourceRectangle: new Rectangle(12, 272, 1, 18),
					Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
				x += 4;
				y += 2;
				e.SpriteBatch.DrawString(Game1.dialogueFont, displayName, new Vector2(x, y) + new Vector2(2f, 2f), Game1.textShadowColor);
				e.SpriteBatch.DrawString(Game1.dialogueFont, displayName, new Vector2(x, y) + new Vector2(0f, 2f), Game1.textShadowColor);
				e.SpriteBatch.DrawString(Game1.dialogueFont, displayName, new Vector2(x, y), Game1.textColor);

				if (Game1.options.showAdvancedCraftingInformation && craftableCount > 0)
				{
					Utility.drawTextWithShadow(e.SpriteBatch, craftableCountText, Game1.smallFont,
						new Vector2(x + displayNameWidthHeight.X, y + displayNameWidthHeight.Y / 2f - craftableCountTextWidthHeight.Y / 2f),
						Game1.textColor);
				}
			}
		}

		private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
		{
			if (e.OldMenu is TitleMenu || e.NewMenu is TitleMenu || Game1.currentLocation == null || Game1.player == null)
				return;

			// Unique after-mail-read behaviours
			if (e.OldMenu is LetterViewerMenu letterClosed && letterClosed.isMail && e.NewMenu == null)
			{
				// Cookbook unlocked mail
				if (letterClosed.mailTitle == MailCookbookUnlocked)
				{
					Game1.player.completelyStopAnimatingOrDoingAction();
					DelayedAction.playSoundAfterDelay("getNewSpecialItem", 750);

					Game1.player.faceDirection(2);
					Game1.player.freezePause = 4000;
					Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[3]
					{
						new FarmerSprite.AnimationFrame(57, 0),
						new FarmerSprite.AnimationFrame(57, 2500, secondaryArm: false, flip: false, Farmer.showHoldingItem),
						new FarmerSprite.AnimationFrame((short)Game1.player.FarmerSprite.CurrentFrame, 500, secondaryArm: false, flip: false,
						delegate { Game1.drawObjectDialogue(i18n.Get("mail.cookbook_unlocked.after")); }, behaviorAtEndOfFrame: true)
					});
					Game1.player.mostRecentlyGrabbedItem = new StardewValley.Object(JsonAssets.GetObjectId(ObjectPrefix + "cookbook"), 0);
					Game1.player.canMove = false;
				}

				// Cooking tool whoops mail
				if (letterClosed.mailTitle == MailFryingPanWhoops)
				{
					Game1.player.addUnearnedMoney(1000);
				}

				return;
			}

			// Add new recipes on level-up for Cooking skill
			if (e.NewMenu is SpaceCore.Interface.SkillLevelUpMenu levelUpMenu1)
			{
				AddAndDisplayNewRecipesOnLevelUp(levelUpMenu1);
				return;
			}

			// Add new objects to shop menus and edit shop stock
			if (e.NewMenu is ShopMenu menu && menu != null && JsonAssets != null)
			{
				if (Game1.currentLocation is SeedShop)
				{
					// Sort Pierre's shop to bring new crops alongside base game crops
					SortSeedShopStock(ref menu);
				}
				else if (Game1.currentLocation is JojaMart && Config.AddNewCropsAndStuff)
				{
					// Add chocolate to Joja Mart
					var o = new StardewValley.Object(Vector2.Zero, JsonAssets.GetObjectId(ChocolateName), int.MaxValue);
					menu.itemPriceAndStock.Add(o, new [] {(int) (o.Price * Game1.MasterPlayer.difficultyModifier), int.MaxValue});
					menu.forSale.Insert(menu.forSale.FindIndex(i => i.Name == "Sugar"), o);
				}
				else if (menu.portraitPerson != null && menu.portraitPerson.Name == "Gus" && !Game1.currentLocation.IsOutdoors
					&& Bundles.IsCommunityCentreComplete())
				{
					// Add chocolate to Gus' shop
					var o = new StardewValley.Object(Vector2.Zero, JsonAssets.GetObjectId(ChocolateName), int.MaxValue);
					menu.itemPriceAndStock.Add(o, new[] { (int)((o.Price - 35) * Game1.MasterPlayer.difficultyModifier), int.MaxValue });
					menu.forSale.Insert(menu.forSale.FindIndex(i => i.Name == "Coffee"), o);
				}

				return;
			}

			if (e.NewMenu != null && (e.NewMenu is CraftingPage || e.NewMenu.GetType().Name == "NewCraftingPage") && Helper.Reflection.GetField<bool>(e.NewMenu, "cooking").GetValue())
			{
				// Open the new Cooking Menu as a substitute when a cooking CraftingPage is opened
				if (Config.AddCookingMenu)
				{
					e.NewMenu.exitThisMenuNoSound();
					Game1.activeClickableMenu = null;
					Helper.Events.GameLoop.UpdateTicked += Event_ReplaceCraftingMenu;
				}

				return;
			}
		}

		private void Player_InventoryChanged(object sender, InventoryChangedEventArgs e)
		{
			// Handle unique craftable input/output
			if (Game1.activeClickableMenu == null
				&& Config.AddNewCropsAndStuff
				&& JsonAssets != null
				&& Game1.currentLocation.Objects.ContainsKey(Game1.currentLocation.getTileAtMousePosition())
				&& Game1.currentLocation.Objects[Game1.currentLocation.getTileAtMousePosition()] is StardewValley.Object craftable
				&& craftable != null && craftable.bigCraftable.Value)
			{
				if (craftable.Name == "Keg")
				{
					if (NettlesEnabled && Game1.player.mostRecentlyGrabbedItem?.Name == ObjectPrefix + "nettles")
					{
						var name = ObjectPrefix + "nettletea";
						craftable.heldObject.Value = new StardewValley.Object(Vector2.Zero, JsonAssets.GetObjectId(name), name,
							canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
						craftable.MinutesUntilReady = 180;
					}
					else if (CiderEnabled && Game1.player.mostRecentlyGrabbedItem != null && Game1.player.mostRecentlyGrabbedItem.Name.EndsWith("Apple"))
					{
						var name = ObjectPrefix + "cider";
						craftable.heldObject.Value = new StardewValley.Object(Vector2.Zero, JsonAssets.GetObjectId(name), name,
							canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
						craftable.MinutesUntilReady = 1900;
					}
					else if (PerryEnabled && Game1.player.mostRecentlyGrabbedItem != null && Game1.player.mostRecentlyGrabbedItem.Name.EndsWith("Pear"))
					{
						var name = ObjectPrefix + "perry";
						craftable.heldObject.Value = new StardewValley.Object(Vector2.Zero, JsonAssets.GetObjectId(name), name,
							canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
						craftable.MinutesUntilReady = 1900;
					}
				}
				else if (craftable.Name == "Preserves Jar")
				{
					if (MarmaladeEnabled && e.Removed.FirstOrDefault(o => ItemDefinitions["MarmaladeFoods"].Contains(o.Name)) is StardewValley.Object dropIn && dropIn != null)
					{
						craftable.heldObject.Value = new StardewValley.Object(Vector2.Zero, JsonAssets.GetObjectId(ObjectPrefix + "marmalade"), dropIn.Name + " Marmalade",
							canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false)
						{
							Price = 65 + dropIn.Price * 2,
							name = dropIn.Name + " Marmalade"
						};
						craftable.MinutesUntilReady = 4600;
					}
				}
			}
		}

		private void Multiplayer_PeerContextReceived(object sender, PeerContextReceivedEventArgs e)
		{
			if (!Context.IsMainPlayer)
				return;
			Log.D($"Peer context received: {e.Peer.PlayerID} : SMAPI:{e.Peer.HasSmapi}" +
				$" CAC:{(e.Peer.Mods?.ToList().FirstOrDefault(mod => mod.ID == Helper.ModRegistry.ModID) is IMultiplayerPeerMod mod && mod != null ? mod.Version.ToString() : "null")}",
				Config.DebugMode);
		}

		private void Multiplayer_PeerConnected(object sender, PeerConnectedEventArgs e)
		{
			if (!Context.IsMainPlayer)
				return;
			Log.D($"Peer connected to multiplayer session: {e.Peer.PlayerID} : SMAPI:{e.Peer.HasSmapi}" +
				$" CAC:{(e.Peer.Mods?.ToList().FirstOrDefault(mod => mod.ID == Helper.ModRegistry.ModID) is IMultiplayerPeerMod mod && mod != null ? mod.Version.ToString() : "null")}",
				Config.DebugMode);
		}

		private void SpaceEvents_ItemEaten(object sender, EventArgs e)
		{
			if (!(Game1.player.itemToEat is StardewValley.Object food)
				|| Game1.player.itemToEat.ParentSheetIndex == 773) // Don't consider Life Elixir for food behaviours or healing over time
				return;

			var objectData = Game1.objectInformation[food.ParentSheetIndex].Split('/');
			States.Value.LastFoodWasDrink = objectData.Length > 6 && objectData[6] == "drink";
			States.Value.LastFoodEaten = food;

			Log.D($"Ate food: {food.Name}\nBuffs: (food) {Game1.buffsDisplay.food?.displaySource} (drink) {Game1.buffsDisplay.drink?.displaySource}",
				Config.DebugMode);

			// Determine food healing
			if (Config.FoodHealingTakesTime)
			{
				// Regenerate health/energy over time
				Game1.player.health = States.Value.HealthOnLastTick;
				Game1.player.Stamina = States.Value.StaminaOnLastTick;
				States.Value.HealthRegeneration += food.healthRecoveredOnConsumption();
				States.Value.StaminaRegeneration += food.staminaRecoveredOnConsumption();
			}
			else if (CookingSkillApi.HasProfession(ICookingSkillAPI.Profession.Restoration))
			{
				// Add additional health
				Game1.player.health = (int) Math.Min(Game1.player.maxHealth,
					Game1.player.health + food.healthRecoveredOnConsumption() * (CookingSkill.RestorationAltValue / 100f));
				Game1.player.Stamina = (int) Math.Min(Game1.player.MaxStamina,
					Game1.player.Stamina + food.staminaRecoveredOnConsumption() * (CookingSkill.RestorationAltValue / 100f));
			}

			var lastBuff = States.Value.LastFoodWasDrink
				? Game1.buffsDisplay.drink
				: Game1.buffsDisplay.food;
			Log.D($"OnItemEaten"
				+ $" | Ate food:  {food.Name}"
				+ $" | Last buff: {lastBuff?.displaySource ?? "null"} (source: {lastBuff?.source ?? "null"})",
				Config.DebugMode);

			// Check to boost buff duration
			if (CookingSkillApi.HasProfession(ICookingSkillAPI.Profession.BuffDuration)
			    && food.displayName == lastBuff?.displaySource)
			{
				var duration = lastBuff.millisecondsDuration;
				if (duration > 0)
				{
					var rate = (Game1.player.health + Game1.player.Stamina) / (Game1.player.maxHealth + Game1.player.MaxStamina);
					duration += (int) Math.Floor(CookingSkill.BuffDurationValue * 1000 * rate);
					lastBuff.millisecondsDuration = duration;
				}
			}

			// Track foods eaten
			if (!States.Value.FoodsEaten.Contains(food.Name))
			{
				States.Value.FoodsEaten.Add(food.Name);
			}

			// Add leftovers from viable foods to the inventory, or drop it on the ground if full
			if (ItemDefinitions["FoodsThatGiveLeftovers"].Contains(food.Name) && Config.AddRecipeRebalancing && JsonAssets != null)
			{
				var name = food.Name.StartsWith(ObjectPrefix) ? $"{food.Name}_half" : $"{food.Name.ToLower().Split(' ').Aggregate(ObjectPrefix, (s, s1) => s + s1)}" + "_half";
				var leftovers = new StardewValley.Object(JsonAssets.GetObjectId(name), 1);
				AddOrDropItem(leftovers);
			}

			// Handle unique kebab effects
			if (food.Name == "Kebab")
			{
				var roll = Game1.random.NextDouble();
				Buff buff = null;
				var duration = -1;
				var message = "";
				if (roll < 0.06f)
				{
					if (Config.FoodHealingTakesTime)
					{
						States.Value.HealthRegeneration -= food.healthRecoveredOnConsumption();
						States.Value.StaminaRegeneration -= food.staminaRecoveredOnConsumption();
					}
					else
					{
						Game1.player.health = (int)States.Value.HealthOnLastTick;;
						Game1.player.Stamina = States.Value.StaminaOnLastTick;
					}
					message = i18n.Get("item.kebab.bad");

					if (roll < 0.03f)
					{
						var stats = new[] {0, 0, 0, 0};
						stats[Game1.random.Next(stats.Length - 1)] = KebabNonCombatBonus * -1;

						message = i18n.Get("item.kebab.worst");
						var displaySource = i18n.Get("buff.kebab.inspect",
							new {quality = i18n.Get("buff.kebab.quality_worst")});
						duration = KebabMalusDuration;
						buff = roll < 0.0125f
							? new Buff(stats[0], stats[1], stats[2], 0, 0, stats[3],
								0, 0, 0, 0, 0, 0,
								duration, KebabBuffSource, displaySource)
							: new Buff(0, 0, 0, 0, 0, 0,
								0, 0, 0, 0,
								KebabCombatBonus * -1, KebabCombatBonus * -1,
								duration, KebabBuffSource, displaySource);
					}
				}
				else if (roll < 0.18f)
				{
					if (Config.FoodHealingTakesTime)
					{
						States.Value.HealthRegeneration += Game1.player.maxHealth / 10;
						States.Value.StaminaRegeneration += Game1.player.MaxStamina / 10;
					}
					else
					{
						Game1.player.health = Math.Min(Game1.player.maxHealth,
							Game1.player.health + Game1.player.maxHealth / 10);
						Game1.player.Stamina = Math.Min(Game1.player.MaxStamina,
							Game1.player.Stamina + Game1.player.MaxStamina / 10f);
					}

					var displaySource = i18n.Get("buff.kebab.inspect",
						new {quality = i18n.Get("buff.kebab.quality_best")});
					message = i18n.Get("item.kebab.best");
					duration = KebabBonusDuration;
					buff = new Buff(0, 0, KebabNonCombatBonus, 0, 0, 0,
						0, 0, 0, 0,
						KebabCombatBonus, KebabCombatBonus,
						duration, KebabBuffSource, displaySource);
				}
				if (string.IsNullOrEmpty(message))
					Game1.addHUDMessage(new HUDMessage(message));
				if (buff != null)
					Game1.buffsDisplay.tryToAddFoodBuff(buff, duration);
			}
		}
		
		private void SpaceEvents_BeforeGiftGiven(object sender, EventArgsBeforeReceiveObject e)
		{
			// Ignore gifts that aren't going to be accepted
			if (!e.Npc.canReceiveThisItemAsGift(e.Gift)
				|| !Game1.player.friendshipData.ContainsKey(e.Npc.Name)
			    || Game1.player.friendshipData[e.Npc.Name].GiftsThisWeek > 1
			    || Game1.player.friendshipData[e.Npc.Name].GiftsToday > 0)
			{
				return;
			}

			// Cooking skill professions influence gift value of Cooking objects
			if (CookingSkillApi.HasProfession(ICookingSkillAPI.Profession.GiftBoost) && e.Gift.Category == CookingCategory)
			{
				Game1.player.changeFriendship(CookingSkill.GiftBoostValue, e.Npc);
			}
		}

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

		public void CheckTileAction(Vector2 position, GameLocation location)
		{
			var property = location.doesTileHaveProperty(
				(int) position.X, (int) position.Y, "Action", "Buildings");
			if (property == null)
				return;
			var action = property.Split(' ');
			switch (action[0])
			{
				case ActionRange:
					// A new cooking range in the Saloon acts as a cooking station
					//if (Config.AddCookingQuestline && Game1.player.getFriendshipHeartLevelForNPC("Gus") < 2)
					if (false)
					{
						CreateInspectDialogue(i18n.Get("world.range_gus.inspect"));
						break;
					}
					OpenNewCookingMenu(null);
					break;

				case ActionDockCrate:
					// Interact with the new crates at the secret beach pier to loot items for quests
					if (JsonAssets != null)
					{
						Game1.currentLocation.playSoundAt("ship", position);
						var roll = Game1.random.NextDouble();
						StardewValley.Object o = null;
						if (roll < 0.2f && Game1.player.eventsSeen.Contains(0))
						{
							o = new StardewValley.Object(832, 1); // Pineapple
							if (roll < 0.05f && Game1.player.eventsSeen.Contains(1))
								o = new StardewValley.Object(JsonAssets.GetObjectId(ChocolateName), 1);
						}
						if (o != null)
							Game1.player.addItemByMenuIfNecessary(o.getOne());
					}
					break;
			}
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

		private void SaveLoadedBehaviours()
		{
			try
			{
				// Reset per-world config values
				var savedConfig = Helper.ReadConfig<Config>();
				Config.AddCookingCommunityCentreBundles = savedConfig.AddCookingCommunityCentreBundles;
			}
			catch (Exception e)
			{
				Log.E("" + e);
			}
			try
			{
				// Load local persistent data from saved modData
				States.Value.IsUsingRecipeGridView = false;
				States.Value.CookingToolLevel = 0;
				States.Value.FoodsEaten = new List<string>();
				States.Value.FavouriteRecipes = new List<string>();
				// Grid view
				if (Game1.player.modData.TryGetValue(AssetPrefix + "grid_view", out var gridView))
					States.Value.IsUsingRecipeGridView = bool.Parse(gridView);
				else
					Log.D("No data found for IsUsingRecipeGridView", Config.DebugMode);
				// Tool level
				if (Game1.player.modData.TryGetValue(AssetPrefix + "tool_level", out var toolLevel))
					States.Value.CookingToolLevel = int.Parse(toolLevel);
				else
					Log.D("No data found for CookingToolLevel", Config.DebugMode);
				// Foods eaten
				if (Game1.player.modData.TryGetValue(AssetPrefix + "foods_eaten", out var foodsEaten))
					States.Value.FoodsEaten = foodsEaten.Split(',').ToList();
				else
					Log.D("No data found for FoodsEaten", Config.DebugMode);
				// Favourite recipes
				if (Game1.player.modData.TryGetValue(AssetPrefix + "favourite_recipes", out var favouriteRecipes))
					States.Value.FavouriteRecipes = favouriteRecipes.Split(',').ToList();
				else
					Log.D("No data found for FavouriteRecipes", Config.DebugMode);

			}
			catch (Exception e)
			{
				Log.E("" + e);
			}
			try
			{
				Bundles.SaveLoadedBehaviours();
			}
			catch (Exception e)
			{
				Log.E("" + e);
			}
			try
			{
				Tools.SaveLoadedBehaviours();
			}
			catch (Exception e)
			{
				Log.E("" + e);
			}

			PrintModData();

			if (Config.AddCookingSkillAndRecipes)
			{
				PrintCookingSkill();
			}

			// Invalidate and reload assets requiring JA indexes
			Log.D("Invalidating assets on save loaded.",
				Config.DebugMode);
			Helper.Content.InvalidateCache(@"Data/ObjectInformation");
			Helper.Content.InvalidateCache(@"Data/CookingRecipes");
			Helper.Content.InvalidateCache(@"Data/Bundles");
			// Assets edited with our spritesheet
			Helper.Content.InvalidateCache(@"LooseSprites/Cursors");

			// Populate NPC home locations for cooking range usage
			var npcData = Game1.content.Load<Dictionary<string, string>>("Data/NPCDispositions");
			NpcHomeLocations = new Dictionary<string, string>();
			foreach (var npc in npcData)
			{
				NpcHomeLocations.Add(npc.Key, npc.Value.Split('/')[10].Split(' ')[0]);
			}

			// Attempt to register Level Extender compatibility
			if (LevelExtender != null)
			{
				_totalSecondsOnLoaded = Game1.currentGameTime.TotalGameTime.TotalSeconds;
				Helper.Events.GameLoop.OneSecondUpdateTicked += this.Event_RegisterLevelExtenderLate;
			}
		}

		internal void OpenNewCookingMenu(List<CraftingRecipe> recipes = null)
		{
			void CreateCookingMenu(NetRef<Chest> fridge, List<Chest> miniFridges)
			{
				var list = new List<Chest>();
				if (fridge.Value != null)
				{
					list.Add(fridge);
				}
				list.AddRange(miniFridges);

				var topLeftPositionForCenteringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(
					800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2);

				var craftingMenu = new CraftingPage(
					(int)topLeftPositionForCenteringOnScreen.X, (int)topLeftPositionForCenteringOnScreen.Y,
					800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2,
					cooking: true, standalone_menu: true, material_containers: list);
				
				if (Config.AddCookingMenu)
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

			if (Game1.player.hasOrWillReceiveMail(MailCookbookUnlocked))
			{
				var ccFridge = Game1.currentLocation is CommunityCenter ? Bundles.GetCommunityCentreFridge() : null;
				var fridge = new NetRef<Chest>();
				var muticies = new List<NetMutex>();
				var miniFridges = new List<Chest>();

				fridge.Set(Game1.currentLocation is FarmHouse farmHouse && GetFarmhouseKitchenLevel(farmHouse) > 0
					? farmHouse.fridge
					: ccFridge != null ? new NetRef<Chest>(ccFridge) : null);

				foreach (var item in Game1.currentLocation.Objects.Values.Where(
					i => i != null && i.bigCraftable.Value && i is Chest && i.ParentSheetIndex == 216))
				{
					miniFridges.Add(item as Chest);
					muticies.Add((item as Chest).mutex);
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
				CreateInspectDialogue(i18n.Get("menu.cooking_station.no_cookbook"));
			}
		}
		
		/// <summary>
		/// Returns the base health/stamina regeneration rate for some food object.
		/// </summary>
		public float GetFoodRegenRate(StardewValley.Object food)
		{
			// Magical numbers live here

			// Regen faster with drinks
			var rate = States.Value.LastFoodWasDrink ? 0.12f : 0.075f;
			// Regen faster with quality
			rate += food.Quality * 0.0085f;
			// Regen faster when drunk
			if (Game1.player.hasBuff(17))
				rate *= 1.3f;
			if (CookingSkillApi.HasProfession(ICookingSkillAPI.Profession.Restoration))
				rate += rate / CookingSkill.RestorationValue;
			return rate;
		}

		private void AddAndDisplayNewRecipesOnLevelUp(SpaceCore.Interface.SkillLevelUpMenu menu)
		{
			// Add cooking recipes
			var skill = Helper.Reflection.GetField<string>(menu, "currentSkill").GetValue();
			if (skill != CookingSkill.InternalName)
			{
				return;
			}
			var level = Helper.Reflection.GetField<int>(menu, "currentLevel").GetValue();
			var cookingRecipes = CookingSkillApi.GetCookingRecipesForLevel(level).ConvertAll(name => new CraftingRecipe(ObjectPrefix + name, true))
				.Where(recipe => !Game1.player.knowsRecipe(recipe.name)).ToList();
			if (cookingRecipes != null && cookingRecipes.Count > 0)
			{
				UpdateEnglishRecipeDisplayNames(ref cookingRecipes);
				foreach (var recipe in cookingRecipes.Where(r => !Game1.player.cookingRecipes.ContainsKey(r.name)))
				{
					Game1.player.cookingRecipes[recipe.name] = 0;
				}
			}

			// Add crafting recipes
			var craftingRecipes = new List<CraftingRecipe>();
			if (level == CookingSkill.CraftCampfireLevel)
			{
				var recipe = new CraftingRecipe("Campfire", false);
				craftingRecipes.Add(recipe);
				if (!Game1.player.craftingRecipes.ContainsKey(recipe.name))
				{
					Game1.player.craftingRecipes[recipe.name] = 0;
				}
			}

			// Apply new recipes
			var combinedRecipes = craftingRecipes.Concat(cookingRecipes).ToList();
			Helper.Reflection.GetField<List<CraftingRecipe>>(menu, "newCraftingRecipes").SetValue(combinedRecipes);
			Log.D(combinedRecipes.Aggregate($"New recipes for level {level}:", (total, cur) => $"{total}\n{cur.name} ({cur.createItem().ParentSheetIndex})"),
				Config.DebugMode);

			// Adjust menu to fit if necessary
			const int defaultMenuHeightInRecipes = 4;
			var menuHeightInRecipes = combinedRecipes.Count + combinedRecipes.Count(recipe => recipe.bigCraftable);
			if (menuHeightInRecipes >= defaultMenuHeightInRecipes)
			{
				menu.height += (menuHeightInRecipes - defaultMenuHeightInRecipes) * 64;
			}
		}

		private List<CraftingRecipe> TakeRecipesFromCraftingPage(CraftingPage cm, bool cookingOnly = true)
		{
			var cooking = Helper.Reflection.GetField<bool>(cm, "cooking").GetValue();
			if (cooking || !cookingOnly)
			{
				var recipePages = Helper.Reflection.GetField
					<List<Dictionary<ClickableTextureComponent, CraftingRecipe>>>(cm, "pagesOfCraftingRecipes").GetValue();
				cm.exitThisMenuNoSound();
				return recipePages.SelectMany(page => page.Values).ToList();
			}
			return null;
		}

		/// <summary>
		/// Identifies the level of the best cooking station within the player's use range.
		/// A cooking station's level influences the number of ingredients slots available to the player.
		/// </summary>
		/// <returns>Level of the best cooking station in range, defaults to 0.</returns>
		public int GetNearbyCookingStationLevel()
		{
			const int radius = 3;
			var cookingStationLevel = 0;

			// If indoors, use the farmhouse or cabin level as a base for cooking levels
			if (!Game1.currentLocation.IsOutdoors)
			{
				var layer = Game1.currentLocation.Map.GetLayer("Buildings");
				var xLimit = Game1.player.getTileX() + radius;
				var yLimit = Game1.player.getTileY() + radius;
				for (var x = Game1.player.getTileX() - radius; x < xLimit && cookingStationLevel == 0; ++x)
				for (var y = Game1.player.getTileY() - radius; y < yLimit && cookingStationLevel == 0; ++y)
				{
					var tile = layer.Tiles[x, y];
					if (tile == null
					    || (Game1.currentLocation.doesTileHaveProperty(x, y, "Action", "Buildings") != "kitchen" 
							&& !ItemDefinitions["IndoorsTileIndexesThatActAsCookingStations"].Contains(tile.TileIndex.ToString())))
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
						Config.DebugMode);
				}
			}
			else
			{
				var xLimit = Game1.player.getTileX() + radius;
				var yLimit = Game1.player.getTileY() + radius;
				for (var x = Game1.player.getTileX() - radius; x < xLimit && cookingStationLevel == 0; ++x)
				for (var y = Game1.player.getTileY() - radius; y < yLimit && cookingStationLevel == 0; ++y)
				{
					Game1.currentLocation.Objects.TryGetValue(new Vector2(x, y), out var o);
					if (o == null || (o.Name != "Campfire" && o.Name != CookingCraftableName))
						continue;
					cookingStationLevel = GetFarmersMaxUsableIngredients();
					Log.D($"Cooking station: {cookingStationLevel}",
						Config.DebugMode);
				}
			}
			Log.D("Cooking station search finished",
				Config.DebugMode);
			return cookingStationLevel;
		}

		/// <summary>
		/// Fetches the cooking station level for the farmhouse based on its upgrade/kitchen level,
		/// accounting for mods that would provide the kitchen at level 0.
		/// </summary>
		public int GetFarmhouseKitchenLevel(FarmHouse farmHouse)
		{
			// A basic (modded) farmhouse has a maximum of 1 slot,
			// and a farmhouse with a kitchen has a minimum of 2+ slots
			var level = Math.Max(farmHouse.upgradeLevel, GetFarmersMaxUsableIngredients());
			if (farmHouse.upgradeLevel == 0 && UsingFarmhouseKitchenStart)
			{
				level = 1;
			}
			return level;
		}

		public int GetFarmersMaxUsableIngredients()
		{
			return (Config.AddCookingToolProgression && States.Value.CookingToolLevel < 4)
				? 1 + States.Value.CookingToolLevel
				: 6;
		}

		/// <summary>
		/// Bunches groups of common items together in the seed shop.
		/// Json Assets appends new stock to the bottom, and we don't want that very much at all.
		/// </summary>
		private void SortSeedShopStock(ref ShopMenu menu)
		{
			// Pair a suffix grouping some common items together with the name of the lowest-index (first-found) item in the group
			var itemList = menu.forSale;
			var suffixes = new Dictionary<string, string>
				{{"seeds", null}, {"bulb", null}, {"starter", null}, {"shoot", null}, {"sapling", null}};
			var debugCount = 0;
			for (var i = 0; i < itemList.Count; ++i)
			{
				// Ignore items without one of our group suffixes
				var suffix = suffixes.Keys.FirstOrDefault(s => itemList[i].Name.ToLower().EndsWith(s));
				if (suffix == null)
					continue;
				// Set the move-to-this-item name to be the first-found item in the group
				suffixes[suffix] ??= itemList[i].Name;
				if (suffixes[suffix] == itemList[i].Name)
					continue;
				// Move newly-found items of a group up to the first item in the group, and change the move-to name to this item
				var item = itemList[i];
				var index = 1 + itemList.FindIndex(i => i.Name == suffixes[suffix]);
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
		internal void UpdateEnglishRecipeDisplayNames(ref List<CraftingRecipe> recipes)
		{
			if (LocalizedContentManager.CurrentLanguageCode.ToString() == "en")
			{
				foreach (var recipe in recipes.Where(r => r.DisplayName.StartsWith(ObjectPrefix)))
				{
					var displayName = Game1.objectInformation[JsonAssets.GetObjectId(recipe.name)].Split('/')[4];
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
			var fields = oldEntry.Split(delimiter);
			if (replace)
				fields = newEntry;
			else for (var i = 0; i < newEntry.Length; ++i)
				if (newEntry[i] != null)
					fields[startIndex + i] = append ? $"{fields[startIndex + i]} {newEntry[i]}" : newEntry[i];
			return string.Join(delimiter.ToString(), fields);
		}
	}
}
