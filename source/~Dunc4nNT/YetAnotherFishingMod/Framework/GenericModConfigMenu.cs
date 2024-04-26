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
using StardewValley.Tools;
using System;
using System.Collections.Generic;

namespace NeverToxic.StardewMods.YetAnotherFishingMod.Framework
{
    internal class GenericModConfigMenu(IModRegistry modRegistry, IManifest manifest, IMonitor monitor, Func<ModConfig> config, Action reset, Action save, List<string> baitList, List<string> tackeList)
    {
        public void Register()
        {
            IGenericModConfigMenuApi configMenu = modRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenu is null)
                return;

            configMenu.Register(mod: manifest, reset: reset, save: save);

            configMenu.AddPageLink(
                mod: manifest,
                pageId: "NeverToxic.YetAnotherFishingMod.General",
                text: I18n.Config_General_PageTitle
            );
            configMenu.AddPageLink(
                mod: manifest,
                pageId: "NeverToxic.YetAnotherFishingMod.Attachments",
                text: I18n.Config_Attachments_PageTitle
            );
            configMenu.AddPageLink(
                mod: manifest,
                pageId: "NeverToxic.YetAnotherFishingMod.Enchantments",
                text: I18n.Config_Enchantments_PageTitle
            );

            configMenu.AddPage(
                mod: manifest,
                pageId: "NeverToxic.YetAnotherFishingMod.General",
                pageTitle: I18n.Config_General_PageTitle
            );
            configMenu.AddSectionTitle(
                mod: manifest,
                text: I18n.Config_General_MinigameSection
            );
            configMenu.AddKeybindList(
                mod: manifest,
                name: I18n.Config_General_DoAutoCast_Name,
                tooltip: I18n.Config_General_DoAutoCast_Tooltip,
                getValue: () => config().Keys.DoAutoCast,
                setValue: value => config().Keys.DoAutoCast = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_General_SkipFishingMinigame_Name,
                tooltip: I18n.Config_General_SkipFishingMinigame_Tooltip,
                getValue: () => config().SkipFishingMinigame,
                setValue: value => config().SkipFishingMinigame = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_General_SkipMinigamePopup_Name,
                tooltip: I18n.Config_General_SkipMinigamePopup_Tooltip,
                getValue: () => config().SkipMinigamePopup,
                setValue: value => config().SkipMinigamePopup = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: I18n.Config_General_SkipFishingMinigameCatchesRequired_Name,
                tooltip: I18n.Config_General_SkipFishingMinigameCatchesRequired_Tooltip,
                getValue: () => config().SkipFishingMinigameCatchesRequired,
                setValue: value => config().SkipFishingMinigameCatchesRequired = value,
                min: 0,
                max: 100,
                interval: 5
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: I18n.Config_General_SkipFishingMinigamePerfectChance_Name,
                tooltip: I18n.Config_General_SkipFishingMinigamePerfectChance_Tooltip,
                getValue: () => config().SkipFishingMinigamePerfectChance,
                setValue: value => config().SkipFishingMinigamePerfectChance = value,
                min: 0f,
                max: 1f,
                interval: 0.01f
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: I18n.Config_General_SkipFishingMinigameTreasureChance_Name,
                tooltip: I18n.Config_General_SkipFishingMinigameTreasureChance_Tooltip,
                getValue: () => config().SkipFishingMinigameTreasureChance,
                setValue: value => config().SkipFishingMinigameTreasureChance = value,
                min: 0f,
                max: 1f,
                interval: 0.01f
            );
            configMenu.AddSectionTitle(
                mod: manifest,
                text: I18n.Config_General_DifficultySection
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: I18n.Config_General_DifficultyMultiplier_Name,
                tooltip: I18n.Config_General_DifficultyMultiplier_Tooltip,
                getValue: () => config().DifficultyMultiplier,
                setValue: value => config().DifficultyMultiplier = value,
                min: 0.1f,
                max: 2f,
                interval: 0.1f
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_General_AdjustXpGainDifficulty_Name,
                tooltip: I18n.Config_General_AdjustXpGainDifficulty_Tooltip,
                getValue: () => config().AdjustXpGainDifficulty,
                setValue: value => config().AdjustXpGainDifficulty = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: I18n.Config_General_FishInBarMultiplier_Name,
                tooltip: I18n.Config_General_FishInBarMultiplier_Tooltip,
                getValue: () => config().FishInBarMultiplier,
                setValue: value => config().FishInBarMultiplier = value,
                min: 0.1f,
                max: 5f,
                interval: 0.1f
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: I18n.Config_General_FishNotInBarPenaltyMultiplier_Name,
                tooltip: I18n.Config_General_FishNotInBarPenaltyMultiplier_Tooltip,
                getValue: () => config().FishNotInBarPenaltyMultiplier,
                setValue: value => config().FishNotInBarPenaltyMultiplier = value,
                min: 0f,
                max: 2f,
                interval: 0.1f
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: I18n.Config_General_BarSizeMultiplier_Name,
                tooltip: I18n.Config_General_BarSizeMultiplier_Tooltip,
                getValue: () => config().BarSizeMultiplier,
                setValue: value => config().BarSizeMultiplier = value,
                min: 0.5f,
                max: 2f,
                interval: 0.1f
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_General_AlwaysMaxCastingPower_Name,
                tooltip: I18n.Config_General_AlwaysMaxCastingPower_Tooltip,
                getValue: () => config().AlwaysMaxCastingPower,
                setValue: value => config().AlwaysMaxCastingPower = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_General_AutoHook_Name,
                tooltip: I18n.Config_General_AutoHook_Tooltip,
                getValue: () => config().AutoHook,
                setValue: value => config().AutoHook = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_General_AlwaysPerfect_Name,
                tooltip: I18n.Config_General_AlwaysPerfect_Tooltip,
                getValue: () => config().AlwaysPerfect,
                setValue: value => config().AlwaysPerfect = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: I18n.Config_General_TreasureInBarMultiplier_Name,
                tooltip: I18n.Config_General_TreasureInBarMultiplier_Tooltip,
                getValue: () => config().TreasureInBarMultiplier,
                setValue: value => config().TreasureInBarMultiplier = value,
                min: 0.1f,
                max: 5f,
                interval: 0.1f
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_General_InstantCatchTreasure_Name,
                tooltip: I18n.Config_General_InstantCatchTreasure_Tooltip,
                getValue: () => config().InstantCatchTreasure,
                setValue: value => config().InstantCatchTreasure = value
            );
            configMenu.AddSectionTitle(
                mod: manifest,
                text: I18n.Config_General_FishingLootSection
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_General_AllowCatchingFish_Name,
                tooltip: I18n.Config_General_AllowCatchingFish_Tooltip,
                getValue: () => config().AllowCatchingFish,
                setValue: value => config().AllowCatchingFish = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_General_AllowCatchingRubbish_Name,
                tooltip: I18n.Config_General_AllowCatchingRubbish_Tooltip,
                getValue: () => config().AllowCatchingRubbish,
                setValue: value => config().AllowCatchingRubbish = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_General_AllowCatchingOther_Name,
                tooltip: I18n.Config_General_AllowCatchingOther_Tooltip,
                getValue: () => config().AllowCatchingOther,
                setValue: value => config().AllowCatchingOther = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: I18n.Config_General_NumberOfFishCaught_Name,
                tooltip: I18n.Config_General_NumberOfFishCaught_Tooltip,
                getValue: () => config().NumberOfFishCaught,
                setValue: value => config().NumberOfFishCaught = value,
                min: 1,
                max: 100
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: I18n.Config_General_FishQuality_Name,
                tooltip: I18n.Config_General_FishQuality_Tooltip,
                getValue: () => config().FishQuality.ToString(),
                setValue: value => config().FishQuality = (Quality)Enum.Parse(typeof(Quality), value),
                allowedValues: Enum.GetNames(typeof(Quality)),
                formatAllowedValue: value => I18n.GetByKey($"config.quality-{value}")
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: I18n.Config_General_MinimumFishQuality_Name,
                tooltip: I18n.Config_General_MinimumFishQuality_Tooltip,
                getValue: () => config().MinimumFishQuality.ToString(),
                setValue: value => config().MinimumFishQuality = (Quality)Enum.Parse(typeof(Quality), value),
                allowedValues: Enum.GetNames(typeof(Quality)),
                formatAllowedValue: value => I18n.GetByKey($"config.quality-{value}")
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: I18n.Config_General_TreasureAppearance_Name,
                tooltip: I18n.Config_General_TreasureAppearance_Tooltip,
                getValue: () => config().TreasureAppearence.ToString(),
                setValue: value => config().TreasureAppearence = (TreasureAppearanceSettings)Enum.Parse(typeof(TreasureAppearanceSettings), value),
                allowedValues: Enum.GetNames(typeof(TreasureAppearanceSettings)),
                formatAllowedValue: value => I18n.GetByKey($"config.treasure-appearance-{value}")
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: I18n.Config_General_GoldenTreasureAppearance_Name,
                tooltip: I18n.Config_General_GoldenTreasureAppearance_Tooltip,
                getValue: () => config().GoldenTreasureAppearance.ToString(),
                setValue: value => config().GoldenTreasureAppearance = (TreasureAppearanceSettings)Enum.Parse(typeof(TreasureAppearanceSettings), value),
                allowedValues: Enum.GetNames(typeof(TreasureAppearanceSettings)),
                formatAllowedValue: value => I18n.GetByKey($"config.treasure-appearance-{value}")
            );
            configMenu.AddSectionTitle(
                mod: manifest,
                text: I18n.Config_General_FasterPleaseSection
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_General_InstantBite_Name,
                tooltip: I18n.Config_General_InstantBite_Tooltip,
                getValue: () => config().InstantBite,
                setValue: value => config().InstantBite = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_General_AutoLootFish_Name,
                tooltip: I18n.Config_General_AutoLootFish_Tooltip,
                getValue: () => config().AutoLootFish,
                setValue: value => config().AutoLootFish = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_General_AutoLootTreasure_Name,
                tooltip: I18n.Config_General_AutoLootTreasure_Tooltip,
                getValue: () => config().AutoLootTreasure,
                setValue: value => config().AutoLootTreasure = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_General_DoSpeedUpAnimations_Name,
                tooltip: I18n.Config_General_DoSpeedUpAnimations_Tooltip,
                getValue: () => config().DoSpeedUpAnimations,
                setValue: value => config().DoSpeedUpAnimations = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_General_DisableVibrations_Name,
                tooltip: I18n.Config_General_DisableVibrations_Tooltip,
                getValue: () => config().DisableVibrations,
                setValue: value => config().DisableVibrations = value
            );

            configMenu.AddPage(
                mod: manifest,
                pageId: "NeverToxic.YetAnotherFishingMod.Attachments",
                pageTitle: I18n.Config_Attachments_PageTitle
            );
            configMenu.AddSectionTitle(
                mod: manifest,
                text: I18n.Config_Attachments_GeneralSection
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Attachments_OverrideAttachmentLimit_Name,
                tooltip: I18n.Config_Attachments_OverrideAttachmentLimit_Tooltip,
                getValue: () => config().OverrideAttachmentLimit,
                setValue: value => config().OverrideAttachmentLimit = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Attachments_ResetAttachmentsLimitWhenNotEquipped_Name,
                tooltip: I18n.Config_Attachments_ResetAttachmentsLimitWhenNotEquipped_Tooltip,
                getValue: () => config().ResetAttachmentsLimitWhenNotEquipped,
                setValue: value => config().ResetAttachmentsLimitWhenNotEquipped = value
            );
            configMenu.AddSectionTitle(
                mod: manifest,
                text: I18n.Config_Attachments_BaitSection
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Attachments_InfiniteBait_Name,
                tooltip: I18n.Config_Attachments_InfiniteBait_Tooltip,
                getValue: () => config().InfiniteBait,
                setValue: value => config().InfiniteBait = value
            );

            for (int i = FishingRod.BaitIndex; i < FishingRod.TackleIndex; i++)
            {
                int baitIndex = i;

                configMenu.AddTextOption(
                    mod: manifest,
                    name: () => I18n.Config_Attachments_SpawnWhichBait_Name() + $" ({baitIndex + 1})",
                    tooltip: I18n.Config_Attachments_SpawnWhichBait_Tooltip,
                    getValue: () => config().BaitToSpawn[baitIndex],
                    setValue: value => config().BaitToSpawn[baitIndex] = value,
                    allowedValues: [.. baitList],
                    formatAllowedValue: value => ItemRegistry.GetData(value)?.DisplayName
                );
            }

            configMenu.AddNumberOption(
                mod: manifest,
                name: I18n.Config_Attachments_AmountOfBait_Name,
                tooltip: I18n.Config_Attachments_AmountOfBait_Tooltip,
                getValue: () => config().AmountOfBait,
                setValue: value => config().AmountOfBait = value,
                min: 9,
                max: 999,
                interval: 9
            );

            configMenu.AddSectionTitle(
                mod: manifest,
                text: I18n.Config_Attachments_TacklesSection
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Attachments_InfiniteTackle_Name,
                tooltip: I18n.Config_Attachments_InfiniteTackle_Tooltip,
                getValue: () => config().InfiniteTackle,
                setValue: value => config().InfiniteTackle = value
            );

            for (int i = FishingRod.TackleIndex; i < FishingRod.TackleIndex + 2; i++)
            {
                int tackleIndex = i - FishingRod.TackleIndex;

                configMenu.AddTextOption(
                    mod: manifest,
                    name: () => I18n.Config_Attachments_SpawnWhichTackle_Name() + $" ({tackleIndex + 1})",
                    tooltip: I18n.Config_Attachments_SpawnWhichTackle_Tooltip,
                    getValue: () => config().TacklesToSpawn[tackleIndex],
                    setValue: value => config().TacklesToSpawn[tackleIndex] = value,
                    allowedValues: [.. tackeList],
                    formatAllowedValue: value => ItemRegistry.GetData(value)?.DisplayName
                );
            }

            configMenu.AddPage(
                mod: manifest,
                    pageId: "NeverToxic.YetAnotherFishingMod.Enchantments",
                    pageTitle: I18n.Config_Enchantments_PageTitle
                );
            configMenu.AddSectionTitle(
                mod: manifest,
                text: I18n.Config_Enchantments_GeneralSection
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Enchantments_DoAddEnchantments_Name,
                tooltip: I18n.Config_Enchantments_DoAddEnchantments_Tooltip,
                getValue: () => config().DoAddEnchantments,
                setValue: value => config().DoAddEnchantments = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Enchantments_ResetEnchantments_Name,
                tooltip: I18n.Config_Enchantments_ResetEnchantments_Tooltip,
                getValue: () => config().ResetEnchantmentsWhenNotEquipped,
                setValue: value => config().ResetEnchantmentsWhenNotEquipped = value
            );
            configMenu.AddSectionTitle(
                mod: manifest,
                text: I18n.Config_Enchantments_EnchantmentsSection
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Enchantments_AddAllEnchantments_Name,
                tooltip: I18n.Config_Enchantments_AddAllEnchantments_Tooltip,
                getValue: () => config().AddAllEnchantments,
                setValue: value => config().AddAllEnchantments = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Enchantments_AddAutoHookEnchantment_Name,
                tooltip: I18n.Config_Enchantments_AddAutoHookEnchantment_Tooltip,
                getValue: () => config().AddAutoHookEnchantment,
                setValue: value => config().AddAutoHookEnchantment = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Enchantments_AddEfficientToolEnchantment_Name,
                tooltip: I18n.Config_Enchantments_AddEfficientToolEnchantment_Tooltip,
                getValue: () => config().AddEfficientToolEnchantment,
                setValue: value => config().AddEfficientToolEnchantment = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Enchantments_AddMasterEnchantment_Name,
                tooltip: I18n.Config_Enchantments_AddMasterEnchantment_Tooltip,
                getValue: () => config().AddMasterEnchantment,
                setValue: value => config().AddMasterEnchantment = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Enchantments_AddPreservingEnchantment_Name,
                tooltip: I18n.Config_Enchantments_AddPreservingEnchantment_Tooltip,
                getValue: () => config().AddPreservingEnchantment,
                setValue: value => config().AddPreservingEnchantment = value
            );
        }
    }
}
