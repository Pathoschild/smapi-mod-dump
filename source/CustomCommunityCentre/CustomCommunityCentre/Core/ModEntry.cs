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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;

namespace CustomCommunityCentre
{
    public class ModEntry : Mod
	{
		public static CustomCommunityCentre.ModEntry Instance;
		public static CustomCommunityCentre.Config Config;
		public static CustomCommunityCentre.AssetManager AssetManager;
		public static List<CustomCommunityCentre.Data.ContentPack> ContentPacks;

		// Constant values
		public const int DummyId = 6830 * 10000;
		private const string CommandPrefix = "bb.ccc.";

		// Player states
		public readonly PerScreen<PlayerState> State = new(createNewState: () => new PlayerState());
		public class PlayerState
		{
			public int LastJunimoNoteMenuArea = 0;
		}

		// Game states
		public bool IsSaveLoaded = false;
		public static bool IsNewGame => Game1.dayOfMonth == 1 && Game1.currentSeason == "spring" && Game1.year == 1;


		public override void Entry(IModHelper helper)
		{
			CustomCommunityCentre.ModEntry.Instance = this;
			CustomCommunityCentre.ModEntry.Config = helper.ReadConfig<CustomCommunityCentre.Config>();
			CustomCommunityCentre.ModEntry.AssetManager = new CustomCommunityCentre.AssetManager();

			this.RegisterEvents();
			this.AddConsoleCommands();

			string id = this.ModManifest.UniqueID;
			HarmonyPatches.ApplyHarmonyPatches(id: id);

			helper.Content.AssetLoaders.Add(CustomCommunityCentre.ModEntry.AssetManager);
			helper.Content.AssetEditors.Add(CustomCommunityCentre.ModEntry.AssetManager);
		}

		public override object GetApi()
		{
			return new CustomCommunityCentre.API.CustomCommunityCentreAPI(reflection: Helper.Reflection);
		}

		private void AddConsoleCommands()
		{
			if (CustomCommunityCentre.ModEntry.Config.DebugMode)
			{
				this.Helper.ConsoleCommands.Add(CustomCommunityCentre.ModEntry.CommandPrefix + "debug1", "...", (s, args) =>
				{
					Log.D($"{nameof(Game1.player.mailForTomorrow)}:{string.Join(System.Environment.NewLine, Game1.player.mailForTomorrow)}");
					Log.D($"{nameof(Game1.player.mailbox)}:{string.Join(System.Environment.NewLine, Game1.player.mailbox)}");
					Log.D($"{nameof(Game1.player.mailReceived)}:{string.Join(System.Environment.NewLine, Game1.player.mailReceived)}");
					Log.D($"{nameof(Game1.player.eventsSeen)}:{string.Join(System.Environment.NewLine, Game1.player.eventsSeen)}");

					/*
					var tempCC = new CommunityCenter();
					var CompletedItems = tempCC.bundlesDict();
					*/
					/*
					if (args.Length < 2)
					{
						Log.D("Missing args: <dialogueKey> <dialogueCharaName> [dialogueSheetName]");
						return;
					}
					string dialogueKey = args[0];
					string dialogueCharaName = args[1];
					NPC dialogueChara = Utility.fuzzyCharacterSearch(dialogueCharaName);
					string dialogueSheetName = args.Length > 2 ? args[2] : $"Characters\\Dialogue\\{dialogueChara.Name}";
					string dialogueKeyComplete = $"{dialogueSheetName}:{dialogueKey}";
					string dialogueString = Game1.content.LoadString(dialogueKeyComplete);
					Dialogue dialogue = new Dialogue(dialogueString, dialogueChara);
					Game1.activeClickableMenu = new StardewValley.Menus.DialogueBox(dialogue);
					Monitor.Log($"{dialogueKeyComplete} : {dialogueString}", LogLevel.Debug);
					*/
				});
			}

			BundleManager.AddConsoleCommands(cmd: CustomCommunityCentre.ModEntry.CommandPrefix);
			Bundles.AddConsoleCommands(cmd: CustomCommunityCentre.ModEntry.CommandPrefix);
		}

		private void RegisterEvents()
		{
			this.Helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
			this.Helper.Events.GameLoop.ReturnedToTitle += this.GameLoop_ReturnedToTitle;
			this.Helper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
			this.Helper.Events.GameLoop.DayStarted += this.GameLoop_DayStarted;

			BundleManager.RegisterEvents();
			Bundles.RegisterEvents();
		}

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.Helper.Events.GameLoop.OneSecondUpdateTicked += Event_LoadAssetsLate;
		}

        private void Event_LoadAssetsLate(object sender, OneSecondUpdateTickedEventArgs e)
		{
			// Reason for being late:
			// Delay content pack loading until after all SMAPI mod-provided content packs have been loaded
			this.Helper.Events.GameLoop.OneSecondUpdateTicked -= Event_LoadAssetsLate;

			if (CustomCommunityCentre.ModEntry.ContentPacks == null)
			{
				// Fetch and load all custom content packs through the SMAPI content pack API,
				// as well as through SMAPI mod-provided event handlers
				CustomCommunityCentre.Data.ContentPack.Load(helper: Helper);
			}
		}

        private void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
		{
			this.IsSaveLoaded = false;
		}

		private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			this.SaveLoadedBehaviours();
		}

		private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
		{
			if (CustomCommunityCentre.ModEntry.IsNewGame && !this.IsSaveLoaded)
			{
				// Perform OnSaveLoaded behaviours when starting a new game
				this.SaveLoadedBehaviours();
			}

			this.DayStartedBehaviours(Bundles.CC);
		}

		private void SaveLoadedBehaviours()
		{
			this.IsSaveLoaded = true;

			AssetManager.ReloadAssets(helper: this.Helper);

			BundleManager.SaveLoadedBehaviours(Bundles.CC);
			Bundles.SaveLoadedBehaviours(Bundles.CC);
		}

		private void DayStartedBehaviours(CommunityCenter cc)
		{
			BundleManager.DayStartedBehaviours(cc);
			Bundles.DayStartedBehaviours(cc);
		}

		public Multiplayer GetMultiplayer()
		{
			Multiplayer multiplayer = Helper.Reflection.GetField
				<Multiplayer>
				(type: typeof(Game1), name: "multiplayer")
				.GetValue();
			return multiplayer;
		}

		public static Vector2 FindFirstPlaceableTileAroundPosition(GameLocation location, StardewValley.Object o, Vector2 tilePosition, int maxIterations)
		{
			// Recursive search logic taken from Stardew Valley
			// See StardewValley.Utility.cs:RecursiveFindOpenTiles()

			int iterations = 0;
			Queue<Vector2> positionsToCheck = new();
			positionsToCheck.Enqueue(tilePosition);
			List<Vector2> closedList = new();
			Vector2[] directionsTileVectors = Utility.DirectionsTileVectors;
			for (; iterations < maxIterations; ++iterations)
			{
				if (positionsToCheck.Count <= 0)
				{
					break;
				}
				Vector2 currentPoint = positionsToCheck.Dequeue();
				closedList.Add(currentPoint);
				if (currentPoint != tilePosition
					&& o.canBePlacedHere(location, currentPoint)
					&& location.isTileLocationOpen(new xTile.Dimensions.Location((int)currentPoint.X, (int)currentPoint.Y)))
				{
					return currentPoint;
				}
				foreach (Vector2 v in directionsTileVectors)
				{
					if (!closedList.Contains(currentPoint + v))
					{
						positionsToCheck.Enqueue(currentPoint + v);
					}
				}
			}
			return Vector2.Zero;
		}
	}
}
