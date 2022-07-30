/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ribeena/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;

namespace BusSpriteAdjust
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        public static ModEntry context;//single instance
        public static Harmony harmony;
        public static TemporaryAnimatedSprite customBusDoor;
        public static Texture2D busWheelsTex;
        public static Vector2 busPosition;
        public static Rectangle busRect = new Rectangle(288, 1247, 128, 64);
        public static Point doorTileOffset;
        public static Vector2 doorOffset;
        public static int blankTile;
        public static int roadTile;
        public static Point busStopTile = new Point(12, 8);

        public static Point desertStopTile = new Point(18, 27);

        public static Rectangle doorOpenRect;
        public static Rectangle doorCloseRect;

        public static Rectangle pamRect;
        public static Rectangle pamOverlay;
        public static Point pamOriginalOffset;
        public static Point pamOffset;
        public static Rectangle pamEraseRect;

        public static int doorState;

        public const int DOOR_OPEN = 0;
        public const int DOOR_CLOSED = 1;
        public const int DOOR_OPENNING = 2;
        public const int DOOR_CLOSING = 3;
        public const int NO_STATE = -1;

        public static bool closingFinishedFunction = false;
        public static bool openingFinishedFunction = false;

        public static bool bs_driveoff = false;
        public static bool bs_return = false;
        public static bool bs_return_open = false;
        public static bool bs_tile_set = false;

        public static bool travelToDesert = false;
        public static bool desertArrived = false;
        public static bool travelToBusStop = false;

        public static bool BusLocationsModded = false;
        public static IBusStopEventsApi busLocationsApi;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += OnWarp;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            context = this;

            customBusDoor = null;
            doorState = NO_STATE;
            doorTileOffset = new Point(2, 0);
            doorOffset = new Vector2(16f+doorTileOffset.X*16f, 26f + doorTileOffset.Y * 16f);

            //Spot on the cursors map
            doorOpenRect = new Rectangle(288, 1311, 16, 38);
            doorCloseRect = new Rectangle(368, 1311, 16, 38);

            //Pam's graphics
            pamRect = new Rectangle(384, 1311, 16, 19);
            pamOverlay = new Rectangle(400, 1311, 16, 19);
            pamOriginalOffset = new Point(0, 29); //original offset
            pamOffset = new Point(10, 28);
            pamEraseRect = new Rectangle(busRect.X+ pamOriginalOffset.X, busRect.Y+ pamOriginalOffset.Y, pamRect.Width, pamRect.Height);

            //Animated wheels
            busWheelsTex = helper.Content.Load<Texture2D>("assets/bus_wheels.png");

            harmony = new Harmony(ModManifest.UniqueID);

            //Before drawing handle the bus door with a new custom temporary animated sprite
            //also fix pam
            harmony.Patch(
                original: AccessTools.Method(typeof(BusStop), nameof(BusStop.draw), new Type[] { typeof(SpriteBatch) }),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(bs_draw_pre))
            );

            //Replace the drive off function
            harmony.Patch(
                original: AccessTools.Method(typeof(BusStop), nameof(BusStop.busDriveOff)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(bs_busDriveOff_post))
            );

            //Fix walking path
            harmony.Patch(
                original: AccessTools.Method(typeof(BusStop), nameof(BusStop.answerDialogue)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(bs_answerDialogue_post))
            );

            //Fix bus drive back
            harmony.Patch(
                original: AccessTools.Method(typeof(BusStop), nameof(BusStop.busDriveBack)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(bs_busDriveBack_post))
            );

            //Desert arrival/leaving
            harmony.Patch(
                original: AccessTools.Method(typeof(Desert), nameof(Desert.UpdateWhenCurrentLocation)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(d_UpdateWhenCurrentLocation_post))
            );

            //Desert drawing/animation
            harmony.Patch(
                original: AccessTools.Method(typeof(Desert), nameof(Desert.draw), new Type[] { typeof(SpriteBatch) }),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(d_draw_pre))
            );

            //Fix leaving animation from desert
            harmony.Patch(
                original: AccessTools.Method(typeof(Desert), nameof(Desert.busDriveOff)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(d_busDriveOff_post))
            );


            

        }

        public static void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            //BusLocations comptaibility
            //https://github.com/ShinyFurretz/StardewMods/blob/master/BusLocations/ModEntry.cs
            context.Monitor.Log("Checking BusLocations", LogLevel.Debug);

            busLocationsApi = context.Helper.ModRegistry.GetApi<IBusStopEventsApi>("hootless.BusLocations");
            if (busLocationsApi != null)
            {
                context.Monitor.Log("BusLocations installed, hooking up events", LogLevel.Debug);

                BusLocationsModded = true;
                //Set the action for when answer is yes
                busLocationsApi.SetJumpToLocation(bs_answerDialogueBusLocations);

                //Use the new warping method
                harmony.Patch(
                    original: AccessTools.Method(typeof(BusStop), "busLeftToDesert"),
                    postfix: new HarmonyMethod(typeof(ModEntry), nameof(post_busLeftToDesert))
                );
            }
        }


        /*********
        ** Private methods
        *********/

        private static void create_customDoor()
        {
            customBusDoor = new TemporaryAnimatedSprite("LooseSprites\\Cursors", doorOpenRect, busPosition + doorOffset * 4f, flipped: false, 0f, Color.White)
            {
                interval = 70f,
                animationLength = 6,
                holdLastFrame = false,
                layerDepth = (busPosition.Y + 192f) / 10000f + 1E-05f,
                scale = 4f
            };
        }

        private static void animate_customDoor()
        {
            //Handle reverse animation a more consistent way below
            if(customBusDoor.pingPong)
            {
                customBusDoor.pingPong = false;
            }
            //Fix static states
            if(doorState == DOOR_CLOSED && customBusDoor.paused == false)
            {
                //jump to the frame
                customBusDoor.sourceRect.X = doorOpenRect.X + customBusDoor.sourceRect.Width * (customBusDoor.animationLength - 1);
                //reset animation
                customBusDoor.currentParentTileIndex = 0;
                customBusDoor.currentNumberOfLoops = 0;
                customBusDoor.paused = true;
            }

            if (doorState == DOOR_OPEN && customBusDoor.paused == false)
            {
                //jump to the frame
                customBusDoor.sourceRect.X = doorOpenRect.X;
                //reset animation
                customBusDoor.currentParentTileIndex = 0;
                customBusDoor.currentNumberOfLoops = 0;
                customBusDoor.paused = true;
            }

            //Transition door states
            if (doorState == DOOR_OPENNING)
            {
                if (customBusDoor.currentParentTileIndex >= customBusDoor.animationLength || customBusDoor.currentNumberOfLoops >= 1)
                {
                    //update the state
                    doorState = DOOR_OPEN;
                    //Jump to the frame
                    customBusDoor.sourceRect.X = doorOpenRect.X;
                    //reset animation
                    customBusDoor.currentParentTileIndex = 0;
                    customBusDoor.currentNumberOfLoops = 0;
                    customBusDoor.paused = true;
                    openingFinishedFunction = true;
                    //context.Monitor.Log("Openning Finished", LogLevel.Debug);
                }
                else
                {
                    openingFinishedFunction = false;
                    customBusDoor.sourceRect.X = doorOpenRect.X + customBusDoor.sourceRect.Width * ((customBusDoor.animationLength - 1) - Math.Min(customBusDoor.currentParentTileIndex, customBusDoor.animationLength - 1));
                }
            }

            if (doorState == DOOR_CLOSING)
            {
                //on the last frame
                if (customBusDoor.currentParentTileIndex >= customBusDoor.animationLength || customBusDoor.currentNumberOfLoops >= 1)
                {
                    //update the state
                    doorState = DOOR_CLOSED;
                    //jump to the frame
                    customBusDoor.sourceRect.X = doorOpenRect.X + customBusDoor.sourceRect.Width * (customBusDoor.animationLength - 1);
                    //reset animation
                    customBusDoor.currentParentTileIndex = 0;
                    customBusDoor.currentNumberOfLoops = 0;
                    customBusDoor.paused = true;
                    //context.Monitor.Log("Closing Finished", LogLevel.Debug);
                    //closed sound effect
                    Game1.playSound("trashcanlid");

                    closingFinishedFunction = true;
                }
                else
                {
                    closingFinishedFunction = false;
                    customBusDoor.sourceRect.X = doorOpenRect.X + customBusDoor.sourceRect.Width * Math.Min(customBusDoor.currentParentTileIndex, customBusDoor.animationLength - 1);
                }

            }
        }

        private static void bs_draw_pre(BusStop __instance, SpriteBatch spriteBatch)
        {
            IReflectedField<Vector2> busPositionRef = context.Helper.Reflection.GetField<Vector2>(__instance, "busPosition");
            busPosition = busPositionRef.GetValue();
            IReflectedField<Vector2> busMotion = context.Helper.Reflection.GetField<Vector2>(__instance, "busMotion");


            bool busDoorExists = false;
            IReflectedField<TemporaryAnimatedSprite> reflectedBusDoor = context.Helper.Reflection.GetField<TemporaryAnimatedSprite>(__instance, "busDoor");
            if (reflectedBusDoor != null)
            {
                //destroy the bus original bus door
                reflectedBusDoor.SetValue(null);

                busDoorExists = true;
            }
            //////////////////////////////
            /// logic for door state
            //The player has unlocked calico desert
            if (Game1.MasterPlayer.mailReceived.Contains("ccVault"))
            {
                //Pam has opened the door when you get near the bus
                NPC pam = Game1.getCharacterFromName("Pam");
                if (Game1.currentLocation.characters.Contains(pam) && pam.getTileLocation().Equals(new Vector2(11f, 10f)) && (Game1.player.getTileLocation().Y == 10f && (Game1.player.getTileLocation().X >= 10f || Game1.player.getTileLocation().X <= 17f)))
                {
                    if (doorState == DOOR_CLOSED)
                    {
                        //customBusDoor = null;//make a new one
                        //context.Monitor.Log("Open the door", LogLevel.Debug);
                        doorState = DOOR_OPENNING;
                        customBusDoor.paused = false;
                    }
                    else
                    {
                        if (doorState == NO_STATE)
                        {
                            //context.Monitor.Log("Door is open", LogLevel.Debug);
                            doorState = DOOR_OPEN;
                        }
                    }
                }
                else
                {
                    if (doorState == NO_STATE)
                    {
                        //context.Monitor.Log("Pam not here, keep the door closed", LogLevel.Debug);
                        doorState = DOOR_CLOSED;
                        //prevent walking into the bus
                        __instance.setMapTileIndex(busStopTile.X + doorTileOffset.X, busStopTile.Y + 1, roadTile, "Buildings");
                    }
                    else
                    {
                        if (doorState == DOOR_OPEN)
                        {
                            //context.Monitor.Log("Close the door", LogLevel.Debug);
                            doorState = DOOR_CLOSING;
                            customBusDoor.paused = false;
                        }
                    }
                }
            } else
            {
                if(doorState == NO_STATE && !bs_tile_set)
                {
                    //prevent walking into the bus
                    __instance.setMapTileIndex(busStopTile.X + doorTileOffset.X, busStopTile.Y + 1, roadTile, "Buildings");
                    bs_tile_set = true;
                }
                
                //bus still broken down, keep the door closed
                doorState = DOOR_CLOSED;
            }


            //Player is returning from the desert
            if (bs_return)
            {
                Game1.player.Position = busPosition + doorOffset;

                if (Math.Abs(busPosition.X - 704f) <= Math.Abs(busMotion.GetValue().X * 1.5f))
                {
                    busPositionRef.SetValue(new Vector2(704f, busPosition.Y));
                    busMotion.SetValue(Vector2.Zero);
                    context.Helper.Reflection.GetField<bool>(__instance, "drivingBack").SetValue(false);
                    bs_return = false;

                    //Open the tile
                    Game1.currentLocation.setMapTileIndex(busStopTile.X + doorTileOffset.X, busStopTile.Y + 1, blankTile, "Buildings");

                    Game1.globalFadeToBlack(delegate
                    {
                        Game1.player.Position = new Vector2(busStopTile.X+doorTileOffset.X, busStopTile.Y) * 64f;
                        __instance.lastTouchActionLocation = Game1.player.getTileLocation();

                        //context.Monitor.Log("Open the door", LogLevel.Debug);
                        doorState = DOOR_OPENNING;
                        customBusDoor.paused = false;
                        openingFinishedFunction = false;
                        bs_return_open = true;

                        //handle the horse message
                        //Copy paste from BusStop:341, with base --> __instance
                        if (Game1.player.horseName.Value != null && Game1.player.horseName.Value != "")
                        {
                            for (int i = 0; i < __instance.characters.Count; i++)
                            {
                                if (__instance.characters[i] is Horse && (__instance.characters[i] as Horse).getOwner() == Game1.player)
                                {
                                    if (__instance.characters[i].Name == null || __instance.characters[i].Name.Length == 0)
                                    {
                                        Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Locations:BusStop_ReturnToHorse2", __instance.characters[i].displayName));
                                    }
                                    else
                                    {
                                        Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Locations:BusStop_ReturnToHorse" + (Game1.random.Next(2) + 1), __instance.characters[i].displayName));
                                    }
                                    break;
                                }
                            }
                        }
                        Game1.globalFadeToClear();
                    });
                }
                else
                {
                    bs_return_open = false;
                }
            }

            if (busDoorExists && customBusDoor == null)
            {
                if (doorState == DOOR_OPEN || doorState == DOOR_CLOSED)
                {
                    create_customDoor();
                }
                if (doorState == DOOR_CLOSED) {
                    //jump to last frame and set up to play reverse
                    customBusDoor.sourceRect.X += customBusDoor.sourceRect.Width * (customBusDoor.animationLength - 1);
                }
                customBusDoor.paused = true;

            }

            //Process custom animation handling
            animate_customDoor();

            //Call the ending of animation
            if (closingFinishedFunction && bs_driveoff)
            {
                closingFinishedFunction = false;
                bs_driveoff = false;
                travelToDesert = true;
                //context.Monitor.Log("Calling busStartMovingOff()", LogLevel.Debug);
                context.Helper.Reflection.GetMethod(__instance, "busStartMovingOff").Invoke(customBusDoor.extraInfoForEndBehavior);
            }

            //Call the ending of animation replaces BusStop.doorOpenAfterReturn()
            if (openingFinishedFunction && bs_return_open)
            {
                __instance.setMapTileIndex(busStopTile.X + doorTileOffset.X, busStopTile.Y + 1, blankTile, "Buildings");

                Game1.displayFarmer = true;
                //Walk out
                Game1.player.controller = new PathFindController(Game1.player, Game1.currentLocation, busStopTile+doorTileOffset+new Point(0,2), 0, bs_pathOffBusDone);

                openingFinishedFunction = false;
                bs_return = false;
                bs_return_open = false;
                //context.Monitor.Log("Back in Stardew", LogLevel.Debug);
            }

            if (Game1.MasterPlayer.mailReceived.Contains("ccVault"))
            {
                //Insert the new door
                reflectedBusDoor.SetValue(customBusDoor);
            }
            else
            {
                //broken down bus
                reflectedBusDoor.SetValue(null);
            }


            ///////////////////////////////////
            //Driver rendering
            if (context.Helper.Reflection.GetField<bool>(Game1.currentLocation, "drivingBack").GetValue() && Desert.warpedToDesert)
            {
                //Cover old player using the cursor2 fron image
                spriteBatch.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)busPosition.X, (int)busPosition.Y - 40 + pamOriginalOffset.Y * 4)), new Rectangle(0, 0, 21, 41), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (busPosition.Y + 192f + 8f) / 10000f);
                
                //Set up the farmer to get an image of
                Game1.player.faceDirection(3);
                Game1.player.blinkTimer = -1000;
                //draw the player
                Game1.displayFarmer = true;
                Game1.player.FarmerRenderer.draw( spriteBatch, new FarmerSprite.AnimationFrame(117, 99999, 0, secondaryArm: false, flip: true), 117, new Rectangle(48, 608, 16, 32), Game1.GlobalToLocal(new Vector2((int)(busPosition.X + 4f +pamOffset.X*4), (int)(busPosition.Y - 8f)+pamOffset.Y*4)), Vector2.Zero, (busPosition.Y + 192f + 9f) / 10000f, Color.White, 0f, 1f, Game1.player);
                Game1.displayFarmer = false;
                //cover the head
                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)busPosition.X+pamOffset.X*4, (int)busPosition.Y)), new Rectangle(busRect.X+pamOffset.X, busRect.Y, pamRect.Width, pamOffset.Y), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (busPosition.Y + 192f + 11f) / 10000f);
                //cover legs
                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)busPosition.X + pamOffset.X * 4, (int)busPosition.Y + pamOffset.Y * 4+pamRect.Height*4)), new Rectangle(busRect.X+pamOffset.X, busRect.Y+pamOffset.Y +pamRect.Height, pamRect.Width, 15), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (busPosition.Y + 192f + 10f) / 10000f);
                //Draw bus overlay
                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)busPosition.X + pamOffset.X * 4, (int)busPosition.Y + pamOffset.Y * 4)), pamOverlay, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (busPosition.Y + 194f + 12f) / 10000f);

            }
            else
            
            if (context.Helper.Reflection.GetField<bool>(Game1.currentLocation, "drivingOff").GetValue() || context.Helper.Reflection.GetField<bool>(Game1.currentLocation, "drivingBack").GetValue())
            {
                //Draw over the Original pam
                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)busPosition.X + pamOriginalOffset.X * 4, (int)busPosition.Y + pamOriginalOffset.Y * 4)), pamEraseRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (busPosition.Y + 194f + 4f) / 10000f);
                
                //Draw new pam
                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)busPosition.X + pamOffset.X * 4, (int)busPosition.Y + pamOffset.Y * 4)), pamRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (busPosition.Y + 194f + 4f + 1f) / 10000f);
                //Draw bus overlay
                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)busPosition.X + pamOffset.X * 4, (int)busPosition.Y + pamOffset.Y * 4)), pamOverlay, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (busPosition.Y + 194f + 4f + 2f) / 10000f);

            }

            if (Game1.MasterPlayer.mailReceived.Contains("ccVault"))
            {
                //////////////////////////////
                /// Wheels rendering
                //animation based on location
                int wheelFrame = ((int)Math.Abs(busPosition.X) / 24) % 6;
                spriteBatch.Draw(busWheelsTex, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)busPosition.X, (int)busPosition.Y + 48f * 4)), new Rectangle(0, wheelFrame * 16, 128, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (busPosition.Y + 194f + 20f) / 10000f);
            }
        }


        private static void bs_busDriveOff_post(BusStop __instance)
        {
            //reset the customBusDoor so the position is right and kills the default finished function
            create_customDoor();

            //flag for custom finished code
            bs_driveoff = true;

            //context.Monitor.Log("Bus Drive off start", LogLevel.Debug);
            //start the animation
            doorState = DOOR_CLOSING;
        }

        private static void bs_answerDialogue_post()
        {
            
            //the player is told to move to the bus door
            if (Game1.player.controller != null)
            {
                //Open the tile
                Game1.currentLocation.setMapTileIndex(busStopTile.X + doorTileOffset.X, busStopTile.Y + 1, blankTile, "Buildings");

                //context.Monitor.Log("New walking path", LogLevel.Debug);
                //Camera follows the player onto the bus
                Game1.viewportFreeze = false;
                Game1.player.controller = new PathFindController(Game1.player, Game1.currentLocation, busStopTile + doorTileOffset, 0, bs_pathDone);
            }

        }

        public static void bs_answerDialogueBusLocations()
        {
            context.Monitor.Log("Start walking path", LogLevel.Debug);

            Game1.freezeControls = true;
            Game1.viewportFreeze = true;
            //Open the tile
            Game1.currentLocation.setMapTileIndex(busStopTile.X + doorTileOffset.X, busStopTile.Y + 1, blankTile, "Buildings");

            Game1.player.controller = new PathFindController(Game1.player, Game1.currentLocation, busStopTile + doorTileOffset, 0, bs_pathDone);
            Game1.player.setRunning(isRunning: true);
            if (Game1.player.mount != null)
            {
                Game1.player.mount.farmerPassesThrough = true;
            }
        }

        private static void bs_pathDone(Character c, GameLocation location) {
            //context.Monitor.Log("Player reached door", LogLevel.Debug);
            //now the bus drives off
            Game1.viewportFreeze = true;
            context.Helper.Reflection.GetMethod(location, "playerReachedBusDoor").Invoke(c, location);
        }

        private static void bs_busDriveBack_post(BusStop __instance)
        {
            //reset the customBusDoor so the position is right and kills the default finished function
            create_customDoor();
            doorState = DOOR_CLOSED;
        }

        private static void bs_pathOffBusDone(Character c, GameLocation location)
        {
            Game1.player.forceCanMove();
            Game1.player.faceDirection(2);
            //close off the access to bus
            Game1.currentLocation.setMapTileIndex(busStopTile.X + doorTileOffset.X, busStopTile.Y + 1, roadTile, "Buildings");
        }

        //BusLocations compatibility
        public static void post_busLeftToDesert()
        {
            if (busLocationsApi.GetDestinationMap() != "Desert") //Run the default code for desert
            {
                //context.Monitor.Log("Player to warp", LogLevel.Debug);
                Game1.viewportFreeze = true;
                Game1.warpFarmer(busLocationsApi.GetDestinationMap(), busLocationsApi.GetDestinationPoint().X, busLocationsApi.GetDestinationPoint().Y, busLocationsApi.GetDestinationFacing());
                Game1.globalFade = false;
            }
        }

        private static void d_UpdateWhenCurrentLocation_post(Desert __instance)
        {
            //Replaces Desert.doorOpenAfterReturn()
            //when the player arrives
            if (desertArrived && openingFinishedFunction)
            {
                //context.Monitor.Log("Farmer released to the desert", LogLevel.Debug);

                desertArrived = false;
                openingFinishedFunction = false;
                Game1.displayFarmer = true;
                Game1.player.forceCanMove();
                Game1.player.faceDirection(2);
                Game1.changeMusicTrack("wavy");
            }

            ////////////////////////////////////
            /// logic for door state
            IReflectedField<Vector2> busPositionRef = context.Helper.Reflection.GetField<Vector2>(__instance, "busPosition");
            busPosition = busPositionRef.GetValue();
            IReflectedField <Vector2> busMotion = context.Helper.Reflection.GetField<Vector2>(__instance, "busMotion");
            if (travelToDesert)
            {
                //Move player with door
                Game1.player.Position = busPosition + doorOffset;
                doorState = DOOR_CLOSED;

                //wait for bus to finish moving
                if (Math.Abs(busPosition.X - 1088f) <= Math.Abs(busMotion.GetValue().X * 1.5f))
                {
                    //context.Monitor.Log("Pull the lever Krunk", LogLevel.Debug);
                    create_customDoor();
                    travelToDesert = false;
                    desertArrived = true;
                    context.Helper.Reflection.GetField<bool>(__instance, "drivingBack").SetValue(false);
                    busPositionRef.SetValue(new Vector2(1088f, busPosition.Y));
                    busMotion.SetValue(Vector2.Zero);

                    Game1.displayFarmer = false;

                    Game1.globalFadeToBlack(delegate
                    {
                        //move the player
                        Game1.player.Position = new Vector2(desertStopTile.X+doorTileOffset.X, desertStopTile.Y) * 64f;//Move farmer over 2 squares
                        Game1.currentLocation.lastTouchActionLocation = Game1.player.getTileLocation();
                        Game1.displayFarmer = false;
                        doorState = DOOR_OPENNING;
                        customBusDoor.paused = false;

                        Game1.globalFadeToClear();
                    });
                }
            }
            ///////////////////////////////////
            
        }

        //Leaving the desert, chosen yes
        private static void d_busDriveOff_post(Desert __instance)
        {
            create_customDoor();
            doorState = DOOR_CLOSING;
            travelToBusStop = true;
        }

        private static void d_draw_pre(Desert __instance, SpriteBatch spriteBatch)
        {
            bool busDoorExists = false;
            IReflectedField<TemporaryAnimatedSprite> reflectedBusDoor = context.Helper.Reflection.GetField<TemporaryAnimatedSprite>(__instance, "busDoor");
            if (reflectedBusDoor != null)
            {
                //destroy the bus
                reflectedBusDoor.SetValue(null);
                busDoorExists = true;
            }

            //
            if (busDoorExists && customBusDoor == null)
            {
                create_customDoor();
            }


            /////////////////////////
            ///Door open logic
            if (doorState == NO_STATE)
            {
                doorState = DOOR_CLOSED;
            }

            if ((Game1.player.getTileLocation().Y == desertStopTile.Y || Game1.player.getTileLocation().Y == desertStopTile.Y+1) && (Game1.player.getTileLocation().X >= 17f && Game1.player.getTileLocation().X <= 24f))
            {
                if (doorState == DOOR_CLOSED)
                {
                    //context.Monitor.Log("Open the door", LogLevel.Debug);
                    doorState = DOOR_OPENNING;
                    customBusDoor.paused = false;
                }
            }
            else
            {
                if (doorState == DOOR_OPEN)
                {
                    //context.Monitor.Log("Close the door", LogLevel.Debug);
                    doorState = DOOR_CLOSING;
                    customBusDoor.paused = false;
                }
            }

            if (travelToBusStop && closingFinishedFunction)
            {
                travelToBusStop = false;
                closingFinishedFunction = false;
                bs_return = true;
                //context.Monitor.Log("Player door closed and leaving desert", LogLevel.Debug);
                context.Helper.Reflection.GetMethod(__instance, "busStartMovingOff").Invoke(0);
            }

            //Process custom animation handling
            animate_customDoor();

            if (Game1.MasterPlayer.mailReceived.Contains("ccVault"))
            {
                //Insert the new door
                reflectedBusDoor.SetValue(customBusDoor);
            }
            else
            {
                //broken down bus
                reflectedBusDoor.SetValue(null);
            }
            

            ///////////////////////////////////
            //Driver rendering
            if (context.Helper.Reflection.GetField<bool>(Game1.currentLocation, "drivingOff").GetValue() && Desert.warpedToDesert)
            {
                //Cover old player using the cursor2 fron image
                spriteBatch.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)busPosition.X, (int)busPosition.Y - 40 + pamOriginalOffset.Y * 4)), new Rectangle(0, 0, 21, 41), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (busPosition.Y + 192f + 8f) / 10000f);

                //Set up the farmer to get an image of
                Game1.player.faceDirection(3);
                Game1.player.blinkTimer = -1000;
                //draw the player
                Game1.displayFarmer = true;
                Game1.player.FarmerRenderer.draw(spriteBatch, new FarmerSprite.AnimationFrame(117, 99999, 0, secondaryArm: false, flip: true), 117, new Rectangle(48, 608, 16, 32), Game1.GlobalToLocal(new Vector2((int)(busPosition.X + 4f + pamOffset.X * 4), (int)(busPosition.Y - 8f) + pamOffset.Y * 4)), Vector2.Zero, (busPosition.Y + 192f + 9f) / 10000f, Color.White, 0f, 1f, Game1.player);
                Game1.displayFarmer = false;
                //cover the head
                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)busPosition.X + pamOffset.X * 4, (int)busPosition.Y)), new Rectangle(busRect.X + pamOffset.X, busRect.Y, pamRect.Width, pamOffset.Y), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (busPosition.Y + 192f + 11f) / 10000f);
                //cover legs
                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)busPosition.X + pamOffset.X * 4, (int)busPosition.Y + pamOffset.Y * 4 + pamRect.Height * 4)), new Rectangle(busRect.X + pamOffset.X, busRect.Y + pamOffset.Y + pamRect.Height, pamRect.Width, 15), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (busPosition.Y + 192f + 10f) / 10000f);
                //Draw bus overlay
                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)busPosition.X + pamOffset.X * 4, (int)busPosition.Y + pamOffset.Y * 4)), pamOverlay, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (busPosition.Y + 194f + 12f) / 10000f);

            }
            else

            if (context.Helper.Reflection.GetField<bool>(Game1.currentLocation, "drivingOff").GetValue() || context.Helper.Reflection.GetField<bool>(Game1.currentLocation, "drivingBack").GetValue())
            {
                //Draw over the Original pam
                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)busPosition.X + pamOriginalOffset.X * 4, (int)busPosition.Y + pamOriginalOffset.Y * 4)), pamEraseRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (busPosition.Y + 194f + 4f) / 10000f);

                //Draw new pam
                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)busPosition.X + pamOffset.X * 4, (int)busPosition.Y + pamOffset.Y * 4)), pamRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (busPosition.Y + 194f + 4f + 1f) / 10000f);
                //Draw bus overlay
                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)busPosition.X + pamOffset.X * 4, (int)busPosition.Y + pamOffset.Y * 4)), pamOverlay, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (busPosition.Y + 194f + 4f + 2f) / 10000f);

            }

            if (Game1.MasterPlayer.mailReceived.Contains("ccVault"))
            {
                //////////////////////////////
                /// Wheels rendering
                //animation based on location
                int wheelFrame = ((int)Math.Abs(busPosition.X) / 24) % 6;
                spriteBatch.Draw(busWheelsTex, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)busPosition.X, (int)busPosition.Y + 48f * 4)), new Rectangle(0, wheelFrame * 16, 128, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (busPosition.Y + 194f + 20f) / 10000f);
            }
        }


        private void OnWarp(object sender, WarpedEventArgs e)
        {
            if (Game1.currentLocation != null)
            {

                if (Game1.currentLocation.Name == "BusStop")
                {
                    //this.Monitor.Log("Welcome to the bus stop", LogLevel.Debug);
                    customBusDoor = null;
                    doorState = NO_STATE;

                    //Override the current map tiles
                    blankTile = -1;// Game1.currentLocation.getTileIndexAt(busStopTile + new Point(0,1),"Buildings");
                    roadTile = 1054;// Game1.currentLocation.getTileIndexAt(busStopTile + new Point(1, 1), "Buildings");
                    Game1.currentLocation.removeTileProperty(busStopTile.X, busStopTile.Y, "Back", "TouchAction");
                    Game1.currentLocation.setMapTileIndex(busStopTile.X, busStopTile.Y + 1, roadTile, "Buildings");
                    //Set the new tiles
                    Game1.currentLocation.setTileProperty(busStopTile.X + doorTileOffset.X, busStopTile.Y, "Back", "TouchAction", "Bus");
                    Game1.currentLocation.setMapTileIndex(busStopTile.X + doorTileOffset.X, busStopTile.Y + 1, blankTile, "Buildings");
                }
                if (Game1.currentLocation.Name == "Desert")
                {

                    //context.Monitor.Log("Welcome to the desert", LogLevel.Debug);
                    //reset the custom door
                    customBusDoor = null;
                    doorState = NO_STATE;
                    desertArrived = true;

                    //Override the current map tiles
                    blankTile = -1;//Game1.currentLocation.getTileIndexAt(desertStopTile, "Buildings");
                    roadTile = 206;//240 in .tmx, 206 according to Game1.currentLocation.getTileIndexAt(desertStopTile + new Point(3, 0), "Buildings");, but 240 in the tmx file

                    Game1.currentLocation.removeTileProperty(desertStopTile.X, desertStopTile.Y, "Back", "TouchAction");
                    //the following lin should work, but it always uses a weird graphic
                    //Game1.currentLocation.setMapTileIndex(desertStopTile.X, desertStopTile.Y, roadTile, "Buildings");

                    //Set the new tiles, the return touch and punch a hole
                    Game1.currentLocation.setTileProperty(desertStopTile.X + doorTileOffset.X, desertStopTile.Y, "Back", "TouchAction", "DesertBus");
                    Game1.currentLocation.setMapTileIndex(desertStopTile.X + doorTileOffset.X, desertStopTile.Y, blankTile, "Buildings");
                }
            }
        }
    }

    public interface IBusStopEventsApi
    {
        public void SetJumpToLocation(Action handler);
        public string GetDestinationMap();
        public Point GetDestinationPoint();
        public int GetDestinationFacing();
    }
}
