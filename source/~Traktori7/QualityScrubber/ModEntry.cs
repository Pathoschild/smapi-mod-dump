/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SObject = StardewValley.Object;


/// <summary>
/// TODO: Machine animation doesn't need PFM in 1.6
/// </summary>

namespace QualityScrubber
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		private const string qualityScrubberType = "Quality Scrubber";

		private ModConfig config = null!;
		private QualityScrubberController controller = null!;
		private ITranslationHelper i18n = null!;


		/*********
		** Public methods
		*********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			i18n = helper.Translation;

			config = helper.ReadConfig<ModConfig>();

			controller = new QualityScrubberController(Monitor, config);

			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.Input.ButtonPressed += OnButtonPressed;
		}
		

		public override object GetApi()
		{
			return new QualityScrubberApi(controller);
		}


		/*********
		** Private methods
		*********/
		private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
		{
			var GMCMApi = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

			if (GMCMApi is null)
				return;

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
				getValue: () => config.Duration,
				setValue: (int val) => config.Duration = val,
				name: () => i18n.Get("gmcm.duration-label"),
				tooltip: () => i18n.Get("gmcm.duration-description"),
				interval: 10,
				min: 10,
				max: 1000
			);

			GMCMApi.AddBoolOption(
				mod: ModManifest,
				getValue: () => config.AllowHoney,
				setValue: (bool val) => config.AllowHoney = val,
				name: () => i18n.Get("gmcm.allow-honey-label"),
				tooltip: () => i18n.Get("gmcm.allow-honey-description")
			);
		}


		/// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
		{
			// ignore if player hasn't loaded a save yet
			if (!Context.IsPlayerFree)
				return;

			if (e.Button.IsActionButton())
			{
				var objects = Game1.player.currentLocation.Objects;

				objects.TryGetValue(e.Cursor.GrabTile, out SObject machine);

				if (machine is not null && machine.Name == qualityScrubberType)
				{
					// See if the machine accepts the item, suppress the input to prevent the eating menu from opening
					if (controller.CanProcess(Game1.player.ActiveObject, machine))
					{
						controller.StartProcessing(Game1.player.ActiveObject, machine, Game1.player);
						Helper.Input.Suppress(e.Button);
					}
				}
			}
		}
	}
}