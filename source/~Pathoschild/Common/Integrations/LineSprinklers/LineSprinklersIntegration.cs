/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace Pathoschild.Stardew.Common.Integrations.LineSprinklers
{
    /// <summary>Handles the logic for integrating with the Line Sprinklers mod.</summary>
    internal class LineSprinklersIntegration : BaseIntegration<ILineSprinklersApi>
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The maximum possible sprinkler radius.</summary>
        public int MaxRadius { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
        /// <param name="monitor">Encapsulates monitoring and logging.</param>
        public LineSprinklersIntegration(IModRegistry modRegistry, IMonitor monitor)
            : base("Line Sprinklers", "hootless.LineSprinklers", "1.1.0", modRegistry, monitor)
        {
            if (base.IsLoaded)
                this.MaxRadius = this.ModApi.GetMaxGridSize();
        }

        /// <summary>Get the configured Sprinkler tiles relative to (0, 0).</summary>
        public IDictionary<int, Vector2[]> GetSprinklerTiles()
        {
            this.AssertLoaded();
            return this.ModApi.GetSprinklerCoverage();
        }
    }
}
