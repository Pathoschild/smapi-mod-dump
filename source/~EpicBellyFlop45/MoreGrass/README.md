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
    }

Only **true** and **false** are accepted, this specifies whether the default grass sprites can also be drawn. If no config.json is present, true is the default. If this config option is set to false and after loading all content packs there is a season with no sprites, then the default game sprites will be added for that season.

## Install
1. Install the latest version of [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400).
2. Install the latest version of [this mod](https://www.nexusmods.com/stardewvalley/mods/5398).
3. Extract the .zip mod file into your StardewValley/Mods folder and run the game using SMAPI.

## Use
Add any content packs to the **StardewValley/Mods** file and run the same using SMAPI.

## Compatibility
More Grass is compatible with Stardew Valley 1.4+ on Windows/Mac/Linus, both single player and multiplayer. To view reported bug visit both the issues on this repo and bug reports on [Nexus](https://www.nexusmods.com/stardewvalley/mods/5398?tab=bugs).
