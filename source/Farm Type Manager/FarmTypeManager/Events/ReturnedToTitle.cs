using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

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
