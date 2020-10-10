**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Annosz/StardewValleyModding**

----

# Highlighted Jars

See [this link](http://www.nexusmods.com/stardewvalley/mods/6833) for the NexusMods mod page, which has a description, screenshots, and a download of the built mod.

## How it works

This mod hooks the _Display_RenderedWorld_ event, and iterates over the items that are being rendered. For jars, kegs and casks it draws an additional object (another red item or the speech bubble) as defined in the config file.