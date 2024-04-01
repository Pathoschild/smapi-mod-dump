**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Eating animations

You can give your items a custom eating animation, or a custom drink color.

# Contents
* [Default content](#default-animations)
* [Changing drink color](#drink-color)
* [Setting animations](#setting-an-animation)
   * [From CustomFields](#via-custom-fields)
   * [From Data](#via-mod)
* [Creating new animations](#creating-new-animations)
    * [Format](#format)
    * [Farmer frames](#farmer-frames)
    * [Example](#example)

--------------------

## Default animations

The mod comes with a few presets for custom animations. Those are:
- base_eat
- base_drink
- base_badfood

## Drink color

You can keep the drinking animation, while only changing the mug color: For that, edit the mod's custom fields with the color you want. (Can be a name, but a hex color is recommended).

```jsonc
"CustomFields": {
  "mistyspring.ItemExtensions/DrinkColor": "green"
}
```

With this, it should work automatically.

## Setting an animation

You can set them in two ways: Via CustomFields, or via Mod data.

### Via custom fields

Just add this to your Object's `CustomFields`:  `mistyspring.ItemExtensions/EatingAnimation` . The ID must exist in the mod's animation data. 

For information on that, see [how to create new animations](#creating-a-new-animation), or [default animations](#default-animations).

You can also set an animation to run after eating the item, with `mistyspring.ItemExtensions/AfterEatingAnimation`

### Via mod

You can create custom eating animations by adding them to `Mods/mistyspring.ItemExtensions/EatingAnimations`.
The animation name can be anything, but it's recommended it starts with your mod Id.

(Advanced) Eating animations can also be set via `/Data`'s `Eating` or `AfterEating` fields. If you're doing multiple changes, this might be more useful.

## Creating new animations

To make a new animation, you must understand how [the farmer sprite](https://stardewvalleywiki.com/Modding:Farmer_sprite) works.

### Format

This type follows the following format, **all are optional except Animation**:

| name          | type            | description                                                             |
|---------------|-----------------|-------------------------------------------------------------------------|
| Animation     | `FarmerFrame[]` | Farmer frames.                                                          |
| Food          | `FoodAnimation` | Animation to perform for the food. If omitted, the item won't be shown. |
| HideItem      | `bool`          | Doesn't show food. Mutually exclusive with `Food`.                      |
| Emote         | `int`           | Emote to do after eating.                                               |
| ShowMessage   | `string`        | Shows this as message.                                                  |
| PlaySound     | `string`        | Plays the given sound.                                                  |
| PlayMusic     | `string`        | Changes the music to this track.                                        |
| SoundDelay    | `int`           | Delay for sound played.                                                 |
| TriggerAction | `string`        | Id of trigger action to run.                                            |

### Farmer frames

Each frame follows this format:

| name         | type   | required | description                   |
|--------------|--------|----------|-------------------------------|
| Frame        | `int`  | Yes      | Frame in sheet.               |
| Duration     | `int`  | Yes      | Time to show frame for.       |
| SecondaryArm | `bool` | No       | If to use secondary arm type. |
| Flip         | `bool` | No       | Flip sprite horizontally.     |
| HideArm      | `bool` | No       | Hides arms.                   |

### Food animation

Food animation moves the item around the scren, using the farmer's face as starting point.

It has these fields:

| name          | type      | required | description                                       |
|---------------|-----------|----------|---------------------------------------------------|
| Duration      | `float`   | Yes      | Time the animation lasts.                         |
| Motion        | `Vector2` | Yes      | How to move the food across the screen.           |
| CustomTexture | `string`  | No       | Custom texture (optional)                         |
| Frames        | `int`     | No       | Frames, in case of animating with custom texture. |
| Loops         | `int`     | No       | Times to loop animation.                          |
| Delay         | `int`     | No       | Time before starting.                             |
| Scale         | `float`   | No       | Food sprite's scale.                              |
| Crunch        | `bool`    | No       | Whether to show food being crunched at the end.   |
| Color         | `string`  | No       | Tint to apply.                                    |
| Transparency  | `float`   | No       | Food transparency, if any.                        |
| Rotation      | `float`   | No       | Sprite rotation.                                  |
| Offset        | `Vector2` | No       | Offset from face.                                 |
| Flip          | `bool`    | No       | Whether to flip horizontally.                     |
| StartSound    | `string`  | No       | Sound to make when starting animation.            |
| EndSound      | `string`  | No       | Sound to make when finishing animation.           |
| Speed         | `Vector2` | No       | Speed to move it at (different for X and Y).      |
| StopX         | `int`     | No       | Stop moving horizontally after X pixels.          |
| StopY         | `int`     | No       | Stop moving vertically after Y pixels.            |

### Example

Here is an example of a custom animation.

```jsonc
{
  "Action": "EditData",
  "Target": "Mods/mistyspring.ItemExtensions/EatingAnimations",
  "Entries": {
    "{{ModId}}_tea": {
      "Animation": [
        {
          "Frame":0,
          "Duration": 200,
          "SecondaryArm": false,
          "Flip": false
        },
        {
          "Frame":18,
          "Duration": 150,
          "SecondaryArm": false,
          "Flip": false
        },
        {
          "Frame":26,
          "Duration": 200,
          "SecondaryArm": false,
          "Flip": false
        },
        {
          "Frame":25,
          "Duration": 300,
          "SecondaryArm": false,
          "Flip": false
        },

        {
          "Frame":68,
          "Duration": 150,
          "SecondaryArm": false,
          "Flip": false
        },

        {
          "Frame":86,
          "Duration": 200,
          "SecondaryArm": false,
          "Flip": false
        },
        {
          "Frame":103,
          "Duration": 200,
          "SecondaryArm": false,
          "Flip": false
        },
        {
          "Frame":86,
          "Duration": 200,
          "SecondaryArm": false,
          "Flip": false
        },
        {
          "Frame":103,
          "Duration": 500,
          "SecondaryArm": false,
          "Flip": false
        }
      ],
      "Food": {
        //time - in milliseconds
        "Duration": 500,
        "Delay": 350,

        //sounds
        "StartSound": "dwop",
        "EndSound": "gulp",
        "Crunch": false,

        //position
        "Offset": "25, 40",
        "Flip": true,
        "Scale": 0.8,

        //movement
        "Speed": "0, 0.02",
        "Motion": "0.0, -0.9",
        "StopX": 0,
        "StopY": -5
      }
    }
  }
}
```