/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using LazyMod.Framework.Automation;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace LazyMod.Framework.Config;

internal class ModConfig
{
    public KeybindList OpenConfigMenuKeybind { get; set; } = new(SButton.R);
    public KeybindList ToggleModStateKeybind { get; set; } = new(SButton.G);
    public int Cooldown { get; set; } = 10;

    #region 耕种

    // 自动耕地
    public BaseConfig AutoTillDirt { get; set; } = new();
    public float StopTillDirtStamina { get; set; } = 3;

    // 自动清理耕地
    public BaseConfig AutoClearTilledDirt { get; set; } = new();
    public float StopClearTilledDirtStamina { get; set; } = 3;

    // 自动浇水
    public BaseConfig AutoWaterDirt { get; set; } = new();
    public float StopWaterDirtStamina { get; set; } = 3;

    // 自动补充水壶
    public BaseConfig AutoRefillWateringCan { get; set; } = new(1);
    public bool FindWateringCanFromInventory { get; set; } = true;

    // 自动播种
    public BaseConfig AutoSeed { get; set; } = new();

    // 自动施肥
    public BaseConfig AutoFertilize { get; set; } = new();

    // 自动收获作物
    public BaseConfig AutoHarvestCrop { get; set; } = new();
    public bool AutoHarvestFlower { get; set; }

    // 自动摇晃果树
    public BaseConfig AutoShakeFruitTree { get; set; } = new(1);

    // 自动清理枯萎作物
    public BaseConfig AutoClearDeadCrop { get; set; } = new();
    public bool FindToolForClearDeadCrop { get; set; } = true;

    #endregion

    #region 动物

    // 自动抚摸动物
    public BaseConfig AutoPetAnimal { get; set; } = new(1);

    // 自动抚摸宠物
    public BaseConfig AutoPetPet { get; set; } = new(1);

    // 自动挤奶
    public BaseConfig AutoMilkAnimal { get; set; } = new(1);
    public float StopMilkAnimalStamina { get; set; } = 3;
    public bool FindMilkPailFromInventory { get; set; } = true;

    // 自动剪毛
    public BaseConfig AutoShearsAnimal { get; set; } = new(1);
    public float StopShearsAnimalStamina { get; set; } = 3;
    public bool FindShearsFromInventory { get; set; } = true;

    // 自动喂食动物饼干
    public BaseConfig AutoFeedAnimalCracker { get; set; } = new(1);

    // 自动打开动物门
    public bool AutoOpenAnimalDoor { get; set; }

    // 自动打开栅栏门
    public BaseConfig AutoOpenFenceGate { get; set; } = new(1);

    #endregion;

    #region 采矿

    // 自动清理石头
    public BaseConfig AutoClearStone { get; set; } = new(1);
    public float StopClearStoneStamina { get; set; } = 3;
    public bool FindPickaxeFromInventory { get; set; } = true;
    public bool ClearStoneOnMineShaft { get; set; }
    public bool ClearStoneOnVolcano { get; set; }
    public bool ClearFarmStone { get; set; } = true;
    public bool ClearOtherStone { get; set; }
    public bool ClearIslandStone { get; set; }
    public bool ClearOreStone { get; set; }
    public bool ClearGemStone { get; set; }
    public bool ClearGeodeStone { get; set; }
    public bool ClearCalicoEggStone { get; set; }
    public bool ClearBoulder { get; set; }
    public bool ClearMeteorite { get; set; }

    // 自动收集煤炭
    public BaseConfig AutoCollectCoal { get; set; } = new(1);

    // 自动破坏容器
    public BaseConfig AutoBreakContainer { get; set; } = new(1);
    public bool FindToolForBreakContainer { get; set; } = true;

    // 自动打开宝藏
    public BaseConfig AutoOpenTreasure { get; set; } = new(1);

    // 自动清理水晶
    public BaseConfig AutoClearCrystal { get; set; } = new(1);
    public bool FindToolForClearCrystal { get; set; } = true;

    // 自动冷却岩浆
    public BaseConfig AutoCoolLava { get; set; } = new(1);
    public float StopCoolLavaStamina { get; set; } = 3;
    public bool FindToolForCoolLava { get; set; } = true;

    // 显示矿井信息
    public bool ShowLadderInfo { get; set; } = true;
    public bool ShowShaftInfo { get; set; } = true;
    public bool ShowMonsterInfo { get; set; } = true;
    public bool ShowMonsterKillInfo { get; set; } = true;
    public bool ShowMineralInfo { get; set; } = true;

    #endregion

    #region 采集

    // 自动觅食
    public BaseConfig AutoForage { get; set; } = new(1);

    // 自动收获姜
    public BaseConfig AutoHarvestGinger { get; set; } = new();
    public float StopHarvestGingerStamina { get; set; } = 3;
    public bool FindToolForHarvestGinger { get; set; } = true;

    // 自动砍树
    public BaseConfig AutoChopTree { get; set; } = new(1);
    public float StopChopTreeStamina { get; set; } = 3;
    public bool ChopTapperTree { get; set; }
    public bool ChopVinegarTree { get; set; }
    public Dictionary<int, bool> ChopOakTree { get; set; } = new() { { 0, true }, { 1, false }, { 2, false }, { 3, false }, { 4, false }, { 5, false }, { -1, false } };
    public Dictionary<int, bool> ChopMapleTree { get; set; } = new() { { 0, true }, { 1, false }, { 2, false }, { 3, false }, { 4, false }, { 5, false }, { -1, false } };
    public Dictionary<int, bool> ChopPineTree { get; set; } = new() { { 0, true }, { 1, false }, { 2, false }, { 3, false }, { 4, false }, { 5, false }, { -1, false } };
    public Dictionary<int, bool> ChopMahoganyTree { get; set; } = new() { { 0, true }, { 1, false }, { 2, false }, { 3, false }, { 4, false }, { 5, false }, { -1, false } };
    public Dictionary<int, bool> ChopPalmTree { get; set; } = new() { { 2, true }, { 3, true }, { 4, false }, { 5, false }, { -1, false } };
    public Dictionary<int, bool> ChopMushroomTree { get; set; } = new() { { 0, true }, { 1, false }, { 2, false }, { 3, false }, { 4, false }, { 5, false }, { -1, false } };
    public Dictionary<int, bool> ChopGreenRainTree { get; set; } = new() { { 0, true }, { 1, false }, { 2, false }, { 3, false }, { 4, false }, { 5, false }, { -1, false } };
    public Dictionary<int, bool> ChopMysticTree { get; set; } = new() { { 0, true }, { 1, false }, { 2, false }, { 3, false }, { 4, false }, { 5, false }, { -1, false } };

    // 自动摇树
    public BaseConfig AutoShakeTree { get; set; } = new(1);

    // 自动收获苔藓
    public BaseConfig AutoHarvestMoss { get; set; } = new(1);
    public bool FindScytheFromInventory { get; set; } = true;

    // 自动放置采集器
    public BaseConfig AutoPlaceTapper { get; set; } = new(1);

    // 自动放置醋
    public BaseConfig AutoPlaceVinegar { get; set; } = new(1);

    // 自动清理木头
    public BaseConfig AutoClearWood { get; set; } = new(1);
    public float StopClearWoodStamina { get; set; } = 3;
    public bool ClearTwig { get; set; } = true;
    public bool ClearStump { get; set; }
    public bool ClearHollowLog { get; set; }
    public bool FindAxeFromInventory { get; set; } = true;

    #endregion

    #region 钓鱼

    // 自动抓取宝箱物品
    public bool AutoGrabTreasureItem;
    public bool AutoExitTreasureMenu;

    // 自动放置蟹笼
    public BaseConfig AutoPlaceCarbPot { get; set; } = new(1);

    // 自动添加鱼饵鱼饵
    public BaseConfig AutoAddBaitForCarbPot { get; set; } = new(1);

    // 自动收获蟹笼
    public BaseConfig AutoHarvestCarbPot { get; set; } = new(1);

    #endregion

    #region 食物

    // 自动吃食物_体力
    public bool AutoEatFoodForStamina { get; set; }
    public float AutoEatFoodStaminaRate { get; set; } = 0.1f;
    public bool IntelligentFoodSelectionForStamina { get; set; } = true;
    public int RedundantStaminaFoodCount { get; set; } = 20;

    // 自动吃食物_生命值
    public bool AutoEatFoodForHealth { get; set; }
    public float AutoEatFoodHealthRate { get; set; } = 0.1f;
    public bool IntelligentFoodSelectionForHealth { get; set; } = true;
    public int RedundantHealthFoodCount { get; set; } = 20;

    // 自动吃增益食物
    public bool AutoEatBuffFood { get; set; }
    public BuffType FoodBuffMaintain1 { get; set; } = BuffType.Speed;
    public BuffType FoodBuffMaintain2 { get; set; } = BuffType.None;

    // 自动喝增益饮料
    public bool AutoDrinkBuffDrink { get; set; }
    public BuffType DrinkBuffMaintain1 { get; set; } = BuffType.Speed;
    public BuffType DrinkBuffMaintain2 { get; set; } = BuffType.None;

    #endregion

    #region 其他

    // 磁力半径增加
    public int MagneticRadiusIncrease { get; set; }

    // 自动清理杂草
    public BaseConfig AutoClearWeeds { get; set; } = new(1);
    public bool FindToolForClearWeeds { get; set; } = true;
    public bool ClearLargeWeeds { get; set; }

    // 自动挖掘远古斑点
    public BaseConfig AutoDigSpots { get; set; } = new();
    public float StopDigSpotsStamina { get; set; } = 3;
    public bool FindHoeFromInventory { get; set; } = true;

    // 自动收获机器
    public BaseConfig AutoHarvestMachine { get; set; } = new(1);

    // 自动触发机器
    public BaseConfig AutoTriggerMachine { get; set; } = new(1);

    // 自动翻垃圾桶
    public BaseConfig AutoGarbageCan { get; set; } = new(1);
    public bool StopGarbageCanNearVillager { get; set; } = true;

    // 自动放置地板
    public BaseConfig AutoPlaceFloor { get; set; } = new();

    #endregion
}