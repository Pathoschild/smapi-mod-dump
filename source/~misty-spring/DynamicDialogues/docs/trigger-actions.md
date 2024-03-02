**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Custom trigger actions

## Contents
* [Adding EXP](#adding-experience-points)
* [Making NPCs speak](#Making-characters-speak)
* [Playing Events](#playing-events)
* [Sending notifications](#sending-notifications)

------------------

## Adding experience points
`mistyspring.dynamicdialogues_AddExp <player> <skill> <amt>`

Adds EXP to the given player's skill.

The skill must be one of vanilla:
- farming
- fishing
- foraging
- mining
- combat
- luck

### Example

`mistyspring.dynamicdialogues_AddExp All farming 20`

For all players, this will add 20 exp points to their farming skill.

-----------------

## Making characters speak
`mistyspring.dynamicdialogues_Speak <NPC> <key> [shouldOverride]`

Same as events' `Speak`, but using a key from character's Dialogue file instead. If `shouldOverride` is true, it will override any ongoing dialogue/menu.

### Example
`mistyspring.dynamicdialogues_Speak Krobus Fri false`
Here, Krobus will say his `Fri` line ("...") as long as there's no open menu or dialogue.

------------------

## Playing events

`mistyspring.dynamicdialogues_DoEvent <eventID> <location> [preconditions] [checkseen] [reset_if_not_played]`

Plays an event. __Can only be run__ from trigger actions __during day start.__

### Parameters

| name                | type   | required | description                                                                    |
|---------------------|--------|----------|--------------------------------------------------------------------------------|
| eventID             | string | yes      | Event ID to play.                                                              |
| location            | string | yes      | Where the event takes place.                                                   |
| preconditions       | bool   | no       | Whether to check preconditions. Default true.                                  |
| checkseen           | bool   | no       | Whether to check if the event was already seen. Default true.                  |
| reset_if_not_played | bool   | no       | Will reset the trigger action if this event doesn't play (e.g sudden day end). |

**Example**

`mistyspring.dynamicdialogues_DoEvent MyCustomEvent ScienceHouse false true true`

Here, `MyCustomEvent` will trigger at ScienceHouse, without checking the event's conditions.

-----------------

## Sending notifications
`mistyspring.dynamicdialogues_SendNotification <id> <check if already sent>`

Sends a notification. Must exist in the notifications file.

### Example
`mistyspring.dynamicdialogues_SendNotification MyCustomNotif true`

Here, the mod searches for MyCustomNotif in `mistyspring.dynamicdialogues/Notifs`, and sends it *as long* as it hasn't been seen yet.
