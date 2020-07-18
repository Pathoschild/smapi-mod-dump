

# Seeds Machines

_to my beloved wife._ :two_hearts:
_I showed you Stardew Valley - now I am writing mods for you._
_As you can see, my love is limitless!_ :smile:

----

**Seed Machines** is a [Stardew Valley](https://stardewvalley.net/) mod that adding a few craftable 
machines, that give you ability to get some seeds.

- **Seed Machine** - machine, that allows you to buy all sort of the seeds in the game. You can set your own base price for seeds that not available in the stores. Also you can set your own multiplier for all the seed's prices.
- **Seed Bandit** - this is slots, but instead of Casino money you give him some money and he gives you random seeds.

## Contents
* [Install](#install)
* [Use](#use)
* [Configure](#configure)
* [Beta Test](#beta-test)
* [Translations](#translations)
* [Assets](#assets)
* [TODO](#todo)
* [Release Notes](#release-notes)

## Install
1. [Install the latest version of SMAPI](https://smapi.io/).
2. [Install this mod from Nexus mods](https://www.nexusmods.com/stardewvalley/mods/6265/).
3. Run the game using SMAPI.

## Use
When you installed the mod and start new/existing game - in your crafting tab you will see new items for crafting - these items will be Seed Machines!)
So, you can craft them, then place anywhere and check how they working:
- **Seed Machine** providing ability to buy any seeds in the game. You can change [settings.json](#configure) file for adjust the settings of the Seed Machine.
- **Seed Bandit** providing something like casino-randomizer of the seeds. You give some money (100 by default) to it - it gives you a random seeds in the game directly in the inventory.

## Configure
After first running of the game in the `%Stardew Valley Folder%/Mods/SeedMachines` directory you will get new file `settings.json`. In this file you can change the settings. Here is the description of the each parameter:
| Parameter | Type | Default | Description |
|--|--|--|--|
| seedMachinePrice | int | 10000 | Price for Seed Machine (If you want to sell or throw it) |
| seedMachineIngredients | String | 787 2 337 5 335 20 | Ingredients for crafting the Seed Machine |
| seedMachinePriceForNonSalableSeeds | int | 300 | Base price for seeds with default price = 1 |
| seedMachinePriceMultiplier | double | 2.0 | Price multiplier for the seeds in Seed Machine |
| seedBanditPrice | int | 10000 | Price for Seed Bandit (If you want to sell or throw it) |
| seedBanditIngredients | String | 787 2 337 5 335 20 | Ingredients for crafting the Seed Bandit |
| seedBanditOneGamePrice | int | 100 | Price for 1 game in Seed Bandit |
| themeName | String | Default | Theme name (you can add your own asset e.g. `assets/SeedMachinesMyTheme.png`. Then this parameter should be "MyTheme". |

## Beta Test
You can help this project with your own game experience with this mod! Just check how it works, try some tricky methods (if you know them) and try to broke it! If you get some crashes or not normal behaviour - please, let me know about it! Create issue on Nexusmods or Github! Thanks in advance!

## Translations
My native language is russian. As you can see, my english is so-so. If you want to help me with some translation fixes or add your language in the mod - feel free to ping me in Discord(!MrVeress#7408) or in Github(https://github.com/mrveress). Always glad to meet someone new in my life!)

## Assets
My pixelart is great (no) as you see. If you want to add your theme in assets not only for yourself - ping me in Discord or in Github. If your work will be greather than mine (any other will be better hehe) - I will make your work as default theme for future releases. But make sure that you understand, that in future releases I'll add new machines and ping you for adding new images into your asset (or I will do this by my hands, and you will be not happy with this lol).

## TODO
My big plans:
- New types of machines (Seeds extractor from the trash or other crazy ideas. Btw, you can help me with ideas :smile:)
- Per-save configs (Separated settings for each save file)
- Multiplayer sync of the settings (settings can set/change only host of the server)
- UI for changing parameters of the mod (with hotkeys)
- UI for "Seed Bandit" game
- Refactoring, checking compatibility with other mods

## Release Notes

 - **0.0.3-beta** - **current version** - Quick changes for parameterless constructors fix (v1)
 - **0.0.2-beta** - Fixed issue with loading of the assets on Android devices. Added compatilibty-fix for JsonAssets mod from spacechase0 (if it installed)
 - **0.0.1-beta** - Added base functionality for Seed Machine and Seed Bandit.
