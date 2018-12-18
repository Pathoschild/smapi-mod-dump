using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
namespace CurrentLocationPopUp
{
    public class Class1 :Mod
    {
        public override void Entry(IModHelper helper)
        {
            StardewModdingAPI.Events.LocationEvents.CurrentLocationChanged += LocationEvents_CurrentLocationChanged;
        }

        public void LocationEvents_CurrentLocationChanged(object sender, StardewModdingAPI.Events.EventArgsCurrentLocationChanged e)
        {
            if (Game1.player == null) return;
            if (Game1.player.currentLocation != null) Game1.showGlobalMessage(Game1.player.currentLocation.name);
        }
    }
}
