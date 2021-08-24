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

namespace StardewHack.Library
{
    public class ModEntry : Mod
    {
        /// <summary>
        /// During startup mods that are broken are added to this list. Used to produce an error message during startup.
        /// </summary>
        static public List<string> broken_mods = new List<string>();
    
        public override void Entry(IModHelper helper) {
            // Check versions
            var harmony_version = typeof(Harmony).Assembly.GetName().Version;
            Monitor.Log($"Loaded StardewHack library v{ModManifest.Version} using Harmony v{harmony_version}.", LogLevel.Info);
            if (harmony_version < new System.Version(1,2,0,1)) {
                Monitor.Log($"Expected Harmony v1.2.0.1 or later. Mods that depend on StardewHack might not work correctly.", LogLevel.Warn);
            }
            
            // Check incompatible mods.
            CheckIncompatible(helper, "bcmpinc.AlwaysScrollMap",    new SemanticVersion(5,0,0));
            CheckIncompatible(helper, "bcmpinc.FixAnimalTools",     new SemanticVersion(5,0,0));
            CheckIncompatible(helper, "bcmpinc.GrassGrowth",        new SemanticVersion(5,0,0));
            CheckIncompatible(helper, "bcmpinc.HarvestWithScythe",  new SemanticVersion(5,1,0));
            CheckIncompatible(helper, "bcmpinc.MovementSpeed",      new SemanticVersion(5,0,0));
            CheckIncompatible(helper, "bcmpinc.TilledSoilDecay",    new SemanticVersion(5,1,0));
            CheckIncompatible(helper, "bcmpinc.TreeSpread",         new SemanticVersion(5,0,0));
            CheckIncompatible(helper, "bcmpinc.WearMoreRings",      new SemanticVersion(5,1,0));
            CheckIncompatible(helper, "spacechase0.BiggerBackpack", new SemanticVersion(5,0,0));
            
            // Register event to show warning in case some mod's patches failed to apply cleanly.
            Helper.Events.GameLoop.OneSecondUpdateTicked += GameLoop_OneSecondUpdateTicked;
        }
        
        public void CheckIncompatible(IModHelper helper, string uniqueID, SemanticVersion version) {
            var mod = helper.ModRegistry.Get(uniqueID);
            if (mod != null && mod.Manifest.Version.IsOlderThan(version)) {
                Monitor.Log($"Mod '{mod.Manifest.Name}' v{mod.Manifest.Version} is outdated. This will likely cause problems. Please update '{mod.Manifest.Name}' to at least v{version}.", LogLevel.Error);
            }
        }

        private void GameLoop_OneSecondUpdateTicked(object sender, StardewModdingAPI.Events.OneSecondUpdateTickedEventArgs e)
        {
            // Only fire after 1 sec. in-game.
            if (e.Ticks < 60) return;
        
            // And only fire once.
            Helper.Events.GameLoop.OneSecondUpdateTicked -= GameLoop_OneSecondUpdateTicked;
            
            // Create a warning message if patches failed to apply cleanly.
            if (broken_mods.Count==0) return;
            
            var mod_list = new List<string>();
            foreach (var i in broken_mods) {
                var mod = Helper.ModRegistry.Get(i).Manifest;
                mod_list.Add($"{mod.Name} (v{mod.Version})");
            }

            // The message is a list containing a single string.
            var dialogue = new List<string>() {
                "StardewHack v" + ModManifest.Version +
                " failed to apply some bytecode patches. The following mods won't work correctly or at all: " +
                mod_list.Join() +
                ". Check your console or error log for further instructions."
            };
            
            // Create the dialogue box. We can't pass a string directly as the signature differs between the PC and android version.
            var box = new StardewValley.Menus.DialogueBox(dialogue);
            StardewValley.Game1.activeClickableMenu = box;
            StardewValley.Game1.dialogueUp = true;
            box.finishTyping();
        }
    }
}

