/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using GenericModConfigMenu;
using StardewDruid.Data;
using StardewDruid.Journal;
using System;
using System.Collections.Generic;
using System.Linq;

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
                name: () => "Rite",
                tooltip: () => "Configure the list or combination of keybinds to use for casting Rites",
                getValue: () => Config.riteButtons,
            setValue: value => Config.riteButtons = value
            );
            configMenu.AddKeybindList(
            mod: mod.ModManifest, 
                name: () => "Action (Druid Only)",
                tooltip: () => "Assigns an alternative keybind for the Action / Left Click / Use tool function, for the purposes of the mod only. This keybind does not override or re-map any keybinds in the base game. Useful for controllers with non-standard button maps.",
                  getValue: () => Config.actionButtons,
                setValue: value => Config.actionButtons = value
            );

            configMenu.AddKeybindList(
                mod: mod.ModManifest,
                name: () => "Special (Druid Only)",
                tooltip: () => "Assigns an alternative keybind for the Check / Special / Right Click / Placedown function, for the purposes of the mod only. This keybind does not override or re-map any keybinds in the base game. Useful for controllers with non-standard button maps.",
                getValue: () => Config.specialButtons,
                setValue: value => Config.specialButtons = value
            );

            configMenu.AddKeybindList(
                mod: mod.ModManifest,
                name: () => "Quests Journal",
                tooltip: () => "Keybind assignment to open the Stardew Druid effects journal while in world. The rite keybind can be used to open the journal from the game questlog.",
                getValue: () => Config.journalButtons,
                setValue: value => Config.journalButtons = value
            );

            configMenu.AddKeybindList(
                mod: mod.ModManifest,
                name: () => "Effects Journal",
                tooltip: () => "Keybind assignment to open the Stardew Druid effects journal while in world.",
                getValue: () => Config.effectsButtons,
                setValue: value => Config.effectsButtons = value
            );

            configMenu.AddKeybindList(
                mod: mod.ModManifest,
                name: () => "Relics Journal",
                tooltip: () => "Keybind assignment to open the Stardew Druid relics journal while in world.",
                getValue: () => Config.relicsButtons,
                setValue: value => Config.relicsButtons = value
            );

            configMenu.AddKeybindList(
                mod: mod.ModManifest,
                name: () => "Herbalism Journal",
                tooltip: () => "Keybind assignment to open the Stardew Druid herbalism journal while in world.",
                getValue: () => Config.herbalismButtons,
                setValue: value => Config.herbalismButtons = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Reverse Journal",
                tooltip: () => "Reverse the order in which Stardew Druid journal entries are displayed. Default: oldest to newest. Enabled: newest to oldest.",
                getValue: () => Config.reverseJournal,
                setValue: value => Config.reverseJournal = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Active Journal",
                tooltip: () => "Show active quests on the front pages of the Stardew Druid journal. Default: active entries on front page. Disabled: no change in order.",
                getValue: () => Config.activeJournal,
                setValue: value => Config.activeJournal = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Disable Cast Hands",
                tooltip: () => "Disables farmer sprite 'cast hands' animation when triggering events. Recommended disable if other game modifications make changes to the farmer rendering or draw cycle.",
                getValue: () => Config.disableHands,
                setValue: value => Config.disableHands = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Auto Progress",
                tooltip: () => "Automatically progress to the next stage of the questline after loading or starting a new day.",
                getValue: () => Config.autoProgress,
                setValue: value => Config.autoProgress = value
            );

            configMenu.AddNumberOption(
                mod: mod.ModManifest,
                name: () => "Set Progress",
                tooltip: () => "Use to adjust progress level on game load. 0 is no change. Note that adjustments may clear or miss levels of progress.",
                min: 0,
                max: Enum.GetNames(typeof(QuestHandle.milestones)).Count() - 1,
                interval: 1,
                getValue: () => Config.setMilestone,
                setValue: value => Config.setMilestone = (int)value,
                fieldId:"setProgress"
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Set Once",
                tooltip: () => "Automatically returns set progress to 0 after reconfiguring one save file.",
                getValue: () => Config.setOnce,
                setValue: value => Config.setOnce = value
            );

            string[] textOption = { "easy", "medium", "hard", };

            configMenu.AddTextOption(
                mod: mod.ModManifest,
                name: () => "Mod Difficulty",
                tooltip: () => "Select difficulty level for mod-spawned monsters and other effects.",
                allowedValues: textOption,
                getValue: () => Config.combatDifficulty,
                setValue: value => Config.combatDifficulty = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Slot Attune",
                tooltip: () => "Rite casts will be based on selected slot in the toolbar as opposed to weapon or tool attunement, as per the below slot assignments. [lunch] will consume any edible item in that slot when health or stamina is below 33%.",
                getValue: () => Config.slotAttune,
                setValue: value => Config.slotAttune = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Slot Consume",
                tooltip: () => "For slots set to [lunch], the mod will consume any edible item in that inventory slot when health or stamina is below 33%.",
                getValue: () => Config.slotConsume,
                setValue: value => Config.slotConsume = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Slot Freedom",
                tooltip: () => "Invalid tool selections will be ignored when slot-attune is active. Proceed with caution!",
                getValue: () => Config.slotFreedom,
                setValue: value => Config.slotFreedom = value
            );

            string[] slotOption = { "none","lunch","weald","mists","stars","fates","ether", };

            configMenu.AddTextOption(
                mod: mod.ModManifest,
                name: () => "Slot 1",
                tooltip: () => "Select slot behaviour for inventory slot one",
                allowedValues: slotOption,
                getValue: () => Config.slotOne,
                setValue: value => Config.slotOne = value
            );

            configMenu.AddTextOption(
                mod: mod.ModManifest,
                name: () => "Slot 2",
                tooltip: () => "Select slot behaviour for inventory slot two",
                allowedValues: slotOption,
                getValue: () => Config.slotTwo,
                setValue: value => Config.slotTwo = value
            );

            configMenu.AddTextOption(
                mod: mod.ModManifest,
                name: () => "Slot 3",
                tooltip: () => "Select slot behaviour for inventory slot three",
                allowedValues: slotOption,
                getValue: () => Config.slotThree,
                setValue: value => Config.slotThree = value
            );

            configMenu.AddTextOption(
                mod: mod.ModManifest,
                name: () => "Slot 4",
                tooltip: () => "Select slot behaviour for inventory slot four",
                allowedValues: slotOption,
                getValue: () => Config.slotFour,
                setValue: value => Config.slotFour = value
            );

            configMenu.AddTextOption(
                mod: mod.ModManifest,
                name: () => "Slot 5",
                tooltip: () => "Select slot behaviour for inventory slot five",
                allowedValues: slotOption,
                getValue: () => Config.slotFive,
                setValue: value => Config.slotFive = value
            );

            configMenu.AddTextOption(
                mod: mod.ModManifest,
                name: () => "Slot 6",
                tooltip: () => "Select slot behaviour for inventory slot six",
                allowedValues: slotOption,
                getValue: () => Config.slotSix,
                setValue: value => Config.slotSix = value
            );

            configMenu.AddTextOption(
                mod: mod.ModManifest,
                name: () => "Slot 7",
                tooltip: () => "Select slot behaviour for inventory slot seven",
                allowedValues: slotOption,
                getValue: () => Config.slotSeven,
                setValue: value => Config.slotSeven = value
            );

            configMenu.AddTextOption(
                mod: mod.ModManifest,
                name: () => "Slot 8",
                tooltip: () => "Select slot behaviour for inventory slot eight",
                allowedValues: slotOption,
                getValue: () => Config.slotEight,
                setValue: value => Config.slotEight = value
            );

            configMenu.AddTextOption(
                mod: mod.ModManifest,
                name: () => "Slot 9",
                tooltip: () => "Select slot behaviour for inventory slot nine",
                allowedValues: slotOption,
                getValue: () => Config.slotNine,
                setValue: value => Config.slotNine = value
            );

            configMenu.AddTextOption(
                mod: mod.ModManifest,
                name: () => "Slot 10",
                tooltip: () => "Select slot behaviour for inventory slot ten",
                allowedValues: slotOption,
                getValue: () => Config.slotTen,
                setValue: value => Config.slotTen = value
            );

            configMenu.AddTextOption(
                mod: mod.ModManifest,
                name: () => "Slot 11",
                tooltip: () => "Select slot behaviour for inventory slot eleven",
                allowedValues: slotOption,
                getValue: () => Config.slotEleven,
                setValue: value => Config.slotEleven = value
            );

            configMenu.AddTextOption(
                mod: mod.ModManifest,
                name: () => "Slot 12",
                tooltip: () => "Select slot behaviour for inventory slot twelve",
                allowedValues: slotOption,
                getValue: () => Config.slotTwelve,
                setValue: value => Config.slotTwelve = value
            );

            configMenu.AddNumberOption(
                mod: mod.ModManifest,
                name: () => "Cultivate Behaviour",
                tooltip: () => "Adjust settings for Weald: Cultivate in regards to crop handling. See readme for specifics. 1 Highest growth rate. 2 Average growth, average quality. 3 Highest quality.",
                min: 1,
                max: 3,
                interval: 1,
                getValue: () => Config.cultivateBehaviour,
                setValue: value => Config.cultivateBehaviour = value
            );

            configMenu.AddNumberOption(
                mod: mod.ModManifest,
                name: () => "Meteor Behaviour",
                tooltip: () => "Adjust risk/reward setting for Stars: Meteor. See readme for specifics. 1 Intelligent targetting, lowest damage. 5 Completely random targets, highest damage.",
                min: 1,
                max: 5,
                interval: 1,
                getValue: () => Config.meteorBehaviour,
                setValue: value => Config.meteorBehaviour = value
            );

            configMenu.AddNumberOption(
                mod: mod.ModManifest,
                name: () => "Adjust rewards (Percentage)",
                tooltip: () => "Adjust monetary rewards that are provided on quest completion.",
                min: 10,
                max: 200,
                interval: 10,
                getValue: () => Config.adjustRewards,
                setValue: value => Config.adjustRewards = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Maximum Damage",
                tooltip: () => "Some spell effects have damage modifiers that consider player combat level, highest upgrade on Pickaxe, Axe, and applied enchantments. Enable to cast at max damage and effect everytime.",
                getValue: () => Config.maxDamage,
                setValue: value => Config.maxDamage = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Cardinal Targetting",
                tooltip: () => "Disables isometric (6 way) targetting for transformation effects. Might look a little misaligned with the transformation animations.",
                getValue: () => Config.cardinalMovement,
                setValue: value => Config.cardinalMovement = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Cast Anywhere",
                tooltip: () => "Disables the Map-based cast restrictions so that any rite effect can be cast anywhere. Proceed with caution!",
                getValue: () => Config.castAnywhere,
                setValue: value => Config.castAnywhere = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Disable Seed Spawn",
                tooltip: () => "Disables wild seasonal seed spawn effect for Rite of the Weald.",
                getValue: () => Config.disableSeeds,
                setValue: value => Config.disableSeeds = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Disable Fish Spawn",
                tooltip: () => "Disables low grade fish spawn effect for Rite of the Weald.",
                getValue: () => Config.disableFish,
                setValue: value => Config.disableFish = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Disable Tree Spawn",
                tooltip: () => "Disables tree spawn effect for Rite of the Weald.",
                getValue: () => Config.disableTrees,
                setValue: value => Config.disableTrees = value
            );

            configMenu.AddBoolOption(
                mod: mod.ModManifest,
                name: () => "Disable Grass Spawn",
                tooltip: () => "Disables grass spawn effect for Rite of the Weald.",
                getValue: () => Config.disableGrass,
                setValue: value => Config.disableGrass = value
            );

            return configMenu;

        }


    }
}
