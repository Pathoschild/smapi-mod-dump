**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

## Changes:
- Doesn't need SpaceCore anymore
- Added GSQ to framework's dialogues
- Fixed custom greetings not working.

## Custom trigger actions:
- `mistyspring.dynamicdialogues_DoEvent <eventID> <location> [preconditions] [checkseen] [reset_if_not_played]`
  
  Plays an event. Can only be run from trigger actions during day start.


- `mistyspring.dynamicdialogues_SendNotification <id> <check if already sent>`

  Sends a notification. Must exist in the notifications file.


- `mistyspring.dynamicdialogues_Speak <NPC> <key> [shouldOverride]`

  Same as events' `Speak`, but using a key from character's Dialogue file instead. If `shouldOverride` is true, it will override any ongoing dialogue/menu.


- `mistyspring.dynamicdialogues_AddExp <player> <skill> <amt>`

  Adds EXP to the given player's skill.

  
## New commands:

- `if <GSQ>##<consequence>##[alternative]`
  
   Checks a GSQ, and runs command if it's true. if not (and an alternative exists), it'll run that instead.


- `append <name>`

  Appends event commands to the current one. `<name>` must exist as entry in the current `Data/Events/` file.


- `addExp <name> <amount>`

  Adds EXP to the given skill. (must be vanilla)


- `addFire <x> <y> [extra X pixels] [extra Y pixels]`

  Adds a fire TAS at the given position, extra X/Y can be used for centering on bigger objects


- `objectHunt <ID>`

  Lets you create object hunts, akin to egg hunt and haley's event. To make these, see [the documentation](https://github.com/misty-spring/StardewMods/blob/main/DynamicDialogues/docs/creating-objecthunts.md).


- `health <set/add> <amount>`
  Lets you set/add to player health, depending on what you chose.


- `stamina <set/add> <amount>`
  Lets you set/add to player health, depending on what you chose.


- `setDating <who> [breakup]`
  Changes the relationship with a NPC to dating. If `breakup` is true, they'll break up like a bouquet would.
  (Note: if they're not datable, this will do nothing.)


- `end lastSleepLocation`
  Ends the event and returns to the last sleeping location, on the last slept bed.


- `end warp [x] [y]`
  Ends the event and warps to desired location. If no arguments are given, the default entry point will be used.


- `end house` / `end farmhouse`
  Ends the event and returns to the farm/farmhouse (as given), on the default entry point.
  
## Custom GSQs

- `mistyspring.dynamicdialogues_PlayerWearing <player> <type> <item>`

  Checks if the player is wearing the given item. For example, `Current Hat CustomHat` will check if current player is wearing a hat with ID CustomHat.


- `mistyspring.dynamicdialogues_ToolUpgrade <player> <tool> [min] [max] [recursive]`

  Checks if the given tool is in the upgrade range. Must be in inventory- if `recursive`, it'll search on chests too.

  For example, `Any Axe 1 3 true` will check whether any player has an axe between copper and gold.

## Overriding arch taste
By default, all NPCs will dislike arch-category this (even if you put the item on Love). You can override this behavior by giving the item the context tag "override_arch_taste"