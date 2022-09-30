**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ribeena/StardewValleyMods**

----

← [author guide](../author-guide.md)

## Contents
* [Introduction](#introduction)
* [How recoloring works](#recoloring-notes)
* [How layers works](#layers)
* [Animation](#animation-frames-when-walking-running-or-riding)
* [Making larger trinkets](#bigger-trinkets)

## Introduction
After creating your [Trinkets folder, JSON file and image](../author-guide.md#trinkets), that's really all you need to
do to create a new trinket.

## Recoloring Notes
The recoloring system for the trinkets uses two color set, greys and magentas;
* for primary color changes, the RGB must be the same color
* for secondary color changes, the RB must be the same color, and the G must be zero

If you want to make a similar grey or magenta, simple change one number a bit,
then the recoloring system shouldn't recognise the color.

## Layers
There are currently 5 different layers for rendering, so when making your JSON file
carefully choose which layers make sense for your trinket, in some cases [0,1,2,3,4] is
fine, but others, like lipstick/blush may only be on layer 0;

The layers;
0.  Above shirts, below beards
1.  Above beards, below vanilla accessories (and hair)
2.  Above hair, below hat
3.  Above hat, below arms
4.  Above arms (everything!)

## Animation Frames when walking, running or riding
The `anim_frames` is a set of number which matches the sprite in the row to a particular frame
of animation. It works the same as the [hair animation frames](hair.md#animation-frames-when-walking-running-or-riding).

## Bigger trinkets
You can make your trinkets a bit bigger than the default 16 pixels by using a few extra options;
```
﻿{
  "trinkets": {
    "Rainbow Scarf": {
      "usesUniqueLeftSprite": false,
      "layers": [2,3,4,5],
      "extraWidth": 16,
      "primaryColor": false,
      "secondaryColor": false,
      "cost": 150,
      "anim_frames": {
        "Walk": { 0: 0, 1:1, 2:0, 3:2 },
        "Run": { 0: 0, 1:0, 2:1, 3:1, 4:0, 5:0, 6:2, 7:2 },
        "Ride": { 0:1, 1:0, 2:2, 3:2, 4:0, 5:1 },
      }
    },
    "Huge Crown": {
      "usesUniqueLeftSprite": false,
      "layers": [5],
      "yOffset": -5,
      "primaryColor": true,
      "secondaryColor": false,
    },
  }
}
```
`yOffset` allows you to move the trinket up if needed, and `extraWidth` let's DynmicBodies know that
your sprite is bigger than 16pixels, eg `"extraWidth": 8` would mean you have a 24 pixel width
trinket. It will always centre it though!