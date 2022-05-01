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
	public class ModEntry : StardewModdingAPI.Mod
	{
		private const string qualityScrubberType = "Quality Scrubber";

		private ModConfig? config;

		private QualityScrubberController? controller;


		/*********
		** Public methods
		*********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			config = helper.ReadConfig<ModConfig>();

			controller = new QualityScrubberController(Monitor, config);

			helper.Events.Input.ButtonPressed += this.OnButtonPressed;
		}

		
		public override object GetApi()
		{
			return new QualityScrubberApi(controller!);
		}


		/*********
		** Private methods
		*********/
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

				if (machine != null && machine.Name == qualityScrubberType)
				{
					// See if the machine accepts the item, suppress the input to prevent the eating menu from opening
					if (controller!.CanProcess(Game1.player.ActiveObject, machine))
					{
						controller.StartProcessing(Game1.player.ActiveObject, machine, Game1.player);
						Helper.Input.Suppress(e.Button);
					}
				}
			}
		}
	}
}