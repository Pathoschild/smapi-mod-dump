**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/drbirbdev/StardewValley**

----

# BirbSharedSpaceCore

This is a shared library for SpaceCore dependencies.

It's mostly just for custom skills, including boilerplate for creating a custom skill and professions.

BirbSkill is an all-purpose generic custom skill.  You only need to define a few things

* id - the unique string representing this skill
* texture - an IRawTextureData holding all UI textures for this skill and professions
* modHelper - the calling mods IModHelper
* professionTokens - a Dictionary<string, object> representing profession names and translation tokens (if any)

Optionally, you can include two properties

ExtraInfo:

A function getting the level up screen info for a given level, ie perks.

HoverText:

A function getting the UI hover text at a given level.

The profession names are presumed from the professionTokens dictionary, even if their translation text requires no tokens, so they must be included.

The above is all that's needed to have a skill and professions registered with SpaceCore and MARGO.  Game logic for how the skill and professions affect the game should be handled by the individual mod.

To get whether a player has a custom profession, you can use the Farmer `HasProfession` method.  This can also check MARGO prestiged professions.
