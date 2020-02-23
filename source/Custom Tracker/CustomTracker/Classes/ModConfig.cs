using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

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

            public ModConfig()
            {

            }
        }
    }
}
