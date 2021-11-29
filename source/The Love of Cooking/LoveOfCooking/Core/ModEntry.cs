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
using Microsoft.Xna.Framework.Graphics;
using SpaceCore.Events;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

// TODO: UPDATE: Ingredients bounce when added to cooking slots, puff when removed, unless using autofill
// TODO: UPDATE: Quests, events, and scripts
// TODO: UPDATE: Hot chocolate at the ice festival

// TODO: COMPATIBILITY: Skill Prestige (https://www.nexusmods.com/stardewvalley/mods/569)
// TODO: COMPATIBILITY: Level Extender (https://www.nexusmods.com/stardewvalley/mods/1471)
//		No current errors or issues, but doesn't interact, either
// TODO: COMPATIBILITY: Tool Upgrade Delivery (https://www.nexusmods.com/stardewvalley/mods/5421)


namespace LoveOfCooking
{
	public class ModEntry : Mod
	{
		public static ModEntry Instance;
		public static Config Config;
		public static Texture2D SpriteSheet;
		public static ICookingSkillAPI CookingSkillApi;

		internal ITranslationHelper i18n => Helper.Translation;

		internal const string AssetPrefix = "blueberry.LoveOfCooking."; // DO NOT EDIT
		internal const string ObjectPrefix = "blueberry.cac."; // DO NOT EDIT
		internal const string MailPrefix = "blueberry.cac.mail."; // DO NOT EDIT
		internal static int NexusId { get; private set; }

		// Player session state
		public readonly PerScreen<State> States = new PerScreen<State>(createNewState: () => new State());
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
			public int HealthAndStaminaRegenRunningValue;
			public uint RegenTicksCurr;
			public readonly Queue<uint> RegenTicksDiff = new Queue<uint>();
			public StardewValley.Object LastFoodEaten;
			public bool LastFoodWasDrink;
			public bool LastTickShowedHealthBar;

			// debug
			public float RegenTickRate;
		}

		// Add Cooking Questline
		internal const string ActionDockCrate = AssetPrefix + "DockCrate";

		// Object definitions
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
		internal string ChocolateName { get { return Interface.Interfaces.UsingPPJACrops ? "Chocolate" : $"{ObjectPrefix}chocolate"; } }
		internal string CabbageName { get { return Interface.Interfaces.UsingPPJACrops ? "Cabbage" : $"{ObjectPrefix}cabbage"; } }
		internal string OnionName { get { return Interface.Interfaces.UsingPPJACrops ? "Onion" : $"{ObjectPrefix}onion"; } }
		internal string CarrotName { get { return Interface.Interfaces.UsingPPJACrops ? "Carrot" : $"{ObjectPrefix}carrot"; } }
		internal string NettleName { get { return Interface.Interfaces.UsingNettlesCrops ? "Nettles" : $"{ObjectPrefix}nettles"; } }
		internal string NettleTeaName { get { return Interface.Interfaces.UsingNettlesCrops ? "Nettle Tea" : $"{ObjectPrefix}nettletea"; } }
		// cook at kitchens
		internal static Dictionary<string, string> NpcHomeLocations;
		internal static readonly List<int> IndoorsTileIndexesThatActAsCookingStations = new List<int>
		{
			498, 499, 631, 632, 633
		};
		internal static readonly List<int> IndoorsTileIndexesOfFridges = new List<int>{ 173, 258, 500, 634 };
		// kebab
		private const string KebabBuffSource = AssetPrefix + "kebab";
		private const int KebabBonusDuration = 220;
		private const int KebabMalusDuration = 140;
		private const int KebabCombatBonus = 3;
		private const int KebabNonCombatBonus = 2;
		// bushes
		internal const string BushNameNettle = "Nettle";
		internal const string BushNameRedberry = "Redberry";

		// Mail titles
		internal static readonly string MailCookbookUnlocked = MailPrefix + "cookbook_unlocked"; // DO NOT EDIT
		internal static readonly string MailFryingPanWhoops = MailPrefix + "im_sorry_lol_pan"; // DO NOT EDIT

		// Mod features
		internal static float DebugGlobalExperienceRate = 1f;
		internal const bool NettlesEnabled = true;
		internal const bool RedberriesEnabled = false;
		internal const bool PFMEnabled = false;
		internal const bool HideBuffIconsOnItems = false;
		internal static bool PrintRename => false;


		public override void Entry(IModHelper helper)
		{
			Instance = this;
			Config = helper.ReadConfig<Config>();
			ModEntry.NexusId = int.Parse(ModManifest.UpdateKeys
				.First(s => s.StartsWith("nexus", StringComparison.InvariantCultureIgnoreCase))
				.Split(':')
				.Last());
			this.PrintConfig();
			AssetManager.Init();
			Interface.Interfaces.Init(helper: Helper, manifest: ModManifest);

			try
			{
				// Apply harmony patches
				Core.HarmonyPatches.HarmonyPatches.Patch();
			}
			catch (Exception e)
			{
				Log.E($"Error in applying Harmony patches:{Environment.NewLine}{e}");
			}

			// Asset editors
			AssetManager assetManager = new AssetManager();
			Helper.Content.AssetLoaders.Add(assetManager);
			Helper.Content.AssetEditors.Add(assetManager);

			// Game events
			Helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
			Helper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
			Helper.Events.GameLoop.Saving += this.GameLoop_Saving;
			Helper.Events.GameLoop.DayStarted += this.GameLoop_DayStarted;
			Helper.Events.GameLoop.DayEnding += this.GameLoop_DayEnding;
			Helper.Events.GameLoop.ReturnedToTitle += this.GameLoop_ReturnedToTitle;
			Helper.Events.GameLoop.UpdateTicked += this.GameLoop_UpdateTicked;
			Helper.Events.Input.ButtonPressed += this.Input_ButtonPressed;
			Helper.Events.Display.MenuChanged += this.Display_MenuChanged;
			Helper.Events.Display.Rendered += this.Display_Rendered;
			Helper.Events.Multiplayer.PeerContextReceived += this.Multiplayer_PeerContextReceived;
			Helper.Events.Multiplayer.PeerConnected += this.Multiplayer_PeerConnected;

			SpaceEvents.OnItemEaten += this.SpaceEvents_ItemEaten;
			SpaceEvents.BeforeGiftGiven += this.SpaceEvents_BeforeGiftGiven;

			Events.BushShaken += this.Events_BushShaken;
			Events.BushToolUsed += this.Events_BushToolUsed;

			Interface.Interfaces.RegisterEvents();
			Tools.RegisterEvents();

			this.AddConsoleCommands();
		}

		private void AddConsoleCommands()
		{
			string cmd = Config.ConsoleCommandPrefix;

			Tools.AddConsoleCommands(cmd);

			Helper.ConsoleCommands.Add(cmd + "menu", "Open cooking menu.", (s, args) =>
			{
				if (!Utils.PlayerAgencyLostCheck())
					Utils.OpenNewCookingMenu();
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
					var dictField = Helper.Reflection.GetField
						<Dictionary<long, Dictionary<string, int>>>
						(typeof(SpaceCore.Skills), "Exp");
					var dict = dictField.GetValue();
					if (args[1].ToLower() == "r")
					{
						Log.D("Resetting level.");
						dict[Game1.player.UniqueMultiplayerID][CookingSkill.InternalName] = 0;
					}
					dictField.SetValue(dict);
				}

				// Reset recipes
				if (args.Length > 1)
				{
					List<string> recipes = CookingSkill.CookingSkillLevelUpRecipes.Values
						.Aggregate(new List<string>(), (list, cur) => list.Concat(cur).ToList());
					foreach (string recipe in recipes)
					{
						if (Game1.player.cookingRecipes.Keys.Contains(ObjectPrefix + recipe))
						{
							Game1.player.cookingRecipes.Remove(ObjectPrefix + recipe);
						}
					}
				}

				// Add to current level
				int level = CookingSkillApi.GetLevel();
				int target = Math.Min(10, level + int.Parse(args[0]));
				CookingSkillApi.AddExperienceDirectly(
					CookingSkillApi.GetTotalExperienceRequiredForLevel(target)
					- CookingSkillApi.GetTotalCurrentExperience());

				// Update professions
				foreach (SpaceCore.Skills.Skill.Profession profession in CookingSkillApi.GetSkill().Professions)
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
				Helper.Reflection.GetMethod(typeof(CookingSkill), "showLevelMenu")
					.Invoke(null, new SpaceCore.Events.EventArgsShowNightEndMenus());
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
				string message = Game1.player.cookingRecipes.Keys.OrderBy(str => str)
					.Aggregate("Cooking recipes:", (cur, str) => $"{cur}\n{str}");
				Log.D(message);
			});
			Helper.ConsoleCommands.Add(cmd + "unlearn", "Remove all unlocked player recipes.", (s, args) =>
			{
				List<string> recipes = CookingSkill.CookingSkillLevelUpRecipes.Values
					.Aggregate(new List<string>(), (list, cur) => list.Concat(cur).ToList());
				foreach (string recipe in recipes)
				{
					if (Game1.player.cookingRecipes.Keys.Contains(ObjectPrefix + recipe))
					{
						Game1.player.cookingRecipes.Remove(ObjectPrefix + recipe);
						Log.D($"Removed {recipe}.");
					}
				}

				string message = Game1.player.cookingRecipes.Keys.OrderBy(str => str)
					.Aggregate("Cooking recipes:", (cur, str) => $"{cur}\n{str}");
				Log.D(message);
			});
			Helper.ConsoleCommands.Add(cmd + "purgerecipes", "Remove all invalid player recipes.", (s, args) =>
			{
				var validRecipes = Game1.content.Load<Dictionary<string, string>>("Data/CookingRecipes");
				List<string> invalidRecipes = Game1.player.cookingRecipes.Keys
					.Where(key => !validRecipes.ContainsKey(key)).ToList();
				foreach (string recipe in invalidRecipes)
				{
					Game1.player.cookingRecipes.Remove(recipe);
					Log.D($"Removed {recipe}.");
				}

				string message = Game1.player.cookingRecipes.Keys.OrderBy(str => str).Aggregate("Cooking recipes:", (cur, str) => $"{cur}\n{str}");
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
			Helper.ConsoleCommands.Add(cmd + "nettles", "Add nettle bushes to the world. Spawn limit is ignored.", (s, p) =>
			{
				Utils.SpawnNettles(force: true);
			});
			Helper.ConsoleCommands.Add(cmd + "findnettles", "Find all nettle bushes in the world.", (s, p) =>
			{
				CustomBush.FindBushesGlobally(variety: BushNameNettle, remove: false);
			});
			Helper.ConsoleCommands.Add(cmd + "removenettles", "Remove all nettle bushes from the world.", (s, p) =>
			{
				CustomBush.FindBushesGlobally(variety: BushNameNettle, remove: true);
			});
			Helper.ConsoleCommands.Add(cmd + "unstuck", "Unlocks player movement if stuck in animations.", (s, args) =>
			{
				if (Game1.activeClickableMenu is CookingMenu)
				{
					Game1.activeClickableMenu.emergencyShutDown();
				}
				Game1.player.Halt();
				Game1.player.completelyStopAnimatingOrDoingAction();
				Game1.player.faceDirection(2);
				Game1.player.Position = Game1.tileSize * Utility.recursiveFindOpenTileForCharacter(
					c: Game1.player,
					l: Game1.currentLocation,
					tileLocation: Game1.player.getTileLocation(),
					maxIterations: 10);
				Game1.freezeControls = false;
			});
			Helper.ConsoleCommands.Add(cmd + "printconfig", "Print config state.", (s, args) =>
			{
				this.PrintConfig();
			});
			Helper.ConsoleCommands.Add(cmd + "printsave", "Print save data state.", (s, args) =>
			{
				this.PrintModData();
			});
			Helper.ConsoleCommands.Add(cmd + "printskill", "Print skill state.", (s, args) =>
			{
				this.PrintCookingSkill();
			});
		}

		private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
		{
			// Load assets after mods and asset editors have been registered to allow for patches, correct load orders
			Helper.Events.GameLoop.OneSecondUpdateTicked += this.Event_LoadAssetsLate;
		}

		private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			this.SaveLoadedBehaviours();
		}

		private void GameLoop_Saving(object sender, SavingEventArgs e)
		{
			// Save persistent player data to player
			Game1.player.modData[AssetPrefix + "grid_view"] = States.Value.IsUsingRecipeGridView.ToString();
			Game1.player.modData[AssetPrefix + "tool_level"] = States.Value.CookingToolLevel.ToString();
			Game1.player.modData[AssetPrefix + "foods_eaten"] = string.Join(",", States.Value.FoodsEaten);
			Game1.player.modData[AssetPrefix + "favourite_recipes"] = string.Join(",", States.Value.FavouriteRecipes);
		}

		private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
		{
			// Perform OnSaveLoaded behaviours when starting a new game
			bool isNewGame = Game1.dayOfMonth == 1 && Game1.currentSeason == "spring" && Game1.year == 1;
			if (isNewGame)
			{
				this.SaveLoadedBehaviours();
			}

			Utils.PopulateMissingRecipes();
			Utils.CalculateFoodRegenModifiers();
			if (Utils.AreNettlesActive())
			{
				Utils.TrySpawnNettles();
				CustomBush.FindBushesGlobally(variety: BushNameNettle, remove: false);
			}

			// Add the cookbook for the player once they've reached the unlock date
			// Internally day and month are zero-indexed, but are one-indexed in data file for consistency with year
			// Alternatively if the player somehow upgrades their house early, add the cookbook mail
			if (Config.AddCookingMenu && !Game1.player.hasOrWillReceiveMail(MailCookbookUnlocked))
			{
				int day = int.Parse(ItemDefinitions["CookbookMailDate"][0]) - 1;
				int month = int.Parse(ItemDefinitions["CookbookMailDate"][1]) - 1;
				int year = int.Parse(ItemDefinitions["CookbookMailDate"][2]);
				int gameMonth = Utility.getSeasonNumber(Game1.currentSeason);
				bool reachedNextYear = (Game1.year > year);
				bool reachedNextMonth = (Game1.year == year && gameMonth > month);
				bool reachedMailDate = (Game1.year == year && gameMonth == month && Game1.dayOfMonth >= day);
				bool unlockedFarmhouseKitchen = Game1.player.HouseUpgradeLevel > 0;
				if (reachedNextYear || reachedNextMonth || reachedMailDate || unlockedFarmhouseKitchen)
				{
					Game1.addMail(MailCookbookUnlocked);
				}
			}
		}

		private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
		{
			// Clear existing nettles at the start of each season
			if (Utils.AreNettlesActive()
				&& (Game1.dayOfMonth == 28
					|| Helper.ModRegistry.IsLoaded("Entoarox.EntoaroxFramework")))
			{
				CustomBush.FindBushesGlobally(variety: $"{ModManifest.UniqueID}.Nettles", remove: true);
			}
		}

		private void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
		{
			// Remove contextual event handlers
			Helper.Events.Input.ButtonPressed -= this.Event_TryDropInItem;
			Helper.Events.Player.InventoryChanged -= this.Event_CheckForDroppedInItem;
			Helper.Events.Display.RenderingHud -= this.Event_DrawRegenBar;

			// Reset session state
			States.Value = new State();
		}

		private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			AssetManager.IsCurrentHoveredItemHidingBuffs = false;

			if (Config.FoodHealingTakesTime)
			{
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
				{
					States.Value.HealthAndStaminaRegenRunningValue = 0;
					return;
				}

				if (Game1.player.health < 1)
				{
					States.Value.HealthRegeneration = 0;
					States.Value.StaminaRegeneration = 0;
					States.Value.HealthOnLastTick = 1;
					States.Value.StaminaOnLastTick = 1;
					return;
				}

				// Fetch all components for the rate of HP/EP regeneration
				const float healthToStaminaRatio = 3f;
				int cookingLevel = CookingSkillApi.GetLevel();
				float panicMultiplier = ((Game1.player.health * healthToStaminaRatio) + Game1.player.Stamina)
					/ ((Game1.player.maxHealth * healthToStaminaRatio) + Game1.player.MaxStamina);
				float foodMultiplier = Utils.GetFoodRegenRate(States.Value.LastFoodEaten);
				int baseRate = int.Parse(ItemDefinitions["RegenBaseRate"][0]);
				float overallScale = float.Parse(ItemDefinitions["RegenSpeedScale"][0]);

				// Calculate regeneration
				double rate = (baseRate - baseRate * States.Value.RegenSkillModifier) * foodMultiplier * 100d;
				rate = Math.Floor(Math.Max(36 - cookingLevel * 1.75f, rate * panicMultiplier) / overallScale);

				States.Value.RegenTickRate = (float)rate;
				++States.Value.RegenTicksCurr;

				// Regenerate player HP/EP when 
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
		}

		private void Event_LoadAssetsLate(object sender, OneSecondUpdateTickedEventArgs e)
		{
			Helper.Events.GameLoop.OneSecondUpdateTicked -= this.Event_LoadAssetsLate;

			this.ReloadAssets();

			CookingSkillApi = new CookingSkillAPI(Helper.Reflection);
			if (Config.AddCookingSkillAndRecipes)
			{
				SpaceCore.Skills.RegisterSkill(new CookingSkill());
			}

			Interface.Interfaces.LoadInterfaces();
		}

		private void Event_AfterSaveLoaded(object sender, OneSecondUpdateTickedEventArgs e)
		{
			// Reference sprite colours for menu elements
			// Reason for being late: Waits for all patches to be applied to the SpriteSheet asset
			Rectangle sourceArea = CookingMenu.CookbookSource;
			Color[] pixels = new Color[sourceArea.Width * sourceArea.Height];
			ModEntry.SpriteSheet.GetData(
				level: 0,
				rect: CookingMenu.CookbookSource,
				data: pixels,
				startIndex: 0,
				elementCount: pixels.Length);

			for (int y = 0; y < sourceArea.Height; ++y)
			{
				int which = (y * sourceArea.Width) + (sourceArea.Width / 4);
				if (pixels[which].A < 255)
					continue;
				CookingMenu.DividerColour = pixels[which];
				break;
			}
		}

		[EventPriority(EventPriority.Low)]
		private void Event_DrawCookingAnimation(object sender, RenderedWorldEventArgs e)
		{
			if (!Context.IsWorldReady || Game1.currentLocation == null)
				return;

			// Draw cooking animation sprites
			Game1.currentLocation.getTemporarySpriteByID(CookingMenu.SpriteId)?.draw(
				e.SpriteBatch,
				localPosition: false,
				xOffset: 0,
				yOffset: 0,
				extraAlpha: 1f);
		}

		private void Event_TryDropInItem(object sender, ButtonPressedEventArgs e)
		{
			if (!Context.IsWorldReady || Game1.currentLocation == null)
				return;

			if (e.Button.IsUseToolButton())
			{
				if (Utils.AreNettlesActive()
					&& Game1.currentLocation.Objects.TryGetValue(e.Cursor.GrabTile, out StardewValley.Object o)
					&& o != null
					&& o.Name == "Keg"
					&& o.heldObject?.Value == null
					&& Game1.player.ActiveObject != null
					&& Game1.player.ActiveObject.Name.EndsWith("nettles", StringComparison.InvariantCultureIgnoreCase))
				{
					if (CookingSkillApi.IsEnabled()
						&& CookingSkillApi.GetLevel() < int.Parse(ItemDefinitions["NettlesUsableLevel"][0]))
					{
						// Ignore Nettles used on Kegs to make Nettle Tea when Cooking skill level is too low
						Game1.playSound("cancel");
					}
					else
					{
						// Since kegs don't accept forage items, we trigger the dropIn behaviours through our inventory changed handler 
						if (--Game1.player.ActiveObject.Stack < 1)
							Game1.player.ActiveObject = null;
						Helper.Input.Suppress(e.Button);
					}
				}
			}
		}

		private void Event_CheckForDroppedInItem(object sender, InventoryChangedEventArgs e)
		{
			if (!Context.IsWorldReady || Game1.currentLocation == null)
				return;

			// Handle unique craftable input/output
			if (Game1.activeClickableMenu == null
				&& Interface.Interfaces.JsonAssets != null)
			{
				Vector2 tilePosition = Game1.currentCursorTile;
				if (Game1.currentLocation.Objects.ContainsKey(tilePosition)
					&& Game1.currentLocation.Objects[tilePosition] is StardewValley.Object craftable
					&& craftable != null
					&& craftable.bigCraftable.Value
					&& Game1.player.mostRecentlyGrabbedItem != null)
				{
					if (Utils.AreNettlesActive()
						&& craftable.Name == "Keg"
						&& Game1.player.mostRecentlyGrabbedItem.Name.EndsWith("nettles", StringComparison.InvariantCultureIgnoreCase)
						&& craftable.heldObject.Value == null // Keg must be empty
						&& e.QuantityChanged.FirstOrDefault(x => x.Item == Game1.player.mostRecentlyGrabbedItem) is ItemStackSizeChange i
						&& i.NewSize < i.OldSize)
					{
						string name = NettleTeaName;
						craftable.heldObject.Value = new StardewValley.Object(
							Vector2.Zero,
							Interface.Interfaces.JsonAssets.GetObjectId(name),
							Givenname: name,
							canBeSetDown: false,
							canBeGrabbed: true,
							isHoedirt: false,
							isSpawnedObject: false);
						craftable.MinutesUntilReady = 180;

						// Since kegs don't accept forage items, we perform the dropIn behaviours ourselves
						Game1.currentLocation.playSound("Ship");
						Game1.currentLocation.playSound("bubbles");
						Multiplayer multiplayer = Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
						multiplayer.broadcastSprites(
							Game1.currentLocation,
							new TemporaryAnimatedSprite(
								textureName: "TileSheets\\animations",
								sourceRect: new Rectangle(256, 1856, 64, 128),
								animationInterval: 80f,
								animationLength: 6,
								numberOfLoops: 999999,
								position: (craftable.TileLocation * Game1.tileSize) + new Vector2(0f, -2 * Game1.tileSize),
								flicker: false,
								flipped: false,
								layerDepth: ((craftable.TileLocation.Y + 1f) * Game1.tileSize / 10000f) + (1 / 10000f),
								alphaFade: 0f,
								color: Color.Lime * 0.75f,
								scale: 1f, scaleChange: 0f, rotation: 0f, rotationChange: 0f)
							{
								alphaFade = 0.005f
							});
					}
					else if (Game1.player.mostRecentlyGrabbedItem.Name.EndsWith("Apple", StringComparison.InvariantCulture)
						&& craftable.heldObject.Value != null // Keg will not be empty, but we override base behaviour
						&& !(craftable.heldObject.Value.Name.EndsWith("cider", StringComparison.InvariantCultureIgnoreCase)))
					{
						var name = ObjectPrefix + "cider";
						craftable.heldObject.Value = new StardewValley.Object(
							Vector2.Zero,
							Interface.Interfaces.JsonAssets.GetObjectId(name),
							Givenname: name,
							canBeSetDown: false,
							canBeGrabbed: true,
							isHoedirt: false,
							isSpawnedObject: false);
						craftable.MinutesUntilReady = 1900;
					}
				}
			}
		}

		[EventPriority(EventPriority.Low)]
		private void Event_DrawRegenBar(object sender, RenderingHudEventArgs e)
		{
			Rectangle viewport = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();

			float currentRegen = States.Value.HealthRegeneration + States.Value.StaminaRegeneration;
			if (Context.IsWorldReady && !Game1.eventUp && Game1.farmEvent == null && currentRegen > 0)
			{
				const int otherBarWidth = 12 * Game1.pixelZoom;
				const int margin = 3 * Game1.pixelZoom;
				const int spacing = 2 * Game1.pixelZoom;
				const int borderWidth = 3 * Game1.pixelZoom;
				int otherBarCount = States.Value.LastTickShowedHealthBar ? 2 : 1;
				int xOffset = ((otherBarWidth) * (1 + otherBarCount)) + (spacing * otherBarCount);

				// Draw background
				int backgroundWidth = AssetManager.RegenBarArea.Width * Game1.pixelZoom;
				int backgroundHeight = AssetManager.RegenBarArea.Height * Game1.pixelZoom;
				Vector2 topOfBar = new Vector2(
					viewport.Right - xOffset,
					viewport.Bottom - margin - backgroundHeight);
				if (Game1.isOutdoorMapSmallerThanViewport())
				{
					topOfBar.X = Math.Min(topOfBar.X, -Game1.viewport.X + Game1.currentLocation.Map.Layers[0].LayerWidth * 64 - xOffset);
				}

				e.SpriteBatch.Draw(
					texture: ModEntry.SpriteSheet,
					position: topOfBar,
					sourceRectangle: AssetManager.RegenBarArea,
					color: Color.White,
					rotation: 0f,
					origin: Vector2.Zero,
					scale: Game1.pixelZoom,
					effects: SpriteEffects.None,
					layerDepth: 1f);

				// Draw fill colour
				Color colour = Utility.getRedToGreenLerpColor(0.5f);
				float startingRegen = States.Value.HealthAndStaminaRegenRunningValue;
				int barHeight = backgroundHeight - 10 - 56;
				int height = (int)(currentRegen / startingRegen * barHeight);
				Rectangle barArea = new Rectangle(
					(int)topOfBar.X + borderWidth,
					(int)topOfBar.Y + backgroundHeight - height - borderWidth,
					backgroundWidth - (borderWidth * 2),
					height);
				// fill colour body
				e.SpriteBatch.Draw(
					texture: Game1.staminaRect,
					destinationRectangle: barArea,
					sourceRectangle: Game1.staminaRect.Bounds,
					color: colour,
					rotation: 0f,
					origin: Vector2.Zero,
					effects: SpriteEffects.None,
					layerDepth: 1f);
				// fill colour top border
				barArea.Height = 4;
				colour.R = (byte)Math.Max(0, colour.R - 50);
				colour.G = (byte)Math.Max(0, colour.G - 50);
				e.SpriteBatch.Draw(
					texture: Game1.staminaRect,
					destinationRectangle: barArea,
					sourceRectangle: Game1.staminaRect.Bounds,
					color: colour,
					rotation: 0f,
					origin: Vector2.Zero,
					effects: SpriteEffects.None,
					layerDepth: 1f);

				// Draw value
				if (Game1.getOldMouseX() >= topOfBar.X
					&& Game1.getOldMouseY() >= topOfBar.Y
					&& Game1.getOldMouseX() < topOfBar.X + 32f)
				{
					SpriteFont font = Game1.smallFont;
					string text = $"H +{Math.Max(0, States.Value.HealthRegeneration)}\nE +{Math.Max(0, States.Value.StaminaRegeneration)}";
					Vector2 position = topOfBar + new Vector2(0f - font.MeasureString(text).X - spacing, 0f);
					e.SpriteBatch.DrawString(
						font,
						text,
						position,
						Color.White);
				}
			}

			if (!Config.DebugMode)
				return;

			// Draw debug info if enabled
			{
				Vector2 position = new Vector2(
					viewport.Right - 125,
					Math.Max(viewport.Top + 420, viewport.Bottom - 224 - 48 - (int)((Game1.player.MaxStamina - 270) * 0.625f)));
				string[] debugFields = new string[]
				{
				$"CUR  {States.Value.RegenTicksCurr}",
				$"RATE {States.Value.RegenTickRate}",
				$"HP+   {States.Value.HealthRegeneration}",
				$"EP+   {States.Value.StaminaRegeneration}"
				};
				for (int i = 0; i < debugFields.Length; ++i)
				{
					e.SpriteBatch.DrawString(
						Game1.smallFont,
						debugFields[i],
						position,
						Color.White);
					position.Y -= Game1.smallFont.MeasureString(debugFields[i]).Y - 8;
				}
				for (int i = 0; i < States.Value.RegenTicksDiff.Count; ++i)
				{
					e.SpriteBatch.DrawString(
						Game1.smallFont,
						$"{(i == 0 ? "DIFF" : "      ")}   {States.Value.RegenTicksDiff.ToArray()[States.Value.RegenTicksDiff.Count - 1 - i]}",
						new Vector2(position.X, position.Y - i * 24),
						Color.White * ((States.Value.RegenTicksDiff.Count - 1 - i + 1f) / (States.Value.RegenTicksDiff.Count / 2f)));
				}
			}
		}

		private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			if (!Context.IsWorldReady || Game1.currentLocation == null)
				return;

			// World interactions
			if (Utils.PlayerAgencyLostCheck())
				return;

			// Debug hotkeys
			if (Config.DebugMode)
			{
				switch (e.Button)
				{
					case SButton.F3:
						Game1.player.warpFarmer(Game1.currentLocation is CommunityCenter
							? new Warp(0, 0, "FarmHouse", 0, 0, false)
							: new Warp(0, 0, "CommunityCenter", 7, 7, false));
						return;
					case SButton.F4:
						if (Game1.activeClickableMenu is CookingMenu cookingMenu)
							cookingMenu.exitThisMenu();
						else
							Utils.OpenNewCookingMenu();
						return;
					case SButton.F5:
						Game1.currentLocation.largeTerrainFeatures.Add(
							new Bush(e.Cursor.GrabTile, 1, Game1.currentLocation));
						return;
					case SButton.F6:
						Game1.currentLocation.largeTerrainFeatures.Add(
							new CustomBush(e.Cursor.GrabTile, Game1.currentLocation, BushNameNettle));
						return;
					case SButton.F7:
						Game1.currentLocation.largeTerrainFeatures.Add(
							new CustomBush(e.Cursor.GrabTile, Game1.currentLocation, BushNameRedberry));
						return;
					case SButton.F8:
						Log.D(CookingSkillApi.GetCurrentProfessions().Aggregate("Current professions:", (str, pair) 
							=> $"{str}{Environment.NewLine}{pair.Key}: {pair.Value}"));
						return;
				}
			}

			// World interactions:
			if (Game1.currentBillboard != 0 || Game1.activeClickableMenu != null || Game1.menuUp // No menus
			    || !Game1.player.CanMove) // Player agency enabled
				return;

			if (e.Button.IsActionButton())
			{
				// Tile actions
				bool shouldOpenCookingMenu = false;
				xTile.Tiles.Tile tile = Game1.currentLocation.Map.GetLayer("Buildings").Tiles[(int)e.Cursor.GrabTile.X, (int)e.Cursor.GrabTile.Y];
				string action = Game1.currentLocation.doesTileHaveProperty((int)e.Cursor.GrabTile.X, (int)e.Cursor.GrabTile.Y, "Action", "Buildings");
				if (tile != null)
				{
					bool isCookingStationTile = IndoorsTileIndexesThatActAsCookingStations.Contains(tile.TileIndex);
					bool isFridgeTile = IndoorsTileIndexesOfFridges.Contains(tile.TileIndex);
					if (!Game1.currentLocation.IsOutdoors && isCookingStationTile)
					{
						// Try to open a new cooking menu when in NPC homes
						string npc = NpcHomeLocations.FirstOrDefault(pair => pair.Value == Game1.currentLocation.Name).Key;
						if (!string.IsNullOrEmpty(npc))
						{
							if (Game1.player.getFriendshipHeartLevelForNPC(npc) >= int.Parse(ItemDefinitions["NpcKitchenFriendshipRequired"][0]))
							{
								if (Game1.player.team.specialOrders.Any(order => order != null && order.objectives.Any(
									obj => obj is DonateObjective dobj && dobj.dropBox.Value.EndsWith("Kitchen"))))
								{
									// Avoid blocking the player from submitting items to special order dropboxes
									return;
								}
								shouldOpenCookingMenu = true;
							}
							else
							{
								string name = NpcHomeLocations.FirstOrDefault(pair => pair.Value == Game1.currentLocation.Name).Key;
								Game1.showRedMessage(i18n.Get("world.range_npc.rejected",
									new { name = Game1.getCharacterFromName(name).displayName }));
							}
						}
						else if (Game1.currentLocation is CommunityCenter)
						{
							shouldOpenCookingMenu = true;
						}
					}
				}

				if (shouldOpenCookingMenu && Utils.CanUseKitchens())
				{
					Utils.OpenNewCookingMenu();
					Helper.Input.Suppress(e.Button);
					return;
				}

				// Use tile actions in maps
				Utils.CheckTileAction(e.Cursor.GrabTile, Game1.currentLocation);
			}
		}

		private void Display_Rendered(object sender, RenderedEventArgs e)
		{
			States.Value.LastTickShowedHealthBar = Game1.showingHealthBar;

			if (!Context.IsWorldReady || Game1.currentLocation == null)
				return;

			// Render the correct English display name in crafting pages over the top of the incorrect display name.
			// Default game draw logic uses internal name for recipes in English locales, ignoring display name altogether.
			// 
			// Also applies to NewCraftingPage instances, found in spacechase0.CookingSkill:
			// https://github.com/spacechase0/CookingSkill/blob/master/NewCraftingPage.cs
			if (Game1.activeClickableMenu == null)
			{
				return;
			}
			bool isEnglish = LocalizedContentManager.CurrentLanguageCode.ToString() == "en";
			bool isCraftingMenu = Game1.activeClickableMenu is CraftingPage || Game1.activeClickableMenu.GetType().Name == "NewCraftingPage";
			bool isCookingMenu = false;
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

			IClickableMenu craftingMenu = Game1.activeClickableMenu;
			Item heldItem = Helper.Reflection.GetField<Item>(craftingMenu, "heldItem").GetValue();
			Item lastCookingHover = Helper.Reflection.GetField<Item>(craftingMenu, "lastCookingHover").GetValue();
			CraftingRecipe hoverRecipe = Helper.Reflection.GetField<CraftingRecipe>(craftingMenu, "hoverRecipe").GetValue();
			if (lastCookingHover != null && hoverRecipe != null && hoverRecipe.name.StartsWith(ObjectPrefix))
			{
				string displayName = lastCookingHover.DisplayName
					+ ((hoverRecipe.numberProducedPerCraft > 1)
						? (" x" + hoverRecipe.numberProducedPerCraft)
						: "");
				Vector2 originalWidthHeight = Game1.dialogueFont.MeasureString(hoverRecipe.name);
				int coverupWidth = (int)originalWidthHeight.X + 4;
				int x = Game1.getOldMouseX() + 44 + (heldItem != null ? 48 : 0);

				// Check to show advanced crafting info
				Vector2 displayNameWidthHeight = Game1.dialogueFont.MeasureString(displayName);
				int craftableCount = hoverRecipe.getCraftableCount(Helper.Reflection.GetMethod(craftingMenu, "getContainerContents").Invoke<IList<Item>>());
				string craftableCountText = " (" + craftableCount + ")";
				Vector2 craftableCountTextWidthHeight = Game1.smallFont.MeasureString(craftableCountText);
				int areaWidth = (int)Math.Max(420f, displayNameWidthHeight.X + craftableCountTextWidthHeight.X + 12f);
				if (Game1.options.showAdvancedCraftingInformation && craftableCount > 0)
				{
					coverupWidth += (int)craftableCountTextWidthHeight.X;
				}

				// Calculate height of the full tooltip to determine Y position
				int healAmountToDisplay = -1;
				string[] buffIconsToDisplay = Game1.objectInformation[(lastCookingHover as StardewValley.Object).ParentSheetIndex].Split('/').Length > 7
					? Game1.objectInformation[(lastCookingHover as StardewValley.Object).ParentSheetIndex].Split('/')[7].Split(' ')
					: null;
				int addedHeight = Math.Max(20 * 3,
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
				int descriptionHeight = hoverRecipe.getDescriptionHeight(areaWidth) + ((healAmountToDisplay == -1) ? (-32) : 0);
				addedHeight += descriptionHeight - 4;
				// Healing
				if (lastCookingHover is StardewValley.Object o && o.Edibility != -300)
				{
					addedHeight = (healAmountToDisplay == -1)
						? (addedHeight + 40)
						: (addedHeight + 40 * ((healAmountToDisplay <= 0) ? 1 : 2));
				}

				// Keep the stupid junk within screen bounds
				int y = Game1.getOldMouseY() + 50 + (heldItem != null ? 48 : 0);
				int coverupHeight = (int)originalWidthHeight.Y + 1;
				int squeezeWidth = (int)Math.Max(384f - 8f, coverupWidth);
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

		[EventPriority(EventPriority.Low)]
		private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
		{
			if (e.OldMenu is TitleMenu || e.NewMenu is TitleMenu || !Context.IsWorldReady || Game1.currentLocation == null || Game1.player == null)
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
					Game1.player.mostRecentlyGrabbedItem = new StardewValley.Object(
						Interface.Interfaces.JsonAssets.GetObjectId(ObjectPrefix + "cookbook"), 0);
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
				Utils.AddAndDisplayNewRecipesOnLevelUp(levelUpMenu1);
				return;
			}

			// Add new objects to shop menus and edit shop stock
			if (e.NewMenu is ShopMenu menu && menu != null && Interface.Interfaces.JsonAssets != null)
			{
				int discount = int.Parse(ModEntry.ItemDefinitions["ShopDiscounts"]
					.Select(s => s.Split(':'))
					.FirstOrDefault(split => split.First() == menu.storeContext)
					?.LastOrDefault() ?? "0");
				if (menu.storeContext == "SeedShop")
				{
					// Sort Pierre's shop to bring new crops alongside base game crops
					Utils.SortSeedShopStock(ref menu);
				}
				if (Config.AddNewCropsAndStuff)
				{
					// Add chocolate to shops
					StardewValley.Object o = new StardewValley.Object(
						tileLocation: Vector2.Zero,
						parentSheetIndex: Interface.Interfaces.JsonAssets.GetObjectId(name: ChocolateName),
						initialStack: int.MaxValue);
					int price = o.Price - discount;
					if (menu.storeContext == "JojaMart")
					{
						Utils.AddToShopAtItemIndex(menu: menu, o: o, targetItemName: "Sugar", price: price);
					}
					if (menu.storeContext == "Saloon" && Game1.MasterPlayer.hasCompletedCommunityCenter())
					{
						Utils.AddToShopAtItemIndex(menu: menu, o: o, targetItemName: "Coffee", price: price);
					}
				}

				return;
			}

			if (e.NewMenu != null
				&& (e.NewMenu is CraftingPage || nameof(e.NewMenu).EndsWith("CraftingPage", StringComparison.InvariantCultureIgnoreCase))
				&& Helper.Reflection.GetField<bool>(e.NewMenu, "cooking") is IReflectedField<bool> field
				&& field != null && field.GetValue())
			{
				// Open the new Cooking Menu as a substitute when a cooking CraftingPage is opened
				if (Config.AddCookingMenu)
				{
                    Utils.ReplaceCraftingMenu(lastMenu: e.NewMenu);
				}

				return;
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
				// Don't consider Life Elixir (ID 773) for food behaviours or healing over time
				|| Game1.player.itemToEat.ParentSheetIndex == 773)
				return;

			string[] objectData = Game1.objectInformation[food.ParentSheetIndex].Split('/');
			States.Value.LastFoodWasDrink = objectData.Length > 6 && objectData[6] == "drink";
			States.Value.LastFoodEaten = food;

			Log.D($"Ate food: {food.Name}\nBuffs: (food) {Game1.buffsDisplay.food?.displaySource}"
				+ $" (drink) {Game1.buffsDisplay.drink?.displaySource}",
				Config.DebugMode);

			if (food.Name == ObjectPrefix + "cookbook")
			{
				// Whoops
				// Yes, it's come up before
				Game1.addMail(MailCookbookUnlocked);
				Game1.addHUDMessage(new HUDMessage("You ate the cookbook, gaining its knowledge.\nHow did this happen??"));
			}

			// Determine food healing
			int foodHealth = food.healthRecoveredOnConsumption();
			int foodStamina = food.staminaRecoveredOnConsumption();
			if (Config.FoodHealingTakesTime)
			{
				// Regenerate health/energy over time
				Game1.player.health = States.Value.HealthOnLastTick;
				Game1.player.Stamina = States.Value.StaminaOnLastTick;
				States.Value.HealthAndStaminaRegenRunningValue += foodHealth + foodStamina;
				States.Value.HealthRegeneration += foodHealth;
				States.Value.StaminaRegeneration += foodStamina;
			}
			else if (CookingSkillApi.HasProfession(ICookingSkillAPI.Profession.Restoration))
			{
				// Add additional health
				Game1.player.health = (int) Math.Min(Game1.player.maxHealth,
					Game1.player.health + food.healthRecoveredOnConsumption() * (CookingSkill.RestorationAltValue / 100f));
				Game1.player.Stamina = (int) Math.Min(Game1.player.MaxStamina,
					Game1.player.Stamina + food.staminaRecoveredOnConsumption() * (CookingSkill.RestorationAltValue / 100f));
			}

			Buff lastBuff = States.Value.LastFoodWasDrink
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
				int duration = lastBuff.millisecondsDuration;
				if (duration > 0)
				{
					float rate = (Game1.player.health + Game1.player.Stamina) / (Game1.player.maxHealth + Game1.player.MaxStamina);
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
			if (ItemDefinitions["FoodsThatGiveLeftovers"].Contains(food.Name)
				&& Config.AddRecipeRebalancing && Interface.Interfaces.JsonAssets != null)
			{
				string name = food.Name.StartsWith(ObjectPrefix)
					? $"{food.Name}_half"
					: $"{food.Name.ToLower().Split(' ').Aggregate(ObjectPrefix, (s, s1) => s + s1)}" + "_half";
				StardewValley.Object leftovers = new StardewValley.Object(
					Interface.Interfaces.JsonAssets.GetObjectId(name),
					1);
				Utils.AddOrDropItem(leftovers);
			}

			// Handle unique kebab effects
			if (food.Name.StartsWith(ModEntry.ObjectPrefix) && food.name.EndsWith("kebab"))
			{
				double roll = Game1.random.NextDouble();
				int duration = -1;
				string message = "";
				string displaySource = "";
				int[] stats = null;
				if (roll < 0.08f)
				{
					// Remove any health/energy restoration from bad kebabs
					if (Config.FoodHealingTakesTime)
					{
						States.Value.HealthRegeneration -= foodHealth;
						States.Value.StaminaRegeneration -= foodStamina;
					}
					else
					{
						Game1.player.health = States.Value.HealthOnLastTick;;
						Game1.player.Stamina = States.Value.StaminaOnLastTick;
					}

					if (roll > 0.04f)
					{
						message = i18n.Get("item.kebab.bad");
						// Add no debuffs
					}
					else
					{
						message = i18n.Get("item.kebab.worst");
						displaySource = i18n.Get("buff.kebab.inspect",
							new { quality = i18n.Get("buff.kebab.quality_worst") });
						duration = KebabMalusDuration;
						if (roll < 0.02f)
						{
							// Add a debuff for a random non-combat stat
							int[] nonCombatStats = new[] { 0, 0, 0, 0 };
							nonCombatStats[Game1.random.Next(stats.Length - 1)] = KebabNonCombatBonus * -1;
							stats = new int[]
							{
								nonCombatStats[0], nonCombatStats[1], nonCombatStats[2], 0, 0, nonCombatStats[3],
								0, 0, 0, 0, 0, 0
							};
						}
						else
						{
							// Add a debuff for combat stats
							stats = new int[]
							{
								0, 0, 0, 0, 0, 0,
								0, 0, 0, 0,
								KebabCombatBonus * -1, KebabCombatBonus * -1
							};
						}
					}
				}
				else if (roll < 0.18f)
				{
					// Add extra health/energy restoration for great kebabs
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

					displaySource = i18n.Get("buff.kebab.inspect",
						new { quality = i18n.Get("buff.kebab.quality_best") });
					message = i18n.Get("item.kebab.best");
					duration = KebabBonusDuration;
					// Add a buff for both non-combat and combat stats
					stats = new int[]
					{
						0, 0, KebabNonCombatBonus, 0, 0, 0,
						0, 0, 0, 0,
						KebabCombatBonus, KebabCombatBonus
					};
				}
				if (!string.IsNullOrEmpty(message))
				{
					Game1.addHUDMessage(new HUDMessage(message: message, leaveMeNull: null));
				}
				if (stats != null)
				{
					Buff buff = new Buff(
						farming: stats[0], fishing: stats[1], mining: stats[2], digging: stats[3],
						luck: stats[4], foraging: stats[5], crafting: stats[6], maxStamina: stats[7],
						magneticRadius: stats[8], speed: stats[9], defense: stats[10], attack: stats[11],
						minutesDuration: duration, source: food.Name, displaySource: displaySource);
					Game1.buffsDisplay.tryToAddFoodBuff(buff, duration);
				}
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

		private void Events_BushToolUsed(object sender, EventArgs e)
		{
			
		}

		private void Events_BushShaken(object sender, EventArgs e)
		{
			Utils.ShakeNettles(bush: ((BushShakenEventArgs)e).Bush);
		}

		private void SaveLoadedBehaviours()
		{
			try
			{
				// Add contextual event handlers
				if (!Interface.Interfaces.UsingProducerFramework)
				{
					Helper.Events.Input.ButtonPressed += this.Event_TryDropInItem;
					Helper.Events.Player.InventoryChanged += this.Event_CheckForDroppedInItem;
				}
				if (Config.ShowFoodRegenBar)
				{
					Helper.Events.Display.RenderingHud += this.Event_DrawRegenBar;
				}

				// Load local persistent data from saved modData
				States.Value.IsUsingRecipeGridView = false;
				States.Value.CookingToolLevel = 0;
				States.Value.FoodsEaten = new List<string>();
				States.Value.FavouriteRecipes = new List<string>();
				// Grid view
				if (Game1.player.modData.TryGetValue(AssetPrefix + "grid_view", out string gridView))
					States.Value.IsUsingRecipeGridView = bool.Parse(gridView);
				else
					Log.D("No data found for IsUsingRecipeGridView", Config.DebugMode);
				// Tool level
				if (Game1.player.modData.TryGetValue(AssetPrefix + "tool_level", out string toolLevel))
					States.Value.CookingToolLevel = int.Parse(toolLevel);
				else
					Log.D("No data found for CookingToolLevel", Config.DebugMode);
				// Foods eaten
				if (Game1.player.modData.TryGetValue(AssetPrefix + "foods_eaten", out string foodsEaten))
					States.Value.FoodsEaten = foodsEaten.Split(',').ToList();
				else
					Log.D("No data found for FoodsEaten", Config.DebugMode);
				// Favourite recipes
				if (Game1.player.modData.TryGetValue(AssetPrefix + "favourite_recipes", out string favouriteRecipes))
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
				Tools.SaveLoadedBehaviours();
				Interface.Interfaces.SaveLoadedBehaviours();
			}
			catch (Exception e)
			{
				Log.E("" + e);
			}

			this.PrintModData();

			if (Config.AddCookingSkillAndRecipes)
			{
				this.PrintCookingSkill();
			}

			// Invalidate and reload our own assets
			this.ReloadAssets();

			// Invalidate and reload assets requiring JA indexes
			Log.D("Invalidating assets on save loaded.",
				Config.DebugMode);
			Helper.Content.InvalidateCache(@"Data/ObjectInformation");
			Helper.Content.InvalidateCache(@"Data/CookingRecipes");

			// Populate NPC home locations for cooking range usage
			var npcData = Game1.content.Load
				<Dictionary<string, string>>
				("Data/NPCDispositions");
			NpcHomeLocations = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> npc in npcData)
			{
				NpcHomeLocations.Add(npc.Key, npc.Value.Split('/')[10].Split(' ')[0]);
			}

			// Add or remove cooking animation draw fix per the config
			if (Config.PlayCookingAnimation)
			{
				Helper.Events.Display.RenderedWorld += this.Event_DrawCookingAnimation;
			}
			else
			{
				Helper.Events.Display.RenderedWorld -= this.Event_DrawCookingAnimation;
			}

			Helper.Events.GameLoop.OneSecondUpdateTicked += this.Event_AfterSaveLoaded;
		}

		public void ReloadAssets()
		{
			// Reload our own assets
			IngredientBuffChart = Game1.content.Load
				<Dictionary<string, string>>
				(AssetManager.GameContentIngredientBuffDataPath);
			ItemDefinitions = Game1.content.Load
				<Dictionary<string, List<string>>>
				(AssetManager.GameContentDefinitionsPath);
			SpriteSheet = Game1.content.Load
				<Texture2D>
				(AssetManager.GameContentSpriteSheetPath);
			CustomBush.Reload();

			// Invalidate other known assets that we edit using our own
			Helper.Content.InvalidateCache(@"LooseSprites/Cursors");
			Helper.Content.InvalidateCache(@"TileSheets/tools");
		}

		private void PrintConfig()
		{
			try
			{
				Log.D("\n== CONFIG SUMMARY ==\n"
					  + $"\nNew Cooking Menu:   {Config.AddCookingMenu}"
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
					  + $"\nShowFoodRegenBar:         {Config.ShowFoodRegenBar}"
					  + $"\nRememberLastSearchFilter: {Config.RememberLastSearchFilter}"
					  + $"\nDefaultSearchFilter:      {Config.DefaultSearchFilter}"
					  + $"\n-------------"
					  + $"\nDebugging:      {Config.DebugMode}"
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
					+ $"\nFarmHouseLevel:   {Utils.GetFarmhouseKitchenLevel(Game1.getLocationFromName("FarmHouse") as FarmHouse)}"
					+ $"\nMaxIngredients:   {Utils.GetFarmersMaxUsableIngredients()}"
					+ $"\nCookbookUnlockedMail: {Game1.player.mailReceived.Contains(MailCookbookUnlocked)}"
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
					int level = CookingSkillApi.GetLevel();
					int current = CookingSkillApi.GetTotalCurrentExperience();
					int total = CookingSkillApi.GetTotalExperienceRequiredForLevel(level + 1);
					int remaining = CookingSkillApi.GetExperienceRemainingUntilLevel(level + 1);
					int required = CookingSkillApi.GetExperienceRequiredForLevel(level + 1);
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
	}
}
