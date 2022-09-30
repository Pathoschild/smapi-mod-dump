**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ribeena/StardewValleyMods**

----

← [author guide](../author-guide.md)

## Contents
* [Introduction](#introduction)
* [How recoloring works](#recoloring-notes)
* [Animation](#animation-frames-when-walking-running-or-riding)
* [Making larger hair](#bigger-hair)
* [Beards](#beards)

## Introduction
After creating your [Hair folder and JSON file](../author-guide.md#hair), that's really all you need to
do for creating a replacement or new hair style.

## Recoloring Notes
If you want your dual hair colours to recolor well, there are some color choices to consider. Make sure that;
*   You use only grey colors (if RGB aren't the same value, the pixel won't recolor)
*   Transparent hair Alpha is more solid than 99 (or 39%)
*   Greys brighter than 240 will go lighter than the user selected hair color (for highlights/shine)
*   Greys darker than 55 will go darker than the user selected dark hair color (for guarenteed deep shadows)

Vanilla uses a slightly red and 36% opacity shadow, which will work fine.

## Animation Frames when walking, running or riding
The `anim_frames` is a set of number which matches the sprite in the row to a particular frame
of animation. For example `"Walk": { 0: 0, 1:1, 2:0, 3:2 },` - the walking animations have 4 frames,
and this hair has 3 different drawings horizontally showing, so on the;
1.  first frame (0) it will show the first sprite (hair down)
2.  second frame (1) it will show the second sprite (hair flick to the left)
3.  third frame (2) it will show the first sprite again
4.  fourth frame (3) it will show the third sprite (hair flick to the right)

Running has 8 frames, so you can double up if you aren't doing a lot of animating. Riding unforutnately
uses 5 frames (that's how the horse is animated), so it needs some creative license.

You can also define the animations for each direction, `"WalkLeft"`/`"WalkUp"`/`"WalkRight"`/`"WalkDown"`,
make sure to do this for all 4 directions. It would look something like;
```
﻿{
  "hairStyles": {
    "Messy Ponytail": {
      "usesUniqueLeftSprite": false,
      "isBaldStyle": false,
      "extraWidth": 8,
      "anim_frames": {
        "WalkLeft": { 0: 0, 1:1, 2:0, 3:1 },
        "WalkRight": { 0: 0, 1:1, 2:0, 3:1 },
        "WalkDown": { 0: 0, 1:0, 2:0, 3:0 },
        "WalkUp": { 0: 0, 1:1, 2:0, 3:2 },
        "RunLeft": { 0: 0, 1:1, 2:1, 3:2, 4:0, 5:1, 6:2, 7:1 },
        "RunRight": { 0: 0, 1:1, 2:1, 3:2, 4:0, 5:1, 6:2, 7:1 },
        "RunDown": { 0: 0, 1:0, 2:0, 3:0, 4:0, 5:0, 6:0, 7:0 },
        "RunUp": { 0: 0, 1:1, 2:1, 3:0, 4:0, 5:2, 6:2, 7:0 },
        "RideLeft": { 0: 1, 1:1, 2:2, 3:2, 4:1, 5:0 },
        "RideRight": { 0: 1, 1:1, 2:2, 3:2, 4:1, 5:0 },
        "RideDown": { 0: 0, 1:0, 2:0, 3:0, 4:0, 5:0 },
        "RideUp": { 0: 1, 1:1, 2:0, 3:2, 4:0, 5:0 },
      }
    },
  }
}
 ```

## Bigger hair!
You can make your hair a bit bigger by using a few extra options;
```
﻿{
  "hairStyles": {
    "8": {
      "usesUniqueLeftSprite": true,
      "isBaldStyle": false,
      "yOffset": -16,
      "anim_frames": {
        "Walk": { 0: 0, 1:1, 2:2, 3:1 },
        "Run": { 0: 0, 1:0, 2:1, 3:1, 4:2, 5:2, 6:1, 7:1 },
        "Ride": { 0: 1, 1:0, 2:1, 3:2, 4:1, 5:0 },
      }
    },
    "24": {
      "usesUniqueLeftSprite": false,
      "isBaldStyle": false,
      "extraWidth": 16,
      "anim_frames": {
        "Walk": { 0: 0, 1:2, 2:4, 3:6 },
        "Run": { 0: 0, 1:1, 2:2, 3:3, 4:4, 5:5, 6:6, 7:7 },
        "Ride": { 0:0, 1:1, 2:2, 3:4, 4:5, 5:6 },
      }
    },
  }
}
```
`yOffset` allows you to move the hair up if needed, and `extraWidth` let's DynmicBodies know that
your hair spirte is bigger than 16pixels, eg `"extraWidth": 8` would mean you have a 24 pixel width
hair. It will always centre it though!

## Beards
Beards work a bit different to hair currently, they are a 16x96 pixel file following
the [same recoloring](#recoloring-notes) method. The sprite are looking down, looking right and looking up. The looking
right sprite is flipped to make the left facing version.

Once a file is made, add an entry to the JSON file;
```
{
  "unisex": {
    "beards": {
      "Long beard":"long",
      ...
    }
  }
}
```
Above in the `unisex` section, a 'long.png' file has been added. The folder will look like;
```
📁 Mods/
   📁 [DB] YourModName/
      🗎 content.json
      🗎 manifest.json
      📁 assets/
         📁 beards/
            🗎 long.png
            ...
         ...
```
