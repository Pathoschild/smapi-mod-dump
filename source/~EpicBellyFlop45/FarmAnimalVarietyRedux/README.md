**Farm Animal Variety Redux** is a [Stardew Valley](http://stardewvalley.net/) framework mod that allows you to add custom animals using json.

![](pics/favr.png)

## Converting a BFAV Pack
To convert an existing BFAV pack to the FAVR layout download the **BFAVToFAVRModConverter** on [Nexus](), once downloaded extract the **.zip** anywhere on your computer. Copy in the **BFAV** content packs in the **\BFAVMods** folder, then run the **Convert.bat** file. This will create each mod in the FAVR format in the **\FAVRMods** folder.

**NOTE:** This will isn't a perfect converter and will still require you to go through the files to validate them. This converter is only meant to do the heavy lifting of pack conversion.

## Creating a Content Pack
1. Create a new folder for the content pack. The convention is: **[FAVR] mod name**.
2. Create a sub folder for each animal you to add (see final result below for reference).
3. Create a sub folder (inside each animal folder) called **assets** (see final result below for reference).
4. Create a **shopdisplay.png** image and add to the respective animal folder. **NOTE:** the image must be called **shopdisplay.png**. This should be **32px x 16px**
5. (Optional) Create a **sound.wav** audio file (if the animal will make a custom sound) and add to the respective animal folder. **NOTE:** the audio file must be called **sound.wav**.
6. Create a **content.json** and add to the respective animal folder (see below for reference). 
7. Create a **manifest.json** (see below for reference).

#### Final Content Pack Layout
    [FAVR] Big Cats
        manifest.json
        Tiger
            assets
                Baby Bengai.png
                Baby Siberian.png
                Baby Sumatran.png
                Bengai.png
                Siberian.png
                Sumatran.png
                Harvested Bengai.png
                Harvested Siberian.png
                Harvested Sumatran.png
            content.json
            shopdisplay.png
            sound.wav
        Cheetah
            assets
                winter
                    Baby Asiatic.png
                    Baby Acinonyx.png
                    Baby South African.png
                Baby Asiatic.png
                Baby Acinonyx.png
                Baby South African.png
                Asiatic.png
                Acinonyx.png
                South African.png
            content.json
            shopdisplay.png
            sound.wav
        Lion
            assets
                Asiatic.png
            content.json
            shopdisplay.png
        

#### Assets folder
The way the assets system works is an 'order of priority' of sorts. The assets folder can contain sub folders for each season, as seen in the Cheetah example. These images have a higher 'priority', what this translates to in game is: in winter, the baby Cheetahs will have the textures that are in the winter folder, while all other seasons will be textured using the ones in the assets folder.

The names for the images have to be exactly the same as in the **content.json** below with either **"Baby "** or **"Harvested "** concatenated on the front (obviously depending on if the sheet is for baby or harvested varients).

The only required asset is an adult sheet (see Lion example above), the animal will still have a child phase (unless specified in **content.json**), however, they will look exactly the same to their adult versions.

Also, a "default" adult sheet (one that is in the assets folder) isn't required if each season has a valid adult sheet

#### Content.json example
    {
        "Name": "Tiger",
        "Buyable": true,
        "UpdatePreviousAnimal": false,
        "AnimalShopInfo": {
            "Description": "A fierce tiger",
            "BuyPrice": 10000
        },
        "Types": [
            {
                "Name": "Bengai",
                "Produce": {
                    "AllSeasons": {
                        "Products": [
                            {
                                "Id": "20",
                                "HarvestType": 0,
                                "ToolName": null,
                                "HeartsRequired": 0
                            }
                        ],
                        "DeluxeProducts": [
                            {
                                "Id": "22",
                                "HarvestType": 0,
                                "ToolName": null,
                                "HeartsRequired": 0
                            }
                        ],
                    },
                    "Spring": null,
                    "Summer": null,
                    "Fall": null,
                    "Winter": null
                }
            },
            {
                "Name": "Siberian",
                "Produce": {
                    "AllSeasons": {
                        "Products": [
                            {
                                "Id": "60",
                                "HarvestType": 1,
                                "ToolName": "Shears",
                                "HeartsRequired": 0
                            }
                        ],
                        "DeluxeProducts": [
                            {   
                                "Id": "62",
                                "HarvestType": 1,
                                "ToolName": "Shears",
                                "HeartsRequired": 0
                            }
                        ],
                    },
                    "Spring": null,
                    "Summer": null,
                    "Fall": null,
                    "Winter": null
                }
            },
            {
                "Name": "Sumatran",
                "Produce": {
                    "AllSeasons": {
                        "Products": [
                            {
                                "Id": "SpaceChase0.JsonAssets:GetObjectId:Tiger Fur",
                                "HarvestType": 2,
                                "ToolName": null,
                                "HeartsRequired": 0
                            }
                        ],
                        "DeluxeProducts": [
                            {   
                                "Id": "Tiger Fang",
                                "HarvestType": 2,
                                "ToolName": null,
                                "HeartsRequired": 0
                            }
                        ],
                    },
                    "Spring": null,
                    "Summer": null,
                    "Fall": null,
                    "Winter": null
                }
            }
        ],
        "DaysToProduce": 2,
        "DaysTillMature": 4,
        "SoundId": null,
        "FrontAndBackSpriteWidth": 32,
        "FrontAndBackSpriteHeight": 32,
        "SideSpriteWidth": 32,
        "SideSpriteHeight": 32,
        "FullnessDrain": 15,
        "HappinessDrain": 5,
        "Buildings": [
            "Deluxe Barn"
        ],
        "WalkSpeed": 2,
        "SeasonsAllowedOutdoors": [
            0,
            1,
            2
        ]
    }

* **Name**: The name of the animal.
* **Buyable**: Whether the animal can be bought from Marnie's shop.
* **UpdatePreviousAnimal**: Whether the animal should update a previously added animal.
* **AnimalShopInfo.Description**: The description of the animal (That's shown in Marnie's shop).
* **AnimalShopInfo.BuyPrice**: The amount the animal costs.
* **Types**: Data about an animal sub type.
* **Types.Name**: The name of the sub type.
* **Types.Produce**: Data about an animal's item production.
* **Types.Produce.{season}**: Which season the production items apply to (**AllSeasons**, **Spring**, **Summer**, **Fall**, **Winter**).
* **Types.Produce.{season}.{productType}**: Which type the production items apply to ("Products", "Deluxe Products").
* **Types.Produce.{season}.{produceType}.Id**: The id of the product. **NOTE:** this support API tokens (see Sumatran for example) the layout is: **uniqueId:methodName:value**.
* **Types.Produce.{season}.{produceType}.HarvestType**: The harvest type of the product (**0**: Lay (produce is placed on floor indoors), **1**: Tool (produce is harvested from animal using below tool), **2** Forage (produce is found in the floor outside)).
* **Types.Produce.{season}.{produceType}.ToolName**: The name of the tool required to harvest to the product (Only used with a harvest type of **1**)
* **Types.Produce.{season}.{produceType}.HeartsRequired**: The number of friendship hearts required for the animal to produce the product.
* **Types.Produce.{season}.{produceType}.PercentChance**: The percent chance of the object being produced.
* **DaysToProduce**: The number of days it takes the animal to produce product.
* **DaysTillMature**: The number of days it takes the animal to become an adult.
* **SoundId**: The id of the sound the animal will make. **NOTE:** leave this blank if you're using a custom sound (**sound.wav**). **SoundId** is to be used with vanilla sound ids only.
* **FrontAndBackSpriteWidth**: The width of the animal sprite when it's looking toward / away from the camera.
* **FrontAndBackSpriteHeight**: The height of the animal sprite when it's looking toward / away from the camera.
* **SideSpriteWidth**: The width of the animal sprite when it's looking to the side.
* **SideSpriteHeight**: The height of the animal sprite when it's looking to the side.
* **FullnessDrain**: The amount the animal's hunger bar will drain each night.
* **HappinessDrain**: The amount the animal's happiness bar will drain each night.
* **Buildings**: The name(s) of the building(s) the animal can be housed in. **NOTE:** each building type should be treated like a separate building, for example: (**Coop, Big Coop, Deluxe Coop**) names must be exactly the same as building names.
* **WalkSpeed**: The walk speed of the animal. **NOTE:** this doesn't support decimals, the default value is 2
* **SeasonsAllowedOutdoors**: The seasons the animal is able to go outside (**0**: Spring, **1**: Summer, **2**: Fall, **3**: Winter).

**NOTE:** Ensure all ids are strings, this is because they also allow API tokens (The layout is: **UniqueModId:MethodName:Value**). When refering to a JA item, you can exclude the **UniqueModId:MethodName:** and just put the item name an example of an API token is: **SpaceChase0.JsonAssets:GetObjectId:Tiger Fang**, this will use an item from JA called **Tiger Fang**. **NOTE**: in this instance you can also put just **Tiger Fang** for the same effect (example can be seen on the Tiger animal above).

#### Manifest.json example
    {
        "Name": "[FAVR] mod name",
        "Author": "your name",
        "Version": "1.0.0",
        "Description": "mod description",
        "UniqueId": "your name.mod name",
        "UpdateKeys": [],
        "ContentPackFor": {
            "UniqueID": "EpicBellyFlop45.FarmAnimalVarietyRedux"
        }
    }

## Install
1. Install the latest version of [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400).
2. Install the latest version of [this mod](https://www.nexusmods.com/stardewvalley/mods/).
3. Extract the .zip mod file into your StardewValley/Mods folder and run the game using SMAPI.

## Use
Add any content packs to the **StardewValley/Mods** file 
Open the game using SMAPI like normal.

## Compatibility
Farm Animal Variety Redux is compatible with Stardew Valley 1.4+ on Windows/Mac/Linux, both single player and multiplayer. To view reported bugs visit both the issues on this repo and bug reports on [Nexus](https://www.nexusmods.com/stardewvalley/mods/?tab=bugs).

TODO: add mod ids to compat and install sections