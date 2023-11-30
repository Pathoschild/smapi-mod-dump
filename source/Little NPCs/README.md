**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/mus-candidus/LittleNPCs**

----

# LittleNPCs

Mod for [Stardew Valley](http://stardewvalley.net/) which turns your children into little NPCs. Requires [ContentPatcher](https://www.nexusmods.com/stardewvalley/mods/1915).

**Create a content pack**

Replacement NPCs for your children must be provided by content packs. See [NPC data](https://stardewvalleywiki.com/Modding:NPC_data). Note that NPCDispositions must not be provided by your content pack, these are generated and handled internally by the mod.

Starting from version 1.1, the mod provides 2 ContentPatcher tokens:

* Candidus42.LittleNPCs/FirstLittleNPC
* Candidus42.LittleNPCs/SecondLittleNPC

Every token supports these arguments:

* Name
* DisplayName
* Gender
* BirthSeason
* BirthDay
* Age

The 1.0 tokens are still there but shouldn't be used for new development.

Only one argument can be given at a time. Token arguments are passed after a colon. For example, if you want the internal name of FirstLittleNPC, use
`{{Candidus42.LittleNPCs/FirstLittleNPC: Name}}`

In your content pack, use these tokens instead of hard-coded names and genders.

**Convert a ChildToNPC content pack**

Content packs for the unmaintained [ChildToNPC](https://www.nexusmods.com/stardewvalley/mods/4568) mod are supposed to be easily convertible, even though some tokens are not provided.

Remove `Data/NPCDispositions` from your content pack first. For replacing tokens see the following table:


| Child2NPC token     | LittleNPC 1.0 token        | LittleNPC 1.1 token and argument | Notes                                                         |
|:--------------------|:---------------------------|:---------------------------------|:--------------------------------------------------------------|
| FirstChildName      | FirstLittleNPCName         | FirstLittleNPC: Name             | Internal asset name, not suitable for dialogue.               |
|                     | FirstLittleNPCDisplayName  | FirstLittleNPC: DisplayName      | Name to show in dialogue.                                     |
| FirstChildBirthday  |                            |                                  | Not needed anymore. Formerly used to provide NPCDispositions. |
|                     |                            | FirstLittleNPC: BirthSeason      | Season of birth: spring, summer, fall or winter.              |
|                     |                            | FirstLittleNPC: BirthDay         | Day of birth: 1 to 28.                                        |
|                     |                            | FirstLittleNPC: Age              | Age of a LittleNPC in years.                                  |
| FirstChildBed       |                            |                                  | Not needed anymore. Formerly used to provide NPCDispositions. |
| FirstChildGender    | FirstLittleNPCGender       | FirstLittleNPC: Gender           |                                                               |
| FirstChildParent    |                            |                                  | Use the standard CP token {{spouse}} instead.                 |
| SecondChildName     | SecondLittleNPCName        | SecondLittleNPC: Name            | Internal asset name, not suitable for dialogue.               |
|                     | SecondLittleNPCDisplayName | SecondLittleNPC: DisplayName     | Name to show in dialogue.                                     |
| SecondChildBirthday |                            |                                  | Not needed anymore. Formerly used to provide NPCDispositions. |
|                     |                            | SecondLittleNPC: BirthSeason     | Season of birth: spring, summer, fall or winter.              |
|                     |                            | SecondLittleNPC: BirthDay        | Day of birth: 1 to 28.                                        |
|                     |                            | SecondLittleNPC: Age             | Age of a LittleNPC in years.                                  |
| SecondChildBed      |                            |                                  | Not needed anymore. Formerly used to provide NPCDispositions. |
| SecondChildGender   | SecondLittleNPCGender      | SecondLittleNPC: Gender          |                                                               |
| SecondChildParent   |                            |                                  | Use the standard CP token {{spouse}} instead.                 |
| NumberTotalChildren |                            |                                  | Not needed anymore. Number of children is handled internally. |

**Config options**

* AgeWhenKidsAreModified: The age in days when a child is replaced by a LittleNPC. Default is 83 days.
* DoChildrenWander: If true, children wander around the house every hour unless they have a schedule.
* DoChildrenHaveCurfew: If true, children will head home at curfew time.
* CurfewTime: The time of curfew when DoChildrenHaveCurfew is true. Default is 1900 (7PM).
* DoChildrenVisitVolcanoIsland: Children visit Volcano Island by chance. Default is false. Note that the [CustomNPCExclusions](https://www.nexusmods.com/stardewvalley/mods/7089) mod doesn't have any influence on that because it runs before child conversion happens.
