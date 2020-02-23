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
        /// <summary>Tasks performed before rendering the HUD.</summary>
        private void Display_RenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (MConfig.DrawBehindInterface) //if trackers should be drawn "beneath" the interface
                RenderCustomTrackers(MConfig.ReplaceTrackersWithForageIcons);
        }
    }
}
