/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

#nullable disable

using Pathoschild.Stardew.Common.Integrations.Automate;
using Pathoschild.Stardew.Common.Integrations.BetterJunimos;
using Pathoschild.Stardew.Common.Integrations.BetterSprinklers;
using Pathoschild.Stardew.Common.Integrations.LineSprinklers;
using Pathoschild.Stardew.Common.Integrations.MultiFertilizer;
using Pathoschild.Stardew.Common.Integrations.PelicanFiber;
using Pathoschild.Stardew.Common.Integrations.SimpleSprinkler;
using StardewModdingAPI;

namespace Pathoschild.Stardew.DataLayers.Framework
{
    /// <summary>A container for the supported mod integrations.</summary>
    internal class ModIntegrations
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Handles access to the Automate mod.</summary>
        public AutomateIntegration Automate { get; }

        /// <summary>Handles access to the Better Junimos mod.</summary>
        public BetterJunimosIntegration BetterJunimos { get; }

        /// <summary>Handles access to the Better Sprinklers mod.</summary>
        public BetterSprinklersIntegration BetterSprinklers { get; }

        /// <summary>Handles access to the Line Sprinklers mod.</summary>
        public LineSprinklersIntegration LineSprinklers { get; }

        /// <summary>Handles access to the MultiFertilizer mod.</summary>
        public MultiFertilizerIntegration MultiFertilizer { get; }

        /// <summary>Handles access to the Pelican Fiber mod.</summary>
        public PelicanFiberIntegration PelicanFiber { get; }

        /// <summary>Handles access to the Simple Sprinkler mod.</summary>
        public SimpleSprinklerIntegration SimpleSprinkler { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="monitor">Encapsulates monitoring and logging.</param>
        /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
        /// <param name="reflection">An API for accessing private code.</param>
        public ModIntegrations(IMonitor monitor, IModRegistry modRegistry, IReflectionHelper reflection)
        {
            this.Automate = new AutomateIntegration(modRegistry, monitor);
            this.BetterJunimos = new BetterJunimosIntegration(modRegistry, monitor);
            this.BetterSprinklers = new BetterSprinklersIntegration(modRegistry, monitor);
            this.LineSprinklers = new LineSprinklersIntegration(modRegistry, monitor);
            this.MultiFertilizer = new MultiFertilizerIntegration(modRegistry, monitor);
            this.PelicanFiber = new PelicanFiberIntegration(modRegistry, reflection, monitor);
            this.SimpleSprinkler = new SimpleSprinklerIntegration(modRegistry, monitor);
        }
    }
}
