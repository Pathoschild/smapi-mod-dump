/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/GreenhouseGatherers
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseGatherers.GreenhouseGatherers.API.Interfaces.ExpandedStorage
{
    public interface IExpandedStorageAPI
    {
        /// <summary>Load a directory as an Expanded Storage content pack.</summary>
        /// <param name="path">Path containing expandedStorage.json file.</param>
        /// <returns>True if content was loaded successfully.</returns>
        bool LoadContentPack(IManifest manifest, string path);
    }
}
