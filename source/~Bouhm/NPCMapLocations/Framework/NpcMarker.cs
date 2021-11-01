/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace NPCMapLocations.Framework
{
    /// <summary>An NPC marker on the map.</summary>
    public class NpcMarker : SyncedNpcMarker
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The NPC's overworld character sprite.</summary>
        public Texture2D Sprite { get; set; }

        /// <summary>The pixel offset to apply when cropping the NPC's head from their sprite.</summary>
        public int CropOffset { get; set; }

        /// <summary>Whether the player has an open quest for the NPC.</summary>
        public bool HasQuest { get; set; }

        /// <summary>Whether to hide the marker from the map.</summary>
        public bool IsHidden { get; set; }

        /// <summary>The reason the NPC is hidden, if applicable.</summary>
        public string ReasonHidden { get; set; }

        /// <summary>The NPC's priority when multiple markers overlap on the map, where higher values are higher priority.</summary>
        public int Layer { get; set; } = 4;
    }
}
