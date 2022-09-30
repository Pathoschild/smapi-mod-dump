/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using Pathoschild.Stardew.Automate;
using StardewModdingAPI;

namespace Omegasis.RevitalizeAutomateCompatibility
{
    public class RevitalizeAutomateCompatibilityModCore : StardewModdingAPI.Mod
    {
        public override void Entry(IModHelper helper)
        {

            helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            IAutomateAPI automate = this.Helper.ModRegistry.GetApi<IAutomateAPI>("Pathoschild.Automate");
            automate.AddFactory(new RevitalizeAutomationFactory());
        }
    }
}
