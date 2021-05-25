/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

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
        /// <summary>Tasks performed when the the game's clock time changes, i.e. every 10 in-game minutes. (Note: This event *sometimes* fires at 6:00AM: apparently every load after the first.)</summary>
        private void TimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't do anything; most of this mod's functions should be limited to the host player
            if (Utility.GameIsSaving) { return; } //if the game is currently saving, don't do anything; as of SDV v1.5.4, time occasionally changes during the save/day-ending process

            if (Utility.StartOfDay.Time < e.NewTime) //if the new time isn't the start of the day
            {
                Generation.SpawnTimedSpawns(Utility.TimedSpawns, e.NewTime); //spawn anything set to appear at the current time
            }
        }
    }
}
