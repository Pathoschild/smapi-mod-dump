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
    public static class CopperPanInjections
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
        public static bool SkipEvent_CopperPan_Prefix(Event __instance)
        {
            try
            {
                if (__instance.id != EventIds.COPPER_PAN)
                {
                    return true; // run original logic
                }

                SkipCopperPanEventArchipelago(__instance);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SkipEvent_CopperPan_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public static void AwardFestivalPrize(Event @event, string[] args, EventContext context)
        public static bool AwardFestivalPrize_CopperPan_Prefix(Event @event, string[] args, EventContext context)
        {
            try
            {
                if (@event.id != EventIds.COPPER_PAN || args.Length <= 1 || args[1].ToLower() != "pan")
                {
                    return true; // run original logic
                }

                CheckCopperPanLocation();

                if (Game1.activeClickableMenu == null)
                    @event.CurrentCommand++;
                @event.CurrentCommand++;

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AwardFestivalPrize_CopperPan_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void SkipCopperPanEventArchipelago(Event copperPanEvent)
        {
            if (copperPanEvent.playerControlSequence)
            {
                copperPanEvent.EndPlayerControlSequence();
            }

            Game1.playSound("drumkit6");

            var actorPositionsAfterMoveField = _modHelper.Reflection.GetField<Dictionary<string, Vector3>>(copperPanEvent, "actorPositionsAfterMove");
            actorPositionsAfterMoveField.GetValue().Clear();

            foreach (var actor in copperPanEvent.actors)
            {
                var ignoreStopAnimation = actor.Sprite.ignoreStopAnimation;
                actor.Sprite.ignoreStopAnimation = true;
                actor.Halt();
                actor.Sprite.ignoreStopAnimation = ignoreStopAnimation;
                copperPanEvent.resetDialogueIfNecessary(actor);
            }

            copperPanEvent.farmer.Halt();
            copperPanEvent.farmer.ignoreCollisions = false;
            Game1.exitActiveMenu();
            Game1.dialogueUp = false;
            Game1.dialogueTyping = false;
            Game1.pauseTime = 0.0f;

            CheckCopperPanLocation();

            copperPanEvent.endBehaviors(new string[4]
            {
                "end",
                "position",
                "43",
                "36",
            }, Game1.currentLocation);
        }

        private static void CheckCopperPanLocation()
        {
            _locationChecker.AddCheckedLocation("Copper Pan Cutscene");
        }
    }
}
