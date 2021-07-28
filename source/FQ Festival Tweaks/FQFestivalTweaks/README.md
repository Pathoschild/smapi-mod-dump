**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/FletcherGoss/FQTweaks**

----

**FQ Festival Tweaks** is a [Stardew Valley](https://www.stardewvalley.net/) mod where you return home at the time a festival was supposed to end.

## Install
1. [Install the latest version of SMAPI](https://smapi.io/).
2. Install [this mod from Nexus Mods](http://www.nexusmods.com/stardewvalley/mods/9062).
3. Run the game using SMAPI.

## Configure
The mod creates a `config.json` file in its mod folder the first time you run it. You can open that file in a text editor to configure the mod.

| Setting  | Description
| -------- | -------------------
| `MinMinutesAtFestival` | Default `0`. The minimum amount of minutes that should pass between you arriving at and leaving the festival; `0` to always end at the advertised end time.
| `MaxEndTime` | Default `2350`. The latest time a festival can end; can be between `0000` and `2600`.

## Compatibility
FQ Festival Tweaks is compatible with Stardew Valley 1.5+ on Windows/Linux/Mac, both single-player and multiplayer. There are no known issues in multiplayer (even if other players don't have it installed).

It should be compatible with mods that alter festival times or add new festivals, providing they modify/patch the XNB files (`FestivalDates.xnb` and `[season][day].xnb`).

## Known Issues
- Equipment will act as though you returned home at the vanilla time.  
	<small>For example, if you go to The Egg Festival at 1200 and finish at 1400, it'll act as though 10 hours have passed. I don't know whether it's possible to fix this, but am exploring options.</small>

## Links
* [Release Notes](release-notes.md)
* [Nexus Mods](http://www.nexusmods.com/stardewvalley/mods/9062)
