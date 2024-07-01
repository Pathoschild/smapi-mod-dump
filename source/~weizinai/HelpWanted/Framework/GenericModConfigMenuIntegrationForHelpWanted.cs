/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using weizinai.StardewValleyMod.Common.Integration;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace weizinai.StardewValleyMod.HelpWanted.Framework;

internal class GenericModConfigMenuIntegrationForHelpWanted
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;
    private readonly bool isRSVLoaded;

    public GenericModConfigMenuIntegrationForHelpWanted(IModHelper helper, IManifest manifest, Func<ModConfig> getConfig, Action reset, Action save)
    {
        this.configMenu = new GenericModConfigMenuIntegration<ModConfig>(helper.ModRegistry, manifest, getConfig, reset, save);
        this.isRSVLoaded = helper.ModRegistry.IsLoaded("Rafseazz.RidgesideVillage");
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (this.configMenu.GetConfig().OpenConfigMenuKeybind.JustPressed() && Context.IsPlayerFree) this.configMenu.OpenMenu();
    }

    public void Register()
    {
        if (!this.configMenu.IsLoaded) return;

        this.configMenu
            .Register()
            // 一般设置
            .AddSectionTitle(I18n.Config_GeneralSettingsTitle_Name)
            .AddKeybindList(
                config => config.OpenConfigMenuKeybind,
                (config, value) => config.OpenConfigMenuKeybind = value,
                I18n.Config_OpenConfigMenuKeybind_Name
            )
            .AddBoolOption(
                config => config.QuestFirstDay,
                (config, value) => config.QuestFirstDay = value,
                I18n.Config_QuestFirstDay_Name,
                I18n.Config_QuestFirstDay_Tooltip
            )
            .AddBoolOption(
                config => config.QuestFestival,
                (config, value) => config.QuestFestival = value,
                I18n.Config_QuestFestival_Name,
                I18n.Config_QuestFestival_Tooltip
            )
            .AddNumberOption(
                config => config.DailyQuestChance,
                (config, value) => config.DailyQuestChance = value,
                I18n.Config_DailyQuestChance_Name,
                null,
                0f,
                1f,
                0.05f
            )
            .AddBoolOption(
                config => config.OneQuestPerVillager,
                (config, value) => config.OneQuestPerVillager = value,
                I18n.Config_OneQuestPerVillager_Name
            )
            .AddBoolOption(
                config => config.ExcludeMaxHeartsNPC,
                (config, value) => config.ExcludeMaxHeartsNPC = value,
                I18n.Config_ExcludeMaxHeartsNPC_Name
            )
            .AddTextOption(
                config => string.Join(", ", config.ExcludeNPCList),
                (config, value) => config.ExcludeNPCList = value.Split(',').Select(s => s.Trim()).ToList(),
                I18n.Config_ExcludeNPCList_Name,
                I18n.Config_ExcludeNPCList_Tooltip
            )
            .AddNumberOption(
                config => config.QuestDays,
                (config, value) => config.QuestDays = value,
                I18n.Config_QuestDays_Name,
                I18n.Config_QuestDays_Tooltip,
                1,
                10
            )
            .AddNumberOption(
                config => config.MaxQuests,
                (config, value) => config.MaxQuests = value,
                I18n.Config_MaxQuests_Name,
                I18n.Config_MaxQuests_Tooltip
            )
            .AddBoolOption(
                config => config.EnableRSVQuestBoard,
                (config, value) => config.EnableRSVQuestBoard = value,
                I18n.Config_EnableRSVQuestBoard_Name,
                enable: this.isRSVLoaded
            )
            .AddNumberOption(
                config => config.MaxRSVQuests,
                (config, value) => config.MaxRSVQuests = value,
                I18n.Config_MaxRSVQuests_Name,
                enable: this.isRSVLoaded
            )
            .AddPageLink("ItemDeliveryQuest", I18n.Config_ItemDeliveryPage_Name)
            .AddPageLink("ResourceCollectionQuest", I18n.Config_ResourceCollectionPage_Name)
            .AddPageLink("FishingQuest", I18n.Config_FishingPage_Name)
            .AddPageLink("SlayMonstersQuest", I18n.Config_SlayMonstersPage_Name)
            .AddPageLink("Appearance", I18n.Config_AppearancePage_Name)

            // 交易任务
            .AddPage("ItemDeliveryQuest", I18n.Config_ItemDeliveryPage_Name)
            .AddNumberOption(
                config => config.ItemDeliveryWeight,
                (config, value) => config.ItemDeliveryWeight = value,
                I18n.Config_ItemDeliveryWeight_Name,
                I18n.Config_ItemDeliveryWeight_Tooltip
            )
            .AddNumberOption(
                config => config.ItemDeliveryRewardMultiplier,
                (config, value) => config.ItemDeliveryRewardMultiplier = value,
                I18n.Config_ItemDeliveryRewardMultiplier_Name,
                null,
                0.25f,
                5f,
                0.25f
            )
            .AddNumberOption(
                config => config.ItemDeliveryFriendshipGain,
                (config, value) => config.ItemDeliveryFriendshipGain = value,
                I18n.Config_ItemDeliveryFriendshipGain_Name,
                I18n.Config_ItemDeliveryFriendshipGain_Tooltip
            )
            .AddSectionTitle(I18n.Config_QuestItemSettingsTitle_Name)
            .AddNumberOption(
                config => config.QuestItemRequirement,
                (config, value) => config.QuestItemRequirement = value,
                I18n.Config_QuestItemRequirement_Name,
                I18n.Config_QuestItemRequirement_Tooltip,
                0,
                4
            )
            .AddNumberOption(
                config => config.MaxPrice,
                (config, value) => config.MaxPrice = value,
                I18n.Config_MaxPrice_Name,
                I18n.Config_MaxPrice_Tooltip
            )
            .AddBoolOption(
                config => config.AllowArtisanGoods,
                (config, value) => config.AllowArtisanGoods = value,
                I18n.Config_AllowArtisanGoods_Name,
                I18n.Config_AllowArtisanGoods_Tooltip
            )
            .AddBoolOption(
                config => config.UseModPossibleItems,
                (config, value) => config.UseModPossibleItems = value,
                I18n.Config_UseModPossibleItems_Name,
                I18n.Config_UseModPossibleItems_Tooltip
            )

            // 采集任务
            .AddPage("ResourceCollectionQuest", I18n.Config_ResourceCollectionPage_Name)
            .AddNumberOption(
                config => config.ResourceCollectionWeight,
                (config, value) => config.ResourceCollectionWeight = value,
                I18n.Config_ResourceCollectionWeight_Name,
                I18n.Config_ResourceCollectionWeight_Tooltip
            )
            .AddNumberOption(
                config => config.ResourceCollectionRewardMultiplier,
                (config, value) => config.ResourceCollectionRewardMultiplier = value,
                I18n.Config_ResourceCollectionRewardMultiplier_Name,
                null,
                0.25f,
                5f,
                0.25f
            )

            // 钓鱼任务
            .AddPage("FishingQuest", I18n.Config_FishingPage_Name)
            .AddNumberOption(
                config => config.FishingWeight,
                (config, value) => config.FishingWeight = value,
                I18n.Config_FishingWeight_Name,
                I18n.Config_FishingWeight_Tooltip
            )
            .AddNumberOption(
                config => config.FishingRewardMultiplier,
                (config, value) => config.FishingRewardMultiplier = value,
                I18n.Config_FishingRewardMultiplier_Name,
                null,
                0.25f,
                5f,
                0.25f
            )

            // 杀怪任务
            .AddPage("SlayMonstersQuest", I18n.Config_SlayMonstersPage_Name)
            .AddNumberOption(
                config => config.SlayMonstersWeight,
                (config, value) => config.SlayMonstersWeight = value,
                I18n.Config_SlayMonstersWeight_Name,
                I18n.Config_SlayMonstersWeight_Tooltip
            )
            .AddNumberOption(
                config => config.SlayMonstersRewardMultiplier,
                (config, value) => config.SlayMonstersRewardMultiplier = value,
                I18n.Config_SlayMonstersRewardMultiplier_Name,
                null,
                0.25f,
                5f,
                0.25f
            )
            .AddBoolOption(
                config => config.MoreSlayMonsterQuest,
                (config, value) => config.MoreSlayMonsterQuest = value,
                I18n.Config_MoreSlayMonsterQuests_Name
            )

            // 外观
            .AddPage("Appearance", I18n.Config_AppearancePage_Name)
            .AddSectionTitle(I18n.Config_NoteAppearanceTitle_Name)
            .AddNumberOption(
                config => config.NoteScale,
                (config, value) => config.NoteScale = value,
                I18n.Config_NoteScale_Name
            )
            .AddNumberOption(
                config => config.XOverlapBoundary,
                (config, value) => config.XOverlapBoundary = value,
                I18n.Config_XOverlapBoundary_Name,
                I18n.Config_XOverlapBoundary_Tooltip,
                0,
                1,
                0.05f
            )
            .AddNumberOption(
                config => config.YOverlapBoundary,
                (config, value) => config.YOverlapBoundary = value,
                I18n.Config_YOverlapBoundary_Name,
                I18n.Config_YOverlapBoundary_Tooltip,
                0,
                1,
                0.05f
            )
            .AddNumberOption(
                config => config.RandomColorMin,
                (config, value) => config.RandomColorMin = value,
                I18n.Config_RandomColorMin_Name,
                null,
                0,
                255
            )
            .AddNumberOption(
                config => config.RandomColorMax,
                (config, value) => config.RandomColorMax = value,
                I18n.Config_RandomColorMax_Name,
                null,
                0,
                255
            )
            .AddSectionTitle(I18n.Config_PortraitAppearanceTitle_Name)
            .AddNumberOption(
                config => config.PortraitScale,
                (config, value) => config.PortraitScale = value,
                I18n.Config_PortraitScale_Name
            )
            .AddNumberOption(
                config => config.PortraitOffsetX,
                (config, value) => config.PortraitOffsetX = value,
                I18n.Config_PortraitOffsetX_Name
            )
            .AddNumberOption(
                config => config.PortraitOffsetY,
                (config, value) => config.PortraitOffsetY = value,
                I18n.Config_PortraitOffsetY_Name
            )
            .AddNumberOption(
                config => config.PortraitTintR,
                (config, value) => config.PortraitTintR = value,
                I18n.Config_PortraitTintR_Name,
                null,
                0,
                255
            )
            .AddNumberOption(
                config => config.PortraitTintG,
                (config, value) => config.PortraitTintG = value,
                I18n.Config_PortraitTintG_Name,
                null,
                0,
                255
            )
            .AddNumberOption(
                config => config.PortraitTintB,
                (config, value) => config.PortraitTintB = value,
                I18n.Config_PortraitTintB_Name,
                null,
                0,
                255
            )
            .AddNumberOption(
                config => config.PortraitTintA,
                (config, value) => config.PortraitTintA = value,
                I18n.Config_PortraitTintA_Name,
                null,
                0,
                255
            );
    }
}