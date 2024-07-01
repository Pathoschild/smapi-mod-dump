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
using weizinai.StardewValleyMod.LazyMod.Automation;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Framework.Integration;

internal class GenericModConfigMenuIntegrationForLazyMod
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    private readonly string[] buffMaintainAllowValues =
        { "Combat", "Farming", "Fishing", "Mining", "Luck", "Foraging", "MaxStamina", "MagneticRadius", "Speed", "Defense", "Attack", "None" };

    public GenericModConfigMenuIntegrationForLazyMod(IModHelper helper, IManifest consumerManifest, Func<ModConfig> getConfig, Action reset, Action save)
    {
        this.configMenu = new GenericModConfigMenuIntegration<ModConfig>(helper.ModRegistry, consumerManifest, getConfig, reset, save);
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
            .AddKeybindList(
                config => config.OpenConfigMenuKeybind,
                (config, value) => config.OpenConfigMenuKeybind = value,
                I18n.Config_OpenConfigMenuKeybind_Name
            )
            .AddKeybindList(
                config => config.ToggleModStateKeybind,
                (config, value) => config.ToggleModStateKeybind = value,
                I18n.Config_ToggleModStateKeybind_Name,
                I18n.Config_ToggleModStateKeybind_Tooltip
            )
            .AddNumberOption(
                config => config.Cooldown,
                (config, value) => config.Cooldown = value,
                I18n.Config_Cooldown_Name,
                I18n.Config_Cooldown_Tooltip,
                0,
                60,
                5
            )
            .AddPageLink("Farming", I18n.Config_FarmingPage_Name)
            .AddPageLink("Animal", I18n.Config_AnimalPage_Name)
            .AddPageLink("Mining", I18n.Config_MiningPage_Name)
            .AddPageLink("Foraging", I18n.Config_ForagingPage_Name)
            .AddPageLink("Fishing", I18n.Config_FishingPage_Name)
            .AddPageLink("Food", I18n.Config_FoodPage_Name)
            .AddPageLink("Other", I18n.Config_OtherPage_Name);

        this.AddFarmingPage();
        this.AddAnimalPage();
        this.AddMiningPage();
        this.AddForagingPage();
        this.AddFishingPage();
        this.AddFoodPage();
        this.AddOtherPage();
        this.AddTreeSettingsPage();
    }

    private void AddFarmingPage()
    {
        this.configMenu
            .AddPage("Farming", I18n.Config_FarmingPage_Name)
            // 自动耕地
            .AddStaminaToolAutomationConfig(config => config.AutoTillDirt, I18n.Config_AutoTillDirt_Name, I18n.Config_AutoTillDirt_Tooltip, 0)
            // 自动清理耕地
            .AddStaminaToolAutomationConfig(config => config.AutoClearTilledDirt, I18n.Config_AutoClearTilledDirt_Name, I18n.Config_AutoClearTilledDirt_Tooltip, 0)
            // 自动浇水
            .AddStaminaToolAutomationConfig(config => config.AutoWaterDirt, I18n.Config_AutoWaterDirt_Name, I18n.Config_AutoWaterDirt_Tooltip, 0)
            // 自动补充水壶
            .AddToolAutomationConfig(config => config.AutoRefillWateringCan, I18n.Config_AutoRefillWateringCan_Name, I18n.Config_AutoRefillWateringCan_Tooltip, 1)
            // 自动播种
            .AddBaseAutomationConfig(config => config.AutoSeed, I18n.Config_AutoSeed_Name, I18n.Config_AutoSeed_Tooltip, 0)
            // 自动施肥
            .AddBaseAutomationConfig(config => config.AutoFertilize, I18n.Config_AutoFertilize_Name, I18n.Config_AutoFertilize_Tooltip, 0)
            // 自动收获作物
            .AddBaseAutomationConfig(config => config.AutoHarvestCrop, I18n.Config_AutoHarvestCrop_Name, null, 0)
            .AddBoolOption(
                config => config.AutoHarvestFlower,
                (config, value) => config.AutoHarvestFlower = value,
                I18n.Config_AutoHarvestFlower_Name
            )
            // 自动摇晃果树
            .AddBaseAutomationConfig(config => config.AutoShakeFruitTree, I18n.Config_AutoShakeFruitTree_Name, null, 1)
            // 自动清理枯萎作物
            .AddToolAutomationConfig(config => config.AutoClearDeadCrop, I18n.Config_AutoClearDeadCrop_Name, I18n.Config_AutoClearDeadCrop_Tooltip, 0);
    }

    private void AddAnimalPage()
    {
        this.configMenu
            .AddPage("Animal", I18n.Config_AnimalPage_Name)
            // 自动抚摸动物
            .AddBaseAutomationConfig(config => config.AutoPetAnimal, I18n.Config_AutoPetAnimal_Name, null, 1)
            // 自动抚摸宠物
            .AddBaseAutomationConfig(config => config.AutoPetPet, I18n.Config_AutoPetPet_Name, null, 1)
            // 自动挤奶
            .AddStaminaToolAutomationConfig(config => config.AutoMilkAnimal, I18n.Config_AutoMilkAnimal_Name, I18n.Config_AutoMilkAnimal_Tooltip, 1)
            // 自动剪毛
            .AddStaminaToolAutomationConfig(config => config.AutoShearsAnimal, I18n.Config_AutoShearsAnimal_Name, I18n.Config_AutoShearsAnimal_Tooltip, 1)
            // 自动喂食动物饼干
            .AddBaseAutomationConfig(config => config.AutoFeedAnimalCracker, I18n.Config_AutoFeedAnimalCracker_Name, I18n.Config_AutoFeedAnimalCracker_Tooltip, 1)
            // 自动打开动物门
            .AddSectionTitle(I18n.Config_AutoOpenAnimalDoor_Name)
            .AddBoolOption(
                config => config.AutoOpenAnimalDoor,
                (config, value) => config.AutoOpenAnimalDoor = value,
                I18n.Config_AutoOpenAnimalDoor_Name,
                I18n.Config_AutoOpenAnimalDoor_Tooltip
            )
            // 自动打开栅栏门
            .AddBaseAutomationConfig(config => config.AutoOpenFenceGate, I18n.Config_AutoOpenFenceGate_Name, null, 1);
    }

    private void AddMiningPage()
    {
        this.configMenu
            .AddPage("Mining", I18n.Config_MiningPage_Name)
            // 自动清理石头
            .AddStaminaToolAutomationConfig(config => config.AutoClearStone, I18n.Config_AutoClearStone_Name, I18n.Config_AutoClearStone_Tooltip, 1)
            .AddBoolOption(
                config => config.ClearStoneOnMineShaft,
                (config, value) => config.ClearStoneOnMineShaft = value,
                I18n.Config_ClearStoneOnMineShaft_Name,
                I18n.Config_ClearStoneOnMineShaft_Tooltip
            )
            .AddBoolOption(
                config => config.ClearStoneOnVolcano,
                (config, value) => config.ClearStoneOnVolcano = value,
                I18n.Config_ClearStoneOnVolcano_Name,
                I18n.Config_ClearStoneOnVolcano_Tooltip
            )
            .AddBoolOption(
                config => config.ClearFarmStone,
                (config, value) => config.ClearFarmStone = value,
                I18n.Config_ClearFarmStone_Name,
                I18n.Config_ClearFarmStone_Tooltip
            )
            .AddBoolOption(
                config => config.ClearOtherStone,
                (config, value) => config.ClearOtherStone = value,
                I18n.Config_ClearOtherStone_Name,
                I18n.Config_ClearOtherStone_Tooltip
            )
            .AddBoolOption(
                config => config.ClearIslandStone,
                (config, value) => config.ClearIslandStone = value,
                I18n.Config_ClearIslandStone_Name,
                I18n.Config_ClearIslandStone_Tooltip
            )
            .AddBoolOption(
                config => config.ClearOreStone,
                (config, value) => config.ClearOreStone = value,
                I18n.Config_ClearOreStone_Name
            )
            .AddBoolOption(
                config => config.ClearGemStone,
                (config, value) => config.ClearGemStone = value,
                I18n.Config_ClearGemStone_Name
            )
            .AddBoolOption(
                config => config.ClearGeodeStone,
                (config, value) => config.ClearGeodeStone = value,
                I18n.Config_ClearGeodeStone_Name
            )
            .AddBoolOption(
                config => config.ClearCalicoEggStone,
                (config, value) => config.ClearCalicoEggStone = value,
                I18n.Config_ClearCalicoEggStone_Name
            )
            .AddBoolOption(
                config => config.ClearBoulder,
                (config, value) => config.ClearBoulder = value,
                I18n.Config_ClearBoulder_Name
            )
            .AddBoolOption(
                config => config.ClearMeteorite,
                (config, value) => config.ClearMeteorite = value,
                I18n.Config_ClearMeteorite_Name
            )
            // 自动收集煤炭
            .AddBaseAutomationConfig(config => config.AutoCollectCoal, I18n.Config_AutoCollectCoal_Name, I18n.Config_AutoCollectCoal_Tooltip, 1)
            // 自动破坏容器
            .AddToolAutomationConfig(config => config.AutoBreakContainer, I18n.Config_AutoBreakContainer_Name, I18n.Config_AutoBreakContainer_Tooltip, 1)
            // 自动打开宝箱
            .AddBaseAutomationConfig(config => config.AutoOpenTreasure, I18n.Config_AutoOpenTreasure_Name, null, 1)
            // 自动清理水晶
            .AddToolAutomationConfig(config => config.AutoClearCrystal, I18n.Config_AutoClearCrystal_Name, null, 1)
            // 自动冷却岩浆
            .AddStaminaToolAutomationConfig(config => config.AutoCoolLava, I18n.Config_AutoCoolLava_Name, I18n.Config_AutoCoolLava_Tooltip, 1)
            // 显示矿井信息
            .AddSectionTitle(I18n.Config_ShowMineShaftInfo_Name)
            .AddBoolOption(
                config => config.ShowLadderInfo,
                (config, value) => config.ShowLadderInfo = value,
                I18n.Config_ShowLadderInfo_Name
            )
            .AddBoolOption(
                config => config.ShowShaftInfo,
                (config, value) => config.ShowShaftInfo = value,
                I18n.Config_ShowShaftInfo_Name
            )
            .AddBoolOption(
                config => config.ShowMonsterInfo,
                (config, value) => config.ShowMonsterInfo = value,
                I18n.Config_ShowMonsterInfo_Name
            )
            .AddBoolOption(
                config => config.ShowMonsterKillInfo,
                (config, value) => config.ShowMonsterKillInfo = value,
                I18n.Config_ShowMonsterKillInfo_Name,
                I18n.Config_ShowMonsterKillInfo_Tooltip
            )
            .AddBoolOption(
                config => config.ShowMineralInfo,
                (config, value) => config.ShowMineralInfo = value,
                I18n.Config_ShowMineralInfo_Name
            );
    }

    private void AddForagingPage()
    {
        this.configMenu
            .AddPage("Foraging", I18n.Config_ForagingPage_Name)
            // 自动觅食
            .AddBaseAutomationConfig(config => config.AutoForage, I18n.Config_AutoForage_Name, null, 1)
            // 自动砍树
            .AddStaminaToolAutomationConfig(config => config.AutoChopTree, I18n.Config_AutoChopTree_Name, I18n.Config_AutoChopTree_Tooltip, 1)
            .AddBoolOption(
                config => config.ChopTapperTree,
                (config, value) => config.ChopTapperTree = value,
                I18n.Config_ChopTapperTree_Name
            )
            .AddBoolOption(
                config => config.ChopVinegarTree,
                (config, value) => config.ChopVinegarTree = value,
                I18n.Config_ChopVinegarTree_Name
            )
            .AddPageLink("TreeSettings", I18n.Config_TreeSettingsPage_Name)
            // 自动收获姜
            .AddStaminaToolAutomationConfig(config => config.AutoHarvestGinger, I18n.Config_AutoHarvestGinger_Name, I18n.Config_AutoHarvestGinger_Tooltip, 0)
            // 自动摇树
            .AddBaseAutomationConfig(config => config.AutoShakeTree, I18n.Config_AutoShakeTree_Name, null, 1)
            // 自动收获苔藓
            .AddToolAutomationConfig(config => config.AutoHarvestMoss, I18n.Config_AutoHarvestMoss_Name, I18n.Config_AutoHarvestMoss_Tooltip, 1)
            // 自动放置采集器
            .AddBaseAutomationConfig(config => config.AutoPlaceTapper, I18n.Config_AutoPlaceTapper_Name, I18n.Config_AutoPlaceTapper_Tooltip, 1)
            // 自动在树上浇醋
            .AddBaseAutomationConfig(config => config.AutoPlaceVinegar, I18n.Config_AutoPlaceVinegar_Name, I18n.Config_AutoPlaceVinegar_Tooltip, 1)
            // 自动清理木头
            .AddStaminaToolAutomationConfig(config => config.AutoClearWood, I18n.Config_AutoPlaceVinegar_Name, I18n.Config_AutoPlaceVinegar_Tooltip, 1)
            .AddBoolOption(
                config => config.ClearTwig,
                (config, value) => config.ClearTwig = value,
                I18n.Config_ClearTwig_Name
            )
            .AddBoolOption(
                config => config.ClearStump,
                (config, value) => config.ClearStump = value,
                I18n.Config_ClearStump_Name
            )
            .AddBoolOption(
                config => config.ClearHollowLog,
                (config, value) => config.ClearHollowLog = value,
                I18n.Config_ClearHollowLog_Name
            );
    }

    private void AddFishingPage()
    {
        this.configMenu
            .AddPage("Fishing", I18n.Config_FishingPage_Name)
            // 自动抓取宝箱物品
            .AddBoolOption(
                config => config.AutoGrabTreasureItem,
                (config, value) => config.AutoGrabTreasureItem = value,
                I18n.Config_AutoGrabTreasureItem_Name
            )
            // 自动退出宝箱菜单
            .AddBoolOption(
                config => config.AutoExitTreasureMenu,
                (config, value) => config.AutoExitTreasureMenu = value,
                I18n.Config_AutoExitTreasureMenu_Name
            )
            // 自动使用蟹笼
            .AddBaseAutomationConfig(config => config.AutoPlaceCarbPot, I18n.Config_AutoPlaceCarbPot_Name, I18n.Config_AutoPlaceCarbPot_Tooltip, 1)
            // 自动添加蟹笼鱼饵
            .AddBaseAutomationConfig(config => config.AutoAddBaitForCarbPot, I18n.Config_AutoAddBaitForCarbPot_Name, I18n.Config_AutoAddBaitForCarbPot_Tooltip, 1)
            // 自动收获蟹笼
            .AddBaseAutomationConfig(config => config.AutoHarvestCarbPot, I18n.Config_AutoHarvestCarbPot_Name, null, 1);
    }

    private void AddFoodPage()
    {
        this.configMenu
            .AddPage("Food", I18n.Config_FoodPage_Name)
            // 自动吃食物-体力
            .AddSectionTitle(I18n.Config_AutoEatFoodForStamina_Name)
            .AddBoolOption(
                config => config.AutoEatFoodForStamina,
                (config, value) => config.AutoEatFoodForStamina = value,
                I18n.Config_AutoEatFoodForStamina_Name
            )
            .AddNumberOption(
                config => config.AutoEatFoodStaminaRate,
                (config, value) => config.AutoEatFoodStaminaRate = value,
                I18n.Config_AutoEatFoodStaminaRate_Name,
                null,
                0.05f,
                0.95f,
                0.05f
            )
            .AddBoolOption(
                config => config.IntelligentFoodSelectionForStamina,
                (config, value) => config.IntelligentFoodSelectionForStamina = value,
                I18n.Config_IntelligentFoodSelectionForStamina_Name,
                I18n.Config_IntelligentFoodSelectionForStamina_Tooltip
            )
            .AddNumberOption(
                config => config.RedundantStaminaFoodCount,
                (config, value) => config.RedundantStaminaFoodCount = value,
                I18n.Config_RedundantFoodCount_Name,
                I18n.Config_RedundantFoodCount_Tooltip,
                0,
                50,
                5
            )
            // 自动吃食物-生命值
            .AddSectionTitle(I18n.Config_AutoEatFoodForHealth_Name)
            .AddBoolOption(
                config => config.AutoEatFoodForHealth,
                (config, value) => config.AutoEatFoodForHealth = value,
                I18n.Config_AutoEatFoodForHealth_Name
            )
            .AddNumberOption(
                config => config.AutoEatFoodHealthRate,
                (config, value) => config.AutoEatFoodHealthRate = value,
                I18n.Config_AutoEatFoodHealthRate_Name,
                null,
                0.05f,
                0.95f,
                0.05f
            )
            .AddBoolOption(
                config => config.IntelligentFoodSelectionForHealth,
                (config, value) => config.IntelligentFoodSelectionForHealth = value,
                I18n.Config_IntelligentFoodSelectionForHealth_Name,
                I18n.Config_IntelligentFoodSelectionForHealth_Tooltip
            )
            .AddNumberOption(
                config => config.RedundantHealthFoodCount,
                (config, value) => config.RedundantHealthFoodCount = value,
                I18n.Config_RedundantFoodCount_Name,
                I18n.Config_RedundantFoodCount_Tooltip,
                0,
                50,
                5
            )
            // 自动吃增益食物
            .AddSectionTitle(I18n.Config_AutoEatBuffFood_Name)
            .AddBoolOption(
                config => config.AutoEatBuffFood,
                (config, value) => config.AutoEatBuffFood = value,
                I18n.Config_AutoEatBuffFood_Name,
                I18n.Config_AutoEatBuffFood_Tooltip
            )
            .AddTextOption(
                config => config.FoodBuffMaintain1.ToString(),
                (config, value) => config.FoodBuffMaintain1 = AutoFood.GetBuffType(value),
                I18n.Config_FoodBuffMaintain1_Name,
                I18n.Config_FoodBuffMaintain_Tooltip, this.buffMaintainAllowValues,
                GetStringFromBuffType
            )
            .AddTextOption(
                config => config.FoodBuffMaintain2.ToString(),
                (config, value) => config.FoodBuffMaintain2 = AutoFood.GetBuffType(value),
                I18n.Config_FoodBuffMaintain2_Name,
                I18n.Config_FoodBuffMaintain_Tooltip, this.buffMaintainAllowValues,
                GetStringFromBuffType
            )
            // 自动喝增益饮料
            .AddSectionTitle(I18n.Config_AutoDrinkBuffDrink_Name)
            .AddBoolOption(
                config => config.AutoDrinkBuffDrink,
                (config, value) => config.AutoDrinkBuffDrink = value,
                I18n.Config_AutoDrinkBuffDrink_Name,
                I18n.Config_AutoDrinkBuffDrink_Tooltip
            )
            .AddTextOption(
                config => config.DrinkBuffMaintain1.ToString(),
                (config, value) => config.DrinkBuffMaintain1 = AutoFood.GetBuffType(value),
                I18n.Config_DrinkBuffMaintain1_Name,
                I18n.Config_DrinkBuffMaintain_ToolTip, this.buffMaintainAllowValues,
                GetStringFromBuffType
            )
            .AddTextOption(
                config => config.DrinkBuffMaintain2.ToString(),
                (config, value) => config.DrinkBuffMaintain2 = AutoFood.GetBuffType(value),
                I18n.Config_DrinkBuffMaintain2_Name,
                I18n.Config_DrinkBuffMaintain_ToolTip, this.buffMaintainAllowValues,
                GetStringFromBuffType
            );
    }

    private void AddOtherPage()
    {
        this.configMenu
            .AddPage("Other", I18n.Config_OtherPage_Name)
            // 磁力范围增加
            .AddSectionTitle(I18n.Config_MagneticRadiusIncrease_Name)
            .AddNumberOption(
                config => config.MagneticRadiusIncrease,
                (config, value) => config.MagneticRadiusIncrease = value,
                I18n.Config_MagneticRadiusIncrease_Name,
                null,
                0,
                10
            )
            // 自动清理杂草
            .AddToolAutomationConfig(config => config.AutoClearWeeds, I18n.Config_AutoClearWeeds_Name, I18n.Config_AutoClearWeeds_Tooltip, 1)
            .AddBoolOption(
                config => config.ClearLargeWeeds,
                (config, value) => config.ClearLargeWeeds = value,
                I18n.Config_ClearLargeWeeds_Name
            )
            // 自动挖掘斑点
            .AddStaminaToolAutomationConfig(config => config.AutoDigSpots, I18n.Config_AutoDigSpots_Name, I18n.Config_AutoDigSpots_Tooltip, 0)
            // 自动收获机器
            .AddBaseAutomationConfig(config => config.AutoHarvestMachine, I18n.Config_AutoHarvestMachine_Name, null, 1)
            // 自动触发机器
            .AddBaseAutomationConfig(config => config.AutoTriggerMachine, I18n.Config_AutoTriggerMachine_Name, null, 1)
            // 自动使用仙尘
            .AddBaseAutomationConfig(config => config.AutoUseFairyDust, I18n.Config_AutoUseFairyDust_Name, I18n.Config_AutoUseFairyDust_Tooltip, 1)
            // 自动翻垃圾桶
            .AddBaseAutomationConfig(config => config.AutoGarbageCan, I18n.Config_AutoGarbageCan_Name, null, 1)
            .AddBoolOption(
                config => config.StopGarbageCanNearVillager,
                (config, value) => config.StopGarbageCanNearVillager = value,
                I18n.Config_StopGarbageCanNearVillager_Name
            )
            // 自动放置地板
            .AddBaseAutomationConfig(config => config.AutoPlaceFloor, I18n.Config_AutoPlaceFloor_Name, I18n.Config_AutoPlaceFloor_Tooltip, 0);
    }

    private void AddTreeSettingsPage()
    {
        this.configMenu
            .AddPage("TreeSettings", I18n.Config_TreeSettingsPage_Name)
            // 橡树
            .AddSectionTitle(I18n.Config_OakTreeTitle_Name)
            .AddBoolOption(
                config => config.ChopOakTree[0],
                (config, value) => config.ChopOakTree[0] = value,
                I18n.Config_ChopSeedStageOakTree_Name
            )
            .AddBoolOption(
                config => config.ChopOakTree[1],
                (config, value) => config.ChopOakTree[1] = value,
                I18n.Config_ChopSproutStageOakTree_Name
            )
            .AddBoolOption(
                config => config.ChopOakTree[2],
                (config, value) => config.ChopOakTree[2] = value,
                I18n.Config_ChopSaplingStageOakTree_Name
            )
            .AddBoolOption(
                config => config.ChopOakTree[3],
                (config, value) => config.ChopOakTree[3] = value,
                I18n.Config_ChopBushStageOakTree_Name
            )
            .AddBoolOption(
                config => config.ChopOakTree[4],
                (config, value) => config.ChopOakTree[4] = value,
                I18n.Config_ChopSmallTreeStageOakTree_Name
            )
            .AddBoolOption(
                config => config.ChopOakTree[5],
                (config, value) => config.ChopOakTree[5] = value,
                I18n.Config_ChopOakTree_Name
            )
            .AddBoolOption(
                config => config.ChopOakTree[-1],
                (config, value) => config.ChopOakTree[-1] = value,
                I18n.Config_ChopStumpStageOakTree_Name
            )
            // 枫树
            .AddSectionTitle(I18n.Config_MapleTreeTitle_Name)
            .AddBoolOption(
                config => config.ChopMapleTree[0],
                (config, value) => config.ChopMapleTree[0] = value,
                I18n.Config_ChopSeedStageMapleTree_Name
            )
            .AddBoolOption(
                config => config.ChopMapleTree[1],
                (config, value) => config.ChopMapleTree[1] = value,
                I18n.Config_ChopSproutStageMapleTree_Name
            )
            .AddBoolOption(
                config => config.ChopMapleTree[2],
                (config, value) => config.ChopMapleTree[2] = value,
                I18n.Config_ChopSaplingStageMapleTree_Name
            )
            .AddBoolOption(
                config => config.ChopMapleTree[3],
                (config, value) => config.ChopMapleTree[3] = value,
                I18n.Config_ChopBushStageMapleTree_Name
            )
            .AddBoolOption(
                config => config.ChopMapleTree[4],
                (config, value) => config.ChopMapleTree[4] = value,
                I18n.Config_ChopSmallTreeStageMapleTree_Name
            )
            .AddBoolOption(
                config => config.ChopMapleTree[5],
                (config, value) => config.ChopMapleTree[5] = value,
                I18n.Config_ChopMapleTree_Name
            )
            .AddBoolOption(
                config => config.ChopMapleTree[-1],
                (config, value) => config.ChopMapleTree[-1] = value,
                I18n.Config_ChopStumpStageMapleTree_Name
            )
            // 松树
            .AddSectionTitle(I18n.Config_PineTreeTitle_Name)
            .AddBoolOption(
                config => config.ChopPineTree[0],
                (config, value) => config.ChopPineTree[0] = value,
                I18n.Config_ChopSeedStagePineTree_Name
            )
            .AddBoolOption(
                config => config.ChopPineTree[1],
                (config, value) => config.ChopPineTree[1] = value,
                I18n.Config_ChopSproutStagePineTree_Name
            )
            .AddBoolOption(
                config => config.ChopPineTree[2],
                (config, value) => config.ChopPineTree[2] = value,
                I18n.Config_ChopSaplingStagePineTree_Name
            )
            .AddBoolOption(
                config => config.ChopPineTree[3],
                (config, value) => config.ChopPineTree[3] = value,
                I18n.Config_ChopBushStagePineTree_Name
            )
            .AddBoolOption(
                config => config.ChopPineTree[4],
                (config, value) => config.ChopPineTree[4] = value,
                I18n.Config_ChopSmallTreeStagePineTree_Name
            )
            .AddBoolOption(
                config => config.ChopPineTree[5],
                (config, value) => config.ChopPineTree[5] = value,
                I18n.Config_ChopPineTree_Name
            )
            .AddBoolOption(
                config => config.ChopPineTree[-1],
                (config, value) => config.ChopPineTree[-1] = value,
                I18n.Config_ChopStumpStagePineTree_Name
            )
            // 桃花心木树
            .AddSectionTitle(I18n.Config_MahoganyTreeTitle_Name)
            .AddBoolOption(
                config => config.ChopMahoganyTree[0],
                (config, value) => config.ChopMahoganyTree[0] = value,
                I18n.Config_ChopSeedStageMahoganyTree_Name
            )
            .AddBoolOption(
                config => config.ChopMahoganyTree[1],
                (config, value) => config.ChopMahoganyTree[1] = value,
                I18n.Config_ChopSproutStageMahoganyTree_Name
            )
            .AddBoolOption(
                config => config.ChopMahoganyTree[2],
                (config, value) => config.ChopMahoganyTree[2] = value,
                I18n.Config_ChopSaplingStageMahoganyTree_Name
            )
            .AddBoolOption(
                config => config.ChopMahoganyTree[3],
                (config, value) => config.ChopMahoganyTree[3] = value,
                I18n.Config_ChopBushStageMahoganyTree_Name
            )
            .AddBoolOption(
                config => config.ChopMahoganyTree[4],
                (config, value) => config.ChopMahoganyTree[4] = value,
                I18n.Config_ChopSmallTreeStageMahoganyTree_Name
            )
            .AddBoolOption(
                config => config.ChopMahoganyTree[5],
                (config, value) => config.ChopMahoganyTree[5] = value,
                I18n.Config_ChopMahoganyTree_Name
            )
            .AddBoolOption(
                config => config.ChopMahoganyTree[-1],
                (config, value) => config.ChopMahoganyTree[-1] = value,
                I18n.Config_ChopStumpStageMahoganyTree_Name
            )
            // 棕榈树
            .AddSectionTitle(I18n.Config_PalmTreeTitle_Name)
            .AddBoolOption(
                config => config.ChopPalmTree[2],
                (config, value) => config.ChopPalmTree[2] = value,
                I18n.Config_ChopSaplingStagePalmTree_Name
            )
            .AddBoolOption(
                config => config.ChopPalmTree[3],
                (config, value) => config.ChopPalmTree[3] = value,
                I18n.Config_ChopBushStagePalmTree_Name
            )
            .AddBoolOption(
                config => config.ChopPalmTree[4],
                (config, value) => config.ChopPalmTree[4] = value,
                I18n.Config_ChopSmallTreeStagePalmTree_Name
            )
            .AddBoolOption(
                config => config.ChopPalmTree[5],
                (config, value) => config.ChopPalmTree[5] = value,
                I18n.Config_ChopPalmTree_Name
            )
            .AddBoolOption(
                config => config.ChopPalmTree[-1],
                (config, value) => config.ChopPalmTree[-1] = value,
                I18n.Config_ChopStumpStagePalmTree_Name
            )
            // 蘑菇树
            .AddSectionTitle(I18n.Config_MushroomTreeTitle_Name)
            .AddBoolOption(
                config => config.ChopMushroomTree[0],
                (config, value) => config.ChopMushroomTree[0] = value,
                I18n.Config_ChopSeedStageMushroomTree_Name
            )
            .AddBoolOption(
                config => config.ChopMushroomTree[1],
                (config, value) => config.ChopMushroomTree[1] = value,
                I18n.Config_ChopSproutStageMushroomTree_Name
            )
            .AddBoolOption(
                config => config.ChopMushroomTree[2],
                (config, value) => config.ChopMushroomTree[2] = value,
                I18n.Config_ChopSaplingStageMushroomTree_Name
            )
            .AddBoolOption(
                config => config.ChopMushroomTree[3],
                (config, value) => config.ChopMushroomTree[3] = value,
                I18n.Config_ChopBushStageMushroomTree_Name
            )
            .AddBoolOption(
                config => config.ChopMushroomTree[4],
                (config, value) => config.ChopMushroomTree[4] = value,
                I18n.Config_ChopSmallTreeStageMushroomTree_Name
            )
            .AddBoolOption(
                config => config.ChopMushroomTree[5],
                (config, value) => config.ChopMushroomTree[5] = value,
                I18n.Config_ChopMushroomTree_Name
            )
            .AddBoolOption(
                config => config.ChopMushroomTree[-1],
                (config, value) => config.ChopMushroomTree[-1] = value,
                I18n.Config_ChopStumpStageMushroomTree_Name
            )
            // 苔雨树
            .AddSectionTitle(I18n.Config_GreenRainTreeTitle_Name)
            .AddBoolOption(
                config => config.ChopGreenRainTree[0],
                (config, value) => config.ChopGreenRainTree[0] = value,
                I18n.Config_ChopSeedStageGreenRainTree_Name
            )
            .AddBoolOption(
                config => config.ChopGreenRainTree[1],
                (config, value) => config.ChopGreenRainTree[1] = value,
                I18n.Config_ChopSproutStageGreenRainTree_Name
            )
            .AddBoolOption(
                config => config.ChopGreenRainTree[2],
                (config, value) => config.ChopGreenRainTree[2] = value,
                I18n.Config_ChopSaplingStageGreenRainTree_Name
            )
            .AddBoolOption(
                config => config.ChopGreenRainTree[3],
                (config, value) => config.ChopGreenRainTree[3] = value,
                I18n.Config_ChopBushStageGreenRainTree_Name
            )
            .AddBoolOption(
                config => config.ChopGreenRainTree[4],
                (config, value) => config.ChopGreenRainTree[4] = value,
                I18n.Config_ChopSmallTreeStageGreenRainTree_Name
            )
            .AddBoolOption(
                config => config.ChopGreenRainTree[5],
                (config, value) => config.ChopGreenRainTree[5] = value,
                I18n.Config_ChopGreenRainTree_Name
            )
            .AddBoolOption(
                config => config.ChopGreenRainTree[-1],
                (config, value) => config.ChopGreenRainTree[-1] = value,
                I18n.Config_ChopStumpStageGreenRainTree_Name
            )
            // 神秘树
            .AddSectionTitle(I18n.Config_MysticTreeTitle_Name)
            .AddBoolOption(
                config => config.ChopMysticTree[0],
                (config, value) => config.ChopMysticTree[0] = value,
                I18n.Config_ChopSeedStageMysticTree_Name
            )
            .AddBoolOption(
                config => config.ChopMysticTree[1],
                (config, value) => config.ChopMysticTree[1] = value,
                I18n.Config_ChopSproutStageMysticTree_Name
            )
            .AddBoolOption(
                config => config.ChopMysticTree[2],
                (config, value) => config.ChopMysticTree[2] = value,
                I18n.Config_ChopSaplingStageMysticTree_Name
            )
            .AddBoolOption(
                config => config.ChopMysticTree[3],
                (config, value) => config.ChopMysticTree[3] = value,
                I18n.Config_ChopBushStageMysticTree_Name
            )
            .AddBoolOption(
                config => config.ChopMysticTree[4],
                (config, value) => config.ChopMysticTree[4] = value,
                I18n.Config_ChopSmallTreeStageMysticTree_Name
            )
            .AddBoolOption(
                config => config.ChopMysticTree[5],
                (config, value) => config.ChopMysticTree[5] = value,
                I18n.Config_ChopMysticTree_Name
            )
            .AddBoolOption(
                config => config.ChopMysticTree[-1],
                (config, value) => config.ChopMysticTree[-1] = value,
                I18n.Config_ChopStumpStageMysticTree_Name
            );
    }

    private static string GetStringFromBuffType(string value)
    {
        return value switch
        {
            "Combat" => I18n.BuffType_Combat_Name(),
            "Farming" => I18n.BuffType_Farming_Name(),
            "Fishing" => I18n.BuffType_Fishing_Name(),
            "Mining" => I18n.BuffType_Mining_Name(),
            "Luck" => I18n.BuffType_Luck_Name(),
            "Foraging" => I18n.BuffType_Foraging_Name(),
            "MaxStamina" => I18n.BuffType_MaxStamina_Name(),
            "MagneticRadius" => I18n.BuffType_MagneticRadius_Name(),
            "Speed" => I18n.BuffType_Speed_Name(),
            "Defense" => I18n.BuffType_Defense_Name(),
            "Attack" => I18n.BuffType_Attack_Name(),
            _ => I18n.BuffType_None_Name()
        };
    }
}