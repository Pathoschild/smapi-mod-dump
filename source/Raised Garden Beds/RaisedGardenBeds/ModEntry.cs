/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/RaisedGardenBeds
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RaisedGardenBeds
{
	public class ModEntry : Mod
	{
		// common
		internal static ModEntry Instance;
		internal static Config Config;

		// definitions
		/// <summary>
		/// Shared object variant dictionary containing entries provided by the content pack, as well as some metadata about the content pack itself.
		/// Entries are keyed by <see cref="OutdoorPot.VariantIndex".
		/// </summary>
		internal static Dictionary<string, ItemDefinition> ItemDefinitions = null;
		/// <summary>
		/// Shared object spritesheet dictionary containing object icon, world sprite component, object breakage, and watered/unwatered soil sprites.
		/// Entries are keyed by <see cref="OutdoorPot.VariantIndex".
		/// </summary>
		internal static Dictionary<string, Texture2D> Sprites = null;
		/// <summary>
		/// List of parsed events loaded from <see cref="AssetManager.GameContentEventDataPath"./>
		/// Event entries are keyed by event ID and conditions.
		/// </summary>
		internal static List<Dictionary<string, string>> EventData = null;

		// others
		internal static int ModUpdateKey;
		internal static int EventRootId => ModEntry.ModUpdateKey * 10000;
		internal const string CommandPrefix = "rgb.";
		internal const string EndOfNightState = "blueberry.rgb.endofnightmenu";


		public override void Entry(IModHelper helper)
		{
			ModEntry.Instance = this;
			ModEntry.Config = helper.ReadConfig<Config>();
			ModEntry.ModUpdateKey = int.Parse(this.ModManifest.UpdateKeys.First().Split(':')[1]);

			helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
		}

		private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
		{
			this.Helper.Events.GameLoop.OneSecondUpdateTicked += this.Event_LoadLate;
		}

		private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			this.SaveLoadedBehaviours();
		}

		private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
		{
			Log.T($"Start of day: Y{Game1.year}/M{1 + Utility.getSeasonNumber(Game1.currentSeason)}/D{Game1.dayOfMonth}");

			// Perform OnSaveLoaded behaviours when starting a new game
			bool isNewGame = Game1.dayOfMonth == 1 && Game1.currentSeason == "spring" && Game1.year == 1;
			if (isNewGame)
			{
				this.SaveLoadedBehaviours();
			}

			// Log root event status
			if (!Game1.player.eventsSeen.Contains(ModEntry.EventRootId))
			{
				bool checkRawConditions (string s)
				{
					return Game1.getFarm().checkEventPrecondition($"{ModEntry.EventRootId}/{s}") != -1;
				};
				string conditions = ModEntry.EventData[0]["Conditions"];
				string checkedConditions = string.Join("/",
					conditions
						.Split('/')
						.ToList()
						.Select(s => checkRawConditions(s)));
				Log.T($"Player has not seen root event."
					+ $"{Environment.NewLine}Preconditions: ({conditions} == {checkedConditions} == {checkRawConditions(conditions)})");
			}
			
			// Add always-available recipes to player list without any unique fanfare
			ModEntry.AddDefaultRecipes();
		}

		private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
		{
			// Break ready objects at the start of each season
			if (ModEntry.Config.RaisedBedsMayBreakWithAge && Game1.dayOfMonth == 28)
			{
				Log.T($"Performing end-of-season breakage: Y{Game1.year}/M{1 + Utility.getSeasonNumber(Game1.currentSeason)}/D{Game1.dayOfMonth}");
				OutdoorPot.BreakAll();
			}
		}

		private void Specialized_LoadStageChanged(object sender, LoadStageChangedEventArgs e)
		{
			if (e.NewStage == StardewModdingAPI.Enums.LoadStage.Loaded && !Context.IsMainPlayer)
			{
				Log.T("Invalidating assets on connected for multiplayer peer.");

				this.Helper.Content.InvalidateCache(Path.Combine("Data", "BigCraftablesInformation"));
				this.Helper.Content.InvalidateCache(Path.Combine("Data", "CraftingRecipes"));
				this.Helper.Content.InvalidateCache(Path.Combine("TileSheets", "Craftables"));
			}
		}

		private void SpaceEvents_ShowNightEndMenus(object sender, SpaceCore.Events.EventArgsShowNightEndMenus e)
		{
			// Add and show any newly-available object recipes to player list at the end of day screens
			List<string> newVarieties = ModEntry.AddNewAvailableRecipes();
			if (newVarieties.Count > 0)
			{
				Log.T(newVarieties.Aggregate($"Unlocked {newVarieties.Count} new recipes:", (str, s) => $"{str}{Environment.NewLine}{s}"));

				NewRecipeMenu.Push(newVarieties);
			}
		}

		private void Event_LoadLate(object sender, OneSecondUpdateTickedEventArgs e)
		{
			this.Helper.Events.GameLoop.OneSecondUpdateTicked -= this.Event_LoadLate;

			if (this.LoadAPIs())
			{
				this.Initialise();
			}
		}

		private bool LoadAPIs()
		{
			Log.T("Loading mod-provided APIs.");
			ISpaceCoreAPI spacecoreAPI = this.Helper.ModRegistry.GetApi<ISpaceCoreAPI>("spacechase0.SpaceCore");
			if (spacecoreAPI == null)
			{
				// Skip all mod behaviours if we fail to load the objects
				Log.E($"Couldn't access mod-provided API for SpaceCore.{Environment.NewLine}Garden beds will not be available, and no changes will be made.");
				return false;
			}

			spacecoreAPI.RegisterSerializerType(typeof(OutdoorPot));

			return true;
		}

		private void Initialise()
		{
			Log.T("Initialising mod data.");

			// Assets
			AssetManager assetManager = new AssetManager(helper: this.Helper);
			this.Helper.Content.AssetLoaders.Add(assetManager);
			this.Helper.Content.AssetEditors.Add(assetManager);

			// Content
			Translations.Initialise();
			this.LoadContentPacks();
			this.AddGenericModConfigMenu();

			// Patches
			HarmonyPatches.Patch(id: this.ModManifest.UniqueID);

			// Events
			this.Helper.Events.Specialized.LoadStageChanged += this.Specialized_LoadStageChanged;
			this.Helper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
			this.Helper.Events.GameLoop.DayStarted += this.GameLoop_DayStarted;
			this.Helper.Events.GameLoop.DayEnding += this.GameLoop_DayEnding;
			SpaceCore.Events.SpaceEvents.ShowNightEndMenus += this.SpaceEvents_ShowNightEndMenus;

			// Console commands
			this.Helper.ConsoleCommands.Add(
				name: ModEntry.CommandPrefix + "eventget",
				documentation: $"Check if event has been seen.{Environment.NewLine}Optional event ID, defaults to root event.",
				callback: ModEntry.Cmd_IsEventSeen);
			this.Helper.ConsoleCommands.Add(
				name: ModEntry.CommandPrefix + "eventset",
				documentation: $"Set state for having seen any event.{Environment.NewLine}Optional event ID, defaults to root event.",
				callback: ModEntry.Cmd_ToggleEventSeen);
			this.Helper.ConsoleCommands.Add(
				name: ModEntry.CommandPrefix + "give",
				documentation: $"Give several of all currently unlocked varieties of raised beds.",
				callback: ModEntry.Cmd_Give);
			this.Helper.ConsoleCommands.Add(
				name: ModEntry.CommandPrefix + "giveall",
				documentation: "Give several of all varieties of raised beds.",
				callback: ModEntry.Cmd_GiveAll);
		}

		private void AddGenericModConfigMenu()
		{
			IGenericModConfigMenuAPI modconfigAPI = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
			if (modconfigAPI != null)
			{
				modconfigAPI.RegisterModConfig(
					mod: this.ModManifest,
					revertToDefault: () => ModEntry.Config = new Config(),
					saveToFile: () => this.Helper.WriteConfig(ModEntry.Config));
				modconfigAPI.SetDefaultIngameOptinValue(
					mod: this.ModManifest,
					optedIn: true);
				System.Reflection.PropertyInfo[] properties = ModEntry.Config
					.GetType()
					.GetProperties()
					.Where(p => p.PropertyType == typeof(bool))
					.ToArray();
				foreach (System.Reflection.PropertyInfo property in properties)
				{
					string key = property.Name.ToLower();
					string description = Translations.GetTranslation($"config.{key}.description");
					modconfigAPI.RegisterSimpleOption(
						mod: this.ModManifest,
						optionName: Translations.GetTranslation($"config.{key}.name"),
						optionDesc: string.IsNullOrWhiteSpace(description) ? null : description,
						optionGet: () => (bool)property.GetValue(ModEntry.Config),
						optionSet: (bool value) => property.SetValue(ModEntry.Config, value: value));
				}
			}
		}

		private void SaveLoadedBehaviours()
		{
			Log.T($"Adding endOfNightStatus definition: {ModEntry.EndOfNightState}");
			Game1.player.team.endOfNightStatus.AddSpriteDefinition(
				key: ModEntry.EndOfNightState,
				file: AssetManager.GameContentEndOfNightSpritesPath,
				x: 48, y: 0, width: 16, height: 16);

			Game1.content.Load  // Return value unused; event data is set in AssetManager.Edit()
				<Dictionary<string, object>>
				(AssetManager.GameContentEventDataPath);

			// Reinitialise objects to recalculate XmlIgnore values
			if (Context.IsMainPlayer)
			{
				OutdoorPot.ArrangeAll();
			}
			else
			{
				OutdoorPot.ArrangeAllOnNextTick();
			}
		}

		public void LoadContentPacks()
		{
			ModEntry.ItemDefinitions = new Dictionary<string, ItemDefinition>();
			ModEntry.Sprites = new Dictionary<string, Texture2D>();

			List<IContentPack> contentPacks = this.Helper.ContentPacks.GetOwned().ToList();
			foreach (IContentPack contentPack in contentPacks)
			{
				string packKey = contentPack.Manifest.UniqueID;
				var sprites = contentPack.LoadAsset
					<Texture2D>
					(ItemDefinition.SpritesFile);
				var data = contentPack.ReadJsonFile
					<Dictionary<string, ItemDefinition>>
					(ItemDefinition.DefinitionsFile);

				// For some quality assurance, we check that there are an equal number of entries in the
				// ItemDefinitions dictionary as there are sprites in the shared framework spritesheet.

				const int minWidth = 160;
				const int minHeight = Game1.smallestTileSize * 2;
				if (sprites.Width < minWidth)
				{
					Log.W($"Did not load content pack {packKey}:{Environment.NewLine}Spritesheet does not meet minimum width (required {minWidth}, found {sprites.Width}).");
					continue;
				}
				if (sprites.Height % minHeight != 0)
				{
					Log.W($"While loading content pack {packKey}:{Environment.NewLine}Found spritesheet with unexpected height (expected multiple of {minHeight}, found {sprites.Height}).{Environment.NewLine}Some variants may fail to load.");
				}

				int numberOfSprites = sprites.Height / minHeight;
				string warnMessage = null;

				Log.T($"Loading content pack {packKey}:{Environment.NewLine}{data.Count} item variant entries and {numberOfSprites} spritesheet entries.");

				int difference = Math.Abs(numberOfSprites - data.Count);
				if (difference != 0)
				{
					warnMessage = $"Found {difference} partially-defined garden beds.";
				}

				if (warnMessage != null)
				{
					Log.W(warnMessage);

					// Remove items until number is within spritesheet bounds
					while (data.Count > numberOfSprites)
					{
						string key = data.Last().Key;
						if (data.Remove(key))
							Log.W($"Removing excess raised bed: {key}");
						else
							Log.E($"Failed to remove excess raised bed: {key}");
					}
				}

				int parentSheetIndex = 0;
				foreach (KeyValuePair<string, ItemDefinition> entry in data)
				{
					string variantKey = $"{packKey}.{entry.Key}";

					// Parse temp values for each entry
					entry.Value.ContentPack = contentPack;
					entry.Value.LocalName = entry.Key;
					entry.Value.SpriteKey = packKey;
					entry.Value.SpriteIndex = parentSheetIndex++;

					// Set default DaysToBreak values to unbreakable
					if (entry.Value.DaysToBreak <= 0)
					{
						entry.Value.DaysToBreak = 99999;
					}

					ModEntry.ItemDefinitions.Add(variantKey, entry.Value);
				}

				// To avoid having to keep many separate spritesheet images updated with any changes,
				// the content pack folder's extra sprite image files required for "ReserveExtraIndexCount"
				// are left blank.
				// We patch the sprites to the game tilesheet in-place where they'd otherwise have appeared,
				// which lets us consolidate all of our sprites into the one framework spritesheet.

				// Patch basic object sprites to game craftables sheet for all variants
				// Compiled sprites are patched in individual regions per sheet index

				// Object sprites are patched in 2 steps, soil and object, since sprites are taken
				// directly from the framework sprite, which stores them separately in order to
				// have the variant's unique soil sprite change when watered.
				if (data.Count > 0)
				{
					IAssetData asset = this.Helper.Content.GetPatchHelper(sprites);
					Rectangle destination = Rectangle.Empty;
					Rectangle source;
					int width = Game1.smallestTileSize;
					// soil
					source = new Rectangle(OutdoorPot.SoilIndexInSheet * width, 0, width, width);
					for (int i = 0; i < data.Count; ++i)
					{
						int yOffset = (width * 2 * i) + (width - data[data.Keys.ElementAt(i)].SoilHeightAboveGround);
						destination = new Rectangle(OutdoorPot.PreviewIndexInSheet * width, yOffset, width, width);
						asset.AsImage().PatchImage(
							source: sprites,
							sourceArea: source,
							targetArea: destination,
							patchMode: PatchMode.Overlay);
					}
					// object
					source = new Rectangle(0, 0, width, sprites.Height);
					destination = new Rectangle(destination.X, 0, width, sprites.Height);
					asset.AsImage().PatchImage(
						source: sprites,
						sourceArea: source,
						targetArea: destination,
						patchMode: PatchMode.Overlay);
				}
				ModEntry.Sprites.Add(packKey, sprites);
			}

			Log.T($"Loaded {contentPacks.Count} content pack(s) containing {ModEntry.ItemDefinitions.Count} valid objects.");
		}

		public static void AddDefaultRecipes()
		{
			List<string> recipesToAdd = new List<string>();
			int[] eventsSeen = Game1.player.eventsSeen.ToArray();
			string precondition = $"{ModEntry.EventRootId}/{ModEntry.EventData[0]["Conditions"]}";
			int rootEventReady = Game1.getFarm().checkEventPrecondition(precondition);
			bool hasOrWillSeeRootEvent = eventsSeen.Contains(ModEntry.EventRootId) || rootEventReady != -1;
			for (int i = 0; i < ModEntry.ItemDefinitions.Count; ++i)
			{
				string variantKey = ModEntry.ItemDefinitions.Keys.ElementAt(i);
				string craftingRecipeName = OutdoorPot.GetNameFromVariantKey(variantKey: variantKey);
				bool isAlreadyKnown = Game1.player.craftingRecipes.ContainsKey(craftingRecipeName);
				bool isDefaultRecipe = ModEntry.ItemDefinitions[variantKey].RecipeIsDefault;
				bool isInitialEventRecipe = string.IsNullOrEmpty(ModEntry.ItemDefinitions[variantKey].RecipeConditions);
				bool shouldAdd = ModEntry.Config.RecipesAlwaysAvailable || isDefaultRecipe || (hasOrWillSeeRootEvent && isInitialEventRecipe);

				if (!isAlreadyKnown && shouldAdd)
				{
					recipesToAdd.Add(craftingRecipeName);
				}
			}
			if (recipesToAdd.Count > 0)
			{
				Log.T($"Adding {recipesToAdd.Count} default recipes:{recipesToAdd.Aggregate(string.Empty, (str, s) => $"{str}{Environment.NewLine}{s}")}");

				for (int i = 0; i < recipesToAdd.Count; ++i)
				{
					Game1.player.craftingRecipes.Add(recipesToAdd[i], 0);
				}
			}
		}

		public static List<string> AddNewAvailableRecipes()
		{
			List<string> newVariants = new List<string>();
			for (int i = 0; i < ModEntry.ItemDefinitions.Count; ++i)
			{
				string variantKey = ModEntry.ItemDefinitions.Keys.ElementAt(i);
				string itemName = OutdoorPot.GetNameFromVariantKey(variantKey);

				if (Game1.player.craftingRecipes.ContainsKey(itemName)
					|| string.IsNullOrEmpty(ModEntry.ItemDefinitions[variantKey].RecipeConditions)
					|| !Game1.player.eventsSeen.Contains(ModEntry.EventRootId))
				{
					continue;
				}

				int eventID = ModEntry.EventRootId + i;
				string eventKey = $"{eventID.ToString()}/{ModEntry.ItemDefinitions[variantKey].RecipeConditions}";
				int precondition = Game1.getFarm().checkEventPrecondition(eventKey);
				if (precondition != -1)
				{
					newVariants.Add(variantKey);
					Game1.player.craftingRecipes.Add(itemName, 0);
				}
			}
			return newVariants;
		}

		private static void Give(string variantKey, int quantity)
		{
			OutdoorPot item = new OutdoorPot(variantKey: variantKey, tileLocation: Vector2.Zero)
			{
				Stack = quantity
			};
			if (!Game1.player.addItemToInventoryBool(item))
			{
				Log.D($"Inventory full: Did not add {variantKey} raised bed.");
			}
		}

		public static void Cmd_Give(string s, string[] args)
		{
			const int defaultQuantity = 25;
			int quantity = args.Length == 0
				? defaultQuantity
				: int.TryParse(args[0], out int argQuantity)
					? argQuantity
					: defaultQuantity;

			Log.D($"Adding {quantity} of each unlocked raised bed. Use '{ModEntry.CommandPrefix}giveall' to add all varieties.");

			IEnumerable<string> unlockedKeys = Game1.player.craftingRecipes.Keys
				.Where(recipe => recipe.StartsWith(OutdoorPot.GenericName))
				.Select(recipe => OutdoorPot.GetVariantKeyFromName(recipe));
			if (!unlockedKeys.Any())
			{
				Log.D($"No raised bed recipes are unlocked! Use '{ModEntry.CommandPrefix}giveall' to add all varieties.");
			}
			else foreach (string variantKey in unlockedKeys)
			{
				ModEntry.Give(variantKey: variantKey, quantity: quantity);
			}
		}

		public static void Cmd_GiveAll(string s, string[] args)
		{
			const int defaultQuantity = 25;
			int quantity = args.Length == 0
				? defaultQuantity
				: int.TryParse(args[0], out int argQuantity)
					? argQuantity
					: defaultQuantity;

			Log.D($"Adding {quantity} of all raised beds. Use '{ModEntry.CommandPrefix}give' to add unlocked varieties only.");

			foreach (string variantKey in ModEntry.ItemDefinitions.Keys)
			{
				ModEntry.Give(variantKey: variantKey, quantity: quantity);
			}
		}

		public static void Cmd_IsEventSeen(string s, string[] args)
		{
			int eventId = args.Length > 0 && int.TryParse(args[0], out int argId)
				? argId
				: ModEntry.EventRootId;

			Log.D($"Player {(Game1.player.eventsSeen.Contains(eventId) ? "has" : "has not")} seen event {eventId}.");
		}

		public static void Cmd_ToggleEventSeen(string s, string[] args)
		{
			int eventId = args.Length > 0 && int.TryParse(args[0], out int argId)
				? argId
				: ModEntry.EventRootId;
			if (Game1.player.eventsSeen.Contains(eventId))
			{
				Game1.player.eventsSeen.Remove(eventId);
			}
			else
			{
				Game1.player.eventsSeen.Add(eventId);
			}
			ModEntry.Cmd_IsEventSeen(s: s, args: args);
		}
	}
}
