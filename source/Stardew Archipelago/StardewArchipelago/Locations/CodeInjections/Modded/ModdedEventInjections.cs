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
using StardewValley;

namespace StardewArchipelago.Locations.CodeInjections.Modded
{
    public static class ModdedEventInjections
    {
        private const int MUSHROOM_KEBAB_EVENT = 181091234;
        private const int CRAYFISH_SOUP_EVENT = 181091246;
        private const int PEMMICAN_EVENT = 181091247;
        private const int VOID_MINT_TEA_ALECTO_EVENT = 181091261;
        private const int VOID_MINT_TEA_WIZARD_EVENT = 181091262;
        private const int SPECIAL_PUMPKIN_SOUP_EVENT = 44120020;
        private const int GINGER_TINCTURE_ALECTO_EVENT = 181091237;
        private const int GINGER_TINCTURE_WIZARD_EVENT = 1810912313;
        private static readonly string RECIPE_SUFFIX = " Recipe";

        private static readonly Dictionary<int, string> eventCooking = new()
        {
            { MUSHROOM_KEBAB_EVENT, "Mushroom Kebab" },
            { CRAYFISH_SOUP_EVENT, "Crayfish Soup" },
            { PEMMICAN_EVENT, "Pemmican" },
            { VOID_MINT_TEA_ALECTO_EVENT, "Void Mint Tea" },
            { VOID_MINT_TEA_WIZARD_EVENT, "Void Mint Tea" },
            { SPECIAL_PUMPKIN_SOUP_EVENT, "Special Pumpkin Soup" },
        };
        private static readonly Dictionary<int, string> eventCrafting = new()
        {
            { GINGER_TINCTURE_ALECTO_EVENT, "Ginger Tincture" },
            { GINGER_TINCTURE_WIZARD_EVENT, "Ginger Tincture" },
        };
        private static readonly List<int> questEventsWithRecipes = new()
        {
            CRAYFISH_SOUP_EVENT, SPECIAL_PUMPKIN_SOUP_EVENT, GINGER_TINCTURE_ALECTO_EVENT, GINGER_TINCTURE_WIZARD_EVENT
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
                var isEventChefsanityLocation = _archipelago.SlotData.Chefsanity.HasFlag(Chefsanity.Friendship) && eventCooking.Keys.Contains(__instance.id);
                var isRecipeFromQuest = _archipelago.SlotData.QuestLocations.StoryQuestsEnabled && questEventsWithRecipes.Contains(__instance.id);
                if (!isRecipeFromQuest && !isEventChefsanityLocation)
                {
                    return true;
                }
                if (!isRecipeFromQuest)
                {
                    _locationChecker.AddCheckedLocation($"{eventCooking[__instance.id]}{RECIPE_SUFFIX}");
                }
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
                var isEventCraftsanityLocation = _archipelago.SlotData.Craftsanity.HasFlag(Craftsanity.All) && eventCrafting.Keys.Contains(__instance.id);
                var isRecipeFromQuest = _archipelago.SlotData.QuestLocations.StoryQuestsEnabled && questEventsWithRecipes.Contains(__instance.id);

                if (!isRecipeFromQuest && !isEventCraftsanityLocation)
                {
                    return true;
                }

                if (!isRecipeFromQuest)
                {
                    _locationChecker.AddCheckedLocation($"{eventCrafting[__instance.id]}{RECIPE_SUFFIX}");
                }
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
                var craftingEvents = eventCrafting.Keys;
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