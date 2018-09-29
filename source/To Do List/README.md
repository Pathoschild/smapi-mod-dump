**To Do List** is a [Stardew Valley](http://stardewvalley.net/) mod that adds a menu allowing you to type small reminders for tasks you want to complete. 

![](Screenshots/usage.gif)

## Contents
* [Install](#install)
* [Use](#use)
* [Configure](#configure)
* [Versions](#versions)
* [See also](#see-also)

## Install
1. [Install the latest version of SMAPI](https://github.com/Pathoschild/SMAPI/releases).
2. [Install this mod from Nexus mods](http://www.nexusmods.com/stardewvalley/mods/NEXUSLINK/).
3. Run the game using SMAPI.

## Use
* Press F2 (configurable) to open the to do list.  
* By typing in the text box and pressing the Ok button or the enter key, you can add a task to the list. New tasks are placed at the top of the list.
* When you no longer need a task reminder, click on it to remove it. 
* Tasks are saved between sessions and are individual to each save file, so any saved tasks will still be in the list when you next load that save.

## Configure
The mod can be configured to change its default settings and enable additional features by editing the "config.json" file. Applied settings are in effect for every save file. 

| setting           | default |what it affects
| ----------------- | --------|-------------------
| `OpenListKey` | F2 | The button that opens the to do list. A keyboard key cannot be a commonly typed character, or it will affect typing in the textbox- if the mod detects an invalid key, it will reset to default.
| `UseLargerFont` | false     | If enabled, switches all menu text to a larger font, but gives less characters to type tasks in.
| `OpenOnStartup` | false | If enabled, as soon as your save file is loaded, the to do list will appear to remind you of the tasks you had saved since last time.

## Versions
1.0:
* Initial release.
