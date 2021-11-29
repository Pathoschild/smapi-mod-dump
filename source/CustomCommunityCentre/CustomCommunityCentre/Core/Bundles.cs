/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CustomCommunityCentre
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomCommunityCentre
{
	public static class Bundles
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static IReflectionHelper Reflection => ModEntry.Instance.Helper.Reflection;
		private static Config Config => ModEntry.Config;
		private static CommunityCenter _cc;
		public static CommunityCenter CC => Context.IsWorldReady
			? _cc ??= Game1.getLocationFromName(nameof(CommunityCenter)) as CommunityCenter
			: _cc;

		// Kitchen areas and bundles
		public static int DefaultMaxArea;
		public static int DefaultMaxBundle;
		public static int CustomAreaInitialIndex;
		public static int CustomBundleInitialIndex;

		public static int DefaultAreaCount => Bundles.CC.areasComplete.Count;
		public static int CustomAreaCount => Bundles.CustomAreasComplete.Count;
		public static int TotalAreaCount => Bundles.DefaultAreaCount + Bundles.CustomAreaCount;
		public static int TotalBundleCount => Bundles.CC.bundles.Count();
		public static int DefaultAreasCompleteCount => Bundles.CC.areasComplete.Count(isComplete => isComplete);
		public static int CustomAreasCompleteCount => Bundles.CustomAreasComplete.Values.Count(isComplete => isComplete);
		public static int TotalAreasCompleteCount => Bundles.DefaultAreasCompleteCount + Bundles.CustomAreasCompleteCount;

		public static Dictionary<string, int> CustomAreaNamesAndNumbers = new();
		public static Dictionary<string, string[]> CustomAreaBundleKeys = new();
		public static Dictionary<int, bool> CustomAreasComplete = new();
		public static List<Item> CustomBundleDonations = new();

		public static readonly Point CustomBundleDonationsChestTile = new(32, 9);
		public static readonly Vector2 CustomAreaStarPosition = new(2096f, 344f);

		public const char BundleKeyDelim = '/';
		public const char ModDataKeyDelim = '/';
		public const char ModDataValueDelim = ':';

		// Mail
		// Ensure all Mail values are referenced with string.Format(pattern, areaName) where appropriate
		public const string MailAreaCompleted = "cc{0}";
		public static string MailAreaCompletedFollowup = "cc{0}_completed_followup";
		public static string MailAreaLastBundleCompleteRewardDelivery = "cc{0}_reward_guarantee";
		public static string ActiveDialogueEventAreaCompleted = "cc_{0}";
		public const int ActiveDialogueEventAreaCompletedDuration = 7;

		// Events
		internal enum EventIds
		{
			CommunityCentreUnlocked = 611439,
			CommunityCentreComplete = 191393,
			JojaWarehouseComplete = 502261,
			AbandonedJojaMartComplete = 192393
		}

		// ModData keys
		internal static string KeyAreasComplete => AssetManager.PrefixAsset(asset: "AreasComplete");
		internal static string KeyBundleRewards => AssetManager.PrefixAsset(asset: "BundleRewards");
		internal static string KeyMutexes => AssetManager.PrefixAsset(asset: "Mutexes");


		internal static void RegisterEvents()
		{
			Helper.Events.Multiplayer.PeerConnected += Bundles.Multiplayer_PeerConnected;
			Helper.Events.Display.MenuChanged += Bundles.Display_MenuChanged;
			Helper.Events.Display.RenderedWorld += Bundles.Display_RenderedWorld;
			Helper.Events.Player.Warped += Bundles.Player_Warped;
			Helper.Events.Input.ButtonPressed += Bundles.Input_ButtonPressed;
		}

		internal static void AddConsoleCommands(string cmd)
		{
			Helper.ConsoleCommands.Add(cmd + "print", "Print Community Centre bundle states.", (s, args) =>
			{
				Bundles.Print(Bundles.CC);
			});

			Helper.ConsoleCommands.Add(cmd + "list", "List all bundle IDs currently loaded.", (s, args) =>
			{
				Dictionary<int, int> bundleAreaDict = Helper.Reflection.GetField
					<Dictionary<int, int>>
					(Bundles.CC, "bundleToAreaDictionary").GetValue();
				string msg = string.Join(Environment.NewLine,
					Game1.netWorldState.Value.BundleData.Select(
						pair => $"[Area {bundleAreaDict[int.Parse(pair.Key.Split(Bundles.BundleKeyDelim).Last())]}] {pair.Key}: {pair.Value.Split(Bundles.BundleKeyDelim).First()}"));
				Log.D(msg);
			});

			Helper.ConsoleCommands.Add(cmd + "bundle", "Give items needed for the given bundle.", (s, args) =>
			{
				if (args.Length == 0 || !int.TryParse(args[0], out int bundle) || !Game1.netWorldState.Value.Bundles.ContainsKey(bundle))
				{
					Log.D("No bundle found.");
					return;
				}

				Bundles.GiveBundleItems(cc: Bundles.CC, bundle, print: true);
			});

			Helper.ConsoleCommands.Add(cmd + "area", "Give items needed for all bundles in the given area.", (s, args) =>
			{
				if (args.Length == 0 || !int.TryParse(args[0], out int area))
				{
					Log.D("No area found.");
					return;
				}

				Bundles.GiveAreaItems(cc: Bundles.CC, whichArea: area, print: true);
			});

			Helper.ConsoleCommands.Add(cmd + "reset", "Reset all area-bundle progress to entirely incomplete.", (s, args) =>
			{
				if (Bundles.IsCommunityCentreDefinitelyComplete(Bundles.CC))
				{
					Log.D("Too late.");
					return;
				}

				Dictionary<int, int[]> areaNamesAndNumbers = Bundles.GetAllCustomAreaNumbersAndBundleNumbers();
				foreach (KeyValuePair<int, int[]> pair in areaNamesAndNumbers)
				{
					foreach (int bundleNumber in pair.Value)
					{
						Bundles.ResetBundleProgress(cc: Bundles.CC, whichBundle: bundleNumber, print: true);
					}

					if (Bundles.IsCustomArea(pair.Key))
					{
						Bundles.CustomAreasComplete[pair.Key] = false;
					}
					else
					{
						Bundles.CC.areasComplete[pair.Key] = false;
					}

					string areaName = CommunityCenter.getAreaNameFromNumber(pair.Key);
					string mailId = string.Format(Bundles.MailAreaCompleted, areaName);
					Game1.player.mailReceived.Remove(mailId);
					Game1.player.mailReceived.Remove(mailId + "%&NL&%");
					Game1.player.mailForTomorrow.Remove(mailId);
					Game1.player.mailForTomorrow.Remove(mailId + "%&NL&%");

					Log.D($"Marked {areaNamesAndNumbers.Count} areas incomplete.");
				}
			});

			Helper.ConsoleCommands.Add(cmd + "reset.b", "Reset a bundle's saved progress to entirely incomplete.", (s, args) =>
			{
				if (args.Length == 0 || !int.TryParse(args[0], out int bundle)
					|| !Game1.netWorldState.Value.Bundles.ContainsKey(bundle))
				{
					Log.D("No bundle found.");
					return;
				}
				if (Bundles.IsCommunityCentreDefinitelyComplete(Bundles.CC))
				{
					Log.D("Too late.");
					return;
				}

				Bundles.ResetBundleProgress(cc: Bundles.CC, bundle, print: true);
			});

			Helper.ConsoleCommands.Add(cmd + "reset.a", "Reset all bundle progress for an area to entirely incomplete.", (s, args) =>
			{
				var abd = Helper.Reflection.GetField
					<Dictionary<int, List<int>>>
					(Bundles.CC, "areaToBundleDictionary")
					.GetValue();
				if (args.Length == 0 || !int.TryParse(args[0], out int area) || !abd.ContainsKey(area))
				{
					Log.D("No area found.");
					return;
				}
				if (Bundles.IsCommunityCentreDefinitelyComplete(Bundles.CC))
				{
					Log.D("Too late.");
					return;
				}

				foreach (int bundle in abd[area])
				{
					Bundles.ResetBundleProgress(cc: Bundles.CC, bundle, print: true);
				}
			});

			Helper.ConsoleCommands.Add(cmd + "setup", $"Prepare the CC for custom bundles.", (s, args) =>
			{
				new List<string> { "ccDoorUnlock", "seenJunimoNote", "wizardJunimoNote", "canReadJunimoText" }
					.ForEach(id => Game1.player.mailReceived.Add(id));
				new List<int> { (int)Bundles.EventIds.CommunityCentreUnlocked }
					.ForEach(id => Game1.player.eventsSeen.Add(id));
				Game1.player.increaseBackpackSize(24);
				int areaNumber = Bundles.GetAreaNumberFromCommandArgs(args);
				if (areaNumber >= 0)
				{
					Bundles.GiveAreaItems(cc: Bundles.CC, whichArea: Bundles.CustomAreaInitialIndex, print: true);
				}

				Log.D("CC set up.");
			});

			Helper.ConsoleCommands.Add(cmd + "joja", $"Prepare the Joja warehouse.", (s, args) =>
			{
				new List<string> { "ccDoorUnlock", "JojaGreeting", "JojaMember" }
					.ForEach(id => Game1.player.mailReceived.Add(id));
				new List<int> { (int)Bundles.EventIds.CommunityCentreUnlocked }
					.ForEach(id => Game1.player.eventsSeen.Add(id));

				Log.D("Joja set up.");
			});

			Helper.ConsoleCommands.Add(cmd + "goto", $"Warp to a junimo note for an area in the CC.", (s, args) =>
			{
				int areaNumber = Bundles.GetAreaNumberFromCommandArgs(args);
				string areaName = CommunityCenter.getAreaNameFromNumber(areaNumber);
				string locationName = nameof(CommunityCenter);

				if (string.IsNullOrWhiteSpace(areaName))
				{
					Log.D($"No valid area name or number found for '{string.Join(" ", args)}'.");
					return;
				}

				Point tileLocation = Helper.Reflection
					.GetMethod(Bundles.CC, "getNotePosition")
					.Invoke<Point>(areaNumber);

				Log.D($"Warping to area {areaNumber} - {CommunityCenter.getAreaNameFromNumber(areaNumber)} ({tileLocation.ToString()})");

				Game1.warpFarmer(
					locationName: locationName,
					tileX: tileLocation.X,
					tileY: tileLocation.Y + 1,
					facingDirectionAfterWarp: 2);
			});

			Helper.ConsoleCommands.Add(cmd + "mail", "Reset and add area complete mail for some area.", (s, args) =>
			{
				string areaName = args.Length > 0 && int.TryParse(args[0], out int i)
					&& CommunityCenter.getAreaNameFromNumber(i) is string str
					&& !string.IsNullOrEmpty(str)
					? str
					: null;

				if (areaName == null)
				{
					Log.D($"No area found for '{areaName}'.");
					return;
				}

				string mailId = string.Format(Bundles.MailAreaCompleted, areaName);
				Game1.player.mailReceived.Remove(mailId);
				Game1.player.mailReceived.Remove(mailId + "%&NL&%");
				Game1.player.mailForTomorrow.Remove(mailId);
				Game1.player.mailForTomorrow.Remove(mailId + "%&NL&%");
				Game1.player.mailForTomorrow.Add(mailId + "%&NL&%");
				Log.D($"Added mail for tomorrow: {mailId}");
			});

			Helper.ConsoleCommands.Add(cmd + "complete", "Mark some area as complete and play junimo dance.", (s, args) =>
			{
				string areaName = args.Length > 0 && int.TryParse(args[0], out int i)
					&& CommunityCenter.getAreaNameFromNumber(i) is string str
					&& !string.IsNullOrEmpty(str)
					? str
					: null;

				if (areaName == null)
				{
					Log.D($"No area found for '{areaName}'.");
					return;
				}

				int areaNumber = CommunityCenter.getAreaNumberFromName(areaName);
				Reflection.GetMethod
					(obj: Bundles.CC, name: "doRestoreAreaCutscene")
					.Invoke(areaNumber);
			});
		}

		internal static void SaveLoadedBehaviours(CommunityCenter cc)
		{
			// . . .
		}

		internal static void DayStartedBehaviours(CommunityCenter cc)
		{
			// Send followup mail when an area is completed
			foreach (KeyValuePair<string, int> areaNameAndNumber in Bundles.CustomAreaNamesAndNumbers)
			{
				string mailId;
				if (Bundles.IsAreaComplete(cc, areaNumber: areaNameAndNumber.Value))
				{
					// Completion mail
					mailId = string.Format(Bundles.MailAreaCompleted, areaNameAndNumber.Key);
					if (!Game1.player.hasOrWillReceiveMail(mailId))
					{
						Log.D($"Sending day-started mail for custom bundle completion ({mailId})",
							ModEntry.Config.DebugMode);
						Game1.player.mailReceived.Add(mailId + "%&NL&%");
					}
					// Followup mail
					mailId = string.Format(Bundles.MailAreaCompletedFollowup, areaNameAndNumber.Key);
					if (!Game1.player.hasOrWillReceiveMail(mailId))
					{
						Log.D($"Sending followup mail for custom bundle completion ({mailId})",
							ModEntry.Config.DebugMode);
						Game1.addMailForTomorrow(mailId);
					}
				}
			}
		}

		private static void Multiplayer_PeerConnected(object sender, PeerConnectedEventArgs e)
		{
			IManifest manifest = ModEntry.Instance.ModManifest;

			IMultiplayerPeerMod mod = e.Peer.HasSmapi ? e.Peer.GetMod(id: manifest.UniqueID) : null;
			Log.D($"Multiplayer peer connected:{Environment.NewLine}{e.Peer.PlayerID} SMAPI:{(e.Peer.HasSmapi ? $"{e.Peer.ApiVersion.ToString()} (SDV:{e.Peer.Platform.Value} {e.Peer.GameVersion})" : "N/A")}",
				Config.DebugMode);

			if (mod == null)
			{
				Log.D($"Peer does not have {manifest.Name} loaded.",
					Config.DebugMode);
			}
			else if (mod.Version.CompareTo(manifest.Version) != 0)
			{
				Log.D($"Peer {manifest.Name} version does not match host (peer: {mod.Version.ToString()}, host: {manifest.Version.ToString()}).",
					Config.DebugMode);
			}
		}

		private static void Display_MenuChanged(object sender, MenuChangedEventArgs e)
		{
			if (e.OldMenu is TitleMenu || e.NewMenu is TitleMenu || Game1.currentLocation == null || Game1.player == null)
				return;

			CommunityCenter cc = Bundles.CC;
			if (e.OldMenu is JunimoNoteMenu junimoNoteMenu && e.NewMenu == null
				&& !Bundles.IsAbandonedJojaMartBundleAvailableOrComplete())
			{
				int areaNumber = Reflection.GetField
					<int>
					(junimoNoteMenu, "whichArea")
					.GetValue();

				// Unlock makeshift mutex
				Bundles.SetCustomAreaMutex(cc: cc, areaNumber: areaNumber, isLocked: false);

				// Play area complete cutscene on closing the completed junimo note menu
				// Without this override the cutscene only plays after re-entering and closing the menu
				if (areaNumber >= Bundles.CustomAreaInitialIndex
					&& Bundles.AreaAllCustomAreasComplete(cc))
				{
					cc.restoreAreaCutscene(areaNumber);
				}
			}
		}

		private static void Display_RenderedWorld(object sender, RenderedWorldEventArgs e)
		{
			// Draw final star on community centre areas complete plaque
			int numberOfStars = Game1.currentLocation is CommunityCenter cc && cc != null
				? cc.numberOfStarsOnPlaque.Value
				: -1;
			if (numberOfStars < Bundles.DefaultAreaCount)
				return;
			// Star opacity is relative to the number of remaining areas complete outside of default star slots
			// Custom and base areas are added to the default star slots, remaining areas count for opacity
			int areasComplete = Bundles.TotalAreasCompleteCount - Bundles.DefaultAreaCount;
			int areasCount = Bundles.TotalAreaCount - Bundles.DefaultAreaCount;
			float alpha = Math.Max(0f, areasCount < 1 ? 1f : ((float)areasComplete / areasCount));
			e.SpriteBatch.Draw(
				texture: Game1.mouseCursors,
				position: Game1.GlobalToLocal(viewport: Game1.viewport, globalPosition: Bundles.CustomAreaStarPosition),
				sourceRectangle: new Rectangle(354, 401, 7, 7),
				color: Color.White * alpha,
				rotation: 0f,
				origin: Vector2.Zero,
				scale: Game1.pixelZoom,
				effects: SpriteEffects.None,
				layerDepth: 0.8f);
		}

		private static void Player_Warped(object sender, WarpedEventArgs e)
		{
			if ((!(e.NewLocation is CommunityCenter) && e.OldLocation is CommunityCenter)
				|| (!(e.OldLocation is CommunityCenter) && e.NewLocation is CommunityCenter))
			{
				Helper.Content.InvalidateCache(@"Maps/townInterior");
			}
		}

		private static void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			// In-game interactions
			if (!Game1.game1.IsActive || Game1.currentLocation == null || !Context.IsWorldReady)
				return;

			// Menu interactions
			if (e.Button.IsUseToolButton())
			{
				// Navigate community centre bundles inventory menu
				if (Game1.activeClickableMenu is JunimoNoteMenu menu && menu != null
					&& !Bundles.IsAbandonedJojaMartBundleAvailableOrComplete()
					&& Reflection.GetField
						<int>
						(menu, "whichArea")
						.GetValue() is int whichArea
					&& Bundles.CC.shouldNoteAppearInArea(whichArea))
				{
					if (!Game1.player.hasOrWillReceiveMail("canReadJunimoText"))
					{
						Game1.activeClickableMenu.exitThisMenu();
						return;
					}

					Point cursor = Utility.Vector2ToPoint(e.Cursor.ScreenPixels);
					Bundles.NavigateJunimoNoteMenu(cc: Bundles.CC, menu: menu, x: cursor.X, y: cursor.Y, whichArea: whichArea);
				}
			}

			// World interactions
			if (!Context.CanPlayerMove)
				return;

			// . . .
		}

		public static bool IsCommunityCentreCompleteEarly(CommunityCenter cc)
		{
			if (cc == null)
				return false;

			// Check pre-completion bundle mail
			bool isProbablyComplete = Game1.MasterPlayer.hasCompletedCommunityCenter();
			bool isDefinitelyComplete = Bundles.IsCommunityCentreDefinitelyComplete(cc);

			return isProbablyComplete || isDefinitelyComplete;
		}

		public static bool IsCommunityCentreDefinitelyComplete(CommunityCenter cc)
		{
			if (cc == null)
				return false;

			// Check completion cutscenes
			bool cutsceneSeen = new int[]
			{
				(int)Bundles.EventIds.CommunityCentreComplete,
				(int)Bundles.EventIds.JojaWarehouseComplete
			}
			.Any(id => Game1.MasterPlayer.eventsSeen.Contains(id));

			// Check post-completion mail flags
			bool mailReceived = new string[]
			{
				"ccIsComplete",
				"abandonedJojaMartAccessible",
				"ccMovieTheater",
				"ccMovieTheater%&NL&%"
			}
			.Any(id => Game1.MasterPlayer.hasOrWillReceiveMail(id));

			return cutsceneSeen && mailReceived;
		}

		public static bool IsAbandonedJojaMartBundleAvailableOrComplete()
		{
			return Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("abandonedJojaMartAccessible");
		}

		public static bool AreAnyCustomAreasLoaded()
		{
			return Bundles.CustomAreaNamesAndNumbers != null && Bundles.CustomAreaNamesAndNumbers.Any();
		}

		public static bool AreAnyCustomBundlesLoaded()
		{
			return Game1.netWorldState.Value.BundleData
				.Any(pair => Bundles.CustomAreaNamesAndNumbers.ContainsKey(pair.Key.Split(Bundles.BundleKeyDelim).First()));
		}

		public static bool AreaAllCustomAreasComplete(CommunityCenter cc)
		{
			if (cc == null)
				return false;

			bool customAreasLoaded = Bundles.AreAnyCustomAreasLoaded();
			bool customAreasComplete = !customAreasLoaded || Bundles.CustomAreasComplete.Values.All(isComplete => isComplete);
			bool customBundlesLoaded = Bundles.AreAnyCustomBundlesLoaded();
			bool customBundlesComplete = cc.bundles.Keys
				.Where(key => key >= CustomBundleInitialIndex)
				.All(key => cc.bundles[key].All(value => value));
			bool ccIsComplete = Bundles.IsCommunityCentreCompleteEarly(cc);

			return !customAreasLoaded || customAreasComplete || !customBundlesLoaded || customBundlesComplete || ccIsComplete;
		}

		public static IEnumerable<CustomCommunityCentre.Data.BundleMetadata> GetAllCustomBundleMetadataEntries()
        {
			return ModEntry.ContentPacks.SelectMany(cp => cp.Metadata.Values);
		}

		public static CustomCommunityCentre.Data.BundleMetadata GetCustomBundleMetadataFromAreaNumber(int areaNumber)
		{
			string areaName = Bundles.GetCustomAreaNameFromNumber(areaNumber);
			return Bundles.GetAllCustomBundleMetadataEntries()
				.FirstOrDefault(bmd => bmd.AreaName == areaName);
		}

		public static int GetCustomAreaNumberFromName(string areaName)
		{
			return Bundles.CustomAreaNamesAndNumbers.TryGetValue(areaName, out int i)
				? i
				: -1;
		}

		public static string GetCustomAreaNameFromNumber(int areaNumber)
		{
			string name = Bundles.CustomAreaNamesAndNumbers.Keys
					.FirstOrDefault(key => Bundles.CustomAreaNamesAndNumbers[key] == areaNumber);
			return name;
		}

		public static string GetAreaNameAsAssetKey(string areaName)
		{
			return string.IsNullOrWhiteSpace(areaName)
				? string.Empty
				: string.Join(string.Empty, areaName.Split(' '));
		}

		public static IEnumerable<string> GetAllAreaNames()
		{
			return Game1.netWorldState.Value.BundleData.Keys.Select(s => s.Split(Bundles.BundleKeyDelim).First()).Distinct();
		}

		public static IEnumerable<int> GetAllCustomBundleNumbers()
		{
			return Bundles.CustomAreaNamesAndNumbers.Keys
				.SelectMany(areaName => Bundles.GetBundleNumbersForArea(areaName));
		}

		public static string GetBundleNameFromNumber(int bundleNumber)
		{
			return Game1.netWorldState.Value.BundleData.Keys
				.FirstOrDefault(pair => int.Parse(pair.Split(Bundles.BundleKeyDelim).Last()) == bundleNumber)
				.Split(Bundles.BundleKeyDelim)
				.First();
		}

		public static int GetBundleNumberFromName(string bundleName)
		{
			return int.Parse(Game1.netWorldState.Value.BundleData
				.FirstOrDefault(pair => pair.Value.Split(Bundles.BundleKeyDelim).First() == bundleName)
				.Key
				.Split(Bundles.BundleKeyDelim)
				.Last());
		}

		public static IEnumerable<(string, int)> GetBundleNamesAndNumbersFromBundleKeys(IEnumerable<string> bundleKeys)
		{
			return bundleKeys.Select(s =>
				(name: s.Split(Bundles.BundleKeyDelim).First(),
				number: int.Parse(s.Split(Bundles.BundleKeyDelim).Last())));
		}

		public static Dictionary<int, int[]> GetAllCustomAreaNumbersAndBundleNumbers()
		{
			return Bundles.CustomAreaNamesAndNumbers.Keys.ToDictionary(
				areaName => Bundles.GetCustomAreaNumberFromName(areaName),
				areaName => Bundles.GetBundleNumbersForArea(areaName).ToArray());
		}

		public static bool IsCustomArea(int areaNumber)
		{
			return areaNumber >= Bundles.CustomAreaInitialIndex;
		}

		public static bool IsCustomBundle(int bundleNumber)
		{
			return bundleNumber >= Bundles.CustomBundleInitialIndex;
		}

		public static bool IsCustomBundle(string bundleName)
		{
			return bundleName.StartsWith(CustomCommunityCentre.AssetManager.RequiredAssetNamePrefix, StringComparison.InvariantCulture);
		}

		public static bool IsAreaComplete(CommunityCenter cc, int areaNumber)
		{
			return Bundles.IsCommunityCentreCompleteEarly(cc)
				|| Bundles.IsDefaultAreaComplete(cc: cc, areaNumber: areaNumber)
				|| (Bundles.IsCustomArea(areaNumber) && Bundles.IsCustomAreaComplete(areaNumber: areaNumber));
		}

		public static bool IsDefaultAreaComplete(CommunityCenter cc, int areaNumber)
		{
			return areaNumber >= 0 && areaNumber < cc.areasComplete.Length && cc.areasComplete[areaNumber];
		}

		public static List<int> GetBundleNumbersForArea(string areaName)
		{
			List<int> bundleNumbers = Bundles.CustomAreaBundleKeys.TryGetValue(areaName ?? "", out string[] bundles)
				&& bundles != null && bundles.Length > 0
				? bundles
				.Select(bundle => int.Parse(bundle.Split(Bundles.BundleKeyDelim).Last()))
					.Where(bundleNumber => Game1.netWorldState.Value.Bundles.Keys.Contains(bundleNumber))
					.Distinct()
					.ToList()
				: new List<int>();
			return bundleNumbers;
		}

		public static bool IsCustomAreaComplete(int areaNumber)
		{
			// Check for AreasComplete entry
			bool isAreaComplete = Bundles.CustomAreasComplete.TryGetValue(areaNumber, out bool isComplete) && isComplete;

			// Custom area is also considered complete if it has no bundles loaded
			string areaName = Bundles.GetCustomAreaNameFromNumber(areaNumber);
			List<int> bundleNumbers = Bundles.GetBundleNumbersForArea(areaName);
			bool isBundleSetComplete = bundleNumbers.Any()
				&& bundleNumbers.All(bundleNumber => Game1.netWorldState.Value.Bundles[bundleNumber].All(b => b));

			return isAreaComplete || isBundleSetComplete;
		}

		public static bool ShouldNoteAppearInCustomArea(CommunityCenter cc, int areaNumber)
		{
			CustomCommunityCentre.Data.BundleMetadata bundleMetadata = Bundles.GetCustomBundleMetadataFromAreaNumber(areaNumber);
			bool isAreaComplete = Bundles.IsAreaComplete(cc: cc, areaNumber: areaNumber);
			int bundlesRequired = bundleMetadata.BundlesRequired;
			int bundlesCompleted = cc.numberOfCompleteBundles();
			return bundlesCompleted >= bundlesRequired && !isAreaComplete;
		}

		public static int GetNumberOfCustomAreasComplete()
		{
			return Bundles.CustomAreasComplete.Values.Count(isComplete => isComplete);
		}

		public static bool HasOrWillReceiveAreaCompletedMailForAllCustomAreas()
		{
			return Bundles.GetAllCustomBundleMetadataEntries()
				.Distinct()
				.All(areaName => Game1.MasterPlayer.hasOrWillReceiveMail(string.Format(Bundles.MailAreaCompleted, areaName)));
		}

		public static bool IsMultiplayer()
		{
			return Game1.IsMultiplayer || Bundles.GetNumberOfCabinsBuilt() > 0;
		}

		internal static int GetAreaNumberFromCommandArgs(string[] args)
		{
			return args.Length > 0
				? int.TryParse(args[0], out int i)
					? i
					: string.Join(" ", args) is string areaName && !string.IsNullOrWhiteSpace(areaName)
						&& CommunityCenter.getAreaNumberFromName(areaName) is int i1
						? i1
					: -1
				: -1;
		}

		public static int GetNumberOfCabinsBuilt()
		{
			return Game1.getFarm().buildings.Count(building => building.buildingType.Value.EndsWith("Cabin"));
		}

		public static void GiveBundleItems(CommunityCenter cc, int whichBundle, bool print)
		{
			KeyValuePair<string, string> bundle = Game1.netWorldState.Value.BundleData
				.FirstOrDefault(pair => pair.Key.Split(Bundles.BundleKeyDelim).Last() == whichBundle.ToString());
			string[] split = bundle.Value.Split(Bundles.BundleKeyDelim);
			string[] itemData = split[2].Split(' ');
			int itemLimit = split.Length < 5 ? 99 : int.Parse(split[4]);
			for (int i = 0; i < itemData.Length && i < itemLimit * 3; ++i)
			{
				int index = int.Parse(itemData[i]);
				int quantity = int.Parse(itemData[++i]);
				int quality = int.Parse(itemData[++i]);
				if (index == -1)
				{
					Game1.player.addUnearnedMoney(quantity);
				}
				else
				{
					if (index < 0)
                    {
						// Get object from category, hopefully avoiding dud item definitions
						index = Game1.objectInformation
							.LastOrDefault(pair => int.TryParse(pair.Value.Split('/')[3].Split(' ').Last(), out int i) && index == i).Key;
                    }
					Game1.createItemDebris(
						item: new StardewValley.Object(index, quantity, isRecipe: false, price: -1, quality: quality),
						origin: Game1.player.Position,
						direction: -1);
				}
			}
			Log.D($"Giving items for {bundle.Key}: {bundle.Value.Split(Bundles.BundleKeyDelim).First()} Bundle.",
				print);
		}

		public static void GiveAreaItems(CommunityCenter cc, int whichArea, bool print)
		{
			var areaBundleDict = Helper.Reflection.GetField
				<Dictionary<int, List<int>>>
				(cc, "areaToBundleDictionary")
				.GetValue();
			if (!areaBundleDict.ContainsKey(whichArea))
			{
				Log.D("No area found.");
				return;
			}

			foreach (int bundle in areaBundleDict[whichArea])
			{
				if (whichArea >= Bundles.CustomAreaInitialIndex)
				{
					if (Bundles.IsAbandonedJojaMartBundleAvailableOrComplete() && bundle >= CustomBundleInitialIndex)
						continue;
					else if (!Bundles.IsAbandonedJojaMartBundleAvailableOrComplete() && bundle < CustomBundleInitialIndex)
						continue;
				}
				if (cc.isBundleComplete(bundle))
					continue;
				Bundles.GiveBundleItems(cc: cc, bundle, print: print);
			}
		}

		public static void ResetBundleProgress(CommunityCenter cc, int whichBundle, bool print)
		{
			Dictionary<int, int> bundleAreaDict = Reflection.GetField
				<Dictionary<int, int>>
				(cc, "bundleToAreaDictionary")
				.GetValue();

			cc.bundleRewards[whichBundle] = false;
			cc.bundles[whichBundle] = new bool[cc.bundles[whichBundle].Length];
			Game1.netWorldState.Value.BundleRewards[whichBundle] = false;
			Game1.netWorldState.Value.Bundles[whichBundle] = new bool[Game1.netWorldState.Value.Bundles[whichBundle].Length];

			int areaNumber = bundleAreaDict[whichBundle];

			if (Bundles.IsAreaComplete(cc: cc, areaNumber: areaNumber))
			{
				if (areaNumber < Bundles.DefaultMaxArea)
				{
					cc.areasComplete[areaNumber] = false;
				}
				else if (Bundles.CustomAreasComplete.ContainsKey(areaNumber))
				{
					Bundles.CustomAreasComplete[areaNumber] = false;
				}

				cc.loadMap(cc.Map.assetPath, force_reload: true);
			}

			KeyValuePair<string, string> bundle = Game1.netWorldState.Value.BundleData
				.FirstOrDefault(pair => pair.Key.Split(Bundles.BundleKeyDelim).Last() == whichBundle.ToString());
			Log.D($"Reset progress for {bundle.Key}: {bundle.Value.Split(Bundles.BundleKeyDelim).First()} bundle.",
				print);
		}

		internal static void NavigateJunimoNoteMenu(CommunityCenter cc, JunimoNoteMenu menu, int x, int y, int whichArea)
		{
			bool isPrevious = menu.areaBackButton != null && menu.areaBackButton.visible && menu.areaBackButton.containsPoint(x, y);
			bool isNext = menu.areaNextButton != null && menu.areaNextButton.visible && menu.areaNextButton.containsPoint(x, y);

			bool isSpecificBundlePage = Reflection.GetField<bool>(obj: menu, name: "specificBundlePage").GetValue();

			if (!menu.isReadyToCloseMenuOrBundle() || !JunimoNoteMenu.canClick || isSpecificBundlePage || !(isPrevious || isNext))
				return;

			ModEntry.Instance.State.Value.LastJunimoNoteMenuArea = -1;

			// Fetch the bounds of the menu, exclusive of our new area since we're assuming we start there
			// Exclude any already-completed areas from this search, since we're looking for unfinished areas

			int[] areaNumbers = Bundles.GetAllAreaNames()
				.Select(areaName => CommunityCenter.getAreaNumberFromName(areaName))
				.Where(areaNumber => !Bundles.IsAreaComplete(cc: cc, areaNumber: areaNumber) && cc.shouldNoteAppearInArea(areaNumber))
				.ToArray();

			int lowestArea = areaNumbers.Min();
			int highestArea = areaNumbers.Max();

			int nextLowestArea = whichArea == lowestArea
				? highestArea
				: areaNumbers.Where(i => i < whichArea).Last();
			int nextHighestArea = whichArea == highestArea
				? lowestArea
				: areaNumbers.Where(i => i > whichArea).First();

			if (isPrevious)
				ModEntry.Instance.State.Value.LastJunimoNoteMenuArea = nextLowestArea;
			else
				ModEntry.Instance.State.Value.LastJunimoNoteMenuArea = nextHighestArea;

			if (ModEntry.Instance.State.Value.LastJunimoNoteMenuArea > -1
				&& ModEntry.Instance.State.Value.LastJunimoNoteMenuArea != whichArea)
			{
				// Change the menu tab on the next tick to avoid errors
				Helper.Events.GameLoop.UpdateTicked += Bundles.Event_ChangeJunimoMenuArea;
			}
		}

		public static void BroadcastPuffSprites(Multiplayer multiplayer, GameLocation location, Vector2 tilePosition)
		{
			TemporaryAnimatedSprite sprite = new (
				rowInAnimationTexture: (Game1.random.NextDouble() < 0.5) ? 5 : 46,
				position: (tilePosition * Game1.tileSize) + new Vector2(0, Game1.smallestTileSize),
				color: Color.White)
			{
				layerDepth = 1f
			};
			multiplayer ??= CustomCommunityCentre.ModEntry.Instance.GetMultiplayer();
			multiplayer.broadcastSprites(location: location, sprites: sprite);
		}

		internal static void SetUpJunimosForGoodbyeDance(CommunityCenter cc)
		{
			List<Junimo> junimos = cc.getCharacters().OfType<Junimo>().ToList();
			Vector2 min = new (junimos.Min(j => j.Position.X), junimos.Min(j => j.Position.Y));
			Vector2 max = new (junimos.Max(j => j.Position.X), junimos.Max(j => j.Position.Y));
			for (int i = 0; i < Bundles.CustomAreaNamesAndNumbers.Count; ++i)
			{
				Junimo junimo = cc.getJunimoForArea(Bundles.CustomAreaInitialIndex + i);

				int xOffset = i * Game1.tileSize;
				Vector2 position;
				position.X = Game1.random.NextDouble() < 0.5f
					? min.X - Game1.tileSize - xOffset
					: max.X + Game1.tileSize + xOffset;
				position.Y = Game1.random.NextDouble() < 0.5f
					? min.Y
					: max.Y;

				junimo.Position = min + (position * Game1.tileSize);

				// Do as in the target method
				junimo.stayStill();
				junimo.faceDirection(1);
				junimo.fadeBack();
				junimo.IsInvisible = false;
				junimo.setAlpha(1f);
			}
		}

		internal static void SetCustomAreaMutex(CommunityCenter cc, int areaNumber, bool isLocked)
		{
			if (!cc.modData.ContainsKey(Bundles.KeyMutexes))
			{
				cc.modData[Bundles.KeyMutexes] = string.Empty;
			}

			string sArea = areaNumber.ToString();
			string lockedAreas = cc.modData[Bundles.KeyMutexes];
			if (isLocked)
			{
				if (string.IsNullOrWhiteSpace(lockedAreas) || lockedAreas.Split(' ').All(s => s != sArea))
				{
					cc.modData[Bundles.KeyMutexes] = lockedAreas.Any() ? string.Join(" ", lockedAreas, sArea) : sArea;
					Game1.activeClickableMenu = new JunimoNoteMenu(whichArea: areaNumber, cc.bundlesDict());
				}
			}
			else
			{
				cc.modData[Bundles.KeyMutexes] = string.Join(" ", lockedAreas.Split(' ').Where(s => s != sArea));
			}
		}

		public static void SetCC(CommunityCenter cc)
		{
			Bundles._cc = cc;
		}

		internal static void Print(CommunityCenter cc)
		{
			if (cc == null)
			{
				Log.D("Cannot print Community Centre info when location is not loaded.");
				return;
			}

			var bundleAreaDict = Reflection.GetField
				<Dictionary<int, int>>
				(cc, "bundleToAreaDictionary")
				.GetValue();
			var areaBundleDict = Reflection.GetField
				<Dictionary<int, List<int>>>
				(cc, "areaToBundleDictionary")
				.GetValue();

			LogLevel logHigh = Config.DebugMode ? LogLevel.Warn : LogLevel.Trace;
			LogLevel logLow = Config.DebugMode ? LogLevel.Info : LogLevel.Trace;

			System.Text.StringBuilder msg = new System.Text.StringBuilder()
				.AppendLine($"Bundle Type: {Game1.bundleType.ToString()}")
				.AppendLine($"Area DefaultMax: {DefaultMaxArea}, Count: {TotalAreaCount}")
				.AppendLine($"Bundle DefaultMax: {DefaultMaxBundle}, Count: {TotalBundleCount}")
				.AppendLine($"Multiplayer: (G:{Game1.IsMultiplayer}-B:{Bundles.IsMultiplayer()}), Host game: ({Game1.IsMasterGame}), Host player: ({Context.IsMainPlayer})")
				.AppendLine($"IsAbandonedJojaMartBundleAvailableOrComplete: {Bundles.IsAbandonedJojaMartBundleAvailableOrComplete()}")
				.AppendLine($"IsCommunityCentreDefinitelyComplete: {Bundles.IsCommunityCentreDefinitelyComplete(cc)}")
				.AppendLine($"IsCommunityCentreComplete: {Bundles.IsCommunityCentreCompleteEarly(cc)}")
				.AppendLine($"AreAnyCustomAreasLoaded:  {Bundles.AreAnyCustomAreasLoaded()}")
				.AppendLine($"AreaAllCustomAreasComplete:  {Bundles.AreaAllCustomAreasComplete(cc)}")
				.AppendLine($"HasOrWillReceiveAreaCompletedMailForAllCustomAreas:  {Bundles.HasOrWillReceiveAreaCompletedMailForAllCustomAreas()}")
				.AppendLine($"BundleMutexes: {cc.bundleMutexes.Count}")
				;

			(string title, string body)[] messages = new[]
			{
				// General info
				($"CCC: General info",
				msg.ToString()),

				// Custom info
				($"CCC: CCC {nameof(Bundles.CustomAreaNamesAndNumbers)}[{Bundles.CustomAreaNamesAndNumbers.Count}]:",
				string.Join(Environment.NewLine,
					Bundles.CustomAreaNamesAndNumbers.Select(pair => $"{pair.Key}: {pair.Value}"))),
				($"CCC: CCC {nameof(Bundles.CustomAreasComplete)}[{Bundles.GetNumberOfCustomAreasComplete()}/{Bundles.CustomAreasComplete.Count}]:",
				string.Join(Environment.NewLine,
					Bundles.CustomAreasComplete)),
				($"CCC: CCC {nameof(Bundles.CustomAreaBundleKeys)}[{Bundles.CustomAreaBundleKeys.Count}]:",
				string.Join(Environment.NewLine,
					Bundles.CustomAreaBundleKeys.Select(pair => $"{pair.Key}: {string.Join(" ", pair.Value.Select(i => i))}"))),

				// Area info
				($"CCC: CC {nameof(cc.areasComplete)}[{Reflection.GetMethod(cc, "getNumberOfAreasComplete").Invoke<int>()}/{cc.areasComplete.Count}]:",
				string.Join(Environment.NewLine,
					cc.areasComplete)),
				($"CCC: CC {nameof(bundleAreaDict)}[{bundleAreaDict.Count}]:",
				string.Join(Environment.NewLine,
				bundleAreaDict.Select(pair => $"({pair.Key}: {pair.Value})"))),
				($"CCC: CC {nameof(areaBundleDict)}[{areaBundleDict.Count}]:",
				string.Join(Environment.NewLine,
					areaBundleDict.Select(pair => $"({pair.Key}: {string.Join(" ", pair.Value.Select(i => i))})"))),

				// Bundle info
				($"CCC: GW {nameof(Game1.netWorldState.Value.BundleData)}[{Game1.netWorldState.Value.BundleData.Count}]:",
				string.Join(Environment.NewLine,
					Game1.netWorldState.Value.BundleData.Select(pair => $"{pair.Key}: {pair.Value}"))),
				($"CCC: GW {nameof(Game1.netWorldState.Value.Bundles)}[{Game1.netWorldState.Value.Bundles.Count()}]:",
				string.Join(Environment.NewLine,
					Game1.netWorldState.Value.Bundles.Pairs.Select(pair => $"{pair.Key}: {string.Join(" ", pair.Value)}"))),
				($"CCC: GW {nameof(Game1.netWorldState.Value.BundleRewards)}[{Game1.netWorldState.Value.BundleRewards.Count()}]:",
				string.Join(Environment.NewLine,
					Game1.netWorldState.Value.BundleRewards.Pairs.Select(pair => $"{pair.Key}: {pair.Value}"))),
			};

			foreach ((string title, string body) in messages)
			{
				Bundles.Monitor.Log(title, logHigh);
				Bundles.Monitor.Log($"{Environment.NewLine}{body}{Environment.NewLine}", logLow);
			}
		}

		private static void Event_ChangeJunimoMenuArea(object sender, UpdateTickedEventArgs e)
		{
			Helper.Events.GameLoop.UpdateTicked -= Bundles.Event_ChangeJunimoMenuArea;
			JunimoNoteMenu junimoNoteMenu = Game1.activeClickableMenu as JunimoNoteMenu;
			// Set JunimoNoteArea field for area currently being displayed
			Reflection.GetField
				<int>
				(junimoNoteMenu, "whichArea")
				.SetValue(ModEntry.Instance.State.Value.LastJunimoNoteMenuArea);
			// Force menu to refresh and display bundles for this area
			junimoNoteMenu.bundles.Clear();
			junimoNoteMenu.setUpMenu(
				whichArea: ModEntry.Instance.State.Value.LastJunimoNoteMenuArea,
				bundlesComplete: Bundles.CC.bundlesDict());
		}
	}
}
