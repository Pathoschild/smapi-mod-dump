**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/pomepome/JoysOfEfficiency**

----

# Overview
This is a changelog from 1.0.22

# Changelog

## 1.0.22
- Moved Utilities to Util.cs
- Added Balanced mode
- Fixed Collect Collectibles feature couldn't work in coop and mine
- Changed distance determining algorithm
- Changed AutoShakeFruitedTree to AutoShakeFruitedPlants

## 1.0.23
- Adjusted GiftInfo Window size
- Added FasterRunningSpeed Function

## 1.0.24
- Tweaked Auto Gate Function

## 1.0.25
- AddedSpeedMultiplier will be capped from 1 to 19
- Fixed Bug that continuously throws NullReferenceException in barn, coop, etc.
- Added AutoDepositIngredient and AutoPullMachineResult function.
- Removed collecting crub pot from AutoHarvest. Use AutoPullMachineResult instead.

## 1.0.26
- Adjusted some default settings to be more fair.
- Added AutoPetNearbyPets feature.
- Added CJBCheatsMenu Detection.
- Fixed AutoReelRod issue.

## 1.0.27
- Added ForceMaxCasting option

## 1.0.28
- Changed Machine Detection algorithm
- Added ProtectNectarProducingFlower option to AutoHarvest.
- Removed ForceMaxCasting

## 1.0.29
- Fixed bug that AutoDepositIngredient couldn't work well with furnace
- You must face to/back-to-back with fence to activate AutoGate

## 1.0.30
- Reduced lag when using ProtectNectarProducingFlower.
- Fixed CanMax value did not changed by its upgrade level.
- Added FPS Counter.
- Fixed auto things couldn't work when not holding watering can

## 1.0.31
- Updated languages

## 1.0.32
- Removed FPSCounter and FastToolUpgrade because I added it to debug and don't need them anymore. (also, they're a bit cheaty)
- Removed FasterRunningSpeed because it causes many confliction with other mods or even game itselef.
- Re-adjusted BalancedMode to be more balanced.

## 1.0.33
- Fixed some sliders appear on inappropriate position.
- AutoEat won't execute when fishing or in menu.

## 1.0.34
- Added Harvest Exception

## 1.0.35
- Auto things won't work when falling down shaft or player can't move.
- Updated default.json

## 1.0.36
- Fixed casks absorbs ingredient infinitely when not in cellar.

## 1.0.37
- Added CasksAnywhere support.

## 1.0.38
- Pickup motion won't be played when riding.

## 1.0.39
- Added FishingProbabilitiesInfo feature.

## 1.0.40
- Updated de.json and es.json
- Added Bee house, mushroom box, etc. support.

## 1.0.41
- Added 'CraftingFromChests' feature.

## 1.0.42
- Show the reason why you can't gift when using GiftInfo.
- Added Wells support for auto-refill watering can.

## 1.2.0
- Updated for SDV 1.3(non-beta)
- Removed debugging feature.
- Moved UI settings to UIs Tab.
- Added ShowShippingPrice feature.

## 1.2.1
- added UnifyFlowerColors feature.

## 1.2.2
- added AutoLootTreasures feature.

## 1.2.3
- added compatibility for ChestsAnywhere created by Pathoschild.
- added game pad support in config menu.

## 1.2.4
- added scroll bar to Config menu.
- added CollectLetterAttachmentsAndQuests feature.
- added FilterBackgroundInMenu config to ConfigMenu.

## 1.2.5
- Removed MuchFasterBiting feature and Cheats tab from the menu.
- Added PauseWhenIdle feature.
- Added AutoPickUpTrash feature.

## 1.2.6
- Fixed problem couldn't assigning KeyToggleBlacklist from Config Menu.

## 1.2.7
- Added AutoShearingAndMilking feature.
- Fixed CloseTreasureWhenAllLooted may crush the dialogue in events.

## 1.2.8
- SButton will be used in config instead of Keys and Buttons enum.
- Fixed CollectLetterAttachmentsAndQuests was not appeared in config menu.

## 1.2.9
- Updated for SDV 1.3.32 and SMAPI 2.8
- Changed bee flower protection to be a area protection.

## 1.2.10
- Updated Russian Text.
- Updated for the latest SMAPI (>=2.9) events.

## 1.2.11
- Added support for CasksAnywhere mod.
- Fixed a bug around the config menu.
- Added Cordination Giving System.

## 1.2.12
- Edited FeaturesAndConfigs.md, so you can now see full list of features.
- Fixed Charcole Kiln consumed 1 extra wood when you used Auto Deposit Ingredients option.
- Rearranged folders and namespaces.
- Fixed some configs did not documented on FeaturesAndConfigs.md .
- Use Point type in config.json .
- Added MorePreciseProbabilities and TrialOfExamine option to FishingProbabilitiesInfo.

## 1.2.13
- Fixed indegredients were not consumed from fridge when cooking.
- Added SafeMode.

## 1.2.14
- Removed redundant yellow box on screen.
- Added selling price info on FishingInfo.

## 1.2.15
- Added Flower Color Registration menu.
- Fixed ProtectNectarProducingFlower was not working

## 1.2.16
- Added customized flower color unification.

## 1.3.0
- Update for SDV 1.4 and SMAPI 3.0

## 1.3.1
- Fixed AutoFishing moves the bar too quickly.
- Fixed IdlePause set illegal time after loading or when new day started.
- Implemented FarmCleaner function.
- Added DisableConfigLimitation for advanced users.

## 1.3.2
- Fixed code which may cause DivededByZeroException.
- Relocated MineInfoGui, so it won't be hidden by the health bar.
- Fixed CraftingFromChests was not working.

## 1.3.3
- Fixed AutoHarvest harvesting crops infinitely.

## 1.3.4
- Removed CraftingFromChests. If you want, I recommend using Convinient Chests instead.
- Added support for double gates.

## 1.3.5
- Fixed the problem that AutoGate did not close a fence gate when BalancedMode is enabled.
- Added coordinates option for Show Shipping Price feature.

## 1.3.6
- Now FishInformationHud shows correct quality when you are fishing perfectly.
- Fixed the problem that FishingProbabilitiesBox may throw an exception.