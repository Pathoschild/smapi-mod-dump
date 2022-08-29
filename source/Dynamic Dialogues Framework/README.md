**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/DynamicDialogues**

----

# DynamicDialogues
A framework which allows for dynamic dialogues throughout the day.


## Contents
* [Features](#features)
* [How to use](#how-to-use)
  * [Adding dialogues](https://github.com/misty-spring/DynamicDialogues/blob/main/docs/creating-dialogues.md)
  * [Adding greetings](https://github.com/misty-spring/DynamicDialogues/blob/main/docs/creating-greetings.md)
  * [Adding questions](https://github.com/misty-spring/DynamicDialogues/blob/main/docs/creating-questions.md)
  * [Adding notifications](https://github.com/misty-spring/DynamicDialogues/blob/main/docs/creating-notifs.md)
  * [Adding quests](https://github.com/misty-spring/DynamicDialogues/blob/main/docs/creating-quests.md)
* [Known issues](#known-issues)


## Features
- Custom notifications
- Custom npc dialogues throughout the day
  - Dialogues have several configuration options. 
  - Both of these are time and/or location dependant.
- Custom greetings (when npcs say hi to each other)
- Custom questions (when there's no more dialogue for the day).
- A random pool of dialogue (when all have been exhausted).
- Adding quests via dialogue.

This mod makes use of ContentPatcher to be edited.

------------

## How to use
Every NPC has its own dialogue file- this is made by checking NPCDispositions when the save is loaded.
So it's compatible with custom NPCs of any type.

Notifications are all in a single file, and so are Greetings (see adding [notifs](#adding-notifications) or [greetings](#adding-greetings) for more info).

If the NPC hasn't been unlocked yet (e.g kent or leo), their entries will be skipped until the player meets them.
**Note:** ALL files are reloaded when the day starts.

### Tutorials are linked [here](#Contents).

------------

## Known issues
None, as of now.

(Keep in mind, this framework updates its information once per game day. So, edits added OnLocationChange/OnTimeChange won't be applied.
The framework contains a time and location condition (for the dynamic content), so this is not a problem).

------------

## For more information
You can send me any question via [nexusmods](https://www.nexusmods.com/users/130944333) or in here.
