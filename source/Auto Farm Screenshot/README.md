**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Dwayneten/AutoFarmScreenshot**

----

![AutoFarmScreenshot](https://raw.githubusercontent.com/Dwayneten/AutoFarmScreenshot/master/header.jpg)
# AutoFarmScreenshot
A Stardew Valley mod that help you automatically takes a screenshot of farm everyday.

### REQUIREMENTS :

[SMAPI - Stardew Modding API 3.0.1+](https://github.com/Pathoschild/SMAPI)


### HOW THIS MOD WORK :

With Stardew Valley 1.4 update it comes a new map export feature

> Added map export feature (except on 32-bit Linux), accessed via a button in the options menu or the /mapscreenshot chat command. Screenshots are saved in the appdata folder; click a button in the options screen to open it. When using the command, you can optionally specify a filename and percentage size like /mapscreenshot test 25.

This mod automatically call this method everyday when you first appear at your farm **AND** the game time update(e.g. pass 10 in-game minutes).

Normally, everyday morning you come outside from your farmhouse and as soon as the game time update(e.g. 6:00 am turn into 6:10am), this mod will auto call the screenshot method for you. It will lag for a bit at that moment.

If nothing wrong, you will see a tooltip at the left-down corner showing you screenshot saved.

Screenshots will be saved in "%appdata%\StardewValley\Screenshots". You can also open that folder via a button in the bottom of options menu.


### WITH THIS MOD YOU CAN :

- Record your farm development progress

- Use these screenshot to generate a GIF to see how your farm has changed.

### CONFIG :

Open the config.json and change the scale number with in (0, 1] (default: 0.25 means 25%)

``` json
{
  "ScaleNumber": 0.25
}
```

### NEXUS PAGE
[AutoFarmScreenshot](https://www.nexusmods.com/stardewvalley/mods/4783/)
