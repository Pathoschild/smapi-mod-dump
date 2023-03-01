/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/urbanyeti/stardew-better-ranching
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;

namespace BetterRanching
{
	/// <summary>The mod entry class loaded by SMAPI.</summary>
	public class BetterRanching : Mod
	{
		private FarmAnimal AnimalBeingRanched { get; set; }
		private ModConfig Config { get; set; }
		private BetterRanchingApi Api { get; set; }

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			Config = helper.ReadConfig<ModConfig>();
			Api = new BetterRanchingApi(Config);
			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			helper.Events.Display.RenderedWorld += OnRenderedWorld;
			helper.Events.Input.ButtonPressed += OnButtonPressed;
		}

		public override object GetApi()
		{
			return new BetterRanchingApi(Config);
		}

		/// <summary>
		///     Raised after the game is launched, right before the first update tick. This happens once per game session
		///     (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up
		///     mod integrations.
		/// </summary>
		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			// get Generic Mod Config Menu's API (if it's installed)
			var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (configMenu is null) return;

			// register mod
			configMenu.Register(
				ModManifest,
				() => Config = new ModConfig(),
				() => Helper.WriteConfig(Config)
			);

			// add some config options
			configMenu.AddBoolOption(
				ModManifest,
				name: () => this.Helper.Translation.Get("config.option.failed_harvest.name"),
				tooltip: () => this.Helper.Translation.Get("config.option.failed_harvest.description"),
				getValue: () => Config.PreventFailedHarvesting,
				setValue: value => Config.PreventFailedHarvesting = value
			);

			configMenu.AddBoolOption(
				ModManifest,
				name: () => this.Helper.Translation.Get("config.option.animal_produce.name"),
				tooltip: () => this.Helper.Translation.Get("config.option.animal_produce.description"),
				getValue: () => Config.DisplayProduce,
				setValue: value => Config.DisplayProduce = value
			);

			configMenu.AddBoolOption(
				ModManifest,
				name: () => this.Helper.Translation.Get("config.option.hearts_animal.name"),
				tooltip: () => this.Helper.Translation.Get("config.option.hearts_animal.description"),
				getValue: () => Config.DisplayFarmAnimalHearts,
				setValue: value => Config.DisplayFarmAnimalHearts = value
			);

			configMenu.AddBoolOption(
				ModManifest,
				name: () => this.Helper.Translation.Get("config.option.hearts_pet.name"),
				tooltip: () => this.Helper.Translation.Get("config.option.hearts_pet.description"),
				getValue: () => Config.DisplayPetHearts,
				setValue: value => Config.DisplayPetHearts = value
			);

			configMenu.AddBoolOption(
				ModManifest,
				name: () => this.Helper.Translation.Get("config.option.hearts_enabled.name"),
				tooltip: () => this.Helper.Translation.Get("config.option.hearts_enabled.description"),
				getValue: () => Config.DisplayHearts,
				setValue: value => Config.DisplayHearts = value
			);

			configMenu.AddBoolOption(
				ModManifest,
				name: () => this.Helper.Translation.Get("config.option.hearts_max_friendship.name"),
				tooltip: () => this.Helper.Translation.Get("config.option.hearts_max_friendship.description"),
				getValue: () => Config.HideHeartsWhenFriendshipIsMax,
				setValue: value => Config.HideHeartsWhenFriendshipIsMax = value
			);
		}

		/// <summary>Raised after the game state is updated (≈60 times per second).</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			//Override auto-click on hold for milk pail
			if (Config.PreventFailedHarvesting && GameExtensions.HoldingOverridableTool() &&
			    GameExtensions.IsClickableArea() && Game1.mouseClickPolling > 50)
				Game1.mouseClickPolling = 50;

			if (!Game1.player.UsingTool && AnimalBeingRanched != null) AnimalBeingRanched = null;
		}

		/// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			if (!Context.IsWorldReady || Game1.currentLocation is not { IsFarm: true }) return;

			if (!e.Button.IsUseToolButton() || !Config.PreventFailedHarvesting ||
			    !GameExtensions.HoldingOverridableTool() || !GameExtensions.IsClickableArea()) return;

			var who = Game1.player;
			var position = !Game1.wasMouseVisibleThisFrame
				? Game1.player.GetToolLocation()
				: new Vector2(Game1.getOldMouseX() + Game1.viewport.X,
					Game1.getOldMouseY() + Game1.viewport.Y);
			var (x, y) = Game1.player.GetToolLocation(position);
			var toolRect = new Rectangle((int)x - 32, (int)y - 32, 64, 64);

			AnimalBeingRanched = Game1.currentLocation switch
			{
				Farm => Utility.GetBestHarvestableFarmAnimal(
					((Farm)Game1.currentLocation).animals.Values, Game1.player.CurrentTool,
					toolRect),
				AnimalHouse => Utility.GetBestHarvestableFarmAnimal(
					((AnimalHouse)Game1.currentLocation).animals.Values,
					Game1.player.CurrentTool, toolRect),
				_ => AnimalBeingRanched
			};

			if (AnimalBeingRanched == null || AnimalBeingRanched.currentProduce.Value < 1 ||
			    AnimalBeingRanched.age.Value < AnimalBeingRanched.ageWhenMature.Value)
				OverrideRanching(Game1.currentLocation, (int)who.GetToolLocation().X, (int)who.GetToolLocation().Y, who,
					e.Button, who.CurrentTool?.Name);
		}

		private void OverrideRanching(GameLocation currentLocation, int x, int y, Farmer who, SButton button,
			string toolName)
		{
			AnimalBeingRanched = null;
			FarmAnimal animal = null;
			var ranchAction = string.Empty;
			var ranchActionPresent = string.Empty;
			var ranchProduct = string.Empty;

			if (toolName == null) return;

			switch (toolName)
			{
				case GameConstants.Tools.MilkPail:
					ranchAction = Helper.Translation.Get("action.unable.milk");
					ranchActionPresent = Helper.Translation.Get("action.out_of_range.milk");
					ranchProduct = Helper.Translation.Get("product.milk");
					break;
				case GameConstants.Tools.Shears:
					ranchAction = Helper.Translation.Get("action.unable.shear");
					ranchActionPresent = Helper.Translation.Get("action.out_of_range.shear");
					ranchProduct = Helper.Translation.Get("product.wool");
					break;
			}

			var rectangle = new Rectangle(x - Game1.tileSize / 2, y - Game1.tileSize / 2, Game1.tileSize,
				Game1.tileSize);

			if (currentLocation is IAnimalLocation animalLocation) animal = animalLocation.GetSelectedAnimal(rectangle);

			if (animal == null)
			{
				Helper.Input.OverwriteState(button, Helper.Translation.Get("notification.out_of_range", new { ranchActionPresent = ranchActionPresent }));
				return;
			}

			if (animal.CanBeRanched(toolName))
			{
				if (who.couldInventoryAcceptThisObject(animal.currentProduce.Value, 1))
					AnimalBeingRanched = animal;
				else
					Helper.Input.OverwriteState(button, Helper.Translation.Get("notification.inventory_full"));
			}
			else if (animal.isBaby() && animal.toolUsedForHarvest.Value.Equals(toolName))
			{
				Helper.Input.OverwriteState(button);
				DelayedAction.showDialogueAfterDelay(
					Helper.Translation.Get("notification.util_action", new { Name = animal.Name, Product = ranchProduct, Days = animal.ageWhenMature.Value - animal.age.Value }),
					0);
			}
			else
			{
				Helper.Input.OverwriteState(button, Helper.Translation.Get("notification.unable", new { Action = ranchAction }));
			}
		}

		private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
		{
			if (!Context.IsWorldReady || !Game1.currentLocation.IsFarm || Game1.eventUp) return;

			var currentLocation = Game1.currentLocation;

			var farmAnimalList = new List<FarmAnimal>();
			switch (currentLocation)
			{
				case AnimalHouse animalHouse:
					farmAnimalList = animalHouse.animals.Values.ToList();
					break;
				case Farm farm:
					farmAnimalList = farm.animals.Values.ToList();
					break;
			}

			foreach (var farmAnimal in farmAnimalList)
				Api.DrawItemBubble(Game1.spriteBatch, farmAnimal, AnimalBeingRanched == farmAnimal);

			if (!Config.DisplayPetHearts || Game1.eventUp) return;

			foreach (var npc in currentLocation.characters)
				if (npc is Pet pet)
					Api.DrawHeartBubble(Game1.spriteBatch, pet,
						() => !pet.lastPetDay.TryGetValue(Game1.player.UniqueMultiplayerID, out var lastValue) || lastValue != Game1.Date.TotalDays);
		}
	}
}