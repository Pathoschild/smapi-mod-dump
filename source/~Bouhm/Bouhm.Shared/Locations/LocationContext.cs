/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Bouhm.Shared.Locations
{
    // Used for determining if an NPC is in the same root location
    // as the player as well as indoor location a room belongs to
    internal class LocationContext
    {
        public LocationType Type { get; set; } // Outdoors, Building, or Room
        public string Root { get; set; } // Top-most outdoor location
        public string Parent { get; set; } // Level above
        public Dictionary<string, Vector2> Neighbors { get; set; } = new(); // Connected outdoor locations
        public List<string> Children { get; set; } // Levels below
        public Vector2 Warp { get; set; } // Position of warp
    }
}
