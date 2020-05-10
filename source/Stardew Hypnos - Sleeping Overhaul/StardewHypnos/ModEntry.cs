using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using xTile.Layers;
using xTile.Tiles;

namespace StardewHypnos
{
    /// <summary>
    /// The mod entry point.
    /// </summary>
    internal class ModEntry : Mod
    {
        /// <summary>
        /// The mod configuration.
        /// </summary>
        private ModConfig config;

        /// <summary>
        /// Indicates whether the "Sleep" GUI should open.
        /// </summary>
        private bool shouldAsk = false;

        // Public methods

        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Load configuration
            config = helper.ReadConfig<ModConfig>();

            // Right-Click Hook: Enter Bachalor(ette) or friends' homes at any time
            if (config.KeepFriendDoorsOpen)
            {
                helper.Events.Input.ButtonPressed += OnButtonPressed;
            }

            // Warp Hook: Trigger OnUpdateTicked (for sleeping) when necessary
            helper.Events.Player.Warped += OnPlayerWarped;
        }

        // Utility methods

        /// <summary>
        /// Gets the TileSheet name associated with the specified ImageSource.
        /// </summary>
        /// <param name="imageSource">The tile's ImageSource.</param>
        /// <returns>A string with the  TileSheet's name.</returns>
        private static string TileSheetNameFromImageSource(string imageSource)
        {
            return imageSource.Replace("Maps\\", string.Empty).Replace("Maps/", string.Empty);
        }

        /// <summary>
        /// Verifies whether the specified tile location isn't a blacklisted bed tile.
        /// </summary>
        /// <param name="mapName">The Map name.</param>
        /// <param name="location">The tile location.</param>
        /// <returns>A bool indicating if the location is allowed.</returns>
        private bool IsAllowedBedLocation(string mapName, Vector2 location)
        {
            if (config.BlacklistedBedTiles.TryGetValue(mapName, out List<Vector2> blacklist))
            {
                foreach (Vector2 blacklistedPos in blacklist)
                {
                    if (location == blacklistedPos)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Verifies whether the specified tile is a bed.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>A bool indicating if the tile is a bed.</returns>
        private bool IsBedTile(Tile tile)
        {
            if (config.BedTileIndexes.TryGetValue(TileSheetNameFromImageSource(tile.TileSheet.ImageSource), out List<int> bedIndexes))
            {
                // There's a tile index configuration for our current Map
                foreach (int bedIndex in bedIndexes)
                {
                    if (tile.TileIndex == bedIndex)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Verifies whether the Farmer is friends with any NPC inside the specified NPCs collection.
        /// </summary>
        /// <param name="npcs">The NPCs collection.</param>
        /// <returns>A bool indicating if the Farmer is friends with any collection NPC.</returns>
        private bool IsFarmerInFriendship(List<string> npcs)
        {
            if (npcs == null || npcs.Count == 0)
            {
                return false;
            }

            // Is the Farmer friends enough with at least one of the passed NPCs?
            foreach (string npc in npcs)
            {
                Game1.player.friendshipData.TryGetValue(npc, out Friendship friendship);

                // 1 Heart = 250 Points
                if (friendship.Points >= 250 * config.MinimumFriendshipHearts)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Verifies whether the Farmer is in a romantic relationship (dating, engaged, married)
        /// with any NPC inside the specified NPCs collection.
        /// </summary>
        /// <param name="npcs">The NPCs collection.</param>
        /// <returns>A bool indicating if the Farmer is romantically engaged with any collection NPC.</returns>
        private bool IsFarmerInRomanticRelationship(List<string> npcs)
        {
            if (npcs == null || npcs.Count == 0)
            {
                return false;
            }

            // Is the Farmer dating, engaged or married with at least one of the passed NPCs?
            foreach (string npc in npcs)
            {
                Game1.player.friendshipData.TryGetValue(npc, out Friendship friendship);

                if (friendship.IsDating() || friendship.IsEngaged() || friendship.IsMarried())
                {
                    return true;
                }
            }

            return false;
        }

        // Event methods

        /// <summary>
        /// The method invoked after the game state is updated.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // Is the Tile under the player a bed?
            Layer frontLayer = Game1.currentLocation.Map?.GetLayer("Front");
            if (frontLayer == null)
            {
                return;
            }

            Vector2 tileLocation = Game1.player.getTileLocation();
            Tile tile = frontLayer.Tiles[Convert.ToInt32(tileLocation.X), Convert.ToInt32(tileLocation.Y)];

            // Failsafe: should never happen..?
            if (tile == null)
            {
                return;
            }

            if (IsBedTile(tile) && IsAllowedBedLocation(Game1.player.currentLocation.Name, tileLocation))
            {
                // Sleep!
                if (!Game1.newDay && Game1.shouldTimePass() && shouldAsk)
                {
                    // Don't ask again before leaving the Bed tile
                    shouldAsk = false;
                    Game1.currentLocation.createQuestionDialogue(
                        Game1.content.LoadString("Strings\\Locations:FarmHouse_Bed_GoToSleep"),
                        Game1.currentLocation.createYesNoResponses(),
                        (Farmer _, string answer) =>
                        {
                            if (answer == "Yes")
                            {
                                if (Game1.IsMultiplayer)
                                {
                                    // Wait for other players to get ready in MP
                                    Game1.player.team.SetLocalReady("sleep", true);
                                    Game1.dialogueUp = false;
                                    Game1.activeClickableMenu = new ReadyCheckDialog(
                                        "sleep",
                                        true,
                                        (Farmer who) =>
                                        {
                                            // The following startSleep call will fail if isInBed == false,
                                            // so we'll change it to true just before sleeping and the
                                            // game won't have a chance of changing it back.
                                            Game1.player.isInBed.Value = true;
                                            Helper.Reflection.GetMethod(Game1.currentLocation, "doSleep").Invoke();
                                        },
                                        (Farmer who) =>
                                        {
                                            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ReadyCheckDialog)
                                            {
                                                (Game1.activeClickableMenu as ReadyCheckDialog).closeDialog(who);
                                            }

                                            who.timeWentToBed.Value = 0;
                                        });
                                }
                                else
                                {
                                    Game1.player.isInBed.Value = true;
                                    Helper.Reflection.GetMethod(Game1.currentLocation, "startSleep").Invoke();
                                }
                            }
                        });
                }
            }
            else
            {
                // Left the Sleeping dialog tile
                shouldAsk = true;
            }
        }

        /// <summary>
        /// The method invoked after the current player moves to a new location.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnPlayerWarped(object sender, WarpedEventArgs e)
        {
            // Warped to a location "owned" by a Bachelor(ette) or a friend?
            config.NPCsByWarp.TryGetValue(e.NewLocation.Name, out List<string> npcsInWarp);

            switch (config.MinimumRelationship)
            {
                case MinimumRelationshipType.OnlyPartners:
                    // No reason to handle this if the Farmer isn't in a relationship with any of the NPCs
                    if (!IsFarmerInRomanticRelationship(npcsInWarp))
                    {
                        Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
                        return;
                    }

                    break;
                case MinimumRelationshipType.Friends:
                    // No reason to handle this if the Farmer isn't in a friendship with any of the NPCs
                    if (!IsFarmerInFriendship(npcsInWarp))
                    {
                        Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
                        return;
                    }

                    break;
                case MinimumRelationshipType.Everyone:
                    // No operation: everyone's beds should be "sleepable"
                    break;
                default:
                    break;
            }

            Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        /// <summary>
        /// The method invoked after the player pressed a keyboard, mouse, or controller button.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // Only accept ingame events with an "unlocked" Farmer
            if (!Context.IsWorldReady || !Context.IsPlayerFree)
            {
                return;
            }

            // Only accept right-click button events
            if (e.Button != SButton.MouseRight)
            {
                return;
            }

            // Get selected Tile and maybe its Action property
            Vector2 tilePosition = e.Cursor.GrabTile;
            string tileProp = Game1.currentLocation.doesTileHaveProperty(
                Convert.ToInt32(tilePosition.X), Convert.ToInt32(tilePosition.Y), "Action", "Buildings");

            if (tileProp == null || tileProp.Length == 0)
            {
                return;
            }

            // Skips "Action", takes only arguments
            string[] propArgs = tileProp.Split(' ');

            // Return if tile doesn't have an (outdoors?) door warp
            if (propArgs[0] != "LockedDoorWarp")
            {
                return;
            }

            string warpLocation = propArgs[3];
            config.NPCsByWarp.TryGetValue(warpLocation, out List<string> npcsInHouse);

            switch (config.MinimumRelationship)
            {
                case MinimumRelationshipType.OnlyPartners:
                    if (!IsFarmerInRomanticRelationship(npcsInHouse))
                    {
                        return;
                    }

                    break;
                case MinimumRelationshipType.Friends:
                case MinimumRelationshipType.Everyone:
                    // 1. Only open doors at all times for people in friendships with you
                    // 2. We don't want to open doors for every ingame location!
                    if (!IsFarmerInFriendship(npcsInHouse))
                    {
                        return;
                    }

                    break;
                default:
                    return;
            }

            // Passed check: open friend or partner door
            Rumble.rumble(0.15f, 200f);
            Game1.player.completelyStopAnimatingOrDoingAction();
            Game1.currentLocation.playSoundAt("doorClose", Game1.player.getTileLocation());
            Game1.warpFarmer(
                warpLocation,
                Convert.ToInt32(propArgs[1], CultureInfo.InvariantCulture),
                Convert.ToInt32(propArgs[2], CultureInfo.InvariantCulture),
                false);

            // Exit with event suppression (avoid game handling this event)
            Helper.Input.Suppress(SButton.MouseRight);
        }
    }
}
