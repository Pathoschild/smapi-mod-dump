**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ribeena/StardewValleyMods**

----

← [author guide](../author-guide.md)

These options can be accessed from Harvey's Hospital Clinic menu, under the face tab.

## Contents
* [Introduction](#introduction)
* [Faces](#faces)
* [Eyes](#eyes)
* [Nose](#nose)
* [Ears](#ears)

## Introduction
After creating your [content JSON file](../author-guide.md#body-parts), you'll need to add
entries for each of the options.

Sometimes looking at the [examples](https://www.nexusmods.com/stardewvalley/mods/12893?tab=files#file-container-optional-files) make it easier!

## Faces
The face options draw over the body and require 2 images to each face,
a with hair [Face sprite](../../assets/Character/face.png), and a [Bald Face sprite](../../assets/Character/face_bald.png).
Because the faces are slightly taller for 'male' and shorter for 'female' these
images need to be specific to a gender (can only have an entry under `male` or `female`).

The images are 128x672, and are mostly the same structure as the Vanilla Farmer Sprite image. The
last 32 pixel column has the new [Eyes](#eyes) system images included, so make sure to draw the
different stages of blinking and looking different ways. The image uses the [Color Palette](color-palette.md)
which means you can include 5 shades of skin etc.

Once you've made your image, you'll need an entry in the content.json;
```
...
  "male": {
    ...
    "faces": {
      "Femme":"femme",
    },
    ...
  }
...
```
In this example, a male face has been added which needs the normal `femme.png` and a `femme_bald.png` file.
The folder will look like;
```
📁 Mods/
   📁 [DB] YourModName/
      🗎 content.json
      🗎 manifest.json
      📁 assets/
         📁 faces/
            🗎 femme.png
            🗎 femme_bald.png
            ...
         ...
```
That's it! Coding done.

## Eyes
The eye options draw over the face, after the nose. Because the faces are slightly taller for 'male'
and shorter for 'female' these
images need to be specific to a gender (can only have an entry under `male` or `female`).

The images are 128x672, and are mostly the same structure as the Vanilla Farmer Sprite image. The
last 32 pixel column includes a new slightly larger system for the eye blinks, going in this order;
* top-right closed eyes
* top-left skin cover (to hide the exisiting eye image)
* looking right
* looking left (provides overlay on character for dual colored eyes)
* partially blinked
* shocked

The image uses the [Color Palette](color-palette.md), if you don't follow
these colors the eyelash, dual eye color, customisable sclera won't work. Make
sure to check you have the colors right as they aren't exactly the same as
Vanilla to avoid conflicts.

Once you've made your image, you'll need an entry in the content.json;
```
...
  "female": {
    ...
    "eyes": {
      "Simple":"f_simple",
    ...
  }
...
```
In this example, a female eyes option has been added which needs a `f_simple.png` file.
The folder will look like;
```
📁 Mods/
   📁 [DB] YourModName/
      🗎 content.json
      🗎 manifest.json
      📁 assets/
         📁 eyes/
            🗎 f_simple.png
            ...
         ...
```
That's it! Coding done.

## Nose
The nose options draw over the face before the eyes. Because the faces are slightly taller for 'male'
and shorter for 'female' these
images need to be specific to a gender (can only have an entry under `male` or `female`).

The images are 96x672, and the same structure as the Vanilla Farmer Sprite image. 
The image uses the [Color Palette](color-palette.md), the extra skin colors
can help create more definition, and more importantly the magenta color
can help hide pixels from the base image.

Once you've made your image, you'll need an entry in the content.json;
```
...
  "male": {
    "nose": {
      ...
      "Big": "big",
      ...
    }
    ...
  },
...
```
In this example, a male eyes option has been added which needs a `big.png` file.
The folder will look like;
```
📁 Mods/
   📁 [DB] YourModName/
      🗎 content.json
      🗎 manifest.json
      📁 assets/
         📁 nose/
            🗎 big.png
            ...
         ...
```
That's it! Coding done.

## Ears
The nose options draw over the face, after eyes, nose and arms. Because the faces are slightly taller for 'male'
and shorter for 'female' these
images need to be specific to a gender (can only have an entry under `male` or `female`).

The images are anything from 96x672 to 288x672, and the same structure as the Vanilla Farmer Sprite image. 
By using wider images, and drawing the ears in the arms section, your ears will appear over
the hair. The image uses the [Color Palette](color-palette.md), the extra skin colors
can help create more definition.

Once you've made your image, you'll need an entry in the content.json;
```
...
  "female": {
    "ears": {
      ...
      "Pointy": "pointy",
      ...
    }
    ...
  },
...
```
In this example, a male eyes option has been added which needs a `pointy.png` file.
The folder will look like;
```
📁 Mods/
   📁 [DB] YourModName/
      🗎 content.json
      🗎 manifest.json
      📁 assets/
         📁 ears/
            🗎 pointy.png
            ...
         ...
```
That's it! Coding done.