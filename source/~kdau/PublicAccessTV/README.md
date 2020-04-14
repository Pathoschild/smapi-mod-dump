# ![[icon]](assets/icon.png) Public Access TV

*a [Stardew Valley](http://stardewvalley.net/) mod by [kdau](https://kdau.gitlab.io)*

Your friends in the valley take to the airwaves to keep you apprised of today's mining conditions<!-- TODO: , shopping opportunities -->, garbage loot, train schedules, rare events and more.

This mod is largely based on the [Stardew Predictor](https://mouseypounds.github.io/stardew-predictor/) web app by MouseyPounds, whose tools are all worth a look. Its companion mod, [Scrying Orb](https://www.nexusmods.com/stardewvalley/mods/5603), offers a different route to some of the same information and more, but looks well beyond the current game day.

## Contents

* [Compatibility](#compatibility)
* [Installation](#installation)
* [Use](#use)
* [Configuration](#configuration)
* [Translation](#translation)
* [Acknowledgments](#acknowledgments)
* [See also](#see-also)

## Compatibility

This version of Public Access TV is compatible with **Stardew Valley 1.4.x**. When SDV 1.5 is released, some of this mod's predictions will start being wrong. I'll put out a new version at that point.

This mod should work on **Linux, Mac, Windows and Android**.

There are no known problems with multiplayer use. Only players with the mod installed will see the new channels.

There are no known conflicts with other mods. Public Access TV fully supports the following:

* [Stardew Valley Expanded](https://www.nexusmods.com/stardewvalley/mods/3753) (different arrangement of garbage cans)

When one of the following mods is installed, the related channel is disabled because it would not be accurate:

* [Better Garbage Cans](https://www.nexusmods.com/stardewvalley/mods/4171)
* [Better Train Loot](https://www.nexusmods.com/stardewvalley/mods/4234)

If any of your other mods affect the areas this mod covers, the TV may make incorrect predictions. If you would like me to add support for another mod, please open an issue [on GitLab](https://gitlab.com/kdau/predictivemods/-/issues) or [on the Nexus page](https://www.nexusmods.com/stardewvalley/mods/5605?tab=bugs).

## Installation

1. Install the latest version of [SMAPI](https://smapi.io/).
1. Install the latest version of the [PyTK](https://www.nexusmods.com/stardewvalley/mods/1726) mod.
1. Download this mod from its [Nexus page](https://www.nexusmods.com/stardewvalley/mods/5605?tab=files) or [ModDrop page](https://www.moddrop.com/stardew-valley/mods/757967-public-access-tv).
1. Unzip the mod and place the `PublicAccessTV` folder inside your `Mods` folder.
1. Run the game using SMAPI.

## Use

Generally, you will need to befriend the host of a program and see a special event with them to get them to start broadcasting. After that, simply turn on any TV and choose the program you want to see.

The standard TV programs, plus any programs added by other mods, will still air normally. The new programs and their requirements are detailed below.

### Governor's Message

The Governor will go live to announce rare events to the entire region. This program is available immediately, but only airs when there is an event to announce. Keep an eye out for it!

### Mining Advisory

Tune in to Marlon and Gil at the Adventurer's Guild on any day you're considering a trek into the mines. They'll give you advance warning of major monster activity and point you to any particularly valuable caches. Complete the "Initiation" quest to access this members-only program; Marlon will write you a letter when it's available to you. *[SVE](https://www.nexusmods.com/stardewvalley/mods/3753) players will also need two hearts of friendship with Marlon.*

<!-- TODO: Shop the Valley -->

### In the Cans

Hosted by Linus, this daily program clues you in on loot you can find in garbage cans that day. This is a passion project he's sharing with you on a little-known channel. Reach four hearts of friendship with Linus, then look in a can yourself, to inspire him to go on the air.

<!-- TODO: Fashion Showcase -->

### Train Timetable

Each day, Demetrius comes to you from his lab with a look at the trains scheduled to pass through town soon. Once the railroad is accessible and you have at least two hearts of friendship with Demetrius, meet him by the tracks. If you share his interest, he'll decide to keep his fellow railfans in the loop.

### Other channels

<details>
<summary>(Spoiler for late-game 1.4 content)</summary>

### Movie Sneak Preview

Once the movie theater opens, your friendly concessionaire will announce the featured and coming attraction daily. She'll also tip you off on whether there's a line for the ever-popular crane game.
</details>

## Configuration

The first time you run the mod, it will generate a `config.json` file in its main folder (`Mods/PublicAccessTV`). Two options are available:

* `BypassFriendships`: Set this to `true` to put all TV hosts on the job as soon as applicable, regardless of their friendship level with the player.
* `InaccuratePredictions`: Set this to `true` to enable channels whose information is inaccurate due to the presence of conflicting mods. For entertainment purposes only.

## Translation

This mod can be translated into any language supported by Stardew Valley. It is currently available in English, French and Russian.

Your contribution would be welcome. Please see the [details on the wiki](https://stardewvalleywiki.com/Modding:Translations) for help. You can send me your work in an issue [on GitLab](https://gitlab.com/kdau/predictivemods/-/issues), [on Nexus](https://www.nexusmods.com/stardewvalley/mods/5605?tab=bugs) or by DM on Discord.

## Acknowledgments

* Like all mods, this one is indebted to ConcernedApe, particularly for the vanilla assets it adapts.
* The prediction logic behind this mod is largely ported from the [Stardew Predictor](https://mouseypounds.github.io/stardew-predictor/) web app by MouseyPounds.
* This mod would not function without [SMAPI](https://smapi.io/) by Pathoschild and [PyTK](https://www.nexusmods.com/stardewvalley/mods/1726) by Platonymous.
* Coding of this mod relied on [Climates of Ferngill](http://www.nexusmods.com/stardewvalley/mods/604) by Sakorona as a key example.
* The #making-mods channel on the [Stardew Valley Discord](https://discordapp.com/invite/StardewValley) offered valuable guidance and feedback.
* The French translation was prepared by Inu'tile.
* The Russian translation was prepared by Ghost3lboom.
* The "Governor's Message" channel's opening jingles are clipped from [News jingle](https://freesound.org/people/jobro/sounds/169214/) by [jobro](https://freesound.org/people/jobro/), used under [CC BY-NC 3.0](http://creativecommons.org/licenses/by-nc/3.0/), and [jingle news](https://freesound.org/people/Jay_You/sounds/460424/) by [Stonefree](http://www.stonefree.de/), used under [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).

<details>
<summary>(Spoiler for late-game 1.4 content)</summary>

* The "Movie Sneak Preview" channel's concessions ambient is assembled from [Popcorn Machine.mp3](https://freesound.org/people/kentdavies959/sounds/466661/) by [kentdavies959](https://freesound.org/people/kentdavies959/), used under [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/), and [Pouring Carbonated Beverage Fizz.wav](https://freesound.org/people/baidonovan/sounds/187355/) by [baidonovan](https://freesound.org/people/baidonovan/), in the public domain.

</details>

## See also

* [Release notes](RELEASE-NOTES.md) for existing versions
* [Issue tracker](https://gitlab.com/kdau/predictivemods/-/issues) for bug fixes and minor enhancements
* [Roadmap](../ROADMAP.md) of major development plans
* [MIT license](../LICENSE) (TLDR: do whatever, but credit me)
* [My other mods](https://kdau.gitlab.io)

Mirrors:

* [This mod on Nexus](https://www.nexusmods.com/stardewvalley/mods/5605)
* [This mod on ModDrop](https://www.moddrop.com/stardew-valley/mods/757967-public-access-tv)
* [This mod on GitLab](https://gitlab.com/kdau/predictivemods/-/tree/master/PublicAccessTV)

Other things you may enjoy:

* ![[icon]](https://kdau.gitlab.io/PortableTV/icon.png) [Portable TV](https://www.nexusmods.com/stardewvalley/mods/5674) mod to watch these channels on the go
* ![[icon]](https://kdau.gitlab.io/ScryingOrb/icon.png) [Scrying Orb](https://www.nexusmods.com/stardewvalley/mods/5603) mod for mystical, forward-looking predictions
* ![[icon]](https://mouseypounds.github.io/stardew-predictor/favicon_p.png) [Stardew Predictor](https://mouseypounds.github.io/stardew-predictor/) web app to see all the predictions at once
* [Gardening with Hisame](https://www.nexusmods.com/stardewvalley/mods/5485) mod for farm beautification tips via TV
* [Monthly Mail](https://www.nexusmods.com/stardewvalley/mods/4523) mod for more periodic content themed around your game activity
