**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/evfredericksen/StardewSpeak**

----

# Release notes

## Unreleased

* Add skills page
* Add social page
* Add gift log
* Add collections page
* Add "hold mouse" and "release mouse" commands

## 0.0.12 (March 25, 2022)

* Fix release bug related to copying user lexicon file
* Fix release bug related to .NET 5 file structure
* Minor logging and build improvements
## 0.0.11 (March 21, 2022)

* Add compatibility for SMAPI >= 3.13.4
* Update mimimum SMAPI version to 3.13.4

## 0.0.10 (August 2, 2021)

* Add the ability to specify types of debris in the clear debris command
* Add mine ore command
* Chest-specific commands for container menu
* Remove three second delay for /mimic command

## 0.0.9 (June 4, 2021)

* Fix issues with moving cursor and changing items while performing certain commands like chopping trees and clearing debris
* Use virtual keyboard buttons rather than internal game method for changing items to avoid occasional glitches
## 0.0.8 (June 1, 2021)
First public release!

* Disable exclusive audio mode for default input device on launch using SoundVolumeView.exe
* Add "ladder down" command for mines and skull cavern
* Continue attack objective even if pathfinding towards a monster fails. This is primarily for flying monsters like bats and cave flies
## 0.0.7 (May 30, 2021)
* Limit request handling when unvalidated update event runs
* Clear grass command
* Debug field for ModConfig
* Bugfixes for shop menu

## 0.0.6 (May 18, 2021)
* Add keybindings to stop and restart speech engine

## 0.0.5 (May 15, 2021)
* Add the ability to dictate dialogue responses
* Remove unneeded 100ms sleep when clicking components

## 0.0.4 (May 12,2021)
* New command to move in a cardinal direction while navigating around terrain.
* Small update to mailbox menu.
* Move catch fish command to fishing menu.
* Show HUD message on failed attempt to equip item.

## 0.0.3 (May 4, 2021)
* Skip cutscene command
* Small tweaks to NPC pathfinding

## 0.0.2 (April 30, 2021)
* Co-op menu
* Co-op farm options in character customization menu
* Add optional number to nearest item command
* Improve logging message when unable to pathfind to next location
* Track previously onscreen items when foraging or digging artifacts
* Separation of always active vs non-menu commands.

## 0.0.1 (April 18, 2021)
* Initial test release  
