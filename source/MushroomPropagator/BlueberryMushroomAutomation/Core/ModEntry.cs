/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/BlueberryMushroomMachine
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace BlueberryMushroomAutomation
{
	public class ModEntry : Mod
	{
		public override void Entry(IModHelper helper)
		{
			this.Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
		}

		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			// Automate setup
			IAutomateAPI automateApi = this.Helper.ModRegistry.GetApi<IAutomateAPI>("Pathoschild.Automate");
			automateApi.AddFactory(new PropagatorFactory());
		}
	}
}
