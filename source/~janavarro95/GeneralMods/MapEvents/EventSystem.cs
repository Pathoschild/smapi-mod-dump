using EventSystem.Framework;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSystem
{ 
    /*
     *TODO: Make Bed/Sleep Event. 
     * 
     *
     */
    public class EventSystem: Mod
    {
        public static IModHelper ModHelper;
        public static IMonitor ModMonitor;

        public static EventManager eventManager;
        public override void Entry(IModHelper helper)
        {
            
            ModHelper = this.Helper;
            ModMonitor = this.Monitor;
            StardewModdingAPI.Events.GameEvents.UpdateTick += GameEvents_UpdateTick;
            StardewModdingAPI.Events.SaveEvents.AfterLoad += SaveEvents_AfterLoad;
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            eventManager = new EventManager();
        }

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (eventManager == null) return;
            eventManager.update();
        }
    }
}
