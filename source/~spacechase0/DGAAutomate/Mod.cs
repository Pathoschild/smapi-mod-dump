/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using Pathoschild.Stardew.Automate;
using SpaceShared;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace DGAAutomate
{
    internal class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;
        public override void Entry(IModHelper helper)
        {
            Mod.instance = this;
            Log.Monitor = this.Monitor;

            this.Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var automate = this.Helper.ModRegistry.GetApi<IAutomateAPI>("Pathoschild.Automate");
            automate.AddFactory(new MyAutomationFactory());
        }
    }
}
