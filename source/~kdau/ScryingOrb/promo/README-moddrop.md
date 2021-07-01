**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://gitlab.com/kdau/predictivemods**

----

![[icon]](https://www.kdau.com/ScryingOrb/icon.png) Craft a Scrying Orb with a recipe from Welwick and you too can peer into the future. Make an offering to the spirits to scry mining conditions, geode contents, rare events, garbage loot and more.

This mod is based on the [Stardew Predictor](https://mouseypounds.github.io/stardew-predictor/) web app by MouseyPounds, whose tools are all worth a look. Its companion mod, [Public Access TV](https://www.moddrop.com/stardew-valley/mods/757967-public-access-tv), offers a different route to much of the same information for the current game day.

## ![[Compatibility]](https://www.kdau.com/headers/compatibility.png)

**Game:** Stardew Valley 1.5.3+ (predictions may be wrong for any future 1.6)

**Platform:** Linux, macOS or Windows (Android: use 1.4 version)

**Multiplayer:** works; every player must install

**Other mods:** There are no known outright conflicts. These mods are handled specially:

* [Better Garbage Cans](https://www.nexusmods.com/stardewvalley/mods/4171): This mod's garbage predictions are hidden.
* [Stardew Valley Expanded](https://www.nexusmods.com/stardewvalley/mods/3753): This mod correctly reflects the different arrangement of garbage cans.
* [Witchy Crystal Farm 2.0](https://www.nexusmods.com/stardewvalley/mods/4330): This mod makes the sculpture on the farm a working Scrying Orb.

If any of your other mods affect the areas this mod covers, the orb may make incorrect predictions. If you would like me to add support for another mod, please open an issue [on GitLab](https://gitlab.com/kdau/predictivemods/-/issues) or in the Bugs tab above.

## ![[Installation]](https://www.kdau.com/headers/installation.png)

1. Install [SMAPI](https://smapi.io/)
1. Install [Json Assets](https://www.nexusmods.com/stardewvalley/mods/1720)
1. Install [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) (optional, for easier configuration)
1. Download this mod from the link in the header above
1. Unzip the mod and place the `ScryingOrb` folder inside your `Mods` folder
1. Run the game using SMAPI

## ![[Use]](https://www.kdau.com/headers/use.png)

The day after you reach two hearts of friendship with the Wizard, you will receive a letter in the mail from his colleague Welwick the diviner (from the "Fortune Teller" TV channel). If you already have two or more hearts of friendship with the Wizard, the letter will arrive the first day you play with the mod installed.

Welwick's letter will give you the recipe for crafting the Scrying Orb. Craft the orb with 3 Refined Quartz (for the orb) and 10 Stone (for the pedestal). You can place it anywhere that is convenient, indoors or outdoors. A pickaxe or axe is needed to pick it up again.

If you live on a Witchy Crystal Farm, you have a Scrying Orb preinstalled for your convenience. It is just south of the Shipping Bin across the creek.

To commune with the spirits, offer the orb an item that is connected with the subject you want to know about. Invalid offerings will be rejected without being consumed. A few very valuable offerings will give you an (in-game) week of unlimited scrying without further offerings.

You can figure out the appropriate items as you go, like giving gifts. If you're in a hurry, check the topics below to see what is accepted.

### Mines and caverns

Learn what floors of the mines and caverns will have monster infestations or valuable caches on any day in the next year. *Must have entered the mines.*

#### Offerings

* 5 Copper Ore
* 3 Iron Ore
* Gold Ore
* Iridium Ore
* 2 Coal

### Geode treasures

Learn what will be found inside the next three geodes of any type, or the next 10 geodes of a particular type. *Must have cracked at least one geode.*

#### Offerings

* 3 Limestone
* 2 Mudstone
* any other [Mineral item found in geodes](https://stardewvalleywiki.com/Minerals#Geode_Minerals)

### Enchanted implements

**(Spoiler for late-game 1.4 content)** Learn what next few enchantments will be cast upon your weapons and tools. *Must have reached the Caldera.*

Offer the spirits the weapon or tool whose enchantments you wish to use; the implement will be returned to you. You must also be holding at least 5 Cinder Shard, which will be consumed as the offering.

### Nocturnal occurrences

Learn what strange events may occur over upcoming nights, or when a particular strange event may next occur.

#### Offerings

* 3 Bat Wing
* Void Egg
* Void Essence
* Void Mayonnaise
* Void Salmon

### Garbage finds

Learn what items will be found in garbage cans on any day in the next year. *Must have looked in at least one can.*

#### Offerings

* 3 Broken CD
* 3 Broken Glasses
* 3 Driftwood
* 3 Joja Cola
* 3 Rotten Plant
* 3 Soggy Newspaper
* 3 Trash
* *or any combination of the above totaling 3*

### Week of unlimited use

(For farmhands in multiplayer, this will work for the current day only.)

#### Offerings

* Golden Pumpkin
* Magic Rock Candy
* Pearl
* Prismatic Shard
* Treasure Chest

### Other

There is a particular item which will get a special reaction from the spirits. You probably know which one I mean.

## ![[Configuration]](https://www.kdau.com/headers/configuration.png)

If you have installed Generic Mod Config Menu, you can access this mod's configuration by clicking the cogwheel button at the lower left corner of the Stardew Valley title screen and then choosing "Scrying Orb".

Otherwise, you can edit this mod's `config.json` file. It will be created in the mod's main folder (`Mods/ScryingOrb`) the first time you run the game with the mod installed. These options are available:

* `InaccuratePredictions`: Set this to `true` to let the spirits make predictions that are inaccurate due to game version mismatch and/or conflicting mods. For entertainment purposes only.
* `InstantRecipe`: Set this to `true` to give the Scrying Orb crafting recipe immediately instead of needing two hearts with the Wizard.
* `UnlimitedUse`: Set this to `true` to have the Scrying Orb work without taking offerings permanently.
* `ActivateKey`: Set this to any valid keybinding that will then activate the Scrying Orb unlimited use menu without any orb or offering. [See the list of keybindings here.](https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings#Available_bindings)

## ![[Translation]](https://www.kdau.com/headers/translation.png)

This mod can be translated into any language supported by Stardew Valley. It is currently available in English, French, Portuguese, Russian and Simplified Chinese.

Your contribution would be welcome. Please see the [details on the wiki](https://stardewvalleywiki.com/Modding:Translations) for help. You can send me your work in [a GitLab issue](https://gitlab.com/kdau/predictivemods/-/issues) or the Comments tab above.

## ![[Acknowledgments]](https://www.kdau.com/headers/acknowledgments.png)

* Like all mods, this one is indebted to ConcernedApe, Pathoschild and the various framework modders.
* The prediction logic behind this mod is largely ported from the [Stardew Predictor](https://mouseypounds.github.io/stardew-predictor/) web app by MouseyPounds.
* Coding of this mod relied on [Producer Framework Mod](https://www.nexusmods.com/stardewvalley/mods/4970) by Digus as a key example.
* The #making-mods channel on the [Stardew Valley Discord](https://discordapp.com/invite/StardewValley) offered valuable guidance and feedback.
* The date picker is based on a concept from [blueberry](https://www.nexusmods.com/stardewvalley/users/71169863?tab=user+files) and the aesthetic of [Goethe's color wheel](https://commons.wikimedia.org/wiki/File:Goethe,_Farbenkreis_zur_Symbolisierung_des_menschlichen_Geistes-_und_Seelenlebens,_1809.jpg), with notes from the #creative-discussion channel on the Discord.
* The French translation was prepared by Inu'tile.
* The Portuguese translation was prepared by Ertila007.
* The Russian translation was prepared by DanyaKirichenko1.
* The Simplified Chinese translation was prepared by liayyMK.

## ![[See also]](https://www.kdau.com/headers/see-also.png)

* [Release notes](https://gitlab.com/kdau/predictivemods/-/blob/master/ScryingOrb/RELEASE-NOTES.md)
* [Source code](https://gitlab.com/kdau/predictivemods/-/tree/master/ScryingOrb)
* [Report bugs](https://gitlab.com/kdau/predictivemods/-/issues)
* [My other Stardew stuff](https://www.kdau.com/stardew)
* Mirrors:
	* [Nexus](https://www.nexusmods.com/stardewvalley/mods/5603),
	* **ModDrop**,
	* [forums](https://forums.stardewvalley.net/resources/scrying-orb.54/)

Other things you may enjoy:

* ![[icon]](https://www.kdau.com/PublicAccessTV/icon.png) [Public Access TV](https://www.moddrop.com/stardew-valley/mods/757967-public-access-tv) for friendship-based, same-day predictions
* ![[icon]](https://mouseypounds.github.io/stardew-predictor/favicon_p.png) [Stardew Predictor](https://mouseypounds.github.io/stardew-predictor/) web app to see all the predictions at once
