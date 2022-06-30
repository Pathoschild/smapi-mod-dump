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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using xTile;

using TraktoriShared.Utils;


namespace MiniDungeons
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		internal static IMonitor logMonitor = null!;
		internal static ITranslationHelper i18n = null!;

		public static readonly string portalAssetName = PathUtilities.NormalizeAssetName("Traktori.MiniDungeons/PortalSprite");

		private readonly string portalSpritePath = Path.Combine("assets", "PortalSprite.png");
		private readonly string challengeDataPath = Path.Combine("assets", "data", "ChallengeData.json");
		private readonly string dungeonDataPath = Path.Combine("assets", "data", "DungeonData.json");
		//private readonly string seedShopDungeonMap = "Maps/SeedShopDungeon_1";

		internal static ModConfig config = null!;
		internal static readonly string modIDPrefix = "Traktori.MiniDungeons";

		//private DungeonManager dungeonManager = null!;

		public static readonly string actionName = $"{modIDPrefix}.Portal";

		// TODO: Dictionary keys are the context tags, the integers are the objectInformation keys of all items that have that context tag.
		// Loop through ObjectContextTags + add manually atleast category tags.
		// JsonAssets inserts its own tags there. That way we catch modded items + modded tags.
		internal readonly Dictionary<string, List<int>> contextTagData = new Dictionary<string, List<int>>();


		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			logMonitor = Monitor;
			i18n = helper.Translation;

			ReadData();

			config = helper.ReadConfig<ModConfig>();

			helper.Events.Content.AssetRequested += OnAssetRequested;
			helper.Events.GameLoop.DayStarted += OnDayStarted;
			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.Saving += OnSaving;
			helper.Events.Player.Warped += OnWarped;
			helper.Events.World.NpcListChanged += OnNpcListChanged;

			DoHarmonyPatches();
		}


		/// <summary>
		/// The event handler for the SMAPI event AssetRequested.
		/// Raised when an asset is being requested from the content pipeline.
		/// The asset isn't necessarily being loaded yet (e.g. the game may be checking if it exists).
		/// </summary>
		private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
		{
			if (e.Name.StartsWith(Path.Combine("Maps", modIDPrefix)))
			{
				e.LoadFromModFile<Map>("assets/maps/SeedShop/SeedShop.tmx", AssetLoadPriority.Low);
			}
			else if (e.Name.IsEquivalentTo(portalAssetName))
			{
				e.LoadFromModFile<Texture2D>(portalSpritePath, AssetLoadPriority.Low);
			}
		}


		/// <summary>
		/// The event handler for the SMAPI event DayStarted.
		/// Raised after the game begins a new day (including when the player loads a save).
		/// </summary>
		private void OnDayStarted(object? sender, DayStartedEventArgs e)
		{
			DungeonManager.PerformDayReset();
		}


		/// <summary>
		/// The event handler for the SMAPI event GameLaunched.
		/// Raised after the game is launched, right before the first update tick.
		/// This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point,
		/// so this is a good time to set up mod integrations.
		/// </summary>
		private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
		{
			var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

			if (configMenu is not null)
			{
				RegisterConfigMenu(configMenu);
			}
		}


		/// <summary>
		/// The event handler for the SMAPI event Saving.
		/// Raised before the game begins writing data to the save file (except the initial save creation).
		/// </summary>
		private void OnSaving(object? sender, SavingEventArgs e)
		{
			DungeonManager.RemoveAddedLocations();
		}


		/// <summary>
		/// The event handler for the SMAPI event Warped.
		/// Raised after a player warps to a new location. NOTE: this event is currently only raised for the current player.
		/// </summary>
		private void OnWarped(object? sender, WarpedEventArgs e)
		{
			if (e.NewLocation is null)
			{
				return;
			}

			DungeonManager.PlayerWarped(e);
		}


		/// <summary>
		/// The event handler for the SMAPI event NpcListChanged.
		/// Raised after NPCs are added/removed in any location (including villagers, horses, Junimos, monsters, and pets).
		/// </summary>
		private void OnNpcListChanged(object? sender, NpcListChangedEventArgs e)
		{
			if (e.IsCurrentLocation && DungeonManager.IsLocationMiniDungeon(Game1.currentLocation))
			{
				DungeonManager.NpcListChangedInDungeon(e);
			}
		}


		/// <summary>
		/// Constructs the GenericModConfigMenu menu's options
		/// </summary>
		/// <param name="configMenu"></param>
		private void RegisterConfigMenu(IGenericModConfigMenuApi configMenu)
		{
			InitializeConfig();

			configMenu.Register(
				mod: ModManifest,
				reset: () => config = new ModConfig(),
				save: () => Helper.WriteConfig(config)
			);

			string enableNotification = "enable-notification";
			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => GetLabelTranslation(enableNotification),
				tooltip: () => GetDescriptionTranslation(enableNotification),
				getValue: () => config.enableHUDNotification,
				setValue: (bool value) => config.enableHUDNotification = value
			);

			string dungeonLimit = "dungeon-limit";
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => GetLabelTranslation(dungeonLimit),
				tooltip: () => GetDescriptionTranslation(dungeonLimit),
				getValue: () => config.maxNumberOfDungeonsPerDay,
				setValue: (int value) => config.maxNumberOfDungeonsPerDay = value,
				min: -1
			);

			string enableFightingChallenge = "enable-fighting-challenge";
			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => GetLabelTranslation(enableFightingChallenge),
				tooltip: () => GetDescriptionTranslation(enableFightingChallenge),
				getValue: () => config.enableFightingchallenges,
				setValue: (bool value) => config.enableFightingchallenges = value
			);

			string enableDeathProtection = "enable-death-protection";
			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => GetLabelTranslation(enableDeathProtection),
				tooltip: () => GetDescriptionTranslation(enableDeathProtection),
				getValue: () => config.enableDeathProtection,
				setValue: (bool value) => config.enableDeathProtection = value
			);

			foreach (Dungeon dungeon in DungeonManager.dungeons)
			{
				string enableDungeon = "enable-dungeon";
				configMenu.AddBoolOption(
					mod: ModManifest,
					name: () => GetLabelTranslation(enableDungeon, new {dungeonName = dungeon.Name}),
					tooltip: () => GetDescriptionTranslation(enableDungeon, new { dungeonName = dungeon.Name }),
					getValue: () => config.enabledDungeons[dungeon.Name],
					setValue: (bool value) => config.enabledDungeons[dungeon.Name] = value
				);

				string dungeonSpawnChance = "dungeon-spawn-chance";
				configMenu.AddNumberOption(
					mod: ModManifest,
					name: () => GetLabelTranslation(dungeonSpawnChance, new { dungeonName = dungeon.Name }),
					tooltip: () => GetDescriptionTranslation(dungeonSpawnChance, new { dungeonName = dungeon.Name, defaultChance = dungeon.SpawnChance }),
					getValue: () => config.dungeonSpawnChances[dungeon.Name],
					setValue: (float value) => config.dungeonSpawnChances[dungeon.Name] = value,
					min: 0f,
					max: 1f,
					interval: 0.01f
				);
			}
		}


		/// <summary>
		/// Initializes the config by removing values that don't match currently loaded dungeons.
		/// </summary>
		private static void InitializeConfig()
		{
			// Makes sure the dictionaries don't contain any old keys, but keeps the old values
			Dictionary<string, bool> temp1 = config.enabledDungeons;
			Dictionary<string, float> temp2 = config.dungeonSpawnChances;

			config.enabledDungeons.Clear();
			config.dungeonSpawnChances.Clear();

			// Initialize with only the values for the currently loaded dungeons
			foreach (Dungeon dungeon in DungeonManager.dungeons)
			{
				if (!config.enabledDungeons.ContainsKey(dungeon.Name))
				{
					config.enabledDungeons[dungeon.Name] = true;
				}
				
				if (!config.dungeonSpawnChances.ContainsKey(dungeon.Name))
				{
					config.dungeonSpawnChances[dungeon.Name] = dungeon.SpawnChance;
				}
			}

			// Overwrite with the old values
			foreach (var item in temp1)
			{
				if (config.enabledDungeons.ContainsKey(item.Key))
				{
					config.enabledDungeons[item.Key] = item.Value;
				}
			}

			foreach (var item in temp2)
			{
				if (config.dungeonSpawnChances.ContainsKey(item.Key))
				{
					config.dungeonSpawnChances[item.Key] = item.Value;
				}
			}
		}


		/// <summary>
		/// Reads the challenge and dungeon datas. Initializes the dungeon manager with the data.
		/// </summary>
		private void ReadData()
		{
			Dictionary<string, Data.Challenge> challengeData = ReadChallengeData();
			Dictionary<string, Data.Dungeon> dungeonData = ReadDungeonData();

			DungeonManager.PopulateDungeonList(dungeonData, challengeData);
		}


		/// <summary>
		/// Reads the challenge data json files.
		/// </summary>
		/// <returns>Returns the dictionary of challenge data, or an empty one if the reading failed.</returns>
		private Dictionary<string, Data.Challenge> ReadChallengeData()
		{
			return GenericHelper.ReadListAssetToDict<Data.Challenge>(challengeDataPath, Helper.Data, Monitor, x => x.ChallengeName);
		}


		/// <summary>
		/// Reads the dungeon data json into a dictionary with the dungeons names used as the keys.
		/// </summary>
		/// <returns>Returns the dictionary of dungeon data, or an empty one if the reading failed.</returns>
		private Dictionary<string, Data.Dungeon> ReadDungeonData()
		{
			return GenericHelper.ReadListAssetToDict<Data.Dungeon>(dungeonDataPath, Helper.Data, Monitor, x => x.DungeonName);
		}


		private static Translation GetLabelTranslation(string optionName, object? tokens = null)
		{
			if (tokens is not null)
			{
				return i18n.Get($"gmcm.{optionName}-label", tokens);
			}
			else
			{
				return i18n.Get($"gmcm.{optionName}-label");
			}
		}


		private static Translation GetDescriptionTranslation(string optionName, object? tokens = null)
		{
			if (tokens is not null)
			{
				return i18n.Get($"gmcm.{optionName}-description", tokens);
			}
			else
			{
				return i18n.Get($"gmcm.{optionName}-description");
			}
		}


		private void DoHarmonyPatches()
		{
			var harmony = new Harmony(ModManifest.UniqueID);

			HarmonyPatches.PerformTouchAction.Initialize(Monitor, i18n);
			
			harmony.Patch(
				original: AccessTools.Method(typeof(StardewValley.GameLocation), nameof(StardewValley.GameLocation.performTouchAction))
					?? throw new InvalidOperationException("Can't find GameLocation.performTouchAction to patch"),
				postfix: new HarmonyMethod(typeof(HarmonyPatches.PerformTouchAction), nameof(HarmonyPatches.PerformTouchAction.PerformTouchAction_Postfix))
			);


			HarmonyPatches.TakeDamage.Initialize(Monitor);

			harmony.Patch(
				original: AccessTools.Method(typeof(StardewValley.Farmer), nameof(StardewValley.Farmer.takeDamage))
					?? throw new InvalidOperationException("Can't find StardewValley.Farmer.takeDamage to patch"),
				postfix: new HarmonyMethod(typeof(HarmonyPatches.TakeDamage), nameof(HarmonyPatches.TakeDamage.TakeDamage_Postfix))
			);
		}
	}
}