![[icon]](https://kdau.gitlab.io/ScryingOrb/icon.png) Craft a Scrying Orb with a recipe from Welwick and you too can peer into the future. Make an offering to the spirits to scry mining conditions, geode contents, rare events or garbage loot.

This mod is based on the [Stardew Predictor](https://mouseypounds.github.io/stardew-predictor/) web app by MouseyPounds, whose tools are all worth a look. Its companion mod, [Public Access TV](https://www.moddrop.com/stardew-valley/mods/757967-public-access-tv), offers a different route to much of the same information for the current game day.

## ![[Compatibility]](https://kdau.gitlab.io/headers/compatibility.png)

This version of Scrying Orb is compatible with **Stardew Valley 1.4.x**. When SDV 1.5 is released, some of this mod's predictions will start being wrong. I'll put out a new version at that point.

This mod should work on **Linux, Mac, Windows and Android**.

There are no known problems with multiplayer use. Every player should install the mod, even if they are not using the Scrying Orb.

There are no known conflicts with other mods. Scrying Orb fully supports the following:

* [Stardew Valley Expanded](https://www.nexusmods.com/stardewvalley/mods/3753) (different arrangement of garbage cans)

When one of the following mods is installed, the related predictions are disabled because they would not be accurate:

* [Better Garbage Cans](https://www.nexusmods.com/stardewvalley/mods/4171)
* [Better Train Loot](https://www.nexusmods.com/stardewvalley/mods/4234)

If any of your other mods affect the areas this mod covers, the orb may make incorrect predictions. If you would like me to add support for another mod, please open an issue [on GitLab](https://gitlab.com/kdau/predictivemods/-/issues) or in the Bugs tab above.

## ![[Installation]](https://kdau.gitlab.io/headers/installation.png)

1. Install the latest version of [SMAPI](https://smapi.io/).
1. Install the [Json Assets](https://www.nexusmods.com/stardewvalley/mods/1720) mod.
1. Download this mod from the link in the header above.
1. Unzip the mod and place *both* the `ScryingOrb` and `[JA]ScryingOrb` folders inside your `Mods` folder.
1. Run the game using SMAPI.

## ![[Use]](https://kdau.gitlab.io/headers/use.png)

The day after you reach two hearts of friendship with the Wizard, you will receive a letter in the mail from his colleague Welwick the diviner (from the "Fortune Teller" TV channel). If you already have two or more hearts of friendship with the Wizard, the letter will arrive the first day you play with the mod installed.

Welwick's letter will give you the recipe for crafting the Scrying Orb. Craft the orb with 3 Refined Quartz (for the orb) and 10 Stone (for the pedestal). You can place it anywhere that is convenient, indoors or outdoors. A pickaxe or axe is needed to pick it up again.

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

## ![[Configuration]](https://kdau.gitlab.io/headers/configuration.png)

The first time you run the mod, it will generate a `config.json` file in its main folder (`Mods/ScryingOrb`). Three options are available:

* `InstantRecipe`: Set this to `true` to give the Scrying Orb crafting recipe immediately instead of needing two hearts with the Wizard.
* `UnlimitedUse`: Set this to `true` to have the Scrying Orb work without taking offerings permanently.
* `InaccuratePredictions`: Set this to `true` to let the spirits make predictions that are inaccurate due to the presence of conflicting mods. For entertainment purposes only.

## ![[Translation]](https://kdau.gitlab.io/headers/translation.png)

This mod can be translated into any language supported by Stardew Valley. It is currently available in English, French, Russian and Simplified Chinese.

Your contribution would be welcome. Please see the [details on the wiki](https://stardewvalleywiki.com/Modding:Translations) for help. You can send me your work in an issue [on GitLab](https://gitlab.com/kdau/predictivemods/-/issues), in the Bugs tab above or by DM on Discord.

## ![[Acknowledgments]](https://kdau.gitlab.io/headers/acknowledgments.png)

* Like all mods, this one is indebted to ConcernedApe, particularly for the vanilla assets it adapts.
* The prediction logic behind this mod is largely ported from the [Stardew Predictor](https://mouseypounds.github.io/stardew-predictor/) web app by MouseyPounds.
* This mod would not function without [SMAPI](https://smapi.io/) by Pathoschild and [Json Assets](https://www.nexusmods.com/stardewvalley/mods/1720) by spacechase0.
* Coding of this mod relied on [Producer Framework Mod](https://www.nexusmods.com/stardewvalley/mods/4970) by Digus as a key example.
* The #making-mods channel on the [Stardew Valley Discord](https://discordapp.com/invite/StardewValley) offered valuable guidance and feedback.
* The date picker is based on a concept from @blueberry on Discord and the aesthetic of [Goethe's color wheel](https://commons.wikimedia.org/wiki/File:Goethe,_Farbenkreis_zur_Symbolisierung_des_menschlichen_Geistes-_und_Seelenlebens,_1809.jpg), with notes from #creative-discussion.
* The French translation was prepared by Inu'tile.
* The Russian translation was prepared by DanyaKirichenko1.
* The Simplified Chinese translation was prepared by liayyMK.

## ![[See also]](https://kdau.gitlab.io/headers/see-also.png)

* [Release notes](https://gitlab.com/kdau/predictivemods/-/blob/master/ScryingOrb/RELEASE-NOTES.md) from existing versions
* [Issue tracker](https://gitlab.com/kdau/predictivemods/-/issues) for bug fixes and minor enhancements
* [Roadmap](https://gitlab.com/kdau/predictivemods/-/blob/master/ROADMAP.md) of major development plans
* [MIT license](https://gitlab.com/kdau/predictivemods/-/blob/master/LICENSE) (TLDR: do whatever, but credit me)
* [My other mods](https://kdau.gitlab.io)

Mirrors:

* [This mod on Nexus](https://www.nexusmods.com/stardewvalley/mods/5603)
* **This mod on ModDrop**
* [This mod on GitLab](https://gitlab.com/kdau/predictivemods/-/tree/master/ScryingOrb)

Other things you may enjoy:

* ![[icon]](https://kdau.gitlab.io/PublicAccessTV/icon.png) [Public Access TV](https://www.moddrop.com/stardew-valley/mods/757967-public-access-tv) for friendship-based, same-day predictions
* ![[icon]](https://mouseypounds.github.io/stardew-predictor/favicon_p.png) [Stardew Predictor](https://mouseypounds.github.io/stardew-predictor/) web app to see all the predictions at once
