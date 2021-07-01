**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/AndyCrocker/StardewMods**

----

**Farm Animal Variety Redux** is a [Stardew Valley](http://stardewvalley.net/) framework mod that allows you to add custom animals using json.

![]()

## Creating a Content Pack
1. Create a new folder for the content pack. The convention is: **[FAVR] mod name**.
2. Create a sub folder for each animal you plan to add/edit (see final result below for reference).
3. Create a sub folder (inside each animal folder) called **assets** (see final result below for reference), if you're editing an animal and don't plan to edit its sprite sheets, this can be ignored.
4. Create a **shopdisplay.png** image (if the animal is going to able to be bought) and add to the respective animal folder (this should be **32px x 16px**), if you're editing an animal and don't plan to edit the shop icon, this can be ignored.
5. (Optional) Create a **sound.wav** audio file (if the animal will make a custom sound) and add to the respective animal folder.
6. Create a **content.json** for each animal folder (see below for reference).
7. (Optional) Create an **incubator.json** (see below for reference).
8. Create a **manifest.json** (see below for reference).

### Final Content Pack Layout
    [FAVR] Big Cats
        manifest.json
        incubator.json
        Tiger
            assets
                Baby Bengal.png
                Baby Siberian.png
                Baby Sumatran.png
                Bengal.png
                Siberian.png
                Sumatran.png
                Harvested Bengal.png
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
The way the assets system works is an 'order of priority' of sorts. The assets folder can contain sub folders for each season, as seen in the Cheetah example above. These images have a higher 'priority', what this translates to in game is: in winter, the baby Cheetahs will have the textures that are in the winter folder, while all other seasons will be textured using the ones in the assets folder.

The names for the images have to be exactly the same as in the **content.json** below with either **"Baby "** or **"Harvested "** concatenated on the front (obviously depending on if the sheet is for the baby or harvested varients).

The only required asset is an adult sheet (see Lion example above), the animal will still have a child phase (unless specified in **content.json**), however, they will look exactly the same to their adult versions.

Also, a "default" adult sheet (the one that is in the root of the assets folder) isn't required if each season has a valid adult sheet in its sub folder.

### Content.json example
**Note: this contains all available properties and doesn't reflect what the Big Cats example above would look like, this is purely to show what a content file will look like with all its properties, see the property data below to see which are optional and their default values**

    {
        "Action": "Add",
        "InternalName": null,
        "Name": "",
        "Buyable": true,
        "CanSwim": false,
        "AnimalShopInfo": 
        {
            Description: "",
            BuyPrice = 1
        },
        "Subtypes": [
            {
                "Action": "Add",
                "InternalName": null,
                "Name": "",
                "IsBuyable": true,
                "IsIncubatable": true,
                "Produce": [
                    {
                        "Action": "Add",
                        "DefaultProductId": "-1",
                        "DefaultProductMinFriendship": 0,
                        "DefaultProductMaxFriendship": 1000,
                        "UpgradedProductId": "-1",
                        "UpgradedProductMinFriendship": 200,
                        "UpgradedProductMaxFriendship": 1000,
                        "PercentChanceForUpgradedProduct": null,
                        "UpgradedProductIsRare": false,
                        "HarvestType": "Lay",
                        "DaysToProduce": 1,
                        "ProduceFasterWithCoopMaster": false,
                        "ProduceFasterWithShepherd": false,
                        "ToolName": null,
                        "ToolHarvestSound": null,
                        "Amount": 1,
                        "Seasons": [ "spring", "summer", "fall", "winter" ],
                        "PercentChance": 100,
                        "PercentChanceForOneExtra": 0,
                        "RequiresMale": null,
                        "RequiresCoopMaster": null,
                        "RequiresShepherd": null,
                        "StandardQualityOnly": false,
                        "DoNotAllowDuplicates": false
                    }
                ],
                "DaysTillMature": 3,
                "SoundId": null,
                "FrontAndBackSpriteWidth": 16,
                "FrontAndBackSpriteHeight": 16,
                "SideSpriteWidth": 16,
                "SideSpriteHeight": 16,
                "MeatId": "-1",
                "HappinessDrain": 255,
                "FullnessGain": null,
                "HappinessGain": null,
                "AutoPetterFriendshipGain": 7,
                "HandPetFriendshipGain": 15,
                "WalkSpeed": 2,
                "BabySellPrice": null,
                "AdultSellPrice": null,
                "IsMale": null,
                "SeasonsAllowedOutdoors": [ "spring", "summer", "fall" ]
            }
        ],
        "Buildings": [ "coop", "big coop" ]
    }

**Note: Any property with a default value specified below is optional, those without a default value are required.**

#### Main Animal Properties
Property       | Default value | Description
-------------- | :-----------: | -----------
Action         | `"Add"`       | See [Special Properties](#special-properties) for details.
InternalName   | `null`        | See [Special Properties](#special-properties) for details.
Name           |               | The name of the animal.
IsBuyable      | `true`        | Whether the animal can be bought at Marnie's shop.
CanSwim        | `false`       | Whether the animal can swim.
AnimalShopInfo | `null`        | See [Shop Info Properties](#shop-info-properties) for details.
Subtypes       |               | See [Subtype Properties](#subtype-properties) for details.
Buildings      |               | The buildings that the animal can live in, these are case insensitive. **Note:** *all* building types an animal can live must be specified, for example: `["coop", "big coop", "deluxe coop"]` (if the animal can live in all three).

#### Shop Info Properties
Property    | Description
----------- | -----------
Description | The description that will get displayed in the animal purchase menu.
BuyPrice    | The amount the animal costs to buy. **Note:** if the `BabySellPrice` and `AdultSellPrice` are both unspecified in [Subtype Properties](#subtype-properties) then this property is used to determine the sell price of the animal using the base game calculation.

#### Subtype Properties
Property                 | Default value                    | Description
------------------------ | :------------------------------: | -----------
Action                   | `"Add"`                          | See [Special Properties](#special-properties) for details.
InternalName             | `null`                           | See [Special Properties](#special-properties) for details.
Name                     |                                  | The name of the subtype.
IsBuyable                | `true`                           | Whether the subtype can be picked when buying the animal at Marnie's shop. **Note:** this property will be ignored if there are no subtypes that are buyable.
IsIncubatable            | `true`                           | Whether the subtype can be picked when an incubator recipe has finished that has specified an animal name (and not a subtype name.) **Note:** this property will be ignored if an incubator recipe has specifically set this subtype as the result.
Produce                  |                                  | See [Produce  Properties](#produce-properties) for details.
AllowForageRepeats       | `true`                           | Whether the same forage product can be found multiple times in the same day. This uses the same calculation as the base game.
DaysTillMature           | `3`                              | The number of days it takes the subtype to become an adult.
SoundId                  | `null`                           | The sound bank id the subtype will play. **Note:** this won't get used if the animal this subtype belongs to has a custom `sound.wav` file in it's folder.
FrontAndBackSpriteWidth  | `16`                             | The width of the subtype sprite when it's looking toward / away from the camera.
FrontAndBackSpriteHeight | `16`                             | The height of the subtype sprite when it's looking toward / away from the camera.
SideSpriteWidth          | `16`                             | The width of the subtype sprite when it's looking to the side.
SideSpriteHeight         | `16`                             | The height of the subtype sprite when it's looking to the side.
MeatId                   | `"-1"`                           | The id of the meat of the subtype (also accepts api tokens, see [Api Tokens](#api-tokens).)
HappinessDrain           | `7`                              | A value between 0 and 255 that determines the amount of the subtype's happiness bar will drain each night, when they're not pet, or not fed.
FullnessGain             | `255`                            | A value between 0 and 255 that determines the amount of the subtype's hunger bar will fill each time they eat something.
HappinessGain            | `(40 - HappinessDrain)`          | The amount of extra happiness an animal will get when being pet when the player has either the Coop Master or Shepherd profession (which ever correlates to the type of building an animal lives in). **Note:** when specifying the value you cannot use maths operations like in the default value.
AutoPetterFriendshipGain | `7`                              | The amount of the subtype's friendship bar will fill up each time they get petted by an auto petter.
HandPetFriendshipGain    | `15`                             | The amount of the subtype's friendship bar will full up each time they get petted by hand.
WalkSpeed                | `2`                              | The walk speed of the subtype.
BabySellPrice            | `null`                           | The sell price of the subtype when it's a baby.
AdultSellPrice           | `null`                           | The sell price of the subtype when it's an adult.
IsMale                   | `null`                           | Whether the subtype is always a male. **Note:** if `true` is specified the animal will always be male, if `false` is specified the animal will always be female, and if `null` is specified there's a 50% chance of either gender.
SeasonsAllowedOutdoors   | `"["spring", "summer", "fall"]"` | The seasons the subtype is able to go outside.

#### Produce Properties
Property                        | Default value                              | Description
------------------------------- | :----------------------------------------: | -----------
UniqueName                      |                                            | The unique string to identify this produce among others on the same subtype, therefore this only needs to be unique among other produce on the same subtype.
Action                          | `"Add"`                                    | See [Special Properties](#special-properties) for details.
DefaultProductId                | `"-1"`                                     | The id of the default product (also accepts api tokens, see [Api Tokens](#api-tokens).)
DefaultProductMinFriendship     | `0`                                        | The minimum friendship required for the default product to drop.
DefaultProductMaxFriendship     | `1000`                                     | The maximum friendship allowed for the default product to drop.
UpgradedProductId               | `"-1"`                                     | The id of the upgraded product (also accepts api tokens, see [Api Tokens](#api-tokens).)
UpgradedProductMinFriendship    | `200`                                      | The minimum friendship required for the upgraded product to drop.
UpgradedProductMaxFriendship    | `1000`                                     | The maximum friendship allowed for the upgraded product to drop.
PercentChanceForUpgradedProduct | `null`                                     | The percent chance of the updated product to drop. **Note:** if `null` is specified, then the chance is calculated using the base game calculation.
UpgradedProductIsRare           | `false`                                    | Whether the upgraded product is a 'rare product' (like the Rabbit Foot or Duck Feather.)
HarvestType                     | `"Lay"`                                    | The harvest type of the product. The allowed values are: `"Lay"`, `"Tool"`, and `"Forage"`.
DaysToProduce                   | `1`                                        | The number of days between each time the item gets produced.
ProduceFasterWithCoopMaster     | `false`                                    | Whether `DaysToProduce` should be reduced by one if the player has the Coop Master profession.
ProduceFasterWithShepherd       | `false`                                    | Whether `DaysToProduce` should be reduced by one if the player has the Shepherd profession.
ToolName                        | `null`                                     | The name of the tool required to harvest the product (when the `HarvestType` is `"Tool"`.)
ToolHarvestSound                | `null`                                     | The sound bank id to play when harvesting the animal with a tool.
Amount                          | `1`                                        | The amount of items that get produced at once.
Seasons                         | `"["spring", "summer", "fall", "winter"]"` | The season the product can be produced.
PercentChance                   | `100`                                      | The percent chance of the object being produced.
PercentChanceForOneExtra        | `0`                                        | The percent chance of the object producing one extra in its stack.
RequiresMale                    | `null`                                     | Whether the animal must be male to produce the item. **Note:** if `true` is specified then animal must be male, if `false` is specified the animal must be female, and if `null` is specified then either gender can produce the item.
RequiresCoopMaster              | `null`                                     | Whether the player must have the Coop Master profession for the animal to produce the item. **Note:** if `true` is specified then the farmer must have the Coop Master profession for the animal to drop the item, if `false` is specified then the farmer must not have the Coop Master profession for the animal to drop the item, if `null` is specified then it doesn't matter whether the farmer has it or not.
RequiresShepherd                | `null`                                     | Whether the player must have the Shepherd profession for the animal to produce the item. **Note:** if `true` is specified then the farmer must have the Shepherd profession for the animal to drop the item, if `false` is specified then the farmer must not have the Shepherd profession for the animal to drop the item, if `null` is specified then it doesn't matter whether the farmer has it or not.
StandardQualityOnly             | `false`                                    | Whether the product should be standard quality only.
DoNotAllowDuplicates            | `false`                                    | Whether the item can not be produced if it's in the player possession. An object is in the players possession if it placed in an animal building or on the farm, in a farmer's inventory, or in a chest.

#### Special properties
Property     | Description
------------ | -----------
Action       | Determines how the data should be interpreted. The allowed values are: `"Add"`, `"Edit"`, and `"Delete"`. **Note:** if this is either `"Edit"` or `"Delete"` then the `InternalName` (or in the case of produce, the `UniqueName`) *must* be specified in order to be valid.
InternalName | The internal name of the animal or animal subtype (case insensitive). This is set to the `mod unique id.name` for example if the mod: `Satozaki.CustomAnimals` adds an animal: `Elephant` the internal name of the animal will be: `Satozaki.CustomAnimals.Elephant`, if the animal has a subtype called `Grey Elephant` then the subtype's internal name will be `Satozaki.CustomAnimals.Grey Elephant`. Any animals that are added by the game have a `unique id` of `game`, for example: `game.Chicken`. **Note:** The internal name cannot be changed, so even if a pack edits the animal name the internal name won't be updated to reflect the name change, this is by design so multiple packs can edit the same animal without worrying about other packs changing the name of the animal. Finally, this is only required when the `Action` is either `"Edit"` or `"Delete"` as it gets automatically generated when the action is `"Add"`.

#### Api Tokens
An api token is a way for you to retrieve data from other mods using their api. The format of an api token is: `"UniqueModId:MethodName:Value"`, an example of an api token is: `spacechase0.JsonAssets:GetObjectId:Rainbow Egg`, this will use an item from Json Assets called `Rainbow Egg`. Not all properties support api tokens, the properties that support api tokens will mention it in their description.

### Incubator.json example
The incubator file is used for specifying custom incubator recipes using either incubator type.

    [
        {
            "IncubatorType": "Regular",
            "InputId": 72,
            "Chance": 1.0,
            "MinutesTillDone": 9000,
            "InternalAnimalName": "game.Chicken"
        }
    ]

Property           | Default value | Description
------------------ | ------------- | -----------
IncubatorType      | `"Regular"`   | The type of incubator the recipe will apply to. The allowed values are: `"Regular"`, `"Ostrich"`, and `"Both"`.
InputId            | `"-1"`        | The id of the item to input into the incubator (also accepts api tokens, see [Api Tokens](#api-tokens).) **Note:** this doesn't need to be unique, if there are multiple recipes with the same input id the below property is used to determine which should be picked.
Chance             | `1.0`         | The chance of this recipe being picked when there are multiple recipes with the same input id. For example, a recipe with a chance of `4` will have 4x more of a chance of being picked over a recipe with a chance of `1`.
MinutesTillDone    | `9000`        | The number of in-game minutes it takes for the incubator to finish. **Note:** as reference, in the base game the regular incubator takes 9000 minutes and the ostrich incubator takes 15000 minutes. FAVR will always have the default value of 9000 even if `IncubatorType` is `"Ostrich"`, if you want to emulate base game behavior using the ostrich incubator, you must specify `15000`.
InternalAnimalName |               | The internal name of either the animal or animal subtype that will be produced. If the internal name of an animal is specified, then any subtype whose `IsIncubatable` property is set the `true` may be picked. If the internal name of an animal's subtype is specified, then only that subtype can be picked regardless of its `IsIncubatable` property. For more info about internal names, see [Special Properties](#special-properties).

### Manifest.json example
    {
        "Name": "[FAVR] mod name",
        "Author": "your name",
        "Version": "1.0.0",
        "Description": "description",
        "UniqueID": "your name.mod name",
        "MinimumApiVersion": "3.8.0",
        "UpdateKeys": [ update key ],
        "ContentPackFor": {
            "UniqueID": "Satozaki.FarmAnimalVarietyRedux"
        }
    }

## Editing with Content Patcher
FAVR enables you to edit both the sprite sheets and purchase menu icons through Content Patcher using custom tokens, these are:

Property     | Description
------------ | -----------
GetAssetPath | Gets the asset path of either an animal sprite sheet or an animal purchase menu icon.
GetSourceX   | Gets the X position of the purchase menu icon source rectangle.
GetSourceY   | Gets the Y position of the purchase menu icon source rectangle.

(See: [Content Patcher Examples](#content-patcher-examples) for full examples, or see below for a description of each property.)

### GetAssetPath
#### Getting an animal sprite sheet
Requires 5 parameters: **internal animal name**, **internal subtype name**, **isBaby**, **isHarvested**, **season**.

Parameter             | Type                       | Description
--------------------- | :------------------------: | -----------
Internal Animal Name  | `string`                   | The *internal* animal name, see [Special Properties](#special-properties) for details.
Internal Subtype Name | `string`                   | The *internal* subtype name, see [Special Properties](#special-properties) for details.
IsBaby                | `bool` (`true` or `false`) | Whether the sprite sheet to retrieve should be of the baby version of the animal.
IsHarvested           | `bool` (`true` or `false`) | Whether the sprite sheet to retrieve should be of the harvested version of the animal.
Season                | `string`                   | The season whose sprite sheet should be retrieved of the animal.

(See [Editing a sprite sheet](#editing-a-sprite-sheet) for an example)

#### Getting an animal purchase menu icon
Requires 1 parameter: **internal animal name**

Parameter             | Type     | Description
--------------------- | :------: | -----------
Internal Animal Name  | `string` | The *internal* animal name, see [Special Properties](#special-properties) for details.

(See [Editing a purchase menu icon](#editing-a-purchase-menu-icon) for an example)

### GetSourceX
When patching the source rectangle of a purchase menu icon, it's important to specify the source X and Y position, this is because the icons can be inside a sprite sheet (instead of their own texture, in the case of default shop icons.)

Requires 1 parameter: **internal animal name**

Parameter             | Type     | Description
--------------------- | :------: | -----------
Internal Animal Name  | `string` | The *internal* animal name, see [Special Properties](#special-properties) for details.

(See [Editing a purchase menu icon](#editing-a-purchase-menu-icon) for an example)

### GetSourceY
When patching the source rectangle of a purchase menu icon, it's important to specify the source X and Y position, this is because the icons can be inside a sprite sheet (instead of their own texture, in the case of default shop icons.)

Requires 1 parameter: **internal animal name**

Parameter             | Type     | Description
--------------------- | :------: | -----------
Internal Animal Name  | `string` | The *internal* animal name, see [Special Properties](#special-properties) for details.

(See [Editing a purchase menu icon](#editing-a-purchase-menu-icon) for an example)

### Content Patcher Examples
#### Editing a sprite sheet
Replaces the **adult**, **non harvested** sprite sheet of the `Grey Elephant` added by `Satozaki.CustomAnimals` with `assets/grey elephant {{season}}.png`:

    "Changes": [
        {
            "Action": "EditImage",
            "Target": "{{Satozaki.FarmAnimalVarietyRedux/GetAssetPath: Satozaki.CustomAnimals.Elephant, Satozaki.CustomAnimals.Grey Elephant, false, false, {{season}}}}",
            "FromFile": "assets/grey elephant {{season}}.png",
            "When": {
                "HasMod": "Satozaki.FarmAnimalVarietyRedux"
            }
        }
    ]


Replaces the **baby**, **non harvested** sprite sheet of the `Grey Elephant` added by `Satozaki.CustomAnimals` with `assets/grey elephant {{season}}.png`:

    "Changes": [
        {
            "Action": "EditImage",
            "Target": "{{Satozaki.FarmAnimalVarietyRedux/GetAssetPath: Satozaki.CustomAnimals.Elephant, Satozaki.CustomAnimals.Grey Elephant, true, false, {{season}}}}",
            "FromFile": "assets/grey elephant {{season}}.png",
            "When": {
                "HasMod": "Satozaki.FarmAnimalVarietyRedux"
            }
        }
    ]


Replaces the **adult**, **harvested** sprite sheet of the `Grey Elephant` added by `Satozaki.CustomAnimals` with `assets/grey elephant {{season}}.png`:

    "Changes": [
        {
            "Action": "EditImage",
            "Target": "{{Satozaki.FarmAnimalVarietyRedux/GetAssetPath: Satozaki.CustomAnimals.Elephant, Satozaki.CustomAnimals.Grey Elephant, false, true, {{season}}}}",
            "FromFile": "assets/grey elephant {{season}}.png",
            "When": {
                "HasMod": "Satozaki.FarmAnimalVarietyRedux"
            }
        }
    ]

#### Editing a purchase menu icon
Replaces the purchase menu icon of the `Elephant` added by `Satozaki.CustomAnimals` with `assets/shopicon.png`:

    "Changes": [
    {
            "Action": "EditImage",
            "Target": "{{Satozaki.FarmAnimalVarietyRedux/GetAssetPath: Satozaki.CustomAnimals.Elephant}}",
            "ToArea": {
                "X": "{{Satozaki.FarmAnimalVarietyRedux/GetSourceX: Satozaki.CustomAnimals.Elephant}}",
                "Y": "{{Satozaki.FarmAnimalVarietyRedux/GetSourceY: Satozaki.CustomAnimals.Elephant}}",
                "Width": 32,
                "Height": 16
            },
            "FromFile": "assets/shopicon.png",
            "When": {
                "HasMod": "Satozaki.FarmAnimalVarietyRedux"
            }
        }
    ]

## Install
1. Install the latest version of [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400).
2. Install the latest version of [this mod](https://www.nexusmods.com/stardewvalley/mods/).
3. Extract the .zip mod file into your StardewValley/Mods folder and run the game using SMAPI.

## Use
Add any content packs to the **StardewValley/Mods** file and run the game using SMAPI.

## Configure
A configuration file will get generated the first time the game is launched through SMAPI with the mod installed, an example of the configuration file can be seen below:

    {
        "BedTime": 1900
    }

Property | Default value | Description
-------- | ------------- | -----------
BedTime  | 1900          | The time that animals will go to bed at. The time format is the hours with the minutes contatenated on the end, for example `1900` is `19` hours (7pm) with 0 minutes.

## Compatibility
Farm Animal Variety Redux is compatible with Stardew Valley 1.5+ on Windows/Mac/Linus, both single player and multiplayer. To view reported bug visit both the issues on this repo and bug reports on [Nexus](https://www.nexusmods.com/stardewvalley/mods/?tab=bugs).

// TODO: ids