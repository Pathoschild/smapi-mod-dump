/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/CustomTracker
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;

namespace CustomTracker
{
    /// <summary>The mod's main class.</summary>
    public partial class ModEntry : Mod
    {
        /// <summary>A representation of this mod's config.json data file.</summary>
        public class ModConfig
        {
            /// <summary>If true, trackers will be enabled even if the player doesn't have the Tracker profession.</summary>
            public bool EnableTrackersWithoutProfession = false;

            /// <summary>If true, an image of the forage being tracked will be displayed instead of the tracker icon.</summary>
            public bool ReplaceTrackersWithForageIcons = false;

            /// <summary>If true, trackers will be drawn behind the HUD. If false, they will be drawn in front of the HUD.</summary>
            public bool DrawBehindInterface = false;

            /// <summary>The scale at which the tracker's texture(s) will be rendered. The size of each pixel in the original texture is multiplied by this value.</summary>
            public float TrackerPixelScale = 4f;

            /// <summary>If true, objects with the "isSpawnedObject" flag should be tracked.</summary>
            public bool TrackDefaultForage = true;

            /// <summary>If true, artifact spots should be tracked.</summary>
            public bool TrackArtifactSpots = true;

            /// <summary>If true, panning spots should be tracked.</summary>
            public bool TrackPanningSpots = true;

            /// <summary>If true, spring onions should be tracked.</summary>
            public bool TrackSpringOnions = false;

            /// <summary>If true, harvestable berry bushes should be tracked.</summary>
            public bool TrackBerryBushes = false;

            /// <summary>If true, harvestable walnut bushes should be tracked.</summary>
            public bool TrackWalnutBushes = false;

            /// <summary>A list of additional object IDs and/or names that should be tracked.</summary>
            public List<object> OtherTrackedObjects = new List<object>();

            public ModConfig()
            {

            }
        }
    }
}
