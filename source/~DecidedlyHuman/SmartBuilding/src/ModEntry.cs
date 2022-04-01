/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmartBuilding.HarmonyPatches;
using SmartBuilding.Helpers;
using SmartBuilding.Utilities;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.SDKs;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

/*
TODO: Add patches to ensure chests can't be opened, fish ponds interacted with, etc. while painting.
TODO: Implement correct spacing restrictions for fruit trees, etc. Should be relatively simple with a change to our adjacent tile detection method.
TODO: Split things into separate classes where it would make things neater.
*/

namespace SmartBuilding
{
	public class ModEntry : Mod, IAssetLoader
	{
		// SMAPI gubbins.
		private static IModHelper _helper;
		private static IMonitor _monitor;
		private static Logger _logger;
		private static ModConfig _config;

		private Dictionary<Vector2, ItemInfo> _tilesSelected = new Dictionary<Vector2, ItemInfo>();
		private Vector2 _currentTile = Vector2.Zero;
		private Vector2 _hudPosition;
		private Texture2D _buildingHud;
		private Texture2D _itemBox;
		private bool _toolbarFlipped = false;
		private int _itemBarWidth = 800; // This is the default.

		private bool _currentlyDrawing = false;
		private bool _currentlyErasing = false;
		private bool _currentlyPlacing = false;
		private bool _buildingMode = false;

		private bool BuildingMode
		{
			get { return _buildingMode; }
			set
			{
				_buildingMode = value;
				HarmonyPatches.Patches.CurrentlyInBuildMode = value;
				
				if (!_buildingMode) // If this is now false, we want to clear the tiles list, and refund everything.
				{
					ClearPaintedTiles();
				}
			}
		}

		private bool CurrentlyDrawing
		{
			get { return _currentlyDrawing; }
			set
			{
				_currentlyDrawing = value;
				HarmonyPatches.Patches.CurrentlyDrawing = value;
			}
		}

		private bool CurrentlyErasing
		{
			get { return _currentlyErasing; }
			set
			{
				_currentlyErasing = value;
				HarmonyPatches.Patches.CurrentlyErasing = value;
			}
		}

		private bool CurrentlyPlacing
		{
			get { return _currentlyPlacing; }
			set
			{
				_currentlyPlacing = value;
				HarmonyPatches.Patches.CurrentlyPlacing = value;
			}
		}

		#region Asset Loading Gubbins

		public bool CanLoad<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals("Mods/DecidedlyHuman/BuildingHUD");
		}

		public T Load<T>(IAssetInfo asset)
		{ // We can just return this, because this mod can load only a single asset.
			return this.Helper.Content.Load<T>(Path.Combine("assets", "HUD.png"));
		}

		#endregion

		public override void Entry(IModHelper helper)
		{
			_helper = helper;
			_monitor = Monitor;
			_logger = new Logger(_monitor);
			_config = _helper.ReadConfig<ModConfig>();
			_hudPosition = new Vector2(50, 0);
			_buildingHud = _helper.Content.Load<Texture2D>("Mods/DecidedlyHuman/BuildingHUD", ContentSource.GameContent);
			_itemBox = _helper.Content.Load<Texture2D>("LooseSprites/tailoring", ContentSource.GameContent);

			Harmony harmony = new Harmony(ModManifest.UniqueID);

			// I'll need more patches to ensure you can't interact with chests, etc., while building. Should be simple. 
			harmony.Patch(
				original: AccessTools.Method(typeof(SObject), nameof(SObject.placementAction)),
				prefix: new HarmonyMethod(typeof(HarmonyPatches.Patches), nameof(HarmonyPatches.Patches.PlacementAction_Prefix)));

			harmony.Patch(
				original: AccessTools.Method(typeof(Chest), nameof(Chest.checkForAction)),
				prefix: new HarmonyMethod(typeof(HarmonyPatches.Patches), nameof(HarmonyPatches.Patches.Chest_CheckForAction_Prefix)));

			harmony.Patch(
				original: AccessTools.Method(typeof(FishPond), nameof(FishPond.doAction)),
				prefix: new HarmonyMethod(typeof(HarmonyPatches.Patches), nameof(HarmonyPatches.Patches.FishPond_DoAction_Prefix)));

			harmony.Patch(
				original: AccessTools.Method(typeof(StorageFurniture), nameof(StorageFurniture.checkForAction)),
				prefix: new HarmonyMethod(typeof(HarmonyPatches.Patches), nameof(HarmonyPatches.Patches.StorageFurniture_DoAction_Prefix)));


			// This is where we'll register with GMCM.
			_helper.Events.GameLoop.GameLaunched += GameLaunched;

			_helper.Events.Input.ButtonsChanged += OnInput;

			// We use this in order to allow for holding and drawing/erasing. The downside is that AddTile gets called twice on the first click.
			// However, the only other way to do this would be to register to UpdateTicked, which would be far more wasteful, potentially.
			_helper.Events.Input.CursorMoved += CursorMoved;

			// This is used to have the queued builds draw themselves in the world.
			_helper.Events.Display.RenderedWorld += RenderedWorld;

			// This is a huge mess, and is used to draw the building mode HUD, and build queue if enabled.
			_helper.Events.Display.RenderedHud += RenderedHud;

			// If the screen is changed, clear our painted tiles, because currently, placing objects is done on the current screen.
			_helper.Events.Player.Warped += (sender, args) =>
			{
				ClearPaintedTiles();
				_buildingMode = false;
				_currentlyDrawing = false;
				HarmonyPatches.Patches.CurrentlyInBuildMode = false;
				HarmonyPatches.Patches.CurrentlyDrawing = false;
				HarmonyPatches.Patches.CurrentlyErasing = false;
				HarmonyPatches.Patches.CurrentlyPlacing = false;
			};
		}

		/// <summary>
		/// SMAPI's <see cref="IInputEvents.ButtonsChanged"> event.
		/// </summary>
		private void OnInput(object? sender, ButtonsChangedEventArgs e)
		{
			// If the world isn't ready, we definitely don't want to do anything.
			if (!Context.IsWorldReady)
				return;

			// If a menu is up, we don't want any of our controls to do anything.
			if (Game1.activeClickableMenu != null)
				return;
			
			if (_config.EnableDebugControls)
			{
				if (_config.IdentifyItem.JustPressed())
				{
					// We now want to identify the currently held item.
					Farmer player = Game1.player;

					if (player.CurrentItem != null)
					{
						if (player.CurrentItem is not Tool)
						{
							ItemType type = IdentifyItemType((SObject)player.CurrentItem);
							Item item = player.CurrentItem;

							_logger.Log($"Item name: {item.Name}");
							_logger.Log($"\t{item.Name}");
							_logger.Log($"ParentSheetIndex:");
							_logger.Log($"\t{item.ParentSheetIndex}");
							_logger.Log($"Stardew Valley category:");
							_logger.Log($"\t{item.Category}");
							_logger.Log($"Stardew Valley type: ");
							_logger.Log($"\t{(item as SObject).Type}");
							_logger.Log($"Identified item as type: ");
							_logger.Log($"\t{type}.");
							_logger.Log("");
						}
					}
				}

				if (_config.IdentifyProducer.JustPressed())
				{
					// We're trying to identify the type of producer under the cursor.
					GameLocation here = Game1.currentLocation;
					Vector2 targetTile = Game1.currentCursorTile;

					if (here.objects.ContainsKey(targetTile))
					{
						SObject producer = here.objects[targetTile];
						ProducerType type = IdentifyProducer(producer);

						_logger.Log($"Identified producer {producer.Name} as {type}.");
					}
				}
			}

			// If the player presses to engage build mode, we flip the bool.
			if (_config.EngageBuildMode.JustPressed())
			{
				// The BuildingMode property takes care of clearing the build queue.
				BuildingMode = !BuildingMode;
			}

			// If the player is drawing placeables in the world. 
			if (_config.HoldToDraw.IsDown())
			{
				if (_buildingMode)
				{
					// We set our CurrentlyDrawing property to true, which will update the value in our patch.
					CurrentlyDrawing = true;

					int inventoryIndex = Game1.player.getIndexOfInventoryItem(Game1.player.CurrentItem);
					AddTile(Game1.player.CurrentItem, Game1.currentCursorTile, inventoryIndex);
				}
			}
			else
			{
				// Otherwise, the key is up, meaning we want to indicate we're not currently drawing.
				CurrentlyDrawing = false;
			}

			if (_config.HoldToErase.IsDown())
			{
				if (_buildingMode)
				{
					// We update this to set both our mod state, and patch bool.
					CurrentlyErasing = true;

					EraseTile(Game1.currentCursorTile);
				}
			}
			else
			{
				// The key is no longer held, so we set this to false.
				CurrentlyErasing = false;
			}

			if (_config.HoldToInsert.IsDown())
			{
				if (_buildingMode)
				{
					// We're in building mode, but we also want to ensure the setting to enable this is on.
					if (_config.EnableInsertingItemsIntoMachines)
					{
						// If it is, we proceed to flag that we're placing items.
						CurrentlyPlacing = true;

						AddItem(Game1.player.CurrentItem, Game1.currentCursorTile);
					}
				}
			}
			else
			{
				CurrentlyPlacing = false;
			}

			if (_config.ConfirmBuild.JustPressed())
			{
				// The build has been confirmed, so we iterate through our Dictionary, and pass each tile into PlaceObject.
				foreach (KeyValuePair<Vector2, ItemInfo> v in _tilesSelected)
				{
					PlaceObject(v);
				}

				// Then, we clear the list, because building is done, and all errors are handled internally.
				_tilesSelected.Clear();
			}

			if (_config.PickUpObject.JustPressed())
			{
				if (_buildingMode) // If we're in building mode, we demolish the tile, indicating we're dealing with an SObject.
					DemolishOnTile(Game1.currentCursorTile, TileFeature.Object);
			}

			if (_config.PickUpFloor.JustPressed())
			{
				if (_buildingMode) // If we're in building mode, we demolish the tile, indicating we're dealing with TerrainFeature.
					DemolishOnTile(Game1.currentCursorTile, TileFeature.TerrainFeature);
			}

			if (_config.PickUpFurniture.JustPressed())
			{
				if (_buildingMode) // If we're in building mode, we demolish the tile, indicating we're dealing with Furniture.
					DemolishOnTile(Game1.currentCursorTile, TileFeature.Furniture);
			}
		}

		private void GameLaunched(object? sender, GameLaunchedEventArgs e)
		{
			RegisterWithGmcm();
		}

		private void RegisterWithGmcm()
		{
			GenericModConfigMenuApi configMenuApi =
				Helper.ModRegistry.GetApi<GenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

			if (configMenuApi == null)
			{
				_logger.Log("The user doesn't have GMCM installed. This is not an error.", LogLevel.Info);

				return;
			}

			configMenuApi.Register(ModManifest,
				() => _config = new ModConfig(),
				() => Helper.WriteConfig(_config));

			configMenuApi.AddSectionTitle(
				mod: ModManifest,
				text: () => "Keybinds"
			);

			configMenuApi.AddParagraph(
				mod: ModManifest,
				text: () => "GMCM currently doesn't support adding mouse keybinds in its config menus. In the meantime, refer to the second page for advice on editing the config.json file to add them manually."
			);

			configMenuApi.AddKeybindList(
				mod: ModManifest,
				name: () => "Engage build mode",
				getValue: () => _config.EngageBuildMode,
				setValue: value => _config.EngageBuildMode = value);

			configMenuApi.AddKeybindList(
				mod: ModManifest,
				name: () => "Hold to draw",
				getValue: () => _config.HoldToDraw,
				setValue: value => _config.HoldToDraw = value);

			configMenuApi.AddKeybindList(
				mod: ModManifest,
				name: () => "Hold to erase",
				getValue: () => _config.HoldToErase,
				setValue: value => _config.HoldToErase = value);

			configMenuApi.AddKeybindList(
				mod: ModManifest,
				name: () => "Hold to insert item",
				getValue: () => _config.HoldToInsert,
				setValue: value => _config.HoldToInsert = value);

			configMenuApi.AddKeybindList(
				mod: ModManifest,
				name: () => "Confirm build",
				getValue: () => _config.ConfirmBuild,
				setValue: value => _config.ConfirmBuild = value);

			configMenuApi.AddKeybindList(
				mod: ModManifest,
				name: () => "Pick up object",
				getValue: () => _config.PickUpObject,
				setValue: value => _config.PickUpObject = value);

			configMenuApi.AddKeybindList(
				mod: ModManifest,
				name: () => "Pick up floor",
				getValue: () => _config.PickUpFloor,
				setValue: value => _config.PickUpFloor = value);

			configMenuApi.AddKeybindList(
				mod: ModManifest,
				name: () => "Pick up furniture",
				getValue: () => _config.PickUpFurniture,
				setValue: value => _config.PickUpFurniture = value);

			configMenuApi.AddSectionTitle(
				mod: ModManifest,
				text: () => "Optional Toggles"
			);

			configMenuApi.AddBoolOption(
				mod: ModManifest,
				name: () => "Show build queue",
				getValue: () => _config.ShowBuildQueue,
				setValue: value => _config.ShowBuildQueue = value
			);

			configMenuApi.AddBoolOption(
				mod: ModManifest,
				name: () => "Can pick up chests",
				tooltip: () => "WARNING: This will drop all contained items on the ground.",
				getValue: () => _config.CanDestroyChests,
				setValue: value => _config.CanDestroyChests = value
			);

			configMenuApi.AddBoolOption(
				mod: ModManifest,
				name: () => "More lax floor placement",
				tooltip: () => "Allows you to place floors essentially anywhere, including UNREACHABLE AREAS. BE CAREFUL WITH THIS.",
				getValue: () => _config.LessRestrictiveFloorPlacement,
				setValue: value => _config.LessRestrictiveFloorPlacement = value
			);

			configMenuApi.AddBoolOption(
				mod: ModManifest,
				name: () => "More lax furniture placement",
				tooltip: () => "Allows you to place furniture essentially anywhere, including UNREACHABLE AREAS. BE CAREFUL WITH THIS.",
				getValue: () => _config.LessRestrictiveFurniturePlacement,
				setValue: value => _config.LessRestrictiveFurniturePlacement = value
			);

			configMenuApi.AddBoolOption(
				mod: ModManifest,
				name: () => "More lax bed placement",
				tooltip: () => "Allows you to place beds essentially anywhere, allowing you to sleep in places you shouldn't be able to sleep in. BE CAREFUL WITH THIS.",
				getValue: () => _config.LessRestrictiveBedPlacement,
				setValue: value => _config.LessRestrictiveBedPlacement = value
			);

			configMenuApi.AddBoolOption(
				mod: ModManifest,
				name: () => "Replaceable floors",
				tooltip: () => "Allows you to replace an existing floor/path with another. Note that you will not get the existing floor back (yet).",
				getValue: () => _config.EnableReplacingFloors,
				setValue: value => _config.EnableReplacingFloors = value
			);

			configMenuApi.AddBoolOption(
				mod: ModManifest,
				name: () => "Replaceable fences",
				tooltip: () => "Allows you to replace an existing fence with another. Note that you will not get the existing fence back.",
				getValue: () => _config.EnableReplacingFences,
				setValue: value => _config.EnableReplacingFences = value
			);

			configMenuApi.AddSectionTitle(
				mod: ModManifest,
				text: () => "The Slightly Cheaty Zone"
			);

			configMenuApi.AddBoolOption(
				mod: ModManifest,
				name: () => "Place crab pots in any water tile",
				getValue: () => _config.CrabPotsInAnyWaterTile,
				setValue: b => _config.CrabPotsInAnyWaterTile = b
			);

			configMenuApi.AddBoolOption(
				mod: ModManifest,
				name: () => "Allow planting crops",
				getValue: () => _config.EnablePlantingCrops,
				setValue: b => _config.EnablePlantingCrops = b
			);

			configMenuApi.AddBoolOption(
				mod: ModManifest,
				name: () => "Allow fertilizing crops",
				getValue: () => _config.EnableCropFertilizers,
				setValue: b => _config.EnableCropFertilizers = b
			);

			configMenuApi.AddBoolOption(
				mod: ModManifest,
				name: () => "Allow fertilizing trees",
				getValue: () => _config.EnableTreeFertilizers,
				setValue: b => _config.EnableTreeFertilizers = b
			);

			configMenuApi.AddBoolOption(
				mod: ModManifest,
				name: () => "Allow tree tappers",
				getValue: () => _config.EnableTreeTappers,
				setValue: b => _config.EnableTreeTappers = b
			);

			configMenuApi.AddBoolOption(
				mod: ModManifest,
				name: () => "Enable placing items into machines",
				getValue: () => _config.EnableInsertingItemsIntoMachines,
				setValue: b => _config.EnableInsertingItemsIntoMachines = b
			);
			
			configMenuApi.AddSectionTitle(
				mod: ModManifest,
				text: () => "Debug"
			);
			
			configMenuApi.AddBoolOption(
				mod: ModManifest,
				name: () => "Enable debug keybinds",
				getValue: () => _config.EnableDebugControls,
				setValue: b => _config.EnableDebugControls = b
			);
			
			configMenuApi.AddKeybindList(
				mod: ModManifest,
				name: () => "Identify producer to console",
				getValue: () => _config.IdentifyProducer,
				setValue: value => _config.IdentifyProducer = value);
			
			configMenuApi.AddKeybindList(
				mod: ModManifest,
				name: () => "Identify held item to console",
				getValue: () => _config.IdentifyItem,
				setValue: value => _config.IdentifyItem = value);

			configMenuApi.AddSectionTitle(
				mod: ModManifest,
				text: () => "THIS NEXT OPTION IS POTENTIALLY DANGEROUS."
			);

			configMenuApi.AddParagraph(
				mod: ModManifest,
				text: () => "You shouldn't, but you might lose items inside your dressers/other storage furniture. If you do, please let me know."
			);

			configMenuApi.AddBoolOption(
				mod: ModManifest,
				name: () => "Enable placing storage furniture",
				tooltip: () => "WARNING: PLACING STORAGE FURNITURE WITH SMART BUILDING IS RISKY. Your items should transfer over just fine, but it's your risk to take.",
				getValue: () => _config.EnablePlacingStorageFurniture,
				setValue: value => _config.EnablePlacingStorageFurniture = value
			);

			configMenuApi.AddPageLink(
				mod: ModManifest,
				pageId: "JsonGuide",
				text: () => "(Click me!) A short guide on adding mouse bindings."
			);

			configMenuApi.AddPage(
				mod: ModManifest,
				pageId: "JsonGuide",
				pageTitle: () => "Mouse Key Bindings"
			);

			configMenuApi.AddParagraph(
				mod: ModManifest,
				text: () => "From: https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings#Multi-key_bindings"
			);

			configMenuApi.AddParagraph(
				mod: ModManifest,
				text: () => "Mods using SMAPI 3.9+ features can support multi-key bindings. That lets you combine multiple button codes into a combo keybind, and list alternate keybinds. For example, \"LeftShoulder, LeftControl + S\" will apply if LeftShoulder is pressed, or if both LeftControl and S are pressed."
			);

			configMenuApi.AddParagraph(
				mod: ModManifest,
				text: () => "Some things to keep in mind:"
			);

			configMenuApi.AddParagraph(
				mod: ModManifest,
				text: () => "The order doesn't matter, so \"LeftControl + S\" and \"S + LeftControl\" are equivalent."
			);

			configMenuApi.AddParagraph(
				mod: ModManifest,
				text: () => "SMAPI doesn't prevent mods from using overlapping hotkeys. For example, if one mod uses \"S\" and the other mod uses \"LeftControl + S\", pressing LeftControl and S will activate both."
			);
		}

		// TODO: Actually comment things in this method.
		private void RenderedHud(object? sender, RenderedHudEventArgs e)
		{
			if (_buildingMode)
			{ // There's absolutely no need to run this while we're not in building mode.
				int windowWidth = Game1.game1.Window.ClientBounds.Width;

				// TODO: Use the newer logic I have to get the toolbar position for this.
				_hudPosition = new Vector2(
					(windowWidth / 2) - (_itemBarWidth / 2) - _buildingHud.Width * 4,
					0);

				e.SpriteBatch.Draw(
					texture: _buildingHud,
					position: _hudPosition,
					sourceRectangle: _buildingHud.Bounds,
					color: Color.White,
					rotation: 0f,
					origin: Vector2.Zero,
					scale: Game1.pixelZoom,
					effects: SpriteEffects.None,
					layerDepth: 1f
				);

				if (_config.ShowBuildQueue)
				{
					Dictionary<Item, int> itemAmounts = new Dictionary<Item, int>();

					foreach (var item in _tilesSelected.Values.GroupBy(x => x))
					{
						itemAmounts.Add(item.Key.Item, item.Count());
					}

					float screenWidth, screenHeight;
					screenWidth = Game1.uiViewport.Width * Game1.options.zoomLevel;
					screenHeight = Game1.uiViewport.Height * Game1.options.zoomLevel;
					Vector2 startingPoint = new Vector2();

					#region Shameless decompile copy

					Point playerGlobalPosition = Game1.player.GetBoundingBox().Center;
					Vector2 playerLocalVector = Game1.GlobalToLocal(globalPosition: new Vector2(playerGlobalPosition.X, playerGlobalPosition.Y), viewport: Game1.viewport);
					bool toolbarAtTop = ((playerLocalVector.Y > (float)(Game1.viewport.Height / 2 + 64)) ? true : false);

					#endregion


					if (toolbarAtTop)
					{
						startingPoint = new Vector2(screenWidth / 2 - 398, 130);
					}
					else
						startingPoint = new Vector2(screenWidth / 2 - 398, screenHeight - 230);

					foreach (var item in itemAmounts)
					{
						e.SpriteBatch.Draw(
							texture: _itemBox,
							position: startingPoint,
							sourceRectangle: new Rectangle(0, 128, 24, 24),
							color: Color.White,
							rotation: 0f,
							origin: Vector2.Zero,
							scale: Game1.pixelZoom,
							effects: SpriteEffects.None,
							layerDepth: 1f
						);

						item.Key.drawInMenu(
							e.SpriteBatch,
							startingPoint + new Vector2(17, 16),
							0.75f, 1f, 4f, StackDrawType.Hide);

						DrawStringWithShadow(
							spriteBatch: e.SpriteBatch,
							font: Game1.smallFont,
							text: item.Value.ToString(),
							position: startingPoint + new Vector2(10, 14) * Game1.pixelZoom,
							textColour: Color.White,
							shadowColour: Color.Black
						);

						startingPoint += new Vector2(24 * Game1.pixelZoom + 4, 0);
					}
				}
			}
		}

		private void DrawStringWithShadow(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color textColour, Color shadowColour)
		{
			spriteBatch.DrawString(
				spriteFont: font,
				text: text,
				position: position + new Vector2(2, 2),
				shadowColour
			);

			spriteBatch.DrawString(
				spriteFont: font,
				text: text,
				position: position,
				textColour
			);
		}

		private void CursorMoved(object? sender, CursorMovedEventArgs e)
		{
			// If the world isn't ready, we definitely don't want to do anything.
			if (!Context.IsWorldReady)
				return;

			// If a menu is up, we don't want any of our controls to do anything.
			if (Game1.activeClickableMenu != null)
				return;

			// If the player is holding down the draw keybind, we want to call AddTile to see if we can
			// add the selected item to the tile under the cursor.
			if (_currentlyDrawing)
			{
				int inventoryIndex = Game1.player.getIndexOfInventoryItem(Game1.player.CurrentItem);

				AddTile(Game1.player.CurrentItem, Game1.currentCursorTile, inventoryIndex);
			}

			// If the player is holding the erase keybind, we want to see if we can remove a registered tile
			// under the cursor, and refund the item where applicable.
			if (_currentlyErasing)
			{
				EraseTile(Game1.currentCursorTile);
			}
		}

		private void EraseTile(Vector2 tile)
		{
			Vector2 flaggedForRemoval = new Vector2();

			foreach (var item in _tilesSelected)
			{
				if (item.Key == tile)
				{
					// If we're over a tile in _tilesSelected, remove it and refund the item to the player.
					Game1.player.addItemToInventoryBool(item.Value.Item.getOne(), false);
					_monitor.Log($"Refunding {item.Value.Item.Name} back into player's inventory.");

					// And flag it for removal from the queue, since we can't remove from within the foreach.
					flaggedForRemoval = tile;
				}
			}

			_tilesSelected.Remove(flaggedForRemoval);
		}

		private bool IsTypeOfObject(SObject o, ItemType type)
		{
			// We try to identify what kind of object we've been passed.
			ItemType oType = IdentifyItemType(o);

			return oType == type;
		}

		private ProducerType IdentifyProducer(SObject o)
		{
			ProducerType type = ProducerType.NotAProducer;

			if (o.Category == -9 && o.Type.Equals("Crafting"))
			{
				// We know this matches the two things all producers (both vanilla and PFM) have in common, so now we can move on to figuring out exactly what type of producer we're looking at.
				string producerName = o.Name;

				// Now, the most efficient thing to do will be to attempt to find only the vanilla machines which do not deduct automatically, as everything else, vanilla and PFM, deducts automatically.
				switch (producerName)
				{
					case "Mayonnaise Machine":
						type = ProducerType.ManualRemoval;
						break;
					case "Preserves Jar":
						type = ProducerType.ManualRemoval;
						break;
					case "Cheese Press":
						type = ProducerType.ManualRemoval;
						break;
					case "Loom":
						type = ProducerType.ManualRemoval;
						break;
					case "Keg":
						type = ProducerType.ManualRemoval;
						break;
					case "Cask":
						type = ProducerType.ManualRemoval;
						break;
					case "Oil Maker":
						type = ProducerType.ManualRemoval;
						break;
					case "Crystalarium":
						type = ProducerType.ManualRemoval;
						break;
					case "Recycling Machine":
						type = ProducerType.ManualRemoval;
						break;
					case "Seed Maker":
						type = ProducerType.ManualRemoval;
						break;
					case "Slime Incubator":
						type = ProducerType.ManualRemoval;
						break;
					case "Ostrich Incubator":
						type = ProducerType.ManualRemoval;
						break;
					case "Deconstructor":
						type = ProducerType.ManualRemoval;
						break;
					default:
						// At this point, we've filtered out all vanilla producers which require manual removal, so we're left with only producers, vanilla and modded, that deduct automatically.
						type = ProducerType.AutomaticRemoval;
						break;
				}

				return type;
			}

			return type;
		}

		private bool CanBeInsertedHere(Vector2 targetTile, Item item)
		{
			// First, we need to ensure there's an SObject here.
			if (Game1.currentLocation.objects.ContainsKey(targetTile))
			{
				// There is one, so we grab a reference to it.
				SObject o = Game1.currentLocation.objects[targetTile];

				// We also need to know what type of producer we're looking at, if any.
				ProducerType type = IdentifyProducer(o);

				// Whether or not we need to manually deduct the item.
				bool needToDeduct = false;

				// If this isn't a producer, we return immediately.
				if (type == ProducerType.NotAProducer)
				{
					return false;
				}
				else if (type == ProducerType.ManualRemoval)
				{
					// If this requires manual removal, we mark that we do need to manually deduct the item.
					needToDeduct = true;
				}
				else if (type == ProducerType.AutomaticRemoval)
				{
					// The producer in question removes automatically, so we don't need to manually deduct the item.
					needToDeduct = false;
				}

				InsertItem(item, o, needToDeduct);
			}

			// There was no object here, so we return false.
			return false;
		}

		private void InsertItem(Item item, SObject o, bool shouldManuallyDeduct)
		{
			// For some reason, apparently, we always need to deduct the held item by one, even if we're working with a producer which does it by itself.
			// TODO: Investigate this, but for now, this seems to work.

			//if (shouldManuallyDeduct)
			//{
			//    // This is marked as needing to be deducted manually, so we do just that.
			//    Game1.player.reduceActiveItemByOne();
			//}

			// First, we perform the drop in action.
			bool successfullyInserted = o.performObjectDropInAction(item, false, Game1.player);

			// Then, perplexingly, we still need to manually deduct the item by one, or we can end up with an item that has a stack size of zero.
			if (successfullyInserted)
			{
				Game1.player.reduceActiveItemByOne();
			}
		}

		/// <summary>
		/// Will return whether or not a tile can be placed 
		/// </summary>
		/// <param name="v">The world-space Tile in which the check is to be performed.</param>
		/// <param name="i">The placeable type.</param>
		/// <returns></returns>
		private bool CanBePlacedHere(Vector2 v, Item i)
		{
			ItemType itemType = IdentifyItemType((SObject)i);
			GameLocation here = Game1.currentLocation;

			switch (itemType)
			{
				case ItemType.CrabPot: // We need to determine if the crab pot is being placed in an appropriate water tile.
					return CrabPot.IsValidCrabPotLocationTile(here, (int)v.X, (int)v.Y) && HasAdjacentNonWaterTile(v);
				case ItemType.GrassStarter:
					// If there's a terrain feature here, we can't possibly place a grass starter.
					return !here.terrainFeatures.ContainsKey(v);
				case ItemType.Floor:
					// In this case, we need to know whether there's a TerrainFeature in the tile.
					if (here.terrainFeatures.ContainsKey(v))
					{
						// At this point, we know there's a terrain feature here, so we grab a reference to it.
						TerrainFeature tf = Game1.currentLocation.terrainFeatures[v];

						// Then we check to see if it is, indeed, Flooring.
						if (tf != null && tf is Flooring)
						{
							// If it is, and if the setting to replace floors with floors is enabled, we return true.
							if (_config.EnableReplacingFloors)
								return true;
						}

						return false;
					}
					else if (here.objects.ContainsKey(v))
					{
						// We know an object exists here now, so we grab it.
						SObject o = here.objects[v];
						ItemType type;
						Item itemToDestroy;

						itemToDestroy = Utility.fuzzyItemSearch(o.Name);
						type = IdentifyItemType((SObject)itemToDestroy);

						if (type == ItemType.Fence)
						{
							// This is a fence, so we return true.

							return true;
						}
					}

					// At this point, we return appropriately with vanilla logic, or true depending on the placement setting.
					return _config.LessRestrictiveFloorPlacement || here.isTileLocationTotallyClearAndPlaceable(v);
				case ItemType.Chest:
					goto case ItemType.Generic;
				case ItemType.Fertilizer:
					// If the setting to enable fertilizers is off, return false to ensure they can't be added to the queue.
					if (!_config.EnableCropFertilizers)
						return false;

					// If there's an object present, we don't want to place any fertilizer.
					// It is technically valid, but there's no reason someone would want to.
					if (here.Objects.ContainsKey(v))
						return false;

					if (here.terrainFeatures.ContainsKey(v))
					{
						// We know there's a TerrainFeature here, so next we want to check if it's HoeDirt.
						if (here.terrainFeatures[v] is HoeDirt)
						{
							// If it is, we want to grab the HoeDirt, and check for the possibility of planting.
							HoeDirt hd = (HoeDirt)here.terrainFeatures[v];

							if (hd.crop != null)
							{
								// If the HoeDirt has a crop, we want to grab it and check for growth phase and fertilization status.
								Crop cropToCheck = hd.crop;

								if (cropToCheck.currentPhase.Value != 0)
								{
									// If the crop's current phase is not zero, we return false.

									return false;
								}
							}

							// At this point, we fall to vanilla logic to determine placement validity.
							return hd.canPlantThisSeedHere(i.ParentSheetIndex, (int)v.X, (int)v.Y, true);
						}
					}

					return false;
				case ItemType.TreeFertilizer:
					// If the setting to enable tree fertilizers is off, return false to ensure they can't be added to the queue.
					if (!_config.EnableTreeFertilizers)
						return false;

					// First, we determine if there's a TerrainFeature here.
					if (here.terrainFeatures.ContainsKey(v))
					{
						// Then we check if it's a tree.
						if (here.terrainFeatures[v] is Tree)
						{
							// It is a tree, so now we check to see if the tree is fertilised.
							Tree tree = (Tree)here.terrainFeatures[v];

							// If it's already fertilised, there's no need for us to want to place tree fertiliser on it, so we return false.
							if (tree.fertilized.Value)
								return false;
							else
								return true;
						}
					}

					return false;
				case ItemType.Seed:
					// If the setting to enable crops is off, return false to ensure they can't be added to the queue.
					if (!_config.EnablePlantingCrops)
						return false;

					// If there's an object present, we don't want to place a seed.
					// It is technically valid, but there's no reason someone would want to.
					if (here.Objects.ContainsKey(v))
						return false;

					// First, we check for a TerrainFeature.
					if (here.terrainFeatures.ContainsKey(v))
					{
						// Then, we check to see if it's HoeDirt.
						if (here.terrainFeatures[v] is HoeDirt)
						{
							// If it is, we grab a reference to the HoeDirt to use its canPlantThisSeedHere method.
							HoeDirt hd = (HoeDirt)here.terrainFeatures[v];

							return hd.canPlantThisSeedHere(i.ParentSheetIndex, (int)v.X, (int)v.Y);
						}
					}

					return false;
				case ItemType.Tapper:
					// If the setting to enable tree tappers is off, we return false here to ensure nothing further happens.
					if (!_config.EnableTreeTappers)
						return false;

					// First, we need to check if there's a TerrainFeature here.
					if (here.terrainFeatures.ContainsKey(v))
					{
						// If there is, we check to see if it's a tree.
						if (here.terrainFeatures[v] is Tree)
						{
							// If it is, we grab a reference to the tree to check its details.
							Tree tree = (Tree)here.terrainFeatures[v];
							
							// If the tree isn't tapped, we confirm that a tapper can be placed here.
							if (!tree.tapped)
							{
								// If the tree is fully grown, we *can* place a tapper.
								return tree.growthStage >= 5;
							}
						}
					}

					return false;
				case ItemType.Fence:
					// We want to deal with fences specifically in order to handle fence replacements.
					if (here.objects.ContainsKey(v))
					{
						// We know there's an object at these coordinates, so we grab a reference.
						SObject o = here.objects[v];

						// Then we return true if this is both a fence, and replacing fences is enabled.
						return IsTypeOfObject(o, ItemType.Fence) && _config.EnableReplacingFences;
					}
					else if (here.terrainFeatures.ContainsKey(v))
					{
						// There's a terrain feature here, so we want to check if it's a HoeDirt with a crop.
						TerrainFeature feature = here.terrainFeatures[v];

						if (feature != null && feature is HoeDirt)
						{
							if ((feature as HoeDirt).crop != null)
							{
								// There's a crop here, so we return false.
								return false;
							}

							// At this point, we know it's a HoeDirt, but has no crop, so we can return true.
							return true;
						}
					}

					goto case ItemType.Generic;
				case ItemType.FishTankFurniture:
					// TODO: Until I figure out how to successfully transplant fish, I'm hard blocking these.
					return false;
				case ItemType.StorageFurniture:
					// Since FishTankFurniture will sneak through here:
					if (i is FishTankFurniture)
						return false;

					// If the setting for allowing storage furniture is off, we get the hell out.
					if (!_config.EnablePlacingStorageFurniture)
						return false;

					if (_config.LessRestrictiveFurniturePlacement)
						return true;
					else
						return (i as StorageFurniture).canBePlacedHere(here, v);
				case ItemType.TVFurniture:
					if (_config.LessRestrictiveFurniturePlacement)
						return true;
					else
						return (i as TV).canBePlacedHere(here, v);
				case ItemType.BedFurniture:
					if (_config.LessRestrictiveBedPlacement)
						return true;
					else
						return (i as BedFurniture).canBePlacedHere(here, v);
				case ItemType.GenericFurniture:
					// In this place, we play fast and loose, and return true.
					if (_config.LessRestrictiveFurniturePlacement)
						return true;
					else
						return (i as Furniture).canBePlacedHere(here, v);
				case ItemType.Generic:
					return Game1.currentLocation.isTileLocationTotallyClearAndPlaceableIgnoreFloors(v);
			}

			// If the PlaceableType is somehow none of these, we want to be safe and return false.
			return false;
		}

		private ItemType IdentifyItemType(SObject item)
		{
			// TODO: Make this detection more robust. If possible, don't depend upon it at all.
			string itemName = item.Name;

			// The whole point of this is to determine whether the object being placed requires
			// special treatment at all, and assist us in determining whether it's a TerrainFeature, or an SObject.
			if (!item.isPlaceable())
				return ItemType.NotPlaceable;
			else if (item is FishTankFurniture)
				return ItemType.FishTankFurniture;
			else if (item is StorageFurniture)
				return ItemType.StorageFurniture;
			else if (item is BedFurniture)
				return ItemType.BedFurniture;
			else if (item is TV)
				return ItemType.TVFurniture;
			else if (item is Furniture)
				return ItemType.GenericFurniture;
			else if (itemName.Contains("Floor") || itemName.Contains("Path") && item.Category == -24)
				return ItemType.Floor;
			else if (itemName.Contains("Chest") || item is Chest)
				return ItemType.Chest;
			else if (itemName.Contains("Fence"))
				return ItemType.Fence;
			else if (itemName.Equals("Grass Starter"))
				return ItemType.GrassStarter;
			else if (itemName.Equals("Crab Pot"))
				return ItemType.CrabPot;
			else if (item.Type == "Seeds" || item.Category == -74)
			{
				if (!item.Name.Contains("Sapling") && !item.Name.Equals("Acorn") && !item.Name.Equals("Maple Seed") && !item.Name.Equals("Pine Cone") && !item.Name.Equals("Mahogany Seed"))
					return ItemType.Seed;
			}
			else if (item.Name.Equals("Tree Fertilizer"))
				return ItemType.TreeFertilizer;
			else if (item.Category == -19)
				return ItemType.Fertilizer;
			else if (item.Name.Equals("Tapper") || item.Name.Equals("Heavy Tapper"))
				return ItemType.Tapper;

			return ItemType.Generic;
		}

		private ItemInfo GetItemInfo(SObject item)
		{
			ItemType itemType = IdentifyItemType(item);

			return new ItemInfo()
			{
				Item = item,
				ItemType = itemType
			};
		}

		private void AddItem(Item item, Vector2 v)
		{
			// If we're not in building mode, we do nothing.
			if (!_buildingMode)
				return;

			// If the player isn't holding an item, we do nothing.
			if (Game1.player.CurrentItem == null)
				return;

			// There is no queue for item insertion, so we simply try to insert.
			CanBeInsertedHere(v, item);
		}

		private void AddTile(Item item, Vector2 v, int itemInventoryIndex)
		{
			// If we're not in building mode, we do nothing.
			if (!_buildingMode)
				return;

			// If the player isn't holding an item, we do nothing.
			if (Game1.player.CurrentItem == null)
				return;

			// If the item isn't placeable, we do nothing.
			if (!item.isPlaceable())
				return;

			// If the item cannot be placed here according to our own rules, we do nothing. This is to allow for slightly custom placement logic.
			if (!CanBePlacedHere(v, item))
				return;

			ItemInfo itemInfo = GetItemInfo((SObject)item);

			// We only want to add the tile if the Dictionary doesn't already contain it. 
			if (!_tilesSelected.ContainsKey(v))
			{
				// We then want to check if the item can even be placed in this spot.
				if (CanBePlacedHere(v, item))
				{
					_tilesSelected.Add(v, itemInfo);
					Game1.player.reduceActiveItemByOne();
				}
			}
		}

		private bool HasAdjacentNonWaterTile(Vector2 v)
		{
			// Although crab pots are the only currently tested object that
			// go in water, I do want to modularise this later.
			// TODO: Modularise for not only crab pots.

			if (_config.CrabPotsInAnyWaterTile)
				return true;

			List<Vector2> directions = new List<Vector2>()
			{
				v + new Vector2(-1, 0), // Left
				v + new Vector2(1, 0), // Right
				v + new Vector2(0, -1), // Up
				v + new Vector2(0, 1), // Down
				v + new Vector2(-1, -1), // Up left
				v + new Vector2(1, -1), // Up right
				v + new Vector2(-1, 1), // Down left
				v + new Vector2(1, 1) // Down right
			};

			foreach (Vector2 vector in directions)
			{
				if (!Game1.currentLocation.isWaterTile((int)vector.X, (int)vector.Y))
					return true;
			}

			return false;
		}

		private void RenderedWorld(object? sender, RenderedWorldEventArgs e)
		{
			foreach (KeyValuePair<Vector2, ItemInfo> item in _tilesSelected)
			{
				// Here, we simply have the Item draw itself in the world.
				item.Value.Item.drawInMenu
				(e.SpriteBatch,
					Game1.GlobalToLocal(
						Game1.viewport,
						item.Key * Game1.tileSize),
					1f, 1f, 4f, StackDrawType.Hide);
			}
		}

		private void ClearPaintedTiles()
		{
			// To clear the painted tiles, we want to iterate through our Dictionary, and refund every item contained therein.
			foreach (var t in _tilesSelected)
			{
				RefundItem(t.Value.Item, "User left build mode. Refunding items.", LogLevel.Trace, false);
			}

			// And, finally, clear it.
			_tilesSelected.Clear();
		}

		// TODO: Modularise this method more. Right now, it just works. It is not well structured for future maintenance.
		private void DemolishOnTile(Vector2 tile, TileFeature feature)
		{
			GameLocation here = Game1.currentLocation;
			Vector2 playerTile = Game1.player.getTileLocation();
			Item itemToDestroy;
			ItemType type;

			// We're working with an SObject in this specific instance.
			if (feature == TileFeature.Object)
			{
				if (here.objects.ContainsKey(tile))
				{
					// We have an object in this tile, so we want to try to figure out what it is.
					SObject o = here.objects[tile];
					itemToDestroy = Utility.fuzzyItemSearch(o.Name);

					type = IdentifyItemType((SObject)itemToDestroy);

					// Chests need special handling because they can store items.
					if (type == ItemType.Chest)
					{
						// We're double checking at this point for safety. I want to be extra careful with chests.
						if (here.objects.ContainsKey(tile))
						{
							// If the setting to disable chest pickup is enabled, we pick up the chest. If not, we do nothing.
							if (_config.CanDestroyChests)
							{
								// This is fairly fragile, but it's fine with vanilla chests, at least.
								Chest chest = new Chest(o.ParentSheetIndex, tile, 0, 1);

								(o as Chest).destroyAndDropContents(tile * 64, here);
								Game1.player.addItemByMenuIfNecessary(chest.getOne());
								here.objects.Remove(tile);
							}
						}
					}
					else if (o is Chest)
					{
						// We're double checking at this point for safety. I want to be extra careful with chests.
						if (here.objects.ContainsKey(tile))
						{
							// If the setting to disable chest pickup is enabled, we pick up the chest. If not, we do nothing.
							if (_config.CanDestroyChests)
							{
								// This is fairly fragile, but it's fine with vanilla chests, at least.
								Chest chest = new Chest(o.ParentSheetIndex, tile, 0, 1);

								(o as Chest).destroyAndDropContents(tile * 64, here);
								Game1.player.addItemByMenuIfNecessary(chest.getOne());
								here.objects.Remove(tile);
							}
						}
					}
					else if (type == ItemType.Fence)
					{
						// We need special handling for fences, since we don't want to pick them up if their health has deteriorated too much.
						Fence fenceToRemove = (Fence)o;

						fenceToRemove.performRemoveAction(tile * 64, here);
						here.objects.Remove(tile);

						// And, if the fence had enough health remaining, we refund it.
						if (fenceToRemove.maxHealth.Value - fenceToRemove.health.Value < 0.5f)
							Game1.player.addItemByMenuIfNecessary(fenceToRemove.getOne());
					}
					else
					{
						if (o.Fragility == 2)
						{
							// A fragility of 2 means the item should not be broken, or able to be picked up, like incubators in coops, so we return.

							return;
						}

						if (o.heldObject.Value != null)
						{
							// If the object's heldObject is not null, that means it has an item in it, so we also need to grab that.
							Game1.player.addItemByMenuIfNecessary(o.heldObject);
						}

						o.performRemoveAction(tile * 64, here);
						Game1.player.addItemByMenuIfNecessary(o.getOne());

						here.objects.Remove(tile);
					}
					// TODO: Temporary return!
					return;
				}
			}

			// We're working with a TerrainFeature.
			if (feature == TileFeature.TerrainFeature)
			{
				if (here.terrainFeatures.ContainsKey(tile))
				{
					TerrainFeature tf = here.terrainFeatures[tile];

					// We only really want to be handling flooring when removing TerrainFeatures.
					if (tf is Flooring)
					{
						Flooring floor = (Flooring)tf;

						int? floorType = floor.whichFloor.Value;
						string? floorName = GetFlooringNameFromId(floorType.Value);
						SObject finalFloor;

						if (floorType.HasValue)
						{
							floorName = GetFlooringNameFromId(floorType.Value);
							finalFloor = (SObject)Utility.fuzzyItemSearch(floorName, 1);
						}
						else
						{
							finalFloor = null;
						}

						if (finalFloor != null)
							Game1.player.addItemByMenuIfNecessary(finalFloor);
						// Game1.createItemDebris(finalFloor, playerTile * 64, 1, here);

						here.terrainFeatures.Remove(tile);
					}
				}
			}

			if (feature == TileFeature.Furniture)
			{
				Furniture furnitureToGrab = null;

				foreach (Furniture f in here.furniture)
				{
					if (f.boundingBox.Value.Intersects(new Rectangle((int)tile.X * 64, (int)tile.Y * 64, 1, 1)))
					{
						furnitureToGrab = f;
					}
				}

				if (furnitureToGrab != null)
				{
					_logger.Log($"Trying to grab {furnitureToGrab.Name}");
					Game1.player.addItemToInventory(furnitureToGrab);
					here.furniture.Remove(furnitureToGrab);
				}
			}
		}

		//private ProducerType IdentifyProducerType(SObject o)
		//{
		//  ProducerType producerType;


		//  return producerType;
		//}

		private void PlaceObject(KeyValuePair<Vector2, ItemInfo> item)
		{
			SObject itemToPlace = (SObject)item.Value.Item;
			Vector2 targetTile = item.Key;
			ItemInfo itemInfo = item.Value;
			GameLocation here = Game1.currentLocation;

			if (itemToPlace != null && CanBePlacedHere(targetTile, itemInfo.Item))
			{ // The item can be placed here.
				if (itemInfo.ItemType == ItemType.Floor)
				{
					// We're specifically dealing with a floor/path.

					int? floorType = GetFlooringIdFromName(itemToPlace.Name);
					Flooring floor;

					if (floorType.HasValue)
						floor = new Flooring(floorType.Value);
					else
					{
						// At this point, something is very wrong, so we want to refund the item to the player's inventory, and print an error.
						RefundItem(itemToPlace, "Couldn't figure out the type of floor. This may be a modded floor/path we don't understand", LogLevel.Error, true);

						return;
					}

					// At this point, we *need* there to be no TerrainFeature present.
					if (!here.terrainFeatures.ContainsKey(targetTile))
						here.terrainFeatures.Add(targetTile, floor);
					else
					{
						// At this point, we know there's a terrain feature here.
						if (_config.EnableReplacingFloors)
						{
							TerrainFeature tf = here.terrainFeatures[targetTile];

							if (tf != null && tf is Flooring)
							{
								// At this point, we know it's Flooring, so we remove the existing terrain feature, and add our new one.
								DemolishOnTile(targetTile, TileFeature.TerrainFeature);
								here.terrainFeatures.Add(targetTile, floor);
							}
							else
							{
								// At this point, there IS a terrain feature here, but it isn't flooring, so we want to refund the item, and return.
								RefundItem(item.Value.Item, "There was already a TerrainFeature present. Maybe you hoed the ground before confirming the build");

								// We now want to jump straight out of this method, because this will flow through to the below if, and bad things will happen.
								return;
							}
						}
					}

					// By this point, we'll have returned false if this could be anything but our freshly placed floor.
					if (!(here.terrainFeatures.ContainsKey(item.Key) && here.terrainFeatures[item.Key] is Flooring))
						RefundItem(item.Value.Item);
				}
				else if (itemInfo.ItemType == ItemType.Chest)
				{
					// We're dealing with a chest.
					int? chestType = GetChestType(itemToPlace.Name);
					Chest chest;

					if (chestType.HasValue)
					{
						chest = new Chest(true, chestType.Value);
					}
					else
					{ // At this point, something is very wrong, so we want to refund the item to the player's inventory, and print an error.
						RefundItem(itemToPlace, "Couldn't recognise this item as a chest. This may be a bug", LogLevel.Error, true);

						return;
					}

					// We do our second placement possibility check, just in case something was placed in the meantime.
					if (CanBePlacedHere(targetTile, itemToPlace))
					{
						bool placed = chest.placementAction(here, (int)targetTile.X * 64, (int)targetTile.Y * 64, Game1.player);

						// Apparently, chests placed in the world are hardcoded with the name "Chest".
						if (!here.objects.ContainsKey(targetTile) || !here.objects[targetTile].Name.Equals("Chest"))
							RefundItem(itemToPlace);
					}
				}
				else if (itemInfo.ItemType == ItemType.Fence)
				{
					// We want to check to see if the target tile contains an object.
					if (here.objects.ContainsKey(targetTile))
					{
						SObject o = here.objects[targetTile];

						if (o != null)
						{
							// We try to identify what kind of object is placed here.
							if (IsTypeOfObject(o, ItemType.Fence))
							{
								if (_config.EnableReplacingFences)
								{
									// We have a fence, so we want to remove it before placing our new one.
									DemolishOnTile(targetTile, TileFeature.Object);
								}
							}
							else
							{
								// If it isn't a fence, we want to refund the item, and return to avoid placing the fence.
								RefundItem(item.Value.Item, "There was something in this place. Did something get placed before you committed the build?");
								return;
							}
						}
					}

					if (!itemToPlace.placementAction(Game1.currentLocation, (int)item.Key.X * 64, (int)item.Key.Y * 64, Game1.player))
						RefundItem(item.Value.Item);
				}
				else if (itemInfo.ItemType == ItemType.GrassStarter)
				{
					Grass grassStarter = new Grass(1, 4);

					// At this point, we *need* there to be no TerrainFeature present.
					if (!here.terrainFeatures.ContainsKey(targetTile))
						here.terrainFeatures.Add(targetTile, grassStarter);
					else
					{
						RefundItem(item.Value.Item, "There was already a TerrainFeature present. Maybe you hoed the ground before confirming the build");

						// We now want to jump straight out of this method, because this will flow through to the below if, and bad things may happen.
						return;
					}

					if (!(here.terrainFeatures.ContainsKey(item.Key) && here.terrainFeatures[targetTile] is Grass))
						RefundItem(item.Value.Item);
				}
				else if (itemInfo.ItemType == ItemType.CrabPot)
				{
					CrabPot pot = new CrabPot(targetTile);

					if (CanBePlacedHere(targetTile, itemToPlace))
					{
						itemToPlace.placementAction(Game1.currentLocation, (int)targetTile.X * 64, (int)targetTile.Y * 64, Game1.player);
					}
				}
				else if (itemInfo.ItemType == ItemType.Seed)
				{
					// Here, we're dealing with a seed, so we need very special logic for this.
					// Item.placementAction for seeds is semi-broken, unless the player is currently
					// holding the specific seed being planted.

					bool successfullyPlaced = false;

					// First, we check for a TerrainFeature.
					if (Game1.currentLocation.terrainFeatures.ContainsKey(targetTile))
					{
						// Then, we check to see if it's a HoeDirt.
						if (Game1.currentLocation.terrainFeatures[targetTile] is HoeDirt)
						{
							// If it is, we grab a reference to it.
							HoeDirt hd = (HoeDirt)Game1.currentLocation.terrainFeatures[targetTile];

							// We check to see if it can be planted, and act appropriately.
							if (hd.canPlantThisSeedHere(itemToPlace.ParentSheetIndex, (int)targetTile.X, (int)targetTile.Y))
							{
								successfullyPlaced = hd.plant(itemToPlace.ParentSheetIndex, (int)targetTile.X, (int)targetTile.Y, Game1.player, false, Game1.currentLocation);
							}
						}
					}

					// If the planting failed, we refund the seed.
					if (!successfullyPlaced)
						RefundItem(item.Value.Item);
				}
				else if (itemInfo.ItemType == ItemType.Fertilizer)
				{
					if (here.terrainFeatures.ContainsKey(targetTile))
					{
						// We know there's a TerrainFeature here, so next we want to check if it's HoeDirt.
						if (here.terrainFeatures[targetTile] is HoeDirt)
						{
							// If it is, we want to grab the HoeDirt, check if it's already got a fertiliser, and fertilise if not.
							HoeDirt hd = (HoeDirt)here.terrainFeatures[targetTile];

							// 0 here means no fertilizer. TODO: Known change in 1.6.
							if (hd.fertilizer.Value == 0)
							{
								// Next, we want to check if there's already a crop here.
								if (hd.crop != null)
								{
									Crop cropToCheck = hd.crop;

									if (cropToCheck.currentPhase.Value == 0)
									{
										// If the current crop phase is zero, we can plant the fertilizer here.

										hd.plant(itemToPlace.ParentSheetIndex, (int)targetTile.X, (int)targetTile.Y, Game1.player, true, Game1.currentLocation);
									}
								}
								else
								{
									// If there is no crop here, we can plant the fertilizer with reckless abandon.
									hd.plant(itemToPlace.ParentSheetIndex, (int)targetTile.X, (int)targetTile.Y, Game1.player, true, Game1.currentLocation);
								}
							}
							else
							{
								// If there is already a fertilizer here, we want to refund the item.
								RefundItem(itemToPlace, "There was already fertilizer placed here", LogLevel.Warn);
							}

							// Now, we want to run the final check to see if the fertilization was successful.
							if (hd.fertilizer.Value == 0)
							{
								// If there's still no fertilizer here, we need to refund the item.
								RefundItem(itemToPlace, "There was either fertilizer already here, or the crop is too grown to accept fertilizer", LogLevel.Warn);
							}
						}
					}

				}
				else if (itemInfo.ItemType == ItemType.TreeFertilizer)
				{
					if (here.terrainFeatures.ContainsKey(targetTile))
					{
						// If there's a TerrainFeature here, we check if it's a tree.
						if (here.terrainFeatures[targetTile] is Tree)
						{
							// It is a tree, so now we check to see if the tree is fertilised.
							Tree tree = (Tree)here.terrainFeatures[targetTile];

							// If it's already fertilised, there's no need for us to want to place tree fertiliser on it.
							if (!tree.fertilized.Value)
								tree.fertilize(here);
						}
					}
				}
				else if (itemInfo.ItemType == ItemType.Tapper)
				{
					if (CanBePlacedHere(targetTile, itemToPlace))
					{
						// If there's a TerrainFeature here, we need to know if it's a tree.
						if (here.terrainFeatures[targetTile] is Tree)
						{
							// If it is, we grab a reference, and check for a tapper on it already.
							Tree tree = (Tree)here.terrainFeatures[targetTile];

							if (!tree.tapped.Value)
							{
								if (!itemToPlace.placementAction(here, (int)targetTile.X * 64, (int)targetTile.Y * 64, Game1.player))
								{
									// If the placement action didn't succeed, we refund the item.
									RefundItem(itemToPlace);
								}
							}
						}
					}
				}
				else if (itemInfo.ItemType == ItemType.FishTankFurniture)
				{
					// TODO: This cannot be reached, because placement of fish tanks is blocked for now.
					// // We're dealing with a fish tank. This has dangerous consequences.
					// if (_config.LessRestrictiveFurniturePlacement)
					// {
					//  FishTankFurniture tank = new FishTankFurniture(itemToPlace.ParentSheetIndex, targetTile);
					//
					//  foreach (var fish in (itemToPlace as FishTankFurniture).tankFish)
					//  {
					//      tank.tankFish.Add(fish);
					//  }
					//
					//  foreach (var fish in tank.tankFish)
					//  {
					//      fish.ConstrainToTank();
					//  }
					//  
					//  here.furniture.Add(tank);
					// }
					// else
					// {
					//  (itemToPlace as FishTankFurniture).placementAction(here, (int)targetTile.X, (int)targetTile.Y, Game1.player);
					// }
				}
				else if (itemInfo.ItemType == ItemType.StorageFurniture)
				{
					if (_config.EnablePlacingStorageFurniture)
					{
						bool placedSuccessfully = false;

						// We need to create a new instance of StorageFurniture.
						StorageFurniture storage = new StorageFurniture(itemToPlace.ParentSheetIndex, targetTile);

						// Then, we iterate through all of the items in the existing StorageFurniture, and add them to the new one.
						foreach (var itemInStorage in (itemToPlace as StorageFurniture).heldItems)
						{
							_logger.Log($"Adding item {itemInStorage.Name} with ParentSheetId {itemInStorage.ParentSheetIndex} to newly created storage.");
							storage.AddItem(itemInStorage);
						}

						// If we have less restrictive furniture placement enabled, we simply try to place it. Otherwise, we use the vanilla placementAction.
						if (_config.LessRestrictiveFurniturePlacement)
							here.furniture.Add(storage as StorageFurniture);
						else
							placedSuccessfully = storage.placementAction(here, (int)targetTile.X * 64, (int)targetTile.Y * 64, Game1.player);

						// Here, we check to see if the placement was successful. If not, we refund the item.
						if (!here.furniture.Contains(storage) && !placedSuccessfully)
							RefundItem(storage);
					}
					else
						RefundItem(itemToPlace, "The (potentially dangerous) setting to enable storage furniture was disabled.", LogLevel.Info, true);
				}
				else if (itemInfo.ItemType == ItemType.TVFurniture)
				{
					bool placedSuccessfully = false;
					TV tv = null;

					// We need to determine which we we're placing this TV based upon the furniture placement restriction option.
					if (_config.LessRestrictiveFurniturePlacement)
					{
						tv = new TV(itemToPlace.ParentSheetIndex, targetTile);
						here.furniture.Add(tv);
					}
					else
					{
						placedSuccessfully = (itemToPlace as TV).placementAction(here, (int)targetTile.X * 64, (int)targetTile.Y * 64, Game1.player);
					}

					// If both of these are false, the furniture was not successfully placed, so we need to refund the item.
					if (tv != null && !here.furniture.Contains(tv as TV) && !placedSuccessfully)
						RefundItem(itemToPlace, "The TV wasn't placed successfully. No details available.");
				}
				else if (itemInfo.ItemType == ItemType.BedFurniture)
				{
					bool placedSuccessfully = false;
					BedFurniture bed = null;

					// We decide exactly how we're placing the furniture based upon the less restrictive setting.
					if (_config.LessRestrictiveBedPlacement)
					{
						bed = new BedFurniture(itemToPlace.ParentSheetIndex, targetTile);
						here.furniture.Add(bed);
					}
					else
						placedSuccessfully = (itemToPlace as BedFurniture).placementAction(here, (int)targetTile.X * 64, (int)targetTile.Y * 64, Game1.player);

					// If both of these are false, the furniture was not successfully placed, so we need to refund the item.
					if (bed != null && !here.furniture.Contains(bed as BedFurniture) && !placedSuccessfully)
						RefundItem(itemToPlace, "The TV wasn't placed successfully. No details available.");

				}
				else if (itemInfo.ItemType == ItemType.GenericFurniture)
				{
					bool placedSuccessfully = false;
					Furniture furniture = null;

					// Determine exactly how we're placing this furniture.
					if (_config.LessRestrictiveFurniturePlacement)
					{
						furniture = new Furniture(itemToPlace.ParentSheetIndex, targetTile);
						here.furniture.Add(furniture);
					}
					else
						placedSuccessfully = (itemToPlace as Furniture).placementAction(here, (int)targetTile.X * 64, (int)targetTile.Y * 64, Game1.player);

					// If both of these are false, the furniture was not successfully placed, so we need to refund the item.
					if (furniture != null && !here.furniture.Contains(furniture as Furniture) && !placedSuccessfully)
						RefundItem(itemToPlace, "The TV wasn't placed successfully. No details available.");
				}
				else
				{ // We're dealing with a generic placeable.
					bool successfullyPlaced = itemToPlace.placementAction(Game1.currentLocation, (int)item.Key.X * 64, (int)item.Key.Y * 64, Game1.player);

					// if (Game1.currentLocation.objects.ContainsKey(item.Key) && Game1.currentLocation.objects[item.Key].Name.Equals(itemToPlace.Name))
					if (successfullyPlaced)
					{

					}
					else
						RefundItem(item.Value.Item);
				}
			}
			else
			{
				RefundItem(item.Value.Item);
			}
		}

		private void RefundItem(Item item, string reason = "Something went wrong", LogLevel logLevel = LogLevel.Error, bool shouldLog = false)
		{
			Game1.player.addItemByMenuIfNecessary(item.getOne());

			if (shouldLog)
				_monitor.Log($"{reason}. Refunding {item.Name} back into player's inventory.", logLevel);
		}

		private int? GetFlooringIdFromName(string itemName)
		{
			// TODO: Investigate whether or not there's a less terrible way to do this.
			switch (itemName)
			{
				case "Wood Floor":
					return 0; // Correct.
				case "Rustic Plank Floor":
					return 11; // Correct.
				case "Straw Floor":
					return 4; // Correct
				case "Weathered Floor":
					return 2; // Correct.
				case "Crystal Floor":
					return 3; // Correct.
				case "Stone Floor":
					return 1; // Correct.
				case "Stone Walkway Floor":
					return 12; // Correct.
				case "Brick Floor":
					return 10; // Correct
				case "Wood Path":
					return 6; // Correct.
				case "Gravel Path":
					return 5; // Correct.
				case "Cobblestone Path":
					return 8; // Correct.
				case "Stepping Stone Path":
					return 9; // Correct.
				case "Crystal Path":
					return 7; // Correct.
				default:
					return null;
			}
		}

		private string? GetFlooringNameFromId(int id)
		{
			// TODO: Investigate whether or not there's a less terrible way to do this.
			switch (id)
			{
				case 0:
					return "Wood Floor"; // Correct.
				case 11:
					return "Rustic Plank Floor"; // Correct.
				case 4:
					return "Straw Floor"; // Correct
				case 2:
					return "Weathered Floor"; // Correct.
				case 3:
					return "Crystal Floor"; // Correct.
				case 1:
					return "Stone Floor"; // Correct.
				case 12:
					return "Stone Walkway Floor"; // Correct.
				case 10:
					return "Brick Floor"; // Correct
				case 6:
					return "Wood Path"; // Correct.
				case 5:
					return "Gravel Path"; // Correct.
				case 8:
					return "Cobblestone Path"; // Correct.
				case 9:
					return "Stepping Stone Path"; // Correct.
				case 7:
					return "Crystal Path"; // Correct.
				default:
					return null;
			}
		}

		private int? GetChestType(string itemName)
		{
			// TODO: Investigate whether or not there's a less terrible way to do this.
			switch (itemName)
			{
				case "Chest":
					return 130;
				case "Stone Chest":
					return 232;
				case "Junimo Chest":
					return 256;
				default:
					return null;
			}
		}
	}
}