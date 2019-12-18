# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [2.3.1] - 2019-12-05
### Added
- `discord` console command to respond to Ask to Join requests via console.

### Changed
- `DiscordRP_TestJoin` changed to `DiscordRP_Join`
- Discord RPC only logs now in verbose mode.

### Removed
- Support for Mac and Linux.

### Fixed
- Fixed crashing from Discord Game SDK by changing libraries.
- Fixed joining games.

## [2.3.0] - 2019-12-05
### Added
- Added a check for Android, which will cancel mod initialization.

### Changed
- Small change to the debug build warning.

### Fixed
- Finally fixed Linux and Mac support and re-added the auto setup.

## [2.3.0-beta1] - 2019-05-12
### Added
- Re-added RPC handler listeners.
- Support for Ask to Join, with a config option to disable it.
- `DiscordRP_TestJoin` console command to join a game via invite code (for testing).

### Removed
- Automatic Mac and Linux setup because it's no longer needed.

### Changed
- Changed to new Discord Game SDK.
- API's `FormatText' function now returns `""` instead of `null` if given an empty string.

### Fixed
- ACTUALLY fixed potential error when generating presence in co-op.

## [2.2.2] - 2018-12-14
### Fixed
- Potential error when generating presence in co-op.
- Support for upcoming SMAPI version.

## [2.2.1] - 2018-11-20
### Added
- Automatic Linux and Mac setup.

### Fixed
- Compatibility with upcoming SMAPI version.

## [2.2.0] - 2018-10-12
### Added
- Instructions for installing on Mac and Linux.
- API for other mods to be able to add template tags for rich presence.
- Added optional "all" parameter to the `DiscordRP_Tags` command to show tags that return an error or are currently unavailable (such as in the menu).

### Fixed
- Compatibility with upcoming SMAPI version.

## [2.0.0] - 2018-07-14
### Added
- Warning in console when using a debug build or pre-release.
- Logging for Discord RPC.
- `DiscordRP_Reload` console command to reload config.
- Added a reload button (default F5) to reload the config. Customizable in the config.
- Rich presence customization using a template system in the config.
- `DiscordRP_Format` console command to format a template string used in the config and print out the result (for testing).
- `DiscordRP_Tags` console command that list all currently available template tags and their current output.

### Changed
- The mod's name in the logs is now "DiscordRP" instead of "Discord Rich Presence"
- Adjusted default rich presence.
- Different states are shown when loading / joining a game or starting a new game.

### Removed
- Discord RPC event handling due to some issues.

### Fixed
- Party IDs now work when playing co-op.

## [1.2.1] - 2018-05-15
### Fixed
- Hosting and Playing Co-Op now appear in the correct contexts.

## [1.2.0] - 2018-05-15
### Added
- Assembly info to the exported DLL file.
- In-game state to large image text, provided by the game.
- Current session play time is now shown.
- Config file with option for hiding farm names. You can hide all farm names by just typing "Farm".
- Logging for Discord events.

### Changed
- Mod is no longer compiled for 64-bit support, since Stardew Valley is 32-bit.
- Presence is now only updated every half second.

### Removed
- Weather and time text to make room for in-game state.

## [1.1.0] - 2018-05-12
### Added
- GitHub update key so SMAPI will check for updates from GitHub.
- Farm type is now shown on the large image.
- Weather icons for Stormy, Wedding Day, and Festival.

### Changed
- Presence now updates every tick instead of every half second.
- Changed text for weather types from Sun to Sunny, Rain to Rainy, and so on.

### Removed
- Removed Discord event handlers due to them not triggering.

[Unreleased]: https://github.com/FayneAldan/SVRichPresence/compare/v2.3.1...HEAD
[2.3.1]: https://github.com/FayneAldan/SVRichPresence/compare/v2.3.0...v2.3.1
[2.3.0]: https://github.com/FayneAldan/SVRichPresence/compare/2.3.0-beta1...v2.3.0
[2.3.0-beta1]: https://github.com/FayneAldan/SVRichPresence/compare/2.2.2...2.3.0-beta1
[2.2.2]: https://github.com/FayneAldan/SVRichPresence/compare/2.2.1...2.2.2
[2.2.1]: https://github.com/FayneAldan/SVRichPresence/compare/2.2.0...2.2.1
[2.2.0]: https://github.com/FayneAldan/SVRichPresence/compare/2.0.0...2.2.0
[2.0.0]: https://github.com/FayneAldan/SVRichPresence/compare/1.2.1...2.0.0
[1.2.1]: https://github.com/FayneAldan/SVRichPresence/compare/1.2.0...1.2.1
[1.2.0]: https://github.com/FayneAldan/SVRichPresence/compare/1.1.0...1.2.0
[1.1.0]: https://github.com/FayneAldan/SVRichPresence/releases/tag/1.1.0
