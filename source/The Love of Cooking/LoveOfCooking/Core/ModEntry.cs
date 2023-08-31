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

		internal static bool IsEnglishLocale => LocalizedContentManager.CurrentLanguageCode.Equals(LocalizedContentManager.LanguageCode.en);

		// Player session state
		public readonly PerScreen<State> States = new(createNewState: () => new());
		public class State
		{
			// Persistent player data
			public int CookingToolLevel = 0;
			public bool IsUsingRecipeGridView = false;
			public List<string> FoodsEaten = new();
			public List<string> FavouriteRecipes = new();

			// Add Cooking Menu
			public CookingMenu.Filter LastFilterThisSession = CookingMenu.Filter.None;
			public bool LastFilterReversed;
			public uint ItemsCooked;

			// Add Cooking Skill
			public readonly Dictionary<string, int> FoodCookedToday = new();

			// Food Heals Over Time
			public Regeneration Regeneration = new();
		}

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
		internal string ChocolateName { get { return Interface.Interfaces.UsingPPJATreesAndRecipes ? "Chocolate" : $"{ObjectPrefix}chocolate"; } }
		internal string CabbageName { get { return Interface.Interfaces.UsingPPJACrops ? "Cabbage" : $"{ObjectPrefix}cabbage"; } }
		internal string OnionName { get { return Interface.Interfaces.UsingPPJACrops ? "Onion" : $"{ObjectPrefix}onion"; } }
		internal string CarrotName { get { return Interface.Interfaces.UsingPPJACrops ? "Carrot" : $"{ObjectPrefix}carrot"; } }
		internal string NettleName { get { return Interface.Interfaces.UsingNettlesCrops ? "Nettles" : $"{ObjectPrefix}nettles"; } }
		internal string NettleTeaName { get { return Interface.Interfaces.UsingNettlesCrops ? "Nettle Tea" : $"{ObjectPrefix}nettletea"; } }
		// cook at kitchens
		internal static Dictionary<string, string> NpcHomeLocations = null;
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
		internal static readonly string MailFryingPanWhoops = MailPrefix + "im_sorry_lol_pan"; // Legacy

		// Mod features
		internal static float DebugGlobalExperienceRate = 1f;
		internal const bool PFMEnabled = false;
		internal const bool HideBuffIconsOnItems = false;
		internal static bool PrintRename => false;


		public override void Entry(IModHelper helper)
		{
			ModEntry.Instance = this;
			ModEntry.Config = helper.ReadConfig<Config>();
			ModEntry.NexusId = int.Parse(this.ModManifest.UpdateKeys
				.First(s => s.StartsWith("nexus", StringComparison.InvariantCultureIgnoreCase))
				.Split(':')
				.Last());
			this.PrintConfig();
			this.Helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
		}

		public override object GetApi()
		{
			return new CookingSkillAPI(reflection: this.Helper.Reflection);
		}

		private bool Init()
		{
			// Interfaces
			try
			{
				if (!Interface.Interfaces.Init())
				{
					Log.E("Failed to load mod-provided APIs.");
					return false;
				}
			}
			catch (Exception e)
			{
				Log.E($"Error in loading mod-provided APIs:{Environment.NewLine}{e}");
				return false;
			}

			// Asset definitions
			try
			{
				if (!AssetManager.Init())
				{
					Log.E("Failed to start asset manager.");
					return false;
				}
			}
			catch (Exception e)
			{
				Log.E($"Error in starting asset manager:{Environment.NewLine}{e}");
				return false;
			}

			// Harmony patches
			try
			{
				HarmonyPatches.HarmonyPatches.Patch(id: this.ModManifest.UniqueID);
			}
			catch (Exception e)
			{
				Log.E($"Error in applying Harmony patches:{Environment.NewLine}{e}");
				return false;
			}

			// Game events
			this.RegisterEvents();

			return true;
		}

		private void RegisterEvents()
		{
			this.Helper.Events.Content.AssetRequested += AssetManager.OnAssetRequested;
			this.Helper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
			this.Helper.Events.GameLoop.Saving += this.GameLoop_Saving;
			this.Helper.Events.GameLoop.DayStarted += this.GameLoop_DayStarted;
			this.Helper.Events.GameLoop.DayEnding += this.GameLoop_DayEnding;
			this.Helper.Events.GameLoop.ReturnedToTitle += this.GameLoop_ReturnedToTitle;
			this.Helper.Events.GameLoop.UpdateTicked += this.GameLoop_UpdateTicked;
			this.Helper.Events.Input.ButtonPressed += this.Input_ButtonPressed;
			this.Helper.Events.Display.MenuChanged += this.Display_MenuChanged;
			this.Helper.Events.Multiplayer.PeerContextReceived += this.Multiplayer_PeerContextReceived;
			this.Helper.Events.Multiplayer.PeerConnected += this.Multiplayer_PeerConnected;

			SpaceEvents.OnItemEaten += this.SpaceEvents_ItemEaten;
			SpaceEvents.BeforeGiftGiven += this.SpaceEvents_BeforeGiftGiven;
			SpaceEvents.AddWalletItems += this.SpaceEvents_AddWalletItems;

			Events.BushShaken += this.Events_BushShaken;
			Events.BushToolUsed += this.Events_BushToolUsed;
		}

		private void AddConsoleCommands()
		{
			string cmd = ModEntry.ItemDefinitions["ConsoleCommandPrefix"][0];

			IEnumerable<string> forgetLoveOfCookingRecipes() {
				IEnumerable<string> recipes = CookingSkill.CookingSkillLevelUpRecipes.Values
					.SelectMany(s => s);
				foreach (string recipe in recipes)
				{
					Game1.player.cookingRecipes.Remove(ModEntry.ObjectPrefix + recipe);
				}
				return recipes;
			}

			string listKnownCookingRecipes()
			{
				return Game1.player.cookingRecipes.Keys
					.OrderBy(s => s)
					.Aggregate("Cooking recipes:", (cur, s) => $"{cur}{Environment.NewLine}{s}");
			}

			this.Helper.ConsoleCommands.Add(
				name: cmd + "open_cooking_menu",
				documentation: "Open the cooking menu.",
				callback: (s, args) =>
				{
					if (!Utils.PlayerAgencyLostCheck())
						Utils.OpenNewCookingMenu(forceOpen: true);
				});
			this.Helper.ConsoleCommands.Add(
				name: cmd + "set_cooking_level",
				documentation: "Set cooking level.",
				callback: (s, args) =>
				{
					if (!ModEntry.Config.AddCookingSkillAndRecipes)
					{
						Log.D("Cooking skill is not enabled.");
						return;
					}
					if (args.Length < 1)
					{
						Log.D($"Choose a level between 0 and {ModEntry.CookingSkillApi.GetMaximumLevel()}.");
						return;
					}

					// Update experience
					this.Helper.Reflection.GetField
						<Dictionary<long, Dictionary<string, int>>>
						(typeof(SpaceCore.Skills), "Exp")
						.GetValue()
						[Game1.player.UniqueMultiplayerID][CookingSkill.InternalName] = 0;

					// Reset recipes
					forgetLoveOfCookingRecipes();

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
			this.Helper.ConsoleCommands.Add(
				name: cmd + "set_tool_level",
				documentation: "Set cooking tool level.",
				callback: (s, args) =>
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
			this.Helper.ConsoleCommands.Add(
				name: cmd + "hurt_me",
				documentation: "Reduce health and stamina. Pass zero, one, or two values.",
				callback: (s, args) =>
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
			this.Helper.ConsoleCommands.Add(
				name: cmd + "forget_cooking_skill_recipes",
				documentation: "Forget all unlocked Cooking Skill recipes until the next level-up.",
				callback: (s, args) =>
				{
					string message;
					IEnumerable<string> recipes = forgetLoveOfCookingRecipes();
					message = $"Forgetting recipes added by Love of Cooking:{Environment.NewLine}" + string.Join(Environment.NewLine, recipes);

					message = listKnownCookingRecipes();
					Log.D(message);
				});
			this.Helper.ConsoleCommands.Add(
				name: cmd + "forget_invalid_recipes",
				documentation: "Forget all invalid player recipes.",
				callback: (s, args) =>
				{
					string message;
					var validRecipes = Game1.content.Load<Dictionary<string, string>>("Data/CookingRecipes");
					List<string> invalidRecipes = Game1.player.cookingRecipes.Keys
						.Where(key => !validRecipes.ContainsKey(key))
						.ToList();

					message = $"Forgetting invalid recipes:{Environment.NewLine}" + string.Join(Environment.NewLine, invalidRecipes);
					Log.D(message);

					foreach (string recipe in invalidRecipes)
					{
						Game1.player.cookingRecipes.Remove(recipe);
					}

					message = listKnownCookingRecipes();
					Log.D(message);
				});
			this.Helper.ConsoleCommands.Add(
				name: cmd + "give_cookbook",
				documentation: "Flag cookbook mail as read, allowing kitchens to be used.",
				callback: (s, args) =>
				{
					if (!Game1.player.hasOrWillReceiveMail(MailCookbookUnlocked))
					{
						Log.D("The cookbook is already in your mailbox or mail history.");
					}
					else
					{
						Game1.player.mailReceived.Add(MailCookbookUnlocked);
						Log.D($"Added cookbook-received mail to your mail-received list.");
					} 
				});
			this.Helper.ConsoleCommands.Add(
				name: cmd + "spawn_nettles",
				documentation: "Add nettle bushes to the world. Spawn limit is ignored.",
				callback: (s, args) =>
				{
					Utils.SpawnNettles(force: true);
				});
			this.Helper.ConsoleCommands.Add(
				name: cmd + "unstuck_me",
				documentation: "Unlocks player movement if stuck in animations.",
				callback: (s, args) =>
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
			this.Helper.ConsoleCommands.Add(
				name: cmd + "print_recipes",
				documentation: "Show all unlocked player recipes.",
				callback: (s, args) =>
				{
					string message = Game1.player.cookingRecipes.Keys.OrderBy(str => str)
						.Aggregate("Cooking recipes:", (cur, str) => $"{cur}{Environment.NewLine}{str}");
					Log.D(message);
				});
			this.Helper.ConsoleCommands.Add(
				name: cmd + "print_config",
				documentation: "Print config state.",
				callback: (s, args) =>
				{
					this.PrintConfig();
				});
			this.Helper.ConsoleCommands.Add(
				name: cmd + "print_data",
				documentation: "Print save data state.",
				callback: (s, args) =>
				{
					this.PrintModData();
				});
			this.Helper.ConsoleCommands.Add(
				name: cmd + "print_skill",
				documentation: "Print skill state.",
				callback: (s, args) =>
				{
					this.PrintCookingSkill();
				});
			this.Helper.ConsoleCommands.Add(
				name: cmd + "print",
				documentation: "Print all mod info.",
				callback: (s, args) =>
				{
					this.PrintConfig();
					this.PrintModData();
					this.PrintCookingSkill();
				});
			this.Helper.ConsoleCommands.Add(
				name: cmd + "bush",
				documentation: "Add a bush or seed.",
				callback: (s, args) =>
				{
					if (args.Length < 1)
						return;

					switch (args[0])
					{
						case "b":
							Game1.currentLocation.largeTerrainFeatures.Add(
								new Bush(Game1.GetPlacementGrabTile(), 1, Game1.currentLocation));
							return;
						case "n":
							Game1.currentLocation.largeTerrainFeatures.Add(
								new CustomBush(Game1.GetPlacementGrabTile(), Game1.currentLocation, BushNameNettle));
							return;
						case "r":
							Game1.currentLocation.largeTerrainFeatures.Add(
								new CustomBush(Game1.GetPlacementGrabTile(), Game1.currentLocation, BushNameRedberry));
							return;
					}
				});
		}

		private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
		{
			// Load assets after mods and asset editors have been registered to allow for patches, correct load orders
			Helper.Events.GameLoop.OneSecondUpdateTicked += this.Event_LoadLate;
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

			// God damn it
			Utils.CleanUpSaveFiles();

			// Food Heals Over Time
			States.Value.Regeneration.UpdateDefinitions();

			// Perform behaviours for adding CustomBush instances to the world
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
			// Reset session state
			States.Value = new State();
		}

		private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			AssetManager.IsCurrentHoveredItemHidingBuffs = false;
		}

		private void Event_LoadLate(object sender, OneSecondUpdateTickedEventArgs e)
		{
			this.Helper.Events.GameLoop.OneSecondUpdateTicked -= this.Event_LoadLate;
			bool isLoaded = false;
			try
			{
				if (!this.Init())
				{
					Log.E($"{this.ModManifest.Name} couldn't be initialised.");
				}
				else
				{
					// Assets and definitions
					this.ReloadAssets();

					// Console commands
					this.AddConsoleCommands();

					// APIs and custom content
					Interface.Interfaces.Load();

					// Cooking Skill API
					ModEntry.CookingSkillApi = new CookingSkillAPI(this.Helper.Reflection);
					if (ModEntry.Config.AddCookingSkillAndRecipes)
					{
						SpaceCore.Skills.RegisterSkill(new CookingSkill());
					}

					isLoaded = true;
				}
			}
			catch (Exception ex)
			{
				Log.E(ex.ToString());
			}
			if (!isLoaded)
			{
				Log.E($"{this.ModManifest.Name} failed to load completely. Mod may not be usable.");
			}
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
			if (!Context.IsWorldReady || Game1.currentLocation is null)
				return;

			// Draw cooking animation sprites
			Game1.currentLocation.getTemporarySpriteByID(CookingMenu.SpriteId)?.draw(
				e.SpriteBatch,
				localPosition: false,
				xOffset: 0,
				yOffset: 0,
				extraAlpha: 1f);
		}
		
		private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			if (!Context.IsWorldReady || Game1.currentLocation is null)
				return;

			// World interactions
			if (Utils.PlayerAgencyLostCheck())
				return;

			// World interactions:
			if (Game1.currentBillboard != 0 || Game1.activeClickableMenu is not null || Game1.menuUp // No menus
			    || !Game1.player.CanMove) // Player agency enabled
				return;

			if (e.Button.IsActionButton())
			{
				// Tile actions
				bool shouldOpenCookingMenu = false;
				xTile.Tiles.Tile tile = Game1.currentLocation.Map.GetLayer("Buildings").Tiles[(int)e.Cursor.GrabTile.X, (int)e.Cursor.GrabTile.Y];
				string action = Game1.currentLocation.doesTileHaveProperty((int)e.Cursor.GrabTile.X, (int)e.Cursor.GrabTile.Y, "Action", "Buildings");
				if (tile is not null)
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
								if (Game1.player.team.specialOrders.Any(order => order is not null && order.objectives.Any(
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
			}
		}

		[EventPriority(EventPriority.Low)]
		private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
		{
			if (e.OldMenu is TitleMenu || e.NewMenu is TitleMenu || !Context.IsWorldReady || Game1.currentLocation is null || Game1.player is null)
				return;

			// Unique after-mail-read behaviours
			if (e.OldMenu is LetterViewerMenu letterClosed && letterClosed.isMail && e.NewMenu is null)
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

				return;
			}

			// Add new recipes on level-up for Cooking skill
			if (e.NewMenu is SpaceCore.Interface.SkillLevelUpMenu levelUpMenu1)
			{
				Utils.AddAndDisplayNewRecipesOnLevelUp(levelUpMenu1);
				return;
			}

			// Add new objects to shop menus and edit shop stock
			if (e.NewMenu is ShopMenu menu and not null && Interface.Interfaces.JsonAssets is not null)
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

			// Open the new Cooking Menu as a substitute when a cooking CraftingPage is opened
			if (Config.AddCookingMenu && e.NewMenu is not Objects.CookingMenu && Utils.IsCookingMenu(e.NewMenu))
			{
				Utils.ReplaceCraftingMenu(lastMenu: e.NewMenu);
				
				return;
			}
		}

		private void Multiplayer_PeerContextReceived(object sender, PeerContextReceivedEventArgs e)
		{
			if (!Context.IsMainPlayer)
				return;
			Log.D($"Peer context received: {e.Peer.PlayerID} : SMAPI:{e.Peer.HasSmapi}" +
				$" CAC:{(e.Peer.Mods?.ToList().FirstOrDefault(mod => mod.ID == Helper.ModRegistry.ModID) is IMultiplayerPeerMod mod && mod is not null ? mod.Version.ToString() : "null")}",
				Config.DebugMode);
		}

		private void Multiplayer_PeerConnected(object sender, PeerConnectedEventArgs e)
		{
			if (!Context.IsMainPlayer)
				return;
			Log.D($"Peer connected to multiplayer session: {e.Peer.PlayerID} : SMAPI:{e.Peer.HasSmapi}" +
				$" CAC:{(e.Peer.Mods?.ToList().FirstOrDefault(mod => mod.ID == Helper.ModRegistry.ModID) is IMultiplayerPeerMod mod && mod is not null ? mod.Version.ToString() : "null")}",
				Config.DebugMode);
		}

		private void SpaceEvents_ItemEaten(object sender, EventArgs e)
		{
			if (Game1.player.itemToEat is not StardewValley.Object food
				// Don't consider Life Elixir (ID 773) for food behaviours or Food Heals Over Time
				|| Game1.player.itemToEat.ParentSheetIndex == 773)
				return;

			string[] foodData = Game1.objectInformation[food.ParentSheetIndex].Split('/');
			bool isDrink = foodData.Length > 6 && foodData[6] == "drink";

			Log.D($"Ate food: {food?.Name ?? "null"}"
				+ $"{Environment.NewLine}Buffs: (food: {Game1.buffsDisplay.food?.displaySource ?? "null"})"
				+ $" (drink: {Game1.buffsDisplay.drink?.displaySource ?? "null"})",
				Config.DebugMode);

			if (food.Name == ObjectPrefix + "cookbook")
			{
				// Whoops
				// Yes, it's come up before
				Game1.addMail(MailCookbookUnlocked);
				Game1.addHUDMessage(new HUDMessage($"You ate the cookbook, gaining its knowledge.{Environment.NewLine}How did this happen??"));
			}

			// Determine food healing
			if (Config.FoodHealingTakesTime)
			{
				States.Value.Regeneration.Eat(food: food);
			}
			else if (CookingSkillApi.HasProfession(ICookingSkillAPI.Profession.Restoration))
			{
				// Add additional health
				Game1.player.health = (int) Math.Min(Game1.player.maxHealth,
					Game1.player.health + food.healthRecoveredOnConsumption() * (CookingSkill.RestorationAltValue / 100f));
				Game1.player.Stamina = (int) Math.Min(Game1.player.MaxStamina,
					Game1.player.Stamina + food.staminaRecoveredOnConsumption() * (CookingSkill.RestorationAltValue / 100f));
			}

			Buff foodBuff = isDrink
				? Game1.buffsDisplay.drink
				: Game1.buffsDisplay.food;
			Log.D($"OnItemEaten"
				+ $" | Ate food:  {food.Name}"
				+ $" | Last buff: {foodBuff?.displaySource ?? "null"} (source: {foodBuff?.source ?? "null"})",
				Config.DebugMode);

			// Check to boost buff duration
			if (CookingSkillApi.HasProfession(ICookingSkillAPI.Profession.BuffDuration)
			    && food.displayName == foodBuff?.displaySource)
			{
				int duration = foodBuff.millisecondsDuration;
				if (duration > 0)
				{
					float rate = (Game1.player.health + Game1.player.Stamina) / (Game1.player.maxHealth + Game1.player.MaxStamina);
					duration += (int) Math.Floor(CookingSkill.BuffDurationValue * 1000 * rate);
					foodBuff.millisecondsDuration = duration;
				}
			}

			// Track foods eaten
			if (!States.Value.FoodsEaten.Contains(food.Name))
			{
				States.Value.FoodsEaten.Add(food.Name);
			}

			// Add leftovers from viable foods to the inventory, or drop it on the ground if full
			if (ItemDefinitions["FoodsThatGiveLeftovers"].Contains(food.Name)
				&& Config.AddRecipeRebalancing && Interface.Interfaces.JsonAssets is not null)
			{
				string leftoversName = food.Name.StartsWith(ObjectPrefix)
					? $"{food.Name}_half"
					: $"{food.Name.ToLower().Split(' ').Aggregate(ObjectPrefix, (s, s1) => s + s1)}" + "_half";
				StardewValley.Object leftovers = new StardewValley.Object(
					Interface.Interfaces.JsonAssets.GetObjectId(leftoversName),
					1);
				Utils.AddOrDropItem(leftovers);
			}

			// Handle unique kebab effects
			if (food.Name.StartsWith(ModEntry.ObjectPrefix) && food.name.EndsWith("kebab"))
			{
				double kebabRoll = Game1.random.NextDouble();
				int kebabDuration = -1;
				string kebabMessage = "";
				string kebabSource = "";
				int[] kebabStats = null;
				if (kebabRoll < 0.08f)
				{
					// Remove any health/energy restoration from bad kebabs
					if (Config.FoodHealingTakesTime)
					{
						States.Value.Regeneration.Add(
							hp: -food.healthRecoveredOnConsumption(),
							ep: -food.staminaRecoveredOnConsumption());
					}
					else
					{
						States.Value.Regeneration.RevertPlayer();
					}

					if (kebabRoll > 0.04f)
					{
						kebabMessage = i18n.Get("item.kebab.bad");
						// Add no debuffs
					}
					else
					{
						kebabMessage = i18n.Get("item.kebab.worst");
						kebabSource = i18n.Get("buff.kebab.inspect",
							new { quality = i18n.Get("buff.kebab.quality_worst") });
						kebabDuration = KebabMalusDuration;
						if (kebabRoll < 0.02f)
						{
							// Add a debuff for a random non-combat stat
							int[] kebabNonCombatStats = new[] { 0, 0, 0, 0 };
							kebabNonCombatStats[Game1.random.Next(kebabStats.Length - 1)] = KebabNonCombatBonus * -1;
							kebabStats = new int[]
							{
								kebabNonCombatStats[0], kebabNonCombatStats[1], kebabNonCombatStats[2], 0, 0, kebabNonCombatStats[3],
								0, 0, 0, 0, 0, 0
							};
						}
						else
						{
							// Add a debuff for combat stats
							kebabStats = new int[]
							{
								0, 0, 0, 0, 0, 0,
								0, 0, 0, 0,
								KebabCombatBonus * -1, KebabCombatBonus * -1
							};
						}
					}
				}
				else if (kebabRoll < 0.18f)
				{
					// Add extra health/energy restoration for great kebabs
					if (Config.FoodHealingTakesTime)
					{
						States.Value.Regeneration.Add(hp: Game1.player.maxHealth / 10, ep: Game1.player.MaxStamina / 10);
					}
					else
					{
						Game1.player.health = Math.Min(Game1.player.maxHealth,
							Game1.player.health + Game1.player.maxHealth / 10);
						Game1.player.Stamina = Math.Min(Game1.player.MaxStamina,
							Game1.player.Stamina + Game1.player.MaxStamina / 10f);
					}

					kebabSource = i18n.Get("buff.kebab.inspect",
						new { quality = i18n.Get("buff.kebab.quality_best") });
					kebabMessage = i18n.Get("item.kebab.best");
					kebabDuration = KebabBonusDuration;
					// Add a buff for both non-combat and combat stats
					kebabStats = new int[]
					{
						0, 0, KebabNonCombatBonus, 0, 0, 0,
						0, 0, 0, 0,
						KebabCombatBonus, KebabCombatBonus
					};
				}
				if (!string.IsNullOrEmpty(kebabMessage))
				{
					Game1.addHUDMessage(new HUDMessage(message: kebabMessage, leaveMeNull: null));
				}
				if (kebabStats is not null)
				{
					Buff kebabBuff = new Buff(
						farming: kebabStats[0], fishing: kebabStats[1], mining: kebabStats[2], digging: kebabStats[3],
						luck: kebabStats[4], foraging: kebabStats[5], crafting: kebabStats[6], maxStamina: kebabStats[7],
						magneticRadius: kebabStats[8], speed: kebabStats[9], defense: kebabStats[10], attack: kebabStats[11],
						minutesDuration: kebabDuration, source: food.Name, displaySource: kebabSource);
					Game1.buffsDisplay.tryToAddFoodBuff(kebabBuff, kebabDuration);
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

		/// <summary>
		/// Add our custom wallet items to the SpaceCore wallet UI.
		/// Invoked when instantiating <see cref="SpaceCore.Interface.NewSkillsPage"/>.
		/// </summary>
		/// <param name="sender">New <see cref="SpaceCore.Interface.NewSkillsPage"/>.</param>
		/// <param name="e">No arguments expected.</param>
		private void SpaceEvents_AddWalletItems(object sender, EventArgs e)
		{
			SpaceCore.Interface.NewSkillsPage menu = sender as SpaceCore.Interface.NewSkillsPage;

			if (Game1.player.hasOrWillReceiveMail(ModEntry.MailCookbookUnlocked))
			{
				// Cookbook
				StardewValley.Object o = new StardewValley.Object(
					parentSheetIndex: Interface.Interfaces.JsonAssets.GetObjectId(name: ModEntry.ObjectPrefix + "cookbook"),
					initialStack: 1);
				Rectangle sourceRect = GameLocation.getSourceRectForObject(tileIndex: o.ParentSheetIndex);
				menu.specialItems.Add(new ClickableTextureComponent(
					name: string.Empty,
					bounds: new Rectangle(-1, -1, sourceRect.Width * Game1.pixelZoom, sourceRect.Height * Game1.pixelZoom),
					label: null,
					hoverText: o.DisplayName,
					texture: Game1.objectSpriteSheet,
					sourceRect: sourceRect,
					scale: Game1.pixelZoom,
					drawShadow: true));

				// Frying Pan
				if (ModEntry.Config.AddCookingToolProgression)
				{
					int cookingToolLevel = this.States.Value.CookingToolLevel;
					sourceRect = Objects.CookingTool.CookingToolSourceRectangle(upgradeLevel: cookingToolLevel);
					menu.specialItems.Add(new ClickableTextureComponent(
						name: string.Empty,
						bounds: new Rectangle(-1, -1, sourceRect.Width * Game1.pixelZoom, sourceRect.Height * Game1.pixelZoom),
						label: null,
						hoverText: Objects.CookingTool.CookingToolQualityDisplayName(upgradeLevel: cookingToolLevel),
						texture: ModEntry.SpriteSheet,
						sourceRect: sourceRect,
						scale: Game1.pixelZoom,
						drawShadow: true));
				}
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
			this.Helper.GameContent.InvalidateCacheAndLocalized(@"Data/ObjectInformation");
			this.Helper.GameContent.InvalidateCacheAndLocalized(@"Data/CookingRecipes");
			
			// Populate NPC home locations for cooking range usage
			var npcData = Game1.content.Load
				<Dictionary<string, string>>
				("Data/NPCDispositions");
			NpcHomeLocations = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> npc in npcData)
			{
				NpcHomeLocations.Add(npc.Key, npc.Value.Split('/')[10].Split(' ')[0]);
			}

			// Food Heals Over Time
			States.Value.Regeneration.RegisterEvents(helper: Helper);

			// Cooking Animations
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
			ModEntry.ItemDefinitions = Game1.content.Load
				<Dictionary<string, List<string>>>
				(AssetManager.GameContentDefinitionsPath);
			ModEntry.IngredientBuffChart = Game1.content.Load
				<Dictionary<string, string>>
				(AssetManager.GameContentIngredientBuffDataPath);
			ModEntry.SpriteSheet = Game1.content.Load
				<Texture2D>
				(AssetManager.GameContentSpriteSheetPath);
			CustomBush.Reload();

			// Invalidate other known assets that we edit using our own
			this.Helper.GameContent.InvalidateCacheAndLocalized(@"LooseSprites/Cursors");
		}

		private void PrintConfig()
		{
			try
			{
				Log.D($"{Environment.NewLine}== CONFIG SUMMARY =={Environment.NewLine}"
					  + $"{Environment.NewLine}New Cooking Menu:   {Config.AddCookingMenu}"
					  + $"{Environment.NewLine}New Cooking Skill:  {Config.AddCookingSkillAndRecipes}"
					  + $"{Environment.NewLine}New Cooking Tool:   {Config.AddCookingToolProgression}"
					  + $"{Environment.NewLine}New Crops & Stuff:  {Config.AddNewCropsAndStuff}"
					  + $"{Environment.NewLine}New Recipe Scaling: {Config.AddRecipeRebalancing}"
					  + $"{Environment.NewLine}New Buff Assigning: {Config.AddBuffReassigning}"
					  + $"{Environment.NewLine}Cooking Animation:  {Config.PlayCookingAnimation}"
					  + $"{Environment.NewLine}Healing Takes Time: {Config.FoodHealingTakesTime}"
					  + $"{Environment.NewLine}Hide Food Buffs:    {Config.HideFoodBuffsUntilEaten}"
					  + $"{Environment.NewLine}Food Can Burn:      {Config.FoodCanBurn}"
					  + $"{Environment.NewLine}-------------"
					  + $"{Environment.NewLine}ShowFoodRegenBar:         {Config.ShowFoodRegenBar}"
					  + $"{Environment.NewLine}RememberLastSearchFilter: {Config.RememberLastSearchFilter}"
					  + $"{Environment.NewLine}DefaultSearchFilter:      {Config.DefaultSearchFilter}"
					  + $"{Environment.NewLine}-------------"
					  + $"{Environment.NewLine}Debugging:      {Config.DebugMode}"
					  + $"{Environment.NewLine}Resize Korean:  {Config.ResizeKoreanFonts}{Environment.NewLine}",
					Config.DebugMode);
			}
			catch (Exception e)
			{
				Log.E($"Error in printing mod configuration.{Environment.NewLine}{e}");
			}
		}

		private void PrintModData()
		{
			try
			{
				Log.D($"{Environment.NewLine}== LOCAL DATA =={Environment.NewLine}"
					+ $"{Environment.NewLine}RecipeGridView:   {States.Value.IsUsingRecipeGridView}"
					+ $"{Environment.NewLine}CookingToolLevel: {States.Value.CookingToolLevel}"
					+ $"{Environment.NewLine}FoodsEaten:       {string.Join(" ", States.Value.FoodsEaten.Select(s => $"({s})"))}"
					+ $"{Environment.NewLine}FavouriteRecipes: {string.Join(" ", States.Value.FavouriteRecipes.Select(s => $"({s})"))}"
					+ $"{Environment.NewLine}CookbookUnlocked: {Game1.player.hasOrWillReceiveMail(MailCookbookUnlocked)}"
					+ $"{Environment.NewLine}Language:         {LocalizedContentManager.CurrentLanguageCode.ToString().ToUpper()}{Environment.NewLine}",
					ModEntry.Config.DebugMode);
			}
			catch (Exception e)
			{
				Log.E($"Error in printing mod save data.{Environment.NewLine}{e}");
			}
		}

		private void PrintCookingSkill()
		{
			if (!ModEntry.Config.AddCookingSkillAndRecipes)
			{
				Log.D("Cooking skill is disabled in mod config.",
					ModEntry.Config.DebugMode);
			}
			else if (ModEntry.CookingSkillApi.GetSkill() is null)
			{
				Log.D("Cooking skill is enabled, but skill is not loaded.",
					ModEntry.Config.DebugMode);
			}
			else
			{
				try
				{
					int level = ModEntry.CookingSkillApi.GetLevel();
					int current = ModEntry.CookingSkillApi.GetTotalCurrentExperience();
					int total = ModEntry.CookingSkillApi.GetTotalExperienceRequiredForLevel(level + 1);
					int remaining = ModEntry.CookingSkillApi.GetExperienceRemainingUntilLevel(level + 1);
					int required = ModEntry.CookingSkillApi.GetExperienceRequiredForLevel(level + 1);
					string professions = string.Join(Environment.NewLine,
						ModEntry.CookingSkillApi.GetCurrentProfessions().Select(pair => $"{pair.Key}: {pair.Value}"));
					Log.D($"{Environment.NewLine}== COOKING SKILL =={Environment.NewLine}"
						+ $"{Environment.NewLine}ID: {CookingSkillApi.GetSkill().GetName()}"
						+ $"{Environment.NewLine}Cooking level: {level}"
						+ $"{Environment.NewLine}Experience until next level: ({required - remaining}/{required})"
						+ $"{Environment.NewLine}Total experience: ({current}/{total})"
						+ $"{Environment.NewLine}Current professions: {professions}{Environment.NewLine}",
						ModEntry.Config.DebugMode);
				}
				catch (Exception e)
				{
					Log.E($"Error in printing custom skill data.{Environment.NewLine}{e}");
				}
			}
		}
	}
}
