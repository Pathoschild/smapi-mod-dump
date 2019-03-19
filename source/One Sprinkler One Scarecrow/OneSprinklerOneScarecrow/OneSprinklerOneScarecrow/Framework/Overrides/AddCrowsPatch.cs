using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SObject = StardewValley.Object;

namespace OneSprinklerOneScarecrow.Framework.Overrides
{
    internal class AddCrowsPatch
    {
        private static IMonitor _monitor;
        public AddCrowsPatch(IMonitor monitor)
        {
            _monitor = monitor;
        }
        public static bool Prefix(ref Farm __instance)
        {
            foreach (var obj in __instance.Objects.Pairs)
            {
                if (obj.Value.Name == "Haxor Crow")
                {
                    _monitor.Log("No crows ran");
                    return false;
                }
                    
            }

            return true;
        }
    }
}
