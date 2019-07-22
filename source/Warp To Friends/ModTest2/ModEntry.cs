using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ModTest2
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		/*********
		** Public methods
		*********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			helper.Events.Input.ButtonPressed += this.OnButtonPressed;
		}

		/*********
		** Private methods
		*********/
		/// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			if (Context.IsWorldReady) // save is loaded
			{

			}
		}
	}
}
