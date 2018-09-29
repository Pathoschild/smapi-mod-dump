using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSystem;
using StardewValley;
using EventSystem.Framework.FunctionEvents;
using EventSystem.Framework.Information;
using Microsoft.Xna.Framework;
using EventSystem.Framework.Events;
namespace SundropMapEvents
{
    public class Class1 :Mod
    {

        public static IModHelper ModHelper;
        public static IMonitor ModMonitor;
        public override void Entry(IModHelper helper)
        {
            ModHelper = this.Helper;
            ModMonitor = this.Monitor;
            StardewModdingAPI.Events.SaveEvents.AfterLoad += SaveEvents_AfterLoad;     
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            EventSystem.EventSystem.eventManager.addEvent(Game1.getLocationFromName("BusStop"), new WarpEvent("toRR", Game1.getLocationFromName("BusStop"), new Vector2(6, 11), new PlayerEvents(null, null), new WarpInformation("BusStop", 10, 12, 2, false)));
            EventSystem.EventSystem.eventManager.addEvent(Game1.getLocationFromName("BusStop"), new DialogueDisplayEvent("Hello.", Game1.getLocationFromName("BusStop"), new Vector2(10, 13),new MouseButtonEvents(null,null) , new MouseEntryLeaveEvent(null,null),"Hello there!"));
        }
    }
}
