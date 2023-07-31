**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/agilbert1412/StardewArchipelago**

----

# Stardew Valley Mods Setup Guide

## What mods are supported?

The following mods are currently supported:

General: 
- DeepWoods, by Max Vollmer https://www.nexusmods.com/stardewvalley/mods/2571
- Skull Cavern Elevator, by Lestoph https://www.nexusmods.com/stardewvalley/mods/963
- Bigger Backpack, by spacechase0 https://www.nexusmods.com/stardewvalley/mods/1845
- Tractor Mod, by Pathoschild https://www.nexusmods.com/stardewvalley/mods/1401

Skills:
- Magic, by spacechase0 https://www.nexusmods.com/stardewvalley/mods/2007
- Luck Skill, by spacechase0 https://www.nexusmods.com/stardewvalley/mods/521
- Socializing Skill, by drbirbdev https://www.nexusmods.com/stardewvalley/mods/14142
- Archaeology, by drbirbdev https://www.nexusmods.com/stardewvalley/mods/15793
- Cooking Skill, by spacechase0 https://www.nexusmods.com/stardewvalley/mods/522
- Binning Skill, by drbirbdev https://www.nexusmods.com/stardewvalley/mods/14073

Custom NPCs:
- Juna - Roommate NPC, by elhrvy https://www.nexusmods.com/stardewvalley/mods/8606
- Professor Jasper Thomas, by Lemurkat https://www.nexusmods.com/stardewvalley/mods/5599
- Alec Revisited, by HopeWasHere and SoftPandaBoi https://www.nexusmods.com/stardewvalley/mods/10697
- Custom NPC - Yoba, by Jerem https://www.nexusmods.com/stardewvalley/mods/14871
- Custom NPC Eugene, by Leroy and translated by Sapiescent https://www.nexusmods.com/stardewvalley/mods/9222
- 'Prophet' Wellwick, by Jyangnam https://www.nexusmods.com/stardewvalley/mods/6462
- Mister Ginger (cat npc), by Lemurkat https://www.nexusmods.com/stardewvalley/mods/5295
- Shiko - New Custom NPC, by Papaya https://www.nexusmods.com/stardewvalley/mods/3732
- Delores - Custom NPC, by blaaze6 and Obamoose https://www.nexusmods.com/stardewvalley/mods/5510
- Ayeisha - The Postal Worker (Custom NPC), by TheLimeyDragon https://www.nexusmods.com/stardewvalley/mods/6427
- Custom NPC - Riley, by SG https://www.nexusmods.com/stardewvalley/mods/5811


## Questions and Answers

**Q:** *Where do I add the mods?  I don't see a spot for it.*

**A:** The way mod support operates is through a list.  The website (as of writing this guide) does not include a section 
for this, but the yaml does.  Simply add the following as an option:

`mods: [Tractor Mod, Delores - Custom NPC, ...]`

The names used in generation are the same names given above.

**Q:** *I think there's a bug I found, should I make an error report?*

**A:** Of course, but there's a few things to note first.
- Test with only the mods requested by the YAML settings to function.  Its possible a mod outside of the support list is causing a conflict.
- The AP mod will note to you a possible mod version discrepancy.  Note this when making the report.

Mod support can always be messy and chaotic due to the many developers working on their own projects.  If you believe there 
is a bug within the mod itself, make sure the mod being noted is actually up-to-date with the mod the author has pushed before 
doing so.  

**Q:** *I think a certain mod would make a great addition to AP, who should I talk to?*

**A:** Currently Witchybun works on mod support, but such mod support is always open to the public to be discussed in 
the Discord server to be implemented and is not particularly gatekept by them in particular.  However for the record, the 
following mods are in the works to be supported:
- Stardew Valley Expanded
- Ridgeside Village
- East Scarp

Other mods are also being considered which are less in scope, but are too plentiful to be noted here.  These can be suggested 
either in #stardew-valley or its thread "Mod Support".