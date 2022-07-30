/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using SObject = StardewValley.Object;

using TraktoriShared.Utils;


namespace IndustrialFurnace
{
	// TODO: Path.Combine and NormalizeAssetName not needed?
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		private const int lightSourceIDMultiplier = 14097000;
		private const string controllerDataSaveKey = "controller-save";
		private const string furnaceBuildingType = "Industrial Furnace";
		private const string saveDataRefreshedMessage = "Save data refreshed";
		private const string requestSaveData = "Request save data";

		private readonly string blueprintsPath = PathUtilities.NormalizeAssetName("Data/Blueprints");

		// Texture strings
		// I think only this one needs to be normalized since it points to an actual file path to make it work on linux
		private readonly string defaultAssetName = PathUtilities.NormalizeAssetName($"Buildings/{furnaceBuildingType}");
		private readonly string assetOnName = PathUtilities.NormalizeAssetName("Traktori.IndustrialFurnace/FurnaceOn");

		private readonly string onPngName = "IndustrialFurnaceOn.png";
		private readonly string offPngName = "IndustrialFurnaceOff.png";

		private readonly string smokeAnimationSpriteName = PathUtilities.NormalizeAssetName("Traktori.IndustrialFurnace/SmokeSprite");
		private readonly string fireAnimationSpriteName = PathUtilities.NormalizeAssetName("Traktori.IndustrialFurnace/FireSprite");

		// Data strings
		private readonly string smeltingRulesDataName = PathUtilities.NormalizeAssetName("Traktori.IndustrialFurnace/SmeltingRules");
		private readonly string smokeAnimationDataName = PathUtilities.NormalizeAssetName("Traktori.IndustrialFurnace/SmokeAnimationData");
		private readonly string fireAnimationDataName = PathUtilities.NormalizeAssetName("Traktori.IndustrialFurnace/FireAnimationData");

		private readonly string blueprintDataPath = Path.Combine("assets", "IndustrialFurnaceBlueprint.json");
		private readonly string smeltingRulesDataPath = Path.Combine("assets", "SmeltingRules.json");
		private readonly string smokeAnimationDataPath = Path.Combine("assets", "SmokeAnimation.json");
		private readonly string fireAnimationDataPath = Path.Combine("assets", "FireAnimation.json");

		private ModConfig config = null!;
		private Data.ModSaveData? modSaveData;
		private Data.Blueprint? blueprintData;
		private Data.SmokeAnimation smokeAnimationData = null!;
		private Data.FireAnimation fireAnimationData = null!;
		private SmeltingRulesContainer newSmeltingRules = null!;
		private ITranslationHelper i18n = null!;

		// The dictionary that temporarily holds the loaded rules that get parsed into the SmeltingRulesContainer
		private Dictionary<string, string> smeltingRulesDictionary = null!;

		// Mod support
		private bool modInstantBuildingsFound = false;

		//Per-screen data
		private readonly PerScreen<int> furnacesBuilt = new PerScreen<int>( () => 0) ;      // Used to identify furnaces, placed in maxOccupants field.
		private readonly PerScreen<int> currentlyLookingAtFurnace = new PerScreen<int>( () => -1 );
		private readonly PerScreen<List<IndustrialFurnaceController>> furnaces = new PerScreen<List<IndustrialFurnaceController>>( () => new List<IndustrialFurnaceController>() );
		

		/*********
		** Public methods
		*********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			i18n = helper.Translation;

			CheckFilesAndModInstalls();

			config = helper.ReadConfig<ModConfig>();

			helper.Events.Content.AssetRequested += OnAssetRequested;
			helper.Events.Display.RenderedWorld += OnRenderedWorld;
			helper.Events.Display.MenuChanged += OnMenuChanged;
			helper.Events.GameLoop.UpdateTicking += OnUpdateTicking;
			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
			helper.Events.GameLoop.Saving += OnSaving;
			helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
			helper.Events.GameLoop.DayStarted += OnDayStarted;
			helper.Events.Input.ButtonPressed += OnButtonPressed;
			helper.Events.World.BuildingListChanged += OnBuildingListChanged;
			helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
			helper.Events.Player.Warped += OnWarped;
		}


		public override object GetApi()
		{
			return new IndustrialFurnaceAPI(this);
		}


		/// <summary>Sends a message for all connected players the updated save data.</summary>
		public void SendUpdateMessage()
		{
			// Refresh the save data for the multiplayer message and send message to all other players
			InitializeSaveData();
			Helper.Multiplayer.SendMessage(modSaveData, saveDataRefreshedMessage, new[] { ModManifest.UniqueID });
		}


		/// <summary>Checks if the building is an industrial furnace based on its buildingType</summary>
		public static bool MainIsBuildingIndustrialFurnace(Building building)
		{
			return building.buildingType.Value.Equals(furnaceBuildingType);
		}


		/// <summary>
		/// Returns the controller that matches the ID
		/// </summary>
		/// <param name="ID">The ID to look for</param>
		/// <returns>The controller or null if none were found</returns>
		public IndustrialFurnaceController? GetController(int ID)
		{
			int index = GetIndexOfFurnaceControllerWithTag(ID);

			if (index > -1)
			{
				try
				{
					return GetPerScreenFurnaceController(index);
				}
				catch (Exception)
				{
					Monitor.Log($"Trying to access invalid furnace controller with ID {ID}", LogLevel.Error);
				}
			}

			return null;
		}


		/*********
		** Private methods
		*********/
		private void CheckFilesAndModInstalls()
		{
			modInstantBuildingsFound = Helper.ModRegistry.IsLoaded("BitwiseJonMods.InstantBuildings");

			if (modInstantBuildingsFound)
			{
				Monitor.Log("Instant Buildings found.");
			}
		}

		private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
		{
			if (e.Name.IsEquivalentTo(assetOnName))
			{
				string seasonalTextureName = $"{Game1.currentSeason}_{onPngName}";

				if (File.Exists(Path.Combine(Helper.DirectoryPath, "assets", seasonalTextureName)))
				{
					Monitor.Log($"Using the texture for {Game1.currentSeason}.");
					e.LoadFromModFile<Texture2D>(Path.Combine("assets", seasonalTextureName), AssetLoadPriority.Low);
				}

				Monitor.Log($"Seasonal texture not found for season {Game1.currentSeason} state On. Using the default.");
				e.LoadFromModFile<Texture2D>(Path.Combine("assets", onPngName), AssetLoadPriority.Low);
			}
			else if (e.Name.IsEquivalentTo(defaultAssetName))
			{
				string seasonalTextureName = $"{Game1.currentSeason}_{offPngName}";

				if (File.Exists(Path.Combine(Helper.DirectoryPath, "assets", seasonalTextureName)))
				{
					Monitor.Log($"Using the texture for {Game1.currentSeason}.");
					e.LoadFromModFile<Texture2D>(Path.Combine("assets", seasonalTextureName), AssetLoadPriority.Low);
				}

				Monitor.Log($"Seasonal texture not found for season {Game1.currentSeason} state Off. Using the default.");
				e.LoadFromModFile<Texture2D>(Path.Combine("assets", offPngName), AssetLoadPriority.Low);
			}
			else if (e.Name.IsEquivalentTo(smokeAnimationSpriteName))
			{
				// Create a right sized dummy texture since its width can't be resized by CP
				e.LoadFrom(() => new Texture2D(Game1.graphics.GraphicsDevice, smokeAnimationData.SpriteSizeX, smokeAnimationData.SpriteSizeY), AssetLoadPriority.Low);
			}
			else if (e.Name.IsEquivalentTo(fireAnimationSpriteName))
			{
				// Create a right sized dummy texture since its width can't be resized by CP
				e.LoadFrom(() => new Texture2D(Game1.graphics.GraphicsDevice, fireAnimationData.SpriteSizeX, fireAnimationData.SpriteSizeY), AssetLoadPriority.Low);
			}
			else if (e.Name.IsEquivalentTo(smeltingRulesDataName))
			{
				e.LoadFrom(() => GenericHelper.LoadAssetOrDefault<Dictionary<string,string>>(smeltingRulesDataPath, Helper.Data, Monitor), AssetLoadPriority.Low);
			}
			else if (e.Name.IsEquivalentTo(smokeAnimationDataName))
			{
				e.LoadFrom(() => GenericHelper.LoadAssetOrDefault<Data.SmokeAnimation>(smokeAnimationDataPath, Helper.Data, Monitor), AssetLoadPriority.Low);
			}
			else if (e.Name.IsEquivalentTo(fireAnimationDataName))
			{
				e.LoadFrom(() => GenericHelper.LoadAssetOrDefault<Data.FireAnimation>(fireAnimationDataPath, Helper.Data, Monitor), AssetLoadPriority.Low);
			}
			else if (e.NameWithoutLocale.IsEquivalentTo(blueprintsPath))
			{
				e.Edit(asset =>
				{
					var dictionary = asset.AsDictionary<string, string>();

					// TODO: Use the name specified in the blueprint?
					blueprintData = GenericHelper.LoadAsset<Data.Blueprint>(blueprintDataPath, Helper.Data, Monitor);

					if (blueprintData is not null)
					{
						dictionary.Data[furnaceBuildingType] = blueprintData.ToBlueprintString(i18n);
					}
				});
			}
		}


		/// <summary>Raised before the game state is updated</summary>
		private void OnUpdateTicking(object? sender, UpdateTickingEventArgs e)
		{
			// This should be true if the player has another window focused or the game is still in the main menu
			if (!Game1.game1.IsActive || Game1.getFarm() is null) return;

			// Check for null since the data loading got moved to OnSaveLoaded and this gets to tick before that happens
			if (smokeAnimationData is null || fireAnimationData is null) return;

			// Don't check anything if the animations aren't enabled
			if (!config.EnableSmokeAnimation && !config.EnableFireAnimation) return;

			GameLocation gameLocation = Game1.player.currentLocation;

			if (gameLocation is not null && gameLocation is BuildableGameLocation buildableLocation)
			{
				for (int i = 0; i < furnaces.Value.Count; i++)
				{
					IndustrialFurnaceController controller = GetPerScreenFurnaceController(i);

					if (!controller.CurrentlyOn) continue;

					if (controller.furnace is null)
					{
						Monitor.Log($"Furnace for controller {controller.ID} was null", LogLevel.Error);
						continue;
					}

					if (!buildableLocation.buildings.Contains(controller.furnace))
					{
						continue;
					}

					int x = controller.furnace.tileX.Value;
					int y = controller.furnace.tileY.Value;

					if (config.EnableSmokeAnimation && e.IsMultipleOf(smokeAnimationData.SpawnFrequency * 60 / 1000))
					{
						buildableLocation.temporarySprites.Add(CreateSmokeSprite(x, y));
					}
					if (config.EnableFireAnimation && e.IsMultipleOf(fireAnimationData.SpawnFrequency * 60 / 1000))
					{
						// Spark only randomly
						if (Game1.random.NextDouble() < fireAnimationData.SpawnChance)
						{
							buildableLocation.temporarySprites.Add(CreateFireSprite(x, y));
						}

						// Puff only randomlierly
						if (Game1.random.NextDouble() < fireAnimationData.SoundEffectChance)
						{
							Game1.playSound("fireball");
						}
					}
				}
			}
		}


		private TemporaryAnimatedSprite CreateSmokeSprite(int x, int y)
		{
			TemporaryAnimatedSprite sprite;

			string textureName;
			Rectangle rectangle;

			if (smokeAnimationData.UseCustomSprite)
			{
				textureName = smokeAnimationSpriteName;
				rectangle = new Rectangle(0, 0, smokeAnimationData.SpriteSizeX, smokeAnimationData.SpriteSizeY);
			}
			else
			{
				textureName = Path.Combine("LooseSprites", "Cursors");
				rectangle = new Rectangle(372, 1956, smokeAnimationData.SpriteSizeX, smokeAnimationData.SpriteSizeY);
			}

			sprite = new TemporaryAnimatedSprite(textureName,
				rectangle,
				new Vector2(x * 64 + smokeAnimationData.SpawnXOffset, y * 64 + smokeAnimationData.SpawnYOffset),
				false,
				1f / 500f,
				Color.Gray)
			{
				alpha = 0.75f,
				motion = new Vector2(0.0f, -0.5f),
				acceleration = new Vector2(1f / 500f, 0.0f),
				interval = 99999f,
				layerDepth = 1f,
				scale = smokeAnimationData.SmokeScale,
				scaleChange = smokeAnimationData.SmokeScaleChange,
				rotationChange = (float)(Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0)
			};

			return sprite;
		}


		private TemporaryAnimatedSprite CreateFireSprite(int x, int y)
		{
			TemporaryAnimatedSprite sprite;

			double randomX = 2 * Game1.random.NextDouble() * fireAnimationData.SpawnXRandomOffset - fireAnimationData.SpawnXRandomOffset;
			double randomY = 2 * Game1.random.NextDouble() * fireAnimationData.SpawnYRandomOffset - fireAnimationData.SpawnYRandomOffset;

			Vector2 pos = new Vector2(x * 64f + fireAnimationData.SpawnXOffset + (float)randomX,
				y * 64f + fireAnimationData.SpawnYOffset + (float)randomY);

			// TODO: The fire sparks are drawn over pretty much everything, change the layer to something else
			if (fireAnimationData.UseCustomSprite)
			{
				sprite = new TemporaryAnimatedSprite(fireAnimationSpriteName,
					new Rectangle(0, 0, fireAnimationData.SpriteSizeX, fireAnimationData.SpriteSizeY),
					fireAnimationData.AnimationSpeed,
					fireAnimationData.AnimationLength,
					10,
					pos,
					false,
					false,
					(float)((y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05 + (x + 1.0) * 64.0 / 10000.0),
					0.005f,
					Color.White,
					1f,
					0f,
					0f,
					0f)
				{
					light = true,
					lightcolor = Color.Black
				};
			}
			else
			{
				sprite = new TemporaryAnimatedSprite(30,
					pos,
					Color.White,
					fireAnimationData.AnimationLength,
					false,
					fireAnimationData.AnimationSpeed,
					10,
					64,
					(float)((y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05 + (x + 1.0) * 64.0 / 10000.0),
					-1,
					0)
				{
					alphaFade = 0.005f,
					light = true,
					lightcolor = Color.Black
				};
			}

			return sprite;
		}


		/// <summary>Raised after the game is launched, right before the first update tick.</summary>
		private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
		{
			// Integration for Generic Mod Config Menu by spacechase0
			var GMCMApi = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

			if (GMCMApi is not null)
			{
				GMCMApi.Register(
					mod: ModManifest,
					reset: () => config = new ModConfig(),
					save: () => Helper.WriteConfig(config)
				);

				GMCMApi.AddSectionTitle(
					mod: ModManifest,
					text: () => i18n.Get("gmcm.main-label"),
					tooltip: null
				);

				GMCMApi.AddNumberOption(
					mod: ModManifest,
					getValue: () => config.CoalAmount,
					setValue: (int val) => config.CoalAmount = val,
					name: () => i18n.Get("gmcm.coal-amount-label"),
					tooltip: () => i18n.Get("gmcm.coal-amount-description"),
					min: 1,
					max: 100
				);

				GMCMApi.AddBoolOption(
					mod: ModManifest,
					getValue: () => config.InstantSmelting,
					setValue: (bool val) => config.InstantSmelting = val,
					name: () => i18n.Get("gmcm.instant-smelting-label"),
					tooltip: () => i18n.Get("gmcm.instant-smelting-description")
				);

				GMCMApi.AddBoolOption(
					mod: ModManifest,
					getValue: () => config.EnableSmokeAnimation,
					setValue: (bool val) => config.EnableSmokeAnimation = val,
					name: () => i18n.Get("gmcm.smoke-label"),
					tooltip: () => i18n.Get("gmcm.smoke-description")
				);

				GMCMApi.AddBoolOption(
					mod: ModManifest,
					getValue: () => config.EnableFireAnimation,
					setValue: (bool val) => config.EnableFireAnimation = val,
					name: () => i18n.Get("gmcm.fire-label"),
					tooltip: () => i18n.Get("gmcm.fire-description")
				);
			}
		}


		/// <summary>Raised after the game returns to the title screen.</summary>
		private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
		{
			// Reset stuff
			modSaveData = null;
			furnaces.Value.Clear();
		}


		/// <summary>Raised after a mod message is received over the network.</summary>
		private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
		{
			if (e.FromModID == ModManifest.UniqueID)
			{
				if (e.Type == saveDataRefreshedMessage)
				{
					// Receive the save data
					modSaveData = e.ReadAs<Data.ModSaveData>();
					// Refresh the furnace data
					InitializeFurnaceControllers(false);

					// If we have a menu open and we're looking at a furnace, the menu is most likely the output menu. Redraw it!
					if (Game1.activeClickableMenu is not null && currentlyLookingAtFurnace.Value != -1)
					{
						DrawOutputMenu(GetPerScreenFurnaceController(GetIndexOfFurnaceControllerWithTag(currentlyLookingAtFurnace.Value)));
					}

					UpdateTextures();
					UpdateFurnaceLights();
				}
				else if (e.Type == requestSaveData)
				{
					RequestSaveData request = e.ReadAs<RequestSaveData>();
					Helper.Multiplayer.SendMessage(modSaveData, saveDataRefreshedMessage, new string[] { ModManifest.UniqueID }, new long[] { request.PlayerID });
				}
			}
		}


		/// <summary>Raised before/after the game writes data to save file.</summary>
		private void OnSaving(object? sender, SavingEventArgs e)
		{
			if (Context.IsMainPlayer)
			{
				InitializeSaveData();
				Helper.Data.WriteSaveData(controllerDataSaveKey, modSaveData);
			}
		}


		/// <summary>
		/// Raised before/after the game reads data from a save file and initialises the world (including when day one starts on a new save).
		/// </summary>
		private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
		{
			smeltingRulesDictionary = Helper.GameContent.Load<Dictionary<string,string>>(smeltingRulesDataName);

			CheckSmeltingRules();

			smokeAnimationData = Helper.GameContent.Load<Data.SmokeAnimation>(smokeAnimationDataName);

			fireAnimationData = Helper.GameContent.Load<Data.FireAnimation>(fireAnimationDataName);


			// Only the person hosting the world loads the furnace controllers' state from the save
			if (Context.IsMainPlayer)
			{
				InitializeFurnaceControllers(true);
			}
		}


		/// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
		private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
		{
			// Ignore if player hasn't loaded in yet, or is stuck in a menu or cutscene
			if (!Context.IsPlayerFree)
				return;

			// Don't check if there are no furnaces
			if (furnaces.Value.Count == 0)
				return;
			
			// This might fix the android issue, also lets the player place items with both clicks
			if (e.Button.IsActionButton() || e.Button.IsUseToolButton() || e.Button == SButton.MouseLeft || e.Button == SButton.MouseRight)
			{
				if (Game1.currentLocation is null || Game1.currentLocation is not BuildableGameLocation)
					return;

				foreach (IndustrialFurnaceController furnace in furnaces.Value)
				{
					if (furnace is null)
					{
						Monitor.Log("Furnace controller was null for some reason in OnButtonPressed");
						continue;
					}

					Vector2 tile;
					Building? building = furnace.furnace;

					if (building is null)
					{
						Monitor.Log("Furnace controller's building was null for some reason in OnButtonPressed");
						continue;
					}

					if (building.daysOfConstructionLeft.Value > 0)
					{
						Monitor.Log("Trying to use furnace that hasn't been completed yet");
						continue;
					}

					// Handle the input checking differently on controllers
					if (Game1.options.gamepadControls)
					{
						tile = new Vector2((int)Game1.player.GetToolLocation().X / 64, (int)Game1.player.GetToolLocation().Y / 64);
					}
					else
					{
						tile = e.Cursor.GrabTile;

						// Allow only clicks that happen when the cursor is above the furnace to prevent trapping android users
						if (!building.occupiesTile(e.Cursor.Tile))
							continue;
					}

					// The mouth of the furnace
					if (tile.X == building.tileX.Value + 1 && tile.Y == building.tileY.Value + 1)
					{
						if (PlaceItemsToTheFurnace(furnace))
						{
							Game1.playSound("coin");

							SendUpdateMessage();
						}

						Helper.Input.Suppress(e.Button);
					}
					// The output chest of the furnace
					else if (tile.X == building.tileX.Value + 3 && tile.Y == building.tileY.Value + 1)
					{
						CollectItemsFromTheFurnace(furnace);
						Helper.Input.Suppress(e.Button);
					}
				}
			}
		}


		/// <summary>The event called when the day starts.</summary>
		private void OnDayStarted(object? sender, DayStartedEventArgs e)
		{
			if (Context.IsMainPlayer)
			{
				// Finish smelting items
				foreach (IndustrialFurnaceController furnace in furnaces.Value)
				{
					if (furnace.CurrentlyOn)
					{
						FinishSmelting(furnace);
					}
				}

				SendUpdateMessage();
			}
			else if (modSaveData is null)
			{
				Helper.Multiplayer.SendMessage(new RequestSaveData(Game1.player.UniqueMultiplayerID), requestSaveData, new string[] { ModManifest.UniqueID });
			}
		}


		/// <summary>Raised after buildings are added/removed in any location.</summary>
		private void OnBuildingListChanged(object? sender, BuildingListChangedEventArgs e)
		{
			// Add added furnaces to the controller list
			foreach (Building building in e.Added)
			{
				if (MainIsBuildingIndustrialFurnace(building))
				{
					// Add the controller that takes care of the functionality of the furnace
					IndustrialFurnaceController controller = new IndustrialFurnaceController(furnacesBuilt.Value, false, this)
					{
						furnace = building
					};

					furnaces.Value.Add(controller);
					furnacesBuilt.Value++;
				}
			}

			// Remove destroyed furnaces from the controller list
			foreach (Building building in e.Removed)
			{
				if (MainIsBuildingIndustrialFurnace(building))
				{
					int index = GetIndexOfFurnaceControllerWithTag(building.maxOccupants.Value);

					if (index > -1)
					{
						furnaces.Value.RemoveAt(index);
					}
				}
			}
		}


		/// <summary>The event called after an active menu is opened or closed.</summary>
		private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
		{
			// If a menu was closed, reset the currently looked at furnace, just in case.
			if (e.NewMenu is null)
			{
				currentlyLookingAtFurnace.Value = -1;
			}
			// Add the blueprint
			else if (e.NewMenu is CarpenterMenu carpenterMenu)
			{
				bool isMagicalMenu = Helper.Reflection.GetField<bool>(carpenterMenu, "magicalConstruction").GetValue();

				if (isMagicalMenu) return;

				IList<BluePrint> blueprints = Helper.Reflection.GetField<List<BluePrint>>(carpenterMenu, "blueprints").GetValue();

				// Add furnace blueprint, and tag it uniquely based on how many have been built
				blueprints.Add(new BluePrint(furnaceBuildingType)
				{
					maxOccupants = furnacesBuilt.Value,
				});
			}
			else if (modInstantBuildingsFound && e.NewMenu is not null)
			{
				// Try to add the furnace to Instant Buildings' menu
				try
				{
					Type? instantBuildMenuType = Type.GetType("BitwiseJonMods.InstantBuildMenu, BitwiseJonMods.InstantBuildings");

					if (e.NewMenu.GetType().Equals(instantBuildMenuType))
					{
						List<BluePrint> blueprints = Helper.Reflection.GetField<List<BluePrint>>(e.NewMenu, "blueprints").GetValue();

						// Add furnace blueprint, and tag it uniquely based on how many have been built
						blueprints.Add(new BluePrint(furnaceBuildingType)
						{
							maxOccupants = furnacesBuilt.Value,
						});

						// Refresh the blueprints to make building instant and maybe free depending on the mod's config
						Helper.Reflection.GetMethod(e.NewMenu, "ModifyBlueprints").Invoke();
					}
				}
				catch (Exception ex)
				{
					Monitor.Log("Failed editing Instant Building's menu", LogLevel.Error);
					Monitor.Log(ex.ToString(), LogLevel.Error);
				}
			}
		}


		/// <summary>Raised after the game world is drawn to the sprite patch, before it's rendered to the screen.
		/// Content drawn to the sprite batch at this point will be drawn over the world, but under any active menu, HUD elements, or cursor.
		/// </summary>
		private void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
		{
			foreach (IndustrialFurnaceController controller in furnaces.Value)
			{
				if (Game1.currentLocation is not BuildableGameLocation buidableLocation
					|| !buidableLocation.buildings.Contains(controller.furnace))
				{
					continue;
				}

				// This gets called before building list gets changed, so check if the furnace has been added yet
				//int index = GetIndexOfFurnaceControllerWithTag(building.maxOccupants.Value);
				//if (index == -1) continue;

				// Copied from Mill.cs draw(SpriteBatch b) with slight edits

				// Check if there is items to render
				if (controller.output.items.Count <= 0 || controller.output.items[0] is null)
					continue;

				Building? building = controller.furnace;

				if (building is null)
					continue;

				// Get the bobbing from current time
				float num = (float)(4.0 * Math.Round(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 250.0), 2));

				e.SpriteBatch.Draw(Game1.mouseCursors,
					Game1.GlobalToLocal(Game1.viewport, new Vector2(building.tileX.Value * 64 + 180, building.tileY.Value * 64 - 64 + num)),
					new Rectangle(141, 465, 20, 24), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f,
					SpriteEffects.None,
					(float)((building.tileY.Value + 1) * 64 / 10000.0 + 9.99999997475243E-07 + building.tileX.Value / 10000.0));

				e.SpriteBatch.Draw(Game1.objectSpriteSheet,
					Game1.GlobalToLocal(Game1.viewport, new Vector2(building.tileX.Value * 64 + 185 + 32 + 4, building.tileY.Value * 64 - 32 + 8 + num)),
					Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, controller.output.items[0].ParentSheetIndex, 16, 16),
					Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None,
					(float)((building.tileY.Value + 1) * 64 / 10000.0 + 9.99999974737875E-06 + building.tileX.Value / 10000.0));
			}
		}


		/// <summary>Raised after the current player moves to a new location.</summary>
		private void OnWarped(object? sender, WarpedEventArgs e)
		{
			if (e.IsLocalPlayer)
				UpdateFurnaceLights();
		}


		/// <summary>Place items to the furnace</summary>
		/// <param name="furnace">The furnace controller</param>
		/// <returns>Whether the placement was successful or not</returns>
		private bool PlaceItemsToTheFurnace(IndustrialFurnaceController furnace)
		{
			// Items can be placed only if the furnace is NOT on
			if (furnace.CurrentlyOn)
			{
				DisplayHudMessage(i18n.Get("message.furnace-running"), HUDMessage.error_type, "cancel");
				return false;
			}

			// Get the current held object, null for tools etc.
			SObject heldItem = Game1.player.ActiveObject;
			if (heldItem is null) return false;

			int objectId = heldItem.ParentSheetIndex;
			Data.SmeltingRule? rule = newSmeltingRules.GetSmeltingRuleFromInputID(objectId);

			// Check if the object is on the smeltables list
			if (rule is not null)
			{
				// Prevent the game from division by 0, even if the player edits the rules
				if (rule.InputItemAmount == 0)
				{
					Monitor.Log($"The smelting rule for object {objectId} had 0 for input amount.", LogLevel.Error);
					return false;
				}

				int amount = heldItem.Stack;

				// Check if the player has enough to smelt
				if (amount >= rule.InputItemAmount)
				{
					// Remove multiples of the required input amount
					int smeltAmount = amount / rule.InputItemAmount;
					Game1.player.removeItemsFromInventory(objectId, smeltAmount * rule.InputItemAmount);
					furnace.AddItemsToSmelt(objectId, smeltAmount * rule.InputItemAmount);

					Monitor.Log($"{Game1.player.Name} placed {smeltAmount * rule.InputItemAmount} {heldItem.Name} to the furnace {furnace.ID}.");
					return true;
				}
				else
				{
					DisplayHudMessage(i18n.Get("message.need-more-ore", new { oreAmount = rule.InputItemAmount }), HUDMessage.error_type, "cancel");
					return false;
				}
			}
			// Check if the player tries to put coal in the furnace and start the smelting
			else if (objectId == SObject.coal && !furnace.CurrentlyOn)
			{
				// The input has items to smelt
				if (furnace.input.items.Count > 0)
				{
					if (heldItem.Stack >= config.CoalAmount)
					{
						Game1.player.removeItemsFromInventory(objectId, config.CoalAmount);

						Monitor.Log($"{Game1.player.Name} started the furnace {furnace.ID} with {config.CoalAmount} {heldItem.Name}.");

						if (config.InstantSmelting)
						{
							Monitor.Log("And it finished immediately.");
							FinishSmelting(furnace);
						}
						else
						{
							furnace.ChangeCurrentlyOn(true);
							UpdateTexture(furnace.furnace, true);

							CreateLight(furnace);
						}

						Game1.playSound("furnace");
						return true;
					}
					else
					{
						DisplayHudMessage(i18n.Get("message.more-coal", new { coalAmount = config.CoalAmount }), HUDMessage.error_type, "cancel");
						return false;
					}
				}
				else
				{
					DisplayHudMessage(i18n.Get("message.place-something-first"), HUDMessage.error_type, "cancel");
					return false;
				}
			}
			else
			{
				DisplayHudMessage(i18n.Get("message.cant-smelt-this"), HUDMessage.error_type, "cancel");
				return false;
			}
		}


		/// <summary>Called when the player tries to interact with the output chest</summary>
		/// <param name="furnace">The furnace controller that's being interacted with</param>
		private void CollectItemsFromTheFurnace(IndustrialFurnaceController furnace)
		{
			// Clear the output of removed items
			furnace.output.clearNulls();

			// Show output chest only if it contains something
			if (furnace.output.items.Count == 0)
				return;

			currentlyLookingAtFurnace.Value = furnace.ID;
			DrawOutputMenu(furnace);
		}


		/// <summary>Processes the input chest's items and places the result to the output</summary>
		/// <param name="furnace"></param>
		private void FinishSmelting(IndustrialFurnaceController furnace)
		{
			// TODO: Add checks to prevent loss of items, since it is possible that 'output amount' > 'input amount'

			Monitor.Log("Processing the outputs.");

			// Collect the object data to a dictionary (ID, amount) first to fix losing items with over 999 stacks
			Dictionary<int, int> smeltablesDictionary = new Dictionary<int, int>();

			foreach (Item item in furnace.input.items)
			{
				int objectId = item.ParentSheetIndex;

				if (smeltablesDictionary.ContainsKey(objectId))
				{
					smeltablesDictionary[objectId] += item.Stack;
				}
				else
				{
					smeltablesDictionary.Add(objectId, item.Stack);
				}
			}

			// Now the dictionary consists of ItemID: Amount
			foreach (KeyValuePair<int, int> kvp in smeltablesDictionary)
			{
				Data.SmeltingRule? rule = newSmeltingRules.GetSmeltingRuleFromInputID(kvp.Key);

				if (rule is null)
				{
					// This should never be hit, but let's error it just incase...
					Monitor.Log($"Item with ID {kvp.Key} wasn't in the smelting rules despite being in the input chest!", LogLevel.Error);
					continue;
				}

				if (rule.InputItemAmount == 0)
				{
					Monitor.Log($"The input amount for object {kvp.Key} was 0. The result can't be processed so the item will be voided.", LogLevel.Error);
				}

				int outputAmount = (kvp.Value / rule.InputItemAmount) * rule.OutputItemAmount;

				Monitor.Log($"Found {kvp.Value} objects with ID {kvp.Key}. The smelting result is {outputAmount} objects of ID {rule.OutputItemID}.");

				// Add the result defined by the smelting rule to the output chest
				// Assumes the value is divisible with the input amount
				furnace.AddItemsToSmeltedChest(rule.OutputItemID, outputAmount);
			}

			for (int i = 0; i < furnace.input.items.Count; i++)
			{
				furnace.input.items[i] = null;
			}
			furnace.input.clearNulls();
			furnace.ChangeCurrentlyOn(false);

			// Update the texture of the furnace
			UpdateTexture(furnace.furnace, false);
		}


		/// <summary>Returns the index of the matching controller in the furnaces list</summary>
		/// <param name="tag">The tag of searched furnace controller</param>
		/// <returns>Either the index or -1 if no tag matches are found</returns>
		private int GetIndexOfFurnaceControllerWithTag(int tag)
		{
			for (int i = 0; i < furnaces.Value.Count; i++)
			{
				// Assumes the furnace has been added to the list once
				if (GetPerScreenFurnaceController(i).ID == tag)
				{
					return i;
				}
			}

			return -1;
		}


		/// <summary>Switches the building's texture between ON and OFF versions</summary>
		/// <param name="building"></param>
		/// <param name="currentlyOn"></param>
		private void UpdateTexture(Building? building, bool currentlyOn)
		{
			if (building is null)
			{
				Monitor.Log("Tried to update the texture of a null building", LogLevel.Error);
				return;
			}

			if (currentlyOn)
			{
				building.texture = new Lazy<Texture2D>(() => Helper.GameContent.Load<Texture2D>(assetOnName));
			}
			else
			{
				building.texture = new Lazy<Texture2D>(() => Helper.GameContent.Load<Texture2D>(defaultAssetName));
			}
		}


		/// <summary>Updates the textures of all furnaces. Used to sync with multiplayer save data changes.</summary>
		private void UpdateTextures()
		{
			for (int i = 0; i < furnaces.Value.Count; i++)
			{
				UpdateTexture(GetPerScreenFurnaceController(i).furnace, GetPerScreenFurnaceController(i).CurrentlyOn);
			}
		}


		/// <summary>
		/// Update the light sources for furnaces, but only of the player is on a BuildableGameLocation.
		/// </summary>
		private void UpdateFurnaceLights()
		{
			for (int i = 0; i < furnaces.Value.Count; i++)
			{
				IndustrialFurnaceController controller = GetPerScreenFurnaceController(i);

				if (controller.CurrentlyOn)
				{
					if (Game1.currentLocation is not BuildableGameLocation buildableLocation
						|| !buildableLocation.buildings.Contains(controller.furnace))
					{
						continue;
					}

					if (controller.lightSource is null)
					{
						CreateLight(controller);
					}
					else if (!Game1.currentLightSources.Contains(controller.lightSource))
					{
						Game1.currentLightSources.Add(controller.lightSource);
					}
				}
				else if (controller.lightSource is not null)
				{
					if (Game1.currentLightSources.Contains(controller.lightSource))
					{
						Game1.currentLightSources.Remove(controller.lightSource);
					}

					controller.lightSource = null;
				}
			}
		}


		/// <summary>
		/// Create a new light source and link it to the furnace
		/// </summary>
		/// <param name="controller"></param>
		private void CreateLight(IndustrialFurnaceController controller)
		{
			Building building = controller.furnace!;
			Vector2 pos = new Vector2(building.tileX.Value * 64 + fireAnimationData.LightSourceXOffset, building.tileY.Value * 64 + fireAnimationData.LightSourceYOffset);

			LightSource light = new LightSource(4, pos, fireAnimationData.LightSourceScaleMultiplier, Color.DarkCyan, controller.ID * lightSourceIDMultiplier);

			// Make the furnace light up the area
			Game1.currentLightSources.Add(light);
			controller.lightSource = light;
		}


		/// <summary>Displays a HUD message of defined type with a possible sound effect</summary>
		/// <param name="s">Displayed message</param>
		/// <param name="type">Message type</param>
		/// <param name="sound">Sound effect</param>
		private static void DisplayHudMessage(string s, int type, string? sound = null)
		{
			Game1.addHUDMessage(new HUDMessage(s, type));

			if (sound is not null)
			{
				Game1.playSound(sound);
			}
		}


		/// <summary>Remove rules that depend on not installed mods</summary>
		private void CheckSmeltingRules()
		{
			newSmeltingRules = new(smeltingRulesDictionary, Monitor);
			//newSmeltingRules.SmeltingRules.RemoveAll(item => item.RequiredModID is not null && !Helper.ModRegistry.IsLoaded(item.RequiredModID));
			newSmeltingRules.SmeltingRules.RemoveAll(item => item.RequiredModID is not null && !IsAllModIDsLoaded(item.RequiredModID));
		}


		/// <summary>
		/// Check if all of the mod IDs in the array are loaded
		/// </summary>
		/// <param name="s">The array of mod ID strings</param>
		/// <returns>If all of the mod IDs were loaded</returns>
		private bool IsAllModIDsLoaded(string[] s)
		{
			// This should never happen. The array should always be either null or have elements, but let's check against it and log it just in case
			if (s.Length == 0)
			{
				Monitor.Log("Empty list of mod ids encountered", LogLevel.Warn);
				return false;
			}

			for (int i = 0; i < s.Length; i++)
			{
				if (!Helper.ModRegistry.IsLoaded(s[i]))
					return false;
			}

			return true;
		}


		/// <summary>Update the furnace data from the save data</summary>
		private void InitializeFurnaceControllers(bool readSaveData)
		{
			// Initialize the lists to prevent data leaking from previous games
			furnaces.Value.Clear();

			// Load the saved data. If not present, initialize new
			if (readSaveData)
			{
				modSaveData = Helper.Data.ReadSaveData<Data.ModSaveData>(controllerDataSaveKey);
			}

			if (modSaveData is null)
			{
				modSaveData = new Data.ModSaveData();
			}
			else
			{
				modSaveData.ParseModSaveDataToControllers(furnaces.Value, this);
			}

			// Update furnacesBuilt counter to match the highest id of built furnaces (+1)
			int highestId = -1;

			for (int i = 0; i < furnaces.Value.Count; i++)
			{
				if (GetPerScreenFurnaceController(i).ID > highestId)
				{
					highestId = GetPerScreenFurnaceController(i).ID;
				}
			}

			furnacesBuilt.Value = highestId + 1;

			// Repopulate the list of furnaces, only checks the farm!
			foreach (Building building in Game1.getFarm().buildings)
			{
				if (MainIsBuildingIndustrialFurnace(building))
				{
					for (int i = 0; i < furnaces.Value.Count; i++)
					{
						if (building.maxOccupants.Value == GetPerScreenFurnaceController(i).ID)
						{
							GetPerScreenFurnaceController(i).furnace = building;
						}
					}
				}
			}

			// Clean controllers with null buildings from the save file
			// Caused by playing (atleast) splitscreen multiplayer with versions older than 1.7.4
			if (readSaveData)
			{
				int removed = furnaces.Value.RemoveAll(item => item.furnace is null);

				if (removed > 0)
				{
					Monitor.Log($"Removed {removed} corrupted furnace controllers from the save file.", LogLevel.Warn);

					// Refresh the modSaveData
					modSaveData.ParseControllersToModSaveData(furnaces.Value);
				}
			}
		}


		private static void DrawOutputMenu(IndustrialFurnaceController furnace)
		{
			// Display the menu for the output chest
			Game1.activeClickableMenu = new ItemGrabMenu(
				furnace.output.items,
				false,
				true,
				new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems),
				null,
				null,
				(item, farmer) => furnace.GrabItemFromChest(item, farmer),
				false,
				true,
				true,
				true,
				false,
				0,
				null,
				-1,
				null);
		}


		/// <summary>Update the save data to match the controllers data</summary>
		private void InitializeSaveData()
		{
			// TODO: Change how mod save data is used? Will probably have to wait for 1.6
			if (modSaveData is null)
			{
				Monitor.Log("Mod save data was unexpectedly null", LogLevel.Error);
				return;
			}

			modSaveData.ClearOldData();
			modSaveData.ParseControllersToModSaveData(furnaces.Value);
		}


		private IndustrialFurnaceController GetPerScreenFurnaceController(int index)
		{
			return furnaces.Value[index];
		}
	}
}