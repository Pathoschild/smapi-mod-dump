**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/agilbert1412/StardewArchipelago**

----

# Stardew Valley Archipelago Supported Mods Setup Guide

## Supported Mods

Supported mods are a set of mods that have been integrated into the Stardew Valley Archipelago implementation. This means that these mods are considered and included in relevant Archipelago features.
For example, mods that add regions will be included in the Entrance Randomizer, mods that add NPCs will be included in friendsanity, mods that add skills will be considered when shuffling skill levels.
Some of these mods also include completely unique checks and items based on the mod's content.

When a mod is supported, only a very specific version of that mod has been evaluated and tested for compatibility. We do not have control over future changes to mods, so players must use that specific mod version to play their Archipelago slot.
A reasonable effort will be made to stay up to date, but it has to be understood that this does not garantee a given timeline for supporting a new version of a mod, especially when content is added.

## What mods are supported?

The following mods are currently supported:

General: 
- [DeepWoods](https://www.nexusmods.com/stardewvalley/mods/2571), by Max Vollmer, Version 3.0.0-beta
- [Skull Cavern Elevator](https://www.nexusmods.com/stardewvalley/mods/963), by Lestoph, Version 1.5.0
- [Bigger Backpack](https://www.nexusmods.com/stardewvalley/mods/1845), by spacechase0, Version 6.0.0
- [Tractor Mod](https://www.nexusmods.com/stardewvalley/mods/1401), by Pathoschild, Version 4.16.4

Skills:
- [Magic](https://www.nexusmods.com/stardewvalley/mods/2007), by spacechase0, Version 0.8.2
- [Luck Skill](https://www.nexusmods.com/stardewvalley/mods/521), by spacechase0, Version 1.2.4
- [Socializing Skill](https://www.nexusmods.com/stardewvalley/mods/14142), by drbirbdev, Version 1.1.5
- [Archaeology](https://www.nexusmods.com/stardewvalley/mods/15793), by drbirbdev, Version 1.5.0
- [Cooking Skill](https://www.nexusmods.com/stardewvalley/mods/522), by spacechase0, Version 1.4.5
- [Binning Skill](https://www.nexusmods.com/stardewvalley/mods/14073), by drbirbdev, Version 1.2.7

Custom NPCs:
- [Juna - Roommate NPC](https://www.nexusmods.com/stardewvalley/mods/8606), by elhrvy 2.1.3
- [Professor Jasper Thomas](https://www.nexusmods.com/stardewvalley/mods/5599), by Lemurkat 1.7.6
- [Alec Revisited](https://www.nexusmods.com/stardewvalley/mods/10697), by HopeWasHere and SoftPandaBoi, Version 2.1.0
- [Custom NPC - Yoba](https://www.nexusmods.com/stardewvalley/mods/14871), by Jerem, Version 1.0.0
- [Custom NPC Eugene](https://www.nexusmods.com/stardewvalley/mods/9222), by Leroy and translated by Sapiescent, Version 1.3.1
- ['Prophet' Wellwick](https://www.nexusmods.com/stardewvalley/mods/6462), by Jyangnam, Version 1.0.0
- [Mister Ginger (cat npc)](https://www.nexusmods.com/stardewvalley/mods/5295), by Lemurkat, Version 1.5.9
- [Shiko - New Custom NPC](https://www.nexusmods.com/stardewvalley/mods/3732), by Papaya, Version 1.1.0
- [Delores - Custom NPC](https://www.nexusmods.com/stardewvalley/mods/5510), by blaaze6 and Obamoose, Version 1.1.2
- [Ayeisha - The Postal Worker (Custom NPC)](https://www.nexusmods.com/stardewvalley/mods/6427), by TheLimeyDragon, Version 0.5.0-alpha
- [Custom NPC - Riley](https://www.nexusmods.com/stardewvalley/mods/5811), by SG, Version 1.2.2

## How to add supported mods to my Archipelago slot?

To include supported mods in your multiworld slot, you need to include a section in your yaml settings called "mods".
This section must be an array with the **exact** names of every mod you wish to include. Any improperly typed mod name will be ignored.
![image](https://i.imgur.com/uOHtXmU.png)

These mods will then be included in the multiworld generation, and considered in logic. For example, the Magic mod includes a spell that allow a player to teleport, and, if included, teleporting can be required to reach checks.

As previously mentioned, the generator and the StardewArchipelago client are designed and tested for a very specific version of any supported mod. When installing them, you must choose the correct version, or you will not be able to play.
The StardewArchipelago mod will inform you if some of your supported mods have the wrong version, and tell you what version you need.

Most mods also have dependencies to some library/utility mods. You will need to install these as well, but dependency versions are less strict.

If you can load the supported mod on the correct version, the exact version of a dependency is not important.


## Bug Reports

Before reporting a bug, make sure to follow these steps
- Uninstall all unecessary mods from your save file, and verify that your bug still happens. If it doesn't, then it might be a conflict with unsupported mods
- Verify if there is a new version of StardewArchipelago available on the [releases page](https://github.com/agilbert1412/StardewArchipelago/releases). Your bug might already be fixed
- Check your SMAPI console when the bug happens. Keep a eye out for anything red, and take screenshots.

To report the bug, you can use the github bug [report page](https://github.com/agilbert1412/StardewArchipelago/issues/new), or head over to the [Archipelago Discord server](https://discord.gg/8Z65BR2) to use the bug reports channel.
You can also simply head to the Stardew Valley channel in the Archipelago Discord to talk about the issue with people and find a workaround.

## I think a certain mod would make a great addition to AP, who should I talk to?

First, make sure you join the [Archipelago Discord server](https://discord.gg/8Z65BR2)

Currently Witchybun works on mod support, but such mod support is always open to the public to be discussed in 
the Discord server to be implemented and is not particularly gatekept by them in particular.  However for the record, the 
following mods are in the works to be supported:
- Stardew Valley Expanded
- Ridgeside Village
- East Scarp

Other mods are also being considered which are smaller in scope, but are too numerous to be listed here.  These can be suggested either in #stardew-valley or its thread "Mod Support".
