using StardewModdingAPI;
using Harmony;

namespace StardewHack.Library
{
    public class ModEntry : StardewModdingAPI.Mod
    {
        /// <summary>
        /// During startup mods that are broken are added to this list. Used to produce an error message during startup.
        /// </summary>
        static public System.Collections.Generic.List<string> broken_mods = new System.Collections.Generic.List<string>();
    
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
            CheckIncompatible(helper, "bcmpinc.HarvestWithScythe", new SemanticVersion(1,1,0));
            CheckIncompatible(helper, "bcmpinc.MovementSpeed",     new SemanticVersion(1,0,0));
            CheckIncompatible(helper, "bcmpinc.TilledSoilDecay",   new SemanticVersion(1,0,0));
            CheckIncompatible(helper, "bcmpinc.TreeSpread",        new SemanticVersion(1,0,0));
            CheckIncompatible(helper, "bcmpinc.WearMoreRings",     new SemanticVersion(1,4,0));
            
            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        }
        
        public void CheckIncompatible(IModHelper helper, string uniqueID, SemanticVersion version) {
            var mod = helper.ModRegistry.Get(uniqueID);
            if (mod != null && mod.Manifest.Version.IsOlderThan(version)) {
                this.Monitor.Log($"Mod '{mod.Manifest.Name}' v{mod.Manifest.Version} is outdated. This will likely cause problems. Please update '{mod.Manifest.Name}' to at least v{version}.", LogLevel.Error);
            }
        }

        void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e) {
            // Create a warning message if patches failed to apply cleanly.
            if (broken_mods.Count==0) return;
            
            System.Collections.Generic.List<string> mod_list = new System.Collections.Generic.List<string>();
            foreach (var i in broken_mods) {
                var mod = Helper.ModRegistry.Get(i).Manifest;
                mod_list.Add($"{mod.Name} (v{mod.Version})");
            }

            StardewValley.Game1.drawDialogueNoTyping(
                "StardewHack failed to apply some bytecode patches. The following mods won't work correctly or at all: " +
                mod_list.Join() +
                ". Check your console for further instructions."
            );
        }
    }
}

