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

namespace Pathoschild.Stardew.Common.Integrations.ExtraMachineConfig
{
    /// <summary>Handles the logic for integrating with the Extra Machine Config mod.</summary>
    internal class ExtraMachineConfigIntegration : BaseIntegration<IExtraMachineConfigApi>
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
        /// <param name="monitor">Encapsulates monitoring and logging.</param>
        public ExtraMachineConfigIntegration(IModRegistry modRegistry, IMonitor monitor)
            : base("ExtraMachineConfig", "selph.ExtraMachineConfig", "1.4.0", modRegistry, monitor) { }
    }
}
