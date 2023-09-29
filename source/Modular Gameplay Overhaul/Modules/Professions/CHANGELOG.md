**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/daleao/modular-overhaul**

----

# PROFS Changelog

## 3.1.3

### Changed

* Changed the way Scavenger and Prospector treasures scale with the streak. Players should now see significantly more treasure if they manage to keep their streaks high.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 3.1.0

### Changed

* [CP] Fish Pond Data is now merged into the main mod, so no longer requires a separate download.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 3.0.2

### Changed

* Parallelized Luremaster logic to maybe improve performance on some systems.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 3.0.0

### Fixed

* SpaceCore skills should once again appear in the Generic Mod Config Menu for Exp. Multipliers.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.5.7

### Added

* Added to the `profs list` command the option to specify a skill name and get a list of professions in that skill. This can be helpful to find out the names of professions to use with the `profs add` command.

### Fixed

* Fix for Binning and Socializing skills not being loaded correctly.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.5.6

### Changed

* Reverted Aquarist to pre-version 2.5.0. Users pointe out that the change made it impossible to get pirate treasure.
    * Catching bar ~~increases more quickly~~ -> decreases more slowly.
* Custom skills are now loaded on GameLaunched. Previously all skills were loaded on SecondSecondUpdateTicked, because Love Of Cooking itself loads on FirstSecondUpdateTicked, and that needs to happen *first*. However, this was confusing and unecessarily complex for other developers. So now, LoC alone continues to load on SecondSecondUpdateTicked while everyone else loads at the reasonable stage.

### Fixed

* Fixed a small issue where the player could still keep the Limit Break associated with a removed profession. This probably doesn't concern 99.8% of players.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.5.5

### Added

* Added config to disable extended progression (level 20) while Prestige skill resetting is enabled.

### Changed

* Default value for ConservationistTaxDeductionCeiling changed to 100%.

### Fixed

* Fixed possible KeyNotFound exception is SLNGS module is not enabled.
* Explosive ammo now slays Mummies as expected.
* Fixed a possible KeyNotFound exception if placing a Crab Pot in an unusual map, like the Boat Tunnel.
* Fixed Prospector panning point tracking pointing off-screen.
* Fixed integration with Mushroom Propagator v2.2.0.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.5.1

### Fixed

* Fixed all xp gain being capped at 1.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.5.0

Major Overhaul for Fishing professions.

### Added

* Prospector Hunt streak now also buffs the chance of spawning panning points.
* Slingshot projectiles now leave a trail effect when overcharged.
* SpaceCore skill prestige choices can now also be respecced at the Statue of Prestige (untested).

### Changed

Major changes to the Fishing tree:
* **Fisher:**
    * Changed from what was essentially a "free permanent bait" effect, to "baits are twice as effective". The end result is more or less equal, but actually encourages using bait instead of trivializing it. Also makes Wild Bait (chance to double the catch) and Magnet (chance for pirate treasure) more appealing.
    * New prestige perk: baits are *three times* more effective. *Pretty obvious and natural progression. With Wild Bait the end result is effectively instant-nibble.*
* **Angler:**
    * Changed from "increased fish value by 1% per unique max-sized fish caught" to "increased fish value by 0.5% per unique fish caught"; i.e., scaling is reduced by half, but no longer requires max-size. Legendaries, likewise, are worth half as much, at 2.5% per.
    * Added: Fully-expending a tackle causes the fishing rod to "remember" the tackle's effects. Effectively, this doubles the duration of all tackles, but more importantly, it allows two tackles to be used at the same time. **Tackle effects cannot stack with themselves.** The currently "memorized" tackle appear in the rod's tooltip next to the equipped tackle, with transparency indicating the number of remaning uses.  
    * Removed "legendary fish recatch" prestige effect. New prestige perk: "increased fish vaue by *another* 0.5% for each unique max-sized fish caught". This makes up the other half of original Angler's scaling bonus.
* **Aquarist:**
    * Changed from "catching bar decreases more slowly" to "increases more quickly" for each unique fish species raised in a Fish Pond. *Actually helps to save time by reducing the mini-game duration, instead of just making the mini-game trivially easy.*
    * Ability to raise legendary fish in ponds is now available without prestige. *No longer will I have to answer folks who don't read the description why their Legend refuses to go in the pond. This perk was always more fun than gamechanging, so no reason to gate this until extremely late game.*
    * New prestige perk: pond capacity +4. *More useful and in-line with the Prestige theme.*
    * Default value for LegendaryPondPopulationCeiling lowered from 6 to 5.
* **Luremaster:**
    * Now also allows **baited** Crab Pots to produce 2x per day. Previous effects remain unchanged. *Huge and desperately needed buff. Also establishes a neat analogy to Producer.*
    * New prestige perk: produce 3x per day. *The natural progression.*
* **Conservationist:**
    * Village favor can now be gained without prestige. *This perk was pretty useless at extremely late game. It now gives further incentive to pick this profession early.*
    * New prestige effect: 50% faster trash build-up. *Not great, but the best I could come up with.*
    * Trash obtained from regular fishing also counts for tax deductions.

### Removed

* Removed LegendaryPondPopulationCeiling option. Max population of legendary ponds is now always half that of regular ponds (yes, Aquarist perk is also halved, for a total max of 7 with prestige).

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.4.0

### Added

* Brute Frenzy now causes Fear status on activation, if CMBT is enabled.
* Added compatibility for [Archery](http://www.nexusmods.com/stardewvalley/mods/16767).
    * Rascal's preservation effect will apply on top of the natural preservation chance of bows.
    * Bows can be overcharged by Desperado, and gain all the corresponding perks (damage bonus, knockback bonus, hitbox bonus and piercing chance). Note, however, that overcharging increases the odds of breaking arrows.
* Added a short delay before Desperado's overcharge begins. It should be barely noticeable while overcharging, but is *very* noticeable when you *don't* want to overcharge, so you don't hear the charging sound as often.

### Fixed

* Fixed Limit Break gauge still rendering with Limit Breaks disabled.
* Fixed a possible conflict with XPDisplay.
* Fixed an issue with Desperado's charging speed buff, which actualy reduced charging speed the player lost HP, instead of the opposite.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.3.0

### Added

* Added config settings to displace the Limit Gauge bar.

### Fixed

* ArtisanGoodsAlwaysInputQuality should now apply to PFM machines.
* Custom professions now show up in GMCM menu for BaseSkillExpMultiplier setting.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.2.8

### Changed

* Luremaster-owned Magnet-baited Crab Pots no longer produce trash, as stated in its description.

### Fixed

* LaxOwnershipRequirements should now apply to Rancher bonus.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.2.2

### Fixed

* Adjusted the position of prestige progression badges, which I guess were messed up by the latest SpaceCore update.
* Fixed ribbon-style prestige badges, which apparently were messed up by an update a while ago and nobody noticed, since nobody apparently uses it anymore.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.2.1

### Changed

* Refactored patchers in the Patchers.Common namespace, just for consistency.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.2.0

### Changed

* The intensity of the Slime ammo's slow effect has been reducing from 50% to 33.3%
* Desperado's overcharge bar now draw on RenderedWorld instead of RenderedHud, which makes it compatible with different zoom levels.

### Fixed

* Fixed possible bugs when a Rascal Slingshot is picked up by a non-Rascal.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.1.0

### Changed

* Special Abilities are now called Limit Breaks. The gauge has been changed accordingly.

### Fixed

* Fixed some possible bugs related to the number of attachments in a slingshot.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.0.5

### Fixed

* Config settings should now save correctly and not reset on every launch.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.0.4

### Fixed

* Fixed an ArgumentOutOfRange exception, which is, interestingly enough, caused by Vanilla when scrolling the toolbar.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.0.0

### Added

* Added a config to toggle the Crystalarium upgrade for Gemologists.
* Added a config to revert Artisan behavior to always preserve input quality.
* Added a config to cap the bonus from Aquarist.
* Conservationist now counts trash fished with a Fishing Rod.

### Changed

* Slingshot attachments upgrade for Rascals now occurs on equip rather than on profession gain. This should be more compatible and prevents a possible Null-Reference.

### Fixed

* Added several missing config options to the GMCM menu.
* Fixed a bug which allowed players to select both level 10 professions for custom skills during level-up.
* Fixed small typo in new Artisan description (removed "is").
* Fixed Poacher still gathering Special Ability charge with Special Abilities disabled.
* Fixed issue with Crab Pots ignoring farmhand professions in multiplayer games with LaxOwnershipRequirement set to `true` (thanks to [ncarigon](https://github.com/ncarigon)).
* Fixed ribbon display with new scrolling feature of SpaceCore.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 1.3.0

### Fixed

* Fixed a bug preventing explosive ammo from exploding without the Desperado profession.
* Fixed a possible Out-Of-Range Exception when hovering a Rascal Slingshot.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 1.2.1

### Changed

* Changes to Pacher Ambush -> *If an enemy is slain within ~~1.5s~~ 0.5s out of Ambush, immediately regain ~~20%~~ 25% special ability charge.*

### Fixed

* Fixed a possible throw when poaching from a monster that has no item to drop.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 1.2.0

### Changed

* Dramatically increased Blaster's chance of producing coal. Apparently the vanilla chance is so retardedly low that just doubling it doesn't make a noticeable difference.
* Piper Slimes no longer take on the properties of Hutch Slimes, because I decided that wasn't really immersive. Piper Slimes are not summoned from the Hutch; they are attracted due to the Piper's "sliminess", and therefore they should be native to the current location.

### Fixed

* The Prestige API should now be fully functional, so custom skill mod authors can now support prestige profession variants.
* Fixed the Agriculturist perk, which I straight up forgot to include in 1.1.0.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 1.1.1

### Changed

* Spelunker momentum is now gained on interaction with a ladder or sink hole, and not on player warped. This prevents exploiting other warp methods such as the elevator to rank up the buff for free.
* Prestige mechanic should now be fully functional for custom SpaceCore skills. Expect Binning and Socialization compatibility soon!

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 1.1.0

### Changed

* **Agriculturist:** The chance for Iridium Quality crops has been reduced to half of the chance with Deluxe Fertilizer.
* **Artisan:** Effect changed from "output quality **is at least as good** as input quality" to "output quality **can be as good** as input quality". I.e., instead of simply preserving the quality of the input, there is now a formula in place to choose a quality that is lower than or equal to that of the input. The 5% promotion chance still applies.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 1.0.2

### Added

* Prospector now also tracks Resource Clumps.
* Added Custom Resource Clumps integration.

### Fixed

* Fixed buggy nodes being spawned by Prospector.
* Fixed a typo in Brute's Rage buff.
* Fixed Custom Ore Nodes integration.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 1.0.1

### Changed

* Prospector ore spawning chance is no longer recursive, which should dramatically reduce the odds of extra node spawns.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 1.0.0

### Changed

* Reduced the spawn chance of extra mining nodes by Prospector.

### Fixed

* Fixed the range for triggering Demolitionist's Get Excited buff.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 0.9.9

### Fixed

* Fixed the display of Brute and Spelunker buffs.
* Brute buff now activates in a more clever way.
* Desperado Overcharge meter no longer renders in legacy firing mode.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 0.9.7

### Changed

* Custom skills are now loaded on 2nd SecondUpdateTicked instead of SaveLoaded.

### Fixed

* Fixed a typo causing a Null-Reference Exception when resetting a custom skill.
* Treasure hunts will no longer trigger during cutscenes.
* Fixed an incorrect patch target which should allow taunted monsters to reset their aggro when the taunter dies.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 0.9.6

### Added

* Added OnProfessionAdded and OnProfessionRemoved events. So far they only serve to invalidate the cache of profession icons, but will likely be useful in the future.

### Changed

* Lowered the log level of Ultimate setter from Warn to Info.

### Fixed

* The level 20 prestige choices should now accurately reflect the prestige branch chosen at level 15.
* The profession choice menu should now correctly display prestige profession icons.
* The skills page profession icons should now also correctly display the prestige choices made on the night previous.
* Replaced two more incorrect FieldGetters with the correct PropertyGetterGetter in Automate integration.
* Fixed a typo in "multiplyer" FieldGetter.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 0.9.4

### Fixed

* Fixed the order of displayed Prestige ribbons in Skills page.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 0.9.2

### Fixed

* Fixed Scavenger and Prospector tracking arrows not being displayed.
* Improved tracking arrow position for tracked bushes on-screen.
* Replaced an incorrect FieldGetter with the correct PropertyGetterGetter in Automate integration.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 0.9.1

### Fixed

* Fixed an issue with Luck Skill integration.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 0.9.0 (Initial release)

### Added

* When a Gemologist reaches a new quality threshold, all currently processing Crystalariums owned by that player will likewise receive a quality upgrade to reflect that.
* Scavenger profession now has a chance, proportional to the Treasure Hunt Streak, to spawn additional forage when entering a new map.
* Prospector profession, likewise, now has a chance, proportional to the Treasure Hunt Streak, to spawn additional ore nodes when entering a new mine level.
* Added golden versions of profession icons for Prestiged professions.
* Added config setting to disable Bee House being affected by Producer profession.
* Added API for custom Skill mods to register prestiged professions.

### Changed

* **Rascal** - ~~Slingshot damage +25%. 60% chance to recover spent ammo.~~ Gain one additional ammo slot. 35% chance to recover spent ammo.
    * The damage perk is gone. I always felt like Slingshot damage was overpowered anyway. In its place comes a second ammo slot; Rascals can now equip 2 different types of ammo (or the same ammo twice). Use the Mod Key (default LeftShift) to toggle between equipped ammos.
    * The ammo recovery perk has been nerfed to account for new enchantments provided by the Arsenal module. In exchange, the Prestige perk now doubles this value to a whopping 70%---a higher value than the original.
    * Ability to equip Slime ammo moved from Slimed Piper to Rascal. It will still cause a slow debuff, but will not heal ally Slimes unless the player *also* has the Piper profession.
* **Slimed Piper** - Summoned Slimes are now friendly (will not attack players, but will still cause damage if touched).
* **Desperado (Prestiged)** - ~~Overcharged shots become spreadshots.~~ Overcharged shots can pierce enemies.
    * The Spreadshot perk has been moved to a new enchantments provided by the Arsenal module. The ability to pierce enemies with overcharged shots is now the new prestige perk.
* The [API](../../API/IModularOverhaulApi.cs) has been slightly changed. Some mods may need to update the corresponding interfaces.

### Fixed

* Fixed an issue with Automated Junimo Chests being unable to decide their owner.
* Fixed a bug where Conservationist Trash Collected would not reset if no trash was collected during the season.
* Fixed a bug where Tapper perk would incorrectly apply to tapped Mushroom Trees in Winter, causing multiple progressively shortened harvests.
* Fixed a bug where the Piper Concerto super ability caused all Slimes in the Hutch to disappear.
* Crab Pots now correctly trap Seaweed instead of Green Algae in the Beach farm map.
* Fixed a bug where Special Abiltiy charge would still accumulate with Enable Specials config set to false.
* Fixed a possible NullReferenceException when shooting at Slimes.
* Fixed TrackerPointerScale and TrackerPointerBobbingRate config settings.
* Added a setter to the CustomArtisanMachines config, preventing it from being reset on game load.
* The print_fishdex console command now takes into account the value of AnglerMultiplierCap config.
* Fixed some translation errors.

### Removed

* Removed the SeaweedIsTrash config setting.
* Removed configs from the [IProfessions interface](../../API/IModularOverhaul.cs).

[🔼 Back to top](#profs-changelog)