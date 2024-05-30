**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/calebstein1/StardewDisableBuildingFade**

----

# Disable Building Fade

A mod for Stardew Valley that disables the buildings becoming transperent when the player walks behind them.
If GenericModConfigMenu is installed, you'll get a toggle in the Mod Settings to turn transperency on or off.

Idea from the Reddit thread [here](https://www.reddit.com/r/StardewValleyMods/comments/1d1swvu/mod_to_disable_buildings_fadingbecoming/)

### Known bugs

- The settings toggle state doesn't save between game launches (if you enable transperency, it will be disabled again the next time you launch the game)
- If you use the settings toggle to disable transperency while the player is behind a building that's currently semi-transperent, the building will remain transperent even after the player walks away from it (re-enabling transperency will fix the visual bug)
