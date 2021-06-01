/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace MoreGrass.Config
{
    /// <summary>The content pack configuration.</summary>
    public class ContentPackConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Whether default grass sprites should be drawn too.</summary>
        public bool EnableDefaultGrass { get; set; } = true;

        /// <summary>The locations that each specified grass is allowed to be in.</summary>
        public Dictionary<string, List<string>> WhiteListedGrass { get; set; } = new Dictionary<string, List<string>>();

        /// <summary>The locations that each specified grass isn't allowed to be in.</summary>
        public Dictionary<string, List<string>> BlackListedGrass { get; set; } = new Dictionary<string, List<string>>();

        /// <summary>The locations that this pack is allowed to retexture grass in.</summary>
        public List<string> WhiteListedLocations { get; set; } = new List<string>();

        /// <summary>The locations that this pack isn't allowed to retexture grass is.</summary>
        public List<string> BlackListedLocations { get; set; } = new List<string>();
    }
}
