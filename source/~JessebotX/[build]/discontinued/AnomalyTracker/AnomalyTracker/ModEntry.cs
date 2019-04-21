using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Framework;
using StardewModdingAPI.Utilities;

namespace AnomalyTracker
{
    class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.GameLoopUpdateTicked;
            helper.Events.GameLoop.DayStarted += this.GameLoopDayStarted;
        }

        private void GameLoopDayStarted(object sender, DayStartedEventArgs)
        {
            this.Monitor.Log("Day started");
        }

        private void GameLoopUpdateTicked(object sender, UpdateTickedEventArgs e)
        {        }
    }
}
