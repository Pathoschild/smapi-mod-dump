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
    public static class NetWorldStatePatches
    {
        private static IMonitor Monitor;

        public static ModConfig Config;

        public static string lastGotCharacter = null;

        // call this method from your Entry class
        public static void Initialize(IMonitor monitor, IModHelper helper, ModConfig config)
        {
            Monitor = monitor;
            Config = config;
        }

        public static bool hasWorldStateID_Prefix(string id, ref bool __result)
        {
            if(Config.EnableMod && Config.BuildAllSpousesRooms && id == "sebastianFrog")
            {
                Monitor.Log($"Preventing frogs");
                __result = false;
                return false;
            }
            return true;
        }
        public static void getCharacterFromName_Prefix(string name)
        {
            if (EventPatches.startingLoadActors)
                lastGotCharacter = name;
        }
    }
}
