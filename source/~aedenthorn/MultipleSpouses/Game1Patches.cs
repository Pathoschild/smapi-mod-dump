/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

namespace MultipleSpouses
{
    public static class Game1Patches
    {
        private static IMonitor Monitor;
        public static string lastGotCharacter = null;

        // call this method from your Entry class
        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static void prepareSpouseForWedding_Prefix(Farmer farmer)
        {
            if(farmer.spouse == null)
            {
                Monitor.Log($"Spouse for {farmer.name} is null");
                foreach (string name in farmer.friendshipData.Keys)
                {
                    if (farmer.friendshipData[name].IsEngaged())
                    {
                        Monitor.Log($"Setting spouse for {farmer.name} to fianc(e)e {name}");
                        farmer.spouse = name;
                        break;
                    }
                }
            }
        }
        public static void getCharacterFromName_Prefix(string name)
        {
            if (EventPatches.startingLoadActors)
                lastGotCharacter = name;
        }
    }
}
