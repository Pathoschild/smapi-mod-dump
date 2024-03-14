/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dunc4nNT/StardewMods
**
*************************************************/

using NeverToxic.StardewMods.Common;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace NeverToxic.StardewMods.YetAnotherFishingMod.Framework
{
    internal class GenericModConfigMenu(IModRegistry modRegistry, IManifest manifest, IMonitor monitor, Func<ModConfig> config, Action reset, Action save, List<string> baitList, List<string> tackeList)
    {
        public void Register()
        {
            var configMenu = modRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenu is null)
                return;

            configMenu.Register(mod: manifest, reset: reset, save: save);

            configMenu.AddPageLink(
                mod: manifest,
                pageId: "NeverToxic.YetAnotherFishingMod.General",
                text: () => I18n.Config_General_PageTitle()
            );
            configMenu.AddPageLink(
                mod: manifest,
                pageId: "NeverToxic.YetAnotherFishingMod.Attachments",
                text: () => I18n.Config_Attachments_PageTitle()
            );
            configMenu.AddPageLink(
                mod: manifest,
                pageId: "NeverToxic.YetAnotherFishingMod.Enchantments",
                text: () => I18n.Config_Enchantments_PageTitle()
            );

            configMenu.AddPage(
                mod: manifest,
                pageId: "NeverToxic.YetAnotherFishingMod.General",
                pageTitle: () => I18n.Config_General_PageTitle()
            );
            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => I18n.Config_General_GeneralSection()
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => I18n.Config_General_CatchFishRetries_Name(),
                tooltip: () => I18n.Config_General_CatchFishRetries_Tooltip(),
                getValue: () => config().CatchFishRetries,
                setValue: value => config().CatchFishRetries = value,
                min: 0,
                max: 100
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => I18n.Config_General_InstantCatchFish_Name(),
                tooltip: () => I18n.Config_General_InstantCatchFish_Tooltip(),
                getValue: () => config().InstantCatchFish,
                setValue: value => config().InstantCatchFish = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => I18n.Config_General_InstantCatchTreasure_Name(),
                tooltip: () => I18n.Config_General_InstantCatchTreasure_Tooltip(),
                getValue: () => config().InstantCatchTreasure,
                setValue: value => config().InstantCatchTreasure = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => I18n.Config_General_AlwaysMaxCastingPower_Name(),
                tooltip: () => I18n.Config_General_AlwaysMaxCastingPower_Tooltip(),
                getValue: () => config().AlwaysMaxCastingPower,
                setValue: value => config().AlwaysMaxCastingPower = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => I18n.Config_General_AlwaysPerfect_Name(),
                tooltip: () => I18n.Config_General_AlwaysPerfect_Tooltip(),
                getValue: () => config().AlwaysPerfect,
                setValue: value => config().AlwaysPerfect = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => I18n.Config_General_TreasureAppearance_Name(),
                tooltip: () => I18n.Config_General_TreasureAppearance_Tooltip(),
                getValue: () => config().TreasureAppearence.ToString(),
                setValue: value => config().TreasureAppearence = (TreasureAppearanceSettings)Enum.Parse(typeof(TreasureAppearanceSettings), value),
                allowedValues: Enum.GetNames(typeof(TreasureAppearanceSettings)),
                formatAllowedValue: value => I18n.GetByKey($"config.treasure-appearance-{value}")
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => I18n.Config_General_AlwaysCatchDouble_Name(),
                tooltip: () => I18n.Config_General_AlwaysCatchDouble_Tooltip(),
                getValue: () => config().AlwaysCatchDouble,
                setValue: value => config().AlwaysCatchDouble = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => I18n.Config_General_InstantBite_Name(),
                tooltip: () => I18n.Config_General_InstantBite_Tooltip(),
                getValue: () => config().InstantBite,
                setValue: value => config().InstantBite = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => I18n.Config_General_AutoHook_Name(),
                tooltip: () => I18n.Config_General_AutoHook_Tooltip(),
                getValue: () => config().AutoHook,
                setValue: value => config().AutoHook = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => I18n.Config_General_AutoLootTreasure_Name(),
                tooltip: () => I18n.Config_General_AutoLootTreasure_Tooltip(),
                getValue: () => config().AutoLootTreasure,
                setValue: value => config().AutoLootTreasure = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => I18n.Config_General_DifficultyMultiplier_Name(),
                tooltip: () => I18n.Config_General_DifficultyMultiplier_Tooltip(),
                getValue: () => (int)config().DifficultyMultiplier,
                setValue: value => config().DifficultyMultiplier = value,
                min: (float)0.1,
                max: (float)5.0
            );
            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => I18n.Config_General_QualitySection()
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => I18n.Config_General_FishQuality_Name(),
                tooltip: () => I18n.Config_General_FishQuality_Tooltip(),
                getValue: () => config().FishQuality.ToString(),
                setValue: value => config().FishQuality = (Quality)Enum.Parse(typeof(Quality), value),
                allowedValues: Enum.GetNames(typeof(Quality)),
                formatAllowedValue: value => I18n.GetByKey($"config.quality-{value}")
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => I18n.Config_General_MinimumFishQuality_Name(),
                tooltip: () => I18n.Config_General_MinimumFishQuality_Tooltip(),
                getValue: () => config().MinimumFishQuality.ToString(),
                setValue: value => config().MinimumFishQuality = (Quality)Enum.Parse(typeof(Quality), value),
                allowedValues: Enum.GetNames(typeof(Quality)),
                formatAllowedValue: value => I18n.GetByKey($"config.quality-{value}")
            );

            configMenu.AddPage(
                mod: manifest,
                pageId: "NeverToxic.YetAnotherFishingMod.Attachments",
                pageTitle: () => I18n.Config_Attachments_PageTitle()
            );
            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => I18n.Config_Attachments_GeneralSection()
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => I18n.Config_Attachments_OverrideAttachmentLimit_Name(),
                tooltip: () => I18n.Config_Attachments_OverrideAttachmentLimit_Tooltip(),
                getValue: () => config().OverrideAttachmentLimit,
                setValue: value => config().OverrideAttachmentLimit = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => I18n.Config_Attachments_ResetAttachments_Name(),
                tooltip: () => I18n.Config_Attachments_ResetAttachments_Tooltip(),
                getValue: () => config().ResetAttachmentsWhenNotEquipped,
                setValue: value => config().ResetAttachmentsWhenNotEquipped = value
            );
            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => I18n.Config_Attachments_BaitSection()
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => I18n.Config_Attachments_SpawnBaitWhenEquipped_Name(),
                tooltip: () => I18n.Config_Attachments_SpawnBaitWhenEquipped_Tooltip(),
                getValue: () => config().SpawnBaitWhenEquipped,
                setValue: value => config().SpawnBaitWhenEquipped = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => I18n.Config_Attachments_SpawnWhichBait_Name(),
                tooltip: () => I18n.Config_Attachments_SpawnWhichBait_Tooltip(),
                getValue: () => config().SpawnWhichBait,
                setValue: value => config().SpawnWhichBait = value,
                allowedValues: [.. baitList],
                formatAllowedValue: value => ItemRegistry.GetData(value).DisplayName
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => I18n.Config_Attachments_InfiniteBait_Name(),
                tooltip: () => I18n.Config_Attachments_InfiniteBait_Tooltip(),
                getValue: () => config().InfiniteBait,
                setValue: value => config().InfiniteBait = value
            );
            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => I18n.Config_Attachments_TacklesSection()
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => I18n.Config_Attachments_SpawnTackleWhenEquipped_Name(),
                tooltip: () => I18n.Config_Attachments_SpawnTackleWhenEquipped_Tooltip(),
                getValue: () => config().SpawnTackleWhenEquipped,
                setValue: value => config().SpawnTackleWhenEquipped = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => I18n.Config_Attachments_SpawnWhichTackle_Name(),
                tooltip: () => I18n.Config_Attachments_SpawnWhichTackle_Tooltip(),
                getValue: () => config().SpawnWhichTackle,
                setValue: value => config().SpawnWhichTackle = value,
                allowedValues: [.. tackeList],
                formatAllowedValue: value => ItemRegistry.GetData(value).DisplayName
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => I18n.Config_Attachments_InfiniteTackle_Name(),
                tooltip: () => I18n.Config_Attachments_InfiniteTackle_Tooltip(),
                getValue: () => config().InfiniteTackle,
                setValue: value => config().InfiniteTackle = value
            );

            configMenu.AddPage(
                mod: manifest,
                pageId: "NeverToxic.YetAnotherFishingMod.Enchantments",
                pageTitle: () => I18n.Config_Enchantments_PageTitle()
            );
            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => I18n.Config_Enchantments_GeneralSection()
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => I18n.Config_Enchantments_DoAddEnchantments_Name(),
                tooltip: () => I18n.Config_Enchantments_DoAddEnchantments_Tooltip(),
                getValue: () => config().DoAddEnchantments,
                setValue: value => config().DoAddEnchantments = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => I18n.Config_Enchantments_ResetEnchantments_Name(),
                tooltip: () => I18n.Config_Enchantments_ResetEnchantments_Tooltip(),
                getValue: () => config().ResetEnchantmentsWhenNotEquipped,
                setValue: value => config().ResetEnchantmentsWhenNotEquipped = value
            );
            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => I18n.Config_Enchantments_EnchantmentsSection()
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => I18n.Config_Enchantments_AddAllEnchantments_Name(),
                tooltip: () => I18n.Config_Enchantments_AddAllEnchantments_Tooltip(),
                getValue: () => config().AddAllEnchantments,
                setValue: value => config().AddAllEnchantments = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => I18n.Config_Enchantments_AddAutoHookEnchantment_Name(),
                tooltip: () => I18n.Config_Enchantments_AddAutoHookEnchantment_Tooltip(),
                getValue: () => config().AddAutoHookEnchantment,
                setValue: value => config().AddAutoHookEnchantment = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => I18n.Config_Enchantments_AddEfficientToolEnchantment_Name(),
                tooltip: () => I18n.Config_Enchantments_AddEfficientToolEnchantment_Tooltip(),
                getValue: () => config().AddEfficientToolEnchantment,
                setValue: value => config().AddEfficientToolEnchantment = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => I18n.Config_Enchantments_AddMasterEnchantment_Name(),
                tooltip: () => I18n.Config_Enchantments_AddMasterEnchantment_Tooltip(),
                getValue: () => config().AddMasterEnchantment,
                setValue: value => config().AddMasterEnchantment = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => I18n.Config_Enchantments_AddPreservingEnchantment_Name(),
                tooltip: () => I18n.Config_Enchantments_AddPreservingEnchantment_Tooltip(),
                getValue: () => config().AddPreservingEnchantment,
                setValue: value => config().AddPreservingEnchantment = value
            );
        }
    }
}
