# Paritee's Quick Patch Buildings

Customize the types of simple buildings you can purchase without needing to replace the default options

## Contents

- [Get Started](#get-started)
- [For Modders](#for-modders)

## Get Started

### Install

1. Install the latest version of [SMAPI](https://smapi.io/)
2. Download the [Paritee's Quick Patch Buildings](https://www.nexusmods.com/stardewvalley/mods/0000) (QPB) mod files from Nexus Mods
4. Unzip the mod files into `Stardew Valley/Mods`
5. Run the game using SMAPI


### Add a Building

The following section explains how to add a new building with the [QPB](https://www.nexusmods.com/stardewvalley/mods/0000) mod. In this example we will be using [Paritee's Animal Troughs](https://www.nexusmods.com/stardewvalley/mods/0000) mod, but this can be done with any building type that has been loaded into `Data/Blueprint`.

1. Install the latest version of [QPB](#install)
2. Install the latest version of [Content Patcher](https://www.nexusmods.com/stardewvalley/mods/1915)
3. Unzip the [Paritee's Animal Troughs](https://www.nexusmods.com/stardewvalley/mods/0000) folder into `Stardew Valley/Mods`
4. Run the game using SMAPI


## For Modders

### Skins

It is possible to override the default spritesheets for the QPB buildings so that you can use your favourite packs (ex. add seasonal support).

#### Add a Skin

The following section explains how to add a skin with the [QPB](https://www.nexusmods.com/stardewvalley/mods/0000) mod and Content Patcher. In this example we will be using [Paritee's Seasonal Animal Troughs](https://www.nexusmods.com/stardewvalley/mods/0000) mod.

1. Install the latest version of [QPB](#install)
2. Install the latest version of [Content Patcher](https://www.nexusmods.com/stardewvalley/mods/1915)
3. Unzip the [Paritee's Seasonal Animal Troughs](https://www.nexusmods.com/stardewvalley/mods/0000) folder into `Stardew Valley/Mods`
4. Run the game using SMAPI

#### Creating or Migrating a Content Patcher Skin

 You need to make sure your Content Patcher skin mod adheres to the following. I'll be using [Paritees's Seasonal Animal Troughs](https://www.nexusmods.com/stardewvalley/mods/0000) as the Content Patcher skin mod and [Paritee's Animal Troughs](https://www.nexusmods.com/stardewvalley/mods/0000) as the QPB building in the example.

1. Add your target QPB building's mod `UniqueID` (found in `manifest.json`) as a required dependency in your Content Patcher skin mod's `manifest.json`.

```json
{
   "Name": "Paritee's Seasonal Animal Troughs",
   ..
   "ContentPackFor": {
      "UniqueID": "Pathoschild.ContentPatcher"
   },
   "Dependencies": [
      {
         "UniqueID": "Paritee.AnimalTroughs",
         "IsRequired": true
      }
   ]
}
```

2. In your Content Patcher skin mod's `content.json`, all assets should be loaded with `"Action": "EditImage"` and the `Target` should be the same as in your QPB building's `content.json`. Below is what your Content Patcher skin mod's `content.json` should look like. In this example, the mod has images in the `assets` folder for each season.

```json
{
  "Format": "1.3",
  "Changes": [
    {
      "Action": "EditImage",
      "Target": "Buildings/Trough",
      "FromFile": "assets/trough_{{season}}.png",
    },    
  ]
}
```


### Content Packs

#### manifest.json

Follows standard SMAPI `manifest.json` format, but must include `ContentPackFor` targetted to `Paritee.QuickPatchBuildings`.

```json
{
   ..
   "ContentPackFor": {
      "UniqueID": "Paritee.QuickPatchBuildings"
   }
}
```


#### content.json

```json
{
  "Format": "1.0",
  "Changes": [
    {
      "Type": "<unique building name>",
      "Data": "<Data/Blueprints formatted value string>",
      "Asset": "assets/<filename>.png",
      "Seasonal": false,
    }
  ]
}
```

Field | Type | Description
--- | --- | ---
Type | string | Must be unique against `Content/Data/Blueprints`
Data | string |  Ex. of Well: `390 75/3/3/-1/-1/-1/-1/null/Well/Provides a place for you to refill your watering can./Buildings/none/32/32/-1/null/Farm/1000/false` (see [Blueprint Data Guide](https://stardewvalleywiki.com/Modding:Blueprint_data))
Asset | string | All lowercase
Seasonal | boolean | Recommend setting this to `false` and using Content Patcher to apply the seasonal variations


#### ./assets

Folder to contain the building's assets - all lowercase and typically `.pngs`. If using the seasonal capability of QPB, your filename format must be `<filename>_<season>.png`.
