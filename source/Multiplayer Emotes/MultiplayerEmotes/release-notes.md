**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/FerMod/StardewMods**

----


# Release Notes

## 1.0.0-beta.4

- Fixed open/close sound playing every left-click
- Fixed emotes sending multiple times

## 1.0.0-beta.3

- Updated mod for SMAPI 3.0 (Thank you Pathoschild)
- Added the ability to see the NPC and FarmAnimals emotes
- Added the emote box and the arrow to change of side if doesnt fit in the screen
- Added new commands:
  - Added `multiplayer_emotes`, that allows to see who else have this mod
  - Added `emote_npc`, that allows to force a NPC to play an emote
  - Added `emote_animal`, that allows to force a FarmAnimal to play an emote
- Added new settings:
  - Added `AllowNonHostEmoteNpcCommand`, that gives/removes permission to non host players to execute the command `emote_npc`
  - Added `AllowNonHostEmoteAnimalCommand`, that gives/removes permission to non host players to execute the command `emote_animal`
- Changed mod description
- Fixed emote interface showing in cutscenes

## 1.0.0-beta.2

- Added mouse scroll support
- Added `MinimumApiVersion` and change `Description` in `manifest.json`
- Fixed emote menu button not receiving mouse clicks
- Fixed emote menu button issues:
  - Fixed duplicated emote menu button when forced to return to `TitleMenu` (e.g. client disconnection from host)
  - Fixed not saving last position
  - Fixed strange animation behaviour

## 1.0.0-beta.1

- Added mod configurations
- Improved the way the emotes send and receive

## 1.0.0-beta

- First release
