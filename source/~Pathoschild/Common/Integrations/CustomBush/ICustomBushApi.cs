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
using System.Diagnostics.CodeAnalysis;
using StardewValley.TerrainFeatures;

namespace Pathoschild.Stardew.Common.Integrations.CustomBush
{
    /// <summary>The API provided by the Custom Bush mod.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "The naming convention is defined by the Custom Bush mod.")]
    public interface ICustomBushApi
    {
        /// <summary>Try to get the custom bush model associated with the given bush.</summary>
        /// <param name="bush">The bush.</param>
        /// <param name="customBush">When this method returns, contains the custom bush associated with the given bush, if found; otherwise, it contains null.</param>
        /// <returns>True if the custom bush associated with the given bush is found; otherwise, false.</returns>
        bool TryGetCustomBush(Bush bush, out ICustomBush? customBush);

        /// <summary>Try to get the custom bush model and id associated with the given bush.</summary>
        /// <param name="bush">The bush.</param>
        /// <param name="customBush">When this method returns, contains the custom bush associated with the given bush, if found; otherwise, it contains null.</param>
        /// <param name="id">The internal bush ID.</param>
        /// <returns>True if the custom bush associated with the given bush is found; otherwise, false.</returns>
        bool TryGetCustomBush(Bush bush, out ICustomBush? customBush, [NotNullWhen(true)] out string? id);

        /// <summary>Try to get the custom bush drop associated with the given bush id.</summary>
        /// <param name="id">The id of the bush.</param>
        /// <param name="drops">When this method returns, contains the items produced by the custom bush.</param>
        /// <returns>True if the drops associated with the given id is found; otherwise, false.</returns>
        bool TryGetDrops(string id, [NotNullWhen(true)] out IList<ICustomBushDrop>? drops);
    }
}
