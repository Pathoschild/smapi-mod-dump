**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# DynamicDialogues
A framework which allows for dynamic dialogues throughout the day.

Aquí puedes encontrar la [versión en español.](https://github.com/misty-spring/StardewMods/tree/main/DynamicDialogues/docs/es/README.md)


## Contents
* [Features](#features)
* [Tutorials](#how-to-use)
  * [Adding dialogues](https://github.com/misty-spring/StardewMods/tree/main/DynamicDialogues/docs/creating-dialogues.md)
    * [Random dialogue](https://github.com/misty-spring/StardewMods/tree/main/DynamicDialogues/docs/creating-randomized-text.md) 
    * [Adding greetings](https://github.com/misty-spring/StardewMods/tree/main/DynamicDialogues/docs/creating-greetings.md)
    * [Adding questions](https://github.com/misty-spring/StardewMods/tree/main/DynamicDialogues/docs/creating-questions.md)
  * [Adding notifications](https://github.com/misty-spring/StardewMods/tree/main/DynamicDialogues/docs/creating-notifs.md)
  * 1.6 only:
    * [New event commands](https://github.com/misty-spring/StardewMods/tree/main/DynamicDialogues/docs/event-commands.md)
    * [New game state queries](https://github.com/misty-spring/StardewMods/tree/main/DynamicDialogues/docs/game-state-queries.md)
    * [New trigger actions](https://github.com/misty-spring/StardewMods/tree/main/DynamicDialogues/docs/trigger-actions.md)
    * [Overriding archaeology gift taste](https://github.com/misty-spring/StardewMods/tree/main/DynamicDialogues/docs/arch-gift-taste.md)
* [Known issues](#known-issues)


## Features
- Custom notifications
- Custom npc dialogues throughout the day
  - Dialogues have several configuration options.
  - They can also be randomized.
- Custom greetings between NPCs
- Custom questions (when there's no more dialogue for the day).
- Custom event commands
- Items can affect one another.
- A lot more.

This mod makes use of ContentPatcher to be edited.

------------

## How to use
Every NPC has its own dialogue file- this is made by checking NPCDispositions when the save is loaded.
So it's compatible with custom NPCs of any type.

Notifications are all in a single file, and so are Greetings (see adding [notifs](#adding-notifications) or [greetings](#adding-greetings) for more info).

If the NPC hasn't been unlocked yet (e.g kent or leo), their entries will be skipped until the player meets them.

### Tutorials are linked [here](#Contents).

------------

## Known issues
None, as of now.

------------

## For more information
You can send me any question via [nexusmods](https://www.nexusmods.com/users/130944333) or in here.
