/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;
using xTile.Dimensions;

namespace StardewArchipelago.Locations.CodeInjections
{
    public static class IsolatedEventInjections
    {
        public const string OLD_MASTER_CANNOLI_AP_LOCATION = "Old Master Cannoli";
        public const string BEACH_BRIDGE_AP_LOCATION = "Beach Bridge Repair";
        public const string GALAXY_SWORD_SHRINE_AP_LOCATION = "Galaxy Sword Shrine";
        public const string RUSTY_SWORD_AP_LOCATION = "The Mines Entrance Cutscene";

        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _helper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        public static bool CheckAction_OldMasterCanolli_Prefix(Woods __instance, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who, ref bool __result)
        {
            try
            {
                var tile = __instance.map.GetLayer("Buildings")
                    .PickTile(new Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);
                if (tile == null || !who.IsLocalPlayer || (tile.TileIndex != 1140 && tile.TileIndex != 1141) || !__instance.hasUnlockedStatue.Value)
                {
                    return true; // run original logic
                }

                if (__instance.hasUnlockedStatue.Value && !__instance.localPlayerHasFoundStardrop() && who.freeSpotsInInventory() > 0)
                {
                    _locationChecker.AddCheckedLocation(OLD_MASTER_CANNOLI_AP_LOCATION);
                    if (!Game1.player.mailReceived.Contains("CF_Statue"))
                        Game1.player.mailReceived.Add("CF_Statue");
                }
                __result = true;

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckAction_OldMasterCanolli_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool AnswerDialogueAction_BeachBridge_Prefix(Beach __instance, string questionAndAnswer, string[] questionParams, ref bool __result)
        {
            try
            {
                if (questionAndAnswer != "BeachBridge_Yes")
                {
                    return true; // run original logic
                }

                Game1.player.removeItemsFromInventory(388, 300);
                _locationChecker.AddCheckedLocation(BEACH_BRIDGE_AP_LOCATION);
                __result = true;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AnswerDialogueAction_BeachBridge_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool CheckAction_BeachBridge_Prefix(Beach __instance, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who, ref bool __result)
        {
            try
            {
                if (tileLocation.X != 58 || tileLocation.Y != 13)
                {
                    return true; // run original logic
                }

                __result = true;
                if (_locationChecker.IsLocationChecked(BEACH_BRIDGE_AP_LOCATION))
                {
                    return false; // don't run original logic
                }

                if (who.hasItemInInventory(388, 300))
                {
                    __instance.createQuestionDialogue(
                        Game1.content.LoadString("Strings\\Locations:Beach_FixBridge_Question"),
                        __instance.createYesNoResponses(), "BeachBridge");
                    return false; // don't run original logic
                }

                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Beach_FixBridge_Hint"));
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckAction_BeachBridge_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool PerformTouchAction_GalaxySwordShrine_Prefix(GameLocation __instance, string fullActionString, Vector2 playerStandingPosition)
        {
            try
            {
                var actionFirstWord = fullActionString.Split(' ')[0];
                if (Game1.eventUp || actionFirstWord != "legendarySword" || _locationChecker.IsLocationChecked(GALAXY_SWORD_SHRINE_AP_LOCATION))
                {
                    return true; // run original logic
                }

                if (Game1.player.ActiveObject != null && Utility.IsNormalObjectAtParentSheetIndex(Game1.player.ActiveObject, 74))
                {
                    Game1.player.Halt();
                    Game1.player.faceDirection(2);
                    Game1.player.showCarrying();
                    Game1.player.jitterStrength = 1f;

                    Game1.pauseThenDoFunction(7000, CheckGalaxySwordApLocation);

                    Game1.changeMusicTrack("none", music_context: Game1.MusicContext.Event);
                    __instance.playSound("crit");
                    Game1.screenGlowOnce(new Color(30, 0, 150), true, 0.01f, 0.999f);
                    DelayedAction.playSoundAfterDelay("stardrop", 1500);
                    Game1.screenOverlayTempSprites.AddRange(Utility.sparkleWithinArea(new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), 500, Color.White, 10, 2000));
                    Game1.afterDialogues += () => Game1.stopMusicTrack(Game1.MusicContext.Event);
                    return false; // don't run original logic
                }

                __instance.localSound("SpringBirds");
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PerformTouchAction_GalaxySwordShrine_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void CheckGalaxySwordApLocation()
        {

            Game1.flashAlpha = 1f;
            Game1.player.holdUpItemThenMessage(new MeleeWeapon(4));
            Game1.player.reduceActiveItemByOne();
            _locationChecker.AddCheckedLocation(GALAXY_SWORD_SHRINE_AP_LOCATION);
            
            // Game1.player.mailReceived.Contains("galaxySword")
            // GameLocation.getGalaxySword

            Game1.player.jitterStrength = 0.0f;
            Game1.screenGlowHold = false;
        }

        public static bool SkipEvent_RustySword_Prefix(Event __instance)
        {
            try
            {
                if (__instance.id != 100162)
                {
                    return true; // run original logic
                }


                if (__instance.playerControlSequence)
                {
                    __instance.EndPlayerControlSequence();
                }

                Game1.playSound("drumkit6");

                var actorPositionsAfterMoveField = _helper.Reflection.GetField<Dictionary<string, Vector3>>(__instance, "actorPositionsAfterMove");
                actorPositionsAfterMoveField.GetValue().Clear();

                foreach (var actor in __instance.actors)
                {
                    var ignoreStopAnimation = actor.Sprite.ignoreStopAnimation;
                    actor.Sprite.ignoreStopAnimation = true;
                    actor.Halt();
                    actor.Sprite.ignoreStopAnimation = ignoreStopAnimation;
                    __instance.resetDialogueIfNecessary(actor);
                }

                __instance.farmer.Halt();
                __instance.farmer.ignoreCollisions = false;
                Game1.exitActiveMenu();
                Game1.dialogueUp = false;
                Game1.dialogueTyping = false;
                Game1.pauseTime = 0.0f;

                _locationChecker.AddCheckedLocation(RUSTY_SWORD_AP_LOCATION);
                
                Game1.player.Position = new Vector2(-9999f, -99999f);
                __instance.endBehaviors(new string[1] { "end" }, Game1.currentLocation);
                return false; // don't run original logic

            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SkipEvent_RustySword_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool AwardFestivalPrize_RustySword_Prefix(Event __instance, GameLocation location, GameTime time, string[] split)
        {
            try
            {
                var festivalWinnersField = _helper.Reflection.GetField<HashSet<long>>(__instance, "festivalWinners");
                if (__instance.id != 100162 ||
                    festivalWinnersField.GetValue().Contains(Game1.player.UniqueMultiplayerID) || split.Length <= 1 ||
                    split[1].ToLower() != "sword") 
                {
                    return true; // run original logic
                }

                if (Game1.activeClickableMenu == null)
                    __instance.CurrentCommand++;
                __instance.CurrentCommand++;

                _locationChecker.AddCheckedLocation(RUSTY_SWORD_AP_LOCATION);
                return false; // don't run original logic

            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AwardFestivalPrize_RustySword_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
