/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// Content source for fishing data. Registering a global binding for this interface will allow
    /// custom fishing content to be added to the game.
    /// </summary>
    public interface IFishingContentSource
    {
        /// <summary>
        /// Reloads the fishing data.
        /// </summary>
        /// <returns>The reloaded fishing data.</returns>
        IEnumerable<FishingContent> Reload();
    }
}