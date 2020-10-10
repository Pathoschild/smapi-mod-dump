**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/danvolchek/StardewMods**

----

# Removable Horse Hats

## Deprecated

Similar functionality was added in SDV 1.4, so this mod is no longer supported.

See [This link](http://www.nexusmods.com/stardewvalley/mods/2223?) for the NexusMods mod page, which has a description, screenshots, and a download of the built mod.

## How it works

The mod uses [Harmony](https://github.com/pardeike/Harmony) to overwrite the horse's `checkAction` method to instead remove the hat the horse is wearing if it is wearing one and the player is holding down the activation key.