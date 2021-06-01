**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/AndyCrocker/StardewMods**

----

**More Grass** is a [Stardew Valley](http://stardewvalley.net/) framework mod that allows you to add custom grass sprites, season dependant using json.

![](pics/moregrass.png)

## Creating a Content Pack
1. Create a new folder for the content pack. The convention is: **[MG] mod name**.
2. Create a sub folder for each season you have grass sprites for (see final result below for reference) NOTE: name is case sensitive, use lowercase.
3. Add all the **.png** images to the respective season folders. NOTE: the images can have any name, there is no convention. These should be **15px x 20px**
4. Create a manifest.json, see below for reference
5. Create a config.json, see below for reference

#### Final Content Pack Layout
    [MG] mod name
        config.json
        manifest.json
        spring
            1.png
            2.png
        summer
            1.png
            2.png
        fall
            1.png
            2.png
        winter
            1.png
            2.png

#### Manifest.json example
    {
        "Name": "[MG] mod name",
        "Author": "your name",
        "Version": "1.0.0",
        "Description": "description",
        "UniqueID": "your name.mod name",
        "MinimumApiVersion": "3.0.0",
        "UpdateKeys": [ update key ],
        "ContentPackFor": {
            "UniqueID": "EpicBellyFlop45.MoreGrass"
        }
    }

#### Config.json example
    {
        "EnableDefaultGrass": true
        "WhiteListedGrass": {
            "1.png": [ "town" ]
        },
        "BlackListedGrass": {
            "2.png": [ "s:farm" ]
        },
        "WhiteListedLocations": [ "farm" ],
        "BlackListedLocations": []
    }

Property             | Default value | Description
------------------   | :-----------: | -----------
EnableDefaultGrass   | `true`        | Whether default grass sprites can also be drawn with the content pack. If this options is set to `false` and there are no available sprites for a given location, then the default sprites will be added.
WhiteListedGrass     | `{}`          | The locations that each specified grass is allowed to be in. See below for special syntax for location names.
BlackListedGrass     | `{}`          | The locations that each specified grass isn't allowed to be in. See below for special syntax for location names.
WhiteListedLocations | `[]`          | The locations that this pack is allowed to retexture grass in. See below for special syntax for location names.
BlackListedLocations | `[]`          | The locations that this pack isn't allowed to retexture grass is. See below for special syntax for location names.

##### Location Name Prefixes
Location names can have special prefixes that will change how the name is interpreted. The three prefixes are `s:`, `c:`, and `e:`. If the location name starts with `s:`, then it'll check if the current location name *starts* with the specified value, for example `s:farm` will apply to both the `farm` and `farmhouse` locations. The `c:` prefix will check if the current location name *contains* the specified value, for example `c:mine` will apply to both the `mine` and all `undergroundmine` locations. Finally, the `e:` prefix will check if the current location name *ends* eith the specified value.

## Install
1. Install the latest version of [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400).
2. Install the latest version of [this mod](https://www.nexusmods.com/stardewvalley/mods/5398).
3. Extract the .zip mod file into your StardewValley/Mods folder and run the game using SMAPI.

## Use
Add any content packs to the **StardewValley/Mods** file and run the game using SMAPI.

## Compatibility
More Grass is compatible with Stardew Valley 1.4+ on Windows/Mac/Linus, both single player and multiplayer. To view reported bug visit both the issues on this repo and bug reports on [Nexus](https://www.nexusmods.com/stardewvalley/mods/5398?tab=bugs).
