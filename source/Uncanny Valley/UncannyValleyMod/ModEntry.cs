/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jasisco5/UncannyValleyMod
**
*************************************************/

using System;
using System.Threading;
using Netcode;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using StardewValley.TerrainFeatures;
using ContentPatcher;
using SpaceShared.APIs;
using Microsoft.Xna.Framework.Audio;
using System.Security.Cryptography;

namespace UncannyValleyMod
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        // Variables
        IContentPatcherAPI cpApi;
        SpaceCore.Api scApi;
        IModHelper helper;
        ModSaveData saveModel;
        Dictionary<string, Token> tokens = new Dictionary<string, Token>();

        // sound variables
        Vector2 previousPosition = new Vector2();
        Random random = new Random();
        bool thunderHasPlayed = false;
        bool office = false;
        bool soundTest = false;
        bool batFlight = false;

        // Other File References
        ModWeapon modWeapon;
        ModQuests modQuests;
        ModMaps modMaps;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.helper = helper;
            // Set Up Events
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;

            helper.Events.Player.Warped += this.OnWarped;

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;

            helper.Events.GameLoop.Saving += this.OnSaving;

            helper.Events.GameLoop.UpdateTicking += this.SoundSystem;

            // Get C# modded content
            modWeapon = new ModWeapon(helper);
            modMaps = new ModMaps(helper, this.Monitor);
            modQuests = new ModQuests(helper, this.Monitor, modWeapon);
        }





        /*********
        ** Private methods
        *********/
        /// <summary>
        /// Connections between the Content Patcher and SMAPI
        /// </summary>
        public void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Reference to Space Core
            scApi = new SpaceCore.Api();
            scApi.RegisterSerializerType(typeof(ReapingEnchantment));
            // Reference to Content Patcher
            cpApi = this.Helper.ModRegistry.GetApi<ContentPatcher.IContentPatcherAPI>("Pathoschild.ContentPatcher");
            // Working with Content Patcher
            AddTokens();
            {

                ///
                /// Loading owned Content Packs
                ///
                foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
                {
                    //this.Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");

                    // Loading JSON Data
                    {
                        //YourDataModel data = contentPack.ReadJsonFile<YourDataFile>("content.json");
                    }
                    // Read Content Data
                    {
                        //Texture2D image = contentPack.LoadAsset<Texture2D>("image.png");

                        // Passing an asset name to game code
                        //tilesheet.ImageSource = contentPack.GetActualAssetKey("image.png");
                    }

                }
            }


        }

        

        private void OnSaving(object sender, SavingEventArgs e)
        {
            this.helper.Data.WriteSaveData("savedata", saveModel);
        }
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Content Patcher Conditionals
            {
                ///
                /// Checking Conditions
                ///
                // Create a model of the conditions you want to check
                var rawConditions = new Dictionary<string, string>
                {
                    ["PlayerGender"] = "male",             // player is male
                    ["Relationship: Abigail"] = "Married", // player is married to Abigail
                    ["HavingChild"] = "{{spouse}}",        // Abigail is having a child
                    ["Season"] = "Winter"                  // current season is winter
                };

                // Call the API to parse the conditions into an IManagedConditions wrapper
                // This is an expensive operation, so stash this wrapper if you want to reuse it
                var conditions = cpApi.ParseConditions(
                   manifest: this.ModManifest,
                   rawConditions: rawConditions,
                   formatVersion: new SemanticVersion("1.30.0")
                );

                // Conditions don't update automatically
                conditions.UpdateContext();
            }

            // Custom Save Data
            saveModel = this.Helper.Data.ReadSaveData<ModSaveData>("savedata");
            if(saveModel == null)
            {
                // create default entry
                this.Helper.Data.WriteSaveData<ModSaveData>("savedata", new ModSaveData());
                saveModel = this.Helper.Data.ReadSaveData<ModSaveData>("savedata");
            }
            modMaps.SetSaveModel(saveModel);
            modWeapon.saveModel = saveModel;
            modQuests.saveModel = saveModel;

            foreach( KeyValuePair<string, Token> entry in tokens ) 
            {
                entry.Value.saveModel = saveModel;
                entry.Value.UpdateContext();
            }

        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // print button presses to the console window
            //this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            //this.Monitor.Log($"{e.Player.Name} warped from {e.OldLocation} to {e.NewLocation}", LogLevel.Debug);
        }


        // Helper Methods
        // Content Patcher Tokens
        private void AddTokens()
        {

            ///
            /// Adding a token to Content Patcher api
            /// 
            // To use it in a Content Pack, list this mod as a dependency 
            cpApi.RegisterToken(this.ModManifest, "PlayerName", () =>
            {
                // save is loaded
                if (Context.IsWorldReady)
                    return new[] { Game1.player.Name };
            
                // or save is currently loading
                if (SaveGame.loaded?.player != null)
                    return new[] { SaveGame.loaded.player.Name };
            
                // no save loaded (e.g. on the title screen)
                return null;
            });
            if (!tokens.ContainsKey("Weapon")) { tokens.Add("Weapon", new WeaponToken());  }
            modWeapon.token = (WeaponToken)tokens["Weapon"];
            cpApi.RegisterToken(this.ModManifest, "WeaponObtained", tokens["Weapon"]);


            modMaps.tokens = tokens;

        }

        // helper method for sounds
        // checks the in game location, then checks the player coordinates before playing the sounds at a specified location
        private void SoundSystem(object sender, EventArgs e)
        {
            SoundPlayer();
        }

        private void SoundPlayer()
        {
            // check game location
            // ---- MANSION EXT ----
            if (Game1.currentLocation == Game1.getLocationFromName("Custom_Mansion_Exterior"))
            {
                // check coordinates
                // Playes a thunder sound the first time the player is in front of the mansion
                if (Game1.player.position.X > (20*64) && Game1.player.position.X < (30*64) && Game1.player.position.Y > (34 * 64) && Game1.player.position.Y < (44 * 64) && thunderHasPlayed == false)
                {
                    // play sound
                    Game1.currentLocation.localSound("thunder");
                    thunderHasPlayed = true;
                }
            }
            else if (Game1.currentLocation == Game1.getLocationFromName("Custom_Mansion_Interior"))
            {
                // plays a ticking a sound when the player is near the grandfather clock
                if (Game1.player.position.X > (60 * 64) && Game1.player.position.X < (64 * 64) && Game1.player.position.Y > (24 * 64) && Game1.player.position.Y < (27 * 64) &&
                    (previousPosition.X < (60 * 64) || previousPosition.X > (64 * 64) || previousPosition.Y < (24 * 64) || previousPosition.Y > (27 * 64)))
                {
                    DelayedAction.playSoundAfterDelay("drumkit2", 1000, pitch: -1000);
                    DelayedAction.playSoundAfterDelay("drumkit2", 2000, pitch: -1000);
                    DelayedAction.playSoundAfterDelay("drumkit2", 3000, pitch: -1000);
                    DelayedAction.playSoundAfterDelay("drumkit2", 4000, pitch: -1000);
                    DelayedAction.playSoundAfterDelay("drumkit2", 5000, pitch: -1000);
                }

                // plays a creaking sound when the player walks into the left hallway
                if (Game1.player.position.X > (29 * 64) && Game1.player.position.X < (31 * 64) && Game1.player.position.Y > (32 * 64) && Game1.player.position.Y < (36 * 64) &&
                    (previousPosition.X < (29 * 64) || previousPosition.X > (31 * 64) || previousPosition.Y < (32 * 64) || previousPosition.Y > (36 * 64)))
                {
                    // play sound
                    Game1.currentLocation.localSound("doorCreakReverse");
                }

                // plays a creaking sound when the player walks left at the top of the stairs
                if (Game1.player.position.X > (35 * 64) && Game1.player.position.X < (37 * 64) && Game1.player.position.Y > (16 * 64) && Game1.player.position.Y < (21 * 64) &&
                    (previousPosition.X < (35 * 64) || previousPosition.X > (37 * 64) || previousPosition.Y < (16 * 64) || previousPosition.Y > (21 * 64)))
                {
                    // play sound
                    Game1.currentLocation.localSound("doorCreakReverse");
                }

                // plays a small thunder sound when the player first walks into the main office
                if (Game1.player.position.X > (90 * 64) && Game1.player.position.X < (92 * 64) && Game1.player.position.Y > (14 * 64) && Game1.player.position.Y < (17 * 64) && !office && 
                    (previousPosition.X < (90 * 64) || previousPosition.X > (92 * 64) || previousPosition.Y < (14 * 64) || previousPosition.Y > (17 * 64)))
                {
                    // play sound
                    Game1.currentLocation.localSound("thunder_small");
                    office = true;
                }

                // plays a small thunder sound when the player first walks into the main office
                if (Game1.player.position.X > (6 * 64) && Game1.player.position.X < (9 * 64) && Game1.player.position.Y > (13 * 64) && Game1.player.position.Y < (15 * 64) && !batFlight &&
                    (previousPosition.X < (6 * 64) || previousPosition.X > (9 * 64) || previousPosition.Y < (13 * 64) || previousPosition.Y > (15 * 64)))
                {
                    // play sound
                    DelayedAction.playSoundAfterDelay("breakingGlass", 1000);
                    DelayedAction.playSoundAfterDelay("batFlap", 1500);
                    DelayedAction.playSoundAfterDelay("batFlap", 1700);
                    DelayedAction.playSoundAfterDelay("batFlap", 1900);
                    DelayedAction.playSoundAfterDelay("batScreech", 2200);
                    batFlight = true;
                }
            }
            else if (Game1.currentLocation == Game1.getLocationFromName("Custom_Mansion_Basement"))
            {
                // plays water drops when the player first gets to the main part of the basement
                if ( Game1.player.position.Y > (40 * 64) && previousPosition.Y < (40 * 64))
                {
                    // play sound
                    DelayedAction.playSoundAfterDelay("dwop", 400, pitch: 100);
                    DelayedAction.playSoundAfterDelay("dwop", 800, pitch: 100);
                    DelayedAction.playSoundAfterDelay("dwop", 1200, pitch: 100);
                }

                // plays a steam sound when the player enters the boiler room
                if (Game1.player.position.Y > (53 * 64) && previousPosition.Y < (53 * 64))
                {
                    // play sound
                    DelayedAction.playSoundAfterDelay("steam", 200);
                    DelayedAction.playSoundAfterDelay("steam", 1000);
                }

                // plays a furnace sound when the player walks near the furnace in the boiler room
                if (Game1.player.position.X > (13 * 64) && Game1.player.position.X < (18 * 64) && Game1.player.position.Y > (54 * 64) && Game1.player.position.Y < (57 * 64) &&
                    (previousPosition.X < (13 * 64) || previousPosition.X > (18 * 64) || previousPosition.Y < (54 * 64) || previousPosition.Y > (57 * 64)))
                {
                    // play sound
                    Game1.currentLocation.localSound("furnace");
                }
            }
            previousPosition = Game1.player.Position;
        }
    }
}
