**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ofts-cqm/StardewValley-Agenda**

----

# StardewValley-Agenda

注意：中文版本在下面

# NOTE: 
This is a beta version, it may be very unstable and probably contains a lot of bugs (I made this in one week!)
If you found any bugs, please post it. 

# Introduction:

* This mod adds an Agenda to your game. You can record your future plan on the Agenda, and it will reminds you when the date is come. You can also use it to replace the vanilla calendar. Default open by "G". 

* You will never forget your schema! Agenda will always reminds you!

## Install
1. Install the latest version of SMAPI.
2. Download this mod and unzip it into Stardew Valley/Mods.
3. Run the game using SMAPI.


## How to use

* You can open the menu by G. On the top right of the screen, you can found two arrows, you can switch seasons by clicking them. The default season is the current season, but you can switch it as you want. On the top left corner, there is a short title tells you which season the Agenda is currently on. 

* Once you opened the Agenda, a calendar-liked menu will pop-up (but each cell is 1.25x!)

* Holidays (include night market) and Birthdays are on the Agenda by default. Marriages are currently not but will come soon. 

* You can edit the agenda by clicking the cell, and a new menu will pop up. You can edit the title and the note by clicking the line/box. You cannot edit the birthdays and Festivals since they are default. If the text overflows it will be cut off but it will still be recorded.

### Trigger: 
- You can choose the condition, time, and count. 
- Once your condition is met, it will be "triggered", with a special pop-up notification
- Common condition include Monday - Sunday, rainy days (both island and mainland) and sunny days, lucky days and unlucky days
- Time can be "before", "on", or  "after"
- count can be "next one" to "next ten", or "every"
- An example of trigger can be found in the image page. 
- You have 14 triggers (meaning you can at most set 14 triggers)
- The trigger page can be found after the Winter page. 

- After open the trigger editing page, you can found the current trigger on the third line. Just click what you want to change to edit. 

- There will be a notification if your current trigger is invalid

- Don't worry, trigger won't involve any kind of programming. You just choose the time, count, and condition that best fit your need. 

* The content is always auto saved so you don't need to worry about losing your progress. (but the main save is updated once a day)

* When a day ends, the notes for that day will be automatically deleted. You can modify the config to disable this function. 

### Note: for some users the menu may be too big and may not be displayed fully. You may adjust the GUI scale to solve this.

## Compatibility
- Works with Stardew Valley 1.5.6 on Linux/macOS/Windows.
- Works in single player, multiplayer and split-screen compatibility is unknown (because I CANT FIND ANYONE TO TEST MULTIPLAYER - COMPABILITY!!!)
- Compatible with Generic Mod Config Menu at Stardew Valley Nexus - Mods and community (nexusmods.com)

## Command
### agenda [season] [date]
* query the note on the specified season (0-3, 0 for spring, 1 for summer, etc.) and date (0-27, the current date - 1)

## Configuration

* default open by 'G'
* You can use it to replace the vanilla calendar if you set "Replace_Calender_With_Agenda" to "true"
* You can set "Auto_Delete_After_Complete" to "false" to disable auto-deletion after completion
* you can configure all above by editing the "Config.json" file


## (Possible) Upcoming Feature

- Trigger - special agenda that will be triggered once a certain user-customized condition is satisfied
- e.g. raining days, Wednesday, everyday, etc. 
- Allow users to customize Agenda texture


## See also

* [My Bilibili Chanel](http://space.bilibili.com/1556047681?spm_id_from=333.1007.0.0)
* [Nexus](https://www.nexusmods.com/stardewvalley/mods/20432)



Note: English Version is Above

# 注意：
这是一个测试版本，可能包含很多bug并且很不稳定，一旦发现任何bug请在评论区说明

# 介绍

* 这个mod添加了一个待议事件簿，您可以记录你未来的计划并在到来之时提醒您。您还可以用它替代原版的日历。默认用G打开

* 麻麻再也不怕我忘记计划了！待议事件簿永远会提醒你！

## 下载
1. 下载最新版本的SMAPI.
2. 下载并解压到 Stardew Valley/Mods.
3. 使用SMAPI运行游戏.


## 如何使用

* 您可以通过G键打开菜单。屏幕右上角有两个箭头，您可以通过点击切换季节。屏幕左上角有一行字显示当前季节。默认季节是游戏当前季节，但您可以通过箭头切换。

* 当您打开待议事件簿之后，一个类似日历的菜单会打开（但是每个格子比日历大1.25倍！）

* 节日（包括夜市）和生日会自动显示在待议事件簿上，婚姻暂时不能但即将来袭

* 您可以通过点击格子来编辑待议事件，点击之后一个新的菜单将弹出。您可以点击标题和下面的框来编辑标题和记录。您不能更改节日和生日。如果一个文本溢出了它将被截断，但仍能被正确记录

### 扳机：
- 你可以选择条件，次数，和时间
- 当条件满足时，扳机会被触发，并伴随着一个特殊的弹窗提示
- 条件可以使周一到周日，雨天和晴天（包括大陆和姜岛），好运天和霉运天
- 时间可以是前一天，当天，和后一天
- 次数可以是下一次到下10次，或者每次 
- 你有14个扳机（也就是说你最多同时设置14个扳机）
- 一个扳机的例子可以再照片中找到
- 扳机页面可以再冬天页面后面找到

- 在打开扳机编辑菜单后，在第三行可以找到当前扳机。只需要点击你想设置的就行。

- 如果当前扳机无效，会有一个提醒

- 不用担心，扳机不涉及任何编程。你只需要选择最符合你需求的条件，时间，和次数。

* 所有内容都是自动保存的，您不用担心丢失进度（但是总存档仍然是每天一次）

* 当一天结束后，当天的计划将被自动删除，您可以通过配置来禁用此功能。

### 注意：对于某些用户，窗口可能会过大导致显示不全，您可以调整窗口大小来解决

## 指令

### agenda [season] [date]
* 获得指定季节（season，0-3，0代表春季，1代表夏季，以此类推）的指定时间（date, 0-27, 当月日期减一） 的规划


## 兼容性
- 兼容Linux/macOS/Windows系统上的星露谷物语
- 兼容单人游戏，多人游戏兼容性未知（没人陪我玩wwwwwwwwQAQ）
- 兼容 Generic Mod Config Menu at Stardew Valley Nexus - Mods and community (nexusmods.com)


## 配置

* 按G打开
* 您可以通过设置"Replace_Calender_With_Agenda" 为 "true" 来用待议事件簿替换原版的日历
* 您可以通过设置"Auto_Delete_After_Complete" 为 "false" 来禁用自动删除
* 您可以通过编辑 "Config.json" 文件调整以上所有


## (可能的) 即将到来的特性

- 触发器 - 会在用户自定义的条件满足后触发的特殊待议事件
- 比如雨天，星期三，每天等
- 允许您自定义材质


## 参考

* [我的Bilibili主页﻿](http://space.bilibili.com/1556047681?spm_id_from=333.1007.0.0)
* [N网](https://www.nexusmods.com/stardewvalley/mods/20432)
