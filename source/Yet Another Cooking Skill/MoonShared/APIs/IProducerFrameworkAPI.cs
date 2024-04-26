/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace MoonShared.APIs
{
    public interface IProducerFrameworkAPI
    {
        /// <summary>
        /// Adds a content pack from the specified directory.
        /// This method expects a content-pack.json file instead of a manifest.json
        /// </summary>
        /// <param name="directory">The absolute path of the content pack.</param>
        /// <returns>true if the content pack was successfully loaded, otherwise false.</returns>
        bool AddContentPack(string directory);
    }
}
