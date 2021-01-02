**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/pomepome/JoysOfEfficiency**

----

# Overview

This is a list of features and configs available in [Joys of Efficiency (JoE)](https://www.nexusmods.com/stardewvalley/mods/2162).

Config description will be like:


	Name("Category Tab in config menu", Type, default value, 
			[minimum or maximum value if its type is int or float]) - "discription of the config".


"Category Tab in config menu" is one of following: "Automation", "UIs", "Cheats", "Misc", "Controls", and "Not on menu".

# Features

## Safe Mode

If Safe Mode is enabled, JoE won't patch the game itself, so it's much safer.

But, some features requiring patching mechanism won't work perfectly.

- SafeMode("Not on menu", bool, default:false) - whether this utility is enabled.


## Config Menu
You can open mod configuration menu when specified key(Default:R).

You can't disable this feature because it's a core feature.

**[CONFIG]**


- KeyShowMenu("Controls", SButton, default:"R") - The key to open config menu.


- FilterBackgroundInMenu("UIs", bool, default:true) - When config menu is opened, outside of it will be darker like inventory menu.

- ShowMousePositionWhenAssigningLocation("UIs", bool, default:true) - When assigning coordinates of window, shows mouse cursor's position.

## Balanced Mode
﻿Did you thought following utilities are a bit cheaty?

﻿This utility lets them not to be executed so often(almost 1 executing per seconds), and
﻿automation radius will be 1 tile.

﻿
﻿This utility affects to ***AutoWaterNearbyCrops, AutoPetNearbyAnimals, AutoHarvest, AutoCollectCollectibles, AutoShakeFruitedPlants,
AutoDigArtifactSpot, AutoDepositIngredient, and AutoPullMachineResult.***

﻿
**[CONFIG]**

- BalancedMode("Automation" bool, default:true) - whether this utility is enabled.

## Mine Info GUI
﻿With this utility, you can check how many stones left, and monster kills tally counter, and whether ladder has spawned in mines.
﻿You can see those when your mouse hovered over the icons.﻿

**[CONFIG]**

- ﻿MineInfoGUI("UIs", bool, default:true) - whether this utility is enabled.

## Auto Water Nearby Crops
﻿With this utility, the crops planted on the soil will be watered automatically.  
﻿To use this, you must have at least one of Watering Can and it to have enough water within, and this costs stamina of the farmer each crops.

**[CONFIG]**

- AutoWaterNearbyCrops("Automation", bool, default:true) - whether this utility is enabled.  ﻿


- AutoWaterRadius("Automation", int, default:1) - How far tiles can be  affected by this utility.  


- FindCanFromInventory("Automation", bool, default:true) - Find Can from entire inventory or just held by player.

## Auto Refill Watering Can
You can refill your watering can automatically from nearby water source.

**[CONFIG]**

- AutoRefillWateringCan("Automation", bool, default:true) - whether this utility is enabled.  ﻿

## Gift Information Tooltip
![](https://i.imgur.com/NOYidaU.gif)

﻿﻿﻿
﻿﻿With this utility, you can check how much do villagers like, dislike the gift before giving it to them.

﻿
**[CONFIG]**

- GiftInformation("UIs", bool, default:true) - whether this utility is enabled.

## Auto Pet Nearby Animals
﻿﻿With this utility, ﻿you don't have to click on animals to pet, just get close to them.

**[CONFIG]**

- AutoPetNearbyAnimals("Automation", bool, default:false) - whether this utility is enabled.
﻿

- AutoPetRadius("Automation", int, default:1) - How far tiles can be affected by this utility.

## Auto Animal Door
﻿With this utility, animal doors will open in morning if it is sunny and not winter, and close at the time day changed without click it manually.﻿

**[CONFIG]**



- AutoAnimalDoor("Automation", bool, default:true) - whether this utility is enabled.

## AFK Fishing
Are you tired to deal with fishing? When this utility is enabled, your computer will catch fish instead of you.
﻿This was requested by @GastlyMister. Many thanks!

**[CONFIG]**

- AutoFishing("Automation", bool, default:false) - whether this plays fishing minigame.


- ﻿CPUThresholdFishing("Automation", float, default:0.2 min:0.0 max:0.5) - determines how often cpu reel up the rod.


- ﻿ThrowPower("Automation", float, default:1.0 min:0.0 max:1.0) - How strong a bobber will be thrown.


- ThresholdStaminaPercentage("Automation", int, default:20 min:10 max:60) - If farmer's stamina percentage is lower than this value, AFK mode will be stopped.


- AutoReelRod("Automation", bool, default:true) - whether it automatically reels up the rod when fish nibbled.﻿


- ToggleAFKFishing("Controls", SButton, default: 'End') - The button to activate/deactivate AFK fishing mode.


## Fishing Information GUI

![](https://i.imgur.com/bLUdo5L.png)

This feature shows the information about the fish when playing fishing minigame.

﻿
**[CONFIG]**

- FishingInfo("UIs", bool, default:true) - whether this utility enabled.

## Auto Gate﻿﻿﻿

![Auto Gate](https://i.imgur.com/ZUiI9Zr.gif)


﻿Are you tired of clicking fence gates? Then try this.
﻿This feature let gates open when farmer is close to them, and otherwise, close them automatically.
It should work with both single-player and coop game modes﻿.
﻿This was requested by @plaah007. Thanks alot!

**[CONFIG]**

- AutoGate("Automation", bool, default:true) - whether this utility enabled.

## Auto Eat
﻿This utility let the farmer to eat something automatically when his/her health or stamina is low.
﻿These threshold ratio can be changed.
﻿﻿This was requested by @GastlyMister. thanks!

**[CONFIG]**



- AutoEat("Automation", bool, default:false) - whether this utility enabled.


- StaminaToEatRatio("Automation", float, default:0.3 min:0.3 max:0.8) - the threshold ratio of stamina to eat something


- HealthToEatRatio("Automation", float, default:0.3 min:0.3 max:0.8) -  the threshold ratio of health to eat something

## Auto Harvest
﻿﻿This utility let the farmer to harvest crops (and spring onions) automatically  when he/she gets closed to it.

**[CONFIG]**

﻿AutoHarvest("Automation", bool, default:false) - whether this utility enabled.
﻿ProtectNectarProducingFlower("Automation", bool, default:true) - this option protects flowers producing nectar not to be Auto harvested.
AutoHarvestRadius("Automation", int, default:1) - How far tiles can be affected by this utility.
﻿HarvestException("Automation", List<int>) - Crop id list not to be auto harvested.﻿
﻿KeyToggleBlackList("Controls", SButton, default:"F2") - Add/Remove crop under cursor to/from blacklist.

## Auto Destroy Dead Crops
This utility destorys dead crops automatically when he/she gets closed to it.

**[CONFIG]**

AutoDestroyDeadCrops("Automation", bool, default:true) - whether this utility enabled.

## Auto Collect Collectibles
﻿﻿﻿This utility let the farmer to collect collectibles (crystals, forages, animal products, and so on) automatically 
﻿when he/she gets closed to it.

**[CONFIG]**

AutoCollectCollectibles("Automation", bool, default:false) - whether this utility enabled.
AutoCollectRadius("Automation", int, default:1) - How far tiles can be affected by this utility.

## Auto Shake Fruited Plants
﻿This utility shakes fruited tree(pines, acorns, apples, cherries, and so on) and berry bushes
﻿automatically when the farmer gets closed to it.

**[CONFIG]**

AutoShakeFruitedPlamts("Automation", bool, default:true) - whether this utility enabled.
AutoShakeRadius("Automation", int, default:1) - How far tiles can be affected by this utility.

## Auto Dig Artifact Spot
﻿﻿This utility digs artifact spots nearby the farmer automatically.

**[CONFIG]**

AutoDigArtifactSpot("Automation", bool, default:false) - whether this utility enabled.
AutoDigRadius("Automation", int, default:1) - How far tiles can be affected by this utility.
﻿FindHoeFromInventory("Automation", bool, default:true) - Find hoe from entire inventory or just held by player.

## Auto Deposit Ingredient
﻿﻿﻿This utility will try to deposit ingredient you held to nearby machines automatically.

**[CONFIG]**

AutoDepositIngredient("Automation", bool, default:false) - whether this utility enabled.
MachineRadius("Automation", int, default:1) - How far tiles can be affected by this utility.

## Auto Pull Machine Result
﻿This utility will try to pull results from nearby machines and give it to the farmer automatically.

**[CONFIG]**

AutoPullMachineResult("Automation", bool, default:true) - whether this utility enabled.
MachineRadius("Automation", int, default:1) - How far tiles can be affected by this utility.
﻿
## Auto Pet Nearby Pets
﻿﻿Oh, seriously you want to pet pets automatically?
﻿All right, this utility pets nearby pets automatically.

**[CONFIG]**

- AutoPetNearbyPets("Automation", bool, default:false) - whether this utility is enabled.
- AutoPetRadius("Automation", int, default:1 min:1 max:3) - How far tiles can be affected by this utility.

## Fishing Probabilities Info
﻿﻿This utility let you know what fish could be caught (and estimated probability of catching) when you are fishing.

**[CONFIG]**

- FishingProbabilitiesInfo("UIs", bool, default:false) - whether this utility is enabled.
﻿

- ProbBoxCoordinates("UIs", Point, default:[100,400]) - Top-left coordinates of the window.


- MorePreciseProbabilities("UIs", bool, default:true) - Displays more plactical and precise probabiilities.


- TrialOfExamine("UIs", int, default:10 min:1 max:50) - Trial number of computing probabilities.


## Show Shipping Price
This utility shows estimated total shipping price when you opened shipping box.

**[CONFIG]**

- EstimateShippingPrice("UIs", bool, default:true) - whether this utility is enabled.

- PriceBoxCoordinates("UIs", Point, default:[100,100]) - Top-left coordinates of the window.

## Unify Flower Colors
This utility unifies flower colors to reduce occupied spaces according to its species.
In config file, you can change the color using "R, G, B, A" format.

**[CONFIG]**

- UnifyFlowerColors("Misc", bool, default:false) - whether this utility is enabled.


- JazzColor("Misc", Color, default:{0, 255, 255, 255}) - The color of Blue Jazz.


- TulipColor("Misc", Color, default:{255, 0, 0, 255}) - The color of Turip.


- PoppyColor("Misc", Color, default:{255, 69, 0, 255}) - The color of Poppy.


- SummerSpangleColor("Misc", Color, default:{255, 215, 0, 255}) - The color of Summer Spangle.


- FairyRoseColor("Misc", Color, default:{216, 191, 216, 255}) - The color of Fairy Rose.


- ButtonToggleFlowerColorUnification("Controls", SButton, default: 'L') - The button to register/unregister flowers to unify their colors.


- CustomizedFlowerColors("Misc", Dictionary<int, Color>) - Customized flower colors for unification.

## Auto Loot Treasures


This utility loots all acceptable items in treasure box, not chests player placed or shipping bin. (e.g. fishing treasures, mine treasures)


**[CONFIG]**


- AutoLootTreasures("Automation", bool, default:true) - whether this utility is enabled.


- CloseTreasureWhenAllLooted("Automation", bool, default:false) - Closes the treasure chest menu when all items taken.


## Collect Letter Attachments And Quests


This feature collects attached items or accepts quests when you opened mail contains those.


**[CONFIG]**


- CollectLetterAttachmentsAndQuests("Automation", bool, default:false) - whether this utility is enabled.


## Pause When Idle


This feature pauses game when you are not in work.


It helps you not to waste in-game time when idling.


**[CONFIG]**


- PauseWhenIdle("Misc", bool, default:false) - whether this utility is enabled.


- IdleTimeout("Misc", int, default:180 min:1 max:300) - Timeout needed to pause the game in seconds.


- PauseNotificationX("Not on menu", int, default:100) - X position of paused notification.


- PauseNotificationY("Not on menu", int, default:700) - Y position of paused notification.


## Auto Pick Up Trash


This feature searches trash can and pick up trash without being detected.


- AutoPickUpTrash("Automation", bool, default:false) - whether this utility is enabled.


- ScavengingRadius("Automation", int, default:2 min:1 max:3) - How far tiles can be affected by this utility.


## Auto Shearing and Milking


This feature let you shear/milk nearby mature sheep/cows automatically when you held shears/bucket.


- AutoShearingAndMilking("Automation", bool, default:true) - whether this utility is enabled.


- AnimalHarvestRadius("Automation", int, default:1 min:1 max:3) - How far tiles can be affected by this utility.


## Farm Cleaner


This feature will clean up small rocks, twigs, and weeds in farm.


Please note that it can consume the farmer's stamina relatively easily.


You have to select appropriate tool in hotbar to use.


**[CONFIG]**

- RadiusFarmCleanup("Automation", int, default:1 min:1 max:3) - How far tiles can be affected by this utility.
 

- CutWeeds("Automation", bool, default:false) - Cuts weeds nearby (requires Scythe).


- BreakRocks("Automation", bool, default:false) - Breaks small rocks nearby (requires Pickaxe).


- ChopTwigs("Automation", bool, default:false) - Chops twigs nearby (requires Axe).