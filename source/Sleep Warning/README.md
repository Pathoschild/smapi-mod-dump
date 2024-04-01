**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/GlimmerDev/StardewValleyMod_SleepWarning**

----

# Sleep Warning

A simple Stardew Valley mod that plays a sound to remind you to head back to your bed when it starts to get late. Good for those (like me) who are bad at time management, or that might not as easily notice the visual cue.

The mod will, by default, play the elevator "ding" sound at the following intervals: 11PM (one ding), 12AM (two dings), 1AM (three dings.) Both the times and the sound can be configured using the below config options.

## Installation
This mod requires Stardew Valley 1.6, and SMAPI >= 4.0.0.

1. Download the mod.
2. Unzip the downloaded ZIP file.
3. Copy the "SleepWarning" mod to your "Mods/" folder.


## Configuration

### Generic Mod Config Menu
This mod supports [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098), which provides a user friendly interface for changing configuraiton options. Simply install GMCM, and Sleep Warning will appear in the list. GMCM is **not required** to use this mod.

### Config File
The config will generate automatically once you load the game with the mod installed. To reset the config, simply delete the "config.json" in the mod folder.

``FirstWarnTime``  
The time to play the first warning sound. Set to ``-1`` to disable.  
*Default: ``2300`` (11PM)*

``SecondWarnTime``  
The time to play the second warning sound. Set to ``-1`` to disable.  
*Default: ``2400`` (12AM)*

``ThirdWarnTime``  
The time to play the third warning sound. Set to ``-1`` to disable.  
*Default: ``2500`` (1AM)*

``WarningSound``  
The sound name of the sound to play at each warning time. See [here](https://stardewvalleywiki.com/Modding:Audio#Sound) for a list of valid sound names.  
*Default: ``"crystal"`` (Elevator "ding")*

