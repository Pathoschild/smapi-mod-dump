/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mjSurber/FarmHouseRedone
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Locations;

namespace FarmHouseRedone.OtherLocations
{
    public static class DecoratableStates
    {
        public static Dictionary<DecoratableLocation, DecoratableState> states;

        public static void init()
        {
            states = new Dictionary<DecoratableLocation, DecoratableState>();
        }

        public static void clearAll()
        {
            foreach (DecoratableLocation location in states.Keys)
            {
                states[location].clear();
            }
        }

        public static DecoratableState getState(DecoratableLocation location)
        {
            if (!states.ContainsKey(location))
            {
                Logger.Log("No state found for " + location.name + "!  (" + location.uniqueName + ")");
                states[location] = new DecoratableState(location);
            }
            return states[location];
        }
    }
}
