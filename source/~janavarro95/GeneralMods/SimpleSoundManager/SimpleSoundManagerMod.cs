using StardewModdingAPI;

namespace SimpleSoundManager
{
    public class SimpleSoundManagerMod : Mod
    {
        internal static IModHelper ModHelper;

        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
        }
    }
}
