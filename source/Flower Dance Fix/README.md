**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/elfuun1/FlowerDanceFix**

----

# Flower Dance Fix

Flower Dance Fix changes the "main event" code of the Flower Dance Festival, allowing for Custom NPCs and edited vanilla NPCs to participate in the dance, and for the pairs of dancers to be randomized. An NPC must be "datable", and not a child, to be eligible to participate in the dance.

## Configuration
Mod settings can be configured in the config.json file included in the mod folder, or on the main menu using the Generic Mod Config Menu. Current config settings are:

**NPCs Have Random Partners** (default: false)
If false, NPCs pairs are generated using the vanilla "Love Interest" pairing. Any NPCs without a valid love interest (all custom NPCs and any edited vanilla NPCs) will be paired with the opposite sex at random.
If true, NPCs are paired with the opposite sex at random.

**Allow Tourist Dancers** (default: false)
If false, NPCs that do not live in the valley will be excluded from the dancer pools. This is calculated using the NPC's "homeRegion" value.
If true, valid NPCs living anywhere are eligible to participate in the dance.

**Maximum Dance Pairs** (default: 6)
The maximum number of dancer that can be seen dancing. Maximum 17, minimum 0.

**NPC Blacklist**
A list of NPCs to be excluded from selection for a dancer line. To configure the blacklist, add the desired NPCs to the string, separated by a single forward slash "/", like so: "Maru/Shane/Emily"

## Incompatibilities/Bugs

This mod will likely break when used alongside any mod that accesses the Event.setUpFestivalMainEvent method.

## Integrations/Compatible Mods

This mod currently contains integrations for the following mods:

[Generic Mod Config Menu by spacechase0](https://www.nexusmods.com/stardewvalley/mods/5098)

This mod is compatible and works as-intended with the following mods:

[Flower Dancing by Kelly2892](https://www.nexusmods.com/stardewvalley/mods/8107)

## Future Development

Further updates will include:
* Handling for non-binary NPCs (ie. NPC "gender" = 2)
* Mixed-gendered dance lines (male NPCs dancing in the top row with female NPCs, and vice-versa), with custom sprites
* Detailed positioning mechanics (thanks for the idea, Maclimes (Nexus)!)
* Crowd animations (thanks for the idea, foggywizard (discord)!)

Some of these features may exist partially in the mod files, or visible in the mod config menu/json. They are unlikely to work, as they are either incomplete or improperly implemented.

## Acknowledgements

* [Kelly2892](https://github.com/kenny2892/StardewValleyMods) - for example code regarding harmony implementation
* matt123337/mdbell - for mentorship, and listening to my gripes
* [The Stardew Valley Wiki's Modding Guide](https://stardewvalleywiki.com/Modding:Modder_Guide/Get_Started) - for general information and laying out step-by-step how to write a mod
* Stardew Valley Discord Server- for answering quick questions and providing inspiration

Custom NPCs in the screenshot are from [Ridgeside Village](https://www.nexusmods.com/stardewvalley/mods/7286)!

## Update Logs

### Version [1.2.1] - July 2 2021
* Adds ModDrop update key in preparation for move to hosting on ModDrop

### Version [1.2.0] - June 22 2021
* Adds expanded dance capacity, up to 17 pairs (34 NPCs total)
* Outlines code for custom animation insertion.

### Version [1.1.0] - May 3 2021
* Added custom love interest pairing method
* Fixed blacklist bug

### Version [1.0.1] - Apr 17 2021
* Added Nexus Mods update key
* Added filter to prevent the selection of children

### Version [1.0.0] - Apr 16 2021
* First release
