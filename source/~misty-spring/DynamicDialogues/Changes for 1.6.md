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
- `mistyspring.dynamicdialogues_DoEvent <eventID> <location> [checkprec] [checkseen] [reset_if_not_played]`
  
  Plays an event. Can only be run from trigger actions during day start.


- `mistyspring.dynamicdialogues_SendNotification <id> <check if already sent>`

  Sends a notification. Must exist in the notifications file.


- `mistyspring.dynamicdialogues_Speak <NPC> <key> [shouldOverride]`

  Same as events' `speak` command.


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


- `playerFind <key> [timer]` (Experimental)

  An 'object hunt'-ish command, gives the player control back temporarily until they find certain items in the map (or time runs out).
  Must be accompanied by an edit to `mistyspring.dynamicdialogues\\Commands\\objectHunt`. Documentation can be found [here](https://github.com/misty-spring/DynamicDialogues/blob/main/docs/event-commands.md#object-hunts).
  
## Custom GSQs

- `mistyspring.dynamicdialogues_PlayerWearing <player> <type> <item>`

  Checks if the player is wearing the given item. For example, `Current Hat CustomHat` will check if current player is wearing a hat with ID CustomHat.
