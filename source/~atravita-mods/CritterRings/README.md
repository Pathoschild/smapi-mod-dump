**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/StardewMods**

----

Critter Rings
=================================

![Header image](docs/image.png)

This incredibly self-indulgent mod has five rings in it: the Butterfly Ring, the Bunny Ring, the Frog Ring, the Owl Ring, and the Firefly Ring. Each summons the relevant critter when worn. Additionally, each has extra secondary effect. Frog jumps, bunny ring has a sprint, the owl ring makes it so enemies are less likely to see you, firefly is a glow ring, and butterfly is a magnetic ring.

## Install

1. Install the latest version of [SMAPI](https://smapi.io).
2. Download and unzip [Spacecore](https://www.nexusmods.com/stardewvalley/mods/1348) and [Json Assets](https://www.nexusmods.com/stardewvalley/mods/1720), and unzip both into `Stardew Valley/Mods`.
2. Download and install [AtraCore](https://www.nexusmods.com/stardewvalley/mods/12932).
2. Download this mod and unzip it into `Stardew Valley/Mods`.
3. Run the game using SMAPI.

## Rings
* Bunny Ring: Allows a 14s "sprint" of +5 (configurable) speed in return for 10 (configurable) stamina. Summons bunnies. Buy from Marlon after receiving the Skull Key and the Magic Ink.
* Butterfly Ring: Acts as magnet ring, summons butterflies before dark. Buy from Marlon after receiving the Skull Key.
* Firefly Ring: Acts as a glow ring, summons fireflies after dark. Buy from Marlon after receiving the Skull Key.
* Frog Ring: Hold down the jump button to charge up a jump! Costs stamina, though. Buy from Marlon after receiving the Skull Key and the Magic Ink.
* Owl Ring: Makes enemies less likely to see you. Summons owls. Buy from Marlon after receiving the Skull Key and the Magic Ink.

## Configuration
Run SMAPI at least once with this mod installed to generate the `config.json`, or use [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) to configure.

#### General Options:
* `CritterSpawnMultiplier`: Controls how many critters will spawn per interval of time. Set it higher to get more critters.
* `PlayAudioEffects`: Whether or not the audio effects from this mod will be played.

#### Butterfly Ring Options
* `ButterfliesSpawnInRain`: if enabled, butterflies will spawn in the rain.

#### Bunny Ring Options.
* `BunnyRingStamina`: How much stamina each sprint of the bunny ring should take.
* `BunnyRingBoost`: Indicates how much speed the bunny ring sprint should add. Default +3.
* `BunnyRingButton`: Indicates which button(s) to use for the bunny ring's sprint.

#### Frog Ring Options
* `MaxFrogJumpDistance`: Maximum distance the frog ring will jump you.
* `FrogRingButton`: The button used for the frog jump.
* `JumpChargeSpeed`: Indicates how quickly the jump should charge up. Higher for faster.
* `JumpCostsStamina`: Whether or not the jump should cost stamina. If enabled, it will cost one stamina point per square moved.
* `ViewportFollowsTarget`: If enabled, the viewport will try to follow the midpoint between the target and the player character.

* `FrogsSpawnInHeat`: If enabled, frogs will spawn in places probably too hot for them, like the volcano.
* `FrogsSpawnInCold`: If enabled, frogs will spawn in places probably too cold for them, like during winter or in the ice levels of the mines.
* `FrogsSpawnOnlyInRain`: If enabled, frogs will only spawn outdoors when it's raining.
* `SaltwaterFrogs`: If enabled, frogs might spawn in saltwater.
* `IndoorFrogs`: If enabled, frogs might spawn indoors.

#### Owl Ring Options

* `OwlsSpawnIndoors`: If enabled, owls will spawn indoors.
* `OwlsSpawnDuringDay`: If enabled, owls will spawn during the day.

## Compatibility

* Works with Stardew Valley 1.5.6 on Linux/macOS/Windows.
* Works in single player, multiplayer, and split-screen mode. In multiplayer, both players should install.
* Should be compatible with most other mods.

## Much thanks to
* [Violetlizabet](https://www.nexusmods.com/stardewvalley/users/120958053) for drawing the ring graphics for me! Check out her [Magic Buildings](https://www.nexusmods.com/stardewvalley/mods/10142) mod!
* [DecidedlyHuman](https://www.nexusmods.com/stardewvalley/users/79440738), for helping me with the viewport code. Check out his [Smart Building](https://www.nexusmods.com/stardewvalley/mods/11158)!
* pavlo2906 for the Russian translation!

## See also

[Changelog](docs/changelog.md)
