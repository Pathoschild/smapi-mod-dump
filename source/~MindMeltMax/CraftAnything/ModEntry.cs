/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using CraftAnything.Integrations.BetterCrafting;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace CraftAnything
{
    internal class ModEntry : Mod
    {
        internal static IModHelper IHelper;

        internal static IMonitor IMonitor;

        public override void Entry(IModHelper helper)
        {
            IHelper = Helper;
            IMonitor = Monitor;

            Helper.Events.GameLoop.GameLaunched += onGameLaunched;
        }

        private void onGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            Patches.Patch(ModManifest.UniqueID);
            BetterCrafting.Register();
        }
    }
}
