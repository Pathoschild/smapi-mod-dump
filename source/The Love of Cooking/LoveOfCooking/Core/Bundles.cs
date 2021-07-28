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
using PyTK.Extensions;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile;
using xTile.Tiles;

namespace LoveOfCooking
{
	public static class Bundles
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static Config Config => ModEntry.Config;
		private static IReflectionHelper Reflection => ModEntry.Instance.Helper.Reflection;
		private static ITranslationHelper i18n => ModEntry.Instance.Helper.Translation;

		public static readonly string CommunityCentreAreaName = "Kitchen";
		public static readonly int CommunityCentreAreaNumber = 6;
		public static readonly Rectangle CommunityCentreArea = new Rectangle(0, 0, 10, 11);
		public static readonly Point CommunityCentreNotePosition = new Point(7, 6);
		// We use Linus' tent interior for the dummy area, since there's surely no conceivable way it'd be in the community centre
		public static readonly Rectangle FridgeOpenedSpriteArea = new Rectangle(32, 560, 16, 32);
		public static readonly Vector2 FridgeChestPosition = new Vector2(ModEntry.NexusId);
		public static string FridgeTilesToUse = "Vanilla";
		public static readonly Dictionary<string, int[]> FridgeTileIndexes = new Dictionary<string, int[]>
		{
			{ "Vanilla", new [] { 602, 634, 1122, 1154 } },
			{ "SVE", new [] { 432, 440, 432, 442 } }
		};
		public static Vector2 FridgeTilePosition = Vector2.Zero;
		public static int BundleStartIndex;
		public static int BundleCount;
		private static int _menuTab;
		private static int _debugLastCabinsCount;

		internal static void RegisterEvents()
		{
			Helper.Events.GameLoop.Saving += GameLoop_Saving;
			Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
			Helper.Events.Input.ButtonPressed += Input_ButtonPressed;
			Helper.Events.Player.Warped += Player_Warped;
			Helper.Events.Display.MenuChanged += Display_MenuChanged;
		}

		public static void AddConsoleCommands(string cmd)
		{
			Helper.ConsoleCommands.Add(cmd + "printcc", "Print Community Centre bundle states.", (s, args) =>
			{
				CommunityCenter cc = GetCommunityCentre();
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
			Helper.ConsoleCommands.Add(cmd + "listcc", "List all bundle IDs currently loaded.", (s, args) =>
			{
				var bad = Helper.Reflection.GetField
					<Dictionary<int, int>>
					(GetCommunityCentre(), "bundleToAreaDictionary").GetValue();
				string msg = Game1.netWorldState.Value.BundleData.Aggregate("", (str, pair)
					=> $"{str}\n[Area {bad[int.Parse(pair.Key.Split('/')[1])]}] {pair.Key}: {pair.Value.Split('/')[0]} bundle");
				Log.D(msg);
			});
			Helper.ConsoleCommands.Add(cmd + "bundle", "Give items needed for the given bundle.", (s, args) =>
			{
				if (args.Length == 0 || !int.TryParse(args[0], out int bundle) || !Game1.netWorldState.Value.Bundles.ContainsKey(bundle))
				{
					Log.D("No bundle found.");
					return;
				}

				GiveBundleItems(bundle, print: true);
			});
			Helper.ConsoleCommands.Add(cmd + "area", "Give items needed for all bundles in the given area.", (s, args) =>
			{
				var abd = Helper.Reflection.GetField
					<Dictionary<int, List<int>>>
					(GetCommunityCentre(), "areaToBundleDictionary").GetValue();
				if (args.Length == 0 || !int.TryParse(args[0], out int area) || !abd.ContainsKey(area))
				{
					Log.D("No area found.");
					return;
				}

				foreach (int bundle in abd[area])
				{
					if (area == CommunityCentreAreaNumber)
					{
						if (IsAbandonedJojaMartBundleAvailable() && bundle >= BundleStartIndex)
							continue;
						else if (!IsAbandonedJojaMartBundleAvailable() && bundle < BundleStartIndex)
							continue;
					}
					if (GetCommunityCentre().isBundleComplete(bundle))
						continue;
					GiveBundleItems(bundle, print: true);
				}
			});
			Helper.ConsoleCommands.Add(cmd + "bundlereset", "Reset a bundle's saved progress to entirely incomplete.", (s, args) =>
			{
				if (args.Length == 0 || !int.TryParse(args[0], out int bundle)
					|| !Game1.netWorldState.Value.Bundles.ContainsKey(bundle))
				{
					Log.D("No bundle found.");
					return;
				}
				if (IsCommunityCentreComplete())
				{
					Log.D("Too late.");
					return;
				}

				ResetBundleProgress(bundle, print: true);
			});
			Helper.ConsoleCommands.Add(cmd + "areareset", "Reset all bundle progress for an area to entirely incomplete.", (s, args) =>
			{
				var abd = Helper.Reflection.GetField
					<Dictionary<int, List<int>>>
					(GetCommunityCentre(), "areaToBundleDictionary").GetValue();
				if (args.Length == 0 || !int.TryParse(args[0], out int area) || !abd.ContainsKey(area))
				{
					Log.D("No area found.");
					return;
				}
				if (IsCommunityCentreComplete())
				{
					Log.D("Too late.");
					return;
				}

				foreach (int bundle in abd[area])
				{
					ResetBundleProgress(bundle, print: true);
				}
			});
		}

		internal static void SaveLoadedBehaviours()
		{
			FridgeTilesToUse = Interface.Interfaces.UsingSVE ? "SVE" : "Vanilla";

			try
			{
				// Reset per-world config values
				Config savedConfig = Helper.ReadConfig<Config>();
				Config.AddCookingCommunityCentreBundles = savedConfig.AddCookingCommunityCentreBundles;
			}
			catch (Exception e)
			{
				Log.E("" + e);
			}

			CommunityCenter cc = GetCommunityCentre();

			// Check for sending warnings re: multiplayer and bundles
			_debugLastCabinsCount = GetNumberOfCabinsBuilt();

			// Handle custom bundle data unloading and loading
			Dictionary<int, string> customBundleData = ParseBundleData();
			BundleCount = customBundleData.Count;
			Log.D($"Bundles identified: [{BundleCount}]: {string.Join(", ", customBundleData.Keys)}",
				Config.DebugMode);
			
			// Fetch tile position for opening/closing fridge visually
			FridgeTilePosition = FindCommunityCentreFridge();
			// Add community centre kitchen fridge container to the map for later
			if (!cc.Objects.ContainsKey(FridgeChestPosition))
			{
				cc.Objects.Add(FridgeChestPosition, new Chest(true, FridgeChestPosition));
			}
			
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
						NotificationMenu.AddNewPendingNotification(NotificationMenu.Notification.BundleMultiplayerWarning);
					}
				}
				else
				{
					Log.D("Loading first-time world bundle data for multiplayer peer.",
						Config.DebugMode);
					LoadBundleData();
				}
			}
		}

		private static void GameLoop_Saving(object sender, SavingEventArgs e)
		{
			// Save local (and/or persistent) community centre data
			Log.D("Unloading world bundle data at end of day.",
				Config.DebugMode);
			SaveAndUnloadBundleData();
		}

		private static void Display_MenuChanged(object sender, MenuChangedEventArgs e)
		{
			if (e.OldMenu is TitleMenu || e.NewMenu is TitleMenu || Game1.currentLocation == null || Game1.player == null)
				return;

			if (e.OldMenu is JunimoNoteMenu junimoNoteMenu && e.NewMenu == null && !IsCommunityCentreComplete())
			{
				// Counteract the silly check for (whichArea == 6) in JunimoNoteMenu.setUpMenu(whichArea, bundlesComplete)
				foreach (Farmer player in Game1.getAllFarmers())
				{
					string[] mailToRemove = new[]
					{
						"abandonedJojaMartAccessible", "hasSeenAbandonedJunimoNote", "ccMovieTheater", "ccMovieTheater%&NL&%", "ccMovieTheaterJoja", "ccMovieTheaterJoja%&NL&%"
					};
					foreach (string mail in mailToRemove)
					{
						if (player.mailForTomorrow.Contains(mail))
						{
							player.mailForTomorrow.Remove(mail);
							Log.D($"Removed premature mail {mail} from tomorrow's mail for {player.Name}.",
								Config.DebugMode);
						}
						if (player.mailReceived.Contains(mail))
						{
							player.mailReceived.Remove(mail);
							Log.D($"Removed premature mail {mail} from received mail for {player.Name}.",
								Config.DebugMode);
						}
					}
				}

				// Play kitchen area complete cutscene on closing the completed junimo note menu, rather than after re-entering and closing it
				CommunityCenter cc = GetCommunityCentre();
				int area = Reflection.GetField
					<int>
					(junimoNoteMenu, "whichArea")
					.GetValue();
				if (area == CommunityCentreAreaNumber && cc.bundles.Keys.Where(key => key >= BundleStartIndex).All(key => cc.bundles[key].All(value => value)))
				{
					cc.restoreAreaCutscene(CommunityCentreAreaNumber);
				}
			}

			// Check to send multiplayer bundle warning mail when building new cabins
			if (IsCommunityCentreKitchenEnabledByHost())
			{
				int currentCabinsCount = GetNumberOfCabinsBuilt();
				if (e.OldMenu is CarpenterMenu && currentCabinsCount > _debugLastCabinsCount)
				{
					_debugLastCabinsCount = currentCabinsCount;
					NotificationMenu.AddNewPendingNotification(NotificationMenu.Notification.CabinBuiltWarning);
				}
			}

			// Close Community Centre fridge door after use in the renovated kitchen
			if (e.OldMenu is ItemGrabMenu && e.NewMenu == null
				&& Game1.currentLocation is CommunityCenter cc1
				&& (IsCommunityCentreComplete() || (cc1.areasComplete.Count > CommunityCentreAreaNumber && cc1.areasComplete[CommunityCentreAreaNumber]))
				&& FridgeTilePosition != Vector2.Zero
				&& cc1.Map.GetLayer("Front").Tiles[(int)FridgeTilePosition.X, (int)FridgeTilePosition.Y - 1] is Tile tileA
				&& cc1.Map.GetLayer("Buildings").Tiles[(int)FridgeTilePosition.X, (int)FridgeTilePosition.Y] is Tile tileB
				&& tileA != null && tileB != null)
			{
				cc1.Map.GetLayer("Front")
					.Tiles[(int)FridgeTilePosition.X, (int)FridgeTilePosition.Y - 1]
					.TileIndex = FridgeTileIndexes[FridgeTilesToUse][0];
				cc1.Map.GetLayer("Buildings")
					.Tiles[(int)FridgeTilePosition.X, (int)FridgeTilePosition.Y]
					.TileIndex = FridgeTileIndexes[FridgeTilesToUse][1];
				return;
			}
		}

		private static void Player_Warped(object sender, WarpedEventArgs e)
		{
			if ((!(e.NewLocation is CommunityCenter) && e.OldLocation is CommunityCenter)
				|| (!(e.OldLocation is CommunityCenter) && e.NewLocation is CommunityCenter))
			{
				Helper.Content.InvalidateCache(@"Maps/townInterior");
			}

			if (e.NewLocation is CommunityCenter cc)
			{
				if (IsCommunityCentreKitchenEnabledByHost())
				{
					Helper.Events.GameLoop.UpdateTicked += Event_MoveJunimo; // fgs fds
					Log.D($"Warped to CC: areasComplete count: {cc.areasComplete.Count}, complete: {IsCommunityCentreComplete()}",
						Config.DebugMode);

					if (GetCommunityCentre().areAllAreasComplete())
					{
						DrawStarInCommunityCentre(cc);
					}
					else
					{
						CheckAndTryToUnrenovateKitchen();
					}
				}
			}
		}

		private static void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			if (!Game1.game1.IsActive || Game1.currentLocation == null)
			{
				return;
			}

			// Menu interactions
			if (e.Button.IsUseToolButton())
			{
				// Navigate community centre bundles inventory menu
				if (Game1.activeClickableMenu is JunimoNoteMenu menu && menu != null
					&& IsCommunityCentreKitchenEnabledByHost()
					&& !IsCommunityCentreKitchenComplete()
					&& GetCommunityCentre() is CommunityCenter cc
					&& cc.shouldNoteAppearInArea(CommunityCentreAreaNumber))
				{
					if (!Game1.player.hasOrWillReceiveMail("canReadJunimoText"))
					{
						Game1.activeClickableMenu.exitThisMenu();
						return;
					}

					Point cursor = Utility.Vector2ToPoint(e.Cursor.ScreenPixels);
					NavigateJunimoNoteMenuAroundKitchen(cc, menu, cursor.X, cursor.Y);
				}
			}

			// World interactions
			if (Utils.PlayerAgencyLostCheck()
				|| Game1.currentBillboard != 0 || Game1.activeClickableMenu != null || Game1.menuUp // No menus
				|| !Game1.player.CanMove) // Player agency enabled
			{
				return;
			}

			if (e.Button.IsActionButton())
			{
				// Tile actions
				Tile tile = Game1.currentLocation.Map.GetLayer("Buildings")
					.Tiles[(int)e.Cursor.GrabTile.X, (int)e.Cursor.GrabTile.Y];

				// Open Community Centre fridge door
				if (Game1.currentLocation is CommunityCenter cc && IsCommunityCentreKitchenComplete()
					&& tile != null && tile.TileIndex == FridgeTileIndexes[FridgeTilesToUse][1])
				{
					// Change tile to use custom open-fridge sprite
					Game1.currentLocation.Map.GetLayer("Front")
						.Tiles[(int)FridgeTilePosition.X, (int)FridgeTilePosition.Y - 1]
						.TileIndex = FridgeTileIndexes[FridgeTilesToUse][2];
					Game1.currentLocation.Map.GetLayer("Buildings")
						.Tiles[(int)FridgeTilePosition.X, (int)FridgeTilePosition.Y]
						.TileIndex = FridgeTileIndexes[FridgeTilesToUse][3];

					// Open the fridge as a chest
					((Chest)cc.Objects[FridgeChestPosition]).fridge.Value = true;
					((Chest)cc.Objects[FridgeChestPosition]).checkForAction(Game1.player);

					Helper.Input.Suppress(e.Button);
				}
			}
		}

		private static void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
		{
			// TODO: UPDATE: Send followup mail when the kitchen bundle is completed
			if (ModEntry.SendBundleFollowupMail && IsCommunityCentreKitchenComplete() && Game1.MasterPlayer.hasOrWillReceiveMail(ModEntry.MailKitchenCompleted))
			{
				Game1.addMailForTomorrow(ModEntry.MailKitchenCompletedFollowup);
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
		}

		public static CommunityCenter GetCommunityCentre()
		{
			return Game1.getLocationFromName("CommunityCenter") as CommunityCenter;
		}

		public static bool IsCommunityCentreComplete()
		{
			CommunityCenter cc = GetCommunityCentre();
			if (cc == null)
				return false;

			bool masterPlayerComplete = Game1.MasterPlayer.hasCompletedCommunityCenter();
			bool cutsceneSeen = Utility.HasAnyPlayerSeenEvent(191393);
			return masterPlayerComplete || cutsceneSeen;
		}

		public static bool IsCommunityCentreKitchenEnabledByHost()
		{
			CommunityCenter cc = GetCommunityCentre();
			if (cc == null)
				return false;

			bool hostEnabled = Game1.IsMasterGame && Config.AddCookingCommunityCentreBundles;
			bool bundlesExist = Game1.netWorldState.Value.Bundles.Keys.Any(key => key > BundleStartIndex);
			bool areasCompleteEntriesExist = cc.areasComplete.Count > CommunityCentreAreaNumber;
			bool clientEnabled = !Game1.IsMasterGame && (bundlesExist || areasCompleteEntriesExist);
			return hostEnabled || clientEnabled;
		}

		public static bool IsCommunityCentreKitchenComplete()
		{
			CommunityCenter cc = GetCommunityCentre();
			if (cc == null)
				return false;

			bool receivedMail = Game1.MasterPlayer != null && Game1.MasterPlayer.hasOrWillReceiveMail(ModEntry.MailKitchenCompleted);
			bool missingAreasCompleteEntries = cc.areasComplete.Count <= CommunityCentreAreaNumber;
			bool areaIsComplete = missingAreasCompleteEntries || cc.areasComplete[CommunityCentreAreaNumber];
			bool ccIsComplete = IsCommunityCentreComplete();
			Log.T($"IsCommunityCentreKitchenCompleted: (mail: {receivedMail}), (entries: {missingAreasCompleteEntries}) || (areas: {areaIsComplete})");
			return receivedMail || missingAreasCompleteEntries || areaIsComplete || ccIsComplete;
		}

		public static bool IsAbandonedJojaMartBundleAvailable()
		{
			return Game1.MasterPlayer != null && !Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater")
				&& Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("abandonedJojaMartAccessible");
		}

		/// <summary>
		/// While the Pantry (area 0) is completed, CommunityCenter.loadArea(0) will patch over the kitchen with a renovated map.
		/// This method undoes the renovated map patch by patching over it again with the ruined map.
		/// </summary>
		internal static void CheckAndTryToUnrenovateKitchen()
		{
			Log.D($"Checking to unrenovate area for kitchen",
				Config.DebugMode);

			CommunityCenter cc = GetCommunityCentre();
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
			bool c1 = cc.isJunimoNoteAtArea(CommunityCentreAreaNumber);
			bool c2 = cc.shouldNoteAppearInArea(CommunityCentreAreaNumber);
			if (!c1 && c2)
			{
				Log.D("Adding junimo note manually",
					Config.DebugMode);
				cc.addJunimoNote(CommunityCentreAreaNumber);
			}
		}

		internal static void SetCommunityCentreKitchenForThisSession(bool isEnabled)
		{
			Config.AddCookingCommunityCentreBundles = isEnabled;
			Helper.Content.InvalidateCache(@"LooseSprites/JunimoNote");
			Helper.Content.InvalidateCache(@"Maps/townInterior");
			Helper.Content.InvalidateCache(@"Strings/Locations");
			Helper.Content.InvalidateCache(@"Strings/UI");
		}

		public static Chest GetCommunityCentreFridge()
		{
			CommunityCenter cc = GetCommunityCentre();
			Chest fridge = IsCommunityCentreKitchenComplete()
					? cc.Objects.ContainsKey(FridgeChestPosition)
						? (Chest)cc.Objects[FridgeChestPosition]
						: null
					: null;
			return fridge;
		}

		private static Vector2 FindCommunityCentreFridge()
		{
			CommunityCenter cc = GetCommunityCentre();
			int w = cc.Map.GetLayer("Buildings").LayerWidth;
			int h = cc.Map.GetLayer("Buildings").LayerHeight;
			for (int x = 0; x < w; ++x)
			{
				for (int y = 0; y < h; ++y)
				{
					if (cc.Map.GetLayer("Buildings").Tiles[x, y] != null
						&& cc.Map.GetLayer("Buildings").Tiles[x, y].TileIndex == FridgeTileIndexes[FridgeTilesToUse][1])
					{
						return new Vector2(x, y);
					}
				}
			}
			return Vector2.Zero;
		}

		internal static void DrawStarInCommunityCentre(CommunityCenter cc)
		{
			const int id = ModEntry.NexusId + 5742;
			if (cc.getTemporarySpriteByID(id) != null)
				return;

			Multiplayer multiplayer = Reflection.GetField
				<Multiplayer>
				(typeof(Game1), "multiplayer").GetValue();
			multiplayer.broadcastSprites(cc,
				new TemporaryAnimatedSprite(
					"LooseSprites\\Cursors",
					new Rectangle(354, 401, 7, 7),
					9999, 1, 9999,
					new Vector2(2096f, 344f),
					false, false, 0.8f, 0f, Color.White,
					4f, 0f, 0f, 0f)
				{
					id = id,
					holdLastFrame = true
				});
		}

		internal static void NavigateJunimoNoteMenuAroundKitchen(CommunityCenter cc, JunimoNoteMenu menu, int x, int y)
		{
			int whichArea = Reflection.GetField
				<int>
				(menu, "whichArea").GetValue();
			int lowestArea = -1;
			int highestArea = -1;
			_menuTab = -1;

			// Fetch the bounds of the menu, exclusive of our new area since we're assuming we start there
			// Exclude any already-completed areas from this search, since we're looking for unfinished areas
			for (int i = CommunityCentreAreaNumber - 1; i >= 0; --i)
			{
				if (cc.areasComplete[i] || !cc.shouldNoteAppearInArea(i))
					continue;

				if (highestArea < 0)
					highestArea = i;
				else
					lowestArea = i;
			}

			bool backButton = menu.areaBackButton != null && menu.areaBackButton.visible && menu.areaBackButton.containsPoint(x, y);
			bool nextButton = menu.areaNextButton != null && menu.areaNextButton.visible && menu.areaNextButton.containsPoint(x, y);
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

		private static void GiveBundleItems(int whichBundle, bool print)
		{
			KeyValuePair<string, string> bundle = Game1.netWorldState.Value.BundleData
				.FirstOrDefault(pair => pair.Key.Split('/')[1] == whichBundle.ToString());
			string[] split = bundle.Value.Split('/');
			string[] itemData = split[2].Split(' ');
			int itemLimit = split.Length < 5 ? 99 : int.Parse(split[4]);
			for (int i = 0; i < itemData.Length && i < itemLimit * 3; ++i)
			{
				int index = int.Parse(itemData[i]);
				int quantity = int.Parse(itemData[++i]);
				int quality = int.Parse(itemData[++i]);
				if (index == -1)
					Game1.player.addUnearnedMoney(quantity);
				else
					Utils.AddOrDropItem(new StardewValley.Object(index, quantity, isRecipe: false, price: -1, quality: quality));
			}
			Log.D($"Giving items for {bundle.Key}: {bundle.Value.Split('/')[0]} bundle.",
				print);
		}

		private static void ResetBundleProgress(int whichBundle, bool print)
		{
			Dictionary<int, int> bad = Reflection.GetField
				<Dictionary<int, int>>
				(GetCommunityCentre(), "bundleToAreaDictionary")
				.GetValue();
			CommunityCenter cc = GetCommunityCentre();

			cc.bundleRewards[whichBundle] = false;
			cc.bundles[whichBundle] = new bool[cc.bundles[whichBundle].Length];
			Game1.netWorldState.Value.BundleRewards[whichBundle] = false;
			Game1.netWorldState.Value.Bundles[whichBundle] = new bool[Game1.netWorldState.Value.Bundles[whichBundle].Length];

			if (cc.areasComplete[bad[whichBundle]])
			{
				cc.areasComplete[bad[whichBundle]] = false;
				cc.loadMap(cc.Map.assetPath, force_reload: true);
			}

			KeyValuePair<string, string> bundle = Game1.netWorldState.Value.BundleData
				.FirstOrDefault(pair => pair.Key.Split('/')[1] == whichBundle.ToString());
			Log.D($"Reset progress for {bundle.Key}: {bundle.Value.Split('/')[0]} bundle.",
				print);
		}

		internal static void ReloadBundleData()
		{
			Log.D("CACB Reloading custom bundle data",
				Config.DebugMode);
			SaveAndUnloadBundleData();
			LoadBundleData();
		}

		internal static void LoadBundleData()
		{
			CommunityCenter cc = GetCommunityCentre();
			Dictionary<int, string> customBundleData = ParseBundleData();

			Log.D(customBundleData.Aggregate("CACB customBundleData: ", (s, pair) => $"{s}\n{pair.Key}: {pair.Value}"),
				Config.DebugMode);

			// Load custom bundle data from persistent world data if it exists, else default to false (bundles not yet started by player)
			var customBundleValues = new Dictionary<int, bool[]>();
			var customBundleRewards = new Dictionary<int, bool>();
			bool isAreaComplete = cc.modData.ContainsKey(ModEntry.AssetPrefix + "area_completed")
				&& !string.IsNullOrEmpty(cc.modData[ModEntry.AssetPrefix + "area_completed"])
				? bool.Parse(cc.modData[ModEntry.AssetPrefix + "area_completed"])
				: false;

			for (int i = 0; i < BundleCount; ++i)
			{
				// Bundle metadata, not synced by multiplayer.broadcastWorldState
				int key = BundleStartIndex + i;
				Game1.netWorldState.Value.BundleData[$"{CommunityCentreAreaName}/{key}"] = customBundleData[key];
			}

			if (Game1.IsMasterGame)
			{
				// Add GW custom bundle data
				for (int i = 0; i < BundleCount; ++i)
				{
					int key = BundleStartIndex + i;
					// Bundle progress
					string dataKey = ModEntry.AssetPrefix + "bundle_values_" + i;
					customBundleValues.Add(key, cc.modData.ContainsKey(dataKey)
						&& !string.IsNullOrEmpty(cc.modData[dataKey])
						? cc.modData[dataKey].Split(',').ToList().ConvertAll(bool.Parse).ToArray()
						: new bool[customBundleData[key].Split('/')[2].Split(' ').Length]);

					// Bundle saved rewards
					dataKey = ModEntry.AssetPrefix + "bundle_rewards_" + i;
					customBundleRewards.Add(key, cc.modData.ContainsKey(dataKey)
						&& !string.IsNullOrEmpty(cc.modData[dataKey])
						? bool.Parse(cc.modData[dataKey])
						: false);

					string msg = $"CACB Added custom bundle value ({key} [{customBundleRewards[key]}]: ";
					Log.D(customBundleValues[key].Aggregate(msg, (str, value) => $"{str} {value}") + ")",
						Config.DebugMode);
				}

				// Regular load-in for custom bundles
				if (IsCommunityCentreKitchenEnabledByHost() && !IsCommunityCentreComplete())
				{
					// Reload custom bundle data to game savedata
					// World state cannot be added to: it has an expected length once set
					var bundles = new Dictionary<int, bool[]>();
					var bundleRewards = new Dictionary<int, bool>();
					// TODO: BUNDLES: NetArray.SetCount()
					// Fetch vanilla GW bundle data
					for (int i = 0; i < BundleStartIndex; ++i)
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

					Multiplayer multiplayer = Reflection.GetField
						<Multiplayer>
						(typeof(Game1), "multiplayer")
						.GetValue();
					multiplayer.broadcastWorldStateDeltas();
					multiplayer.broadcastLocationDelta(GetCommunityCentre());

					Log.D($"CACB Loaded GW bundle data progress and broadcasted world state.",
						Config.DebugMode);
				}
				else
				{
					Log.D("CACB Did not load GW custom bundle data, kitchen disabled or completed.",
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
					NetArray<bool, NetBool> oldAreas = cc.areasComplete;
					var newAreas = new NetArray<bool, NetBool>(CommunityCentreAreaNumber + 1);
					for (int i = 0; i < oldAreas.Count; ++i)
						newAreas[i] = oldAreas[i];
					newAreas[newAreas.Length - 1] = Game1.MasterPlayer.hasOrWillReceiveMail(ModEntry.MailKitchenCompleted);
					cc.areasComplete.Clear();
					cc.areasComplete.Set(newAreas);
				}

				var badField = Reflection.GetField
					<Dictionary<int, int>>
					(cc, "bundleToAreaDictionary");
				var bad = badField.GetValue();
				if (customBundleData.Keys.Any(key => !bad.ContainsKey(key)))
				{
					Log.D("CACB Adding new data to CC bundle-area dictionary.",
						Config.DebugMode);

					// Add a reference to the new community centre kitchen area to the reference dictionary
					for (int i = 0; i < BundleCount; ++i)
					{
						bad[BundleStartIndex + i] = CommunityCentreAreaNumber;
					}
					badField.SetValue(bad);
				}

				var abdField = Reflection.GetField
					<Dictionary<int, List<int>>>
					(cc, "areaToBundleDictionary");
				var abd = abdField.GetValue();
				if (customBundleData.Keys.Any(key => !abd[CommunityCentreAreaNumber].Contains(key)))
				{
					Log.D("CACB Adding new data to CC area-bundle dictionary.",
						Config.DebugMode);

					// Add references to the new community centre bundles to the reference dictionary
					foreach (int bundle in customBundleData.Keys)
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

		internal static void SaveAndUnloadBundleData()
		{
			CommunityCenter cc = GetCommunityCentre();
			if (Game1.IsMasterGame)
			{
				// Save GW bundle data to persistent data in community centre
				if (Game1.netWorldState.Value != null
					&& Game1.netWorldState.Value.Bundles != null
					&& Game1.netWorldState.Value.BundleRewards != null)
				{
					for (int i = 0; i < Math.Max(3, BundleCount); ++i)
					{
						int key = Math.Max(37, BundleStartIndex) + i;

						if (Game1.netWorldState.Value.Bundles.ContainsKey(key))
						{
							cc.modData[ModEntry.AssetPrefix + "bundle_values_" + i]
								= string.Join(",", Game1.netWorldState.Value.Bundles[key]);
						}
						if (Game1.netWorldState.Value.BundleRewards.ContainsKey(key))
						{
							cc.modData[ModEntry.AssetPrefix + "bundle_rewards_" + i]
								= Game1.netWorldState.Value.BundleRewards[key].ToString();
						}

						// Remove custom bundle data from GW data to avoid failed save loading under various circumstances
						Game1.netWorldState.Value.Bundles.Remove(key);
						Game1.netWorldState.Value.BundleRewards.Remove(key);
						Game1.netWorldState.Value.BundleData.Remove($"{CommunityCentreAreaName}/{key}");

						Log.D($"CACB Saved and unloaded bundle progress for {key}",
							Config.DebugMode);
					}
				}
				cc.modData[ModEntry.AssetPrefix + "area_completed"] = IsCommunityCentreKitchenComplete().ToString();
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
					Reflection.GetMethod(cc, "initAreaBundleConversions").Invoke();

					// Remove new areasComplete entry
					NetArray<bool, NetBool> oldAreas = cc.areasComplete;
					var newAreas = new NetArray<bool, NetBool>(CommunityCentreAreaNumber);
					for (int i = 0; i < newAreas.Count; ++i)
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

		internal static void PrintBundleData(CommunityCenter cc)
		{
			// aauugh

			// Community centre data (LOCAL)
			var bad = Reflection.GetField
				<Dictionary<int, int>>
				(cc, "bundleToAreaDictionary")
				.GetValue();
			var abd = Reflection.GetField
				<Dictionary<int, List<int>>>
				(cc, "areaToBundleDictionary")
				.GetValue();

			Log.D($"StartIndex: {BundleStartIndex}, Count: {BundleCount}", Config.DebugMode);
			Log.D($"CACB Multiplayer: ({Game1.IsMultiplayer}-{IsMultiplayer()}), Host game: ({Game1.IsMasterGame}), Host player: ({Game1.MasterPlayer.UniqueMultiplayerID == Game1.player.UniqueMultiplayerID})", Config.DebugMode);
			Log.D($"CACB CC IsCommunityCentreComplete: {IsCommunityCentreComplete()}", Config.DebugMode);
			Log.D($"CACB CC IsKitchenEnabledByHost:  {IsCommunityCentreKitchenEnabledByHost()}", Config.DebugMode);
			Log.D($"CACB CC IsKitchenComplete:  {IsCommunityCentreKitchenComplete()}", Config.DebugMode);
			Log.D(cc.areasComplete.Aggregate($"CACB CC areasComplete[{cc.areasComplete.Count}]:    ", (s, b) => $"{s} ({b})"), Config.DebugMode);
			Log.D(bad.Aggregate($"CACB CC bundleToAreaDictionary[{bad.Count}]:", (s, pair) => $"{s} ({pair.Key}: {pair.Value})"), Config.DebugMode);
			Log.D(abd.Aggregate($"CACB CC areaToBundleDictionary[{abd.Count}]:", (s, pair) => $"{s} ({pair.Key}: {pair.Value.Aggregate("", (s1, value) => s1 + " " + value)})"), Config.DebugMode);
			Log.D($"CACB CC NumOfAreasComplete:        {Reflection.GetMethod(cc, "getNumberOfAreasComplete").Invoke<int>()}", Config.DebugMode);

			// World state data (SYNCHRONISED)
			Log.D(Game1.netWorldState.Value.BundleData.Aggregate("CACB GW bundleData: ", (s, pair)
				=> $"{s}\n{pair.Key}: {pair.Value}"), Config.DebugMode);
			Log.D(Game1.netWorldState.Value.Bundles.Aggregate("CACB GW bundles: ", (s, boolses)
				=> boolses?.Count > 0 ? $"{s}\n{boolses.Aggregate("", (s1, pair) => $"{s1}\n{pair.Key}: {pair.Value.Aggregate("", (s2, complete) => $"{s2} {complete}")}")}" : "none"), Config.DebugMode);
			Log.D(Game1.netWorldState.Value.BundleRewards.Aggregate("CACB GW bundleRewards: ", (s, boolses)
				=> boolses?.Count > 0 ? $"{s}\n{boolses.Aggregate("", (s1, pair) => $"{s1} ({pair.Key}: {pair.Value})")}" : "(none)"), Config.DebugMode);
		}

		internal static Dictionary<int, string> ParseBundleData()
		{
			var sourceBundleList = Game1.content.Load
				<Dictionary<string, Dictionary<string, List<string>>>>
				(AssetManager.GameContentBundleDataPath);
			string whichBundleList = Interface.Interfaces.JsonAssets == null
				? "Vanilla"
				: Interface.Interfaces.UsingPPJACrops
					? "PPJA"
					: Config.AddNewCropsAndStuff
						? "Custom"
						: "Vanilla";
			Dictionary<string, List<string>> sourceBundles = sourceBundleList[whichBundleList];
			var newData = new Dictionary<int, string>();

			// Iterate over each custom bundle to add their data to game Bundles dictionary
			int index = 0;
			List<string> keys = sourceBundles.Keys.ToList();
			keys.Sort();
			foreach (string key in keys)
			{
				// Bundle data
				var parsedBundle = new List<List<string>>();

				// Parse the bundle metadata
				Translation displayName = i18n.Get($"world.community_centre.bundle.{index + 1}");
				string itemsToComplete = sourceBundles[key][2];
				string colour = sourceBundles[key][3];
				parsedBundle.Add(new List<string> { key });

				// Fill in rewardsData section of the new bundle data
				string[] rewardsData = sourceBundles[key][0].Split(' ');
				string rewardName = string.Join(" ", rewardsData.Skip(1).Take(rewardsData.Length - 2));
				int rewardId = Interface.Interfaces.JsonAssets.GetObjectId(rewardName);
				if (rewardId < 0)
				{
					rewardId = rewardsData[0] == "BO"
						? Game1.bigCraftablesInformation.FirstOrDefault(o => o.Value.Split('/')[0] == rewardName).Key
						: Game1.objectInformation.FirstOrDefault(o => o.Value.Split('/')[0] == rewardName).Key;
				}

				// Add parsed rewards
				List<string> parsedRewards = new List<string> 
					{ rewardsData[0], rewardId.ToString(), rewardsData[rewardsData.Length - 1] };
				parsedBundle.Add(parsedRewards);

				// Parse and add item requirements for this bundle
				List<int> parsedItems = ParseBundleItems(sourceBundles[key][1].Split(' '));
				parsedBundle.Add(parsedItems.ConvertAll(o => o.ToString()));

				// Patch new data into the target bundle dictionary, including mininmum completion count and display name
				string value = string.Join("/",
						parsedBundle
							.Select(list => string.Join(" ", list)))
					+ $"/{colour}/{itemsToComplete}";
				if (LocalizedContentManager.CurrentLanguageCode.ToString() != "en")
				{
					value += $"/{displayName}";
				}
				newData.Add(BundleStartIndex + index, value);
				++index;
			}
			return newData;
		}

		internal static List<int> ParseBundleItems(string[] sourceItems)
		{
			var parsedItems = new List<int>();

			// Iterate over each word in the items list, formatted as [<Name With Spaces> <Quantity> <Quality>]
			int startIndex = 0;
			for (int j = 0; j < sourceItems.Length; ++j)
			{
				// Group and parse each [name quantity quality] cluster
				if (j != startIndex && int.TryParse(sourceItems[j], out int itemQuantity))
				{
					string itemName = string.Join(" ", sourceItems.Skip(startIndex).Take(j - startIndex).ToArray());
					int itemQuality = int.Parse(sourceItems[++j]);
					int itemId = Interface.Interfaces.JsonAssets.GetObjectId(itemName);

					// Add parsed item data to the requiredItems section of the new bundle data
					if (itemId < 0)
					{
						itemId = Game1.objectInformation.FirstOrDefault(o => o.Value.Split('/')[0] == itemName).Key;
					}
					if (itemId > 0)
					{
						parsedItems.AddRange(new List<int> { itemId, itemQuantity, itemQuality });
					}

					startIndex = ++j;
				}
			}

			return parsedItems;
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

		private static void Event_MoveJunimo(object sender, UpdateTickedEventArgs e)
		{
			Helper.Events.GameLoop.UpdateTicked -= Event_MoveJunimo;
			CommunityCenter cc = GetCommunityCentre();
			Point p = CommunityCentreNotePosition;
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

		private static void Event_ChangeJunimoMenuTab(object sender, UpdateTickedEventArgs e)
		{
			Helper.Events.GameLoop.UpdateTicked -= Event_ChangeJunimoMenuTab;
			Reflection.GetField<int>((JunimoNoteMenu)Game1.activeClickableMenu, "whichArea").SetValue(_menuTab);
			if (_menuTab == CommunityCentreAreaNumber)
			{
				((JunimoNoteMenu)Game1.activeClickableMenu).bundles.Clear();
				((JunimoNoteMenu)Game1.activeClickableMenu).setUpMenu(CommunityCentreAreaNumber, GetCommunityCentre().bundlesDict());
			}
		}
	}
}
