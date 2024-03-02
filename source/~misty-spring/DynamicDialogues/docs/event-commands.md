**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----


# Event commands

## Contents

* [Add skill experience](#add-skill-experience)
* [Append](#append)
* [Change health/stamina](#change-health--stamina)
* [Dating NPC](#dating-npcs)
* [New "end" commands](#end-commands)
  * [house / farmhouse](#house--farmhouse)
  * [lastSleepLocation](#last-sleep-location)
  * [warp](#warp)
* [If/Else](#if--else)
  * [Parameters](#parameters)
* [Scenes](#pictures-in-events)
  * [Adding a scene](#adding-a-scene)
  * [Removing a scene](#removing-a-scene)


----------

## Add skill experience
`addExp <name> <amount>`

Adds EXP to the given skill (must be vanilla).
This ony affects the event player.

**Example**

To add 20 points to the combat skill: `addExp combat 20`

----------

## Append

`append <key>`

Appends event commands to the current one. The command string must exist as entry in the current `Data/Events/` file.

This is most useful when paired with [if/else](#if--else) (e.g, having events "branch out" without needing to make two events).

### Example (using if/else)
```jsonc
{
  "Action": "EditData",
  "Target": "Data/Events/IslandNorth",
  "Entries": {
    "CustomEvent/f Abigail 250": "continue/40 25/Abigail 38 26 2 farmer 41 33 0/skippable/if PLAYER_HAS_MAIL Current MyCustomFlag##append continuation##append alt/emote Abigail 16/end",
    "continuation":"pause 1500/move farmer 0 -5 0",
    "alt":"pause 1200/playsound crash/jump Abigail"
  }
}
```

Here, the event branches out: If you've received mail MyCustomFlag, `continuation` will play. Otherwise, `alt` will.

### Breakdown
The main event must follow event format (ie, start with `music/viewport/actors` and have an `end` command). In our case, this is `continue/40 25/Abigail 38 26 2 farmer 41 33 0`.

However, `append`ed events **don't** follow this format. They must only have the commands to play.

----------

## Change health / stamina
These commands let you change player's health. 

`health <set/add> <amount>`
`stamina <set/add> <amount>`

Lets you set/add to player health or stamina. If the result is 0 or less, it changes to 1 instead.

You can also refill it by using `health reset` or `stamina reset`.

**Example 1**

`health set 50`

This will set the player's health to 50.

**Example 2**

Let's say the player has 90 energy left. If we use `stamina - 100`, it will change it to 1.

(This is because -10 stamina isn't possible in-game).

**Aliases**

These are valid aliases for the value change:

| name   | alias         |
|--------|---------------|
| set    | any (default) |
| add    | `+`, `more`   |
| reduce | `-`, `less`   |
| reset  | none          |

So, both `health = 10` and `health set 10` will change current health to 10.

----------

## Dating NPCs
`setDating <who> [breakup]`

Changes the relationship with a NPC to dating. If `breakup` is true, they'll break up like a bouquet would.

(Note: if they're not datable, this will do nothing.)

----------
## End commands


### House / farmhouse
`end house` / `end farmhouse`

Ends the event and returns to the farm/farmhouse (as given), on the default entry point.

### Last sleep location
`end lastSleepLocation`

Ends the event and returns to the last sleeping location, on the last slept bed.

### Warp
`end warp <location> [x] [y]`
Ends the event and warps to desired location. If no x/y are given, the default entry point will be used.

----------

## If / Else
`if <GSQ>##<consequence>##[alternative]`

You can make an event command only play under certain conditions, using [Game State Queries](https://stardewvalleywiki.com/Modding:Game_state_queries):

**\*Note: Dialogue must have double backslash** (ie, `\\\"` instead of `\"`).

### Parameters
| name        | required | description                                                                                           |
|-------------|----------|-------------------------------------------------------------------------------------------------------|
| GSQ         | yes      | The [game state query](https://stardewvalleywiki.com/Modding:Game_state_queries) to use as condition. |
| Consequence | yes      | Command to run if conditions apply.                                                                   |
| Alternative | no       | Ran if conditions DON'T apply.                                                                        |

**Example**

`if PLAYER_COMBAT_LEVEL Current 5 ## speak Abigail \\\"Wow, you're strong!\\\"`

In this case, if the player's combat level is higher than 5, Abigail will speak. Otherwise, nothing will happen.

----------
## Pictures in events
Event scenes work just like vanilla scenes (for example, Caroline Tea or the Onions event.)

### Adding a Scene


First, you must load the scene to `mistyspring.dynamicdialogues/Scenes/<name-of-scene>`. 

Files with a height of **112** will be automatically centered on the screen.


**Example:**

```
{
  "Action": "Load",
  "Target": "mistyspring.dynamicdialogues/Scenes/MyScene",
  "FromFile": "assets/Scenes/MyScene.png"
}
```

Now that the scene is loaded, we can use it for events.

| field        | required | description                                                       |
|--------------|----------|-------------------------------------------------------------------|
| AddScene     | yes      | The command.                                                      |
| FileName     | yes      | The name of the scene to load.                                    |
| ID           | yes      | Number assigned to this scene. Recommended to use mod's UniqueID. |
| Frames\*     | no       | Used for animations. How many frames this scene has.              |
| Milliseconds | no       | Used for animations. How many milliseconds each frame will have.  |

\*= If the scene is animated, each frame must have the same size (e.g 100 width). A 5-frame scene should use a file with a width of 500.

**Example:**

Let's say our scene is a 112x112 image.

`/AddScene MyScene 12195`

This will add the scene *immediately.* 
(For a fade effect, add `/fade/viewport -300 -300/` before this command)

----------

### Removing a Scene

To remove a scene, simply use the ID we assigned.

`/RemoveScene <ID>`

This will immediately remove it from the event.

**Example:**
`/fade/RemoveScene 12195/unfade`

First, the scene will fade out. Then, the scene will be removed.
When the game unfades, our scene will be gone.
