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
        /// <summary>Tasks performed when the the game's clock time changes, i.e. every 10 in-game minutes. (Note: This event *sometimes* fires at 6:00AM: apaprently every load after the first.)</summary>
        private void TimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't do anything; most of this mod's functions should be limited to the host player

            if (e.NewTime != 600) //if it's not currently 6:00AM
            {
                Generation.SpawnTimedSpawns(Utility.TimedSpawns, e.NewTime); //spawn anything set to appear at the current time
            }
        }
    }
}
