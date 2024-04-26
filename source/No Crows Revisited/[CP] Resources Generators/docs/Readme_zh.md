**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/siweipancc/StardewMods**

----

# [CP] Resources Generators

[English](Readme.md)

## Description

大型可制造器械, 自动化生产基础物料: 泥土, 纤维, 石头, 木头等.

## 机器种类

### 泥土机

价格 3,000, 产出:

1. 100% \[330] Clay // 粘土
2. 005% \[382] Coal // 煤炭
3. 004% \[535] Geode // 晶球
4. 004% \[536] Frozen Geode // 冰封晶球
5. 004% \[537] Magma Geode // 熔岩晶球
6. 003% \[749] Omani Geode // 万象晶球
7. 002% \[MysteryBox] Mystery Box // 谜之盒\(齐先生飞机事件后)

### 纤维机

价格 2,000, 产出:

1. 100% \[771] Fiber // 纤维
2. 006% \[770] Mixed Seeds // 混合种子
3. 005% \[495~498] Spring/Summer/Fall/Winter Seeds // 对应四级季节的种子
4. 006% \[MixedFlowerSeeds] Mixed Flower Seeds // 混合花卉种子\(游戏时间一个月之后)
5. 008% \[Moss] Moss // 苔藓\(绿雨事件之后)
6. 001% \[MossySeed] Mossy Seed // 苔藓种\(绿雨事件之后)

### 石头机

价格 3,000, 产出:

1. 100% \[390] Stone // 石头
2. 006% \[378] Copper Ore // 铜矿石
3. 004% \[380] Iron Ore // 铁矿石
4. 003% \[384] Gold Ore // 黄金矿石
5. 002% \[386] Iridium Ore // 铱矿石

### 木头机

价格 3,000, 产出:

1. 100% \[388] Wood // 木头
2. 010% \[709] Hardwood //硬木
3. 015% \[382] Coal // 煤炭

## 商店更改

所有新添加的器械可以在铁匠铺处购买.

## 附录

**Condition** 源实现 `StardewValley.GameStateQuery.DefaultResolvers`

1. 齐先生飞机事件: PLAYER_HAS_MAIL Host sawQiPlane received , 在代码 `StardewValley.Utility.tryRollMysteryBox`
2. 绿雨事件: PLAYER_HAS_MAIL Host GreenRainGus received , 在邮件 `Data\mail`