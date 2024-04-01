/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using StardewModdingAPI;
using HarmonyLib;
using System.Collections.Generic;
using StardewValley;
using System.Text;

namespace StardewHack.Library
{
    public record ModError(Mod mod, string error, bool fatal);

    public static class ModChecks {
        /// <summary>
        /// During startup mods that are broken are added to this list. Used to produce an error message during startup.
        /// </summary>
        static public List<ModError> errors = new List<ModError>();

        static private Dictionary<string, SemanticVersion> version_checks = new Dictionary<string, SemanticVersion>() {
            {"bcmpinc.AlwaysScrollMap"   , new SemanticVersion(7,1,0)},
            {"bcmpinc.FixAnimalTools"    , new SemanticVersion(7,1,0)},
            {"bcmpinc.FlexibleArms"      , new SemanticVersion(7,1,0)},
            {"bcmpinc.GrassGrowth"       , new SemanticVersion(7,1,0)},
            {"bcmpinc.HarvestWithScythe" , new SemanticVersion(7,1,0)},
            {"bcmpinc.MovementSpeed"     , new SemanticVersion(7,1,0)},
            {"bcmpinc.TilledSoilDecay"   , new SemanticVersion(7,1,0)},
            {"bcmpinc.TreeSpread"        , new SemanticVersion(7,1,0)},
            {"bcmpinc.WearMoreRings"     , new SemanticVersion(7,1,0)},
            {"spacechase0.BiggerBackpack", new SemanticVersion(7,1,0)},
        };

        public static void validateAssemblyVersion(this Mod mod)
        {
            var assembly = mod.GetType().Assembly.GetName();
            var assembly_version = assembly.Version;
            var manifest_version = mod.ModManifest.Version;
            if (assembly_version.Major == manifest_version.MajorVersion && assembly_version.Minor == manifest_version.MinorVersion) {
                return;
            }
            mod.Monitor.Log($"Version mismatch between assembly ({assembly_version}) and manifest ({manifest_version})", LogLevel.Error);
            mod.Monitor.Log($"Your installation of {assembly.Name} has corrupted. Please re-install the mod.", LogLevel.Error);
            errors.Add(new ModError(mod, I18n.Corrupted(), true));
        }
        public static void checkIncompatible(this Mod mod) {
            SemanticVersion version = version_checks.GetValueOrDefault(mod.ModManifest.UniqueID, null);
            var manifest = mod.ModManifest;
            if (version == null) {
                mod.Monitor.Log($"Mod '{manifest.Name}' v{manifest.Version} is not known to the currently used StardewHack.", LogLevel.Info);
                return;
            }
            if (manifest.Version.IsOlderThan(version)) {
                mod.Monitor.Log($"Mod '{manifest.Name}' v{manifest.Version} is outdated. This will likely cause problems. Please update '{manifest.Name}' to at least v{version}.", LogLevel.Error);
                errors.Add(new ModError(mod, I18n.OutdatedVersion(), true));
            }
        }

        public static void failedPatches(Mod mod) {
            errors.Add(new ModError(mod, I18n.FailedPatch(), false));
        }
        public static void InitializationError(Mod mod) {
            errors.Add(new ModError(mod, I18n.InitializationError(), true));
        }
    }

    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper) {
            I18n.Init(helper.Translation);

            // Check versions
            var harmony_version = typeof(Harmony).Assembly.GetName().Version;
            Monitor.Log($"Loaded StardewHack library v{ModManifest.Version} using Harmony v{harmony_version}.", LogLevel.Info);
            if (harmony_version < new System.Version(2,2,2,0)) {
                Monitor.Log($"Expected Harmony v2.2.2.0 or later. Mods that depend on StardewHack might not work correctly.", LogLevel.Error);
                ModChecks.errors.Add(new ModError(this, I18n.OutdatedHarmony(), true));
            }
            ModChecks.validateAssemblyVersion(this);
            
            // Register event to show warning in case some mod's patches failed to apply cleanly.
            Helper.Events.GameLoop.OneSecondUpdateTicked += GameLoop_OneSecondUpdateTicked;
        }
        
        private void GameLoop_OneSecondUpdateTicked(object sender, StardewModdingAPI.Events.OneSecondUpdateTickedEventArgs e)
        {
            // Only fire after 1 sec. in-game.
            if (e.Ticks < 60) return;
        
            // And only fire once.
            Helper.Events.GameLoop.OneSecondUpdateTicked -= GameLoop_OneSecondUpdateTicked;
            
            // Create a warning message if patches failed to apply cleanly.
            if (ModChecks.errors.Count==0) return;
            

            var msg = new StringBuilder();
            msg.AppendLine(I18n.Errors("v"+ModManifest.Version));

            bool fatal = false;
            foreach (var err in ModChecks.errors) {
                var mod = err.mod.ModManifest;
                msg.AppendLine($"^{mod.Name} v{mod.Version}:^  " + err.error);
                fatal |= err.fatal;
            }

            msg.Append("^");
            if (fatal) {
                msg.AppendLine(I18n.Fatal() + " ");
            }
            msg.AppendLine(I18n.Footer());

            // The message is a list containing a single string.
            var dialogue = new List<string>() { msg.ToString() };
            
            // Create the dialogue box. We can't pass a string directly as the signature differs between the PC and android version.
            var box = new StardewValley.Menus.DialogueBox(dialogue);
            Game1.activeClickableMenu = box;
            Game1.dialogueUp = true;
            box.finishTyping();
            
            if (fatal) {
                Game1.afterDialogues = () => { 
                    Monitor.Log($"Quiting Stardew Valley due to fatal errors.", LogLevel.Error);
                    Game1.quit = true;
                };
            }
        }
    }
}

