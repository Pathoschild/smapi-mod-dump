/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Armitxes/StardewValley_UnlimitedPlayers
**
*************************************************/

using StardewModdingAPI;
using UnlimitedPlayers.Events.Display;
using UnlimitedPlayers.Events.GameLoop;

namespace UnlimitedPlayers
{
	public class ModEntry : Mod
	{
		public override void Entry(IModHelper helper)
		{
			ConfigParser parser = helper.ReadConfig<ConfigParser>();
			parser.Store();                // Now we can access the config from every class without helper or passing the instance
			LazyHelper.ModHelper = helper; // And here I am just absolutly lazy - terribly sorry >.<
			LazyHelper.ModEntry = this;    // There will always only be one valid instance + see above
			helper.Events.GameLoop.GameLaunched += new TickEvents().FirstUpdateTick;
			helper.Events.GameLoop.DayStarted += new DayEvents().DayStarted;
			helper.Events.Display.RenderingActiveMenu += new RenderingActiveMenuEvents().RenderingActiveMenu;
			helper.Events.Display.MenuChanged += new MenuEvents().MenuChanged;
			LazyHelper.ModEntry.Monitor.Log("Default player limit set to " + LazyHelper.PlayerLimit + " players.", LogLevel.Info);
		}
	}
}
