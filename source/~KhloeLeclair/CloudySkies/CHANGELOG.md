**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/KhloeLeclair/StardewMods**

----

# Changelog

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
