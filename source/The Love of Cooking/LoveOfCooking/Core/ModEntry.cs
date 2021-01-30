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
using LoveOfCooking.GameObjects.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using PyTK.Extensions;
using SpaceCore;
using SpaceCore.Events;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using xTile;
using xTile.Dimensions;
using Object = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;


// TODO: TEST: Add CookingSkill API, experience gained event for bridging with LevEx

// TODO: LOCALE: Italian COOK! button sprite

// TODO: FIX: Add mail with refund for Iridium Frying Pans
// TODO: FIX: Duplicating items when inventory full and cooking menu closes
// TODO: FIX: CC Kitchen star doesn't show up on board for host when CC completed; empty star shows for peers (https://i.imgur.com/UZXTopu.png)
// TODO: FIX: Some cooking recipes (Starbrew Valley) have 6 ingredients, and the menu will need support for them

// TODO: 1.5 FIX: Test Qi Seasoning
// TODO: 1.5 FIX: Evelyn's special request board quest is uncompleteable as it requires using the kitchen
// TODO: 1.5 FIX: New skill menu erases Golden Walnut count

// TODO: UPDATE: Cooked food has a chance (scaling with Cooking level) of taking the quality of its ingredients,
//		Final quality is decided by random choice from list of qualities of each ingredient
// TODO: UPDATE: Finish the cookbook sprite bottom + spine and have the cookbook + inventory space outwards vertically on large resolutions

// TODO: ???: Ping Pathos about what Game1.IsServer does

// TODO: CONTENT: Quests, events, and scripts
// TODO: CONTENT: Hot chocolate at the ice festival

// TODO: COMPATIBILITY: Remote Fridge Storage (https://www.nexusmods.com/stardewvalley/mods/2545)
//		Will need new checks for adding chests to the minifridge list, and display chest icon if true
// TODO: COMPATIBILITY: Limited Campfire Cooking (https://www.nexusmods.com/stardewvalley/mods/4971)
//		In DisplayMenuChanged intercept for CraftingPage, OpenCookingMenu is delayed a tick for mutex request on fridges
//		Campfires have their menu intercepted correctly, but no longer have the limited crafting recipe list passed on
// TODO: COMPATIBILITY: Skill Prestige (https://www.nexusmods.com/stardewvalley/mods/569)
// TODO: COMPATIBILITY: Level Extender (https://www.nexusmods.com/stardewvalley/mods/1471)
// TODO: COMPATIBILITY: Convenient Chests (https://www.nexusmods.com/stardewvalley/mods/2196)
// TODO: COMPATIBILITY: Tool Upgrade Delivery (https://www.nexusmods.com/stardewvalley/mods/5421)
// TODO: COMPATIBILITY: Expanded Fridge (https://forums.stardewvalley.net/threads/unofficial-mod-updates.2096/page-6#post-20884)


namespace LoveOfCooking
{
	public class ModEntry : Mod
	{
		internal static ModEntry Instance;
		internal Config Config;

		internal ITranslationHelper i18n => Helper.Translation;
		internal static IJsonAssetsApi JsonAssets;
		internal static Texture2D SpriteSheet;

		internal const string SaveDataKey = "SaveData";
		internal const string AssetPrefix = "blueberry.LoveOfCooking.";
		internal const string ObjectPrefix = "blueberry.cac.";
		internal const string MailPrefix = "blueberry.cac.mail.";

		// Assets
		internal static readonly string BasicObjectsPack = Path.Combine("assets", "BasicObjectsPack");
		internal static readonly string NewRecipesPackPath = Path.Combine("assets", "NewRecipesPack");
		internal static readonly string NewCropsPackPath = Path.Combine("assets", "NewCropsPack");
		internal static readonly string NettlesPackPath = Path.Combine("assets", "NettlesPack");
		//internal static readonly string CookingBundlePackPath = Path.Combine("assets", "CookingBundlePack");
		internal static readonly string GameContentSpriteSheetPath = @"SpriteSheet";
		internal static readonly string LocalSpriteSheetPath = Path.Combine("assets", "sprites");
		internal static readonly string MapTileSheetPath = Path.Combine("assets", "maptiles");
		internal static readonly string GameContentBundleDataPath = @"Bundles";
		internal static readonly string LocalBundleDataPath = Path.Combine("assets", "bundles");
		internal static readonly string BuffDataPath = Path.Combine("assets", "ingredientBuffChart");

		// Persistent player data
		public int CookingToolLevel = 0;
		public bool IsUsingRecipeGridView = false;
		public List<string> FoodsEaten = new List<string>();
		public List<string> FavouriteRecipes = new List<string>();

		// Mail titles
		internal static readonly string MailKitchenCompleted = $"cc{CommunityCentreAreaName}";
		internal static readonly string MailCookbookUnlocked = MailPrefix + "cookbook_unlocked";
		internal static readonly string MailKitchenCompletedFollowup = MailPrefix + "kitchen_completed_followup";
		internal static readonly string MailKitchenLastBundleCompleteRewardDelivery = MailPrefix + "kitchen_reward_guarantee";
		internal static readonly string MailFryingPanWhoops = MailPrefix + "im_sorry_lol_pan";

		// Add Cooking Menu
		public const int CookbookMailDate = 14;
		public static CookingMenu.Filter LastFilterThisSession = CookingMenu.Filter.None;
		public static bool LastFilterReversed;

		// Add Cooking Skill
		public static ICookingSkillAPI CookingSkillApi;
		public static readonly Dictionary<int, int> FoodCookedToday = new Dictionary<int, int>();
		public const int MaxFoodStackPerDayForExperienceGains = 20;
		public const int CraftNettleTeaLevel = 3;
		public const int CraftCampfireLevel = 1;
		private Buff _watchingBuff;

		// Add Cooking to the Community Centre
		public static readonly string CommunityCentreAreaName = "Kitchen";
		public static readonly string CookingCraftableName = ObjectPrefix + "cookingcraftable";
		public static readonly int CommunityCentreAreaNumber = 6;
		public static readonly Rectangle CommunityCentreArea = new Rectangle(0, 0, 10, 11);
		public static readonly Point CommunityCentreNotePosition = new Point(7, 6);
		// We use Linus' tent interior for the dummy area, since there's surely no conceivable way it'd be in the community centre
		public static readonly Rectangle DummyOpenFridgeSpriteArea = new Rectangle(32, 560, 16, 32);
		public static readonly Vector2 DummyFridgePosition = new Vector2(6830);
		public static Vector2 CommunityCentreFridgePosition = Vector2.Zero;
		public static int BundleStartIndex;
		public static int BundleCount;
		private int _menuTab;

		// Add Cooking Questline
		internal const string ActionDockCrate = AssetPrefix + "DockCrate";
		internal const string ActionRange = AssetPrefix + "Range";

		// Food Healing Takes Time
		private const float CombatRegenModifier = 0.02f;
		private const float CookingRegenModifier = 0.005f;
		private const float ForagingRegenModifier = 0.0012f;
		private float _healthOnLastTick, _staminaOnLastTick;
		private int _healthRegeneration, _staminaRegeneration;
		private uint _regenTicksCurr;
		private Queue<uint> _regenTicksDiff = new Queue<uint>();
		private Object _lastFoodEaten;
		private bool _lastFoodWasDrink;
		// debug
		private float _debugRegenRate;
		private uint _debugElapsedTime;

		// Play Cooking Animation
		public static readonly string[] SoupyFoods = new[]
		{
			"soup",
			"bisque",
			"chowder",
			"stew",
			"pot",
			"broth",
			"stock",
		};
		public static readonly string[] DrinkyFoods = new[]
		{
			"candy",
			"cocoa",
			"chocolate",
			"milkshake",
			"smoothie",
			"milk",
			"tea",
			"coffee",
			"espresso",
			"mocha",
			"latte",
			"cappucino",
			"drink",
		};
		public static readonly string[] SaladyFoods = new[]
		{
			"coleslaw",
			"salad",
			"lunch",
			"taco",
			"roll",
			"sashimi",
			"sushi",
			"sandwich",
			"unagi",
		};
		public static readonly string[] BakeyFoods = new[]
		{
			"cookie",
			"roast",
			"bake",
			"cupcake",
		};
		public static readonly string[] CakeyFoods = new[]
		{
			"bread",
			"bun",
			"cake",
			"cakes",
			"pie",
			"pudding",
			"bake",
			"biscuit",
			"brownie",
			"brownies",
			"cobbler",
			"cookie",
			"cookies",
			"crumble",
			"cupcake",
			"fingers",
			"muffin",
			"quiche",
			"tart",
			"turnover"
		};
		public static readonly string[] PancakeyFoods = new[]
		{
			"pancake",
			"crepe",
			"hotcake"
		};
		public static readonly string[] PizzayFoods = new[]
		{
			"pizza",
			"pitta",
			"calzone",
			"tortilla",
		};

		// Others:
		private const string ChocolateName = ObjectPrefix + "chocolate";
		private const string NettlesUsableMachine = "Keg";
		private const int NettlesUsableLevel = 2;
		// cook at kitchens
		internal static readonly Location SaloonCookingRangePosition = new Location(18, 17);
		internal static Dictionary<string, string> NpcHomeLocations;
		internal const int NpcKitchenFriendshipRequired = 7;
		// kebab
		private const string KebabBuffSource = AssetPrefix + "kebab";
		private const int KebabBonusDuration = 220;
		private const int KebabMalusDuration = 140;
		private const int KebabCombatBonus = 3;
		private const int KebabNonCombatBonus = 2;
		// configuration
		public static readonly List<int> IndoorsTileIndexesThatActAsCookingStations = new List<int>
		{
			498, 499, 632, 633
		};
		public static readonly List<string> FoodsThatGiveLeftovers = new List<string>
		{
			"Pizza",
			"Chocolate Cake",
			"Pink Cake",
			ObjectPrefix + "cake",
			ObjectPrefix + "seafoodsando",
			ObjectPrefix + "eggsando",
			ObjectPrefix + "saladsando",
			ObjectPrefix + "watermelon",
		};
		// extra keg products
		public static readonly List<string> MarmaladeFoods = new List<string>
		{
			"Lemon", "Lime", "Citron", "Yuzu", "Grapefruit", "Pomelo", "Orange", "Mandarin", "Satsuma"
		};

		// Notifications
		internal enum Notification
		{
			BundleMultiplayerWarning,
			CabinBuiltWarning,
		}
		internal static NotificationButton NotificationButton = new NotificationButton();
		internal static List<Notification> PendingNotifications = new List<Notification>();
		internal static bool HasUnreadNotifications;
		private static int _debugLastCabinsCount;


		internal static readonly bool CiderEnabled = true;
		internal static readonly bool PerryEnabled = false;
		internal static readonly bool MarmaladeEnabled = false;
		internal static readonly bool NettlesEnabled = false;
		internal static readonly bool RedberriesEnabled = false;
		internal static readonly bool CookingAddedLevelsEnabled = false;
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
					+ $"\nRecipeGridView:   {IsUsingRecipeGridView}"
					+ $"\nCookingToolLevel: {CookingToolLevel}"
					+ $"\nCanUpgradeTool:   {CanFarmerUpgradeCookingEquipment()}"
					+ $"\nFoodsEaten:       {FoodsEaten.Aggregate("", (s, cur) => $"{s} ({cur})")}"
					+ $"\nFavouriteRecipes: {FavouriteRecipes.Aggregate("", (s, cur) => $"{s} ({cur})")}\n"
					+ "-- OTHERS --"
					+ $"\nLanguage:         {LocalizedContentManager.CurrentLanguageCode.ToString().ToUpper()}"
					+ $"\nFarmHouseLevel:   {GetFarmhouseKitchenLevel(Game1.getLocationFromName("FarmHouse") as FarmHouse)}"
					+ $"\nNumberOfCabins:   {GetNumberOfCabinsBuilt()}"
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
				Log.D("Cooking skill is disabled in mod config.");
			}
			else if (CookingSkillApi.GetSkill() == null)
			{
				Log.D("Cooking skill is enabled, but skill is not loaded.");
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

			// Asset editors
			var assetManager = new AssetManager();
			Helper.Content.AssetEditors.Add(assetManager);
			Helper.Content.AssetLoaders.Add(assetManager);
			SpriteSheet = Game1.content.Load<Texture2D>(GameContentSpriteSheetPath);
			
			// Game events
			Helper.Events.GameLoop.GameLaunched += GameLoopOnGameLaunched;
			Helper.Events.GameLoop.SaveLoaded += GameLoopOnSaveLoaded;
			Helper.Events.GameLoop.DayStarted += GameLoopOnDayStarted;
			Helper.Events.GameLoop.DayEnding += GameLoopOnDayEnding;
			Helper.Events.GameLoop.ReturnedToTitle += GameLoopOnReturnedToTitle;
			Helper.Events.GameLoop.UpdateTicked += GameLoopUpdateTicked;
			Helper.Events.Player.Warped += PlayerOnWarped;
			Helper.Events.Player.InventoryChanged += PlayerOnInventoryChanged;
			Helper.Events.Input.ButtonPressed += InputOnButtonPressed;
			Helper.Events.Display.MenuChanged += DisplayOnMenuChanged;
			Helper.Events.Multiplayer.PeerContextReceived += MultiplayerOnPeerContextReceived;
			Helper.Events.Multiplayer.PeerConnected += MultiplayerOnPeerConnected;

			CookingSkillApi = new CookingSkillAPI(Helper.Reflection);
			if (Config.AddCookingSkillAndRecipes)
			{
				Skills.RegisterSkill(new CookingSkill());
			}
			if (Config.DebugMode && Config.DebugRegenTracker)
			{
				Helper.Events.Display.RenderedHud += Event_DrawDebugRegenTracker;
			}
			SpaceEvents.OnItemEaten += SpaceEventsOnItemEaten;
			SpaceEvents.BeforeGiftGiven += SpaceEventsOnBeforeGiftGiven;

			// Console commands
			var cmd = Config.ConsoleCommandPrefix;
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

				CookingToolLevel = int.Parse(args[0]);
				Log.D($"Set Cooking tool to {CookingToolLevel}");
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
			Helper.ConsoleCommands.Add(cmd + "spawnpan", "Add a broken frying pan object to inventory.", (s, args) =>
			{
				var level = args.Length > 0 ? int.Parse(args[0]) : 0;
				level = Math.Max(0, Math.Min(3, level));
				var tool = GenerateCookingTool(level);
				Game1.player.addItemByMenuIfNecessary(tool);
			});
			Helper.ConsoleCommands.Add(cmd + "purgepan", "Remove broken frying pan objects from inventories and chests.", (s, args) =>
			{
				Log.D($"Purging frying pans{(args.Length > 0 ? " and sending mail." : ".")}");
				PurgeBrokenFryingPans(sendMail: args.Length > 0);
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
				PendingNotifications.Clear();
				Log.D($"Cleared notifications: {(PendingNotifications.Count <= 0)}");
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
			Helper.ConsoleCommands.Add(cmd + "printcc", "Print Community Centre bundle states.", (s, args) =>
			{
				var cc = GetCommunityCentre();
				PrintBundleData(cc);
			});
			Helper.ConsoleCommands.Add(cmd + "loadcc", "Load custom bundle data into the game.", (s, args) =>
			{
				LoadBundleData();
			});
			Helper.ConsoleCommands.Add(cmd + "unloadcc", "Clear all bundle data from the game.", (s, args) =>
			{
				SaveAndUnloadBundleData();
			});
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
			else if (Helper.ModRegistry.IsLoaded("PPJA.FruitsAndVeggies"))
			{
				Log.I("Did not add new crops: [PPJA] Fruits and Veggies already adds these objects.");
				Config.AddNewCropsAndStuff = false;
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

			//if (Config.DebugMode)
				//Log.W("Loading Cooking Bundle Pack.");
			//JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, CookingBundlePackPath));
		}

		private void GameLoopOnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			LoadJsonAssetsObjects();
		}

		private void GameLoopOnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			SaveLoadedBehaviours();
		}

		private void GameLoopOnDayEnding(object sender, DayEndingEventArgs e)
		{
			// Counteract the silly check for (whichArea == 6) in JunimoNoteMenu.setUpMenu(whichArea, bundlesComplete)
			if (!IsCommunityCentreComplete())
			{
				var whichMail = "hasSeenAbandonedJunimoNote";
				if (Game1.player.mailForTomorrow.Contains(whichMail))
				{
					Game1.player.mailForTomorrow.Remove(whichMail);
					Log.D("Removed premature abandoned bundle note mail from tomorrow's mail.",
						Config.DebugMode);
				}
				if (Game1.player.mailReceived.Contains(whichMail))
				{
					Game1.player.mailReceived.Remove(whichMail);
					Log.D("Removed premature abandoned bundle note from received mail.",
						Config.DebugMode);
				}
				return;
			}

			// Save persistent player data to player
			Game1.player.modData[AssetPrefix + "grid_view"] = IsUsingRecipeGridView.ToString();
			Game1.player.modData[AssetPrefix + "tool_level"] = CookingToolLevel.ToString();
			Game1.player.modData[AssetPrefix + "foods_eaten"] = string.Join(",", FoodsEaten);
			Game1.player.modData[AssetPrefix + "favourite_recipes"] = string.Join(",", FavouriteRecipes);

			// Save local (and/or persistent) community centre data
			Log.D("Unloading world bundle data at end of day.",
				Config.DebugMode);
			SaveAndUnloadBundleData();
		}

		private void GameLoopOnDayStarted(object sender, DayStartedEventArgs e)
		{
			// Perform OnSaveLoaded behaviours when starting a new game
			if (Game1.dayOfMonth == 1 && Game1.currentSeason == "spring" && Game1.year == 1)
			{
				SaveLoadedBehaviours();
			}

			// Load starting recipes
			foreach (var recipe in CookingSkill.StartingRecipes)
			{
				if (!Game1.player.cookingRecipes.ContainsKey(recipe))
					Game1.player.cookingRecipes.Add(recipe, 0);
			}

			// Set up vanilla campfire recipe
			if (Config.AddCookingSkillAndRecipes && CookingSkillApi.GetLevel() < CraftCampfireLevel)
			{
				// Campfire is added on level-up for cooking skill users
				Game1.player.craftingRecipes.Remove("Campfire");
			}
			else if (!Config.AddCookingSkillAndRecipes && !Game1.player.craftingRecipes.ContainsKey("Campfire"))
			{
				// Re-add campfire to the player's recipe list otherwise if it's missing
				Game1.player.craftingRecipes["Campfire"] = 0;
			}

			// Clear daily cooking to free up Cooking experience gains
			if (Config.AddCookingSkillAndRecipes)
			{
				FoodCookedToday.Clear();
			}

			// Add the cookbook for the player after some days
			if (Config.AddCookingMenu
				&& (Game1.year > 1 || Game1.dayOfMonth > CookbookMailDate || Game1.currentSeason != "spring")
				&& !Game1.player.hasOrWillReceiveMail(MailCookbookUnlocked))
			{
				Game1.player.mailbox.Add(MailCookbookUnlocked);
			}

			// TODO: UPDATE: Send followup mail when the kitchen bundle is completed
			if (SendBundleFollowupMail && (IsCommunityCentreKitchenEnabledByHost() && Game1.MasterPlayer.hasOrWillReceiveMail(MailKitchenCompleted)))
			{
				Game1.addMailForTomorrow(MailKitchenCompletedFollowup);
			}

			// Attempt to place a wild nettle as forage around other weeds
			if (NettlesEnabled && (Game1.currentSeason == "summer" || ((Game1.currentSeason == "spring" || Game1.currentSeason == "fall") && Game1.dayOfMonth % 2 == 0)))
			{
				foreach (var l in new[] {"Mountain", "Forest", "Railroad", "Farm"})
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

			// Load in new community centre bundle data
			// Hosts must have the Community Centre changes enabled, and hosts/farmhands must be joining a world with custom bundles apparently enabled
			if (IsCommunityCentreComplete())
			{
				Log.D("Community centre complete, unloading any bundle data.",
					Config.DebugMode);
				SaveAndUnloadBundleData();
				SetCommunityCentreKitchenForThisSession(false);

			}
			else if (IsCommunityCentreKitchenEnabledByHost())
			{
				Log.D("Community centre incomplete, loading bundle data.",
					Config.DebugMode);
				LoadBundleData();
			}

			// Reapply Harmony patches
			HarmonyPatches.Patch();
		}

		private void GameLoopOnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
		{
			// Clear food history
			_watchingBuff = null;
			_lastFoodEaten = null;
			_lastFoodWasDrink = false;

			// Cancel ongoing regeneration
			_regenTicksDiff.Clear();
			_regenTicksCurr = 0;
			_healthRegeneration = _staminaRegeneration = 0;
			_healthOnLastTick = _staminaOnLastTick = 0;
			_debugRegenRate = _debugElapsedTime = 0;
		}

		private void GameLoopUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (Game1.player != null)
			{
				_healthOnLastTick = Game1.player.health;
				_staminaOnLastTick = Game1.player.Stamina;
			}
		}
		
		private void Event_DrawDebugRegenTracker(object sender, RenderedHudEventArgs e)
		{
			for (var i = 0; i < _regenTicksDiff.Count; ++i)
			{
				e.SpriteBatch.DrawString(
					Game1.smallFont,
					$"{(i == 0 ? "DIFF" : "      ")}   {_regenTicksDiff.ToArray()[_regenTicksDiff.Count - 1 - i]}",
					new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width - 222, Game1.graphics.GraphicsDevice.Viewport.Height - 144 - i * 24),
					Color.White * ((_regenTicksDiff.Count - 1 - i + 1f) / (_regenTicksDiff.Count / 2f)));
			}
			e.SpriteBatch.DrawString(
				Game1.smallFont,
				$"MOD  {(_debugRegenRate < 1 ? 0 :_debugElapsedTime % _debugRegenRate)}",
				new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width - 222, Game1.graphics.GraphicsDevice.Viewport.Height - 120),
				Color.White);
			e.SpriteBatch.DrawString(
				Game1.smallFont,
				$"RATE {_debugRegenRate}",
				new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width - 222, Game1.graphics.GraphicsDevice.Viewport.Height - 96),
				Color.White);
			e.SpriteBatch.DrawString(
				Game1.smallFont,
				$"HP+   {_healthRegeneration}",
				new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width - 222, Game1.graphics.GraphicsDevice.Viewport.Height - 72),
				Color.White);
			e.SpriteBatch.DrawString(
				Game1.smallFont,
				$"EP+   {_staminaRegeneration}",
				new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width - 222, Game1.graphics.GraphicsDevice.Viewport.Height - 48),
				Color.White);
		}

		private void Event_FoodRegeneration(object sender, UpdateTickedEventArgs e)
		{
			if ((!Game1.IsMultiplayer && !Game1.game1.IsActive) || (Game1.activeClickableMenu != null && !Game1.shouldTimePass()))
				return;

			if (Game1.player.health < 1 || _healthRegeneration < 1 && _staminaRegeneration < 1)
			{
				Helper.Events.GameLoop.UpdateTicked -= Event_FoodRegeneration;
				return;
			}

			var cookingLevel = CookingSkillApi.GetLevel();
			var baseRate = 128;
			var panicRate = (Game1.player.health * 3f + Game1.player.Stamina)
			                / (Game1.player.maxHealth * 3f + Game1.player.MaxStamina);
			var regenRate = GetFoodRegenRate(_lastFoodEaten);
			var scaling =
				(Game1.player.CombatLevel * CombatRegenModifier
				   + (Config.AddCookingSkillAndRecipes ? cookingLevel * CookingRegenModifier : 0)
				   + Game1.player.ForagingLevel * ForagingRegenModifier)
				/ (10 * CombatRegenModifier
				   + (Config.AddCookingSkillAndRecipes ? 10 * CookingRegenModifier : 0)
				   + 10 * ForagingRegenModifier);
			var rate = (baseRate - baseRate * scaling) * regenRate * 100d;
			rate = Math.Floor(Math.Max(36 - cookingLevel * 1.75f, rate * panicRate));

			_debugRegenRate = (float) rate;
			_debugElapsedTime = e.Ticks;
			++_regenTicksCurr;

			if (_regenTicksCurr < rate)
				return;

			_regenTicksDiff.Enqueue(_regenTicksCurr);
			if (_regenTicksDiff.Count > 5)
				_regenTicksDiff.Dequeue();
			_regenTicksCurr = 0;

			if (_healthRegeneration > 0)
			{
				if (Game1.player.health < Game1.player.maxHealth)
					++Game1.player.health;
				--_healthRegeneration;
			}

			if (_staminaRegeneration > 0)
			{
				if (Game1.player.Stamina < Game1.player.MaxStamina)
					++Game1.player.Stamina;
				--_staminaRegeneration;
			}
		}

		private void Event_WatchingToolUpgrades(object sender, UpdateTickedEventArgs e)
		{
			// Checks for purchasing a cooking tool upgrade from Clint's upgrade menu
			if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ShopMenu menu
				&& menu.heldItem != null && menu.heldItem is StardewValley.Tools.GenericTool tool)
			{
				if (DoesCookingToolNameMatch(tool) && tool.IndexOfMenuItemView - 17 < 4)
				{
					Game1.player.toolBeingUpgraded.Value = tool;
					Game1.player.daysLeftForToolUpgrade.Value = Config.DebugMode ? 0 : 2;
					Game1.playSound("parry");
					Game1.exitActiveMenu();
					Game1.drawDialogue(Game1.getCharacterFromName("Clint"),
						Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14317"));
				}
			}

			// Checks for collecting your upgraded cooking tool from Clint after waiting the upgrade period
			if (Game1.player.mostRecentlyGrabbedItem != null && Game1.player.mostRecentlyGrabbedItem is StardewValley.Tools.GenericTool tool1)
			{
				if (DoesCookingToolNameMatch(tool1) && tool1.IndexOfMenuItemView - 17 > CookingToolLevel - 1)
				{
					++CookingToolLevel;
				}
			}

			if (Game1.currentLocation == null || Game1.currentLocation.Name != "Blacksmith")
			{
				Log.D("Ending watch for blacksmith tool upgrades.",
					Config.DebugMode);
				Helper.Events.GameLoop.UpdateTicked -= Event_WatchingToolUpgrades;
			}
		}

		private void Event_WatchingBuffs(object sender, UpdateTickedEventArgs e)
		{
			if ((Game1.activeClickableMenu != null && Game1.activeClickableMenu is TitleMenu)
				|| Game1.player == null
				|| _watchingBuff == null
				|| (Game1.buffsDisplay.food?.source != _watchingBuff.source
					&& Game1.buffsDisplay.drink?.source != _watchingBuff.source
					&& Game1.buffsDisplay.otherBuffs.Any()
					&& Game1.buffsDisplay.otherBuffs.All(buff => buff?.source != _watchingBuff.source)))
			{
				Helper.Events.GameLoop.UpdateTicked -= Event_WatchingBuffs;

				_watchingBuff = null;
			}
		}
		
		private void Event_MoveJunimo(object sender, UpdateTickedEventArgs e)
		{
			Helper.Events.GameLoop.UpdateTicked -= Event_MoveJunimo;
			var cc = GetCommunityCentre();
			var p = CommunityCentreNotePosition;
			if (cc.characters.FirstOrDefault(c => c is Junimo j && j.whichArea.Value == CommunityCentreAreaNumber) == null)
			{
				Log.D($"No junimo in area {CommunityCentreAreaNumber} to move!",
					Config.DebugMode);
			}
			else
			{
				cc.characters.FirstOrDefault(c => c is Junimo j && j.whichArea.Value == CommunityCentreAreaNumber)
					.Position = new Vector2(p.X, p.Y + 2) * 64f;
				Log.D("Moving junimo",
					Config.DebugMode);
			}
		}

		private void Event_ChangeJunimoMenuTab(object sender, UpdateTickedEventArgs e)
		{
			Helper.Events.GameLoop.UpdateTicked -= Event_ChangeJunimoMenuTab;
			Helper.Reflection.GetField<int>((JunimoNoteMenu)Game1.activeClickableMenu, "whichArea").SetValue(_menuTab);
			if (_menuTab == CommunityCentreAreaNumber)
			{
				((JunimoNoteMenu)Game1.activeClickableMenu).bundles.Clear();
				((JunimoNoteMenu)Game1.activeClickableMenu).setUpMenu(CommunityCentreAreaNumber, GetCommunityCentre().bundlesDict());
			}
		}

		/// <summary>
		/// TemporaryAnimatedSprite shows behind player and game objects in its default position,
		/// so all this hub-bub is needed to draw above other game elements.
		/// </summary>
		internal static void Event_RenderTempSpriteOverWorld(object sender, RenderedWorldEventArgs e)
		{
			var sprite = Game1.currentLocation.getTemporarySpriteByID(CookingMenu.SpriteId);
			if (sprite == null)
			{
				Instance.Helper.Events.Display.RenderedWorld -= Event_RenderTempSpriteOverWorld;
				return;
			}
			sprite.draw(e.SpriteBatch, localPosition: false, xOffset: 0, yOffset: 0, extraAlpha: 1f);
		}

		private void Event_ReplaceCraftingMenu(object sender, UpdateTickedEventArgs e)
		{
			Helper.Events.GameLoop.UpdateTicked -= Event_ReplaceCraftingMenu;
			OpenNewCookingMenu();
		}

		private void Event_DrawOverEnglishRecipeNames(object sender, RenderedEventArgs e)
		{
			if (!(Game1.activeClickableMenu is CraftingPage cm) || !Helper.Reflection.GetField<bool>(cm, "cooking").GetValue()
				|| LocalizedContentManager.CurrentLanguageCode.ToString() != "en")
			{
				Helper.Events.Display.Rendered -= Event_DrawOverEnglishRecipeNames;
				return;
			}

			var craftingMenu = Game1.activeClickableMenu as CraftingPage;
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
				var buffIconsToDisplay = Game1.objectInformation[(lastCookingHover as Object).ParentSheetIndex].Split('/').Length > 7
					? Game1.objectInformation[(lastCookingHover as Object).parentSheetIndex].Split('/')[7].Split(' ')
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
				if (lastCookingHover is Object o && o.Edibility != -300)
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

		private void InputOnButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			if (!Game1.game1.IsActive || Game1.currentLocation == null)
			{
				return;
			}

			// Menu interactions
			if (e.Button.IsUseToolButton())
			{
				/*
				// Add experience for cooking skill when not using the new cooking menu
				if (Config.AddCookingSkillAndRecipes && !Config.AddCookingMenu
					&& Game1.activeClickableMenu is CraftingPage cookingMenu && Helper.Reflection.GetField<bool>(cookingMenu, "cooking").GetValue())
				{
					var shift = Game1.oldKBState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift);
					var item = Helper.Reflection.GetField<Item>(cookingMenu, "heldItem").GetValue();
					var currentPage = Helper.Reflection.GetField<int>(cookingMenu, "currentCraftingPage").GetValue();
					var ingredientsCount = Helper.Reflection.GetField<Dictionary<int, int>>(new CraftingRecipe(item.Name, true), "recipeList").GetValue().Count;
					// TODO: DEBUG: Cooking experience without cooking menu is not going to work, needs to account for 
					foreach (var recipe in cookingMenu.pagesOfCraftingRecipes[currentPage].Keys)
					{
						if (recipe.containsPoint(Game1.getMouseX(), Game1.getMouseY()) && !recipe.hoverText.Equals("ghosted")
							&& cookingMenu.pagesOfCraftingRecipes[currentPage][recipe].doesFarmerHaveIngredientsInInventory(
								Helper.Reflection.GetMethod(cookingMenu, "getContainerContents").Invoke<IList<Item>>()))
						{
							item = null;
							break;
						}
					}
					if (item != null)
					{
						item.Stack = shift ? 5 : 1;
						CookingSkillApi.CalculateExperienceGainedFromCookingItem(item, ingredientsCount, apply: true);
					}
				}
				*/
				// Navigate community centre bundles inventory menu
				var cc = GetCommunityCentre();
				var cursor = Utility.Vector2ToPoint(e.Cursor.ScreenPixels);
				if (IsCommunityCentreKitchenEnabledByHost() && Game1.activeClickableMenu is JunimoNoteMenu menu && menu != null
					&& cc.areasComplete.Count > CommunityCentreAreaNumber && !cc.areasComplete[CommunityCentreAreaNumber]
					&& cc.shouldNoteAppearInArea(CommunityCentreAreaNumber))
				{
					if (!Game1.player.hasOrWillReceiveMail("canReadJunimoText"))
					{
						Game1.activeClickableMenu.exitThisMenu();
						return;
					}

					NavigateJunimoNoteMenuAroundKitchen(cc, menu, cursor.X, cursor.Y);
				}
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
							: new Warp(0, 0, "CommunityCenter", 12, 6, false));
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
					else if (IndoorsTileIndexesThatActAsCookingStations.Contains(tile.TileIndex))
					{
						if (NpcHomeLocations.Any(pair => pair.Value == Game1.currentLocation.Name
								&& Game1.player.getFriendshipHeartLevelForNPC(pair.Key) >= NpcKitchenFriendshipRequired)
							|| NpcHomeLocations.All(pair => pair.Value != Game1.currentLocation.Name))
						{
							Log.D($"Clicked the kitchen at {Game1.currentLocation.Name}",
								Config.DebugMode);
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

				// Open Community Centre fridge door
				if (IsCommunityCentreKitchenEnabledByHost() && Game1.currentLocation is CommunityCenter cc
					&& tile != null && tile.TileIndex == 634)
				{
					CommunityCentreFridgePosition = e.Cursor.GrabTile;

					// Change tile to use custom open-fridge sprite
					Game1.currentLocation.Map.GetLayer("Front")
						.Tiles[(int)CommunityCentreFridgePosition.X, (int)CommunityCentreFridgePosition.Y - 1]
						.TileIndex = 1122;
					Game1.currentLocation.Map.GetLayer("Buildings")
						.Tiles[(int)CommunityCentreFridgePosition.X, (int)CommunityCentreFridgePosition.Y]
						.TileIndex = 1154;

					if (!((CommunityCenter)Game1.currentLocation).Objects.ContainsKey(DummyFridgePosition))
					{
						((CommunityCenter)Game1.currentLocation).Objects.Add(
							DummyFridgePosition, new Chest(true, DummyFridgePosition));
					}

					// Open the fridge as a chest
					((Chest)cc.Objects[DummyFridgePosition]).fridge.Value = true;
					((Chest)cc.Objects[DummyFridgePosition]).checkForAction(Game1.player);

					Helper.Input.Suppress(e.Button);
				}

				// Use tile actions in maps
				CheckTileAction(e.Cursor.GrabTile, Game1.currentLocation);
			}
			else if (e.Button.IsUseToolButton())
			{
				// Ignore Nettles used on Kegs to make Nettle Tea when Cooking skill level is too low
				if ((!Config.AddCookingSkillAndRecipes || CookingSkillApi.GetLevel() < NettlesUsableLevel)
					&& Game1.player.ActiveObject != null
					&& Game1.player.ActiveObject.Name.ToLower().EndsWith("nettles")
					&& Game1.currentLocation.Objects[e.Cursor.GrabTile]?.Name == NettlesUsableMachine)
				{
					Helper.Input.Suppress(e.Button);
					Game1.playSound("cancel");
				}
			}
		}

		private void DisplayOnMenuChanged(object sender, MenuChangedEventArgs e)
		{
			if (e.OldMenu is TitleMenu || e.NewMenu is TitleMenu || Game1.currentLocation == null || Game1.player == null)
			{
				return;
			}

			// Check to send multiplayer bundle warning mail when building new cabins
			if (IsCommunityCentreKitchenEnabledByHost())
			{
				var currentCabinsCount = GetNumberOfCabinsBuilt();
				if (e.OldMenu is CarpenterMenu && currentCabinsCount > _debugLastCabinsCount)
				{
					_debugLastCabinsCount = currentCabinsCount;
					AddNewPendingNotification(Notification.CabinBuiltWarning);
				}
			}

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
					Game1.player.mostRecentlyGrabbedItem = new Object(JsonAssets.GetObjectId(ObjectPrefix + "cookbook"), 0);
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
					var o = new Object(Vector2.Zero, JsonAssets.GetObjectId(ChocolateName), int.MaxValue);
					menu.itemPriceAndStock.Add(o, new [] {(int) (o.Price * Game1.MasterPlayer.difficultyModifier), int.MaxValue});
					menu.forSale.Insert(menu.forSale.FindIndex(i => i.Name == "Sugar"), o);
				}
				else if (menu.portraitPerson != null && menu.portraitPerson.Name == "Gus" && !Game1.currentLocation.IsOutdoors && IsCommunityCentreComplete())
				{
					// Add chocolate to Gus' shop
					var o = new Object(Vector2.Zero, JsonAssets.GetObjectId(ChocolateName), int.MaxValue);
					menu.itemPriceAndStock.Add(o, new[] { (int)((o.Price - 35) * Game1.MasterPlayer.difficultyModifier), int.MaxValue });
					menu.forSale.Insert(menu.forSale.FindIndex(i => i.Name == "Coffee"), o);
				}
				else if (Config.AddCookingToolProgression && Game1.currentLocation?.Name == "Blacksmith")
				{
					// Upgrade cooking equipment at the blacksmith
					var canUpgrade = CanFarmerUpgradeCookingEquipment();
					var level = CookingToolLevel;
					if (canUpgrade)
					{
						if (e.NewMenu is ShopMenu upgradeMenu && upgradeMenu.itemPriceAndStock.Keys.All(key => key.Name != "Coal"))
						{
							var cookingTool = GenerateCookingTool(level);
							var price = Helper.Reflection.GetMethod(
								typeof(Utility), "priceForToolUpgradeLevel").Invoke<int>(level + 1);
							var index = Helper.Reflection.GetMethod(
								typeof(Utility), "indexOfExtraMaterialForToolUpgrade").Invoke<int>(level + 1);
							upgradeMenu.itemPriceAndStock.Add(cookingTool, new int[3] { price / 2, 1, index });
							upgradeMenu.forSale.Add(cookingTool);
						}
					}
				}

				return;
			}

			if (e.NewMenu is CraftingPage craftingMenu && Helper.Reflection.GetField<bool>(craftingMenu, "cooking").GetValue())
			{
				// Open the new Cooking Menu as a substitute when a cooking CraftingPage is opened
				if (Config.AddCookingMenu)
				{
					craftingMenu.exitThisMenuNoSound();
					Game1.activeClickableMenu = null;
					Helper.Events.GameLoop.UpdateTicked += Event_ReplaceCraftingMenu;
				}

				return;
			}

			// Close Community Centre fridge door after use in the renovated kitchen
			if (e.OldMenu is ItemGrabMenu && e.NewMenu == null
				&& IsCommunityCentreKitchenEnabledByHost() && Game1.currentLocation is CommunityCenter cc
				&& (IsCommunityCentreComplete() || (cc.areasComplete.Count > CommunityCentreAreaNumber && cc.areasComplete[CommunityCentreAreaNumber])))
			{
				cc.Map.GetLayer("Front")
					.Tiles[(int)CommunityCentreFridgePosition.X, (int)CommunityCentreFridgePosition.Y - 1]
					.TileIndex = 602;
				cc.Map.GetLayer("Buildings")
					.Tiles[(int)CommunityCentreFridgePosition.X, (int)CommunityCentreFridgePosition.Y]
					.TileIndex = 634;
				return;
			}
		}

		private void PlayerOnInventoryChanged(object sender, InventoryChangedEventArgs e)
		{
			// Add experience when cooking food without the new cooking menu enabled
			/*
			if (Config.AddCookingSkillAndRecipes && !Config.AddCookingMenu)
			{
				var itemsAdded = e.Added.Where(item => item.Category == -7);
				var itemsIncreased = e.QuantityChanged.Where(pair => pair.Item.Category == -7 && pair.NewSize > pair.OldSize);
				var items = itemsAdded.ToDictionary(item => item, item => item.Stack)
					.Concat(itemsIncreased.ToDictionary(pair => pair.Item, pair => pair.NewSize - pair.OldSize));

				foreach (var pair in items)
				{
					for (var i = 0; i < pair.Value; ++i)
					{
						var recipe = new CraftingRecipe(pair.Key.Name);
						CookingSkillApi.CalculateExperienceGainedFromCookingItem(pair.Key, ingredientsCount: recipe.recipeList.Count, apply: false);
					}
				}
			}
			*/

			// Handle unique craftable input/output
			if (Game1.activeClickableMenu == null
				&& Config.AddNewCropsAndStuff
				&& JsonAssets != null
				&& Game1.currentLocation.Objects.ContainsKey(Game1.currentLocation.getTileAtMousePosition())
				&& Game1.currentLocation.Objects[Game1.currentLocation.getTileAtMousePosition()] is Object craftable
				&& craftable != null && craftable.bigCraftable.Value)
			{
				if (craftable.Name == "Keg")
				{
					if (NettlesEnabled && Game1.player.mostRecentlyGrabbedItem?.Name == ObjectPrefix + "nettles")
					{
						var name = ObjectPrefix + "nettletea";
						craftable.heldObject.Value = new Object(Vector2.Zero, JsonAssets.GetObjectId(name), name,
							canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
						craftable.MinutesUntilReady = 180;
					}
					else if (CiderEnabled && Game1.player.mostRecentlyGrabbedItem != null && Game1.player.mostRecentlyGrabbedItem.Name.EndsWith("Apple"))
					{
						var name = ObjectPrefix + "cider";
						craftable.heldObject.Value = new Object(Vector2.Zero, JsonAssets.GetObjectId(name), name,
							canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
						craftable.MinutesUntilReady = 1900;
					}
					else if (PerryEnabled && Game1.player.mostRecentlyGrabbedItem != null && Game1.player.mostRecentlyGrabbedItem.Name.EndsWith("Pear"))
					{
						var name = ObjectPrefix + "perry";
						craftable.heldObject.Value = new Object(Vector2.Zero, JsonAssets.GetObjectId(name), name,
							canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
						craftable.MinutesUntilReady = 1900;
					}
				}
				else if (craftable.Name == "Preserves Jar")
				{
					if (MarmaladeEnabled && e.Removed.FirstOrDefault(o => MarmaladeFoods.Contains(o.Name)) is Object dropIn && dropIn != null)
					{
						craftable.heldObject.Value = new Object(Vector2.Zero, JsonAssets.GetObjectId(ObjectPrefix + "marmalade"), dropIn.Name + " Marmalade",
							canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
						craftable.heldObject.Value.Price = 65 + dropIn.Price * 2;
						craftable.heldObject.Value.name = dropIn.Name + " Marmalade";
						craftable.MinutesUntilReady = 4600;
					}
				}
			}
		}

		private void PlayerOnWarped(object sender, WarpedEventArgs e)
		{
			if (Config.AddCookingToolProgression && e.NewLocation.Name == "Blacksmith")
			{
				Log.D("Watching for blacksmith tool upgrades.",
					Config.DebugMode);
				Helper.Events.GameLoop.UpdateTicked += Event_WatchingToolUpgrades;
			}

			if ((!(e.NewLocation is CommunityCenter) && e.OldLocation is CommunityCenter)
				|| !(e.OldLocation is CommunityCenter) && e.NewLocation is CommunityCenter)
			{
				Helper.Content.InvalidateCache(@"Maps/townInterior");
			}

			if (e.NewLocation is CommunityCenter cc)
			{
				// TODO: SYSTEM???: Add failsafe for delivering community centre completed mail with all bundles complete,
				// assuming that our bundle was removed when the usual number of bundles were completed
				if (false && GetCommunityCentre().areAllAreasComplete()
					&& !Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
				{
					Log.W("Hit unusual failsafe for all CC areas completed without CC considered complete");
					int x = 32, y = 13;
					Utility.getDefaultWarpLocation(cc.Name, ref x, ref y);
					Game1.player.Position = new Vector2(x, y) * 64f;
					cc.junimoGoodbyeDance();
				}

				if (IsCommunityCentreKitchenEnabledByHost())
				{
					Helper.Events.GameLoop.UpdateTicked += Event_MoveJunimo; // fgs fds
					Log.D($"Warped to CC: areasComplete count: {cc.areasComplete.Count}, complete: {IsCommunityCentreComplete()}",
						Config.DebugMode);

					if (IsCommunityCentreComplete())
					{
						var multiplayer = Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
						multiplayer.broadcastSprites(
							Game1.currentLocation,
							new TemporaryAnimatedSprite(
								"LooseSprites\\Cursors",
								new Rectangle(354, 401, 7, 7),
								9999, 1, 9999,
								new Vector2(2096f, 344f),
								false, false, 0.8f, 0f, Color.White,
								4f, 0f, 0f, 0f)
							{
								holdLastFrame = true
							});
					}
					else
					{
						CheckAndTryToUnrenovateKitchen();
					}
				}
			}
		}

		private void MultiplayerOnPeerContextReceived(object sender, PeerContextReceivedEventArgs e)
		{
			Log.D($"Peer context received: {e.Peer.PlayerID} : SMAPI:{e.Peer.HasSmapi}" +
				$" CAC:{(e.Peer.Mods?.ToList().FirstOrDefault(mod => mod.ID == Helper.ModRegistry.ModID) is IMultiplayerPeerMod mod && mod != null ? mod.Version.ToString() : "null")}",
				Config.DebugMode);
			/*
			if (Game1.IsMasterGame)
			{
				Log.I("A new player has joined your multiplayer session.");

				if (Config.AddCookingCommunityCentreBundles)
				{
					Log.I("Disabling new Community Centre bundles for multiplayer.");
					SetCommunityCentreKitchenForThisSession(false);
				}
				if (Game1.activeClickableMenu is JunimoNoteMenu || Game1.activeClickableMenu is GameMenu)
				{
					Log.I("Closing bundle menus.");
					Game1.activeClickableMenu.emergencyShutDown();
				}
				if (Game1.currentLocation is CommunityCenter)
				{
					Log.I("Stepping out of the Community Centre.");
					Game1.player.warpFarmer(new Warp(0, 0, "Town", 52, 22, false));
				}
				Log.I("Unloading any custom bundle data.");
				SaveAndUnloadBundleData();
			}
			*/
		}

		private void MultiplayerOnPeerConnected(object sender, PeerConnectedEventArgs e)
		{
			Log.D($"Peer connected to multiplayer session: {e.Peer.PlayerID} : SMAPI:{e.Peer.HasSmapi}" +
				$" CAC:{(e.Peer.Mods?.ToList().FirstOrDefault(mod => mod.ID == Helper.ModRegistry.ModID) is IMultiplayerPeerMod mod && mod != null ? mod.Version.ToString() : "null")}",
				Config.DebugMode);

			/*
			if (Game1.IsMasterGame)
			{
				Log.I("A new player has joined your multiplayer session.");

				if (Config.AddCookingCommunityCentreBundles)
				{
					Log.I("Disabling new Community Centre bundles for multiplayer.");
					SetCommunityCentreKitchenForThisSession(false);
				}
				if (Game1.activeClickableMenu is JunimoNoteMenu || Game1.activeClickableMenu is GameMenu)
				{
					Log.I("Closing bundle menus.");
					Game1.activeClickableMenu.emergencyShutDown();
				}
				if (Game1.currentLocation is CommunityCenter)
				{
					Log.I("Stepping out of the Community Centre.");
					Game1.player.warpFarmer(new Warp(0, 0, "Town", 52, 22, false));
				}
				Log.I("Unloading any custom bundle data.");
				SaveAndUnloadBundleData();
				//ReloadBundleData();

				// Send the peer their saved LocalData from SharedData
				//BroadcastPeerData(e.Peer.PlayerID);
			}
			else
			{
				//ReloadBundleData();
			}
			*/
		}

		private void SpaceEventsOnItemEaten(object sender, EventArgs e)
		{
			if (!(Game1.player.itemToEat is Object food))
				return;

			var objectData = Game1.objectInformation[food.ParentSheetIndex].Split('/');
			_lastFoodWasDrink = objectData.Length > 6 && objectData[6] == "drink";
			_lastFoodEaten = food;

			Log.D($"Ate food: {food.Name}\nBuffs: (food) {Game1.buffsDisplay.food?.displaySource} (drink) {Game1.buffsDisplay.drink?.displaySource}",
				Config.DebugMode);

			// Determine food healing
			if (Config.FoodHealingTakesTime)
			{
				// Regenerate health/energy over time
				Helper.Events.GameLoop.UpdateTicked += Event_FoodRegeneration;
				Game1.player.health = (int)_healthOnLastTick;
				Game1.player.Stamina = _staminaOnLastTick;
				_healthRegeneration += food.healthRecoveredOnConsumption();
				_staminaRegeneration += food.staminaRecoveredOnConsumption();
			}
			else if (Config.AddCookingSkillAndRecipes
			         && Game1.player.HasCustomProfession(CookingSkillApi.GetSkill().Professions[(int) CookingSkill.ProfId.Restoration]))
			{
				// Add additional health
				Game1.player.health = (int) Math.Min(Game1.player.maxHealth,
					Game1.player.health + food.healthRecoveredOnConsumption() * (CookingSkill.RestorationAltValue / 100f));
				Game1.player.Stamina = (int) Math.Min(Game1.player.MaxStamina,
					Game1.player.Stamina + food.staminaRecoveredOnConsumption() * (CookingSkill.RestorationAltValue / 100f));
			}

			var lastBuff = _lastFoodWasDrink
				? Game1.buffsDisplay.drink
				: Game1.buffsDisplay.food;
			Log.D($"OnItemEaten"
				+ $" | Ate food:  {food.Name}"
				+ $" | Last buff: {lastBuff?.displaySource ?? "null"} (source: {lastBuff?.source ?? "null"})",
				Config.DebugMode);

			// Check to boost buff duration
			if ((Config.AddCookingSkillAndRecipes
			    && Game1.player.HasCustomProfession(CookingSkillApi.GetSkill().Professions[(int)CookingSkill.ProfId.BuffDuration]))
			    && food.displayName == lastBuff?.displaySource)
			{
				var duration = lastBuff.millisecondsDuration;
				if (duration > 0)
				{
					var rate = (Game1.player.health + Game1.player.Stamina)
					               / (Game1.player.maxHealth + Game1.player.MaxStamina);
					duration += (int) Math.Floor(CookingSkill.BuffDurationValue * 1000 * rate);
					lastBuff.millisecondsDuration = duration;
				}
			}

			// Track buffs received
			_watchingBuff = lastBuff;

			// Track foods eaten
			if (!FoodsEaten.Contains(food.Name))
			{
				FoodsEaten.Add(food.Name);
			}

			// Add leftovers from viable foods to the inventory, or drop it on the ground if full
			if (FoodsThatGiveLeftovers.Contains(food.Name) && Config.AddRecipeRebalancing && JsonAssets != null)
			{
				var name = food.Name.StartsWith(ObjectPrefix) ? $"{food.Name}_half" : $"{food.Name.ToLower().Split(' ').Aggregate(ObjectPrefix, (s, s1) => s + s1)}" + "_half";
				var leftovers = new Object(JsonAssets.GetObjectId(name), 1);
				if (Game1.player.couldInventoryAcceptThisItem(leftovers))
					Game1.player.addItemToInventory(leftovers);
				else
					Game1.createItemDebris(leftovers, Game1.player.Position, -1);
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
						_healthRegeneration -= food.healthRecoveredOnConsumption();
						_staminaRegeneration -= food.staminaRecoveredOnConsumption();
					}
					else
					{
						Game1.player.health = (int)_healthOnLastTick;;
						Game1.player.Stamina = _staminaOnLastTick;
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
						_healthRegeneration += Game1.player.maxHealth / 10;
						_staminaRegeneration += Game1.player.MaxStamina / 10;
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

			// Track added buffs
			if (CookingAddedLevelsEnabled && Config.AddCookingSkillAndRecipes
				&& ((!_lastFoodWasDrink && Game1.buffsDisplay.food?.source == food.Name)
					|| (_lastFoodWasDrink && Game1.buffsDisplay.drink?.source == food.Name)))
			{
				// TODO: UPDATE: Cooking Skill added levels
				CookingSkillApi.GetSkill().AddedLevel = 0;
				Helper.Events.GameLoop.UpdateTicked += Event_WatchingBuffs;
			}
		}
		
		private void SpaceEventsOnBeforeGiftGiven(object sender, EventArgsBeforeReceiveObject e)
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
			if (Config.AddCookingSkillAndRecipes
			    && Game1.player.HasCustomProfession(CookingSkillApi.GetSkill().Professions[(int)CookingSkill.ProfId.GiftBoost])
			    && e.Gift.Category == -7)
			{
				Game1.player.changeFriendship(CookingSkill.GiftBoostValue, e.Npc);
			}
		}

		/// <summary>
		/// Checks whether the player has agency during gameplay, cutscenes, and while in menus.
		/// </summary>
		public bool PlayerAgencyLostCheck()
		{
			// HOUSE RULES
			return !Game1.game1.IsActive // No alt-tabbed game state
			       || (Game1.eventUp && Game1.currentLocation != null && !Game1.currentLocation.currentEvent.playerControlSequence) // No event cutscenes
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
						Object o = null;
						if (roll < 0.2f && Game1.player.eventsSeen.Contains(0))
						{
							o = new Object(832, 1); // Pineapple
							if (roll < 0.05f && Game1.player.eventsSeen.Contains(1))
								o = new Object(JsonAssets.GetObjectId(ChocolateName), 1);
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
		private void CreateInspectDialogue(string dialogue)
		{
			Game1.drawDialogueNoTyping(dialogue);
		}

		private void SaveLoadedBehaviours()
		{
			// Reset per-world config values
			var savedConfig = Helper.ReadConfig<Config>();
			Config.AddCookingCommunityCentreBundles = savedConfig.AddCookingCommunityCentreBundles;

			HarmonyPatches.Patch();

			// Load local persistent data from saved modData
			IsUsingRecipeGridView = Game1.player.modData.TryGetValue(
				AssetPrefix + "grid_view", out var gridView) ? bool.Parse(gridView) : false;
			CookingToolLevel = Game1.player.modData.TryGetValue(
				AssetPrefix + "tool_level", out var toolLevel) ? int.Parse(toolLevel) : 0;
			FoodsEaten = Game1.player.modData.TryGetValue(
				AssetPrefix + "foods_eaten", out var foodsEaten) ? foodsEaten.Split(',').ToList() : new List<string>();
			FavouriteRecipes = Game1.player.modData.TryGetValue(
				AssetPrefix + "favourite_recipes", out var favouriteRecipes) ? favouriteRecipes.Split(',').ToList() : new List<string>();

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

			// Populate NPC home locations for cooking range usage
			var npcData = Game1.content.Load<Dictionary<string, string>>("Data/NPCDispositions");
			NpcHomeLocations = new Dictionary<string, string>();
			foreach (var npc in npcData)
			{
				NpcHomeLocations.Add(npc.Key, npc.Value.Split('/')[10].Split(' ')[0]);
			}

			// Check for sending warnings re: multiplayer and bundles
			_debugLastCabinsCount = GetNumberOfCabinsBuilt();

			// Handle custom bundle data unloading and loading
			var customBundleData = ParseBundleData();
			BundleCount = customBundleData.Count;
			Log.D($"Bundles identified: [{BundleCount}]: {string.Join(", ", customBundleData.Keys)}",
				Config.DebugMode);

			PrintBundleData(GetCommunityCentre());
			Log.D("End of default world bundle data. Now unloading custom bundles.",
				Config.DebugMode);
			SaveAndUnloadBundleData();

			if (!IsCommunityCentreKitchenEnabledByHost())
			{
				Log.D("Did not load bundle data: Community Centre bundles not enabled by host.",
					Config.DebugMode);
			}
			else if (IsCommunityCentreComplete())
			{
				Log.D("Did not load bundle data: Community Centre already completed.",
					Config.DebugMode);
			}
			else
			{
				if (Game1.IsMasterGame)
				{
					// For hosts loading worlds with cabins, show opt-in notification
					if (!Game1.IsMultiplayer && IsMultiplayer())
					{
						Log.D("Sending notification re: multiplayer world bundle data opt-in.",
							Config.DebugMode);
						SetCommunityCentreKitchenForThisSession(false);
						AddNewPendingNotification(Notification.BundleMultiplayerWarning);
					}
				}
				else
				{
					Log.D("Loading first-time world bundle data for multiplayer peer.",
						Config.DebugMode);
					LoadBundleData();
				}
			}

			PurgeBrokenFryingPans(sendMail: true);
		}

		private void OpenNewCookingMenu(List<CraftingRecipe> recipes = null)
		{
			Log.D("Check to open new cooking menu.",
				Config.DebugMode);

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
						|| Game1.activeClickableMenu is CookingMenu menu && menu.PopMenuStack(true, true))
					{
						Log.D("Created new CookingMenu",
							Config.DebugMode);
						Game1.activeClickableMenu = new CookingMenu(recipes ?? TakeRecipesFromCraftingPage(craftingMenu));
					}
					else
					{
						Log.D("???",
							Config.DebugMode);
					}
				}
				else
				{
					Log.D("Created new CraftingPage",
						Config.DebugMode);
					Game1.activeClickableMenu = craftingMenu;

					// Draw over recipe display names to remove package naming convention in English locale
					Helper.Events.Display.Rendered += Event_DrawOverEnglishRecipeNames;
				}
			}

			if (Game1.player.hasOrWillReceiveMail(MailCookbookUnlocked))
			{
				var ccFridge = Game1.currentLocation is CommunityCenter cc
					&& (IsCommunityCentreComplete() || (IsCommunityCentreKitchenEnabledByHost() && IsCommunityCentreKitchenCompleted()))
						? cc.Objects.ContainsKey(CommunityCentreFridgePosition) ? (Chest)cc.Objects[CommunityCentreFridgePosition] : null
						: null;
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
					Log.D($"Mutex locked, did not open new cooking menu for fridge at {Game1.currentLocation.Name} {fridge.Value.TileLocation.ToString()}",
						Config.DebugMode);
					Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Kitchen_InUse"));
				}
				else if (fridge.Value == null)
				{
					Log.D($"Opening new cooking menu.",
						Config.DebugMode);
					CreateCookingMenu(fridge, miniFridges);
				}
				else
				{
					Log.D($"Planting mutex request on fridge at {Game1.currentLocation.Name} {fridge.Value.TileLocation.ToString()}",
						Config.DebugMode);
					MultipleMutexRequest multiple_mutex_request = null;
					multiple_mutex_request = new MultipleMutexRequest(muticies, delegate
					{
						fridge.Value.mutex.RequestLock(delegate
						{
							Log.D($"Opening new cooking menu with mutex lock.",
								Config.DebugMode);
							CreateCookingMenu(fridge, miniFridges);
							Game1.activeClickableMenu.exitFunction = delegate
							{
								Log.D($"Releasing mutex locks on fridge at {Game1.currentLocation.Name} {fridge.Value.TileLocation.ToString()}.",
									Config.DebugMode);
								fridge.Value.mutex.ReleaseLock();
								multiple_mutex_request.ReleaseLocks();
							};
						}, delegate
						{
							Log.D($"Mutex locked, did not open new cooking menu for fridge at {Game1.currentLocation.Name} {fridge.Value.TileLocation.ToString()}",
								Config.DebugMode);
							Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Kitchen_InUse"));
							multiple_mutex_request.ReleaseLocks();
						});
					}, delegate
					{
						Log.D($"Mutex locked, did not open new cooking menu for fridge at {Game1.currentLocation.Name} {fridge.Value.TileLocation.ToString()}",
							Config.DebugMode);
						Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Kitchen_InUse"));
					});
				}
			}
			else
			{
				Log.D($"Mail not yet received, did not open cooking menu.",
					Config.DebugMode);
				Game1.activeClickableMenu?.exitThisMenuNoSound();
				CreateInspectDialogue(i18n.Get("menu.cooking_station.no_cookbook"));
			}
		}
		
		/// <summary>
		/// Returns the base health/stamina regeneration rate for some food object.
		/// </summary>
		public float GetFoodRegenRate(Object food)
		{
			// Magical numbers live here

			// Regen faster with drinks
			var rate = _lastFoodWasDrink ? 0.12f : 0.075f;
			// Regen faster with quality
			rate += food.Quality * 0.0085f;
			// Regen faster when drunk
			if (Game1.player.hasBuff(17))
				rate *= 1.3f;
			if (Config.AddCookingSkillAndRecipes && Game1.player.HasCustomProfession(
				CookingSkillApi.GetSkill().Professions[(int)CookingSkill.ProfId.Restoration]))
				rate += rate / CookingSkill.RestorationValue;
			return rate;
		}

		private void AddAndDisplayNewRecipesOnLevelUp(SpaceCore.Interface.SkillLevelUpMenu menu)
		{
			// Add cooking recipes
			var level = CookingSkillApi.GetLevel();
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
			if (level == CraftCampfireLevel)
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
					    || Game1.currentLocation.doesTileHaveProperty(x, y, "Action", "Buildings") != "kitchen" 
					    && !IndoorsTileIndexesThatActAsCookingStations.Contains(tile.TileIndex))
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
			// Thanks Lenne
			if (farmHouse.upgradeLevel == 0
				&& (Helper.ModRegistry.IsLoaded("Allayna.Kitchen")
					|| Helper.ModRegistry.IsLoaded("Froststar11.CustomFarmhouse")
					|| Helper.ModRegistry.IsLoaded("burakmese.products")
					|| Helper.ModRegistry.IsLoaded("minervamaga.FR.BiggerFarmhouses")))
			{
				level = 1;
			}
			return level;
		}

		public int GetFarmersMaxUsableIngredients()
		{
			return Config.AddCookingToolProgression
				? 1 + CookingToolLevel
				: 5;
		}

		private bool CanFarmerUpgradeCookingEquipment()
		{
			var hasMail = Game1.player.mailReceived.Contains(MailCookbookUnlocked);
			var level = CookingToolLevel < 4;
			return hasMail && level;
		}
		
		/// <summary>
		/// Bunches groups of common items together in the seed shop.
		/// Json Assets appends new stock to the bottom, and we don't want that very much at all.
		/// </summary>
		private void SortSeedShopStock(ref ShopMenu menu)
		{
			// Pair a suffix grouping some common items together with the name of the lowest-index (first-found) item in the group
			var itemList = menu.forSale;
			//Log.D(itemList.Aggregate("Shop stock:", (total, cur) => $"{total}\n{cur.Name}"));
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
				//Log.D($"Moved {item.Name} to {itemList[index - 1].Name} at {index}");
			}
			//Log.D($"Sorted seed shop stock, {debugCount} moves.", Config.DebugMode);
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
		/// Update display names for all new cooking recipe objects for the active crafting menu
		/// With English locale, recipes' display names default to the internal name, so we have to replace it
		/// </summary>
		internal void UpdateEnglishRecipeDisplayNames()
		{
			var craftingMenu = (CraftingPage) Game1.activeClickableMenu;
			if (LocalizedContentManager.CurrentLanguageCode.ToString() == "en")
			{
				/*
				for (var i = 0; i < craftingMenu.inventory.actualInventory.Count; ++i)
				{
					if (craftingMenu.inventory.actualInventory[i] == null)
						continue;

					var displayName = Game1.objectInformation[JsonAssets.GetObjectId(craftingMenu.inventory.actualInventory[i].Name)].Split('/')[4];
					craftingMenu.inventory.actualInventory[i].DisplayName = displayName;
				}
				*/
				/*
				for (var i = 0; i < craftingMenu.pagesOfCraftingRecipes.Count; ++i)
				{
					foreach (var pair in craftingMenu.pagesOfCraftingRecipes[i].Where(pair => pair.Value.DisplayName.StartsWith(ObjectPrefix)))
					{
						var displayName = Game1.objectInformation[JsonAssets.GetObjectId(pair.Value.name)].Split('/')[4];
						((CraftingPage)Game1.activeClickableMenu).pagesOfCraftingRecipes[i][pair.Key].DisplayName = displayName;
					}
				}
				*/
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
			return SplitToString(fields, delimiter);
		}

		public static string SplitToString(IEnumerable<string> splitString, char delimiter = '/')
		{
			return splitString.Aggregate((cur, str) => $"{cur}{delimiter}{str}").Remove(0, 0);
		}

		public static bool IsMultiplayer()
		{
			return Game1.IsMultiplayer || GetNumberOfCabinsBuilt() > 0;
		}

		public static int GetNumberOfCabinsBuilt()
		{
			return Game1.getFarm().getNumberBuildingsConstructed("Stone Cabin")
				+ Game1.getFarm().getNumberBuildingsConstructed("Plank Cabin") + Game1.getFarm().getNumberBuildingsConstructed("Log Cabin");
		}

		public static CommunityCenter GetCommunityCentre()
		{
			return Game1.getLocationFromName("CommunityCenter") as CommunityCenter;
		}

		public static bool IsCommunityCentreComplete()
		{
			var cc = GetCommunityCentre();
			if (cc == null)
				return false;

			var ccMasterPlayerComplete = Game1.MasterPlayer.hasCompletedCommunityCenter();
			var ccAreasComplete = cc.areAllAreasComplete();
			return ccMasterPlayerComplete || ccAreasComplete;
		}

		public bool IsCommunityCentreKitchenEnabledByHost()
		{
			var cc = GetCommunityCentre();
			if (cc == null)
				return false;

			var hostEnabled = Game1.IsMasterGame && Config.AddCookingCommunityCentreBundles;
			var bundlesExist = Game1.netWorldState.Value.Bundles.Keys.Any(key => key > BundleStartIndex);
			var areasCompleteEntriesExist = cc.areasComplete.Count > CommunityCentreAreaNumber;
			var clientEnabled = !Game1.IsMasterGame && (bundlesExist || areasCompleteEntriesExist);
			return hostEnabled || clientEnabled;
		}

		public bool IsCommunityCentreKitchenCompleted()
		{
			var cc = GetCommunityCentre();
			if (cc == null)
				return false;

			var receivedMail = Game1.MasterPlayer != null && Game1.MasterPlayer.mailReceived.Contains(MailKitchenCompleted);
			var missingAreasCompleteEntries = cc.areasComplete.Count <= CommunityCentreAreaNumber;
			var areaIsComplete = missingAreasCompleteEntries || cc.areasComplete[CommunityCentreAreaNumber];
			Log.T($"IsCommunityCentreKitchenCompleted: (mail: {receivedMail}), (entries: {missingAreasCompleteEntries}) || (areas: {areaIsComplete})");
			return receivedMail || missingAreasCompleteEntries || areaIsComplete;
		}

		public static bool IsAbandonedJojaMartBundleAvailable()
		{
			return Game1.MasterPlayer != null && !Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater")
				&& Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("abandonedJojaMartAccessible");
		}

		internal void NavigateJunimoNoteMenuAroundKitchen(CommunityCenter cc, JunimoNoteMenu menu, int x, int y)
		{
			var whichArea = Helper.Reflection.GetField<int>(menu, "whichArea").GetValue();
			var lowestArea = -1;
			var highestArea = -1;
			_menuTab = -1;

			// Fetch the bounds of the menu, exclusive of our new area since we're assuming we start there
			// Exclude any already-completed areas from this search, since we're looking for unfinished areas
			for (var i = CommunityCentreAreaNumber - 1; i >= 0; --i)
			{
				if (cc.areasComplete[i] || !cc.shouldNoteAppearInArea(i))
					continue;

				if (highestArea < 0)
					highestArea = i;
				else
					lowestArea = i;
			}

			var backButton = menu.areaBackButton != null && menu.areaBackButton.visible && menu.areaBackButton.containsPoint(x, y);
			var nextButton = menu.areaNextButton != null && menu.areaNextButton.visible && menu.areaNextButton.containsPoint(x, y);
			// When on either the highest or lowest bounds, clicking towards our area will change to it
			if ((whichArea == lowestArea && backButton) || (whichArea == highestArea && nextButton))
			{
				_menuTab = CommunityCentreAreaNumber;
			}
			else if (whichArea == CommunityCentreAreaNumber)
			{
				// When clicking the <= Back button on our area, we'll move to the next-highest index area
				if (backButton)
					_menuTab = highestArea;
				// When clicking the => Next button, we'll move to the first-or-nearest area
				else if (nextButton)
					_menuTab = lowestArea;
			}

			if (_menuTab > -1)
			{
				// Change the menu tab on the next tick to avoid errors
				Helper.Events.GameLoop.UpdateTicked += Event_ChangeJunimoMenuTab;
			}
		}

		/// <summary>
		/// While the Pantry (area 0) is completed, CommunityCenter.loadArea(0) will patch over the kitchen with a renovated map.
		/// This method undoes the renovated map patch by patching over it again with the ruined map.
		/// </summary>
		internal void CheckAndTryToUnrenovateKitchen()
		{
			Log.D($"Checking to unrenovate area for kitchen",
				Config.DebugMode);

			var cc = GetCommunityCentre();
			if (cc.areasComplete.Count <= CommunityCentreAreaNumber || cc.areasComplete[CommunityCentreAreaNumber])
				return;

			Log.D($"Unrenovating kitchen",
				Config.DebugMode);

			// Replace tiles
			cc.Map = Game1.content.Load<Map>(@"Maps/CommunityCenter_Ruins").mergeInto(cc.Map, Vector2.Zero, CommunityCentreArea);

			// Replace lighting
			cc.loadLights();
			cc.addLightGlows();
			Game1.currentLightSources.RemoveWhere(light =>
				light.position.X / 64 < CommunityCentreArea.Width && light.position.Y / 64 < CommunityCentreArea.Height);

			// Add junimo note
			var c1 = cc.isJunimoNoteAtArea(CommunityCentreAreaNumber);
			var c2 = cc.shouldNoteAppearInArea(CommunityCentreAreaNumber);
			if (!c1 && c2)
			{
				Log.D("Adding junimo note manually",
					Config.DebugMode);
				cc.addJunimoNote(CommunityCentreAreaNumber);
			}
		}

		internal void SetCommunityCentreKitchenForThisSession(bool isEnabled)
		{
			Config.AddCookingCommunityCentreBundles = isEnabled;
			Helper.Content.InvalidateCache(@"LooseSprites/JunimoNote");
			Helper.Content.InvalidateCache(@"Maps/townInterior");
			Helper.Content.InvalidateCache(@"Strings/Locations");
			Helper.Content.InvalidateCache(@"Strings/UI");
		}
		
		internal void ReloadBundleData()
		{
			Log.D("CACB Reloading custom bundle data",
				Config.DebugMode);
			SaveAndUnloadBundleData();
			LoadBundleData();
		}

		internal void LoadBundleData()
		{
			var cc = GetCommunityCentre();
			var customBundleData = ParseBundleData();

			Log.D(customBundleData.Aggregate("CACB customBundleData: ", (s, pair) => $"{s}\n{pair.Key}: {pair.Value}"),
				Config.DebugMode);

			// Load custom bundle data from persistent world data if it exists, else default to false (bundles not yet started by player)
			var customBundleValues = new Dictionary<int, bool[]>();
			var customBundleRewards = new Dictionary<int, bool>();
			var isAreaComplete = cc.modData.ContainsKey(AssetPrefix + "area_completed") && !string.IsNullOrEmpty(cc.modData[AssetPrefix + "area_completed"])
				? bool.Parse(cc.modData[AssetPrefix + "area_completed"])
				: false;

			for (var i = 0; i < BundleCount; ++i)
			{
				var key = BundleStartIndex + i;
				// Bundle metadata, not synced with multiplayer.broadcastWorldState()
				Game1.netWorldState.Value.BundleData[$"{CommunityCentreAreaName}/{key}"] = customBundleData[key];
			}

			if (Game1.IsMasterGame)
			{
				// Add GW custom bundle data
				for (var i = 0; i < BundleCount; ++i)
				{
					var key = BundleStartIndex + i;
					// Bundle progress
					var dataKey = AssetPrefix + "bundle_values_" + i;
					customBundleValues.Add(key, cc.modData.ContainsKey(dataKey) && !string.IsNullOrEmpty(cc.modData[dataKey])
						? cc.modData[dataKey].Split(',').ToList().ConvertAll(bool.Parse).ToArray()
						: new bool[customBundleData[key].Split('/')[2].Split(' ').Length]);
					// Bundle saved rewards
					dataKey = AssetPrefix + "bundle_rewards_" + i;
					customBundleRewards.Add(key, cc.modData.ContainsKey(dataKey) && !string.IsNullOrEmpty(cc.modData[dataKey])
						? bool.Parse(cc.modData[dataKey])
						: false);

					Log.D($"CACB Added custom bundle value ({key} [{customBundleRewards[key]}]: {customBundleValues[key].Aggregate("", (str, value) => $"{str} {value}")})",
						Config.DebugMode);
				}

				// Regular load-in for custom bundles
				if (IsCommunityCentreKitchenEnabledByHost() && !IsCommunityCentreComplete())
				{
					// Reload custom bundle data to game savedata
					// World state cannot be added to: it has an expected length once set
					var bundles = new Dictionary<int, bool[]>();
					var bundleRewards = new Dictionary<int, bool>();

					// Fetch vanilla GW bundle data
					for (var i = 0; i < BundleStartIndex; ++i)
					{
						if (Game1.netWorldState.Value.Bundles.ContainsKey(i))
							bundles.Add(i, Game1.netWorldState.Value.Bundles[i]);
						if (Game1.netWorldState.Value.BundleRewards.ContainsKey(i))
							bundleRewards.Add(i, Game1.netWorldState.Value.BundleRewards[i]);
					}

					// Add custom bundle data
					bundles = bundles.Concat(customBundleValues).ToDictionary(pair => pair.Key, pair => pair.Value);
					bundleRewards = bundleRewards.Concat(customBundleRewards).ToDictionary(pair => pair.Key, pair => pair.Value);

					// Apply merged bundle data to world state
					if (customBundleValues.Any(bundle => !Game1.netWorldState.Value.Bundles.ContainsKey(bundle.Key)))
					{
						Log.D("CACB Adding missing GW bundle entries with reset",
							Config.DebugMode);
						Game1.netWorldState.Value.Bundles.Clear();
						Game1.netWorldState.Value.Bundles.Set(bundles);
					}
					if (customBundleData.Any(bundle => !Game1.netWorldState.Value.BundleRewards.ContainsKey(bundle.Key)))
					{
						Log.D("CACB Adding missing GW bundleReward entries with reset",
							Config.DebugMode);
						Game1.netWorldState.Value.BundleRewards.Clear();
						Game1.netWorldState.Value.BundleRewards.Set(bundleRewards);
					}

					var multiplayer = Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
					multiplayer.broadcastWorldStateDeltas();
					multiplayer.broadcastLocationDelta(GetCommunityCentre());

					Log.D($"CACB Loaded GW bundle data progress and broadcasted world state.",
						Config.DebugMode);
				}
			}
			else
			{
				Log.D("CACB Did not load GW custom bundle data, peer is not host.",
					Config.DebugMode);
			}

			try
			{
				if (cc.areasComplete.Count <= CommunityCentreAreaNumber)
				{
					Log.D("CACB Adding new bundle data to CC areas-complete dictionary.",
						Config.DebugMode);

					// Add a new entry to areas complete game data
					var oldAreas = cc.areasComplete;
					var newAreas = new NetArray<bool, NetBool>(CommunityCentreAreaNumber + 1);
					for (var i = 0; i < oldAreas.Count; ++i)
						newAreas[i] = oldAreas[i];
					newAreas[newAreas.Length - 1] = Game1.MasterPlayer.hasOrWillReceiveMail(MailKitchenCompleted);
					cc.areasComplete.Clear();
					cc.areasComplete.Set(newAreas);
				}

				var badField = Helper.Reflection.GetField<Dictionary<int, int>>(cc, "bundleToAreaDictionary");
				var bad = badField.GetValue();
				if (customBundleData.Keys.Any(key => !bad.ContainsKey(key)))
				{
					Log.D("CACB Adding new data to CC bundle-area dictionary.",
						Config.DebugMode);

					// Add a reference to the new community centre kitchen area to the reference dictionary
					for (var i = 0; i < BundleCount; ++i)
					{
						bad[BundleStartIndex + i] = CommunityCentreAreaNumber;
					}
					badField.SetValue(bad);
				}

				var abdField = Helper.Reflection.GetField<Dictionary<int, List<int>>>(cc, "areaToBundleDictionary");
				var abd = abdField.GetValue();
				if (customBundleData.Keys.Any(key => !abd[CommunityCentreAreaNumber].Contains(key)))
				{
					Log.D("CACB Adding new data to CC area-bundle dictionary.",
						Config.DebugMode);

					// Add references to the new community centre bundles to the reference dictionary
					foreach (var bundle in customBundleData.Keys)
					{
						if (!abd[CommunityCentreAreaNumber].Contains(bundle))
							abd[CommunityCentreAreaNumber].Add(bundle);
					}
					abdField.SetValue(abd);
				}

				Log.D($"CACB Loaded CC bundle data progress",
					Config.DebugMode);
			}
			catch (Exception e)
			{
				Log.E($"CACB Error while updating CC areasComplete/bundleAreas/areaBundles:"
					+ $"\nMultiplayer: {Game1.IsMultiplayer}-{IsMultiplayer()}"
					+ $", MasterGame: {Game1.IsMasterGame}"
					+ $", MasterPlayer: {Game1.player.UniqueMultiplayerID == Game1.MasterPlayer.UniqueMultiplayerID}"
					+ $", FarmHands: {Game1.getAllFarmhands().Count()}"
					+ $"\n{e}");
			}
			PrintBundleData(cc);
			Log.D("CACB End of loaded custom bundle data.",
				Config.DebugMode);
		}

		internal void SaveAndUnloadBundleData()
		{
			var cc = GetCommunityCentre();
			if (Game1.IsMasterGame)
			{
				for (var i = 0; i < BundleCount; ++i)
				{
					var key = BundleStartIndex + i;

					// Save GW bundle data to persistent data in community centre
					if (Game1.netWorldState.Value.Bundles.ContainsKey(key))
						cc.modData[AssetPrefix + "bundle_values_" + i] = string.Join(",", Game1.netWorldState.Value.Bundles[key]);
					if (Game1.netWorldState.Value.BundleRewards.ContainsKey(key))
						cc.modData[AssetPrefix + "bundle_rewards_" + i] = Game1.netWorldState.Value.BundleRewards[key].ToString();

					// Remove custom bundle data from GW data to avoid failed save loading under various circumstances
					Game1.netWorldState.Value.Bundles.Remove(key);
					Game1.netWorldState.Value.BundleRewards.Remove(key);
					Game1.netWorldState.Value.BundleData.Remove($"{CommunityCentreAreaName}/{key}");

					Log.D($"CACB Saved and unloaded bundle progress for {key}",
						Config.DebugMode);
				}
				cc.modData[AssetPrefix + "area_completed"] = IsCommunityCentreKitchenCompleted().ToString();
			}
			else
			{
				Log.D($"CACB Did not save and unload GW bundle data: Peer was not host player.",
					Config.DebugMode);
			}

			if (cc.areasComplete.Count > CommunityCentreAreaNumber)
			{
				// Remove local community centre data
				try
				{
					// Recalibrate area-bundle reference dictionaries
					Helper.Reflection.GetMethod(cc, "initAreaBundleConversions").Invoke();

					// Remove new areasComplete entry
					var oldAreas = cc.areasComplete;
					var newAreas = new NetArray<bool, NetBool>(CommunityCentreAreaNumber);
					for (var i = 0; i < newAreas.Count; ++i)
						newAreas[i] = oldAreas[i];
					cc.areasComplete.Clear();
					cc.areasComplete.Set(newAreas);

					Log.D($"CACB Unloaded CC data.",
						Config.DebugMode);
				}
				catch (Exception e)
				{
					Log.E($"CACB Error while updating CC areasComplete[{cc.areasComplete.Count()}] NetArray:"
						+ $"\nMultiplayer: {Game1.IsMultiplayer}-{IsMultiplayer()}"
						+ $", MasterGame: {Game1.IsMasterGame}"
						+ $", MasterPlayer: {Game1.player.UniqueMultiplayerID == Game1.MasterPlayer.UniqueMultiplayerID}"
						+ $", FarmHands: {Game1.getAllFarmhands().Count()}"
						+ $"\n{e}");
				}
			}

			PrintBundleData(cc);
			Log.D("CACB End of unloaded vanilla bundle data.",
				Config.DebugMode);
		}

		internal void PrintBundleData(CommunityCenter cc)
		{
			// aauugh

			// Community centre data (LOCAL)
			var bad = Helper.Reflection.GetField<Dictionary<int, int>>(cc, "bundleToAreaDictionary").GetValue();
			var abd = Helper.Reflection.GetField<Dictionary<int, List<int>>>(cc, "areaToBundleDictionary").GetValue();

			Log.D($"CACB Multiplayer: ({Game1.IsMultiplayer}-{IsMultiplayer()}), Host game: ({Game1.IsMasterGame}), Host player: ({Game1.MasterPlayer.UniqueMultiplayerID == Game1.player.UniqueMultiplayerID})", Config.DebugMode);
			Log.D($"CACB CC IsCommunityCentreComplete: {IsCommunityCentreComplete()}", Config.DebugMode); 
			Log.D($"CACB CC IsKitchenEnabledByHost:  {IsCommunityCentreKitchenEnabledByHost()}", Config.DebugMode);
			Log.D(cc.areasComplete.Aggregate($"CACB CC areasComplete[{cc.areasComplete.Count}]:    ", (s, b) => $"{s} ({b})"), Config.DebugMode);
			Log.D(bad.Aggregate($"CACB CC bundleToAreaDictionary[{bad.Count}]:", (s, pair) => $"{s} ({pair.Key}: {pair.Value})"), Config.DebugMode);
			Log.D(abd.Aggregate($"CACB CC areaToBundleDictionary[{abd.Count}]:", (s, pair) => $"{s} ({pair.Key}: {pair.Value.Aggregate("", (s1, value) => s1 + " " + value)})"), Config.DebugMode);
			Log.D($"CACB CC NumOfAreasComplete:        {Helper.Reflection.GetMethod(cc, "getNumberOfAreasComplete").Invoke<int>()}", Config.DebugMode);

			// World state data (SYNCHRONISED)
			Log.D(Game1.netWorldState.Value.BundleData.Aggregate("CACB GW bundleData: ", (s, pair)
				=> $"{s}\n{pair.Key}: {pair.Value}"), Config.DebugMode);
			Log.D(Game1.netWorldState.Value.Bundles.Aggregate("CACB GW bundles: ", (s, boolses)
				=> boolses?.Count > 0 ? $"{s}\n{boolses.Aggregate("", (s1, pair) => $"{s1}\n{pair.Key}: {pair.Value.Aggregate("", (s2, complete) => $"{s2} {complete}")}")}" : "none"), Config.DebugMode);
			Log.D(Game1.netWorldState.Value.BundleRewards.Aggregate("CACB GW bundleRewards: ", (s, boolses)
				=> boolses?.Count > 0 ? $"{s}\n{boolses.Aggregate("", (s1, pair) => $"{s1} ({pair.Key}: {pair.Value})")}" : "(none)"), Config.DebugMode);
		}

		internal Dictionary<int, string> ParseBundleData()
		{
			var newData = new Dictionary<int, string>();
			var sourceBundleList = Game1.content.Load<Dictionary<string, List<List<string>>>>(GameContentBundleDataPath);
			var sourceBundle = sourceBundleList[(JsonAssets != null && Config.AddNewCropsAndStuff) ? "Custom" : "Vanilla"];

			// Iterate over each custom bundle to add their data to game Bundles dictionary
			for (var i = 0; i < sourceBundle.Count; ++i)
			{
				// Bundle data
				var parsedBundle = new List<List<string>>();

				var index = BundleStartIndex + i;
				var displayName = i18n.Get($"world.community_centre.bundle.{i + 1}");
				var itemsToComplete = sourceBundle[i][2];
				var colour = sourceBundle[i][3];
				parsedBundle.Add(new List<string> { displayName.ToString() });

				// Fill in rewardsData section of the new bundle data
				var rewardsData = sourceBundle[i][0].Split(' ');
				var rewardName = SplitToString(rewardsData.Skip(1).Take(rewardsData.Length - 2), ' ');
				var rewardId = JsonAssets.GetObjectId(rewardName);
				if (rewardId < 0)
				{
					rewardId = rewardsData[0] == "BO"
						? Game1.bigCraftablesInformation.FirstOrDefault(o => o.Value.Split('/')[0] == rewardName).Key
						: Game1.objectInformation.FirstOrDefault(o => o.Value.Split('/')[0] == rewardName).Key;
				}
				parsedBundle.Add(new List<string> { rewardsData[0], rewardId.ToString(), rewardsData[rewardsData.Length - 1] });

				// Iterate over each word in the items list, formatted as [<Name With Spaces> <Quantity> <Quality>]
				parsedBundle.Add(new List<string>());
				var startIndex = 0;
				var requirementsData = sourceBundle[i][1].Split(' ');
				for (var j = 0; j < requirementsData.Length; ++j)
				{
					// Group and parse each [name quantity quality] cluster
					if (j != startIndex && int.TryParse(requirementsData[j], out var itemQuantity))
					{
						var itemName = SplitToString(requirementsData.Skip(startIndex).Take(j - startIndex).ToArray(), ' ');
						var itemQuality = int.Parse(requirementsData[++j]);
						var itemId = JsonAssets.GetObjectId(itemName);

						// Add parsed item data to the requiredItems section of the new bundle data
						if (itemId < 0)
						{
							itemId = Game1.objectInformation.FirstOrDefault(o => o.Value.Split('/')[0] == itemName).Key;
						}
						if (itemId > 0)
						{
							parsedBundle[2].AddRange(new List<int> { itemId, itemQuantity, itemQuality }.ConvertAll(o => o.ToString()));
						}

						startIndex = ++j;
					}
				}

				// Patch new data into the target bundle dictionary, including mininmum completion count and display name
				var value = SplitToString(parsedBundle.Select(list => SplitToString(list, ' ')), '/') + $"/{colour}/{itemsToComplete}";
				if (LocalizedContentManager.CurrentLanguageCode.ToString() != "en")
				{
					value += $"/{displayName}";
				}
				newData.Add(index, value);
			}
			return newData;
		}

		internal static void AddNotificationButton()
		{
			if (Game1.onScreenMenus.Contains(NotificationButton))
				return;

			Log.D("Adding NotificationButton to HUD",
				Instance.Config.DebugMode);
			Game1.onScreenMenus.Add(NotificationButton);
		}

		internal static void RemoveNotificationButton()
		{
			if (!Game1.onScreenMenus.Contains(NotificationButton))
				return;

			Log.D("Removing NotificationButton from HUD",
				Instance.Config.DebugMode);
			Game1.onScreenMenus.Remove(NotificationButton);
		}

		private static void AddNewPendingNotification(Notification notification)
		{
			// Ignore duplicate notifications
			if (PendingNotifications.Contains(notification))
				return;

			Log.D($"Sending {notification.ToString()} notification",
				Instance.Config.DebugMode);

			Game1.playSound("give_gift");
			Game1.showGlobalMessage("Love of Cooking: " + Instance.i18n.Get($"notification.{notification.ToString().ToLower()}.inspect"));
			HasUnreadNotifications = true;
			PendingNotifications.Add(notification);
			AddNotificationButton();
		}

		public StardewValley.Tools.GenericTool GenerateCookingTool(int level)
		{
			var toolName = string.Format(
						$"{Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs." + (14299 + level))}",
						i18n.Get("menu.cooking_equipment.name").ToString());
			var toolDescription = i18n.Get("menu.cooking_equipment.description", new { level = level + 2 }).ToString();
			return new StardewValley.Tools.GenericTool(toolName, toolDescription, level + 1, 17 + level, 17 + level);
		}

		public bool DoesCookingToolNameMatch(Tool tool)
		{
			var locale = LocalizedContentManager.CurrentLanguageCode.ToString();

			var toolName = i18n.Get("menu.cooking_equipment.name");
			var nameMatches = tool.Name.EndsWith(toolName) // Select locales use noun-adjective ordering
				|| (tool.Name.StartsWith(toolName) && new [] { "ru", "fr", "es", "pt" }.Contains(locale));
			return nameMatches;
		}

		public void PurgeBrokenFryingPans(bool sendMail)
		{
			Log.D("Checking for broken cooking tools.",
				Config.DebugMode);

			var name = i18n.Get("menu.cooking_equipment.name");
			var found = 0;

			for (var i = Game1.player.Items.Count - 1; i >= 0; --i)
			{
				if (Game1.player.Items[i] == null
					|| (!Game1.player.Items[i].Name.EndsWith(name) && !Game1.player.Items[i].Name.EndsWith(AssetPrefix + "tool")))
					continue;

				Log.D($"Removing a broken Cooking tool in {Game1.player.Name}'s inventory slot {i}.",
					Config.DebugMode);

				++found;
				Game1.player.removeItemFromInventory(Game1.player.Items[i]);
			}

			foreach (var location in Game1.locations)
			{
				foreach (var chest in location.Objects.Values.Where(o => o != null && o is Chest chest && chest.items.Count > 0))
				{
					for (var i = ((Chest)chest).items.Count - 1; i >= 0; --i)
					{
						if (((Chest)chest).items[i] == null
							|| (!((Chest)chest).items[i].Name.EndsWith(name) && !((Chest)chest).items[i].Name.EndsWith(AssetPrefix + "tool")))
							continue;

						Log.D($"Removing a broken Cooking tool in chest at {location.Name} {chest.TileLocation.ToString()} item slot {i}.",
							Config.DebugMode);

						++found;
						((Chest)chest).items.RemoveAt(i);
					}
				}
			}

			if (found > 0 && sendMail)
			{
				if (!Game1.player.mailReceived.Contains(MailFryingPanWhoops))
				{
					Game1.player.mailbox.Add(MailFryingPanWhoops);
				}
			}
		}
	}
}
