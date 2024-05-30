/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Common.Integrations;
using LazyMod.Framework.Automation;
using LazyMod.Framework.Config;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace LazyMod.Framework;

internal class GenericModConfigMenuIntegrationForLazyMod
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    private readonly string[] buffMaintainAllowValues =
        { "Combat", "Farming", "Fishing", "Mining", "Luck", "Foraging", "MaxStamina", "MagneticRadius", "Speed", "Defense", "Attack", "None" };

    public GenericModConfigMenuIntegrationForLazyMod(IModHelper helper, IManifest consumerManifest, Func<ModConfig> getConfig, Action reset, Action save)
    {
        configMenu = new GenericModConfigMenuIntegration<ModConfig>(helper.ModRegistry, consumerManifest, getConfig, reset, save);
        helper.Events.Input.ButtonsChanged += OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (configMenu.GetConfig().OpenConfigMenuKeybind.JustPressed() && Context.IsPlayerFree)
            configMenu.OpenMenu();
            
    }

    public void Register()
    {
        if (!configMenu.IsLoaded) return;

        configMenu
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
            .AddPageLink("Other", I18n.Config_OtherPage_Name)
            // 耕种
            .AddPage("Farming", I18n.Config_FarmingPage_Name)
            // 自动耕地
            .AddSectionTitle(I18n.Config_AutoTillDirt_Name, I18n.Config_AutoTillDirt_Tooltip)
            .AddBoolOption(
                config => config.AutoTillDirt.IsEnable,
                (config, value) => config.AutoTillDirt.IsEnable = value,
                I18n.Config_AutoTillDirt_Name
            )
            .AddNumberOption(
                config => config.AutoTillDirt.Range,
                (config, value) => config.AutoTillDirt.Range = value,
                I18n.Config_AutoTillDirtRange_Name,
                null,
                0,
                3
            )
            .AddNumberOption(
                config => config.StopTillDirtStamina,
                (config, value) => config.StopTillDirtStamina = value,
                I18n.Config_StopTillDirtStamina_Name
            )
            // 自动清理耕地
            .AddSectionTitle(I18n.Config_AutoClearTilledDirt_Name, I18n.Config_AutoClearTilledDirt_Tooltip)
            .AddBoolOption(
                config => config.AutoClearTilledDirt.IsEnable,
                (config, value) => config.AutoClearTilledDirt.IsEnable = value,
                I18n.Config_AutoClearTilledDirt_Name
            )
            .AddNumberOption(
                config => config.AutoClearTilledDirt.Range,
                (config, value) => config.AutoClearTilledDirt.Range = value,
                I18n.Config_AutoClearTilledDirtRange_Name,
                null,
                0,
                3
            )
            .AddNumberOption(
                config => config.StopClearTilledDirtStamina,
                (config, value) => config.StopClearTilledDirtStamina = value,
                I18n.Config_StopClearTilledDirtStamina_Name
            )
            // 自动浇水
            .AddSectionTitle(I18n.Config_AutoWaterDirt_Name, I18n.Config_AutoWaterDirt_Tooltip)
            .AddBoolOption(
                config => config.AutoWaterDirt.IsEnable,
                (config, value) => config.AutoWaterDirt.IsEnable = value,
                I18n.Config_AutoWaterDirt_Name
            )
            .AddNumberOption(
                config => config.AutoWaterDirt.Range,
                (config, value) => config.AutoWaterDirt.Range = value,
                I18n.Config_AutoWaterDirtRange_Name,
                null,
                0,
                3
            )
            .AddNumberOption(
                config => config.StopWaterDirtStamina,
                (config, value) => config.StopWaterDirtStamina = value,
                I18n.Config_StopWaterDirtStamina_Name
            )
            // 自动补充水壶
            .AddSectionTitle(I18n.Config_AutoRefillWateringCan_Name, I18n.Config_AutoRefillWateringCan_Tooltip)
            .AddBoolOption(
                config => config.AutoRefillWateringCan.IsEnable,
                (config, value) => config.AutoRefillWateringCan.IsEnable = value,
                I18n.Config_AutoRefillWateringCan_Name
            )
            .AddNumberOption(
                config => config.AutoRefillWateringCan.Range,
                (config, value) => config.AutoRefillWateringCan.Range = value,
                I18n.Config_AutoRefillWateringCanRange_Name,
                null,
                1,
                3
            )
            .AddBoolOption(
                config => config.FindWateringCanFromInventory,
                (config, value) => config.FindWateringCanFromInventory = value,
                I18n.Config_FindWateringCanFromInventory_Name,
                I18n.Config_FindWateringCanFromInventory_Tooltip
            )
            // 自动播种
            .AddSectionTitle(I18n.Config_AutoSeed_Name, I18n.Config_AutoSeed_Tooltip)
            .AddBoolOption(
                config => config.AutoSeed.IsEnable,
                (config, value) => config.AutoSeed.IsEnable = value,
                I18n.Config_AutoSeed_Name
            )
            .AddNumberOption(
                config => config.AutoSeed.Range,
                (config, value) => config.AutoSeed.Range = value,
                I18n.Config_AutoSeedRange_Name,
                null,
                0,
                3
            )
            // 自动施肥
            .AddSectionTitle(I18n.Config_AutoFertilize_Name, I18n.Config_AutoFertilize_Tooltip)
            .AddBoolOption(
                config => config.AutoFertilize.IsEnable,
                (config, value) => config.AutoFertilize.IsEnable = value,
                I18n.Config_AutoFertilize_Name
            )
            .AddNumberOption(
                config => config.AutoFertilize.Range,
                (config, value) => config.AutoFertilize.Range = value,
                I18n.Config_AutoFertilizeRange_Name,
                null,
                0,
                3
            )
            // 自动收获作物
            .AddSectionTitle(I18n.Config_AutoHarvestCrop_Name)
            .AddBoolOption(
                config => config.AutoHarvestCrop.IsEnable,
                (config, value) => config.AutoHarvestCrop.IsEnable = value,
                I18n.Config_AutoHarvestCrop_Name
            )
            .AddNumberOption(
                config => config.AutoHarvestCrop.Range,
                (config, value) => config.AutoHarvestCrop.Range = value,
                I18n.Config_AutoHarvestCropRange_Name,
                null,
                0,
                3
            )
            .AddBoolOption(
                config => config.AutoHarvestFlower,
                (config, value) => config.AutoHarvestFlower = value,
                I18n.Config_AutoHarvestFlower_Name
            )
            // 自动摇晃果树
            .AddSectionTitle(I18n.Config_AutoShakeFruitTree_Name)
            .AddBoolOption(
                config => config.AutoShakeFruitTree.IsEnable,
                (config, value) => config.AutoShakeFruitTree.IsEnable = value,
                I18n.Config_AutoShakeFruitTree_Name
            )
            .AddNumberOption(
                config => config.AutoShakeFruitTree.Range,
                (config, value) => config.AutoShakeFruitTree.Range = value,
                I18n.Config_AutoShakeFruitTreeRange_Name,
                null,
                1,
                3
            )
            // 自动清理枯萎作物
            .AddSectionTitle(I18n.Config_AutoClearDeadCrop_Name, I18n.Config_AutoClearDeadCrop_Tooltip)
            .AddBoolOption(
                config => config.AutoClearDeadCrop.IsEnable,
                (config, value) => config.AutoClearDeadCrop.IsEnable = value,
                I18n.Config_AutoClearDeadCrop_Name
            )
            .AddNumberOption(
                config => config.AutoClearDeadCrop.Range,
                (config, value) => config.AutoClearDeadCrop.Range = value,
                I18n.Config_AutoClearDeadCropRange_Name,
                null,
                0,
                3
            )
            .AddBoolOption(
                config => config.FindToolForClearDeadCrop,
                (config, value) => config.FindToolForClearDeadCrop = value,
                I18n.Config_FindToolForClearDeadCrop_Name,
                I18n.Config_FindToolForClearDeadCrop_Tooltip
            )
            // 动物
            .AddPage("Animal", I18n.Config_AnimalPage_Name)
            // 自动抚摸动物
            .AddSectionTitle(I18n.Config_AutoPetAnimal_Name)
            .AddBoolOption(
                config => config.AutoPetAnimal.IsEnable,
                (config, value) => config.AutoPetAnimal.IsEnable = value,
                I18n.Config_AutoPetAnimal_Name
            )
            .AddNumberOption(
                config => config.AutoPetAnimal.Range,
                (config, value) => config.AutoPetAnimal.Range = value,
                I18n.Config_AutoPetAnimalRange_Name,
                null,
                1,
                3
            )
            // 自动抚摸宠物
            .AddSectionTitle(I18n.Config_AutoPetPet_Name)
            .AddBoolOption(
                config => config.AutoPetPet.IsEnable,
                (config, value) => config.AutoPetPet.IsEnable = value,
                I18n.Config_AutoPetPet_Name
            )
            .AddNumberOption(
                config => config.AutoPetPet.Range,
                (config, value) => config.AutoPetPet.Range = value,
                I18n.Config_AutoPetPetRange_Name,
                null,
                1,
                3
            )
            // 自动挤奶
            .AddSectionTitle(I18n.Config_AutoMilkAnimal_Name, I18n.Config_AutoMilkAnimal_Tooltip)
            .AddBoolOption(
                config => config.AutoMilkAnimal.IsEnable,
                (config, value) => config.AutoMilkAnimal.IsEnable = value,
                I18n.Config_AutoMilkAnimal_Name
            )
            .AddNumberOption(
                config => config.AutoMilkAnimal.Range,
                (config, value) => config.AutoMilkAnimal.Range = value,
                I18n.Config_AutoMilkAnimalRange_Name,
                null,
                1,
                3
            )
            .AddNumberOption(
                config => config.StopMilkAnimalStamina,
                (config, value) => config.StopMilkAnimalStamina = value,
                I18n.Config_StopMilkAnimalStamina_Name
            )
            .AddBoolOption(
                config => config.FindMilkPailFromInventory,
                (config, value) => config.FindMilkPailFromInventory = value,
                I18n.Config_FindMilkPailFromInventory_Name,
                I18n.Config_FindMilkPailFromInventory_Tooltip
            )
            // 自动剪毛
            .AddSectionTitle(I18n.Config_AutoShearsAnimal_Name, I18n.Config_AutoShearsAnimal_Tooltip)
            .AddBoolOption(
                config => config.AutoShearsAnimal.IsEnable,
                (config, value) => config.AutoShearsAnimal.IsEnable = value,
                I18n.Config_AutoShearsAnimal_Name
            )
            .AddNumberOption(
                config => config.AutoShearsAnimal.Range,
                (config, value) => config.AutoShearsAnimal.Range = value,
                I18n.Config_AutoShearsAnimalRange_Name,
                null,
                1,
                3
            )
            .AddNumberOption(
                config => config.StopShearsAnimalStamina,
                (config, value) => config.StopShearsAnimalStamina = value,
                I18n.Config_StopShearsAnimalStamina_Name
            )
            .AddBoolOption(
                config => config.FindShearsFromInventory,
                (config, value) => config.FindShearsFromInventory = value,
                I18n.Config_FindShearsFromInventory_Name,
                I18n.Config_FindShearsFromInventory_Tooltip
            )
            // 自动喂食动物饼干
            .AddSectionTitle(I18n.Config_AutoFeedAnimalCracker_Name, I18n.Config_AutoFeedAnimalCracker_Tooltip)
            .AddBoolOption(
                config => config.AutoFeedAnimalCracker.IsEnable,
                (config, value) => config.AutoFeedAnimalCracker.IsEnable = value,
                I18n.Config_AutoFeedAnimalCracker_Name
            )
            .AddNumberOption(
                config => config.AutoFeedAnimalCracker.Range,
                (config, value) => config.AutoFeedAnimalCracker.Range = value,
                I18n.Config_AutoFeedAnimalCrackerRange_Name,
                null,
                1,
                3
            )
            // 自动打开动物门
            .AddSectionTitle(I18n.Config_AutoOpenAnimalDoor_Name)
            .AddBoolOption(
                config => config.AutoOpenAnimalDoor,
                (config, value) => config.AutoOpenAnimalDoor = value,
                I18n.Config_AutoOpenAnimalDoor_Name,
                I18n.Config_AutoOpenAnimalDoor_Tooltip
            )
            // 自动打开栅栏门
            .AddSectionTitle(I18n.Config_AutoOpenFenceGate_Name)
            .AddBoolOption(
                config => config.AutoOpenFenceGate.IsEnable,
                (config, value) => config.AutoOpenFenceGate.IsEnable = value,
                I18n.Config_AutoOpenFenceGate_Name
            )
            .AddNumberOption(
                config => config.AutoOpenFenceGate.Range,
                (config, value) => config.AutoOpenFenceGate.Range = value,
                I18n.Config_AutoOpenFenceGateRange_Name,
                null,
                1,
                3
            )
            // 采矿
            .AddPage("Mining", I18n.Config_MiningPage_Name)
            // 自动清理石头
            .AddSectionTitle(I18n.Config_AutoClearStone_Name, I18n.Config_AutoClearStone_Tooltip)
            .AddBoolOption(
                config => config.AutoClearStone.IsEnable,
                (config, value) => config.AutoClearStone.IsEnable = value,
                I18n.Config_AutoClearStone_Name
            )
            .AddNumberOption(
                config => config.AutoClearStone.Range,
                (config, value) => config.AutoClearStone.Range = value,
                I18n.Config_AutoClearStoneRange_Name,
                null,
                1,
                3
            )
            .AddNumberOption(
                config => config.StopClearStoneStamina,
                (config, value) => config.StopClearStoneStamina = value,
                I18n.Config_StopClearStoneStamina_Name
            )
            .AddBoolOption(
                config => config.FindPickaxeFromInventory,
                (config, value) => config.FindPickaxeFromInventory = value,
                I18n.Config_FindPickaxeFromInventory_Name,
                I18n.Config_FindPickaxeFromInventory_Tooltip
            )
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
            .AddSectionTitle(I18n.Config_AutoCollectCoal_Name, I18n.Config_AutoCollectCoal_Tooltip)
            .AddBoolOption(
                config => config.AutoCollectCoal.IsEnable,
                (config, value) => config.AutoCollectCoal.IsEnable = value,
                I18n.Config_AutoCollectCoal_Name
            )
            .AddNumberOption(
                config => config.AutoCollectCoal.Range,
                (config, value) => config.AutoCollectCoal.Range = value,
                I18n.Config_AutoCollectCoalRange_Name,
                null,
                1,
                3
            )
            // 自动破坏容器
            .AddSectionTitle(I18n.Config_AutoBreakContainer_Name, I18n.Config_AutoBreakContainer_Tooltip)
            .AddBoolOption(
                config => config.AutoBreakContainer.IsEnable,
                (config, value) => config.AutoBreakContainer.IsEnable = value,
                I18n.Config_AutoBreakContainer_Name
            )
            .AddNumberOption(
                config => config.AutoBreakContainer.Range,
                (config, value) => config.AutoBreakContainer.Range = value,
                I18n.Config_AutoBreakContainerRange_Name,
                null,
                1,
                3
            )
            .AddBoolOption(
                config => config.FindToolForBreakContainer,
                (config, value) => config.FindToolForBreakContainer = value,
                I18n.Config_FindToolForBreakContainer_Name,
                I18n.Config_FindToolForBreakContainer_Tooltip
            )
            // 自动打开宝箱
            .AddSectionTitle(I18n.Config_AutoOpenTreasure_Name)
            .AddBoolOption(
                config => config.AutoOpenTreasure.IsEnable,
                (config, value) => config.AutoOpenTreasure.IsEnable = value,
                I18n.Config_AutoOpenTreasure_Name
            )
            .AddNumberOption(
                config => config.AutoOpenTreasure.Range,
                (config, value) => config.AutoOpenTreasure.Range = value,
                I18n.Config_AutoOpenTreasureRange_Name,
                null,
                1,
                3
            )
            // 自动清理水晶
            .AddSectionTitle(I18n.Config_AutoClearCrystal_Name)
            .AddBoolOption(
                config => config.AutoClearCrystal.IsEnable,
                (config, value) => config.AutoClearCrystal.IsEnable = value,
                I18n.Config_AutoClearCrystal_Name
            )
            .AddNumberOption(
                config => config.AutoClearCrystal.Range,
                (config, value) => config.AutoClearCrystal.Range = value,
                I18n.Config_AutoClearCrystalRange_Name,
                null,
                1,
                3
            )
            .AddBoolOption(
                config => config.FindToolForClearCrystal,
                (config, value) => config.FindToolForClearCrystal = value,
                I18n.Config_FindToolForClearCrystal_Name,
                I18n.Config_FindToolForClearCrystal_Tooltip
            )
            // 自动冷却岩浆
            .AddSectionTitle(I18n.Config_AutoCoolLava_Name, I18n.Config_AutoCoolLava_Tooltip)
            .AddBoolOption(
                config => config.AutoCoolLava.IsEnable,
                (config, value) => config.AutoCoolLava.IsEnable = value,
                I18n.Config_AutoCoolLava_Name
            )
            .AddNumberOption(
                config => config.AutoCoolLava.Range,
                (config, value) => config.AutoCoolLava.Range = value,
                I18n.Config_AutoCoolLavaRange_Name,
                null,
                1,
                3
            )
            .AddNumberOption(
                config => config.StopCoolLavaStamina,
                (config, value) => config.StopCoolLavaStamina = value,
                I18n.Config_StopCoolLavaStamina_Name
            )
            .AddBoolOption(
                config => config.FindToolForCoolLava,
                (config, value) => config.FindToolForCoolLava = value,
                I18n.Config_FindToolForCoolLava_Name,
                I18n.Config_FindToolForCoolLava_Tooltip
            )
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
            )
            // 采集
            .AddPage("Foraging", I18n.Config_ForagingPage_Name)
            // 自动觅食
            .AddSectionTitle(I18n.Config_AutoForage_Name)
            .AddBoolOption(
                config => config.AutoForage.IsEnable,
                (config, value) => config.AutoForage.IsEnable = value,
                I18n.Config_AutoForage_Name
            )
            .AddNumberOption(
                config => config.AutoForage.Range,
                (config, value) => config.AutoForage.Range = value,
                I18n.Config_AutoForageRange_Name,
                null,
                0,
                3
            )
            // 自动砍树
            .AddSectionTitle(I18n.Config_AutoChopTree_Name, I18n.Config_AutoChopTree_Tooltip)
            .AddBoolOption(
                config => config.AutoChopTree.IsEnable,
                (config, value) => config.AutoChopTree.IsEnable = value,
                I18n.Config_AutoChopTree_Name
            )
            .AddNumberOption(
                config => config.AutoChopTree.Range,
                (config, value) => config.AutoChopTree.Range = value,
                I18n.Config_AutoChopTreeRange_Name,
                null,
                1,
                3
            )
            .AddNumberOption(
                config => config.StopChopTreeStamina,
                (config, value) => config.StopChopTreeStamina = value,
                I18n.Config_StopChopTreeStamina_Name
            )
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
            .AddSectionTitle(I18n.Config_AutoHarvestGinger_Name, I18n.Config_AutoHarvestGinger_Tooltip)
            .AddBoolOption(
                config => config.AutoHarvestGinger.IsEnable,
                (config, value) => config.AutoHarvestGinger.IsEnable = value,
                I18n.Config_AutoHarvestGinger_Name
            )
            .AddNumberOption(
                config => config.AutoHarvestGinger.Range,
                (config, value) => config.AutoHarvestGinger.Range = value,
                I18n.Config_AutoHarvestGingerRange_Name,
                null,
                0,
                3
            )
            .AddNumberOption(
                config => config.StopHarvestGingerStamina,
                (config, value) => config.StopHarvestGingerStamina = value,
                I18n.Config_StopHarvestGingerStamina_Name
            )
            .AddBoolOption(
                config => config.FindToolForHarvestGinger,
                (config, value) => config.FindToolForHarvestGinger = value,
                I18n.Config_FindToolForHarvestGinger_Name,
                I18n.Config_FindToolForHarvestGinger_Tooltip
            )
            // 自动摇树
            .AddSectionTitle(I18n.Config_AutoShakeTree_Name)
            .AddBoolOption(
                config => config.AutoShakeTree.IsEnable,
                (config, value) => config.AutoShakeTree.IsEnable = value,
                I18n.Config_AutoShakeTree_Name
            )
            .AddNumberOption(
                config => config.AutoShakeTree.Range,
                (config, value) => config.AutoShakeTree.Range = value,
                I18n.Config_AutoShakeTreeRange_Name,
                null,
                0,
                3
            )
            // 自动收获苔藓
            .AddSectionTitle(I18n.Config_AutoHarvestMoss_Name, I18n.Config_AutoHarvestMoss_Tooltip)
            .AddBoolOption(
                config => config.AutoHarvestMoss.IsEnable,
                (config, value) => config.AutoHarvestMoss.IsEnable = value,
                I18n.Config_AutoHarvestMoss_Name
            )
            .AddNumberOption(
                config => config.AutoHarvestMoss.Range,
                (config, value) => config.AutoHarvestMoss.Range = value,
                I18n.Config_AutoHarvestMossRange_Name,
                null,
                1,
                3
            )
            .AddBoolOption(
                config => config.FindScytheFromInventory,
                (config, value) => config.FindScytheFromInventory = value,
                I18n.Config_FindScytheFromInventory_Name,
                I18n.Config_FindScytheFromInventory_Tooltip
            )
            // 自动放置采集器
            .AddSectionTitle(I18n.Config_AutoPlaceTapper_Name, I18n.Config_AutoPlaceTapper_Tooltip)
            .AddBoolOption(
                config => config.AutoPlaceTapper.IsEnable,
                (config, value) => config.AutoPlaceTapper.IsEnable = value,
                I18n.Config_AutoPlaceTapper_Name
            )
            .AddNumberOption(
                config => config.AutoPlaceTapper.Range,
                (config, value) => config.AutoPlaceTapper.Range = value,
                I18n.Config_AutoPlaceTapperRange_Name,
                null,
                1,
                3
            )
            // 自动在树上浇醋
            .AddSectionTitle(I18n.Config_AutoPlaceVinegar_Name, I18n.Config_AutoPlaceVinegar_Tooltip)
            .AddBoolOption(
                config => config.AutoPlaceVinegar.IsEnable,
                (config, value) => config.AutoPlaceVinegar.IsEnable = value,
                I18n.Config_AutoPlaceVinegar_Name
            )
            .AddNumberOption(
                config => config.AutoPlaceVinegar.Range,
                (config, value) => config.AutoPlaceVinegar.Range = value,
                I18n.Config_AutoPlaceVinegarRange_Name,
                null,
                1,
                3
            )
            // 自动清理木头
            .AddSectionTitle(I18n.Config_AutoClearWood_Name, I18n.Config_AutoClearWood_Tooltip)
            .AddBoolOption(
                config => config.AutoClearWood.IsEnable,
                (config, value) => config.AutoClearWood.IsEnable = value,
                I18n.Config_AutoClearWood_Name
            )
            .AddNumberOption(
                config => config.AutoClearWood.Range,
                (config, value) => config.AutoClearWood.Range = value,
                I18n.Config_AutoClearWoodRange_Name,
                null,
                1,
                3
            )
            .AddNumberOption(
                config => config.StopClearWoodStamina,
                (config, value) => config.StopClearWoodStamina = value,
                I18n.Config_StopClearWoodStamina_Name
            )
            .AddBoolOption(
                config => config.FindAxeFromInventory,
                (config, value) => config.FindAxeFromInventory = value,
                I18n.Config_FindAxeFromInventory_Name,
                I18n.Config_FindAxeFromInventory_Tooltip
            )
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
            )
            // 钓鱼
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
            .AddSectionTitle(I18n.Config_AutoPlaceCarbPot_Name, I18n.Config_AutoPlaceCarbPot_Tooltip)
            .AddBoolOption(
                config => config.AutoPlaceCarbPot.IsEnable,
                (config, value) => config.AutoPlaceCarbPot.IsEnable = value,
                I18n.Config_AutoPlaceCarbPot_Name
            )
            .AddNumberOption(
                config => config.AutoPlaceCarbPot.Range,
                (config, value) => config.AutoPlaceCarbPot.Range = value,
                I18n.Config_AutoPlaceCarbPotRange_Name,
                null,
                1,
                3
            )
            // 自动添加蟹笼鱼饵
            .AddSectionTitle(I18n.Config_AutoAddBaitForCarbPot_Name, I18n.Config_AutoAddBaitForCarbPot_Tooltip)
            .AddBoolOption(
                config => config.AutoAddBaitForCarbPot.IsEnable,
                (config, value) => config.AutoAddBaitForCarbPot.IsEnable = value,
                I18n.Config_AutoAddBaitForCarbPot_Name
            )
            .AddNumberOption(
                config => config.AutoAddBaitForCarbPot.Range,
                (config, value) => config.AutoAddBaitForCarbPot.Range = value,
                I18n.Config_AutoAddBaitForCarbPotRange_Name,
                null,
                1,
                3
            )
            // 自动收获蟹笼
            .AddSectionTitle(I18n.Config_AutoHarvestCarbPot_Name)
            .AddBoolOption(
                config => config.AutoHarvestCarbPot.IsEnable,
                (config, value) => config.AutoHarvestCarbPot.IsEnable = value,
                I18n.Config_AutoHarvestCarbPot_Name
            )
            .AddNumberOption(
                config => config.AutoHarvestCarbPot.Range,
                (config, value) => config.AutoHarvestCarbPot.Range = value,
                I18n.Config_AutoHarvestCarbPotRange_Name,
                null,
                1,
                3
            )
            // 食物
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
                I18n.Config_FoodBuffMaintain_Tooltip,
                buffMaintainAllowValues,
                GetStringFromBuffType
            )
            .AddTextOption(
                config => config.FoodBuffMaintain2.ToString(),
                (config, value) => config.FoodBuffMaintain2 = AutoFood.GetBuffType(value),
                I18n.Config_FoodBuffMaintain2_Name,
                I18n.Config_FoodBuffMaintain_Tooltip,
                buffMaintainAllowValues,
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
                I18n.Config_DrinkBuffMaintain_ToolTip,
                buffMaintainAllowValues,
                GetStringFromBuffType
            )
            .AddTextOption(
                config => config.DrinkBuffMaintain2.ToString(),
                (config, value) => config.DrinkBuffMaintain2 = AutoFood.GetBuffType(value),
                I18n.Config_DrinkBuffMaintain2_Name,
                I18n.Config_DrinkBuffMaintain_ToolTip,
                buffMaintainAllowValues,
                GetStringFromBuffType
            )
            // 其他
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
            .AddSectionTitle(I18n.Config_AutoClearWeeds_Name, I18n.Config_AutoClearWeeds_Tooltip)
            .AddBoolOption(
                config => config.AutoClearWeeds.IsEnable,
                (config, value) => config.AutoClearWeeds.IsEnable = value,
                I18n.Config_AutoClearWeeds_Name
            )
            .AddNumberOption(
                config => config.AutoClearWeeds.Range,
                (config, value) => config.AutoClearWeeds.Range = value,
                I18n.Config_AutoClearWeedsRange_Name,
                null,
                1,
                3
            )
            .AddBoolOption(
                config => config.FindToolForClearWeeds,
                (config, value) => config.FindToolForClearWeeds = value,
                I18n.Config_FindToolForClearWeeds_Name,
                I18n.Config_FindToolForClearWeeds_Tooltip
            )
            .AddBoolOption(
                config => config.ClearLargeWeeds,
                (config, value) => config.ClearLargeWeeds = value,
                I18n.Config_ClearLargeWeeds_Name
            )
            // 自动挖掘斑点
            .AddSectionTitle(I18n.Config_AutoDigSpots_Name, I18n.Config_AutoDigSpots_Tooltip)
            .AddBoolOption(
                config => config.AutoDigSpots.IsEnable,
                (config, value) => config.AutoDigSpots.IsEnable = value,
                I18n.Config_AutoDigSpots_Name
            )
            .AddNumberOption(
                config => config.AutoDigSpots.Range,
                (config, value) => config.AutoDigSpots.Range = value,
                I18n.Config_AutoDigSpotsRange_Name,
                null,
                0,
                3
            )
            .AddNumberOption(
                config => config.StopDigSpotsStamina,
                (config, value) => config.StopDigSpotsStamina = value,
                I18n.Config_StopDigSpotsStamina_Name
            )
            .AddBoolOption(
                config => config.FindHoeFromInventory,
                (config, value) => config.FindHoeFromInventory = value,
                I18n.Config_FindHoeFromInventory_Name,
                I18n.Config_FindHoeFromInventory_Tooltip
            )
            // 自动收获机器
            .AddSectionTitle(I18n.Config_AutoHarvestMachine_Name)
            .AddBoolOption(
                config => config.AutoHarvestMachine.IsEnable,
                (config, value) => config.AutoHarvestMachine.IsEnable = value,
                I18n.Config_AutoHarvestMachine_Name
            )
            .AddNumberOption(
                config => config.AutoHarvestMachine.Range,
                (config, value) => config.AutoHarvestMachine.Range = value,
                I18n.Config_AutoHarvestMachineRange_Name,
                null,
                1,
                3
            )
            // 自动触发机器
            .AddSectionTitle(I18n.Config_AutoTriggerMachine_Name)
            .AddBoolOption(
                config => config.AutoTriggerMachine.IsEnable,
                (config, value) => config.AutoTriggerMachine.IsEnable = value,
                I18n.Config_AutoTriggerMachine_Name
            )
            .AddNumberOption(
                config => config.AutoTriggerMachine.Range,
                (config, value) => config.AutoTriggerMachine.Range = value,
                I18n.Config_AutoTriggerMachineRange_Name,
                null,
                1,
                3
            )
            // 自动翻垃圾桶
            .AddSectionTitle(I18n.Config_AutoGarbageCan_Name)
            .AddBoolOption(
                config => config.AutoGarbageCan.IsEnable,
                (config, value) => config.AutoGarbageCan.IsEnable = value,
                I18n.Config_AutoGarbageCan_Name
            )
            .AddNumberOption(
                config => config.AutoGarbageCan.Range,
                (config, value) => config.AutoGarbageCan.Range = value,
                I18n.Config_AutoGarbageCanRange_Name,
                null,
                1,
                3
            )
            .AddBoolOption(
                config => config.StopGarbageCanNearVillager,
                (config, value) => config.StopGarbageCanNearVillager = value,
                I18n.Config_StopGarbageCanNearVillager_Name
            )
            // 自动放置地板
            .AddSectionTitle(I18n.Config_AutoPlaceFloor_Name, I18n.Config_AutoPlaceFloor_Tooltip)
            .AddBoolOption(
                config => config.AutoPlaceFloor.IsEnable,
                (config, value) => config.AutoPlaceFloor.IsEnable = value,
                I18n.Config_AutoPlaceFloor_Name
            )
            .AddNumberOption(
                config => config.AutoPlaceFloor.Range,
                (config, value) => config.AutoPlaceFloor.Range = value,
                I18n.Config_AutoPlaceFloorRange_Name,
                null,
                0,
                3
            )
            // 树设置
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