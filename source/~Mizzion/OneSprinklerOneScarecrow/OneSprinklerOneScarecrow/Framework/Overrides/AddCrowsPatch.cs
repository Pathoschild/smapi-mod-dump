/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using StardewModdingAPI;
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
                if (obj.Value.Name == "Haxarecrow")
                {
                    _monitor.Log("No crows ran");
                    return false;
                }
                    
            }

            return true;
        }
    }
}
