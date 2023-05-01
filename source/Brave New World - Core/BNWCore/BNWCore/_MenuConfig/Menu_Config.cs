/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using StardewModdingAPI.Events;
using GenericModConfigMenu;

namespace BNWCore
{
    public class Menu_Config
    {
        public void OnGameLaunched(GameLaunchedEventArgs e)
        {
            var configMenu = ModEntry.ModHelper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;
            configMenu.Register(
                mod: ModEntry.Manifest,
                reset: () => ModEntry.Config = new ModConfig(),
                save: () => ModEntry.ModHelper.WriteConfig(ModEntry.Config)
            );
            configMenu.AddSectionTitle(
                mod: ModEntry.Manifest,
                text: () => ModEntry.ModHelper.Translation.Get("translator_menu_gameplay_section"),
                tooltip: () => ModEntry.ModHelper.Translation.Get("translator_menu_gameplay_section_tooltip")
            );
            configMenu.AddBoolOption(
                mod: ModEntry.Manifest,
                name: () => ModEntry.ModHelper.Translation.Get("translator_menu_gameplay_wateringxp_option"),
                tooltip: () => ModEntry.ModHelper.Translation.Get("translator_menu_gameplay_wateringxp_option_tooltip"),
                getValue: () => ModEntry.Config.BNWCoreEnableWateringXP,
                setValue: value => ModEntry.Config.BNWCoreEnableWateringXP = value
            );
            configMenu.AddSectionTitle(
                mod: ModEntry.Manifest,
                text: () => ModEntry.ModHelper.Translation.Get("translator_menu_grabber_section"),
                tooltip: () => ModEntry.ModHelper.Translation.Get("translator_menu_grabber_section_tooltip")
            );
            configMenu.AddBoolOption(
                mod: ModEntry.Manifest,
                name: () => ModEntry.ModHelper.Translation.Get("translator_menu_grabber_slimehutch_option"),
                tooltip: () => ModEntry.ModHelper.Translation.Get("translator_menu_grabber_slimehutch_option_tooltip"),
                getValue: () => ModEntry.Config.BNWCoreslimeHutch,
                setValue: value => ModEntry.Config.BNWCoreslimeHutch = value
            );
            configMenu.AddBoolOption(
                mod: ModEntry.Manifest,
                name: () => ModEntry.ModHelper.Translation.Get("translator_menu_grabber_harvestcrops_option"),
                tooltip: () => ModEntry.ModHelper.Translation.Get("translator_menu_grabber_harvestcrops_option_tooltip"),
                getValue: () => ModEntry.Config.BNWCoreharvestCrops,
                setValue: value => ModEntry.Config.BNWCoreharvestCrops = value
            );
            configMenu.AddBoolOption(
                mod: ModEntry.Manifest,
                name: () => ModEntry.ModHelper.Translation.Get("translator_menu_grabber_indoorpots_option"),
                tooltip: () => ModEntry.ModHelper.Translation.Get("translator_menu_grabber_indoorpots_option_tooltip"),
                getValue: () => ModEntry.Config.BNWCoreharvestCropsIndoorPots,
                setValue: value => ModEntry.Config.BNWCoreharvestCropsIndoorPots = value
            );
            configMenu.AddBoolOption(
                mod: ModEntry.Manifest,
                name: () => ModEntry.ModHelper.Translation.Get("translator_menu_grabber_artifact_option"),
                tooltip: () => ModEntry.ModHelper.Translation.Get("translator_menu_grabber_artifact_option_tooltip"),
                getValue: () => ModEntry.Config.BNWCoreartifactSpots,
                setValue: value => ModEntry.Config.BNWCoreartifactSpots = value
            );
            configMenu.AddBoolOption(
                mod: ModEntry.Manifest,
                name: () => ModEntry.ModHelper.Translation.Get("translator_menu_grabber_ore_option"),
                tooltip: () => ModEntry.ModHelper.Translation.Get("translator_menu_grabber_ore_option_tooltip"),
                getValue: () => ModEntry.Config.BNWCoreorePan,
                setValue: value => ModEntry.Config.BNWCoreorePan = value
            );
            configMenu.AddBoolOption(
                mod: ModEntry.Manifest,
                name: () => ModEntry.ModHelper.Translation.Get("translator_menu_grabber_fruittree_option"),
                tooltip: () => ModEntry.ModHelper.Translation.Get("translator_menu_grabber_fruittree_option_tooltip"),
                getValue: () => ModEntry.Config.BNWCorefruitTrees,
                setValue: value => ModEntry.Config.BNWCorefruitTrees = value
            );
            configMenu.AddBoolOption(
                mod: ModEntry.Manifest,
                name: () => ModEntry.ModHelper.Translation.Get("translator_menu_grabber_normaltree_option"),
                tooltip: () => ModEntry.ModHelper.Translation.Get("translator_menu_grabber_normaltree_option_tooltip"),
                getValue: () => ModEntry.Config.BNWCoreseedTrees,
                setValue: value => ModEntry.Config.BNWCoreseedTrees = value
            );
            configMenu.AddBoolOption(
                mod: ModEntry.Manifest,
                name: () => ModEntry.ModHelper.Translation.Get("translator_menu_grabber_harvestflower_option"),
                tooltip: () => ModEntry.ModHelper.Translation.Get("translator_menu_grabber_harvestflower_option_tooltip"),
                getValue: () => ModEntry.Config.BNWCoreflowers,
                setValue: value => ModEntry.Config.BNWCoreflowers = value
            );
            configMenu.AddBoolOption(
                mod: ModEntry.Manifest,
                name: () => ModEntry.ModHelper.Translation.Get("translator_menu_grabber_trashcan_option"),
                tooltip: () => ModEntry.ModHelper.Translation.Get("translator_menu_grabber_trashcan_option_tooltip"),
                getValue: () => ModEntry.Config.BNWCoregarbageCans,
                setValue: value => ModEntry.Config.BNWCoregarbageCans = value
            );
            configMenu.AddSectionTitle(
                mod: ModEntry.Manifest,
                text: () => ModEntry.ModHelper.Translation.Get("translator_menu_tools_section"),
                 tooltip: () => ModEntry.ModHelper.Translation.Get("translator_menu_tools_section_tooltip")
            );
            configMenu.AddNumberOption(
                mod: ModEntry.Manifest,
                name: () => ModEntry.ModHelper.Translation.Get("translator_menu_toollength_option"),
                tooltip: () => ModEntry.ModHelper.Translation.Get("translator_menu_toollength_option_tooltip"),
                getValue: () => ModEntry.Config.BNWCoreToolLength,
                setValue: value => ModEntry.Config.BNWCoreToolLength = value
            );
            configMenu.AddNumberOption(
                mod: ModEntry.Manifest,
                name: () => ModEntry.ModHelper.Translation.Get("translator_menu_toolwidth_option"),
                tooltip: () => ModEntry.ModHelper.Translation.Get("translator_menu_toolwidth_option_tooltip"),
                getValue: () => ModEntry.Config.BNWCoreToolWidth,
                setValue: value => ModEntry.Config.BNWCoreToolWidth = value
            );
            configMenu.AddSectionTitle(
                mod: ModEntry.Manifest,
                text: () => ModEntry.ModHelper.Translation.Get("translator_menu_soundloop_section"),
                 tooltip: () => ModEntry.ModHelper.Translation.Get("translator_menu_soundloop_section_tooltip")
            );
            configMenu.AddBoolOption(
                mod: ModEntry.Manifest,
                name: () => ModEntry.ModHelper.Translation.Get("translator_menu_soundloop_enable_option"),
                tooltip: () => ModEntry.ModHelper.Translation.Get("translator_menu_soundloop_enable_option_tooltip"),
                getValue: () => ModEntry.Config.BNWCoresetSoundLoop,
                setValue: value => ModEntry.Config.BNWCoresetSoundLoop = value
            );
        }
    }
}
