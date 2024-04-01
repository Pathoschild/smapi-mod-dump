/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/SkullCavernToggle
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using StardewValley;
using System.Collections.Generic;


namespace SkullCavernToggle
{
    public class ModEntry
        : Mod
    {
        private ModConfig config;

        public static Shrine Shrine { get; private set; } = new Shrine();

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.Toggle;
            helper.Events.Player.Warped += this.OnWarp;
            helper.Events.Multiplayer.ModMessageReceived += this.MessageReceived;
            helper.Events.GameLoop.DayStarted += this.DayStarted;

            this.config = helper.ReadConfig<ModConfig>();
        }



        // Apply shrine tiles when player is in the correct location and shrine should be shown
        private void OnWarp(object sender, WarpedEventArgs e)
        {
            if (e.NewLocation.NameOrUniqueName == "SkullCave" && ShowShrine() == true)
            {
                Shrine.ApplyTiles(this.Helper);
            }
        }

        // Fix conflicting shrines
        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            if (Game1.netWorldState.Value.SkullCavesDifficulty > 1)
            {
                Game1.netWorldState.Value.SkullCavesDifficulty = 1;
            }
            else if (Game1.netWorldState.Value.SkullCavesDifficulty < 0)
            {
                Game1.netWorldState.Value.SkullCavesDifficulty = 0;
            }
        }

        // Are the toggle conditions met?
        private bool ShouldToggle()
        {
            // Get completed orders
            var order = Game1.player.team.completedSpecialOrders;
         
            // Is quest active?
            if (Game1.player.team.SpecialOrderActive("QiChallenge10") == true)
            {
                // Yes, can't toggle right now
                return false;
            }

            // Must quest be complete?
            if (this.config.MustCompleteQuest == true)
            {
                // Yes, is it complete?

                // Iterate through completed quests
                foreach (string soid in new List<string>(order))
                {
                    // Is quest part of the collection?
                    if (soid.Contains("QiChallenge10") == true)
                    {
                        // Yes, quest is complete, difficulty can toggle
                        return true;
                    }
                }
            }

            else
            {
                // No, difficulty can toggle

                return true;
            }

            // If quest isn't complete and it must be complete, difficulty can't toggle
            return false;
        }

        // Should the shrine be added to the map?
        private bool ShowShrine()
        {
            // Is shrine toggle being used?
            if (this.config.ShrineToggle == true)
            {
                // Yes, check if the conditions are met

                // Get completed orders
                var order = Game1.player.team.completedSpecialOrders;

                // Must the quest be completed first?
                if (this.config.MustCompleteQuest == true)
                {
                    // Yes, is it complete?

                    // Iterate through completed orders
                    foreach (string soid in new List<string>(order))
                    {
                        
                        if (soid.Contains("QiChallenge10") == true)
                        {
                            // Yes, order complete, add shrine
                            return true;
                        }
                    }
                }

                else
                {
                    // No, quest is not a condition, add the shrine
                    return true;
                }
                
            }

            // No, conditions not met, shrine should not be placed
            return false;            
        }

        // Toggle difficulty after confirmation
        public void ShrineMenu(int difficulty)
        {
            // Toggle accordingly
            if (difficulty > 0)
            {
                // Normal
                Game1.netWorldState.Value.SkullCavesDifficulty = 0;
                Game1.addHUDMessage(new HUDMessage("Skull Cavern toggled to normal") { noIcon = true});

            }
            else
            {
                // Dangerous
                Game1.netWorldState.Value.SkullCavesDifficulty = 1;
                Game1.addHUDMessage(new HUDMessage("Skull Cavern toggled to dangerous") { noIcon = true });
            }

            // Fix shrine appearance for new difficulty
            Shrine.ApplyTiles(this.Helper);
            // Play sound cue
            Game1.playSound("serpentDie");


            // Log new difficulty, difficulty will update after the clock ticks in multiplayer (10 in-game minutes)
            this.Monitor.Log("Skull Cavern Difficulty: " + Game1.netWorldState.Value.SkullCavesDifficulty, LogLevel.Trace);
            Multiplayer message = new Multiplayer();
            this.Helper.Multiplayer.SendMessage(message, "Toggled", modIDs: new[] { this.ModManifest.UniqueID });
        }

        // Toggle difficulty using button/key
        private void Toggle(object sender, ButtonPressedEventArgs e)
        {
            // Has button/key been pressed, shrine is not used
            if (this.config.ToggleDifficulty.JustPressed() == true && this.config.ShrineToggle == false)
            {
                // Has correct button been pushed, conditions for toggle been met and world is ready?
                if (ShouldToggle() == true && Context.IsWorldReady == true)
                {
                    // Yes, toggle difficulty

                    if (Game1.netWorldState.Value.SkullCavesDifficulty > 0)
                    {
                        // Normal
                        Game1.netWorldState.Value.SkullCavesDifficulty = 0;
                        Game1.addHUDMessage(new HUDMessage("Skull Cavern toggled to normal") { noIcon = true });

                    }
                    else
                    {
                        // Dangerous
                        Game1.netWorldState.Value.SkullCavesDifficulty = 1;
                        Game1.addHUDMessage(new HUDMessage("Skull Cavern toggled to dangerous") { noIcon = true });
                    }

                    // Log new difficulty, difficulty will update after the clock ticks in multiplayer (10 in-game minutes)
                    this.Monitor.Log("Skull Cavern Difficulty: " + Game1.netWorldState.Value.SkullCavesDifficulty, LogLevel.Trace);

                    Multiplayer message = new Multiplayer();
                    this.Helper.Multiplayer.SendMessage(message, "Toggled", modIDs: new[] { this.ModManifest.UniqueID });
                }

                else if (ShouldToggle() == false && Context.IsWorldReady == true)
                {
                    // No, display message to say difficulty can't be toggled

                    if (Game1.player.team.SpecialOrderActive("QiChallenge10") == true)
                    {
                        Game1.addHUDMessage(new HUDMessage("Skull Cavern Invasion is active", 3));
                    }

                    else
                    {
                        Game1.addHUDMessage(new HUDMessage("Skull Cavern Invasion not completed", 3));
                    }
                }
            }

            // Using shrine
            else if (
                // Correct button is pressed
                e.Button.IsActionButton() == true
                // World is ready
                && Context.IsWorldReady == true
                // Correct location
                && Game1.currentLocation.NameOrUniqueName == "SkullCave"
                // Player can move
                && Game1.player.canMove == true
                // Shrine is showing
                && ShowShrine() == true)
            {
                GameLocation location = Game1.currentLocation;

                var TileX = e.Cursor.GrabTile.X;
                var TileY = e.Cursor.GrabTile.Y;

                // Use tiles relative to player location for controller uses (don't use the cursor position)
                if (e.Button == SButton.ControllerA)
                {
                    if (Game1.player.FacingDirection != 2 && Game1.player.Tile.X == 2 && Game1.player.Tile.Y == 5)
                    {
                        TileX = Game1.player.Tile.X;
                        TileY = Game1.player.Tile.Y - 2;
                    }

                    else if(Game1.player.FacingDirection == 3 && Game1.player.Tile.X == 3 && Game1.player.Tile.Y == 4)
                    {
                        TileX = Game1.player.Tile.X - 1;
                        TileY = Game1.player.Tile.Y - 1;
                    }
                }

                // Get tile properties
                string[] tileproperty = location.doesTileHavePropertyNoNull((int)TileX, (int)TileY, "Action", "Buildings").Split(' ');

                // If player clicks tile with SnakeShrine property, display the appropriate response
                if (tileproperty[0] == "SnakeShrine")
                {                   
                    if (ShouldToggle() == true)
                    {
                                             
                        if (Game1.netWorldState.Value.SkullCavesDifficulty > 0)
                        {
                            location.createQuestionDialogue("--Shrine Of Greater Challenge--^Summon an ancient magi-seal protection, returning the Skull Cavern to it's original state?", location.createYesNoResponses(), delegate (Farmer _, string answer)
                            {
                                if (answer == "Yes")
                                {
                                    ShrineMenu(1);
                                }
                            });
                        }
                        else
                        {
                            location.createQuestionDialogue("--Shrine Of Greater Challenge--^Dispel the ancient magi-seal of protection, allowing powerful monsters to enter the cavern?", location.createYesNoResponses(), delegate (Farmer _, string answer)
                            {
                                if (answer == "Yes")
                                {
                                    ShrineMenu(0);
                                }
                            });                            
                        }
                    }

                    // Skull Cavern Invasion is active, don't try and make the challenge easier
                    else if (ShouldToggle() == false && Game1.player.team.SpecialOrderActive("QiChallenge10") == true)
                    {
                        Game1.activeClickableMenu = new DialogueBox("Mr. Qi wants you to beat this fair and square. Ask again when Skull Cavern Invasion isn't active.");
                    }

                    // Shouldn't see this dialogue, kept in as a safeguard
                    else
                    {
                        Game1.activeClickableMenu = new DialogueBox("You haven't completed Skull Cavern Invasion... I don't think you can handle this yet.");
                    }
                    
                }
            }
            
        }        

        private void MessageReceived(object sender, ModMessageReceivedEventArgs e)
        {            
            if (e.FromModID == this.ModManifest.UniqueID && e.Type == "Toggled")
            {
                Multiplayer message = e.ReadAs<Multiplayer>();
                
                // Display message to say difficulty toggled, delay in changes means opposite conditions are used for display purposes
                if(Game1.netWorldState.Value.SkullCavesDifficulty > 0)
                {
                    Game1.addHUDMessage(new HUDMessage($"Skull Cavern difficulty toggled to normal by {message.Player}") { noIcon = true });
                }
                else
                {
                    Game1.addHUDMessage(new HUDMessage($"Skull Cavern difficulty toggled to dangerous by {message.Player}") { noIcon = true });
                }
                
                // Update shrine tiles using opposite conditions
                if(this.config.ShrineToggle == true)
                {
                    Shrine.ApplyTiles(this.Helper, true);
                }

            }
        }

    }
}
