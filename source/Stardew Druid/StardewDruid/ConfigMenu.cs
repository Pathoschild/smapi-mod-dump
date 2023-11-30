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
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley.Monsters;

namespace StardewDruid
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

            configMenu.AddKeybindList(
                mod: mod.ModManifest,
                name: () => "Action Keybinds (Optional)",
                tooltip: () => "This configuration indicates to the mod which button corresponds to Action/Use tool. It's only for the purposes of reporting action presses, and does not override or re-map any keybinds in the base game. Useful for controllers with non-standard button maps.",
                getValue: () => Config.actionButtons,
                setValue: value => Config.actionButtons = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Slot Attune",
                tooltip: () => "Rite casts will be based on selected slot in the toolbar as opposed to weapon or tool attunement. Slot 1 = Earth, Slot 2 = Water, Slot 3 = Stars, Slot 4 = Fates.",
                getValue: () => Config.slotAttune,
                setValue: value => Config.slotAttune = value
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

            configMenu.AddNumberOption(
                mod: mod.ModManifest,
                name: () => "Set Progress",
                tooltip: () => "Use to adjust progress level on game load. -1 is no change. 0-5 Rite of the Earth, 6-11 Rite of the Water, 12-13 Rite of the Stars, 14 Hidden Challenges, 15-20 Rite of the Fates. Note that adjustments may clear or miss levels of progress.",
                min: -1,
                max: 21,
                interval: 1,
                getValue: () => Config.setProgress,
                setValue: value => Config.setProgress = value
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
                name: () => "Cast Anywhere",
                tooltip: () => "Disables the Map-based cast restrictions so that any rite effect can be cast anywhere. Proceed with caution!",
                getValue: () => Config.castAnywhere,
                setValue: value => Config.castAnywhere = value
            );

            /*configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Unrestricted Stars",
                tooltip: () => "Disables the cast buffer on Rite of Stars, so that it casts every button press instead of with reasonable delay.",
                getValue: () => Config.unrestrictedStars,
                setValue: value => Config.unrestrictedStars = value
            );*/

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Ostentatious Hats",
                tooltip: () => "Adds hats to some monsters. Cosmetic effect only.",
                getValue: () => Config.partyHats,
                setValue: value => Config.partyHats = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Disable Seed Spawn",
                tooltip: () => "Disables wild seasonal seed spawn effect for Rite of the Earth, despite in game effect management. Disabling, saving in game, then reenabling will require additional re-enablement in game.",
                getValue: () => Config.disableSeeds,
                setValue: value => Config.disableSeeds = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Disable Fish Spawn",
                tooltip: () => "Disables low grade fish spawn effect for Rite of the Earth, despite in game effect management. Disabling, saving in game, then reenabling will require additional re-enablement in game.",
                getValue: () => Config.disableFish,
                setValue: value => Config.disableFish = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Disable Wild Spawn",
                tooltip: () => "Disables wild monster spawn effect for Rite of the Earth, despite in game effect management. Disabling, saving in game, then reenabling will require additional re-enablement in game.",
                getValue: () => Config.disableWildspawn,
                setValue: value => Config.disableWildspawn = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Disable Tree Spawn",
                tooltip: () => "Disables tree and grass spawn effect for Rite of the Earth, despite in game effect management. Disabling, saving in game, then reenabling will require additional re-enablement in game.",
                getValue: () => Config.disableTrees,
                setValue: value => Config.disableTrees = value
            );

            return configMenu;

        }


    }
}
