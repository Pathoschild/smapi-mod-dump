/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Tasks performed after the game returns to the title screen.</summary>
        private void ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't do anything

            //clear data to avoid errors (e.g. when exiting to title and creating a new farm, "DayEnding" will fire before "DayStarted" and check this data)
            Utility.FarmDataList.Clear();
        }
    }
}
