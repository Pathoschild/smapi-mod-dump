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
        /// <summary>Tasks performed at the start of each in-game day.</summary>
        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            LoadTrackerSprites();
        }
    }
}
