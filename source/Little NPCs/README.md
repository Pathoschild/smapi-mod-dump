**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/mus-candidus/LittleNPCs**

----

# LittleNPCs

Mod for [Stardew Valley](http://stardewvalley.net/) which turns your children into little NPCs. Requires [ContentPatcher](https://www.nexusmods.com/stardewvalley/mods/1915).

**Create a content pack**

Replacement NPCs for your children must be provided by content packs. See [NPC data](https://stardewvalleywiki.com/Modding:NPC_data). Note that NPCDispositions must not be provided by your content pack, these are generated and handled internally by the mod.

The mod provides 6 ContentPatcher tokens:

* Candidus42.LittleNPCs/FirstLittleNPCName
* Candidus42.LittleNPCs/FirstLittleNPCDisplayName
* Candidus42.LittleNPCs/FirstLittleNPCGender
* Candidus42.LittleNPCs/SecondLittleNPCName
* Candidus42.LittleNPCs/SecondLittleNPCDisplayName
* Candidus42.LittleNPCs/SecondLittleNPCGender

In your content pack, use these tokens instead of hard-coded names and genders.

**Convert a ChildToNPC content pack**

Content packs for the unmaintained [ChildToNPC](https://www.nexusmods.com/stardewvalley/mods/4568) mod are supposed to be easily convertible, even though some tokens are not provided.

Remove `Data/NPCDispositions` from your content pack first. For replacing tokens see the following table:


| Child2NPC token     | LittleNPC token            | Notes                                                         |
|:--------------------|:---------------------------|:--------------------------------------------------------------|
| FirstChildName      | FirstLittleNPCName         | Internal asset name, not suitable for dialogue.               |
|                     | FirstLittleNPCDisplayName  | Name to show in dialogue.                                     |
| FirstChildBirthday  |                            | Not needed anymore. Formerly used to provide NPCDispositions. |
| FirstChildBed       |                            | Not needed anymore. Formerly used to provide NPCDispositions. |
| FirstChildGender    | FirstLittleNPCGender       |                                                               |
| FirstChildParent    |                            | Use the standard CP token {{spouse}} instead.                 |
| SecondChildName     | SecondLittleNPCName        | Internal asset name, not suitable for dialogue.               |
|                     | SecondLittleNPCDisplayName | Name to show in dialogue.                                     |
| SecondChildBirthday |                            | Not needed anymore. Formerly used to provide NPCDispositions. |
| SecondChildBed      |                            | Not needed anymore. Formerly used to provide NPCDispositions. |
| SecondChildGender   | SecondLittleNPCGender      |                                                               |
| SecondChildParent   |                            | Use the standard CP token {{spouse}} instead.                 |
| NumberTotalChildren |                            | Not needed anymore. Number of children is handled internally. |

**Config options**

* AgeWhenKidsAreModified: The age in days when a child is replaced by a LittleNPC. Default is 83 days.
* DoChildrenWander: If true, children wander around the house every hour unless they have a schedule.
* DoChildrenHaveCurfew: If true, children will head home at curfew time.
* CurfewTime: The time of curfew when DoChildrenHaveCurfew is true. Default is 1900 (7PM).
* DoChildrenVisitVolcanoIsland: Children visit Volcano Island by chance. Default is false. Note that the [CustomNPCExclusions](https://www.nexusmods.com/stardewvalley/mods/7089) mod doesn't have any influence on that because it runs before child conversion happens.
