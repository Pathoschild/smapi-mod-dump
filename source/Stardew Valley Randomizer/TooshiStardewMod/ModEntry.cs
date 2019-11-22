using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.IO;

namespace Randomizer {
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod {

        public static Dictionary<string, bool> configDict;

        public PossibleSwap[] PossibleSwaps = {
            new PossibleSwap("Pierre", "Lewis"), new PossibleSwap("Wizard", "Sandy"), new PossibleSwap("Willy", "Pam"), new PossibleSwap("Abigail", "Marnie"), new PossibleSwap("MrQi", "Gunther"), new PossibleSwap("Marlon", "Governor"), new PossibleSwap("Caroline", "Evelyn"), new PossibleSwap("Pam", "Haley"), new PossibleSwap("Morris", "Krobus"), new PossibleSwap("Gus", "Elliott"), new PossibleSwap("Linus", "Pam"), new PossibleSwap("Kent", "Pierre"),
            new PossibleSwap("Sandy", "Maru"), new PossibleSwap("Sebastian", "Wizard"), new PossibleSwap("Jas", "Vincent"), new PossibleSwap("Krobus", "Dwarf"), new PossibleSwap("Leah", "Marnie"), new PossibleSwap("Henchman", "Bouncer"), 
            new PossibleSwap("Harvey", "Gus"), new PossibleSwap("Bouncer", "Gunther"), new PossibleSwap("Gunther", "Governor"), new PossibleSwap("Evelyn", "Jodi"), new PossibleSwap("George", "Wizard"), new PossibleSwap("Emily", "Marnie"), new PossibleSwap("Sam", "Linus"), new PossibleSwap("Alex", "Gus"), new PossibleSwap("Penny", "Sandy"), new PossibleSwap("Morris", "Governor"),

            new PossibleSwap("Haley", "Alex"), new PossibleSwap("Harvey", "Maru"), new PossibleSwap("Abigail", "Sebastian"), new PossibleSwap("Penny", "Sam"), new PossibleSwap("Leah", "Elliott"), new PossibleSwap("Shane", "Emily"),
            new PossibleSwap("Shane", "Pam")
        };

        private AssetLoader _modAssetLoader;
        private AssetEditor _modAssetEditor;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        ///
        public override void Entry(IModHelper helper) {

            //config file read in
            string[] config = System.IO.File.ReadAllLines("Mods/Randomizer/RandomizerSettings.txt");
            configDict = new Dictionary<string, bool>();

            foreach (string line in config)
            {
                string[] tokens = line.Split('=');
                if (tokens.Length != 2) continue;
                configDict.Add(tokens[0].Trim().ToLower(), (tokens[1].Trim().ToLower() == "true"));
            }


            this._modAssetLoader = new AssetLoader(this);
            this._modAssetEditor = new AssetEditor(this);

            //SaveEvents.AfterCreate += this.OnCreateOrLoad;
            //SaveEvents.AfterLoad += this.OnCreateOrLoad;

            helper.Content.AssetLoaders.Add(this._modAssetLoader);
            helper.Content.AssetEditors.Add(this._modAssetEditor);



            //Changes made before game loads
            this.PreLoadReplacments();

            // Calculate on creation replacements
            SaveEvents.AfterCreate += (sender, args) => this.CalculateOneTimeReplacements();

            // Calculate all replacements when the save is loaded
            SaveEvents.AfterLoad += (sender, args) => this.CalculateAllReplacements();

            //PlayerEvents.Warped += (sender, args) => this.PlayerChangedLocations();

            GameEvents.UpdateTick += (sender, args) => this.CheckSong();
 
        }

        //private void OnCreateOrLoad(object sender, EventArgs e)
        //{}
        public void PreLoadReplacments()
        {
            Random placeHolderNum = new Random();
            this._modAssetLoader.CalculateReplacementsBeforeLoad(placeHolderNum);
            this._modAssetEditor.CalculateEditsBeforeLoad(placeHolderNum);
        }

        public void CalculateOneTimeReplacements()
        {
            byte[] seedvar2 = (new SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(Game1.player.farmName));
            int seed2 = BitConverter.ToInt32(seedvar2, 0);
            Random rng2 = new Random(seed2);

            this._modAssetLoader.CalculateReplacementsOnCreation(rng2);
        }

        public void CalculateAllReplacements() {
            //Seed is pulled from unique game ID
            //int seed = ((int) ((uint) ((int) Game1.uniqueIDForThisGame / 2)));

            //Seed is pulled from farm name
            byte[] seedvar = (new SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(Game1.player.farmName));
            int seed = BitConverter.ToInt32(seedvar, 0);

            this.Monitor.Log($"Seed Set: {seed}");

            Random rng = new Random(seed);

            // Make replacements and edits
            this._modAssetLoader.CalculateReplacements(rng);
            this._modAssetEditor.CalculateEdits(rng);


            // Invalidate all replaced and edited assets so they are reloaded
            this._modAssetLoader.InvalidateCache();
            this._modAssetEditor.InvalidateCache();

        }

        public void CheckSong()
        {

            //Game1.addHUDMessage(new HUDMessage(Game1.currentSong?.Name));



            if (this._modAssetLoader.musicSwap.TryGetValue(Game1.currentSong?.Name?.ToLower()??"", out string value))
            {
                Game1.changeMusicTrack(value);
            }
  

        }



        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the player presses a controller, keyboard, or mouse button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void InputEvents_ButtonPressed(object sender, EventArgsInput e) {
            if (Context.IsWorldReady) // save is loaded
            {
                // this.Monitor.Log($"Save ID: {Game1.uniqueIDForThisGame}");
                // this.Monitor.Log($"Seed: {seed}");
            }
        }

    }
}
