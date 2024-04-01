**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/qixing-jk/QiXingAutoGrabTruffles**

----

# QiXingAutoGrabTruffles
> This mod is a fix for [Auto-Grab Truffles](https://www.nexusmods.com/stardewvalley/mods/14162)﻿ (updated to .NET 6.0, adapted to Stardew Valley 1.6 and SMAPI 4.0)
> 此Mod为 [Auto-Grab Truffles 自动抓取松露](https://www.nexusmods.com/stardewvalley/mods/14162) 的 修复版（更新至.NET 6.0，适配 星露谷1.6 和 SMAPI 4.0）
> ﻿﻿[Telegram group](https://t.me/qixing_chat) (also a way to contact support)	﻿[Telegram群组](https://t.me/qixing_chat) （也算是联系支持的一种方式）

Auto-Grab Truffles will extend vanilla barn auto-grabber functionality to also work on truffles. And only truffles.
自动抓取松露将扩展原版谷仓自动抓取器的功能，使其也适用于松露。而且只有松露。

Truffles are placed in an auto-grabber in the barn where the pig that found them is housed. Truffles will only be collected if they are found while the mod is installed **AND** enabled (first configuration setting).
松露被放置在谷仓的自动抓取器中，发现松露的猪就住在谷仓里。仅当安装并启用模组（第一个配置设置）时发现松露时，才会收集松露。

This mod (when enabled) will override the truffle collection behavior in [Deluxe Grabber Redux](https://www.nexusmods.com/stardewvalley/mods/7920) if installed; other functions of DGR should be unaffected.
如果安装了此模组（启用后）将覆盖 Deluxe Grabber Redux 中的松露收集行为； DGR 的其他功能应不受影响。

**Configuration** (Edit with [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) or through the config.json file)
配置（使用通用 Mod 配置菜单或通过 config.json 文件进行编辑）

- **Enable Auto-Grab Truffle** [Default: true, false] : Enable automatic collection of truffles
  Enable Auto-Grab Truffle[默认: true, false] : 启用自动收集松露﻿
- ﻿Collection Frequency [Default: "Instantly", "Hourly", "Daily"] : How often truffles are collected
  收集频率[默认：“立即”、“每小时”、“每日”]：收集松露的频率
  - **Instantly** (Collect as soon as truffles are found)
    立即（发现松露后立即收集）
  - **Hourly** (Collect truffles every hour)
    每小时（每小时收集松露）
  - **Daily** (Collect truffles once a day (once a night, really) when the player goes to sleep)
    每天（每天一次（实际上是每晚一次）当玩家睡觉时收集松露）
- ﻿﻿**Gain Experience** [Default: true, false] : Gain foraging experience for each truffle collected
  获得经验[默认值：true, false]：收集每块松露获得觅食经验
- ﻿﻿**Apply Gatherer Bonus** [Default: true, false] : Collected truffles benefit from the Gatherer profession (if picked by the player)
  应用采集者奖励[默认：true, false]：收集的松露从采集者职业中受益（如果由玩家选择）
- ﻿﻿**Apply Botanist Bonus** [Default: true, false] : Collected truffles benefit from the Botanist profession (if picked by the player)
  应用植物学家奖励[默认：true, false]：收集的松露受益于植物学家职业（如果由玩家选择）

**Notes on Multiplayer 多人游戏注意事项**
This mod has not been tested in multiplayer. In theory, truffles are collected if they are found by pigs owned by the current player, but again, this is not tested outside of single player. Feel free to try this mod in your multiplayer game, but be warned that some things might get borked.
该模组尚未在多人游戏中进行测试。理论上，如果当前玩家拥有的猪发现了松露，那么松露就会被收集，但同样，这没有在单人游戏之外进行测试。请随意在多人游戏中尝试此模组，但请注意，有些事情可能会变得无聊。
(If y'all do try it, let me know how it goes.)
（如果你们都尝试一下，请告诉我进展如何。）