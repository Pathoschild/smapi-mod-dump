**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://gitlab.com/Capaldi12/wherearethey**

----

# Where Are They

Stardew Valley mod to display location of every player in HUD.

## Download

You can download mod from its [page on Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/13208).

## Roadmap (kinda)

- [x] Getting farmers' locations and saving them
- [x] Overlay, displaying locations on the screen
	- [x] Display location name
	- [x] Displaying farmer icon (face)
- [x] Refactor to proper OOP code
- [x] Proper location names - I18n  // Thanks to shekurika I don't need to make it all myself
- [x] Configuration and customization
	- [x] Make configurable through GMCM
	- [x] Paddings, margins and spacings
	- [x] Position of overlay and anchor point
	- [x] Icon position (left/right, relative to text)
	- [x] Hide your own line
	- [x] Hide in Singleplayer
		- [x] And when alone on a multiplayer  // Works automatically
- [x] Publish as version 1.0 to nexus?
- [x] Fix split screen
- [x] Add update keys
- [x] Hide on festival and in cutscene
- [ ] Align lines right-side when icons are after text?
- [ ] Tooltip with name when hovering over icon
- [ ] Extra stuff to display
	- [ ] Cutscene
	- [ ] Talking
	- [ ] Sleeping
	- [ ] Fishing ?
	- [ ] Passed out ??

## Changelog

### 0.0.1

- Getting and displaying farmer locations

### 0.0.2

- Displaying farmer icon instead of the name

### 0.0.3

- Refactoring

### 0.0.4

- Proper location names and their localization

### 0.0.5

- Configuration and GMCM integration

### 0.0.6

- More configuration options:
	- Position and offset
	- Icon position

### 0.0.7

- More configuration options:
	- Hide when alone/singleplayer
	- Hide own line
	- Highlight in same location

### 0.0.8

- Russian localization of mod and vanilla names

### 1.0.0

- Published on nexus

### 1.0.1

- Fixed split-screen multiplayer overlay flickering

### 1.0.2

- Added update keys for nexus page

### 1.0.3

- Added options to hide overlay during cutscene and festival

### 1.0.4

- Fixed mod not working for farmhands in non-splitscreen multiplayer

## Localization

Thanks to shekurika, I have all vanilla location names and some modded ones, but there is still a lot missing (e.g. there's no modded locations in German), as well as some languages entirely. If you have new translation or additions/improvements to existing one, feel free to message me or make a pull request.

### Current state

_I don't know yet what mods are included since I just copied it all form shekurika's EventLookup mod. I'll update (expand) the table as I go with development_

| Language | Mod | Vanilla locations | Modded locations |
| --- | --- | --- | --- |
| English | + | + | ~ |
| German | - | + | - |
| Japanese | - | + | ~ |
| Portuguese | - | + | ~ | 
| Russian | - | + | - | 
| Turkish | - | + | ~ |
| Chinese | - | + | ~ | 

## Credits

Thank you to:

- [shekurika](https://www.nexusmods.com/stardewvalley/users/69153238) (shekurika#8884 on Discord) for letting me use his location names list and saving me a lot of time.