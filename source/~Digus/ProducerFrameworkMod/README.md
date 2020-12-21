**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Digus/StardewValleyMods**

----

[Producer Framework Mod](https://www.nexusmods.com/stardewvalley/mods/4970) is a [Stardew Valley](http://stardewvalley.net/) mod which allows custom machines to be added to the game & to modify the input/output of vanilla machines..
                                                                                                           
**This documentation is for modders. If you're a player, see the [Nexus page](https://www.nexusmods.com/stardewvalley/mods/4970) instead.**
                                                                                                           
## Contents 
* [Install](#install)
* [Introduction](#introduction)
  * [What is Producer Framework Mod?](#what-is-producer-framework-mod)
  * [Companion Mods](#companion-mods)
* [Basic Features](#basic-features)
  * [Overview](#overview)
  * [producerRules](#producerRules)
    * [Star Quality Inputs](#star-quality-inputs)
    * [OutputColorConfig](#outputcolorconfig)
    * [AdditionalOutputs](#additionaloutputs)
    * [LookForInputWhenReady](#lookforinputwhenready)
  * [producersConfig](#producersconfig)
* [Machine Animations](#machine-animations)
* [FAQ](#faq)
    * [Custom Name Already in Use](#custom-name-already-in-use)
* [See Also](#see-also)

## Install
1. [Install the latest version of SMAPI](https://smapi.io/).
2. Install [this mod from Nexus mods](https://www.nexusmods.com/stardewvalley/mods/4970).
3. Unzip any Producer Framework Mod content packs into `Mods` to install them.
4. Run the game using SMAPI.

## Introduction
### What is Producer Framework Mod?
Producer Framework Mod allows you to add custom machines to the game and to modify the input/output of vanilla machines without having to create a SMAPI mod or altering vanilla files.

I highly recommend looking up preexisting content packs for further examples:

* [Artisan Valley](https://www.nexusmods.com/stardewvalley/mods/1926) 
* [Artisanal Soda Makers](https://www.nexusmods.com/stardewvalley/mods/5173)
* [Trents New Animals](https://www.nexusmods.com/stardewvalley/mods/3634)

### Companion Mods
There are other frameworks out there that pair well with Producer Framework Mod:

 * [PFMAutomate](https://www.nexusmods.com/stardewvalley/mods/5038) adds support for [Automate](https://www.nexusmods.com/stardewvalley/mods/1063),
 * [Content Patcher](https://www.nexusmods.com/stardewvalley/mods/1915).
 * [Json Assets](https://www.nexusmods.com/stardewvalley/mods/1720) to add custom inputs/outputs and to add the machine sprites..
 * [Mail Framework Mod](https://www.nexusmods.com/stardewvalley/mods/1536) to send objects & cooking/crafting recipes.
 * [Custom Cask Mod](https://www.nexusmods.com/stardewvalley/mods/2642) to add custom items as valid cask inputs.
 * [Custom Producer Mod](https://www.nexusmods.com/stardewvalley/mods/4992) includes a list of all vanilla inputs/outputs.

## Basic Features
### Overview
There are two main files you will see when downloading a content pack.
* producerRules.json
* producersConfig.json

You will also see a `manifest.json` for SMAPI to read (see [content packs](https://stardewvalleywiki.com/Modding:SMAPI_APIs#Manifest) on the wiki). These names are case sensitive and will not work on MacOS is the case is changed.

### producerRules
All lines that have a default value can be removed and that value will be used. You can only have one producer rule per input. To have additional outputs you will need to use `AdditionalOutputs`.

The `producerRules.json` can contain these fields:

field                                   | purpose
----------------------------------------|--------
`ProducerName`                          | The name you would like your producer to have.
`InputIdentifier`                       | The identifier of the input. Can be the Index of the object, the category of the object if it's a negative value. Can be the name of the object, or a context_tag of the object. Can be null only if there is a `NoInputStartMode` for the producer. When using an object ID make sure to put quotes around the number. This is especially important for categories. Default is `null`.
`InputStack`                            | _(optional)_ The required stack of the input. Default value is `1`.
`ExcludeIdentifiers`                    | _(optional)_ List of identifiers to exclude from the rule. Follow the same rules for `InputIdentifier`. Value must be in brackets and quotes. Example: `[ "Peach" ]` or `[ "248" ]`. Default value is `null`.
`FuelIdentifier`                        | _(optional)_ The identifier of the fuel. Fuel is the extra item used when producing. Can be the Index, the Name or the category of the object. Default is `null`.
`FuelStack`                             | _(optional)_ The required stack of the Fuel. Only used if fuel is not null. Default is `1`. 
`AdditionalFuel`                        | _(optional)_ You don't need to set a Fuel to use `AdditionalFuel`. The format is a pair of identifier and stack amount, following the same rules of the other fuel property. Can use categories, object IDs, and custom object names.Default is `null`.
`MinutesUntilReady`                     | The amount of minutes it takes to produce. Should be divisible by 10. Required.
`SubtractTimeOfDay`                     | If `MinutesUntilReady` should be subtracted by the current time of day. Set to `true` or `false`.
`OutputIdentifier`                      | The identifier of the output. Can be the Index or the Name of the object. Required.
`OutputName`                            | _(optional)_ The Name of the object. This will replace the basic output name. It accepts 4 tags that are dynamically replaced. Default is `null`.
`OutputTranslationKey`                  | _(optional)_ The translation key. This is the key in the i18n file where the mod will look for the format of the OutputName in other languages. The value of this key follows the same rules of the OutputName property above. Default is `null`. "PFM only supports one translation name per output index."[Source](https://forums.nexusmods.com/index.php?/topic/8257593-producer-framework-mod/page-9#entry77508673) 
`OutputGenericParentName`               | _(optional)_ The generic parent name is for when there is no parent name for the object that needs a parent, like Wild for Honey. Default is `null`.
`OutputGenericParentNameTranslationKey` | _(optional)_ The translation key. This is the key in the i18n file where the mod will look for the OutputGenericParentName in other languages. Default is `null`.
`PreserveType`                          | _(optional)_ [Wine|Jelly|Pickle|Juice|Roe|AgedRoe] - If the output is one of the vanilla game Preserves Type. Default is `nul`l.
`KeepInputParentIndex`                  | _(optional)_ When true the input of the preserved parent index will be used as the parent index of the output. Set to `true` or `false`. Default is `false`
`InputPriceBased`                       | _(optional)_ If the base price of the output should be replaced with the base price of the input. Set to `true` or `false`. Default is `false`.
`OutputPriceIncrement`                  | _(optional)_ Increment the base price by this amount. Default is `0`.
`OutputPriceMultiplier`                 | _(optional)_ Multiply the base price by this amount. Default is `1`.
`KeepInputQuality`                      | _(optional)_ If the output should have the same quality as the input. Set to `true` or `false`. Default is `false`.
`OutputQuality`                         | _(optional)_ Set the output quality. Default is `0`. Valid numbers are 0 (No Quality), 1 (Silver Quality), 2 (Gold Quality), 4 (Iridium Quality).
`OutputStack`                           | _(optional)_ Set the output stack quantity. Default is `1`.
`OutputMaxStack`                        | _(optional)_ Set the max output stack, if you want a random output stack. It'll be ignored if smaller than `OutputStack`. Default is `1`.
`SilverQualityInput`                    | _(optional)_ Define an alternative stack if they input is silver quality. Removing will ignore this since probability will be 0. See [Star Quality Inputs](#star-quality-inputs) for more information on the fields it contains.
`GoldQualityInput`                      | _(optional)_ Define an alternative stack if they input is gold quality. Removing will ignore this since probability will be 0. See [Star Quality Inputs](#star-quality-inputs) for more information on the fields it contains.
`IridiumQualityInput`                   | _(optional)_ Define an alternative stack if they input is iridium quality. Removing will ignore this since probability will be 0. See [Star Quality Inputs](#star-quality-inputs) for more information on the fields it contains.
`OutputColorConfig`                     | _(optional)_ If set, the output will be a Colored Object. Remember that colored objects need the next sprite in the sheet to be a pallet for the color. Default is null. See [OutputColorConfig](#outputcolorconfig) for more information on the fields it contains.
`AdditionalOutputs`                     | _(optional)_ Define a list of additional outputs. You don't need to set a default output to use the additional ones, the first additional output will be the default in that case. Only one output is given, randomly chosen between the options. Default is an empty list. See [AdditionalOutputs](#additionaloutputs) for more information on the fields it contains.
`Sounds`                                | List of sound to make when an input is placed. Default is an empty list. A list of sounds can be found [here](https://docs.google.com/spreadsheets/d/1CpDrw23peQiq-C7F2FjYOMePaYe0Rc9BwQsj3h6sjyo/edit#gid=1007039997).
`DelayedSounds`                         | _(optional)_ List of sounds to make after a delay. Default is an empty list.
`PlacingAnimation`                      | _(optional)_ [Bubbles|Fire|CharcoalKiltSmoke] Animation to make when placing the input. Default is `null`.
`PlacingAnimationColorName`             | _(optional)_ The color of the animation. A [color chart](http://www.foszor.com/blog/xna-color-chart/) can be found here. Default `White`.
`PlacingAnimationOffsetX`               | _(optional)_ The offset of the X axis. The offset is relative to the animation default location, not the position of the machine. Default is `0`.
`PlacingAnimationOffsetY`               | _(optional)_ The offset of the Y axis. The offset is relative to the animation default location, not the position of the machine. Default is `0`.
`IncrementStatsOnInput`                 | _(optional)_ Game stats to increment when a input is placed. Will increment the stats by the amount of the stack. Vanilla machines already increment stats when an item is removed from the machine. Only the Recycle Machine increment the stats on input. Default is an empty list.
`OverrideMod`                           | _(optional)_ If defined, this rule can override rules with the same producer/input combination from the mods in the list. Otherwise the first one to be loaded will be used. 
`LookForInputWhenReady`                 | _(optional)_ Set this config if you want to look for a specific object when ready. If the rule has an input, it will consume the input on placement, check restrictions to choose the output, but won't apply name, price, quality and similar modifications based on that input. The found object when ready will be used for these modifications instead. See [LookForInputWhenReady](#lookforinputwhenready) for more information.

#### Star Quality Inputs
Below is more information on the `SilverQualityInput`, `GoldQualityInput`, `IridiumQualityInput` field. 

This "will only apply if the input is exactly that quality. If an specific quality is not set, it will use the default stack for no quality. Same if the probability is not met} [Source](https://forums.nexusmods.com/index.php?/topic/8257593-producer-framework-mod/page-3#entry75988918).

All three utilize the same fields.

field             | purpose
------------------| -------
`Probability`     | Probability to use the alternative stack. Default is 0.
`OutputStack`     | Set the output stack quantity. Default is 1.
`OutputMaxStack`  | Set the max output stack, if you want a random output stack. It'll be ignored if smaller than `OutputStack`. Default is 1.

Example:

```
"SilverQualityInput": { 
      "Probability": 0.2, 
      "OutputStack": 2, 
      "OutputMaxStack": 4 
    },
```

#### OutputColorConfig
Below is more information on the `OutputColorConfig` field. 

field     | purpose
----------| -------
`Type`    | [ObjectColor|ObjectDyeColor|DefinedColor] If `ObjectColor` and the input is a colored object, it will use the color of the input. If `ObjectDyeColor` and the input has a dye color specified in the [context tags](https://github.com/spacechase0/JsonAssets/blob/master/README.md#context-tags), it will use this color. If `DefinedColor` or it does find the color on the other options, it will create the color based on the Red, Green and Blue values. Default is `DefinedColor`.
`Red`     | The red value of the color. Default is 255.
`Green`   | The green value of the color. Default is 255.
`Blue`    | The blue value of the color. Default is 255.

Example:

```
"OutputColorConfig":
        {
            "Type": "ObjectDyeColor",
            "Red": 204,
            "Green": 203,
            "Blue": 243
        },
```

#### AdditionalOutputs
Below is more information on the `AdditionalOutputs` field. All other properties that refer to the output can be used in the AdditionalOutputs. Remember to use commas to separate properties.

The `AdditionalOutputs` may contain the following:

field                            | purpose
---------------------------------| -------
`OutputProbability`              | _(optional)_ The absolute probability of getting that output, from 0 to 1. Default is `0`. If 0 the remain percent not defined will be equally divided between all outputs with 0 probability. Probability are checked from the first to the last output, so with the total is bigger than 1, the remaining outputs will be ignored.
`OutputIdentifier`               | This is required. Works the same as the OutputIdentifier described in the rule.
`MinutesUntilReady`              | The amount of minutes it takes to produce this output. This will override the default MinutesUntilReady set in the rule. Should be divisible by 10.
`RequiredInputQuality`           | _(optional)_ Required input quality for the output to be possible. Default is no required quality.
`RequiredFuel`                   | _(optional)_ Required fuel for the output to be possible. These fuels will be consumed in additional to the rule defined fuels. The format is a pair of identifier and stack amount, following the same rules of the other fuel property. Default is no required fuel.
`RequiredSeason`                 | _(optional)_ Required season for the output to be possible. Default is no required season.
`RequiredWeather`                | _(optional)_ [Sunny|Rainy|Stormy|Snowy|Windy] Required weather for the output to be possible. Default is no required weather.
`RequiredLocation`               | _(optional)_ Required location for the output to be possible. Default is no required location.
`RequiredOutdoors`               | _(optional)_ [true|false|null] If true, the output can only be produced outdoors. If false, the output can only be produced indoors. If the line is removed or the value is null, it there is no restriction.
`RequiredInputParentIdentifier`  | Required input parent for the output to be possible. Parent is the item that originated the input.(ex. Salmon Roe has Salmon as parent) The identifier is compared with the input parent on this order: Index, Name, Category, Context_Tag.

Example:

```
"AdditionalOutputs": [ 
      {
        "OutputProbability": 0.2, 
        "OutputIdentifier": "Apple", 
        "MinutesUntilReady": 100, 
        "RequiredInputQuality": [ 0, 1 ], 
        "RequiredFuel": { "Coal": 1 }, 
        "RequiredSeason": [ "spring", "summer" ], 
        "RequiredWeather": [ "Sunny", "Windy" ], 
        "RequiredLocation": [ "Cellar", "FarmCave" ], 
        "RequiredOutdoors": true, 
        "RequiredInputParentIdentifier": [ "22", "-80" ] 
      }
    ],
```

#### LookForInputWhenReady
Below is more information on the `LookForInputWhenReady` field. This field is similar to how Automate works however it can look for any set input and exclude certain inputs.

The `LookForInputWhenReady` may contain the following:

field                            | purpose
---------------------------------| -------
`Range`                          | Max range the input will be looked for. A negative number will be considered no limit (will stop at 150 to avoid crash). Default is `-1`.
`InputIdentifier`                | Identifier of the input. Can be the index, name or category.
`Crop`                           | _(optional)_ If should look up for crops. The identifier will be compared with the harvest object. Default is `false`.
`ExcludeForageCrops`             | _(optional)_ If it should ignore Forage crops. Default is `false`.
`GardenPot`                      | _(optional)_ If it should look for crops inside garden pots. Default is `false`.
`FruitTree`                      | _(optional)_ If it should look up for fruit trees. The identifier will be compared with the fruit. Default is `false`.
`BigCraftable`                   | _(optional)_ If it should look up for BigCraftables. The identifier will be compared with the held object. Default is `false`.

Example:

```
"LookForInputWhenReady": {  
      "Range": 5,
      "InputIdentifier": [ "Orange", "-80" ], 
      "Crop": true, 
      "ExcludeForageCrops": true, 
      "GardenPot": true, 
      "FruitTree": true, 
      "BigCraftable": false 
    }
```

### producersConfig
A `producersConfig` is not needed if you are only using machines from another mod. It is only needed when new machines are being added. 

Scenarios:
1) You're using the Dehydrator from Artisan Valley. You do not need a `producersConfig`. 

2) You're using the Dehydrator from Artisan Valley and a new machine you made. You need a `producersConfig` entry for the new machine.

3) You're using the vanilla machines. You do not need a `producersConfig`.

field                                   | purpose
----------------------------------------|--------
`ProducerName`                          | Name of the producer.
`AlternateFrameProducing`               | If the producer should use the alternate frame when producing.
`AlternateFrameWhenReady`               | If the producer should use the alternate frame and ready for harvest.
`DisableBouncingAnimationWhileWorking`  | If the producer bouncing animation while working should be disabled.
`NoInputStartMode`                      | [Placement|DayUpdate] If the machine has a value for this property, it can only have one `ProducerRule` without an `InputIdentifier`. If `Placement`, the machine will start on placement, and will restart every time the produced object is taken out. If `DayUpdate`, the machine will start at the begin of the day. Default is `null`.
`IncrementStatsOnOutput`                | _(optional)_ Pairs of "stats:object" that identify what should be incremented.
`MultipleStatsIncrement`                | _(optional)_ If all stats that match should be incremented. If false, just the first match will be increased. Default is `false`.
`LightSource`                           | _(optional)_ Defines the light source the producer should use when working. Default is `null`.
`WorkingTime`                           | _(optional)_ If a value is defined, the machine will only produce during this period. Production will be on hold out of this period. Default is working any time. Must have a `Begin` and `End`. Format [HHmm]. Default is 0.
`WorkingWeather`                        | _(optional)_ [Sunny|Rainy|Stormy|Snowy|Windy] The weathers in which the machine works. Default is working in any weather.
`WorkingLocation`                       | _(optional)_ The locations in which the machine will work. Can be any map of building. Default is working in any location. 
`WorkingOutdoors`                       | _(optional)_ [true|false|null] If true, the machine will only work outdoor. If false, the machine will only work indoor. If the line is removed or the value is null, it will work on both kind of location.
`WorkingSeason`                         | _(optional)_ The seasons in which the machine will work. Different than other conditions, season clean the machine at the start of the day if in a not working season. The default is working in any season. 
`OverrideMod`                           | _(optional)_ If defined, this config can override the configs for the same producer from the mods in the list. Otherwise the first one to be loaded will be used.
`ProducingAnimation`                    | _(optional)_ Set a configuration for animating the producing state of the machine. Default is `null`. See [Machine Animations](#machine-animations) for more details.
`ReadyAnimation`                        | _(optional)_ Set a configuration for animating the ready state of the machine. Same format of configuration as WorkingAnimation. Default is `null`. See [Machine Animations](#machine-animations) for more details.

#### LightSource

field                   | purpose
------------------------|--------
`TextureIndex`          | _(optional)_ Shape of the light source. All vanilla machines uses "sconceLight". 1=lantern, 2=windowLight, 4=sconceLight, 5=cauldronLight, 6=indoorWindowLight, 7=projectorLight. Default is `4`.
`Radius`                | _(optional)_ The actual size of the light source depends on the texture used. Default is `1.5` (the size of the furnace light source with the `sconceLight` texture)
`OffsetX`               | _(optional)_ The horizontal offset from the center of the producer tile. Default is `0`.
`OffsetY`               | _(optional)_ The vertical offset from the top of the producer tile. Default is `0`.
`ColorType`             | [ObjectColor|ObjectDyeColor|DefinedColor] If `ObjectColor` and the output is a colored object, it will use the color of the output. If `ObjectDyeColor` and the output has a dye color, it will use this color. If `DefinedColor` or it doesn't find the color on the other options, it will create the color based on the Red, Green and Blue values.
`ColorRed`              | The red value of the light color. Default is 255.
`ColorGreen`            | The green value of the light color. Default is 255.
`ColorBlue`             | The blue value of the light color. Default is 255.
`ColorAlpha`            | The transparency value of the light color. 0 is fully transparent, 255 is fully opaque. Default is 255.
`ColorFactor`           | _(optional)_ The factor that will multiply all other colors parameters. Default is `1`.
`AlwaysOn`              | _(optional)_ If true, the light source will be lit even while not producing. Default is `false`. 

Example:
```
"LightSource": {  
      "TextureIndex": 4, 
      "Radius": 1.5,
      "OffsetX": 0, 
      "OffsetY": 0, 
      "ColorType": "DefinedColor",
      "ColorRed": 255, 
      "ColorGreen": 255, 
      "ColorBlue": 255,
      "ColorAlpha": 255, 
      "ColorFactor": 0.75,
      "AlwaysOn": true 
    },
```

### Machine Animations
Below is an excerpt taken from the [Json Assets ReadMe](https://github.com/spacechase0/JsonAssets/blob/master/README.md) regarding machine animations. 

`ReserveExtraIndexCount` is used primarily for big-craftable machines. It may also be useful for a SMPAI mod that utilizes chest animation. Unlike CFR, each frame of the machine will need to be it's own image. Starting with `big-craftble`, `big-craftable-2` `big-craftable-3` and so on. `big-craftable` (no numbers) is considered to be 0 in the index. So for our example of the Alembic, there is the starting frame and then 7 additional frames afterwards for the animation.

Here is a preview of the folder contents
[Imgur](https://i.imgur.com/Du4WNM5.png)

Example:
```
{
    "Name": "Alembic",
    "Description": "Distills flowers, fruits, herbs, and vegetables into essential oils.",
    "Price": 1,
    "ProvidesLight": false,
    "ReserveExtraIndexCount": 7,
    "Recipe":
    {
        "ResultCount": 1,
        "Ingredients": [
        {
            "Object": 334,
            "Count": 5,
        },
        {
            "Object": 766,
            "Count": 50,
        },
        {
            "Object": 709,
            "Count": 10,
        }, ],
        "CanPurchase": false,
    },
}
```

If you want an image to constantly animate you will need to use the [Content Patcher API](#content-patcher-api).

<details>
  <summary> <b>Expand for more information on PFM useage </b> </summary>

When using with PFM in the `producersConfig.json` this information would translate to:

```
{
    "ProducerName": "Alembic", 
    "AlternateFrameProducing": false, 
    "AlternateFrameWhenReady": false, 
    "DisableBouncingAnimationWhileWorking": true, // Disables defualt bouncing animation
    "ProducingAnimation": { 
            "RelativeFrameIndex": [1,2,3,4,5,6], //big-craftable-2 through big-craftable-7
            "FrameInterval": 10 
        },
        "ReadyAnimation": 
        {
          "RelativeFrameIndex": [7], // big-craftable-8
      },
  },
```

This is mentioned because JA & PFM indexs are one off of each other. `big-craftable` is your idle animation. `big-craftable-2` through `big-craftable-7` are your `ProducingAnimation` `RelativeFrameIndex`. Finally `big-craftable-8` is your `ReadyAnimation` `RelativeFrameIndex`. You can have less or more than 8 `big-craftable` just keep in mind to bump each number down one.

</details>

## FAQ
### Custom Name Already in Use
```
WARN Producer Framework Mod] The custom name 'Ground {inputName}' is already in use for the object with the index '2371'. The custom name '{inputName} Modo(a)' will be ignored.
```

This is a harmless error. It means another pack is using the same translation key. Please see which was is the newer of the two and report the issue to the author. The solution is to either rename the translation key or remove the i18n entry (best if the mods are by the same author/rely on each other to work).

## See Also

* [Nexus Page](https://www.nexusmods.com/stardewvalley/mods/4970)
