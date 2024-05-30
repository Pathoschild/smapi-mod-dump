**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/rokugin/StardewMods**

----

# Colorful Fish Ponds


## Contents
* [Introduction](#introduction)
* [Content Pack](#content-pack)
  * [Manifest Json](#manifest-json)
    * [Manifest Example](#manifest-example)
  * [Content Json](#content-json)
    * [Single Overrides Example](#single-overrides)
    * [Group Overrides Example](#group-overrides)
    * [Both Overrides Example](#both-overrides)
  * [Suggested Tags](#suggested-tags)

## Introduction<span id="introduction"></span>
Colorful Fish Ponds(CFP) by default makes every fish pond that normally doesn't change color change to the dye color the fish is set to.

CFP uses content packs to allow for custom color overrides.

## Content Pack<span id="content-pack"></span>
Create a new folder in your Mods folder, naming convention dictates that you name it `[CFP] Mod Name`, but as long as your manifest is correct the folder name doesn't matter.

### Manifest Json<span id="manifest-json"></span>
Create a manifest.json inside your content pack folder.


<br>Example manifest.json:<span id="manifest-example"></span>
```json
{
    "Name": "Content Pack Name",
    "Author": "yourName",
    "Version": "1.0.0",
    "Description": "Description.",
    "UniqueID": "yourName.contentPackName",
    "UpdateKeys": [],
    "Dependencies": [],
    "ContentPackFor": {
      "UniqueID": "rokugin.colorfulfishponds",
      "MinimumVersion": "1.0.0"
    }
}
```

**Name** is the name of your mod, something short that tells anyone else at a glance what it does is best here.

**Author** is your name, this is so people know who made the mod.

**Version** is what version of your mod this is, also used when updating your mod to let others know there's an update if you have an update key.

**Description** is where you can put a longer description of the purpose of your mod.

**UniqueID** is conventionally your name and mod name separated by a period without any spaces. It's best to stick to convention here to avoid any errors.

**UpdateKeys** is where you put the update keys that alert others to when you've updated your mod. More info [here](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Update_checks).

**Dependencies** can be used to only apply your mod if another mod is installed. More info [here](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Manifest#Dependencies).

**ContentPackFor** tells SMAPI what mod this content pack is for, you should use this exactly as it is for this mod.

### Content Json<span id="content-json"></span>
Create a content.json inside your content pack folder, alongside your manifest.json.


<br>Use either or both of the following blocks:
```json
{
    "SingleOverrides": {
        
    },
    "GroupOverrides": {
    
    }
}
```

Individual fish color overrides go in **SingleOverrides** and <br>
overrides for fish caught all in the same location go in **GroupOverrides**.


<br>Example content.json for SingleOverrides:<span id="single-overrides"></span>
```json
{
    "SingleOverrides": {
        "Blobfish": {
            "FishID": "800",
            "PondColor": {
                "R": 255,
                "G": 0,
                "B": 255
            }
        },
        "Coral": {
            "FishID": "393",
            "PondColor": {
                "R": 215,
                "G": 117,
                "B": 222
            }
        }
    }
}
```

**Blobfish** is the entry key, it needs to be unique but doesn't need to be related to the actual fish you're overriding color for.

**FishID** is a string reference to the unqualified item ID of the fish you want to override the color of.<br>
ID's can be found in `Data/Objects` or at https://mateusaquino.github.io/stardewids/

**PondColor** holds the red, green, and blue values for the color override.


<br>Example content.json for GroupOverrides:<span id="group-overrides"></span>
```json
{
    "GroupOverrides": {
        "River": {
            "GroupTag": "fish_river",
            "PondColor": {
                "R": 127,
                "G": 255,
                "B": 212
            }
        },
        "Ocean": {
            "GroupTag": "fish_ocean",
            "PondColor": {
                "R": 0,
                "G": 150,
                "B": 255
            }
        }
    }
}
```

**River/Ocean** is the entry key, it needs to be unique dbut doesn't need to be related to the group of fish you're overriding color for.

**GroupTag** is the context tag for the group you're overriding color for.<br>
Really, any tag can be used here but suggested tags can be found [here](#suggested-tags).

**PondColor** holds the red, green, and blue values for the color override.


<br>Depending on your config settings, you can make use of both override options at the same time so you can also put all your overrides in the same pack.


<br>Example of content.json with both overrides:<span id="both-overrides"></span>
```json
{
    "SingleOverrides": {
        "Blobfish": {
            "FishID": "800",
            "PondColor": {
                "R": 255,
                "G": 0,
                "B": 255
            }
        },
        "Coral": {
            "FishID": "393",
            "PondColor": {
                "R": 215,
                "G": 117,
                "B": 222
            }
        }
    },
    "GroupOverrides": {
        "River": {
            "GroupTag": "fish_river",
            "PondColor": {
                "R": 127,
                "G": 255,
                "B": 212
            }
        },
        "Ocean": {
            "GroupTag": "fish_ocean",
            "PondColor": {
                "R": 0,
                "G": 150,
                "B": 255
            }
        }
    }
}
```

### Suggested Tags<span id="suggested-tags"></span>
Every fish has a context tag that indicates where it's caught, which can be used for convenient group coloring.

Here are the most common ones:<br>
`fish_bug_lair`<br>
`fish_desert`<br>
`fish_freshwater`<br>
`fish_lake`<br>
`fish_mines`<br>
`fish_night_market`<br>
`fish_ocean`<br>
`fish_pond`<br>
`fish_river`<br>
`fish_secret_pond`<br>
`fish_sewers`<br>
`fish_swamp`

There are a few exceptions and issues with grouping fish.

Coral and sea urchin can be placed inside fish ponds but don't have a location tag because they're technically forage.<br>
In order to work around this, you can make single fish overrides for them or use the group tag `forage_item_beach` to override both their colors.

One issue is that some fish have several tags such as carp which has `fish_lake`, `fish_secret_pond`, and `fish_sewers`.<br>
The color override will choose the first match it finds for the group tag,<br>
so if you have a `fish_lake` override followed by a `fish_secret_pond` override, the mod will always apply the `fish_lake` override to any fish with both tags.<br>
In order to work around this, you would have to use a single fish override.

`fish_freshwater` is only used for crab pot fish caught in fresh water like lakes or rivers. `fish_ocean` includes all the regular caught ocean fish as well as the crab pot caught ocean fish.<br>
`fish_crab_pot` could be used if you wanted all crab pot caught fish to have the same color