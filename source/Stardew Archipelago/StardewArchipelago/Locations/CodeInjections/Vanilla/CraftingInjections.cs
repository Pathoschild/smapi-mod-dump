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
using StardewArchipelago.Constants.Modded;
using StardewArchipelago.Stardew.Ids.Vanilla;
using StardewArchipelago.Stardew.NameMapping;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class CraftingInjections
    {
        public const string CRAFTING_LOCATION_PREFIX = "Craft ";

        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static CompoundNameMapper _nameMapper;

        public static void Initialize(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _helper = helper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _nameMapper = new CompoundNameMapper(archipelago.SlotData);
        }

        // public void checkForCraftingAchievements()
        public static void CheckForCraftingAchievements_CheckCraftsanityLocation_Postfix(Stats __instance)
        {
            try
            {
                var craftedRecipes = Game1.player.craftingRecipes;
                foreach (var recipe in craftedRecipes.Keys)
                {
                    if (craftedRecipes[recipe] <= 0)
                    {
                        continue;
                    }
                    var itemName = _nameMapper.GetItemName(recipe); // Some names are iffy
                    if (IgnoredModdedStrings.Craftables.Contains(itemName))
                    {
                        continue;
                    }
                    var location = $"{CRAFTING_LOCATION_PREFIX}{itemName}";
                    _locationChecker.AddCheckedLocation(location);
                }

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckForCraftingAchievements_CheckCraftsanityLocation_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public static void AddCraftingRecipe(Event @event, string[] args, EventContext context)
        public static bool AddCraftingRecipe_SkipLearningFurnace_Prefix(Event @event, string[] args, EventContext context)
        {
            try
            {
                if (!@event.eventCommands[@event.CurrentCommand].Contains("Furnace"))
                {
                    return true; // run original logic
                }

                ++@event.CurrentCommand;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AddCraftingRecipe_SkipLearningFurnace_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public void skipEvent()
        public static bool SkipEvent_FurnaceRecipe_Prefix(Event __instance)
        {
            try
            {
                if (__instance.id != EventIds.FURNACE_RECIPE)
                {
                    return true; // run original logic
                }

                SkipFurnaceRecipeEventArchipelago(__instance);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SkipEvent_FurnaceRecipe_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void SkipFurnaceRecipeEventArchipelago(Event furnaceEvent)
        {
            if (furnaceEvent.playerControlSequence)
            {
                furnaceEvent.EndPlayerControlSequence();
            }

            Game1.playSound("drumkit6");

            var actorPositionsAfterMoveField = _helper.Reflection.GetField<Dictionary<string, Vector3>>(furnaceEvent, "actorPositionsAfterMove");
            actorPositionsAfterMoveField.GetValue().Clear();

            foreach (var actor in furnaceEvent.actors)
            {
                var ignoreStopAnimation = actor.Sprite.ignoreStopAnimation;
                actor.Sprite.ignoreStopAnimation = true;
                actor.Halt();
                actor.Sprite.ignoreStopAnimation = ignoreStopAnimation;
                furnaceEvent.resetDialogueIfNecessary(actor);
            }

            furnaceEvent.farmer.Halt();
            furnaceEvent.farmer.ignoreCollisions = false;
            Game1.exitActiveMenu();
            Game1.dialogueUp = false;
            Game1.dialogueTyping = false;
            Game1.pauseTime = 0.0f;

            // Game1.player.craftingRecipes.TryAdd("Furnace", 0);
            Game1.player.addQuest("11");
            furnaceEvent.endBehaviors();
        }
    }
}
