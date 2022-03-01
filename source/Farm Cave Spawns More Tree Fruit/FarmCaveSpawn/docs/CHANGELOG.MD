**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/FarmCaveSpawn**

----

Changelog
======
#### Todo

* Nothing! Awaiting future object updates, but have no more ideas at current.

#### 1.6 todo
* Switch all ID lookup to the new system.
* Figure out DGA fruit trees.
<!-- bat eyes for custom locations? -->

#### Version 1.0.9

* Fixes error items spawning in whenever no tree fruits are possible.
* Adds in a setting to exclude winter from Seasonal Only.
* Renames console command.

#### Version 1.0.8

* Corrects `UseModCave` option (would previously always use the additional locations list.) Adds new option to only use the six usual vanilla tree fruit.
* Add error checking for badly formatted config files, and clamps the possible values for the config files. Adds version checking for GMCM, and disables GMCM integration if the version is too old.
* Add bats to mine entrance if spawning there is enabled.
* (Internal) Fixes tile shuffling logic.
* (Internal) Use delegates

#### Version 1.0.7

* Adds compat with FarmCaveFramework (https://www.nexusmods.com/stardewvalley/mods/10506).

#### Version 1.0.6

* Adds ability for other mods to add locations this mod will spawn fruit in, with the ability to limit the spawn area. (Move SVE additional farmcave location support to that.) 
* Add configuration option that can disable that.

#### Version 1.0.5

Fix the options, because I screwed them up trying to de-hardcode them...

#### Version 1.0.4

* Fix check for hostplayer
* New setting `PriceCap` will limit the value of fruit that can be spawned
* Mods can now prevent this mod from spawning their fruit.

#### Version 1.0.3

Adds ability to spawn in the mine entrances as well

#### Version 1.0.2

Fix bug for early spawning settings

#### Version 1.0.1

Adds options for early farm cave and limiting to seasonal fruit only; adds more support for SVE.

#### Version 1.0.0

Initial upload

