/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

// Synced NPC positions for multiplayer
using System.Collections.Generic;

namespace Bouhm.Shared.Locations
{
    internal class SyncedNpcLocationData
    {
        /*********
        ** Accessors
        *********/
        public Dictionary<string, LocationData> Locations { get; set; } = new();
    }
}
