**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/KhloeLeclair/StardewMods**

----

# Changelog

## 1.7.0
Released May 18th, 2024.

### Added
* There are now settings for enabling / disabling shaders.
* Custom weather types have more control over music with their new
  `SoftMusicOverrides` property.
* Shader layers now have an optional `Color` that can be passed
  to the underlying shader for drawing.
* Console command for resetting locations if you happen to make
  a mistake when messing with the new Stardew 1.6 weather type.
* New trigger action for converting fruit trees from one type to another.
* New built-in `Blur` shader.

### Fixed
* Effects and layers not being properly disposed of when entering
  a location without an active custom weather type.
* The shader layer now understands how to send an array of float
  values to a shader.

### Translation
* Added Japanese translation courtesy of mitekano23 on NexusMods.
  Thank you!
* Updated French translation courtesy of Caranud on NexusMods.
  Thank you!


## 1.6.0
Released May 17th, 2024.

### Added
* `Shader` layers have arrived! Do fancier rendering for weather,
  if you dare.
* All effects and layers now have a `TargetMapType` for letting
  you specify if they should be active on indoor maps, outdoor
  maps, or both. Outdoor by default, of course.
* Custom art for Ginger Isle based on the original game assets to
  make it fit in a bit more when you're on the region selection menu.
* Support for the Ultimate Fertilizer mod with our trigger actions.
* Added a new trigger action for spawning custom artifact spots.

### Fixed
* The TV weather channel rendering on top of the world.
* Equality checks not working correctly for some layer/effect types.
* Do not render the timing overlay when taking a screenshot.
* The `SpawnForage` command not properly accepting more than one
  spawnable.

## Translation
* Added a localization entry for Calico Desert.
* Added Chinese translation courtesy of EtaMyosotis on NexusMods.
  Thank you!
* Added French translation courtesy of Caranud on NexusMods.
  Thank you!


## 1.5.0
Released May 10th, 2024.

### Added
* System for tracking the previous week's weather in all locations,
  which can be queried using game state queries.
* New trigger actions for: converting trees to other tree types,
  fertilizing dirt, growing crops, growing giant crops, growing
  fruit trees, growing trees, killing crops, spawning forage,
  un-fertilizing dirt, un-growing trees, un-watering dirt,
  un-watering pet bowls, watering dirt, and watering pet bowls.
* New game state query for checking the current weather type
  that supports matching multiple types, as well as matching
  the weather up to one week in the past.
* Whether or not a given water condition performs the standard
  logic to water crops and pet bowls is now a separate flag in
  weather data.
* Added a new `cs_history` command for viewing the stored
  historical weather data, which will have at most 8 entries.

### Changed
* The various game state queries for checking weather flags
  now have an optional argument for checking the weather up
  to one week in the past.
* Deprecated the action to water crops. Use the new, better
  action please.
* Reduce the severity of some debugging message from Debug to Trace.

### Fixed
* If the TV patches can't apply cleanly, then don't replace the
  TV with our custom menu. (Assuming we un-patch cleanly.)


## 1.4.0
Released May 4th, 2024.

### Added
* Added a new trigger action for watering crops / pet bowls that
  could come in handy for doing interesting weather stuff.

### Fixed
* Ambient outdoor opacity not being calculated correctly when it is
  time for it to get dark out in-game.
* The in-game TV not displaying its screen in the correct position
  when combined with our Weather channel replacement and using a
  zoom other than 100%.

### API
* The API now supports letting other C# mods add custom layer and
  effect types.


## 1.3.0
Released April 29th, 2024.

### Added
* We now replace the TV Menu! Get weather for more locations. This
  requires the locations to opt into the new system.
* There is now a more flexible system for applying screen tints,
  letting you use game state queries and have multiple data points
  across time that are smoothly blended between.

### Changed
* Deprecated the old screen tints system.
* Internal changes to prepare for letting C# mods add custom
  effect and layer types.

### API
* The API now supports modifying data assets from C# mods using the
  AssetRequested event.


## 1.2.0
Released April 26th, 2024.

### Changed
* The `Buff` effect now has `CustomFields`, though they haven't been
  hooked up to anything yet. (SpaceCore Skill support coming soon.)
* Merge `Snow` and `TextureScroll` into a single layer type.

### Fixed
* Layers did not have the correct sprite sorting applied.
* Typo in the Content Patcher token. `Lighting` instead of `Lightning`.
* Some Harmony patches not working on 1.6.6. They have been updated
  to hopefully be more robust against game changes.
* When invalidating the icon for a `Buff` texture, the buff's
  texture was not being updated correctly.
* The equality comparisons for some layer types were not working as
  expected, causing layers to be recreated unnecessarily.

### API
* We have a proper author's guide now!


## 1.1.0
Released April 25th, 2024.

### Added
* Custom weather totems are now possible!
* Weather effects!
* There are now Game State Queries and a Content Patcher
  token for reading the weather state. This is useful for,
  as an example, determining if it's rainy without checking
  for the "Rain" weather explicitly.

### Changes
* The `cs_list` command now displays all weather conditions, not just
  those provided by Cloudy Skies, as well as what the current and
  future weather are for all location contexts.
* The `cs_tomorrow` command now simulates using a custom weather totem.

### Fixed
* Mistake in `GetMorningSong()` transpiler that could cause an
  argument null exception.

### API Changes
* You can now make an item behave as a custom weather totem by
  setting per-instance modData or an object-wide custom field
  with the key `leclair.cloudyskies/WeatherTotem` and a value
  that is the id of the desired weather.
* Added fields to the weather data model for controlling the
  behavior of custom totems.
* By default, whether or not a weather totem works in a given
  location context is controlled by the `AllowRainTotem` field
  of that location context. You can add a custom field with
  the key `leclair.cloudyskies/AllowTotem:ID` where `ID` is
  the weather Id and the value is `true` or `false` to allow
  or disallow changing that context with any given totem.
* Added the following game state queries for checking the
  state of the weather in a location: `CS_LOCATION_IGNORE_DEBRIS_WEATHER`,
  `CS_WEATHER_IS_RAINING`, `CS_WEATHER_IS_SNOWING`, `CS_WEATHER_IS_LIGHTNING`,
  `CS_WEATHER_IS_DEBRIS`, and `CS_WEATHER_IS_GREEN_RAIN`.
* Added the `leclair.cloudyskies/Weather` Content Patcher
  token for determining the state of the location's weather.
  Possible values: `Raining`, `Snowing`, `Lightning`, `Debris`,
  `GreenRain`, `Sunny`, `Music`, and `NightTiles`.
* Custom weather types can now have effects. These are things that
  happen to the player when they are in a location where the weather
  is applied. Currently there are effects to modify the player's
  health, modify the player's stamina, apply a buff to the player, or
  to run trigger actions.


## 1.0.0

This is the initial release of Cloudy Skies. Please look forward
to it, everyone! I look forward to seeing what the community comes up
with, and what we can create together.
