using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Randomizer
{
	/// <summary>The mod entry point</summary>
	public class ModEntry : Mod
	{
		public PossibleSwap[] PossibleSwaps = {
			new PossibleSwap("Pierre", "Lewis"),
			new PossibleSwap("Wizard", "Sandy"),
			new PossibleSwap("Willy", "Pam"),
			new PossibleSwap("Abigail", "Marnie"),
			new PossibleSwap("MrQi", "Gunther"),
			new PossibleSwap("Marlon", "Governor"),
			new PossibleSwap("Caroline", "Evelyn"),
			new PossibleSwap("Pam", "Haley"),
			new PossibleSwap("Morris", "Krobus"),
			new PossibleSwap("Gus", "Elliott"),
			new PossibleSwap("Linus", "Pam"),
			new PossibleSwap("Kent", "Pierre"),
			new PossibleSwap("Sandy", "Maru"),
			new PossibleSwap("Sebastian", "Wizard"),
			new PossibleSwap("Jas", "Vincent"),
			new PossibleSwap("Krobus", "Dwarf"),
			new PossibleSwap("Leah", "Marnie"),
			new PossibleSwap("Henchman", "Bouncer"),
			new PossibleSwap("Harvey", "Gus"),
			new PossibleSwap("Bouncer", "Gunther"),
			new PossibleSwap("Gunther", "Governor"),
			new PossibleSwap("Evelyn", "Jodi"),
			new PossibleSwap("George", "Wizard"),
			new PossibleSwap("Emily", "Marnie"),
			new PossibleSwap("Sam", "Linus"),
			new PossibleSwap("Alex", "Gus"),
			new PossibleSwap("Penny", "Sandy"),
			new PossibleSwap("Morris", "Governor"),
			new PossibleSwap("Haley", "Alex"),
			new PossibleSwap("Harvey", "Maru"),
			new PossibleSwap("Abigail", "Sebastian"),
			new PossibleSwap("Penny", "Sam"),
			new PossibleSwap("Leah", "Elliott"),
			new PossibleSwap("Shane", "Emily"),
			new PossibleSwap("Shane", "Pam")
		};

		private AssetLoader _modAssetLoader;
		private AssetEditor _modAssetEditor;

		private IModHelper _helper;

		/// <summary>The mod entry point, called after the mod is first loaded</summary>
		/// <param name="helper">Provides simplified APIs for writing mods</param>
		public override void Entry(IModHelper helper)
		{
			ImageBuilder.CleanUpReplacementFiles();

			_helper = helper;
			Globals.ModRef = this;
			Globals.Config = Helper.ReadConfig<ModConfig>();

			this._modAssetLoader = new AssetLoader(this);
			this._modAssetEditor = new AssetEditor(this);
			helper.Content.AssetLoaders.Add(this._modAssetLoader);
			helper.Content.AssetEditors.Add(this._modAssetEditor);

			this.PreLoadReplacments();
			helper.Events.GameLoop.SaveLoaded += (sender, args) => this.CalculateAllReplacements();
			helper.Events.Multiplayer.PeerContextReceived += (sender, args) => FixParsnipSeedBox();

			helper.Events.Display.RenderingActiveMenu += (sender, args) => _modAssetLoader.TryReplaceTitleScreen();
			helper.Events.Display.MenuChanged += BundleMenuAdjustments.FixRingSelection;
			helper.Events.Display.RenderingActiveMenu += (sender, args) => BundleMenuAdjustments.FixRingDeposits();
			helper.Events.GameLoop.ReturnedToTitle += (sender, args) => _modAssetLoader.ReplaceTitleScreenAfterReturning();
			helper.Events.GameLoop.ReturnedToTitle += (sender, args) => ImageBuilder.CleanUpReplacementFiles();

			if (Globals.Config.RandomizeMusic) { helper.Events.GameLoop.UpdateTicked += (sender, args) => this.TryReplaceSong(); }
			if (Globals.Config.RandomizeRain) { helper.Events.GameLoop.DayEnding += _modAssetLoader.ReplaceRain; }
			if (Globals.Config.RandomizeCrops || Globals.Config.RandomizeFish)
			{
				helper.Events.Display.RenderingActiveMenu += (sender, args) => CraftingRecipeAdjustments.HandleCraftingMenus();
			}

			helper.Events.GameLoop.DayStarted += (sender, args) => UseOverriddenSubmarine();
			helper.Events.GameLoop.DayEnding += (sender, args) => RestoreSubmarineLocation();
		}


		/// <summary>
		/// The old submarine location
		/// </summary>
		private Submarine NormalSubmarineLocation { get; set; }

		/// <summary>
		/// Replaces the submarine location with an overridden one so that the fish that
		/// appear there are correct
		/// </summary>
		public void UseOverriddenSubmarine()
		{
			int submarineIndex;
			foreach (GameLocation location in Game1.locations)
			{
				if (location.Name == "Submarine")
				{
					if (location.GetType() != typeof(OverriddenSubmarine))
					{
						NormalSubmarineLocation = (Submarine)location;
					}

					submarineIndex = Game1.locations.IndexOf(location);
					Game1.locations[submarineIndex] = new OverriddenSubmarine();
					break;
				}
			}
		}

		/// <summary>
		/// Restores the submarine location - this should be done before saving the game
		/// to avoid a crash
		/// </summary>
		public void RestoreSubmarineLocation()
		{
			if (NormalSubmarineLocation == null) { return; }

			int submarineIndex;
			foreach (GameLocation location in Game1.locations)
			{
				if (location.Name == "Submarine")
				{
					submarineIndex = Game1.locations.IndexOf(location);
					Game1.locations[submarineIndex] = NormalSubmarineLocation;
					break;
				}
			}
		}

		/// <summary>
		/// Loads the replacements that can be loaded before a game is selected
		/// </summary>
		public void PreLoadReplacments()
		{
			_modAssetLoader.CalculateReplacementsBeforeLoad();
			_modAssetEditor.CalculateEditsBeforeLoad();
		}

		/// <summary>
		/// Does all the randomizer replacements that take place after a game is loaded
		/// </summary>
		public void CalculateAllReplacements()
		{
			//Seed is pulled from farm name
			byte[] seedvar = (new SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(Game1.player.farmName.Value));
			int seed = BitConverter.ToInt32(seedvar, 0);

			this.Monitor.Log($"Seed Set: {seed}");

			Globals.RNG = new Random(seed);
			Globals.SpoilerLog = new SpoilerLogger(Game1.player.farmName.Value);

			// Make replacements and edits
			_modAssetLoader.CalculateReplacements();
			_modAssetEditor.CalculateEdits();
			_modAssetLoader.RandomizeImages();
			Globals.SpoilerLog.WriteFile();

			// Invalidate all replaced and edited assets so they are reloaded
			_modAssetLoader.InvalidateCache();
			_modAssetEditor.InvalidateCache();

			ChangeDayOneForagables();
			FixParsnipSeedBox();
			OverriddenSeedShop.ReplaceShopStockMethod();
			OverriddenAdventureShop.FixAdventureShopBuyAndSellPrices();
		}

		/// <summary>
		/// Fixes the foragables on day 1 - the save file is created too quickly for it to be
		/// randomized right away, so we'll change them on the spot on the first day
		/// </summary>
		public void ChangeDayOneForagables()
		{
			// Replace all the foragables on day 1
			SDate currentDate = SDate.Now();
			if (currentDate.DaysSinceStart < 2)
			{
				List<GameLocation> locations = Game1.locations
					.Concat(
						from location in Game1.locations.OfType<BuildableGameLocation>()
						from building in location.buildings
						where building.indoors.Value != null
						select building.indoors.Value
					).ToList();

				List<Item> newForagables =
				ItemList.GetForagables(Seasons.Spring)
					.Where(x => x.ShouldBeForagable) // Removes the 1/1000 items
					.Cast<Item>().ToList();

				foreach (GameLocation location in locations)
				{
					List<int> foragableIds = ItemList.GetForagables().Select(x => x.Id).ToList();
					List<Vector2> tiles = location.Objects.Pairs
						.Where(x => foragableIds.Contains(x.Value.ParentSheetIndex))
						.Select(x => x.Key)
						.ToList();

					foreach (Vector2 oldForagableKey in tiles)
					{
						Item newForagable = Globals.RNGGetRandomValueFromList(newForagables);
						location.Objects[oldForagableKey].ParentSheetIndex = newForagable.Id;
						location.Objects[oldForagableKey].Name = newForagable.Name;
					}
				}
			}
		}

		/// <summary>
		/// Fixes the item name that you get at the start of the game
		/// </summary>
		public void FixParsnipSeedBox()
		{
			GameLocation farmHouse = Game1.locations.Where(x => x.Name == "FarmHouse").First();

			List<StardewValley.Objects.Chest> chestsInRoom =
				farmHouse.Objects.Values.Where(x =>
					x.DisplayName == "Chest")
					.Cast<StardewValley.Objects.Chest>()
					.Where(x => x.giftbox.Value)
				.ToList();

			if (chestsInRoom.Count > 0)
			{
				string parsnipSeedsName = ItemList.GetItemName((int)ObjectIndexes.ParsnipSeeds);
				StardewValley.Item itemInChest = chestsInRoom[0].items[0];
				if (itemInChest.Name == "Parsnip Seeds")
				{
					itemInChest.Name = parsnipSeedsName;
					itemInChest.DisplayName = parsnipSeedsName;
				}
			}
		}

		/// <summary>
		/// The last song that played/is playing
		/// </summary>
		private string _lastCurrentSong { get; set; }

		/// <summary>
		/// Attempts to replace the current song with a different one
		/// If the song was barely replaced, it doesn't do anything
		/// </summary>
		public void TryReplaceSong()
		{
			//Game1.addHUDMessage(new HUDMessage(Game1.currentSong?.Name));

			string currentSong = Game1.currentSong?.Name;
			if (this._modAssetEditor.MusicReplacements.TryGetValue(currentSong?.ToLower() ?? "", out string value) && _lastCurrentSong != currentSong)
			{
				_lastCurrentSong = value;
				Game1.changeMusicTrack(value);
			}
		}
	}
}