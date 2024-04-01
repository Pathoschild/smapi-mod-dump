**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/DecidedlyHuman/StardewValleyMods**

----

# Mapping Extensions and Extra Properties (MEEP) Documentation
**Version 2.3.0 -- The 1.6 Special!**
*PLEASE NOTE: The layers different tile properties go on have changed in MEEP 2.0.0 as a result of changes in 1.6.*

All releases can be found on my [Nexus page](https://www.nexusmods.com/users/79440738?tab=user+files).

## What it does
This mod does nothing on its own. Its primary purpose is to allow map authors to spice up their maps with the new custom tile properties, extra features, etc., that this mod adds.

## Current features
Click on the link to go to the mini-docs for each one

| Updated in version | **Detailed Description**                                                              | **Layer** | **Description**                                                                                                                                                                                                                                                                                                                   |
|:-------------------|---------------------------------------------------------------------------------------|-----------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 2.0.0              | [*Closeup Interaction*](#Using-the-CloseupInteraction-tile-properties)                | Buildings | This tile property will display a specified image on the screen when the player interacts with the tile it's placed on. If you want the player to be able to examine a photo on a desk and actually see the photo up-close, this is the one to use.                                                                               |
| 2.0.0              | [*Closeup Interaction Text*](#Using-the-CloseupInteraction-tile-properties)           | Buildings | This tile property only works in conjunction with `CloseupInteraction_Image`, and will display the specified text as a description below the image.                                                                                                                                                                               |
| 2.0.0              | [*Closeup Interaction Reel*](#Using-the-CloseupInteraction-reel-tile-properties)      | Buildings | This is a special variation of the closeup interaction properties. With this method, the mod will display the first image, and allow the player to also look at image 2, image 3, etc., all while allowing you to optionally have a text description for required images.                                                         |
| 2.0.0              | [*Closeup Interaction Sound*](#Using-the-MEEP_CloseupInteraction_Sound-tile-property) | Buildings | This is a tile property you can use alongside any of the other Closeup Interaction properties. When you specify a game sound cue using it, the sound will play when the player opens the interaction, or turns the page in the case of a reel.                                                                                    |
| 2.0.0              | [*Set Mail Flag*](#Using-the-MEEP_SetMailFlag-tile-property)                          | Buildings | This tile property will set the specified mail flag when the player interacts with the tile it's on.                                                                                                                                                                                                                              |
| **2.2.0**          | [Adding a conversation topic](#Using-the-MEEP_AddConversationTopic-tile-property)     | Buildings | MEEP allows you to add a conversation topic to the player that interacts with a tile. This works alongside any other MEEP tile properties. For instance, you can set a conversation topic when the player clicks to look at something on an NPC's table.                                                                          |
| 2.0.0              | [*Fake NPC*](#Using-the-MEEP_FakeNPC-tile-property)                                   | Back      | This tile property will spawn a fake NPC on the tile it's placed on. This NPC will breathe like a normal NPC, face you like a normal NPC, and can be talked to like a normal NPC. You can also specify a custom sprite size for the NPC. For example: a 32x32 NPC, or a 64x64 NPC. Other sizes may work, but haven't been tested. |
| 2.0.0              | [*Letter*](#Using-the-MEEP-Letter-tile-property)                                      | Buildings | With the Letter tile properties, you can trigger a vanilla-style letter/mail when the player interacts with the specified tile.                                                                                                                                                                                                   |
| 2.0.0              | [*Letter Type*](#MEEP_Letter_Type)                                                    | Buildings | This property allows you to specify a vanilla letter background, *or* a custom letter background image.                                                                                                                                                                                                                           |
| 2.1.0          | [Farm Animal Spawning](#Spawning-farm-animals)                                        | N/A       | MEEP lets you spawn any farm animal in the game (including custom ones added with 1.6's new custom farm animal feature) on a map of your choosing. Farm animals spawned by MEEP can't be milked/sheared/sold, and can display a custom message when you chat with them.

## Using the features
Using the features is fairly simple. There are a few things you'll need to know that I won't be covering here:
1) The basics of creating a Content Patcher pack. See [the Content Patcher docs](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide.md).
2) How to load an image asset using Content Patcher. See [documentation for the `Load` action](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/action-load.md).
3) How to patch tile properties using Content Patcher (see [documentation for the `EditMap` action](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/action-editmap.md#edit-map-tiles)), or how to add tile properties to your map [directly using Tiled here](https://stardewvalleywiki.com/Modding:Maps#Tile_properties).
4) How to patch data models using Content Patcher (see [the documentation for the `EditData` action here](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/action-editdata.md)).
5) Add the appropriate keys to your mod's manifest to tell MEEP you're using certain features (see the [first section here](#Adding-meep-feature-keys-to-your-manifest)).

### Adding MEEP feature keys to your manifest
This part is simple, but important. In order for MEEP to know which features to enable, it needs to know
which feature every mod using it requires, and which version of MEEP the mod is expecting.

All of the current keys are as follows:

* `DH.MEEP` (Mandatory to use MEEP.)
* `DH.MEEP.CloseupInteractions`
* `DH.MEEP.FakeNPCs`
* `DH.MEEP.VanillaLetters`
* `DH.MEEP.SetMailFlag`
* `DH.MEEP.FarmAnimalSpawns`
* `DH.MEEP.AddConversationTopic`

The following example uses closeup interactions, fake NPCs, and was made for version 2.0.0 of MEEP.

```json
{
    "Name": "Tile Property Test Mod",
    "Author": "DecidedlyHuman",
    "Version": "1.0.0",
    "Description": "Tile properties for testing.",
    "UniqueID": "DecidedlyHuman.TilePropertyTestMod",
    "UpdateKeys": [],
    "ContentPackFor": {
        "UniqueID": "Pathoschild.ContentPatcher"
    },
    "DH.MEEP": "2.0.0",
    "DH.MEEP.CloseupInteractions": "",
    "DH.MEEP.FakeNPCs": ""
}
```

This next example uses only the letter and set mail flag properties.
```json
{
    "Name": "Tile Property Test Mod",
    "Author": "DecidedlyHuman",
    "Version": "1.0.0",
    "Description": "Tile properties for testing.",
    "UniqueID": "DecidedlyHuman.TilePropertyTestMod",
    "UpdateKeys": [],
    "ContentPackFor": {
        "UniqueID": "Pathoschild.ContentPatcher"
    },
    "DH.MEEP": "2.0.0",
    "DH.MEEP.VanillaLetters": "",
    "DH.MEEP.SetMailFlag": ""
}
```

### Using the CloseupInteraction tile properties
The basic format for `CloseupInteraction` is in the following snippet of an `EditMap` patch using Content Patcher.

```json
{
    "Format": "1.28.0",
    "Changes": [
        {
            // Apply the tile property
            "Action": "EditMap",
            "Target": "Maps/SeedShop",
            "MapTiles": [
                {
                    "Position": {
                        "X": 8,
                        "Y": 18
                    },
                    "Layer": "Buildings",
                    "SetProperties": {
                        "Action": "MEEP_CloseupInteraction_Image LooseSprites/Cursors 540 305 42 28",
                        "MEEP_CloseupInteraction_Text": "The spirits tell me you're learning how to use a new mod..."
                    }
                }
            ]
        }
    ]
}
```
`MEEP_CloseupInteraction_Image` takes a few "arguments". The first, and only mandatory one, is the asset name (which can be a built-in Stardew image).
The second is the region of the specified image you want to be displayed.


For example...
```json
"Action": "MEEP_CloseupInteraction_Image LooseSprites/Cursors 540 305 42 28",
"MEEP_CloseupInteraction_Text": "The spirits tell me you're learning how to use a new mod..."
```
Will display the fortune teller, and a message reading "The spirits tell me you're learning how to use a new mod...".

In `540 305 42 28`, `540` is the x co-ordinate of the top-left corner of the region of the specified image you want to display,
`305` is the y co-ordinate, `42` is the width, and `28` is the height.

**Warning**: It's worth keeping in mind the size of the image, and whether or not it will interfere with Stardew Valley
when running at lower resolutions when combined with the text display option. I recommend you **always test your images
at a varying UI scale settings and window sizes** if you want to play it safe.

### Using the CloseupInteraction reel tile properties
An example of a closeup interaction reel looks like so:
```json
{
    "Changes": [
        {
            // Demonstration of a multiple image reel (option one).
            "Action": "EditMap",
            "Target": "Maps/Town",
            "Update": "OnTimeChange",
            "MapTiles": [
                {
                    "Position": {
                        "X": 24,
                        "Y": 53
                    },
                    "Layer": "Buildings",
                    "SetProperties": {
                        "Action": "MEEP_CloseupInteractionReel",
                        "MEEP_CloseupInteraction_Image_1": "LooseSprites/Cursors 540 305 42 28",
                        "MEEP_CloseupInteraction_Text_1": "The spirits tell me you're learning how to use a new mod...",
                        "MEEP_CloseupInteraction_Image_2": "LooseSprites/Cursors 644 361 42 28",
                        "MEEP_CloseupInteraction_Text_2": "FOOD!",
                        "MEEP_CloseupInteraction_Image_3": "LooseSprites/Cursors 112 656 16 64",
                        "MEEP_CloseupInteraction_Image_4": "LooseSprites/Cursors 160 660 32 60",
                        "MEEP_CloseupInteraction_Text_4": "Weird, pink tentacle spiral thing?"
                    }
                }
            ]
        }
    ]
}
```
The `Action` `MEEP_CloseupInteraction_Image_1`, `MEEP_CloseupInteraction_Image_2`, etc. properties are required, and you cannot
have more `MEEP_CloseupInteraction_Text_1` properties than image ones, as they'll simply be ignored.

In this example, there are four images:

(`MEEP_CloseupInteraction_Image_1`, `MEEP_CloseupInteraction_Image_2` `MEEP_CloseupInteraction_Image_3` `MEEP_CloseupInteraction_Image_3`)

and three descriptions

(`MEEP_CloseupInteraction_Text_1`, `MEEP_CloseupInteraction_Text_2`, `MEEP_CloseupInteraction_Text_4`)

Note how there is no `MEEP_CloseupInteraction_Text_3`.This simply means that when the user switches to the third page,
the image on that page won't have any text beneath it.

Finally, and most importantly, the `Action` tile property with the value `MEEP_CloseupInteractionReel`. This tells MEEP
to treat this like a closeup interaction reel.

### Using the MEEP_CloseupInteraction_Sound tile property
You can spice up your closeup interactions by specifying that a given sound cue be played when
the interaction is opened, or the page is turned in the reel.

```json
{
    "Position": {
        "X": 40,
        "Y": 22
    },
    "Layer": "Buildings",
    "SetProperties": {
        "Action": "MEEP_CloseupInteraction_Image Mods/DecidedlyHuman/MaruRobot",
        "MEEP_CloseupInteraction_Text": "It's Maru's robot! Did someone copy the design?",
        "MEEP_CloseupInteraction_Sound": "dog_bark"
    }
}
```

The sound cue must be valid, or MEEP will log an error every time the property is interacted with, and no sound will be played.

### Using the MEEP_SetMailFlag tile property
This one is fairly self-explanatory. You would add the tile property `DHSetMailFlag`, and the value for it is the mail flag you want to be set. for example:

```json
{
    "Format": "1.28.0",
    "Changes": [
        {
            // Apply the tile property
            "Action": "EditMap",
            "Target": "Maps/SeedShop",
            "When": {
                "HasFlag |contains=DHSeenFortuneTellerImage": false
            },
            "MapTiles": [
                {
                    "Position": {
                        "X": 8,
                        "Y": 18
                    },
                    "Layer": "Buildings",
                    "SetProperties": {
                        "Action": "MEEP_CloseupInteraction_Image LooseSprites/Cursors 540 305 42 28",
                        "MEEP_CloseupInteraction_Text": "The spirits tell me you're learning how to use a new mod...",
                        "MEEP_SetMailFlag": "DHSeenFortuneTellerImage"
                    }
                }
            ]
        }
    ]
}
```

With this example, interacting with the tile will bring up the fortune teller image and message, and set the mail flag `DHSeenFortuneTellerImage`. Whenever Content Patcher refreshes its patches, the interaction to bring up the image and description will vanish. You can specify a custom update rate [as seen here](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide.md#how-often-are-patch-changes-applied).

You could also use this for any kind of conditional patch that checks for a mail flag.

### Using the MEEP_AddConversationTopic tile property
This one is also fairly self-explanatory. It sets a conversation topic whenever a tile is interacted with!
```json
{
    "Format": "1.28.0",
    "Changes": [
        {
            // Apply the tile property
            "Action": "EditMap",
            "Target": "Maps/SeedShop",
            "MapTiles": [
                {
                    "Position": {
                        "X": 13,
                        "Y": 18
                    },
                    "Layer": "Buildings",
                    "SetProperties": {
                        "Action": "MEEP_Letter Super Evil Joja Plans^^Ha, fooled you! No plans here.",
                        "MEEP_Letter_Type": "Mods/DecidedlyHuman/JojaLetterBG",
                        "MEEP_AddConversationTopic": "DH.MEEP.SeenEvilJojaLetter 7"
                    }
                }
            ]
        }
    ]
}
```
All you do is add the `MEEP_AddConversationTopic` tile property as you see above. It takes one or two arguments. In:
```
"MEEP_AddConversationTopic": "DH.MEEP.SeenEvilJojaLetter 7"
```

`DH.MEEP.SeenEvilJojaLetter`: This is the conversation topic you want to add.
`7`: This is the number of days the conversation topic will remain active.

### Using the MEEP_FakeNPC tile property
This tile property will allow you to spawn a "fake" NPC on a given tile. Unlike a "real" NPC, which needs a disposition, and lots of setup, a "fake" NPC needs very, very little. Fake NPCs cannot receive gifts, don't have a schedule, and won't move around. They're intended to be a middle ground between a simple static NPC sprite, and a fully-fledged NPC.

The most basic setup is as follows:
```json
{
    "Format": "1.28.0",
    "Changes": [
        {
            "Action": "Load",
            "Target": "Portraits/NotAbigail",
            "FromFile": "assets/NotAbigail/NotAbigailPortrait.png"
        },
        {
            "Action": "Load",
            "Target": "Characters/NotAbigail",
            "FromFile": "assets/NotAbigail/NotAbigail.png"
        },
        {
            "Action": "EditData",
            "Target": "MEEP/FakeNPC/Dialogue/NotAbigail",
            "Entries": {
                "DialogueOne": "Hey @, you think you could build a raft?#$e#I saw a few cool islands on the way here I want to visit."
            }
        },
        {
            "Action": "EditMap",
            "Target": "Maps/Town",
            "MapTiles": [
                {
                    "Position": {
                        "X": 29,
                        "Y": 56
                    },
                    "Layer": "Back",
                    "SetProperties": {
                        "MEEP_FakeNPC": "NotAbigail"
                    }
                }
            ]
        }
    ]
}
```

Firstly, we need to load in a portrait for the NPC. You'll need to set the `FromFile` to point to wherever you have the source image.
```json
{
    "Action": "Load",
    "Target": "Portraits/NotAbigail",
    "FromFile": "assets/NotAbigail/NotAbigailPortrait.png"
}
```
Secondly, we need to load a spritesheet for the NPC.
```json
{
    "Action": "Load",
    "Target": "Characters/NotAbigail",
    "FromFile": "assets/NotAbigail/NotAbigail.png"
}
```

Thirdly, and optionally, we can add some dialogue if we want the NPC to speak. Most dialogue commands should work, but let me know if you run into anything that doesn't work as expected.

For dialogue keys, location-specific keys should work, but keep in mind that the NPC exists only where you put it. You could have the location of the NPC set via the tile property conditional, and have a different dialogue key for each location.
```json
{
    "Action": "EditData",
    "Target": "MEEP/FakeNPC/Dialogue/NotAbigail",
    "Entries": {
        "DialogueOne": "Hey @, you think you could build a raft?#$e#I saw a few cool islands on the way here I want to visit."
    }
}
```

Finally, we need to add a tile property to specify where we want the NPC to spawn.
```json
{
    "Action": "EditMap",
    "Target": "Maps/Town",
    "MapTiles": [
        {
            "Position": {
                "X": 29,
                "Y": 56
            },
            "Layer": "Back",
            "SetProperties": {
                "MEEP_FakeNPC": "NotAbigail"
            }
        }
    ]
}
```

#### Optional argument: NPC sprite size
We can also, however, specify a custom NPC sprite size. Yes, that means you can have an NPC larger than 16x32. How to do that is very simple:
```json
{
    "Action": "EditMap",
    "Target": "Maps/Town",
    "MapTiles": [
        {
            "Position": {
                "X": 29,
                "Y": 56
            },
            "Layer": "Back",
            "SetProperties": {
                "MEEP_FakeNPC": "NotAbigail 32 32"
            }
        }
    ]
}
```

This will create a fake NPC with a size of 32x32. Currently, I've only tested 32x32, and 64x64.

Do note, however, that using a custom size for your NPC will disable the breathing animation and shadow. This is primarily because the vanilla shadow is very specifically designed for a 16x32 NPC, and looks very bad on different sizes of NPC.

Instead, you can just draw the shadows into the NPC sprite. This allows for much nicer looking shadows, too!

### Using the MEEP Letter tile property
The `Letter` tile properties are fairly simple. There are two of them -- `MEEP_Letter`, and `MEEP_Letter_Type`.

#### MEEP_Letter
This is the mandatory property, and is fairly simple:
```json
{
    "Changes": [
        {
            "Action": "EditMap",
            "Target": "Maps/Custom_MEEP_Station",
            "MapTiles": [
                {
                    "Position": {
                        "X": 13,
                        "Y": 13
                    },
                    "Layer": "Buildings",
                    "SetProperties": {
                        "Action": "MEEP_Letter Super Evil Joja Plans^^Ha, fooled you! No plans here.^^Have some wood, though.%item object 388 50 %%",
                        "MEEP_Letter_Type": "2"
                    }
                }
            ]
        }
    ]
}
```

You just need to specify the tile property on the desired tile, and format the letter contents as you would for any mail/letter.

**Important note**: Most, if not all vanilla mail commands should work here. However, given that the letter can be opened as many times as the player desires as long as the patch remains in place, you want to be very, very careful not to allow for infinite items, and other weird issues.

#### MEEP_Letter_Type
There are two ways to use `MEEP_Letter_Type`. One is to simply specify the vanilla letter background type (seen [here](https://i.imgur.com/llJupGQ.png)), *or* specify your own custom background image.

Your custom image *must* follow the vanilla size, however. The file should be 320x180 pixels, but *visually* does not need to fill up the entire image.

##### Vanilla background example
```json
{
    "Position": {
        "X": 13,
        "Y": 12
    },
    "Layer": "Buildings",
    "SetProperties": {
        "Action": "MEEP_Letter Pretend this is a very, very long letter with multiple evil Joja-related plans in it.",
        "MEEP_Letter_Type": "2"
    }
}
```

Note nothing but the specific vanilla letter background ID in the property.

##### Custom background example
Firstly, you need to load your image somewhere in your Content Patcher mod as demonstrated below.
```json
{
    "Action": "Load",
    "Target": "Mods/DecidedlyHuman/JojaLetterBG",
    "FromFile": "assets/letter-bg.png"
}
```

Then, as seen the example below, specify the loaded image in the property.

```json
{
    "Position": {
        "X": 13,
        "Y": 18
    },
    "Layer": "Buildings",
    "SetProperties": {
        "Action": "MEEP_Letter Super Evil Joja Plans^^Ha, fooled you! No plans here.",
        "MEEP_Letter_Type": "Mods/DecidedlyHuman/JojaLetterBG"
    }
}
```

### Spawning farm animals

To spawn farm animals with MEEP, you'll be editing one of MEEP's data models using CP's `EditData` property. If you're unsure of how to do this, you can find a link to the `EditData` section of CP's documentation near the top of the readme.

Here's an example of an edit that will spawn two farm animals. One in the submarine, and one in Alex's house respectively:

```json
{
    "Changes": [
        {
            "Action": "EditData",
            "Target": "MEEP/FarmAnimals/SpawnData",
            "Entries": {
                "DH.TilePropertyTestMod.WhiteChickenSubmarine": {
                    "AnimalId": "White Chicken",
                    "Age": 0,
                    "LocationId": "Submarine",
                    "DisplayName": "Animal One Name",
                    "PetMessage": [
                        "UwU",
                        "I'm a baby chicken!"
                    ],
                    "HomeTileX": 13,
                    "HomeTileY": 5,
                    "Condition": ""
                },
                "DH.TilePropertyTestMod.WhiteChickenSubmarine": {
                    "AnimalId": "White Chicken",
                    "Age": 100,
                    "LocationId": "Submarine",
                    "DisplayName": "Animal One Name",
                    "PetMessage": [
                        "I'm a very old chicken, so you won't get an UwU from me!"
                    ],
                    "HomeTileX": 13,
                    "HomeTileY": 5,
                    "Condition": ""
                },
                "DH.TilePropertyTestMod.BrownCowJoshHouse": {
                    "Id": "DH.TilePropertyTestMod.BrownCowJoshHouse",
                    "AnimalId": "Brown Cow",
                    "LocationId": "JoshHouse",
                    "DisplayName": "Animal Two Name",
                    "PetMessage": [
                        "MUwU",
                        "What? I'm a cow, what else would I say?",
                        "Certainly not \"moo\"!"
                    ],
                    "HomeTileX": 9,
                    "HomeTileY": 20,
                    "Condition": ""
                }
            }
        }
    ]
}
```

In this case, we're adding two animals to `MEEP/FarmAnimals/SpawnData`. Each one is separated with a comma like many different things you would patch with CP.
Let's look at one in isolation.

```json
"DH.TilePropertyTestMod.WhiteChickenSubmarine": {
    "AnimalId": "White Chicken",
    "Age": 0,
    "LocationId": "Submarine",
    "DisplayName": "Animal One Name",
    "PetMessage": [
        "UwU",
        "I'm a baby chicken!"
    ],
    "HomeTileX": 13,
    "HomeTileY": 5,
    "Condition": ""
},
```

* `"DH.TilePropertyTestMod.WhiteChickenSubmarine"` is the spawn ID for the animal. This needs to be 100% unique per animal spawn, so it's recommended that you use the format `YourName.YourMod.AnimalType`. You can also do, for example, `YourName.YourMod.Animal1` if you plan on spawning multiple of the same animal. Just increment the number at the end!
* `"AnimalId": "White Chicken"`: This is the internal animal ID of the animal. In this case, we're spawning a vanilla white chicken, and its internal ID is `White Chicken`.
* `"Age": 0,`: This is the age of the animal!
* `"LocationId": "Submarine"`: This is the location name. For example, `SeedShop` is Pierre's shop, `JoshHouse` is Alex's house, and `ScienceHouse` is Robin's house.
* `"DisplayName": "Animal One Name"`: This is currently unused, but feel free to add it for the future when the name will be displayed alongside the pet message.
```json
"PetMessage": [
        "UwU",
        "I'm a baby chicken!"
]
```

This is a JSON list of messages to be displayed when the player interacts with a farm animal. In the above example, there are two entries. Just one entry will work, but you can have as many as you like.

```json
"HomeTileX": 13,
"HomeTileY": 5
```

These are the tiles the animal spawns on. They will wander around as usual, however.

```json
"Condition": ""
```

The condition field is a [Game State Query](https://stardewvalleywiki.com/Modding:Migrate_to_Stardew_Valley_1.6#Game_state_queries). The animal will only spawn if this condition is true, so you can have animals that only spawn in the sun, in the rain, or any other number of things supported by the game.
