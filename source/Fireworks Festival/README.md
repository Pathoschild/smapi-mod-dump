**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/elizabethcd/FireworksFestival**

----

# FireworksFestival

A Stardew Valley mod that adds a summer fireworks festival and craftable, functional fireworks. 

Slippery slime to bind
Rotten plants turned to brimstone
The boom of exploding fireworks
A constellation of sparkles
Voyage of the Fireworks Festival


*Made for the Stardew Valley Mod Jam January 2023*


# What Does This Mod Do?
* Adds a new festival, the Firework Festival, which takes place on Summer 20 between 8pm and midnight
* At this festival, you can buy fireworks, fruit, new foods, clothing, and even shop at the Traveling Cart
* You can also buy a fireworks manufacturing license, which comes with instructions on how to make fireworks 
* During the festival, there's a fireworks show
* The fireworks require a variety of ingredients to make them, which you can largely make with the Chemizer, a new machine you learn how to craft at Mining Level 6

# How to Install This Mod
* Install Dynamic Game Assets, Content Patcher, and their dependencies
* Download this mod from Nexus: https://www.nexusmods.com/stardewvalley/mods/15261
* If you are updating the mod, please **delete the old mod entirely** first
* Unzip and place into Mods folder

# Fireworks Crafting

Fireworks require:
* A metal salt (to give it color)
* Black powder (to make it go boom)
* Fiber (to package it all)
* Slime (to stick everything together)

There are 6 kinds of metal salts:
* Strontium salt makes red fireworks
* Limestone (calcium carbonate) makes orange fireworks
* Iron salt makes yellow fireworks
* Barium salt makes green fireworks
* Copper salt makes blue fireworks
* Iridium salt makes purple fireworks
* Magnesium salt makes white fireworks

These are mostly from real-life fireworks chemistry! (With the exception of iridium, which is clearly a much more versatile material in Stardew Valley than it is in real life.) To make these salts, you can place various items in the Chemizer, largely based on the real-life minerals and their chemical compositions. You can also place bone items to get Limestone, and eggs to get rotten eggs (because nothing good happens when you randomly jam eggs into a machine). Rotten plants, rotten eggs, and certain minerals make Brimstone. 

Black powder requires:
* Niter (also known as saltpeter or potassium nitrate, buy from Clint)
* Brimstone (also known as sulfur, made from sulfurous minerals, rotten eggs, or rotten plants)
* Coal
* This is the classic recipe! In the real world, proportions matter, but in Stardew Valley you always get those just right on the first try.

For full crafting details, see the section below.

# Known Bugs
* DGA is not compatible with Lookup Anything. If you use Lookup Anything on the items or the Chemizer, you will get nonsense (Weeds error item). 
* DGA recipes do not show up in the crafting menu if you have Custom Crafting Stations installed. I believe this is something to do with how it tries to remove recipes for its stations. 

# Translations
To translate this mod, there are *exactly* 2 files that must be translated. There is `default.json` in the `i18n` folder in the `[CP] Fireworks Festival` and `[DGA] Fireworks Festival` folders. Please do not translate anything in the `content.json` files or you may cause problems in the mod. 

# Mod Compatibility
* This should be generally compatible with other mods, including recolors
* There is built-in NPC compatibility for: Jasper, Jessie and Juliet, Mister Ginger, Jean and Jorts. There is planned future compatibility for Always Raining in the Valley NPCs. If you have an NPC mod, you can look through the NPCcompat.json for examples of how to add your NPC to the map and add dialogue. The .tmx file for the map has red dots on the Paths layer where NPCs currently stand, and in the future it is planned to add the festival map to the [Custom NPC Tiles spreadsheet](https://stardewmodding.miraheze.org/wiki/Custom_NPC_Tiles).
* Unless another mod adds a festival on Summer 20, you should be good to go (I checked the mods I know of that add festivals, Surfing Festival, Ridgeside, and SVE all seem to not have festivals on this day.)
* This is compatible with multiplayer, but may not be compatible with splitscreen
* This is not compatible with mobile

# Full Crafting Details 

## Fireworks recipe
In order to craft fireworks, you must purchase a Fireworks Manufacturing License from the fireworks vendor at the fireworks festival. Once you do this, you will automatically learn the recipes after sleeping a night. (They will show up in the crafting menu, but you won't get any kind of notification about it.)
* Black powder
* Metal salt   
  - Strontium salt for red   
  - Limestone for orange    
  - Iron salt for yellow    
  - Barium salt for green    
  - Copper salt for blue    
  - Iridium salt for purple    
  - Magnesium salt for white
* Slime (for binder)
* Fiber (for paper)

## Black powder recipe
You automatically learn to craft it after reaching Mining level 6. (It will show up in the crafting menu, but you won't get any kind of notification about it.)
* Coal (for charcoal)
* Niter (saltpeter, from Clint's shop)
* Brimstone (sulfur, from chemizer)

## Chemizer
The Chemizer processes metals and minerals into useful ingredients for fireworks. The recipe for the Chemizer calls for 5 iron bars, 2 gold bars, 1 iridium bar, and 25 slime. You automatically learn to craft it after reaching Mining level 6. (It will show up in the crafting menu, but you won't get any kind of notification about it.)
- Egg to Rotten Egg (500 min)
- Rotten Egg to Brimstone (1000 min)
- Rotten plant to Brimstone (1000 min)
- Orpiment to Brimstone (1000 min)
- Jamborite to Brimstone (1000 min)

Various metals and minerals to metal salts (all in 1500 min):
- Celestine (representing Celestite) to Strontium Salt    
- Ghost Crystal (representing Strontianite) to Strontium Salt    
- Bone items to Limestone   
- Calcite to Limestone   
- Iron Ore and Bars to Iron Salt    
- Bixite (representing Bixbyite) to Iron Salt    
- Pyrite to Iron Salt    
- Hematite to Iron Salt    
- Baryte (representing Barite) to Barium Salt    
- Ocean Stone to Barium Salt    
- Sandstone to Barium Salt    
- Granite to Barium Salt    
- Copper Ore and Bars to Copper Salt   
- Geminite to Copper Salt    
- Malachite to Copper Salt    
- Iridium Ore and Bars to Iridium Salt    
- Star shards to Iridium Salt    
- Dolomite to Magnesium Salt   
- Soapstone to Magnesium Salt    
- Basalt to Magnesium Salt

# Thanks

Big extra thanks to: [atravita](https://www.nexusmods.com/stardewvalley/users/116553368?tab=user+files) for helping me debug DGA and countless of my own silly mistakes, [DaLion](https://www.nexusmods.com/stardewvalley/users/9473360?tab=user+files), [kittycatcasey](https://www.nexusmods.com/stardewvalley/users/34250790?tab=user+files), [Shockah](https://www.nexusmods.com/stardewvalley/users/133612513?tab=user+files), and Misha for helping me test multiplayer, and everyone who helped me out on the Stardew Valley Discord. Thank you to [Matt](https://www.nexusmods.com/stardewvalley/users/1643034?tab=user+files) for running the mod jam! The fireworks fox vendor sprite and portrait are based off the red fox from [SmolHooman's Wild Animals mod](https://www.nexusmods.com/stardewvalley/mods/9063), which I highly recommend. 
