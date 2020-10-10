/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal class LocationConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The secondary location configs to read. This is ignored on a secondary location config.</summary>
        public IList<string> Includes { get; set; } = new List<string>();

        public IList<Location> Locations { get; set; } = new List<Location>();
        public IList<Override> Overrides { get; set; } = new List<Override>();
        public IList<Redirect> Redirects { get; set; } = new List<Redirect>();
        public IList<Tilesheet> Tilesheets { get; set; } = new List<Tilesheet>();
        public IList<Tile> Tiles { get; set; } = new List<Tile>();
        public IList<Property> Properties { get; set; } = new List<Property>();
        public IList<Warp> Warps { get; set; } = new List<Warp>();
        public IList<Conditional> Conditionals { get; set; } = new List<Conditional>();
        public IList<TeleporterList> Teleporters { get; set; } = new List<TeleporterList>();
        public IList<string> Shops { get; set; } = new List<string>();
    }
}
