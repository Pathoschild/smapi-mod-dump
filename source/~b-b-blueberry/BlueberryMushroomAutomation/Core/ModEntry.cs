/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/BlueberryMushroomMachine
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace BlueberryMushroomAutomation
{
	public class ModEntry : Mod
	{
		public override void Entry(IModHelper helper)
		{
			Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
		}

		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			// Automate setup
			var automateApi = Helper.ModRegistry.GetApi<Core.IAutomateAPI>("Pathoschild.Automate");
			automateApi.AddFactory(new Core.PropagatorFactory());
		}
	}
}
