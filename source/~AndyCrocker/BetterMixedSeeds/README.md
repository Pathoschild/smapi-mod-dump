**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/AndyCrocker/StardewMods**

----

**Better Mixed Seeds** is a [Stardew Valley](http://stardewvalley.net/) mod that enabled [Mixed Seeds](https://stardewvalleywiki.com/Mixed_Seeds) to drop any seed you want.

![](pics/greenhouse.png)

Better Mixed Seeds supports any mod that adds crops using Json Assets

## Install
1. Install the latest version of [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400).
2. Install the latest version of [this mod](https://www.nexusmods.com/stardewvalley/mods/3012).
3. Extract the .zip mod file into your StardewValley/Mods folder and run the game using SMAPI.

## API
**Note: ForceExcludeCrop & ReincludeCrop will throw an InvalidOperationException if you call them when a save isn't loaded**

#### GetExcludedCrops()

Retrieves the crop names that have been forcibly excluded.

#### ForceExcludeCrop(params string[] cropNames)

This overrides the players configuration to forcibly exclude crops from being dropped from mixed seeds.

Parameter | Description
--------- | -----------
cropNames | The names of the crops to foribly exclude

**Note: This should be a last resort for mod authors who have added hard to get, highly profitable crops which can't be implemented alongside BMS without destroying in-game economy**

Due to players have no way of overriding this, it's suggested to add a config option on your mod to determine whether this api should get called so players can let Better Mixed Seeds drop the crops if they really want.

#### ReincludeCrop(params string[] cropNames)

This reincludes crops that have been forcible removed using the above **ForceExcludeCrop**

Parameter | Description
--------- | -----------
cropNames | The names of the crops to reinclude

## Use
First, open the game using SMAPI like normal, this will generate a config.json file in the Mods/BetterMixedSeeds folder.
Then using the below section configure the mixed seeds to your liking.
Lastly load back into the game and use mixed seeds like normal.

## Configure
The configuration file creates an object for every mod you have installed that adds crops using Json Assets. Each mod object has four objects, one for each season. Each season has a list of crops that contain the properties you can edit. Below is the basic layout for each mod.

    "ModUniqueId": {
        "Spring": {
            "Crops": [
                {
                    "Name": "CropName",
                    "Enabled": true,
                    "Chance": 1
                }
            ]
        },
        "Summer": {
            "Crops": [
                {
                    "Name": "CropName",
                    "Enabled": true,
                    "Chance": 1
                }
            ]   
        },
        "Fall": {
            "Crops": [
                {
                    "Name": "CropName",
                    "Enabled": true,
                    "Chance": 1
                }
            ]
        },
        "Winter": {
            "Crops": [
                {
                    "Name": "CropName",
                    "Enabled": true,
                    "Chance": 1
                }
            ]
        }
    }

The **Name** property **should not be changed**. This will stop the mod working correctly and will show errors in the SMAPI console.
The **Enabled** property only accepts either **true** or **false**, this will determine whether mixed seeds can plant this seed.
The **Chance** property accepts any decimal number, this is the chance of the seed has of getting planted.

Finally, at the top of the configuration file there are the properies:
**PercentDropChanceForMixedSeedsWhenNotFiber**: This accepts an integer between 0 and 100 that is the chance that mixed seeds will drop from weeds when fiber isn't dropped.
**UseSeedYearRequirements**: This accepts either **true** or **false**, it will determine whether seeds can be planted if their year requirement is met.
**EnableTrellisCrops**: This accepts either **true** or **false**, it will determine whether seeds can be planted if they are trellis crops. NOTE: if this is **false** then no trellis crop will be planted, even if the **Enabled** property for that crop is **true**.

## Compatibility
Better Mixed Seeds is compatible with Stardew Valley 1.3+ on Windows/Mac/Linus, both single player and multiplayer. To view reported bug visit both the issues on this repo and bug reports on [Nexus](https://www.nexusmods.com/stardewvalley/mods/3012?tab=bugs).
