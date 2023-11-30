**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/drbirbdev/StardewValley**

----

# Gameboy Arcade

A Gameboy emulator integrated into Stardew Valley.  Use SMAPI content packs to add roms, which can be started via events, tile actions, or custom big-craftable interactions.

## Available Configs

Control scheme setup

## Translation

Translation is automated by DeepL.  Feel free to PR suggested changes in your language.

## Available to Modders

### Content Packs

Include a ROM as a content pack, with some options.

#### SaveStyle

How should game saves be handled.

* LOCAL - Each player on each farm gets a different save.
* GLOBAL - Each player gets one save shared between their farms.
* SHARED - There is one file that all players share.
* NONE - There is no save data.

#### LinkStyle

Doesn't matter, doesn't work.

#### SoundStyle

How should sound be handled.  Currently sound isn't emulated well, so should probably leave it off.

* BROKEN - Attempt to use the broken sound emulation.  It sounds bad.
* NONE - No sound is played by the emulator.

### Tile Action

`drbirbdev.GameboyArcade_Play <modId_contentId>`

Creates a choice dialogue which allows the player to play a game or not, similar to JunimoKart.

### Event Command

`drbirbdev.GameboyArcade_StartGame <modId_contentId>`

Begins a Gameboy Arcade minigame during an event.  Event controls should still mostly work, such as displaying dialogues.

`updateMinigame <id>`

Use this to control the minigame via the event.  Id is a number corresponding to game controls.

* 0 - End game (Power)
* 1 - Press Up
* 2 - Press Down
* 3 - Press Left
* 4 - Press Right
* 5 - Press A
* 6 - Press B
* 7 - Press Start
* 8 - Press Select
* 9 - Enable player control (enabled by default)

negative numbers release buttons (or disable player control in the minigame).

### BigCraftable Custom Field

`drbirbdev.GameboyArcade_GameID`

Creates a choice dialogue which allows the player to play a game or not, when interacting with this BigCraftable.

Use to create arcade machine items.

## Roadmap (or wishlist, I haven't been able to figure this stuff out)

* Fixed sound (currently am unable to sync sound correctly, probably due to unpredictable delay between ticks in stardew, as well as unknown delay in Monogame)
* Local Multiplayer link cable support
* Remote Multiplayer link cable support via double emulation
