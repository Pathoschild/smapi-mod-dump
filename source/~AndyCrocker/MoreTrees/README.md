**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/AndyCrocker/StardewMods**

----

**More Trees** is a [Stardew Valley](http://stardewvalley.net/) framework mod that allows you to add custom trees using json.

![](pics/moretrees.png)

## Creating a Content Pack
1. Create a new folder for the content pack. The convention is: **[MT] mod name**.
2. Create a sub folder for each tree you plan to add (see final result below for reference).
3. Create a **tree.png** image and add to the respective tree folder. NOTE: the images must be called **tree.png**.
4. Create a content.json and add to the respective tree folder (see below for reference). 
5. Create a manifest.json (see below for reference).
6. Create a config.json (see below for reference).

#### Final Content Pack Layout
    [MT] mod name
        manifest.json
        Hard Oak
            tree.png
            content.json
        Birch
            tree.png
            content.json
        Spruce
            tree.png
            content.json
        Maple
            tree.png
            content.json
        Rowan
            tree.png
            content.json

#### Tree.png example
// TODO: give game example and pixel locations of each stage, season, and debarked versions

#### Content.json example
    {
        "TappingProduct": {
            "DaysBetweenProduce": 4,
            "Product": "id"
        },
        "WoodProduct": "id",
        "Seed": "id",
        "ShakingProducts": [
            {
                "DaysBetweenProduce": 1,
                "Product": "id"
            },
            {
                "DaysBetweenProduce": 3,
                "Product": "id"
            }
        ],
        "IncludeIfModIsPresent": [ "uniqueModId", "uniqueModId" ],
        "ExcludeIfModIsPresent": [ "uniqueModId", "uniqueModId" ],
        "RequiresExtendedMode": true,
        "BarkProduct": {
            "DaysBetweenProduce": 4,
            "Product": "id"
        }
    }

* **Tapping**: This is the product that the tree drops when using a [tapper](https://stardewvalleywiki.com/Tapper) on it.
* **Tapping.DaysBetweenProduce**: The number of days between each harvest.
* **Tapping.Product**: The product that gets harvested when using a tapper.
* **WoodProduct**: This is the product that the tree drops when it gets cut down.
* **Seed**: This is the item to plant for the tree to grow.
* **ShakingProduct**: This is a list of products that drop when shaking the tree.
* **ShakingProduct.DaysBetweenProduce**: The number of days between the product can be dropped again.
* **ShakingProduct.Product**: The product that will get dropped.
* **IncludeIfModIsPresent**: The tree will only get loaded if one of the listed mods (by uniqueId) is present.
* **ExcludeIfModIsPresent**: The tree will only get loaded if none of the listed mods (by uniqueId) are present.
* **RequiredExtendedMode**: Whether the tree requires the user to have the extended mode of MoreTrees enables. (Extended mode adds the ability to harvest bark with the **Bark Remover**).
* **BarkProduct**: This is the product that the tree drops when using the **Bark Remover**.
* **BarkProduct.DaysBetweenProduce**: The number of days between each harvest.
* **BarkProduct.Product**: The product that gets harvested when using a **Bark Remover**.

**NOTE:** Ensure all ids are strings, this is because they also allow API tokens (The layout is: "UniqueModId:MethodName:Value"), and example of an API token is: **spacechase0.JsonAssets:GetObjectId:Maple Bark**, this will use an item from JA called **Maple Bark**.

#### Manifest.json example
    {
        "Name": "[MT] mod name",
        "Author": "your name",
        "Version": "1.0.0",
        "Description": "description",
        "UniqueID": "your name.mod name",
        "MinimumApiVersion": "3.0.0",
        "UpdateKeys": [ update key ],
        "ContentPackFor": {
            "UniqueID": "EpicBellyFlop45.MoreTrees"
        }
    }

## Add to tilemap
// TODO: explain tile data for trees

## Install
1. Install the latest version of [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400).
2. Install the latest version of [this mod](https://www.nexusmods.com/stardewvalley/mods/).
3. Extract the .zip mod file into your StardewValley/Mods folder and run the game using SMAPI.

## Use
First, open the game using SMAPI like normal, this will generate a config.json file in the Mods/MoreTrees folder.
Then using the below section configure the mod to your liking.
Add any content packs to the **StardewValley/Mods** file 
Lastly load back into the game with SMAPI and play like normal.

## Configure
    {
        "UseExtendedMode": false
    }
**ExtendedMode** adds the ability to harvest bark. Some content packs won't work correctly if this is disabled (Although you will be warned in the console if a mod requires ExtendedMode and you have it disabled). Enabling this will add a new item called the **Bark Remover** that can be purchased from [Robin](https://stardewvalleywiki.com/Robin) for 5k.

## Compatibility
More Trees is compatible with Stardew Valley 1.4+ on Windows/Mac/Linus, both single player and multiplayer. To view reported bugs visit both the issues on this repo and bug reports on [Nexus](https://www.nexusmods.com/stardewvalley/mods/?tab=bugs).

TODO: add mod ids to compat and install sections