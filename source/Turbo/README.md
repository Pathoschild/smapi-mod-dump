**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/PrimmR/Turbo**

----

# Turbo
A mod for Stardew Valley that allows you to alter the game's execution speed.

This tool is mainly intended to be used by mod developers, e.g. to reduce the waiting involved with checking custom NPC's schedules.

This is a spiritual successor to the [GameSpeed](https://www.nexusmods.com/stardewvalley/mods/7211) mod, which sadly isn't compatible with Stardew Valley 1.6. 

*Please note: I am not responsible for any damage this causes to your game/save.*  
*Always back everything up before you decide to start messing around with the fabric of time.*

## Usage
This mod provides two main functions: setting the execution speed of the game and setting how the in-game clock responds to the speed change.

The execution speed is measured as a multiplier of the regular game speed, with values within the range (0, 1) representing slower execution, and values greater than one representing a greater speed of execution.  
This mod keeps the speed in the range [0.25, 32], as running the game at speeds beyond these makes it very difficult to control.

The behaviour of the in-game clock is saved between sessions and can have one of three states:

|Clock Mode|Value|Description|
|---|---|---|
|**Regular**|`0`|The clock increments proportionally with game speed
|**Constant**|`1`|The clock increments at a constant rate, regardless of game speed
|**Frozen**|`2`|The clock is frozen

### In Game
This mod adds 4 actions that can be triggered in-game through keyboard input. The default keybinds can be edited in the `config.json` file.

|Action|Key|KeyName|Description|
|---|---|---|---|
|Decrement Speed|**< (,)**|`OemComma`|Halves the current game speed
|Increment Speed|**> (.)**|`OemPeriod`|Doubles the current game speed
|Reset Speed|**;**|`OemSemicolon`|Sets the game speed to 1x
|Cycle Clock Mode|**'**|`OemQuotes`|Cycles through the clock mode options

### SMAPI Console
This mod also includes 2 SMAPI console commands that can be used in place of the in-game keybinds:

`set_speed <value>`  
`set_clock_mode <value>`

*The former command allows you to select any speed for the game to run at, which includes values outside of the 'safe range'. So make sure you double check your input before pressing enter!*


## Bugs
If you notice a problem with the behaviour of the mod, please raise an issue in the GitHub issue tracker, thanks!