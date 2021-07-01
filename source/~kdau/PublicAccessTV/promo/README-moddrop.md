**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://gitlab.com/kdau/predictivemods**

----

![[icon]](https://www.kdau.com/PublicAccessTV/icon.png) Your friends in the valley take to the airwaves to keep you apprised of today's mining conditions, garbage loot, train schedules, rare events and more.

This mod is largely based on the [Stardew Predictor](https://mouseypounds.github.io/stardew-predictor/) web app by MouseyPounds, whose tools are all worth a look. Its companion mod, [Scrying Orb](https://www.moddrop.com/stardew-valley/mods/756553-scrying-orb), offers a different route to some of the same information and more, but looks well beyond the current game day.

## ![[Compatibility]](https://www.kdau.com/headers/compatibility.png)

**Game:** Stardew Valley 1.5.x (predictions may be wrong for any future 1.6)

**Platform:** Linux, macOS or Windows (Android: use 1.4 version)

**Multiplayer:** works; only players wanting the new channels need to install

**Other mods:** There are no known outright conflicts. These mods are handled specially:

* [Better Garbage Cans](https://www.nexusmods.com/stardewvalley/mods/4171): This mod's "In the Cans" channel is hidden.
* [Better Train Loot](https://www.nexusmods.com/stardewvalley/mods/4234): This mod's "Train Timetable" channel is hidden.
* [Stardew Valley Expanded](https://www.nexusmods.com/stardewvalley/mods/3753): This mod's "In the Cans" channel correctly reflects the different arrangement of garbage cans.

If any of your other mods affect the areas this mod covers, the TV may make incorrect predictions. If you would like me to add support for another mod, please open an issue [on GitLab](https://gitlab.com/kdau/predictivemods/-/issues) or in the Bugs tab above.

## ![[Installation]](https://www.kdau.com/headers/installation.png)

1. Install [SMAPI](https://smapi.io/)
1. Install [PlatoTK](https://www.nexusmods.com/stardewvalley/mods/6589) (This is different from PyTK!)
1. Install [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) (optional, for easier configuration)
1. Download this mod from the link in the header above
1. Unzip the mod and place the `PublicAccessTV` folder inside your `Mods` folder
1. Run the game using SMAPI

## ![[Use]](https://www.kdau.com/headers/use.png)

Generally, you will need to befriend the host of a program and see a special event with them to get them to start broadcasting. After that, simply turn on any TV and choose the program you want to see.

The standard TV programs, plus any programs added by other mods, will still air normally. The new programs and their requirements are detailed below.

### Governor's Message

The Governor will go live to announce rare events to the entire region. This program is available immediately, but only airs when there is an event to announce. Keep an eye out for it!

### Mining Advisory

Tune in to Marlon and Gil at the Adventurer's Guild on any day you're considering a trek into the mines. They'll give you advance warning of major monster activity and point you to any particularly valuable caches. Complete the "Initiation" quest to access this members-only program; Marlon will write you a letter when it's available to you. *[SVE](https://www.nexusmods.com/stardewvalley/mods/3753) players will also need two hearts of friendship with Marlon.*

### In the Cans

Hosted by Linus, this daily program clues you in on loot you can find in garbage cans that day. This is a passion project he's sharing with you on a little-known channel. Reach four hearts of friendship with Linus, then look in a can yourself, to inspire him to go on the air.

### Train Timetable

Each day, Demetrius comes to you from his lab with a look at the trains scheduled to pass through town soon. Once the railroad is accessible and you have at least two hearts of friendship with Demetrius, meet him by the tracks. If you share his interest, he'll decide to keep his fellow railfans in the loop.

### Other channels

**(Spoiler for late-game 1.4 content)** Movie Sneak Preview: Once the movie theater opens, your friendly concessionaire will announce the featured and coming attraction daily. She'll also tip you off on whether there's a line for the ever-popular crane game.

## ![[Configuration]](https://www.kdau.com/headers/configuration.png)

If you have installed Generic Mod Config Menu, you can access this mod's configuration by clicking the cogwheel button at the lower left corner of the Stardew Valley title screen and then choosing "Public Access TV".

Otherwise, you can edit this mod's `config.json` file. It will be created in the mod's main folder (`Mods/PublicAccessTV`) the first time you run the game with the mod installed. These options are available:

* `InaccuratePredictions`: Set this to `true` to enable channels whose information is inaccurate due to game version mismatch and/or conflicting mods. For entertainment purposes only.
* `BypassFriendships`: Set this to `true` to put all TV hosts on the job as soon as applicable, regardless of their friendship level with the player.

## ![[Translation]](https://www.kdau.com/headers/translation.png)

This mod can be translated into any language supported by Stardew Valley. It is currently available in English, French, Korean, Portuguese, Russian and Simplified Chinese.

Your contribution would be welcome. Please see the [details on the wiki](https://stardewvalleywiki.com/Modding:Translations) for help. You can send me your work in [a GitLab issue](https://gitlab.com/kdau/predictivemods/-/issues) or the Comments tab above.

## ![[Acknowledgments]](https://www.kdau.com/headers/acknowledgments.png)

* Like all mods, this one is indebted to ConcernedApe, Pathoschild and the various framework modders.
* The prediction logic behind this mod is largely ported from the [Stardew Predictor](https://mouseypounds.github.io/stardew-predictor/) web app by MouseyPounds.
* Coding of this mod relied on [Climates of Ferngill](https://www.moddrop.com/stardew-valley/mods/664033-climates-of-ferngill) by Sakorona as a key example.
* The #making-mods channel on the [Stardew Valley Discord](https://discordapp.com/invite/StardewValley) offered valuable guidance and feedback.
* The French translation was prepared by Inu'tile.
* The Korean translation was prepared by lando793.
* The Portuguese translation was prepared by Ertila007.
* The Russian translation was prepared by Ghost3lboom.
* The Simplified Chinese translation was prepared by caisijing.
* The "Governor's Message" channel's opening jingles are clipped from [News jingle](https://freesound.org/people/jobro/sounds/169214/) by [jobro](https://freesound.org/people/jobro/), used under [CC BY-NC 3.0](http://creativecommons.org/licenses/by-nc/3.0/), and [jingle news](https://freesound.org/people/Jay_You/sounds/460424/) by [Stonefree](http://www.stonefree.de/), used under [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).
* **(Spoiler for late-game 1.4 content)** The "Movie Sneak Preview" channel's concessions ambient is assembled from [Popcorn Machine.mp3](https://freesound.org/people/kentdavies959/sounds/466661/) by [kentdavies959](https://freesound.org/people/kentdavies959/), used under [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/), and [Pouring Carbonated Beverage Fizz.wav](https://freesound.org/people/baidonovan/sounds/187355/) by [baidonovan](https://freesound.org/people/baidonovan/), in the public domain.

## ![[See also]](https://www.kdau.com/headers/see-also.png)

* [Release notes](https://gitlab.com/kdau/predictivemods/-/blob/master/PublicAccessTV/RELEASE-NOTES.md)
* [Source code](https://gitlab.com/kdau/predictivemods/-/tree/master/PublicAccessTV)
* [Report bugs](https://gitlab.com/kdau/predictivemods/-/issues)
* [My other Stardew stuff](https://www.kdau.com/stardew)
* Mirrors:
	* [Nexus](https://www.nexusmods.com/stardewvalley/mods/5605),
	* **ModDrop**,
	* [forums](https://forums.stardewvalley.net/resources/public-access-tv.55/)

Other things you may enjoy:

* ![[icon]](https://www.kdau.com/PortableTV/icon.png) [Portable TV](https://www.moddrop.com/stardew-valley/mods/761325-portable-tv) mod to watch these channels on the go
* ![[icon]](https://www.kdau.com/ScryingOrb/icon.png) [Scrying Orb](https://www.moddrop.com/stardew-valley/mods/756553-scrying-orb) mod for mystical, forward-looking predictions
* ![[icon]](https://mouseypounds.github.io/stardew-predictor/favicon_p.png) [Stardew Predictor](https://mouseypounds.github.io/stardew-predictor/) web app to see all the predictions at once
* [Gardening with Hisame](https://www.nexusmods.com/stardewvalley/mods/5485) mod for farm beautification tips via TV
* [Monthly Mail](https://www.nexusmods.com/stardewvalley/mods/4523) mod for more periodic content themed around your game activity
