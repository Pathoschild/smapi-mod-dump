**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ichortower/SecretNoteFramework**

----

# Secret Note Framework

This Stardew Valley mod is a framework allowing mod authors to add Secret Notes
to the game, with flexible options and without worrying about compatibility
with other mods.


## Why?

Secret Notes are one of the few types of game data left in Stardew Valley 1.6
that are required to have integer IDs. Right away, this means any two mods that
wish to add secret notes face a coordination problem, but there are some other
issues too:

* the ID is overloaded and also determines the type of note it is (any ID >=
  1000 is a Journal Scrap and spawns in the Island location context; any lower
  ID spawns elsewhere)
* the ID also determines the note's label in the collection menu
* all secret notes must share the same image texture (for picture notes)
* the secret notes tab in the collection menu knows there are only 38 notes,
  and is set up to draw only one page
* I probably forgot some, secret notes are not ideal

I thought it would be fun to use secret notes to add some extra lore to [one of
my other mods](https://github.com/ichortower/HatMouseLacey); when I discovered
these infelicities, I sidetracked myself into making this framework.


## How to Use

As a user, you will need SMAPI 4.0+ and Stardew Valley 1.6+. Just install this
mod like any other (unzip it into your Mods folder), and your other mods that
depend on it will do their work.

As a mod author, this framework provides a data asset which your mod should
edit in order to add entries for your secret notes. You can use SMAPI's content
API or [Content
Patcher](https://github.com/Pathoschild/StardewMods/tree/stable/ContentPatcher)
for this. I may add content pack support or a C# API in the future, if there is
demand for it, but I suspect using Content Patcher will suffice for most
authors.

A more detailed explanation of how to use this mod's features is in the [author
guide](author-guide.md).
