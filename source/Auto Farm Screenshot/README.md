**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Dwayneten/AutoFarmScreenshot**

----

![AutoFarmScreenshot](https://raw.githubusercontent.com/Dwayneten/AutoFarmScreenshot/master/header.jpg)
# AutoFarmScreenshot
A Stardew Valley mod that help you automatically takes a screenshot of farm everyday.

### REQUIREMENTS :

[SMAPI - Stardew Modding API 3.8.0+](https://github.com/Pathoschild/SMAPI)


### HOW THIS MOD WORKS :

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

Open config.json and change the values you would like to change 

``` json
{
  "ScaleNumber": 0.25,
  "ScreenshotFormat": "{PlayerName}_{Season}_{Day}_{Year}"
}
```

* ScaleNumber: the zoom scale for the [0, 1] (default: 0.25 is 25% zoom)
* ScreenshotFormat: The formatting for the name of the screenshots being taken
  * {PlayerName} is replaced with the Farmer's name
  * {Season} is replaced with the name of the current season
  * {Day} is replaced with the day of the season in 2 digit format (01, 05, 11)
  * {Year} is replaced with the current year in 2 digit format  (01, 05, 11)
  * {TotalDays} is replaced with the total number of days that have passed since Spring 1, Year 1 in 4 digit format (0001, 0023)
  * {FarmName} is replaced by the name of the farm
  * Anything else is added literally to the screnshot name as is.
  * I.E {PlayerName}_{Season}_{Day}_{Year} is turned into Bob_spring_03_01 for a character named Bob, on spring day 3, year 1

### NEXUS PAGE
[AutoFarmScreenshot](https://www.nexusmods.com/stardewvalley/mods/4783/)
