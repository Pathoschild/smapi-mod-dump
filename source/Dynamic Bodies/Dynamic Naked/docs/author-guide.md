**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ribeena/StardewValleyMods**

----

← [README](README.md)

This document helps mod authors create a content pack for Dynamic Bodies.

**See the [main README](README.md) for other info**.

## Contents
* [Introduction](#introduction)
  * [What is, and why, Dynamic Bodies?](#what-is-and-why-dynamic-bodies)
  * [What does a content pack look like?](#what-does-a-content-pack-look-like)
* [Get started](#get-started)
* [Features](#features)
  * [Body Parts](#body-parts)
  * [Hair and Beards](#hair)
  * [Shoes](#shoes)
  * [Shirt Overlays](#shirt-overlays)
  * [Trinkets or Accessories](#trinkets)
* [Working with other mods to provide shirts etc](author-guide/other-mods.md)
* [Changing assets for other mods](#other-mods-pipeline)
  * [Maps](#maps)
* [Translations](#translations)

## Introduction
### What is, and why, Dynamic Bodies?
Dynamic Bodies changes the way the Farmers are rendered, similar to [Fashion Sense](https://www.nexusmods.com/stardewvalley/mods/9969),
it breaks components like eyes, ears, arms, feet and bodies up into customisable parts which can be
changed by individual players and show differently for all. This mod aims to fit in with SDV so uses the cost
and friendship requirement for changing looks, and adds other similar mechanics. It's aimed to be simpler
for mod creators making vanilla-like changes, e.g. to make new arms is a few lines of json and then making the images.

In order to access Dynamic Bodies options, there are easier to use User Interfaces, with simple tools
like premade colors. As it uses a shader to render color-swapping, it allows for many extra
color swaps like two different colored eyes, 2 sets of skin tones, 5 skin tone colors, etc. Using a similar method, 
it renders hair using two colors to better match the NPCs of the game. 

There's plenty of options, [check the mod page for more](https://www.nexusmods.com/stardewvalley/mods/12893),
but why not just use Fashion Sense? I actually started trying out Fashion Sense by making some some shoes
and struggled, matching the animation frame timings to the character, it started to be easier to reanimate
the whole legs, but then that caused other problems - so this mod aimed to make some simpler adjustments easier.
Fashion Sense is quite different - highly customisable visual mod, not aimed at following the 'rules of SDV',
so great if you want to make your game look more like an Anime, turn your character into an animal etc that's what you might want!
In these ways, Fashion Sense is a sister mod - same family but with different name and interests, but alas
these sisters don't work together!

### What does a content pack look like?
A content pack is a folder with a few text files and images in it. You need a`manifest.json`
(which has info like your mod name, and Dynamic Bodies as a dependency) and files decribing
rules and locations of images, such as `content.json`, `hair.json`, `shirts.json` and `boots.json`. A
folder with customisation options for all posibilities will generally look like:
```
📁 Mods/
   📁 [DB] YourModName/
      🗎 content.json
      🗎 manifest.json
      📁 assets/
         📁 arms/
            🗎 toned_Long.png
            🗎 toned_Normal.png
            🗎 toned_Short.png
            🗎 toned_Sleeveless.png
            ...
         📁 bodies/
            🗎 toned.png
            ...
         📁 ears/
            🗎 f_pokey.png
            🗎 pokey.png
            ...
         📁 faces/
            🗎 cute.png
            ...
         📁 nakedLower/
            🗎 bikini.png
            ...
         📁 nakedUpper/
            🗎 bikinibra.png
            ...
         📁 nose/
            🗎 big.png
            ...
      📁 Boots/
         🗎 boots.json
         🗎 sneakers.png
         ...
      📁 Hair/
         🗎 hair.json
         🗎 Short Messy Curls.png
         ...
      📁 Shirts/
         🗎 shirts.json
         🗎 hangingoveralls.png
         ...
```

Have a look at the [existing examples on the mod page](https://www.nexusmods.com/stardewvalley/mods/12893?tab=files#file-container-optional-files). This guide goes into more detail below.

## Get started
To start a new content pack for Dynamic Bodies;

1. Install [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400) and [Dynamic Bodies](https://www.nexusmods.com/stardewvalley/mods/12893).
2. Create an empty folder in your `Mods` folder, and name it `[DB] YourModName`. Replace
   `YourModName` with a unique name for your mod.
3. Create a `manifest.json` file with this content:
   ```js
   {
       "Name": "Your Mod Name",
       "Author": "Your Name",
       "Version": "1.0.0",
       "Description": "One or two sentences about the mod.",
       "UniqueID": "YourName.DB.YourModName",
       "UpdateKeys": [],
       "ContentPackFor": {
           "UniqueID": "ribeena.dynamicbodies"
       }
   }
   ```
4. Change the `Name`, `Author`, `Description`, and `UniqueID` values to describe your mod. (Don't
   change the `UniqueID` under `ContentPackFor`!)

That's it! You now have a pack, though it doesn't do anything yet - you need to add content and
the relevant JSON files needed.

## Features
You don't need to know or use all of these features shown below to make a content pack. However,
you may want to open your `DynamicBodies\config.json` file and change it to show debug messages;
```
{
  "freecustomisation": false,
  "debugmsgs": true
}
```

### Body Parts
This features uses the main `content.json` file to define what files are present in the
assets folder, and what part of the body they change.

A sample JSON file looks like;
```
{
  "unisex": {
    "nakedLowers": {
      "White Breifs": {
        "name": "tighty",
        "options": ["no skin", "below accessories"]
      }
    },
    "nakedUppers": {
      "White Bra": {
        "name": "bra",
        "options": ["no skin"]
      },
      "Striped Bikini": {
        "name": "striped bikini",
        "options": ["no animation"]
      }
    }
  },

  "male": {
    "arms": {
      "Toned Arms":"Toned"
    },
    "bodyStyles": {
      "Toned": "toned",
    },
    "faces": {
      "Femme":"femme",
    },
    "ears": {
      "Pokey":"pokey",
    },
    "nose": {
      "Big": "big",
      "No nose": "noseless",
    }
  },

  "female": {
    "faces": {
      "Homme":"homme",
    },
    "ears": {
      "Pokey":"pokey",
    },
    "nose": {
      "Big": "f_big",
      "No nose": "f_noseless",
    }
  }

}
```

The JSON is split into 3 parts, Unisex, Male and Female. This is because some
graphics will show fine across either gender height, while others (on the face generally)
won't work without custom graphics.

In each of the gender profiles, there are options for you to create a body part, some
can be Unisex or tied to a gender if you choose, others must be either for male/female height.

More specific details about each body-part feature below;
*   [Face, Eyes, Ears and Nose](author-guide/face-and-parts.md)
*   [Bodies and Naked Overlays](author-guide/bodies.md)
*   [Arms (and shirt sleeve lengths)](author-guide/arms.md)
*   [The color palette](author-guide/color-palette.md)

### Hair
You do not need to do anything special to get dual colors etc running with hair,
but [there are some tips to get the best results](author-guide/hair.md#recoloring-notes). This
hair feature allows you to create some basic animation to the hair, whether its an override
of a vanilla hairstyle, or a completely new one.

Under the `[DB] YourModName\Hair` folder create a `hair.json` file which may look like;
```
﻿{
  "hairStyles": {
    "6": {
      "usesUniqueLeftSprite": true,
      "isBaldStyle": false,
      "anim_frames": {
        "Walk": { 0: 0, 1:1, 2:0, 3:2 },
        "Run": { 0: 0, 1:0, 2:1, 3:1, 4:0, 5:0, 6:2, 7:2 },
        "Ride": { 0:1, 1:0, 2:2, 3:2, 4:0, 5:1 },
      }
    },
    "Short Messy Curls": {
      "usesUniqueLeftSprite": false,
      "isBaldStyle": false,
      "anim_frames": {
        "Walk": { 0: 0, 1:1, 2:0, 3:1 },
        "Run": { 0: 0, 1:0, 2:1, 3:1, 4:0, 5:0, 6:1, 7:1 },
        "Ride": { 0: 1, 1:1, 2:0, 3:0, 4:1, 5:0 },
      }
    },
  }
}
```
Under `hairStyles` is the name of the files for the hair. If using a number, Dynamic Bodies
will check if it matches an exisiting Vanilla hairstyle and replace that. Similar to making
a hair mod for Content Patcher, you can define whether you have 4 images (`"usesUniqueLeftSprite": true`)
and if the base body uses the shadowwed forehead or larger without 'bald' head (`"isBaldStyle": true`).

The hair textures need to be structured like vanilla, facing foward is the top row, facing right
the second row, and facing up is the third row. If you have set `"usesUniqueLeftSprite": true` then the
fourth row is looking left.

[`Hair` documentation](author-guide/hair.md) for more info.

### Shoes
This shoe (boots? SDV seems to use both!) feature allows you to create graphics that cover
the feet of the character and load when a pair of boots is equipped. You can use the
[feet.png](../assets/Character/feet.png) file of this mod as your base on how many
pixels are needed.

Once you have your image, under the `[DB] YourModName\Boots` folder create a `boots.json` file which may look like;

```
{
	"overrides": {
		"small": ["Firewalker Boots", "Genie Shoes", "Leprechaun Shoes",
			"Cinderclown Shoes", "Crystal Shoes", "Socks"],
		"hightops": ["Vonnies"],
		"sneakers": ["Sneakers"],
		"smallblack": ["Work Boots", "Dark Boots", "Cowboy Boots"]
	}
}
```
This is relatively simple, `small` (left) indicates the name of the file (`small.png`) in the Boots folder.
The next part is a fuzzy-find for when the boots are equipped - it could be a full name, or just the
start of the name. In the about example, 'Sneakers' will match to "Sneakers - Yellow".

[`Shoes` documentation](author-guide/shoes.md) for more info.

### Shirt Overlays
The vanilla outfits are already done for the shirts-overalls color overlay, you can see the vanilla
version with the [shirts_overlay.png](../assets/Character/shirts_overlay.png) file.

These add the pants color over the shirt for things like overalls.

You can add your own using [JSONAssets](author-guide/other-mods.md#ja-shirt-overlays) or [DGA](author-guide/other-mods.md#dga-shirt-overlays).

### Trinkets
Trinkets is a new system for accessories, it allows you to add a cost to the items which can be purchased and
equipped in Haley's place. Unlike other customisation options, this requires you to buy each item, but
once bought you can wear it as often as you like. This is because the source code has a list of what
looks to be initial accessories you can use, but others you have to access later, but the code 
wasn't finished - so here it is!

The trinkets can work the same as the default accessories, but they also allow for 1 or 2 customisable
colours, to limit the amount similar lipsticks/blushes etc being made.

Under the `[DB] YourModName\Trinkets` folder create a `trinkets.json` file which may look like;
```
{
  "trinkets": {
    "Glasses": {
      "usesUniqueLeftSprite": false,
      "layers": [0,1,2,3,4],
      "primaryColor": true,
      "secondaryColor": true,
      "cost": 50,
    },
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
  }
}
```
Under `trinkets` is the name of the files for the trinket. You can specify what
[layers](author-guide/trinkets.md#layers) the trinket can be equipped on, which
[colour customisations](author-guide/trinkets.md#recoloring-notes) it has,
and using the basic animation system for movement.

The textures need to be structured like vanilla, facing foward is the top row, facing right
the second row, and facing up is the third row. If you have set `"usesUniqueLeftSprite": true` then the
fourth row is looking left.

[`Trinkets` documentation](author-guide/trinkets.md) for more info.


### Other mods pipeline
With SMAPI's new content pipeline this mod has been made with that in mind. You can use [Content Patcher](https://www.nexusmods.com/stardewvalley/mods/1915)
to easily adjust any of the default graphics. This means if you want to make a farmer sprite
overhaul mod, you can without changing any of the files, or just want to make it work
with your favourite UI mod.

To modify any file, find it in the [assets folder](../assets) and take note of the name and folders, you
can then modify it using a Content Patcher mod like below;

```
{
	"Format": "1.27.0",


	"Changes": [
		{
		"Action": "EditImage",
		"PatchMode": "Overlay",
		"Target": "Mods/ribeena.dynamicbodies/assets/Interface/ui.png",
		"FromFile": "assets/ui_DustBeauty.png",
		"FromArea": { "X": 0, "Y": 0, "Width": 160, "Height": 80 },
		"ToArea": { "X": 0, "Y": 0, "Width": 160, "Height": 80 },
		"When": {
			"HasMod": "Hesper.RusticCountrysideTownInterior",
			},
		},
    ]
}
```

It follows `Target` being `Mods/ribeena.dynamicbodies/` followed by the name and folders.
It's recommended you use the `HasMod` condition to make the mod only apply when needed.

#### Maps
You can use the config settings to set "adjustmaps" to "false" - this will stop the mod
from adding graphics and access points to the new menus. To add them to a custom map
you will need to use Content Patcher and the following actions;
*   DynamicBodies:Doctors
*   DynamicBodies:Haley
*   DynamicBodies:Leah
*   DynamicBodies:Pam

You can make a simple [ContentPatcher](https://www.nexusmods.com/stardewvalley/mods/1915) mod to adjust any custom maps to include these
points to access the menus of this mod.

### Translations
There are no translations, however Dynamic Bodies is set up for translations - use the [default.json](../i18n/default.json)
file as a reference to translate from US English. Follow the [SMAPI guide](https://stardewcommunitywiki.com/Modding:Translations)
for more help on how to translate this mod.

Using the unofficial [British English](https://www.nexusmods.com/stardewvalley/mods/7183) mod,
there are already [translations](../assets/i18n/en-gb.json) that happen automatically.
