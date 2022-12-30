**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/tlitookilakin/WarpNetwork**

----

# Warp Network
Warp Network is a flexible and lightweight fast-travel mod for Stardew Valley. It is designed to be easily expandable and easy-to-use. For more information on integrating mods, please see the wiki.

## Configuration
Warp Network can be configured either in-game (if Generic Mod Config Menu is also installed), or by directly editing the `config.json`file in the Warp Network mod folder. (Note: you need to run the game with the mod loaded **at least once** before the config file will appear.)

For a list of config options and a description of what they do, please visit the wiki page.

## Known Issues
* Adding the `WarpNetworkEntry` property to one of the vanilla warp locations (Mountain, Farm, Beach, Desert, Ginger Island) using `EditMap` will not prevent the warp network from creating an access point at the normal warp position. (Use it with Load, or edit the destination data to match.)
* Warping into a festival (or other event) will always place the player at the usual starting/entry position. Due to the way events and festivals work, this will not be fixed.

## Compatibility
No known incompatibilities.

## Troubleshooting
When reporting a bug, please follow these steps:

1. Fully launch the game, then get to the part where the problem occurs.
2. If this does not crash the game, type `warpnet debug` into console. if the game crashed, skip this step.
3. If the game is still open at this point, close it.
4. Upload a log [here](https://smapi.io/log), following the instructions on the page.
5. Post a bug in the issue tracker, including a link to your uploaded log, and a *detailed* description of the bug. 

It is also recommended to look for help on the `#using-mods` channel on the SDV discord, but not required.
