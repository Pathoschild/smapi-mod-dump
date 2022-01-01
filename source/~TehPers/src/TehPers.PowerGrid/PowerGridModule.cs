/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using TehPers.Core.Api.DI;
using TehPers.Core.Api.Setup;
using TehPers.PowerGrid.World;
using TehPers.PowerGrid.Services;
using TehPers.PowerGrid.Services.Setup;

namespace TehPers.PowerGrid
{
    internal class PowerGridModule : ModModule
    {
        public override void Load()
        {
            // Setup
            this.Bind<ISetup>().To<NetworkWatcher>().InSingletonScope();

            // Services
            this.Bind<NetworkFinder>().ToSelf().InSingletonScope();
        }
    }
}