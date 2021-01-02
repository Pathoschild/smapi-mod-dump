**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/jltaylor-us/StardewToDew**

----

# Stardew Valley To-Dew List

An in-game to-do list mod for Stardew Valley.  Horrible pun included at no extra charge.

You can find this mod on [Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/7409)
if you prefer.


## Installation

Follow the usual installation proceedure for SMAPI mods:
1. Install [SMAPI](https://smapi.io)
2. Download the latest realease of this mod and unzip it into the `Mods` directory
3. Run the game using SMAPI


## How To Use

Bring up the to-do list by pressing the configurable hotkey (default `L`
for "**l**ist").

Enter text in the textbox.  Press enter to add it to the list.

Use the scroll gesture on your mouse or trackpad to scroll the list if it
has more items than fit on one screen.  (Arrows will appear to indicate
that there are more items.)

Click a list item to remove it from the list.

Press escape to close the list.

List data is saved in the game save file, so any changes since the last
save will be lost when you exit.

## Configuration

When SMAPI runs the mod for the first time it will create a `config.json`
in the mod directory.  You can edit this file to configure the hotkey for
opening the to-do list.

If [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098)
is installed then there will be an entry for To-Dew in the in-game config
menu.


## Compatibility

Works with Stardew Valley 1.5 / SMAPI 3.8, single and multiplayer (but see
further notes on Multiplayer below).  No known incompatibilities with
other mods.


## Multiplayer Support

There is a single list for the farm, and everyone (with the mod
installed) can edit it.

List data is saved in the game save file, which is only accessible to the
host.  So, in order to use the to-do list, the host must have To-Dew
installed.  Farmhands that have To-Dew installed will also be able to use
the list.  If a farmhand has To-Dew installed but the host does not then
the farmhand will not be able to use the to-do list.  (I.e., there is no
combination of places where the mod is installed or not that will result
in the game being unplayable.)

If the host and a farmhand have different versions of To-Dew installed,
the mod attempts to make intelligent decisions about what to do.  Look for
messages in the SMAPI console log.  This scenario is probably not
well-tested and may contain bugs.
