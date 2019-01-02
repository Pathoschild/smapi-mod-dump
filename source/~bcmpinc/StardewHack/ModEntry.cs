using StardewModdingAPI;
using Harmony;

namespace StardewHack.Library
{
    public class ModEntry : StardewModdingAPI.Mod
    {
        public override void Entry(IModHelper helper) {
            var harmony_version = typeof(HarmonyInstance).Assembly.GetName().Version;
            Monitor.Log($"Loaded StardewHack library v{ModManifest.Version} using Harmony v{harmony_version}.", LogLevel.Info);
            if (harmony_version < new System.Version(1,2,0,1)) {
                Monitor.Log($"Expected Harmony v1.2.0.1 or later. Mods that depend on StardewHack might not work correctly.", LogLevel.Warn);
            }
        }
    }
}

