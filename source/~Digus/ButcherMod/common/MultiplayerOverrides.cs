using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace AnimalHusbandryMod.common
{
    public class MultiplayerOverrides
    {
        public static bool broadcastEvent(Event evt)
        {
            EventsLoader.BroadcastEvent(evt.id);
            return true;
        }
    }
}
