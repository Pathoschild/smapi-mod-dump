/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace BetterDoors.Framework.ContentPacks
{
    /*********
    ** Accessors
    *********/

    /// <summary> The format content packs that provide door animations must follow.</summary>
    internal class ContentPack
    {
        /// <summary>The version of the content pack.</summary>
        public string Version { get; set; }

        /// <summary>The doors the content pack provides.</summary>
        public IDictionary<string, IList<string>> Doors { get; set; }
    }
}
