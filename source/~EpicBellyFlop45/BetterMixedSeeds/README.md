**Better Mixed Seeds** is a [Stardew Valley](http://stardewvalley.net/) mod that enabled [Mixed Seeds](https://stardewvalleywiki.com/Mixed_Seeds) to drop any seed you want.

![](pics/greenhouse.png)

Better Mixed Seeds currently supports 28 other crop mods. These can be found below.

## Integrated Mods
* [Fantasy Crops](https://www.nexusmods.com/stardewvalley/mods/1610)
* [Fresh Meat](https://www.nexusmods.com/stardewvalley/mods/1721)
* [Fruits and Veggies](https://www.nexusmods.com/stardewvalley/mods/1598)
* [Mizus Flowers](https://www.nexusmods.com/stardewvalley/mods/2028)
* [Cannabis Kit](https://www.nexusmods.com/stardewvalley/mods/1741)
* Six Plantable Crops (No longer available to download)
* [Bonster's Crops](https://www.nexusmods.com/stardewvalley/mods/3438)
* [Revenant's Crops](https://www.nexusmods.com/stardewvalley/mods/1663)
* [Farmer to Florist](https://www.nexusmods.com/stardewvalley/mods/2075)
* [Lucky Clover](https://www.nexusmods.com/stardewvalley/mods/3568)
* [Fish's Flowers](https://www.nexusmods.com/stardewvalley/mods/3553)
* [Fish's Flowers Compatibility Version](https://www.nexusmods.com/stardewvalley/mods/5020)
* [Stephan's Lots of Crops](https://www.nexusmods.com/stardewvalley/mods/3171)
* [Eemies Crops](https://www.nexusmods.com/stardewvalley/mods/3523)
* [Tea Time](https://www.nexusmods.com/stardewvalley/mods/2607)
* [Forage to Farm](https://www.nexusmods.com/stardewvalley/mods/2815)
* [Gem and Mineral Crops](https://www.nexusmods.com/stardewvalley/mods/3395)
* [Mouse Ears Cress](https://www.nexusmods.com/stardewvalley/mods/4401)
* [Ancient Crops](https://www.nexusmods.com/stardewvalley/mods/4472)
* [Poke Crops](https://www.nexusmods.com/stardewvalley/mods/2065)
* [Starbound Valley](https://www.nexusmods.com/stardewvalley/mods/2046)
* [IKeychain's Winter Lychee Plant](https://www.nexusmods.com/stardewvalley/mods/4980)
* [Green Pear](https://www.nexusmods.com/stardewvalley/mods/5023)
* [Soda Vine](https://www.nexusmods.com/stardewvalley/mods/4482)
* [Spoopy Valley](https://www.nexusmods.com/stardewvalley/mods/4513)
* [Stardew Bakery](https://www.nexusmods.com/stardewvalley/mods/5094)
* [Succulents](https://www.nexusmods.com/stardewvalley/mods/5310)
* [Tropical Farm](https://www.nexusmods.com/stardewvalley/mods/5585)

## Request Mod Integration
If you have a mod you want to be integrated into this mod: Simply create an issue, in this issue it is important to include a link to the original mod, this could be a: Nexus Mod link, Chuckle fish link etc. It is important the mod is: not abandoned, and maintained (up-to-date) officially and not through unofficial patches OR you can go into the [Stardew Valley Discord](https://www.discordapp.com/invite/stardewvalley) and post in the #modding channel making sure to @EpicBellyFlop45 and we can have a chat about the details etc.

## Install
1. Install the latest version of [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400).
2. Install the latest version of [this mod](https://www.nexusmods.com/stardewvalley/mods/3012).
3. Extract the .zip mod file into your StardewValley/Mods folder and run the game using SMAPI.

## Use
First, open the game using SMAPI like normal, this will generate a config.json file in the Mods/BetterMixedSeeds folder.
Then using the below section configure the mixed seeds to your liking.
Lastly load back into the game and use mixed seeds like normal.

## Configure
The configuration file creates an object for every compatible mod, regardless of if you have it installed. Each mod object has 4 objects, 1 for each season. Each season has a list of crops that contain the properties you can edit. Below is the basic layout for each mod.

    "ModName": {
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
(If a mod doesn't have any crops then the season will simply be null)

The **Name** property **should not be changed**. This will stop the mod working correctly and will show errors in the SMAPI console.
The **Enabled** property only accepts either **true** or **false**, this will determine whether mixed seeds can plant this seed.
The **Chance** property accepts any integer, this is the number of times the seed gets added to the seed list eg: If the **chance** is set to **5** it be 5 times more likely to be planted from mixed seeds compared to a crop with the chance of 1. It is important to not set the chance number too high for many crops as this can affect performance.
Finally, at the top of the configuration file there is a propery named **PercentDropChanceForMixedSeedsWhenNotFiber** This accepts an integer between 0 and 100. If you put in a number not in this range, the code will clamp it to the whichever is closest. (-482 turns to 0 and 24634 turns to 100 etc) 

## Compatibility
Better Mixed Seeds is compatible with Stardew Valley 1.3+ on Windows/Mac/Linus, both single player and multiplayer. To view reported bug visit both the issues on this repo and bug reports on [Nexus](https://www.nexusmods.com/stardewvalley/mods/3012?tab=bugs).
