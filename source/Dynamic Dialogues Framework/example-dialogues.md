**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/DynamicDialogues**

----

## Contents
* [Dialogue examples](#dialogue-examples)
  * [From-To (time condition)](#using-from-to)
  * [Remove dialogue if NPC leaves](#using-clearonmove)
  * [Overriding dialogue](#using-override)
  * [Adding an animation](#using-animation)


## Dialogue examples

As mentioned before, these are the options you can use for dialogues:

name | description
-----|------------
Time | (\*) Time to set dialogue at. 
From | (\*\*) Min. time to apply dialogue at.
To | (\*\*) Max time to apply dialogue at
Location | (\*) Name of the map the NPC has to be in. 
Dialogue | The text to display.
ClearOnMove | (Optional) If `true` and dialogue isn't read, it'll disappear once the NPC moves. 
Override | (Optional) Removes any previous dialogue.
Force | (Optional) Will show this NPC's dialogue even if you're not in the location.
IsBubble | (Optional) `true`/`false`. Makes the dialogue a bubble over their head.
Jump | (Optional) If `true`, NPC will jump. 
Shake | (Optional) Shake for the milliseconds stated (e.g Shake 1000 for 1 second).
Emote | (Optional) Will display the emote at that index ([see list of emotes](https://docs.google.com/spreadsheets/d/18AtLClQPuC96rJOC-A4Kb1ZkuqtTmCRFAKn9JJiFiYE/edit#gid=693962458))
FaceDirection | (Optional) Changes NPC's facing direction. allowed values: `0` to `3` or `up`,`down`,`left`,`right`.
Animation | (Optional) Adds momentary animation.


\* = You need either a time or location (or both) for the dialogue to load.

\*\* = Mutually exclusive with "Time". Useful if you want a dialogue to show up *only* when the player is present.

------------

### Using From-To
From-To will only apply the changes when the player is present, and when the time fits the given range.
The time can be anywhere between 610 and 2550. 

_"Why not earlier/later?"_: The mod adds dialogue when time changes. 
- When a day starts (6 am), no time change has occurred yet. 
- At 2600 the day ends, so you wouldn't get to see the dialogues (most they'd do is load, and immediately get discarded by the game).

**Example**: 
Let's say you want Willy to jump and say something- *only* between 610 - 8am and at the beach. The patch would look like this:

```
"fishEscaped": {
          "From": 610,
          "To": 800,
          "Location": "Beach",
          "Dialogue": "Argh! The fish escaped!",
          "IsBubble": true,
          "Jump": true,
        },
```

So, if the player enters the beach (between the specified time), willy will do this. 

------------

### Using ClearOnMove
This option is specific to "box" dialogues (ones you have to click to see). If used with `"IsBubble": true`, it won't do anything.

Basically, it will remove a dialogue if the NPC moves. This is useful if you need the dialogue to disappear once the npc changes locations / to avoid "out of context" messages.

**Example:**
This makes Leah say something at Pierre's. If she starts walking (e.g to exit), the dialogue will be removed.
```
"pricesWentUp": {
          "Location": "SeedShop",
          "Dialogue": "Hi, @. Buying groceries too?#$b#...Did the prices go up?$2",
          "ClearOnMove": true,
        },
```
------------

### Using Override
This option is for "box" dialogues (ones you have to click to see). If used with `"IsBubble": true`, it won't do anything.

It will force the dialogue to be added- regardless of the current dialogue. Useful if you want the NPC to have a dialogue mid schedule <u>animation</u>.
**Note:** This will remove any previous dialogue, so use with caution.

**Example:**
If you want Emily to say something when she's working at Gus', you'll need to use Override. (Otherwise, the dialogue will get "buried" under the schedule one).
```
"SaloonTime": {
          "Location": "Saloon",
          "Dialogue": "Did you come buy something?",
          "Override": true,
        },
```
------------

### Using Animation
"Animation" will animate the character once.
This will work as long as the character isn't moving already. 
(e.g: if you try to make Harvey animate during aerobics, it won't work- because he's already "moving". Similarly, if a character is walking somewhere the animation won't be applied (since this would mess up the entire sprite).

When time changes again, the animation is removed.

\* You must set "Enabled" to `true` inside animations. This can have any animation, even vanilla ones.
\** You must use a valid frame- if you choose a frame that doesn't exist, it will cause errors (this is part of in-game errors / something i can't do anything about.) 
- Explanation: Since some mods add *extra* animations to sprites, the framework has no way of knowing what the max frame is.

name | description
-----|------------
Enabled | Whether to enable animations.
StartingFrame | The frame to start the animation from.
AmountOfFrames | The frames (counting the starting one) to include.
Interval | Milliseconds to show every frame for.

Frames start at 0, from the top left (and continue to the right, then the next row. Located in the game's `Content/Characters` folder).
If you need help, see [here](https://stardewvalleywiki.com/Modding:NPC_data#Overworld_sprites).

**Example:** 
When at the beach, Alex will momentarily play with the gridball.
```
"gridBall": {

          "Location": "Beach",
          "Dialogue": "This is fun.",
          "IsBubble": true,
          "Animation": 
          {
            "Enabled": true,
            "StartingFrame": 16,
            "AmountOfFrames": 7,
            "Interval": 150,
          }
}
```
