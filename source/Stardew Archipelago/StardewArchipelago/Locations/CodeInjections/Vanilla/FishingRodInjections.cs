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
using StardewArchipelago.Stardew.Ids.Vanilla;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class FishingRodInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        // public void skipEvent()
        public static bool SkipEvent_BambooPole_Prefix(Event __instance)
        {
            try
            {
                if (__instance.id != EventIds.BAMBOO_POLE)
                {
                    return true; // run original logic
                }

                SkipBambooPoleEventArchipelago(__instance);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SkipEvent_BambooPole_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public void skipEvent()
        public static bool SkipEvent_WillyFishingLesson_Prefix(Event __instance)
        {
            try
            {
                if (__instance.id != EventIds.WILLY_FISHING_LESSON)
                {
                    return true; // run original logic
                }

                SkipFishingLessonEventArchipelago(__instance);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SkipEvent_WillyFishingLesson_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public static void AwardFestivalPrize(Event @event, string[] args, EventContext context)
        public static bool AwardFestivalPrize_BambooPole_Prefix(Event @event, string[] args, EventContext context)
        {
            try
            {
                if (@event.id != EventIds.BAMBOO_POLE || args.Length <= 1 || args[1].ToLower() != "rod")
                {
                    return true; // run original logic
                }

                CheckBambooPoleLocation();

                if (Game1.activeClickableMenu == null)
                    @event.CurrentCommand++;
                @event.CurrentCommand++;

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AwardFestivalPrize_BambooPole_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public static void AwardFestivalPrize(Event @event, string[] args, EventContext context)
        public static bool AwardFestivalPrize_WillyFishingLesson_Prefix(Event @event, string[] args, EventContext context)
        {
            try
            {
                if (@event.id != EventIds.WILLY_FISHING_LESSON)
                {
                    return true; // run original logic
                }

                CheckFishingLessonLocations();

                if (Game1.activeClickableMenu == null)
                    @event.CurrentCommand++;
                @event.CurrentCommand++;

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AwardFestivalPrize_WillyFishingLesson_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public static void GainSkill(Event @event, string[] args, EventContext context)
        public static bool GainSkill_WillyFishingLesson_Prefix(Event @event, string[] args, EventContext context)
        {
            try
            {
                if (@event.id != EventIds.WILLY_FISHING_LESSON)
                {
                    return true; // run original logic
                }

                CheckFishingLessonLocations();
                
                @event.CurrentCommand++;

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GainSkill_WillyFishingLesson_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void SkipBambooPoleEventArchipelago(Event bambooPoleEvent)
        {
            if (bambooPoleEvent.playerControlSequence)
            {
                bambooPoleEvent.EndPlayerControlSequence();
            }

            Game1.playSound("drumkit6");

            var actorPositionsAfterMoveField = _modHelper.Reflection.GetField<Dictionary<string, Vector3>>(bambooPoleEvent, "actorPositionsAfterMove");
            actorPositionsAfterMoveField.GetValue().Clear();

            foreach (var actor in bambooPoleEvent.actors)
            {
                var ignoreStopAnimation = actor.Sprite.ignoreStopAnimation;
                actor.Sprite.ignoreStopAnimation = true;
                actor.Halt();
                actor.Sprite.ignoreStopAnimation = ignoreStopAnimation;
                bambooPoleEvent.resetDialogueIfNecessary(actor);
            }

            bambooPoleEvent.farmer.Halt();
            bambooPoleEvent.farmer.ignoreCollisions = false;
            Game1.exitActiveMenu();
            Game1.dialogueUp = false;
            Game1.dialogueTyping = false;
            Game1.pauseTime = 0.0f;

            CheckBambooPoleLocation();

            bambooPoleEvent.endBehaviors(new string[4]
            {
                "end",
                "position",
                "43",
                "36",
            }, Game1.currentLocation);
        }

        private static void CheckBambooPoleLocation()
        {
            _locationChecker.AddCheckedLocation("Bamboo Pole Cutscene");
        }

        private static void SkipFishingLessonEventArchipelago(Event bambooPoleEvent)
        {
            if (bambooPoleEvent.playerControlSequence)
            {
                bambooPoleEvent.EndPlayerControlSequence();
            }

            Game1.playSound("drumkit6");

            var actorPositionsAfterMoveField = _modHelper.Reflection.GetField<Dictionary<string, Vector3>>(bambooPoleEvent, "actorPositionsAfterMove");
            actorPositionsAfterMoveField.GetValue().Clear();

            foreach (var actor in bambooPoleEvent.actors)
            {
                var ignoreStopAnimation = actor.Sprite.ignoreStopAnimation;
                actor.Sprite.ignoreStopAnimation = true;
                actor.Halt();
                actor.Sprite.ignoreStopAnimation = ignoreStopAnimation;
                bambooPoleEvent.resetDialogueIfNecessary(actor);
            }

            bambooPoleEvent.farmer.Halt();
            bambooPoleEvent.farmer.ignoreCollisions = false;
            Game1.exitActiveMenu();
            Game1.dialogueUp = false;
            Game1.dialogueTyping = false;
            Game1.pauseTime = 0.0f;

            CheckFishingLessonLocations();

            bambooPoleEvent.endBehaviors(new string[4]
            {
                "end",
                "position",
                "43",
                "36",
            }, Game1.currentLocation);
        }

        private static void CheckFishingLessonLocations()
        {
            _locationChecker.AddCheckedLocation("Level 1 Fishing");
            _locationChecker.AddCheckedLocation("Purchase Training Rod");
        }
    }
}
