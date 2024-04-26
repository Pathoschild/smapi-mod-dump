/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Common.Integration;
using Common.Patch;
using HelpWanted.Framework;
using HelpWanted.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace HelpWanted;

internal partial class ModEntry : Mod
{
    public static IMonitor SMonitor = null!;
    private QuestManager questManager = null!;

    private static ModConfig config = new();

    public static readonly List<QuestData> QuestList = new();

    public override void Entry(IModHelper helper)
    {
        // 初始化
        config = helper.ReadConfig<ModConfig>();
        SMonitor = Monitor;
        questManager = new QuestManager(config, Monitor, new AppearanceManager(helper, config));
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        // 注册Harmony补丁
        HarmonyPatcher.Patch(this, new BillboardPatcher(config), new UtilityPatcher(), new ItemDeliveryQuestPatcher(config),
            new SlayMonsterQuestPatcher(config), new ResourceCollectionQuestPatcher(config), new FishingQuestPatcher(config));
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        if (!Context.IsMainPlayer) return;
        if (Game1.stats.DaysPlayed <= 1 && !config.QuestFirstDay) return;
        if ((Utility.isFestivalDay() || Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.season)) && !config.QuestFestival) return;
        if (Game1.random.NextDouble() >= config.DailyQuestChance) return;

        Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        questManager.InitQuestList(QuestList);
        Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
        if (configMenu is null) return;

        configMenu.Register(
            ModManifest,
            () => config = new ModConfig(),
            () => Helper.WriteConfig(config)
        );

        // 一般设置
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_GeneralSettingsTitle_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.QuestFirstDay,
            value => config.QuestFirstDay = value,
            I18n.Config_QuestFirstDay_Name,
            I18n.Config_QuestFirstDay_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.QuestFestival,
            value => config.QuestFestival = value,
            I18n.Config_QuestFestival_Name,
            I18n.Config_QuestFestival_Tooltip
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.DailyQuestChance,
            value => config.DailyQuestChance = value,
            I18n.Config_DailyQuestChance_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.OneQuestPerVillager,
            value => config.OneQuestPerVillager = value,
            I18n.Config_OneQuestPerVillager_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ExcludeMaxHeartsNPC,
            value => config.ExcludeMaxHeartsNPC = value,
            I18n.Config_ExcludeMaxHeartsNPC_Name
        );
        configMenu.AddTextOption(
            ModManifest,
            () => string.Join(", ", config.ExcludeNPCList),
            value => config.ExcludeNPCList = value.Split(',').Select(s => s.Trim()).ToList(),
            I18n.Config_ExcludeNPCList_Name,
            I18n.Config_ExcludeNPCList_Tooltip
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.QuestDays,
            value => config.QuestDays = value,
            I18n.Config_QuestDays_Name,
            I18n.Config_QuestDays_Tooltip,
            1,
            10
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.MaxQuests,
            value => config.MaxQuests = value,
            I18n.Config_MaxQuests_Name,
            I18n.Config_MaxQuests_Tooltip
        );
        // 交易任务
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_ItemDeliveryTitle_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.ItemDeliveryWeight,
            value => config.ItemDeliveryWeight = value,
            I18n.Config_ItemDeliveryWeight_Name,
            I18n.Config_ItemDeliveryWeight_Tooltip
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.ItemDeliveryRewardMultiplier,
            value => config.ItemDeliveryRewardMultiplier = value,
            I18n.Config_ItemDeliveryRewardMultiplier_Name,
            null,
            0.25f,
            5f,
            0.25f
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.MustLikeItem,
            value => config.MustLikeItem = value,
            I18n.Config_MustLikeItem_Name,
            I18n.Config_MustLikeItem_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.MustLoveItem,
            value => config.MustLoveItem = value,
            I18n.Config_MustLoveItem_Name,
            I18n.Config_MustLoveItem_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AllowArtisanGoods,
            value => config.AllowArtisanGoods = value,
            I18n.Config_AllowArtisanGoods_Name,
            I18n.Config_AllowArtisanGoods_Tooltip
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.MaxPrice,
            value => config.MaxPrice = value,
            I18n.Config_MaxPrice_Name,
            I18n.Config_MaxPrice_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.IgnoreVanillaItemRestriction,
            value => config.IgnoreVanillaItemRestriction = value,
            I18n.Config_IgnoreVanillaItemRestriction_Name,
            I18n.Config_IgnoreVanillaItemRestriction_Tooltip
        );
        // 采集任务
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_ResourceCollectionTitle_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.ResourceCollectionWeight,
            value => config.ResourceCollectionWeight = value,
            I18n.Config_ResourceCollectionWeight_Name,
            I18n.Config_ResourceCollectionWeight_Tooltip
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.ResourceCollectionRewardMultiplier,
            value => config.ResourceCollectionRewardMultiplier = value,
            I18n.Config_ResourceCollectionRewardMultiplier_Name,
            null,
            0.25f,
            5f,
            0.25f
        );
        // 钓鱼任务
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_FishingTitle_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.FishingWeight,
            value => config.FishingWeight = value,
            I18n.Config_FishingWeight_Name,
            I18n.Config_FishingWeight_Tooltip
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.FishingRewardMultiplier,
            value => config.FishingRewardMultiplier = value,
            I18n.Config_FishingRewardMultiplier_Name,
            null,
            0.25f,
            5f,
            0.25f
        );
        // 杀怪任务
        configMenu.AddPageLink(
            ModManifest,
            "SlayMonsters",
            I18n.Config_SlayMonstersPage_Name
        );
        // 外观
        configMenu.AddPageLink(
            ModManifest,
            "Appearance",
            I18n.Config_AppearancePage_Name
        );

        #region 杀怪任务

        configMenu.AddPage(
            ModManifest,
            "SlayMonsters",
            I18n.Config_SlayMonstersPage_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.SlayMonstersWeight,
            value => config.SlayMonstersWeight = value,
            I18n.Config_SlayMonstersWeight_Name,
            I18n.Config_SlayMonstersWeight_Tooltip
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.SlayMonstersRewardMultiplier,
            value => config.SlayMonstersRewardMultiplier = value,
            I18n.Config_SlayMonstersRewardMultiplier_Name,
            null,
            0.25f,
            5f,
            0.25f
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.MoreSlayMonsterQuest,
            value => config.MoreSlayMonsterQuest = value,
            I18n.Config_MoreSlayMonsterQuests_Name
        );

        #endregion

        #region 外观

        configMenu.AddPage(
            ModManifest,
            "Appearance",
            I18n.Config_AppearancePage_Name
        );
        // 便签外观标题
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_NoteAppearanceTitle_Name
        );
        // 便签缩放
        configMenu.AddNumberOption(
            ModManifest,
            () => config.NoteScale,
            value => config.NoteScale = value,
            I18n.Config_NoteScale_Name,
            I18n.Config_NoteScale_Tooltip
        );
        // 便签重叠率
        configMenu.AddNumberOption(
            ModManifest,
            () => config.XOverlapBoundary,
            value => config.XOverlapBoundary = value,
            I18n.Config_XOverlapBoundary_Name,
            I18n.Config_XOverlapBoundary_Tooltip,
            0,
            1,
            0.05f
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.YOverlapBoundary,
            value => config.YOverlapBoundary = value,
            I18n.Config_YOverlapBoundary_Name,
            I18n.Config_YOverlapBoundary_Tooltip,
            0,
            1,
            0.05f
        );
        // 随机颜色
        configMenu.AddNumberOption(
            ModManifest,
            () => config.RandomColorMin,
            value => config.RandomColorMin = value,
            I18n.Config_RandomColorMin_Name,
            null,
            0,
            255
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.RandomColorMax,
            value => config.RandomColorMax = value,
            I18n.Config_RandomColorMax_Name,
            null,
            0,
            255
        );
        // 肖像外观标题
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_PortraitAppearanceTitle_Name
        );
        // 肖像缩放
        configMenu.AddNumberOption(
            ModManifest,
            () => config.PortraitScale,
            value => config.PortraitScale = value,
            I18n.Config_PortraitScale_Name
        );
        // 肖像偏移
        configMenu.AddNumberOption(
            ModManifest,
            () => config.PortraitOffsetX,
            value => config.PortraitOffsetX = value,
            I18n.Config_PortraitOffsetX_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.PortraitOffsetY,
            value => config.PortraitOffsetY = value,
            I18n.Config_PortraitOffsetY_Name
        );
        // 肖像颜色
        configMenu.AddNumberOption(
            ModManifest,
            () => config.PortraitTintR,
            value => config.PortraitTintR = value,
            I18n.Config_PortraitTintR_Name,
            null,
            0,
            255
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.PortraitTintG,
            value => config.PortraitTintG = value,
            I18n.Config_PortraitTintG_Name,
            null,
            0,
            255
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.PortraitTintB,
            value => config.PortraitTintB = value,
            I18n.Config_PortraitTintB_Name,
            null,
            0,
            255
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.PortraitTintA,
            value => config.PortraitTintA = value,
            I18n.Config_PortraitTintA_Name,
            null,
            0,
            255
        );

        #endregion
    }
}