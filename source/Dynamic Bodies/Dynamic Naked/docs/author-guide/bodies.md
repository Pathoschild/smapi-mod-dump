**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ribeena/StardewValleyMods**

----

← [author guide](../author-guide.md)

## Contents
* [Introduction](#introduction)
* [The Body](#body)
* [Body Hair](#body-hair)
* [Naked Overlays](#naked-overlays)
  * [Upper](#naked-upper-overlays)
  * [Lower](#naked-lower-overlays)

## Introduction
After creating your [content JSON file](../author-guide.md#body-parts), you'll need to add
entries for each of the options.

Sometimes looking at the [examples](https://www.nexusmods.com/stardewvalley/mods/12893?tab=files#file-container-optional-files) make it easier!

## Body
The main body file only includes the torso and leg graphics of the character. These
define the body when no shirt or pants are worn, or likewise shorts etc. It doesn't
include the feet either, to create those graphics, you'll want to make
[boots](/shoes.md) that overlay onto this.

When making you picture it's good to start off by copying the
[default one from this mod](../../asset/Character/farmer_base.png). The image is the full size
288x672 but only the first 96 pixels in width will be use, as the rest is
overwritten by subsequent features (arms, and facial features).

Once a file is made, add an entry to the JSON file;

```
...
  "male": {
    ...
    "bodyStyles": {
        "Toned": "toned",
    },
...
```
Above in the `male` section, a 'toned.png' file has been added. The folder will look like;
```
📁 Mods/
   📁 [DB] YourModName/
      🗎 content.json
      🗎 manifest.json
      📁 assets/
         📁 bodies/
            🗎 toned.png
            ...
         ...
```

## Body Hair
The body hair option [renders to the hair colors](hair.md#recoloring-notes) chosen by the player.
These are an image
96x672 pixels which bakes ontop of the body. It uses all the frames of animation
as it can cover from neck down to ankles.

Once a file is made, add an entry to the JSON file;
```
  "male": {
    ...
    "bodyHair": {
      "Lots": "lots",
      ...
    }
  }
```

Above in the `male` section, a 'lots.png' file has been added. The folder will look like;
```
📁 Mods/
   📁 [DB] YourModName/
      🗎 content.json
      🗎 manifest.json
      📁 assets/
         📁 bodyhair/
            🗎 lots.png
            ...
         ...
```
## Naked Overlays
Naked Overlays allows the player to decide how their character will look when they don't
have pants or shirts equipped. The player can change the option of whether these show
when swimming/bathing ingame in the tailoring menu.

As such these overlays could be NSFW using the [extended skin colors](color-palette.md), they could be censor
squares/items or they might just be swimming outfits or underwear. There are two versions,
one for the upper body and one for the lower body which shown when shirt or pants aren't
present.

### Naked Upper Overlays
The naked upper overlay option draws similar to shirts. These are an image
96x672 pixels (all the frames of animation) or 16x128 (faster, automatically placed)
image which draws ontop of the body.

Once a file is made, add an entry to the JSON file;
```
  "unisex": {
    "nakedUppers": {
      "Striped Bikini": {
        "name": "striped bikini",
        "options": ["no animation"]
      },
      ...
    }
  }
```

Above in the `unisex` section, a 'stripped bikini.png' file has been added. Under the 
`options` there is an optional `"no animation"`, this means this image will be the
smaller 16x128 image using sprites for facing down, right, up and left. The folder will look like;
```
📁 Mods/
   📁 [DB] YourModName/
      🗎 content.json
      🗎 manifest.json
      📁 assets/
         📁 nakedUpper/
            🗎 stripped bikini.png
            ...
         ...
```
Not including any options, this will default to the 96x672 pixel full animation.

### Naked Lower Overlays
The naked lower overlay option draws similar to pants. These are an image
96x672 pixels (all the frames of animation) image which draws ontop of the body,
and above accessories by default. Unlike upper overlays, there's no small image
version.

Once a file is made, add an entry to the JSON file;
```
  "unisex": {
    "nakedLowers": {
      "White Breifs": {
        "name": "tighty",
        "options": ["below accessories"]
      }
      ...
    }
  }
```

Above in the `unisex` section, a 'tighty.png' file has been added. Under the 
`options` there is an optional `"below accessories"`, this means this overlay
will show below hair and accessories. The folder will look like;
```
📁 Mods/
   📁 [DB] YourModName/
      🗎 content.json
      🗎 manifest.json
      📁 assets/
         📁 nakedLower/
            🗎 tighty.png
            ...
         ...
```