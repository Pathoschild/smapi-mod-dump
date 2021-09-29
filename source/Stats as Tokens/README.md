**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/dtomlinson-ga/StatsAsTokens**

----


## Stats As Tokens
*by Vertigon*

[NexusMods Page](https://www.nexusmods.com/stardewvalley/mods/9659)

This mod allows Content Patcher pack creators to access all stats tracked by the game through custom CP tokens, allowing for patches to trigger
on various player milestones including number of seeds sown, truffles found, palm trees cut down, trash cans checked and many more! See below for a complete list.

### Usage
Download this mod, place it in your Mods folder, and list it as a dependency in your `manifest.json` in order to access its custom tokens.

### Dependencies
* [SMAPI](https://smapi.io/)  v3.9.5 or higher is a required dependency.
* [Content Patcher](https://www.nexusmods.com/stardewvalley/mods/1915) v1.23.0 or higher is a required dependency.

## Custom Tokens Provided

### **`Vertigon.StatsAsTokens/Stats`**

This token takes exactly two named arguments (both must be provided in order for it to work):
* **`player`**: Must be one of the following:
  * `host`: The player hosting the lobby, or
  * `local`: The player on the local splitscreen or computer, if not the host
* **`stat`**: The stat to track. See below for a complete list.

The arguments are case-insensitive and space-insensitive.

For example:
`{{Vertigon.StatsAsTokens/Stats:player=host|stat=diamondsFound}}` will be parsed as the number of diamonds found by the host player.

Here is a complete list of stats currently usable as arguments:

* `seedsSown`
* `itemsShipped`
* `itemsCooked`
* `itemsCrafted`
* `chickenEggsLayed`
* `duckEggsLayed`
* `cowMilkProduced`
* `goatMilkProduced`
* `rabbitWoolProduced`
* `sheepWoolProduced`
* `cheeseMade`
* `goatCheeseMade`
* `trufflesFound`
* `stoneGathered`
* `rocksCrushed`
* `dirtHoed`
* `giftsGiven`
* `timesUnconscious`
* `averageBedtime`
* `timesFished`
* `fishCaught`
* `bouldersCracked`
* `stumpsChopped`
* `stepsTaken`
* `monstersKilled`
* `diamondsFound`
* `prismaticShardsFound`
* `otherPreciousGemsFound`
* `caveCarrotsFound`
* `copperFound`
* `ironFound`
* `coalFound`
* `coinsFound`
* `goldFound`
* `iridiumFound`
* `barsSmelted`
* `beveragesMade`
* `preservesMade`
* `piecesOfTrashRecycled`
* `mysticStonesCrushed`
* `daysPlayed`
* `weedsEliminated`
* `sticksChopped`
* `notesFound`
* `questsCompleted`
* `starLevelCropsShipped`
* `cropsShipped`
* `itemsForaged`
* `slimesKilled`
* `geodesCracked`
* `goodFriends`
* `totalMoneyGifted`
* `individualMoneyEarned`
* `timesEnchanted`
* `beachFarmSpawns`
* `hardModeMonstersKilled`
* `childrenTurnedToDoves`
* `boatRidesToIsland`
* `trashCansChecked`

### **`Vertigon.StatsAsTokens/MonstersKilled`**

This token takes exactly two named arguments (both must be provided in order for it to work):
* **`player`**: Must be one of the following:
  * `host`: The player hosting the lobby, or
  * `local`: The player on the local splitscreen or computer, if not the host
* **`monster`**: The monster to check kills for. See below for a complete list.

The arguments are case-insensitive and space-insensitive.

For example:
`{{Vertigon.StatsAsTokens/MonstersKilled:player=local|monster=Green Slime}}` will be parsed as the number of slimes (all slimes except Big Slimes and Tiger Slimes are considered Green Slime for this purpose) killed by the local player.

Here is a complete list of monsters currently usable as arguments:

* `Green Slime`
* `Dust Spirit`
* `Bat`
* `Frost Bat`
* `Lava Bat`
* `Iridium Bat`
* `Stone Golem`
* `Wilderness Golem`
* `Grub`
* `Fly`
* `Frost Jelly`
* `Sludge`
* `Shadow Guy`
* `Ghost`
* `Carbon Ghost`
* `Duggy`
* `Rock Crab`
* `Lava Crab`
* `Iridium Crab`
* `Fireball`
* `Squid Kid`
* `Skeleton Warrior`
* `Crow`*
* `Frog`*
* `Cat`*
* `Shadow Brute`
* `Shadow Shaman`
* `Skeleton`
* `Skeleton Mage`
* `Metal Head`
* `Spiker`
* `Bug`
* `Mummy`
* `Big Slime`
* `Serpent`
* `Pepper Rex`
* `Tiger Slime`
* `Lava Lurk`
* `Hot Head`
* `Magma Sprite`
* `Magma Duggy`
* `Magma Sparker`
* `False Magma Cap`
* `Dwarvish Sentry`
* `Putrid Ghost`
* `Shadow Sniper`
* `Spider`
* `Royal Serpent`
* `Blue Squid`  
\* not actually monsters, but stored in Data/Monsters. Probably will never be anything other than 0. 

Theoretically this token supports custom monster types as well, so long as they provide the game with a proper Name variable when instantiated.

### **`Vertigon.StatsAsTokens/FoodEaten`**

This token takes exactly two named arguments (both must be provided in order for it to work):
* **`player`**: Must be one of the following:
  * `host`: The player hosting the lobby, or
  * `local`: The player on the local splitscreen or computer, if not the host
* **`food`**: The edible item to check for amount eaten. You can provide either the food name, or the item ID. You can also provide the keyword `any` to get the total number of food items eaten by the selected player.

The arguments are case-insensitive and space-insensitive.

For example:
`{{Vertigon.StatsAsTokens/FoodEaten:player=local|food=434}}` will be parsed as the number of Stardrops (item ID 434) eaten by the local player.  
`{{Vertigon.StatsAsTokens/FoodEaten:player=host|food=leek}}` will be parsed as the number of Leeks eaten by the host.  
`{{Vertigon.StatsAsTokens/FoodEaten:player=local|food=any}}` will be parsed as the total number of food items the local player has eaten.  

#### Note that this *should* support JA/DGA items. However, without a hard DGA dependency I can't initialize DGA items in the internal list (JA items should be okay). What does this mean?  
Well, if a player hasn't yet eaten a DGA item, if you try to get the number eaten it will be returned as "" instead of 0. This will break the enclosing Query expression.
In practice, this *should* have the same effect as if the conditions were false, but it may have unintended effects. If DGA's API gets expanded I will revisit this issue and try to resolve it in a satisfactory manner.

### **`Vertigon.StatsAsTokens/TreesFelled`**

This token takes exactly two named arguments (both must be provided in order for it to work):
* **`player`**: Must be one of the following:
  * `host`: The player hosting the lobby, or
  * `local`: The player on the local splitscreen or computer, if not the host
* **`type`**: The type of tree to check. You can provide either the tree name, or the internal tree type number. See below for a complete list. You can also provide the keyword `any` to get the total number of trees felled by the selected player.

The arguments are case-insensitive and space-insensitive.

For example:
`{{Vertigon.StatsAsTokens/TreesFelled:player=local|type=1}}` will be parsed as the number of trees of type 1 (oak trees) felled by the local player.  
`{{Vertigon.StatsAsTokens/TreesFelled:player=host|type=maple}}` will be parsed as the number of maple trees (type 2) felled by the host.  
`{{Vertigon.StatsAsTokens/TreesFelled:player=local|type=any}}` will be parsed as the total number of trees felled by the local player.  

Here is a complete list of tree types/names currently usable as arguments:

* `oak` (`1`)
* `maple` (`2`)
* `pine` (`3`)
* `palm` (`6`) *
* `mushroom` (`7`)
* `mahogany` (`8`)
* `palm2` (`9`) *
\* For simplicity's sake, `palm` and `palm2` are condensed into one stored value under the hood. Inputting either as the type will give you the total number of palms felled.

## Utilizing Tokens Effectively
While the tokens provided allow for a wide range of new options, for maximum effect you will need to pair them with several core features provided by Content Patcher.

Firstly, I highly recommend familiarizing yourself with the Content Patcher documentation. The [tokens guide](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-tokens-guide.md) in particular will come in handy as the global tokens provided are extremely powerful, especially in conjunction with those provided by this mod.

As the majority of tokens provided by Stats As Tokens are numerical in nature, [number manipulation](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-tokens-guide.md#number-manipulation) tokens will allow you a greater range of expression, especially once more of the planned features have been implemented.

**[Query Expressions](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-tokens-guide.md#query-expressions)** allow you to evaluate mathematical and logical expressions within a When condition or inside a field or token. Here is an example which checks to see if the player has consumed 5 or more Joja Colas in order to determine whether or not to apply a patch:

    "When": {
	    "Query: {{Vertigon.StatsAsTokens/FoodEaten:player=host|food=joja}} >= 5": true
    }
Note that the `food` argument attempts to match the item name using 'fuzzy string matching', as demonstrated here - it will match `joja` to `Joja Cola` automatically. However, to avoid unforeseen issues (i.e. what happens when somebody adds a `Joja Apple`?), it is best to fully write out the item name.

**Query expressions, while powerful, are also unpredictable** - they are not fully validated ahead of time, and may just fail without warning if improperly formatted. Test your expressions thoroughly with the `patch parse` command.

### Upcoming Features
 * Track more custom stats! Message me on Discord (Vertigon#1851) if you have ideas for custom stats to track
	* Number of items gifted by type - game already tracks total number
 * Track animals owned by players
 * Track spouse anniversaries/days married
 * Track children birthdays/days alive

#### If you have any issues:
Make sure SMAPI and Content Patcher are up-to-date.
You can reach me on the Stardew Valley discord (Vertigon#1851) or on the Nexus mod page.
Please provide a SMAPI log, as well as your manifest.json and content.json, so that I can assist you better.
