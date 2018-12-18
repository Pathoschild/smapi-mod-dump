using StardewModdingAPI;

namespace SimpleSoundManager
{
    public class SimpleSoundManagerMod : Mod
    {
        internal static IModHelper ModHelper;
        internal static IMonitor ModMonitor;

        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = Monitor;
        }
    }
}
