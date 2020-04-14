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