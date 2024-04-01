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
using StardewModdingAPI.Events;

namespace CustomTracker
{
    /// <summary>The mod's main class.</summary>
    public partial class ModEntry : Mod
    {
        /// <summary>Tasks performed after rendering the HUD.</summary>
        private void Display_RenderedHud(object sender, RenderedHudEventArgs e)
        {
            if (!Config.DrawBehindInterface) //if trackers should be drawn "above" the interface
                RenderCustomTrackers(Config.ReplaceTrackersWithForageIcons);
        }
    }
}
