/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CustomCommunityCentre
**
*************************************************/

using CustomCommunityCentre;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Tiles;

namespace CommunityKitchen
{
	public static class Kitchen
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IReflectionHelper Reflection => ModEntry.Instance.Helper.Reflection;

		// Kitchen definitions
		public const string KitchenAreaName = "Custom_blueberry_Kitchen_Area0";

		// Kitchen fridge
		// We use Linus' tent interior for the dummy area, since there's surely no conceivable way it'd be in the community centre
		public static readonly Rectangle FridgeOpenedSpriteArea = new(32, 560, 16, 32);
		public static readonly Vector2 FridgeChestPosition = new(6830);
		public static string FridgeTilesToUse = "Vanilla";
		public static readonly Dictionary<string, int[]> FridgeTileIndexes = new()
        {
			{ "Vanilla", new [] { 602, 634, 1122, 1154 } },
			{ "SVE", new [] { 432, 440, 432, 442 } }
		};
		public static readonly Dictionary<string, int[]> CookingTileIndexes = new()
        {
			{ "Vanilla", new [] { 498, 499, 631, 632, 633 } },
			{ "SVE", new [] { 498, 499, 631, 632, 633 } },
		};
		public static Vector2 FridgeTilePosition = Vector2.Zero;

		// Esca's Modding Plugins integrations
		public static string EMPTileActionKitchen;
		public static string EMPMapPropertyMinifridges;


		internal static void RegisterEvents()
		{
            Helper.Events.GameLoop.GameLaunched += Kitchen.GameLoop_GameLaunched;
			Helper.Events.Input.ButtonPressed += Kitchen.Input_ButtonPressed;
			Helper.Events.Display.MenuChanged += Kitchen.Display_MenuChanged;

            CustomCommunityCentre.Events.Content.LoadingContentPacks += Kitchen.Events_OnLoadingContentPacks;
			CustomCommunityCentre.Events.Game.AreaLoaded += Kitchen.Events_OnLoadedArea;
			CustomCommunityCentre.Events.Game.ResetSharedState += Kitchen.Events_OnResetSharedState;
		}

        internal static void AddConsoleCommands(string cmd)
		{
			// . . .
		}

		internal static void SaveLoadedBehaviours()
		{
			// . . .
		}

		internal static void DayStartedBehaviours()
		{
			// . . .
		}

		private static void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
		{
			Kitchen.LoadEMPValues();
		}

		private static void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			// In-game interactions
			if (!Game1.game1.IsActive || Game1.currentLocation == null || !Context.IsWorldReady)
				return;

			// . . .

			// World interactions
			if (!Context.CanPlayerMove)
				return;

			if (e.Button.IsActionButton())
			{
				// Tile actions
				Tile tile = Game1.currentLocation.Map.GetLayer("Buildings")
					.Tiles[(int)e.Cursor.GrabTile.X, (int)e.Cursor.GrabTile.Y];
				if (tile != null)
				{
					if (Game1.currentLocation is CommunityCenter cc && Kitchen.IsKitchenComplete(Bundles.CC))
					{
						// Use Community Centre kitchen as a cooking station
						if (Kitchen.CookingTileIndexes[Kitchen.FridgeTilesToUse].Contains(tile.TileIndex))
						{
							Kitchen.TryOpenCookingMenuFromKitchenTile(cc: cc, tilePosition: e.Cursor.GrabTile, button: e.Button);

							return;
						}

						// Open Community Centre fridge door
						if (tile.TileIndex == Kitchen.FridgeTileIndexes[Kitchen.FridgeTilesToUse][1])
						{
							// Open the fridge as a chest
							Kitchen.TrySetFridgeDoor(cc: cc, isOpening: true, isUsingChest: true, button: e.Button);

							return;
						}
					}
				}
			}
		}

		private static void Display_MenuChanged(object sender, MenuChangedEventArgs e)
		{
			if (e.OldMenu is TitleMenu || e.NewMenu is TitleMenu || Game1.currentLocation == null || Game1.player == null)
				return;

			bool isCraftingMenu(IClickableMenu menu)
            {
				return menu is ItemGrabMenu
					|| menu is CraftingPage
					|| nameof(menu).EndsWith("CraftingPage", StringComparison.InvariantCultureIgnoreCase)
					|| nameof(menu).EndsWith("CookingMenu", StringComparison.InvariantCultureIgnoreCase);
			}

			// Close Community Centre fridge door after use in the renovated kitchen
			if (Game1.currentLocation is CommunityCenter cc && isCraftingMenu(e.OldMenu) && !isCraftingMenu(e.NewMenu))
			{
				Kitchen.TrySetFridgeDoor(cc: cc, isOpening: false, isUsingChest: false);

				return;
			}
		}

		private static void Events_OnLoadingContentPacks(object sender, CustomCommunityCentre.Events.Content.LoadingContentPacksEventArgs e)
		{
			string path = System.IO.Path.Combine(Helper.DirectoryPath, CommunityKitchen.AssetManager.KitchenContentPackPath);
			e.LoadContentPack(path);
		}

		private static void Events_OnLoadedArea(object sender, CustomCommunityCentre.Events.Game.AreaLoadedEventArgs e)
		{
			if (e.AreaName != Kitchen.KitchenAreaName)
				return;

			Kitchen.SetUpKitchen(e.CommunityCentre);
		}

		private static void Events_OnResetSharedState(object sender, CustomCommunityCentre.Events.Game.ResetSharedStateEventArgs e)
		{
			if (Kitchen.IsKitchenComplete(e.CommunityCentre))
			{
				Kitchen.SetUpKitchen(e.CommunityCentre);
			}
		}

		private static void LoadEMPValues()
		{
			// Load values from Esca's Modding Plugins
			if (Helper.ModRegistry.IsLoaded("Esca.EMP"))
			{
				Type type;

				type = HarmonyLib.AccessTools.TypeByName("EscasModdingPlugins.HarmonyPatch_ActionKitchen");
				Kitchen.EMPTileActionKitchen = (string)type
					?.GetProperty("ActionName", bindingAttr: System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
					?.GetValue(obj: null);

				type = HarmonyLib.AccessTools.TypeByName("EscasModdingPlugins.HarmonyPatch_AllowMiniFridges");
				Kitchen.EMPMapPropertyMinifridges = (string)type
					?.GetProperty("MapPropertyName", bindingAttr: System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
					?.GetValue(obj: null);

				if (Kitchen.EMPTileActionKitchen == null || Kitchen.EMPMapPropertyMinifridges == null)
				{
					Log.E($"Failed to load values from Esca's Modding Plugins. Submit a bug report!");
				}
			}
		}

		public static bool IsKitchenLoaded(CommunityCenter cc)
		{
			if (cc == null)
				return false;

			bool bundlesExist = Game1.netWorldState.Value.BundleData.Keys
				.Any(key => key.Split(Bundles.BundleKeyDelim).First() == Kitchen.KitchenAreaName);
			return bundlesExist;
		}

		public static bool IsKitchenComplete(CommunityCenter cc)
		{
			if (cc == null)
				return false;

			int kitchenNumber = Bundles.GetCustomAreaNumberFromName(Kitchen.KitchenAreaName);

			bool receivedMail = Kitchen.HasOrWillReceiveKitchenCompletedMail();
			bool noCustomAreas = kitchenNumber < 0 || Bundles.AreAnyCustomAreasLoaded();
			bool customAreasComplete = noCustomAreas || Bundles.IsCustomAreaComplete(kitchenNumber);
			bool ccIsComplete = Bundles.IsCommunityCentreCompleteEarly(cc);
			return receivedMail || noCustomAreas || customAreasComplete || ccIsComplete;
		}

		internal static void SetUpKitchen(CommunityCenter cc)
		{
			Helper.Content.InvalidateCache(@"LooseSprites/JunimoNote");
			Helper.Content.InvalidateCache(@"Maps/townInterior");
			Helper.Content.InvalidateCache(@"Strings/Locations");
			Helper.Content.InvalidateCache(@"Strings/UI");

			// Set tiles used for opening/closing fridge
			Kitchen.FridgeTilesToUse = Helper.ModRegistry.IsLoaded("FlashShifter.StardewValleyExpandedCP") ? "SVE" : "Vanilla";
			Kitchen.FridgeTilePosition = Kitchen.GetKitchenFridgeTilePosition(cc);

			// Update kitchen fridge chest
			Kitchen.GetKitchenFridge(cc);

			// Implement Esca's Modding Plugins features if available
			if (Kitchen.EMPMapPropertyMinifridges != null)
			{
				// Allow mini-fridges to be placed in the Community Centre
				cc.Map.Properties[Kitchen.EMPMapPropertyMinifridges] = true;
			}
		}

		public static Chest GetKitchenFridge(CommunityCenter cc)
		{
            // Update fridge chest object if null
            if (!cc.Objects.TryGetValue(Kitchen.FridgeChestPosition, out StardewValley.Object o) || o is not Chest chest)
            {
                chest = new Chest(playerChest: true, tileLocation: Kitchen.FridgeChestPosition, parentSheetIndex: 216);
                chest.fridge.Value = true;
                cc.Objects[Kitchen.FridgeChestPosition] = chest;
            }

			Chest fridge = Kitchen.IsKitchenComplete(cc) ? chest : null;
			return fridge;
		}

		private static Vector2 GetKitchenFridgeTilePosition(CommunityCenter cc)
		{
			int w = cc.Map.GetLayer("Buildings").LayerWidth;
			int h = cc.Map.GetLayer("Buildings").LayerHeight;
			for (int x = 0; x < w; ++x)
			{
				for (int y = 0; y < h; ++y)
				{
					if (cc.Map.GetLayer("Buildings").Tiles[x, y] != null
						&& cc.Map.GetLayer("Buildings").Tiles[x, y].TileIndex == Kitchen.FridgeTileIndexes[Kitchen.FridgeTilesToUse][1])
					{
						return new Vector2(x, y);
					}
				}
			}
			return Vector2.Zero;
		}

		public static bool HasOrWillReceiveKitchenCompletedMail()
		{
			return Game1.MasterPlayer.hasOrWillReceiveMail(string.Format(Bundles.MailAreaCompleted, Kitchen.KitchenAreaName));
		}

		private static void TryOpenCookingMenuFromKitchenTile(CommunityCenter cc, Vector2 tilePosition, SButton button)
		{
			Helper.Input.Suppress(button);
			
			Kitchen.TryOpenCookingMenu(cc: cc, tilePosition: tilePosition, fridge: Kitchen.GetKitchenFridge(cc));
		}

		private static bool TrySetFridgeDoor(CommunityCenter cc, bool isOpening, bool isUsingChest, SButton? button = null)
		{
			if (button != null)
			{
				Helper.Input.Suppress(button.Value);
			}

			Point tilePosition = Utility.Vector2ToPoint(Kitchen.FridgeTilePosition);
            (xTile.Dimensions.Location tileA, xTile.Dimensions.Location tileB) = (
				new (tilePosition.X, tilePosition.Y - 1),
				new (tilePosition.X, tilePosition.Y));
			if (Kitchen.IsKitchenComplete(cc) && Kitchen.FridgeTilePosition != Vector2.Zero)
			{
				// Set fridge tiles to default if fridge door is closing or if fridge chest is not in use
				// Set fridge tiles to alternate (open) if fridge door is open or fridge chest is in use
				int fridgeTiles = !isOpening
					|| (isUsingChest && !((Chest)cc.Objects[Kitchen.FridgeChestPosition]).checkForAction(Game1.player))
					? 0
					: 2;
				if (!isOpening)
                {
					((Chest)cc.Objects[Kitchen.FridgeChestPosition]).mutex.ReleaseLock();
				}
				cc.Map.GetLayer("Front").Tiles[tileA].TileIndex
					= Kitchen.FridgeTileIndexes[Kitchen.FridgeTilesToUse][fridgeTiles];
				cc.Map.GetLayer("Buildings").Tiles[tileB].TileIndex
					= Kitchen.FridgeTileIndexes[Kitchen.FridgeTilesToUse][fridgeTiles + 1];
				return true;
			}
			return false;
		}

		// Taken from StardewValley.Locations.FarmHouse.cs:ActivateKitchen(NetRef<Chest> fridge)
		// Edited to remove netref, mini-fridge and multiple-mutex references
		public static void TryOpenCookingMenu(CommunityCenter cc, Chest fridge, Vector2 tilePosition)
		{
			// Try opening the cooking menu via Esca's Modding Plugins
			xTile.Dimensions.Location location = new ((int)tilePosition.X, (int)tilePosition.Y);
			if (Kitchen.EMPTileActionKitchen != null
				&& cc.performAction(action: Kitchen.EMPTileActionKitchen, who: Game1.player, tileLocation: location))
				return;

			// Try opening a cooking menu using our own logic otherwise
			// Minifridges are ignored as they are only usable with Esca's Modding Plugins
			if (fridge != null && fridge.mutex.IsLocked())
			{
				Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Kitchen_InUse"));
				return;
			}
			fridge?.mutex.RequestLock(
				acquired: delegate
				{
					// Set fridge door visuals
					Kitchen.TrySetFridgeDoor(cc: cc, isOpening: true, isUsingChest: false);

					// Set new crafting page
					Point dimensions = new (
						x: 800 + IClickableMenu.borderWidth * 2,
						y: 600 + IClickableMenu.borderWidth * 2);
					Point position = Utility.Vector2ToPoint(Utility.getTopLeftPositionForCenteringOnScreen(
						width: dimensions.X,
						height: dimensions.Y));
					IClickableMenu menu = null;
					menu = new CraftingPage(
						x: position.X,
						y: position.Y,
						width: dimensions.X,
						height: dimensions.Y,
						cooking: true,
						standalone_menu: true,
						material_containers: new List<Chest> { fridge })
					{
						exitFunction = delegate
                        {
                            fridge.mutex.ReleaseLock();
						}
					};
					Game1.activeClickableMenu = menu;
				},
				failed: delegate
				{
					Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Kitchen_InUse"));
				});
		}
	}
}
