/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericModConfigMenu;
using StardewModdingAPI;
using StardewValley.Monsters;

namespace StardewDruid.Map
{
    public static class ConfigMenu
    {
    
        public static IGenericModConfigMenuApi MenuConfig(Mod mod)
        {

            ModData Config = mod.Config;

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenu is null)
            {
                return null;
            }

            // register mod
            configMenu.Register(
                mod: mod.ModManifest,
                reset: () => Config = new ModData(),
                save: () => mod.Helper.WriteConfig(Config)
            );

            configMenu.AddKeybindList(
                mod: mod.ModManifest,
                name: () => "Rite Keybinds",
                tooltip: () => "Configure the list of keybinds to use for casting Rites",
                getValue: () => Config.riteButtons,
                setValue: value => Config.riteButtons = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Cast Buffs",
                tooltip: () => "Enables magnetism buff when casting. Enables a conditional speed buff that activates when moving through grass while casting.",
                getValue: () => Config.castBuffs,
                setValue: value => Config.castBuffs = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Consume Roughage",
                tooltip: () => "Enables automatic consumption of usually inedible but often inventory-crowding items: Sap, Tree seeds, Slime, Batwings, Red mushrooms; Triggers when casting with critically low stamina. These items are far more abundant in Stardew Druid due to Rite of Earth behaviour.",
                getValue: () => Config.consumeRoughage,
                setValue: value => Config.consumeRoughage = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Consume Lunch",
                tooltip: () => "Enables automatic consumption of common sustenance items: SpringOnion, Snackbar, Mushrooms, Algae, Seaweed, Carrots, Sashimi, Salmonberry, Cheese; Triggers when casting with critically low stamina.",
                getValue: () => Config.consumeQuicksnack,
                setValue: value => Config.consumeQuicksnack = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Consume Caffeine",
                tooltip: () => "Enables automatic consumption of caffeinated items: Cola, Coffee Bean, Coffee, Triple Espresso; Triggers when casting with low stamina without an active drink buff.",
                getValue: () => Config.consumeCaffeine,
                setValue: value => Config.consumeCaffeine = value
            );

            /*configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Master Start",
                tooltip: () => "Start with all Rite levels unlocked (not recommended for immersion). Note that activating, saving in game, then deactivating afterwards, will reset all your Rite Levels and Effigy Quests.",
                getValue: () => Config.masterStart,
                setValue: value => Config.masterStart = value
            );*/

            configMenu.AddNumberOption(
                mod: mod.ModManifest,
                name: () => "Set Progress",
                tooltip: () => "Use to adjust progress level on game load. -1 is no change. 0-5 Rite of Earth, 6-11 Rite of Water, 12+ Rite of Stars. Note that adjustments may clear or miss levels of progress.",
                min: -1,
                max: 14,
                interval: 1,
                getValue: () => Config.setProgress,
                setValue: value => Config.setProgress = (int)value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Maximum Damage",
                tooltip: () => "Some spell effects have damage modifiers that consider player combat level, highest upgrade on Pickaxe, Axe, and applied enchantments. Enable to cast at max damage and effect everytime.",
                getValue: () => Config.maxDamage,
                setValue: value => Config.maxDamage = value
            );

            string[] textOption = { "easy", "medium", "hard", };

            configMenu.AddTextOption(
                mod: mod.ModManifest,
                name: () => "Monster Difficulty",
                tooltip: () => "Select difficulty level for mod-spawned monsters.",
                allowedValues: textOption,
                getValue: () => Config.combatDifficulty,
                setValue: value => Config.combatDifficulty = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Unrestricted Stars",
                tooltip: () => "Disables the cast buffer on Rite of Stars, so that it casts every button press instead of with reasonable delay.",
                getValue: () => Config.unrestrictedStars,
                setValue: value => Config.unrestrictedStars = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Ostentatious Hats",
                tooltip: () => "Adds hats to some monsters. Cosmetic effect only.",
                getValue: () => Config.partyHats,
                setValue: value => Config.partyHats = value
            );

            configMenu.AddNumberOption(
                mod: mod.ModManifest,
                name: () => "Effigy tileX",
                tooltip: () => "Move the Effigy's position in the farmcave on the X Axis",
                getValue: () => Config.farmCaveStatueX,
                setValue: value => Config.farmCaveStatueX = value
            );

            configMenu.AddNumberOption(
                mod: mod.ModManifest,
                name: () => "Effigy tileY",
                tooltip: () => "Move the Effigy's position in the farmcave on the Y Axis",
                getValue: () => Config.farmCaveStatueY,
                setValue: value => Config.farmCaveStatueY = value
            );

            configMenu.AddNumberOption(
                mod: mod.ModManifest,
                name: () => "Approach tileX location",
                tooltip: () => "Move the position of the action tile used to trigger dialogue with the Effigy on the X Axis.",
                getValue: () => Config.farmCaveActionX,
                setValue: value => Config.farmCaveActionX = value
            );

            configMenu.AddNumberOption(
                mod: mod.ModManifest,
                name: () => "Approach tileY location",
                tooltip: () => "Move the position of the action tile used to trigger dialogue with the Effigy on the Y Axis",
                getValue: () => Config.farmCaveActionY,
                setValue: value => Config.farmCaveActionY = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Hide Effigy",
                tooltip: () => "Hide the Effigy, and instead use the Rite Key anywhere on the farmcave map to converse with its disembodied voice",
                getValue: () => Config.farmCaveHideStatue,
                setValue: value => Config.farmCaveHideStatue = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Recess Effigy Space",
                tooltip: () => "Configure the backwall of the farmcave to situate the Effigy",
                getValue: () => Config.farmCaveMakeSpace,
                setValue: value => Config.farmCaveMakeSpace = value
            );

            return configMenu;

        }


    }
}
