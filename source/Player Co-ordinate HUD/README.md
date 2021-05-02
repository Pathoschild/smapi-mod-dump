**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/DecidedlyHuman/StardewMods**

----

# DecidedlyHuman's Stardew Valley Mods
This is a repo where I'll be putting all of my Stardew mods. Entire solution is included, sans Pathoschild's ModBuildConfig nuget package. You'll need to install that yourself, though the references for it in the .csproj files should be fine.

All releases can be found on my [Nexus page](https://www.nexusmods.com/stardewvalley/users/79440738). That is the only *official* place to find them.

## Player Co-ordinates
This mod currently does two very simple things:
* It draws either your player, or cursor co-ordinates to the HUD in the top-left of the screen (this will be configurable at some point)
* It can log your player or cursor co-ordinates to a file (currently \<StardewDirectory\>/Mods/PlayerCoordinates/coordinate_output.txt)

Everything has a keybind in the mod's config.json file. Toggling the HUD, switching between player/cursor tracking, and finally, a bind to log the currently tracked co-ordinates to a file.

## Crop Reminder
This is something I want very, very badly personally. It doesn't exist yet.