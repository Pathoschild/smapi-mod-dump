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
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using StardewValley;
using StardewArchipelago.Locations.CodeInjections.Modded.SVE;

namespace StardewArchipelago.Locations.CodeInjections.Modded
{
    public static class ModdedEventInjections
    {
        private static readonly Dictionary<string, string[]> Base_Static_Events = new()
        {

        };

        private static readonly Dictionary<string, string[]> Total_Static_Events = new()
        {

        };

        private static readonly Dictionary<string, string[]> Base_OnWarped_Events = new()
        {

        };

        public static Dictionary<string, string[]> Total_OnWarped_Events = new()
        {

        };

        private static readonly string RECIPE_SUFFIX = " Recipe";

        private static readonly Dictionary<int, string> eventCooking = new()
        {
            { 181091234, "Mushroom Kebab" },
            { 181091246, "Crayfish Soup" },
            { 181091247, "Pemmican" },
            { 181091261, "Void Mint Tea" }, //Alecto
            { 181091262, "Void Mint Tea" }, //Wizard
        };

        private static readonly Dictionary<int, string> eventCrafting = new()
        {
            { 181091237, "Ginger Tincture" }, //Alecto
            { 1810912313, "Ginger Tincture" }, //Wizard
        };

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
        
        public static bool AddCookingRecipe_CheckForStrayRecipe_Prefix(Event __instance, GameLocation location, GameTime time, string[] split)
        {
            try
            {
                if (!_archipelago.SlotData.Chefsanity.HasFlag(Chefsanity.Friendship) || !eventCooking.Keys.Contains(__instance.id))
                {
                    return true;
                }

                _locationChecker.AddCheckedLocation($"{eventCooking[__instance.id]}{RECIPE_SUFFIX}");
                __instance.CurrentCommand++;
                return false; // don't run original logic

            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AddCookingRecipe_CheckForStrayRecipe_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool AddCraftingRecipe_CheckForStrayRecipe_Prefix(Event __instance, GameLocation location, GameTime time, string[] split)
        {
            try
            {
                if (!_archipelago.SlotData.Craftsanity.HasFlag(Craftsanity.All) || !eventCrafting.Keys.Contains(__instance.id))
                {
                    return true;
                }

                _locationChecker.AddCheckedLocation($"{eventCrafting[__instance.id]}{RECIPE_SUFFIX}");
                __instance.CurrentCommand++;
                return false; // don't run original logic

            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AddCraftingRecipe_CheckForStrayRecipe_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool SkipEvent_ReplaceRecipe_Prefix(Event __instance)
        {
            try
            {
                var cookingEvents = eventCooking.Keys;
                var craftingEvents = eventCooking.Keys;
                if (!craftingEvents.Contains(__instance.id) && !cookingEvents.Contains(__instance.id))
                {
                    return true; // run original logic
                }

                var cookingEvent = cookingEvents.Contains(__instance.id);
                SkipRecipeEventArchipelago(__instance, cookingEvent);
                return false; // don't run original logic

            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SkipEvent_ReplaceRecipe_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void SkipRecipeEventArchipelago(Event __instance, bool cookingEvent)
        {
            if (__instance.playerControlSequence)
            {
                __instance.EndPlayerControlSequence();
            }

            Game1.playSound("drumkit6");

            var actorPositionsAfterMoveField = _modHelper.Reflection.GetField<Dictionary<string, Vector3>>(__instance, "actorPositionsAfterMove");
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

            OnCheckRecipeLocation(__instance.id, cookingEvent);

            __instance.endBehaviors(new string[1]
            {
                "end"
            }, Game1.currentLocation);
        }


        private static void OnCheckRecipeLocation(int eventID, bool cookingEvent)
        {
            if (cookingEvent && _archipelago.SlotData.Chefsanity.HasFlag(Chefsanity.Friendship))
            {
                var eventName = eventCooking[eventID];
                var recipeName = $"{eventName}{RECIPE_SUFFIX}";
                _locationChecker.AddCheckedLocation(recipeName);
                if (_archipelago.GetReceivedItemCount(recipeName) <= 0)
                {
                    Game1.player.cookingRecipes.Remove(eventName);
                }

                return;

            }

            if (_archipelago.SlotData.Craftsanity.HasFlag(Craftsanity.All))
            {
                var eventName = eventCrafting[eventID];
                var recipeName = $"{eventName}{RECIPE_SUFFIX}";
                _locationChecker.AddCheckedLocation(recipeName);
                if (_archipelago.GetReceivedItemCount(recipeName) <= 0)
                {
                    Game1.player.craftingRecipes.Remove(eventName);
                }

            }

            return;
        }

        /*public static void SkipEvent_ReplaceRecipe_Postfix(Event __instance)
        {
            try
            {
                foreach(KeyValuePair<int, string> eventData in eventCooking)
                {
                    if (__instance.id == eventData.Key && _archipelago.SlotData.Chefsanity.HasFlag(Chefsanity.Friendship))
                    {
                        var recipeName = $"{eventData.Value}{RECIPE_SUFFIX}";
                        _locationChecker.AddCheckedLocation(recipeName);
                        if (_archipelago.GetReceivedItemCount(recipeName) <= 0)
                        {
                            Game1.player.cookingRecipes.Remove(eventData.Value);
                        }

                    }
                }
                foreach(KeyValuePair<int, string> eventData in eventCrafting)
                {
                    if (__instance.id == eventData.Key && _archipelago.SlotData.Craftsanity.HasFlag(Craftsanity.All))
                    {
                        var recipeName = $"{eventData.Value}{RECIPE_SUFFIX}";
                        _locationChecker.AddCheckedLocation(recipeName);
                        if (_archipelago.GetReceivedItemCount(recipeName) <= 0)
                        {
                            Game1.player.craftingRecipes.Remove(eventData.Value);
                        }

                    }
                }
                return;

            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SkipEvent_ReplaceRecipe_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }*/


    }
}