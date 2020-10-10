/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium/Stardew_Valley_Mods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace YourProjectName
{
    public class RealTime : Mod
    {
        private int time;
        
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += onUpdate;
        }

        private int currentTime;
        private void onUpdate(object sneder,UpdateTickedEventArgs e)
        {
            DateTime dateTime = new DateTime();
            dateTime = DateTime.Now;
            Game1.timeOfDay = dateTime.Hour * 100 + dateTime.Minute;
        }
    }
}