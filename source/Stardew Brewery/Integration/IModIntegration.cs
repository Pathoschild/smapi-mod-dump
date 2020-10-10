/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JustCylon/stardew-brewery
**
*************************************************/

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
