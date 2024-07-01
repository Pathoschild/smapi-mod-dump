**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/weizinai/StardewValleyMods**

----

# 介绍

该模组能让你对客机玩家的模组进行限制。该模组只需在主机端玩家处安装即可，若客机端玩家安装了该模组，则在SMAPI控制台会收到不满足的模组要求的信息。

# 如何使用

在`config.json`中设置`AllowedModList`、`RequiredModList`和`BannedModList`选项

# 设置

## AllowedModList

该选项仅在限制模式为`白名单模式`时才有效果。该选项中的模组是允许安装的模组。

``` json5
"AllowedModList": 
{
    "Default":
    [
        "ModUniqueId1"
    ]
    "TestName1": 
    [
        "ModUniqueId2",
        "ModUniqueId3"
    ],
    "TestName2": 
    [
        "ModUniqueId4",
        "ModUniqueId5"
    ]
}
```

## RequiredModList

该选项在限制模式为`白名单模式`和`黑名单模式`时均有效果。该选项中的模组是必须安装的模组。

``` json5
"RequiredModList": 
{
    "Default":
    [
        "ModUniqueId1"
    ]
    "TestName1": 
    [
        "ModUniqueId2",
        "ModUniqueId3"
    ],
    "TestName2": 
    [
        "ModUniqueId4",
        "ModUniqueId5"
    ]
}
```

## BannedModList

该选项仅在限制模式为`黑名单模式`时才有效果。该选项中的模组是禁止安装的模组。

``` json5
"BannedModList": 
{
    "Default":
    [
        "ModUniqueId1"
    ]
    "TestName1": 
    [
        "ModUniqueId2",
        "ModUniqueId3"
    ],
    "TestName2": 
    [
        "ModUniqueId4",
        "ModUniqueId5"
    ]
}
```

# 其他

- [源码](https://github.com/weizinai/StardewValleyMods)
- [我的其他模组](https://next.nexusmods.com/profile/weizinai/mods?gameId=1303)