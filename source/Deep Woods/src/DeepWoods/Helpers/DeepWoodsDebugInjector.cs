/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/

//using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;

namespace DeepWoodsMod.Helpers
{
    /*
    public class DeepWoodsDebugInjector : Game1
    {
        public static void Patch(string id)
        {
            var harmony = new Harmony(id);

            harmony.Patch(
               original: AccessTools.Method(typeof(Game1), "onFadeToBlackComplete"),
               prefix: new HarmonyMethod(typeof(DeepWoodsDebugInjector), nameof(DeepWoodsDebugInjector.onFadeToBlackCompletePatch))
            );
        }

        public static bool onFadeToBlackCompletePatch(ref bool __result)
        {
            bool result = false;
            if (killScreen)
            {
                viewportFreeze = true;
                viewport.X = -10000;
            }

            if (exitToTitle)
            {
                menuUp = false;
                setGameMode(4);
                menuChoice = 0;
                fadeIn = false;
                fadeToBlack = true;
                fadeToBlackAlpha = 0.01f;
                exitToTitle = false;
                changeMusicTrack("none");
                debrisWeather.Clear();
                __result = true;
                return false;
            }

            if (timeOfDayAfterFade != -1)
            {
                timeOfDay = timeOfDayAfterFade;
                timeOfDayAfterFade = -1;
            }

            if (!nonWarpFade && locationRequest != null && !menuUp)
            {
                GameLocation gameLocation = currentLocation;
                if (emoteMenu != null)
                {
                    emoteMenu.exitThisMenuNoSound();
                }

                if (client != null && currentLocation != null)
                {
                    currentLocation.StoreCachedMultiplayerMap(multiplayer.cachedMultiplayerMaps);
                }

                currentLocation.cleanupBeforePlayerExit();
                multiplayer.broadcastLocationDelta(currentLocation);
                bool flag = false;
                displayFarmer = true;
                if (eventOver)
                {
                    eventFinished();
                    if (dayOfMonth == 0)
                    {
                        newDayAfterFade(delegate
                        {
                            player.Position = new Vector2(320f, 320f);
                        });
                    }

                    __result = true;
                    return false;
                }

                if (locationRequest.IsRequestFor(currentLocation) && player.previousLocationName != "" && !eventUp && !currentLocation.Name.StartsWith("UndergroundMine"))
                {
                    player.Position = new Vector2(xLocationAfterWarp * 64, yLocationAfterWarp * 64 - (player.Sprite.getHeight() - 32) + 16);
                    viewportFreeze = false;
                    currentLocation.resetForPlayerEntry();
                    flag = true;
                }
                else
                {
                    if (locationRequest.Name.StartsWith("UndergroundMine"))
                    {
                        if (!currentLocation.Name.StartsWith("UndergroundMine"))
                        {
                            changeMusicTrack("none");
                        }

                        MineShaft mineShaft = locationRequest.Location as MineShaft;
                        if (player.IsSitting())
                        {
                            player.StopSitting(animate: false);
                        }

                        player.Halt();
                        player.forceCanMove();
                        if (!IsClient || (locationRequest.Location != null && locationRequest.Location.Root != null))
                        {
                            mineShaft.resetForPlayerEntry();
                            flag = true;
                        }

                        currentLocation = mineShaft;
                        currentLocation.Map.LoadTileSheets(mapDisplayDevice);
                        checkForRunButton(GetKeyboardState());
                    }

                    if (!eventUp && !menuUp)
                    {
                        player.Position = new Vector2(xLocationAfterWarp * 64, yLocationAfterWarp * 64 - (player.Sprite.getHeight() - 32) + 16);
                    }

                    if (!locationRequest.Name.StartsWith("UndergroundMine"))
                    {
                        currentLocation = locationRequest.Location;
                        if (!IsClient)
                        {
                            locationRequest.Loaded(locationRequest.Location);
                            currentLocation.resetForPlayerEntry();
                            flag = true;
                        }

                        currentLocation.Map.LoadTileSheets(mapDisplayDevice);
                        if (!viewportFreeze && currentLocation.Map.DisplayWidth <= viewport.Width)
                        {
                            viewport.X = (currentLocation.Map.DisplayWidth - viewport.Width) / 2;
                        }

                        if (!viewportFreeze && currentLocation.Map.DisplayHeight <= viewport.Height)
                        {
                            viewport.Y = (currentLocation.Map.DisplayHeight - viewport.Height) / 2;
                        }

                        checkForRunButton(GetKeyboardState(), ignoreKeyPressQualifier: true);
                    }

                    if (!eventUp)
                    {
                        viewportFreeze = false;
                    }
                }

                forceSnapOnNextViewportUpdate = true;
                player.FarmerSprite.PauseForSingleAnimation = false;
                player.faceDirection(facingDirectionAfterWarp);
                _isWarping = false;
                if (player.ActiveObject != null)
                {
                    player.showCarrying();
                }
                else
                {
                    player.showNotCarrying();
                }

                if (IsClient)
                {
                    if (locationRequest.Location != null && locationRequest.Location.Root != null && multiplayer.isActiveLocation(locationRequest.Location))
                    {
                        currentLocation = locationRequest.Location;
                        locationRequest.Loaded(locationRequest.Location);
                        if (!flag)
                        {
                            currentLocation.resetForPlayerEntry();
                        }

                        player.currentLocation = currentLocation;
                        locationRequest.Warped(currentLocation);
                        currentLocation.updateSeasonalTileSheets();
                        if (IsDebrisWeatherHere())
                        {
                            populateDebrisWeatherArray();
                        }

                        warpingForForcedRemoteEvent = false;
                        locationRequest = null;
                    }
                    else
                    {
                        requestLocationInfoFromServer();
                        if (currentLocation == null)
                        {
                            __result = true;
                            return false;
                        }
                    }
                }
                else
                {
                    player.currentLocation = locationRequest.Location;
                    locationRequest.Warped(locationRequest.Location);
                    locationRequest = null;
                }

                if (locationRequest == null && currentLocation.Name == "Farm" && !eventUp)
                {
                    if (player.position.X / 64f >= (float)(currentLocation.map.Layers[0].LayerWidth - 1))
                    {
                        player.position.X -= 64f;
                    }
                    else if (player.position.Y / 64f >= (float)(currentLocation.map.Layers[0].LayerHeight - 1))
                    {
                        player.position.Y -= 32f;
                    }

                    if (player.position.Y / 64f >= (float)(currentLocation.map.Layers[0].LayerHeight - 2))
                    {
                        player.position.X -= 48f;
                    }
                }

                if (gameLocation != null && gameLocation.Name.StartsWith("UndergroundMine") && currentLocation != null && !currentLocation.Name.StartsWith("UndergroundMine"))
                {
                    MineShaft.OnLeftMines();
                }

                result = true;
            }

            if (newDay)
            {
                newDayAfterFade(delegate
                {
                    if (eventOver)
                    {
                        eventFinished();
                        if (dayOfMonth == 0)
                        {
                            newDayAfterFade(delegate
                            {
                                player.Position = new Vector2(320f, 320f);
                            });
                        }
                    }

                    nonWarpFade = false;
                    fadeIn = false;
                });
                __result = true;
                return false;
            }

            if (eventOver)
            {
                eventFinished();
                if (dayOfMonth == 0)
                {
                    newDayAfterFade(delegate
                    {
                        currentLocation.resetForPlayerEntry();
                        nonWarpFade = false;
                        fadeIn = false;
                    });
                }

                __result = true;
                return false;
            }

            if (boardingBus)
            {
                boardingBus = false;
                drawObjectDialogue(content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2694") + (currentLocation.Name.Equals("Desert") ? content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2696") : content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2697")));
                messagePause = true;
                viewportFreeze = false;
            }

            if (IsRainingHere() && currentSong != null && currentSong != null && currentSong.Name.Equals("rain"))
            {
                if (currentLocation.IsOutdoors)
                {
                    currentSong.SetVariable("Frequency", 100f);
                }
                else if (!currentLocation.Name.StartsWith("UndergroundMine"))
                {
                    currentSong.SetVariable("Frequency", 15f);
                }
            }

            __result = result;
            return false;
        }

    }
    */
}
