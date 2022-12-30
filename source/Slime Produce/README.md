**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/andraemon/SlimeProduce**

----

# Slime Produce Mod

Slime Produce Mod repository.

Thanks to Pathoschild for their amazing Stardew Valley modding framework, [SMAPI](https://github.com/Pathoschild/SMAPI)!

Enjoy!

## Specifics
This mod adds customizable color-based drops to slime balls, as well as changing the color of the balls themselves to reflect this. Whenever a slime ball spawns in a hutch, it will be assigned a color randomly chosen from one of the slimes in the hutch. When broken, it will drop items based on its color. By default, these items are essentially the same as the color-specific drops for slimes in vanilla.

## Config File Entries
Color-specific slime ball drops are fully customizable (though they will always drop slime and have a chance to drop petrified slime). The fields in the config files are as follows:
- **EnableSpecialWhiteSlimeDrops (default true)** - Because the drops for white slimes (i.e. those with RGB values all greater than 230) are handled specially in the code, the same had to be done for white slime ball drops. If you'd like to specify your own drops for white slime balls, or don't want them to drop special items, set this to false.

- **EnableSpecialPurpleSlimeDrops (default true)** - Same situation as above but for purple slimes (Red > 150, Green < 50, Blue > 180).

- **EnableSpecialTigerSlimeDrops (default true)** - Tiger slimes are a special kind of slime whose color is technically white, so they have to be handled specially. If you want to specify your own tiger slime drops, or don't want them to drop special items, set this to false. The color {R: 255, G: 128, B: 0} must fall within the color range of your custom drop table (see below) for the items in said drop table to drop from slime balls produced by tiger slimes.

- **EnableSpecialColorDrops (default false)** - When enabled, this allows slime balls with colors corresponding to one of the [dye colors](https://stardewcommunitywiki.com/Dyeing#Dye_Strength)﻿ to drop a random item from the list of items which share that dye color. This may or may not be balanced, so it's disabled by default, but I considered it a fun enough feature to keep it.

- **SpecialColorMinDrop (default 1)** - This is an integer which represents the minimum number of colored items which have the chance to drop if EnableSpecialColorDrops is true. As always, please exercise caution with extremely large values. This value is inclusive.

- **SpecialColorMaxDrop (default 1)** - Same as above, but this is the maximum number of items that can drop. This value is inclusive.

- **SpecialColorDropChance (default 0.05)** - The chance a colored item will drop if EnableSpecialColorDrops is true. This should be a value between 0 and 1 inclusive.

## Drop Tables
The final entry in the configuration file is a list of drop tables. Each drop table has two fields: **colorRange** and **itemDrops**. 

The **colorRange** field contains three pairs of integers between 0 and 255, one for each of a color's red, green, and blue components. The first number in each pair is the minimum value a color can take, and the second is the maximum value. These values are inclusive.

The **itemDrops** field is a list of objects, each of which contains four values: **parentSheetIndex**, **minDrop**, **maxDrop**, and **dropChance**. The parentSheetIndex represents the item ID of the item dropped, as enumerated [here](https://stardewcommunitywiki.com/Modding:Object_data#Raw_data). As above, the minDrop and maxDrop represent the minimum and maximum number of this item which can drop (these values are also inclusive). The dropChance is a number between 0 and 1 inclusive which represents the chance the specified item will drop.

When a slime ball is broken, it will check in the config file to see if its color falls within any colorRanges in the list of drop tables. If it does fall within a colorRange, it will then drop additional items as specified in the list of itemDrops. If it falls within multiple colorRanges, it will use the first from the top. Importantly, if the slime ball falls within the definition of white or purple above, or if it was left by a tiger slime, it will not look in the config file for drops. If you want it to, you should set the relevant config option to false, as described above.

## Default Drops
Below are the default drops for each color of slime, in order of priority. If a slime is first-generation, then it was hatched from an egg as opposed to being bred. A slime's special number is a random integer between 0 and 99 
inclusive, which is assigned to it when it hatches/spawns.
- **Tiger Slime Drops**
  - 15-25 Sap
  - 1-2 Jade
  - 4-8 Ginger (65%)
  - 5-10 Cinder Shards (50%, first-generation only)
  - 1-2 Magma Caps (33%, first-generation only)
  - 1-2 Dragon Teeth (33%, first-generation only)

- **White Slime Drops (R,G,B > 230)**
  - 2-4 Refined Quartz (only if R is odd)
  - 2-4 Refined Quartz (only if G is odd)
  - 10-20 Iron Ore (only if both R and G are even)
  - 1-2 Diamonds (only if R,G,B are all even OR R,G,B are all 255)

- **Purple Slime Drops (R > 150, G < 50, B > 180)**
  - 5-10 Iridium Ore (only if special number is divisible by 4 (if first-generation) or is even (otherwise))
  - 1 Iridium Bar (7.2%, only if first-generation and special number satisfies condition above) 

- **Black Slime Drops (R,G,B < 80)**
  - 5-10 Coal
  - 1-2 Neptunite (5%)
  - 1-2 Bixite (5%)

- **Yellow Slime Drops (R > 200, Green > 180, Blue < 50)**
  - 10-20 Gold Ore

- **Red Slime Drops (R > 220, 150 > G > 90, B < 50)**
  - 10-20 Copper Ore

- **Grey Slime Drops (R,G,B > 150)**
  - 20-40 Stone

## Contact
Please contact me with any issues you may have! You can find me on Discord, where I'm exotico#5747.