using StardewModdingAPI;
using Harmony;

namespace StardewHack.Library
{
    public class ModEntry : StardewModdingAPI.Mod
    {
        public override void Entry(IModHelper helper) {
            // Check versions
            var harmony_version = typeof(HarmonyInstance).Assembly.GetName().Version;
            Monitor.Log($"Loaded StardewHack library v{ModManifest.Version} using Harmony v{harmony_version}.", LogLevel.Info);
            if (harmony_version < new System.Version(1,2,0,1)) {
                Monitor.Log($"Expected Harmony v1.2.0.1 or later. Mods that depend on StardewHack might not work correctly.", LogLevel.Warn);
            }
            
            // Check incompatible mods.
            CheckIncompatible(helper, "bcmpinc.AlwaysScrollMap",   new SemanticVersion(1,0,0));
            CheckIncompatible(helper, "bcmpinc.CraftCounter",      new SemanticVersion(1,0,0));
            CheckIncompatible(helper, "bcmpinc.FixAnimalTools",    new SemanticVersion(1,0,0));
            CheckIncompatible(helper, "bcmpinc.GrassGrowth",       new SemanticVersion(1,0,0));
            CheckIncompatible(helper, "bcmpinc.HarvestWithScythe", new SemanticVersion(1,0,0));
            CheckIncompatible(helper, "bcmpinc.MovementSpeed",     new SemanticVersion(1,0,0));
            CheckIncompatible(helper, "bcmpinc.TilledSoilDecay",   new SemanticVersion(1,0,0));
            CheckIncompatible(helper, "bcmpinc.TreeSpread",        new SemanticVersion(1,0,0));
            CheckIncompatible(helper, "bcmpinc.WearMoreRings",     new SemanticVersion(1,0,0));
        }
        
        public void CheckIncompatible(IModHelper helper, string uniqueID, SemanticVersion version) {
            var mod = helper.ModRegistry.Get(uniqueID);
            if (mod != null && mod.Manifest.Version.IsOlderThan(version)) {
                this.Monitor.Log($"Mod '{mod.Manifest.Name}' v{mod.Manifest.Version} is outdated. This will likely cause problems. Please update '{mod.Manifest.Name}' to at least v{version}.", LogLevel.Error);
            }            
        }
    }
}

