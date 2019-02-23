using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using System.Collections.Generic;

namespace bwdylib
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;
#if DEBUG
        public static bool Debug = true;
#else
        public static bool Debug = false;
#endif
        public override void Entry(IModHelper helper)
        {
            instance = this;
            Monitor.Log("bwdylib reporting in. " + (Debug ? "Debug" : "Release") + " build active.");

        }
    }
}