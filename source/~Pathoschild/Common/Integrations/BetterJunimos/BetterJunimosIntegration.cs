/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace Pathoschild.Stardew.Common.Integrations.BetterJunimos
{
    /// <summary>Handles the logic for integrating with the Better Junimos mod.</summary>
    internal class BetterJunimosIntegration : BaseIntegration<IBetterJunimosApi>
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The Junimo Hut coverage radius.</summary>
        public int MaxRadius { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
        /// <param name="monitor">Encapsulates monitoring and logging.</param>
        public BetterJunimosIntegration(IModRegistry modRegistry, IMonitor monitor)
            : base("Better Junimos", "hawkfalcon.BetterJunimos", "0.5.0", modRegistry, monitor)
        {
            if (base.IsLoaded)
                this.MaxRadius = this.ModApi.GetJunimoHutMaxRadius();
        }
    }
}
