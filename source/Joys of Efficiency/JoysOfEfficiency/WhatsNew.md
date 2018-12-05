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
