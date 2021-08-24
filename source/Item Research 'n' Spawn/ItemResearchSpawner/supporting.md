**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/TSlex/StardewValley**

----

[(← Back to readme)](README.md)

If you want to support this mod development, you can:

* [Help balancing categories](#balance-categories)
* [Help balancing individual item prices for the "Buy" mode](#balance-individual-item-prices)
* [Suggest improvement on the forum](#suggest-improvement)
* [Endorse this mod and tell somebody about it :)](#endorse-this-mod-and-tell-somebody-about-it)

### [Tip] How to get an item "Unique Key"

Unique key is by far the best way to determine item, because some items does not have unique ID and unique Name   
Unique key is a combination of __\<item name\>:\<item id\>__   
Unique key was tested on every item, and from __2000+__ items __only 2__ was not unique (two Copper Pans :/)

---
1. Select item in hotbar   
2. Use command ```rns_get_key``` to get unique key
3. Get key. Ex.: "Wood:388"

### Balance categories

You can balance categories by changing needed for research count, base item price and change item categories

**Remember that by default mod forces default categories and pricelist. Either modify the files in assets folder or turn that of in config.**

Open mod default categories file located in .../Stardew Valley/Mods/ItemResearchSpawner/assets/config/categories.json   
or dump it using the following command ```rns_dump_categories```

File has the following syntax:

```json5
[
  {
    "Label": "category i18n name (do not change this)",
    "When": { //used to determine which items belong to the category
      "ObjCategory": [ //item game-categories. See https://stardewvalleywiki.com/Modding:Object_data#Categories
        -19,
        ...
      ],
      "ItemId": [ //item IDs. Note: not every item has ID! See https://stardewids.com/
        "Object:286",
        ...
      ],
      "Class": [  //item classes. See Stardew Valley sourse code to understand this :/
        "StardewValley.Tools.Slingshot"
      ],
      "UniqueKey": [ //item Unique Keys. See: [Tip] How to get an item "Unique Key"
        "Qi Fruit:889",
        "Ancient Fruit:454"
      ],
    },
    "Except": { //used to determine which items do not belong to the category
      "ObjCategory": [],
      "ItemId": [],
      "Class": [],
      "UniqueKey": [],
    },
    "ResearchCount": 10, //item count for research completion. Note, "Buy mode" use "1" instead of this
    "BaseCost": 100 //item base cost for "Buy" mode in case item does not have own price
  },
  {}...
]
```

Please pay attention to commas (or validate via any json validator online)

To change research count, edit __\"ResearchCount\": \<number here\>__   
To change base price, edit __\"BaseCost\": \<number here\>__ (number must be non-negative)

To change which item belong to category please consider using [Unique Keys](#tip-how-to-get-an-item-unique-key).
If you experienced enough you can use ObjCategory, ItemId, Class as well. However here is some note: this file is read top-down, so
the first category that privatise the item, will have it. If you want to add item to category below, you __should add it to 
"When" of new category and to "Except" in old category__.

You can upload your file content to ["Categories balancing" forum topic](https://www.nexusmods.com/stardewvalley/mods/8933/?tab=forum&topic_id=10179748)

If you dumped file, load it using the following command ```rns_load_categories``` (Remember to save the day :))

### Balance individual item prices

You can change items prices (for menu only) by commands and file pricelist.json

**Remember that by default mod forces default categories and pricelist. Either modify the files in assets folder or turn that of in config.**

```
rns_set_price [0+] //change price for hotbar active item for "Buy" mode. Price must be non-negative!

rns_reset_price //reset price for hotbar active item for "Buy" mode to initial values
```

This commands will change the item price globally for all farms   
__Please open menu at least once and execute commands in game only!__

In addition, you can change the config file located in
.../Stardew Valley/Mods/ItemResearchSpawner/assets/config/pricelist.json    
or dump it using the following command ```rns_dump_pricelist```

File has the following syntax:

```json5
{
  "Solar Essence:768": 99999, //"Unique Key": price (must be non-negative, can be 0)
  ...
}
```

Please pay attention to commas (or validate via any json validator online)

You can add, modify and remove the items from this file. After saving the file execute the following command to 
apply changes in game. Note the game auto-saves and loads the content of the file, so please execute command before continuing the game

```
research_reload_prices //load prices from price-config.json file
```

You can upload you file content to ["Individual item prices balancing" forum topic](https://www.nexusmods.com/stardewvalley/mods/8933/?tab=forum&topic_id=10179773)

If you dumped file, load it using the following command ```rns_load_pricelist``` (Remember to save the day :))

### Suggest improvement

You can suggest improvement on [forum](https://www.nexusmods.com/stardewvalley/mods/8933?tab=forum). But please consider the following
* I might have no time or mood for your big plans, so please be patient, polite and understanding
* I might not answer right away, but i will try to do this asap
* I might not accept suggestion, if it changes a lot of functionality
* Froggie memes will make me feel so much better :)

### Endorse this mod and tell somebody about it
:)