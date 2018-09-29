using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewBrewery.Integration
{
    /// <summary>Handles integration with a given mod.</summary>
    internal interface IModIntegration
    {
        /*********
        ** Accessors
        *********/
        /// <summary>A human-readable name for the mod.</summary>
        string Label { get; }

        /// <summary>Whether the mod is available.</summary>
        bool IsLoaded { get; }
    }
}
