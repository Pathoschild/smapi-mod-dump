**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/CompSciLauren/stardew-valley-daily-screenshot-mod**

----

# ToDo List

## Documentation

- [x] Functions
- [x] User

## User Input Validation

- [x] Triggers
- [x] Rules
- [x] Config

## Warnings

- [x] Colliding filenames
  - [x] Location
  - [x] Weather
  - [x] Date
  - [x] Directory
  - [x] Game ID
  - [x] Time of day
- [x] Changes to rules
- [x] Rules that won't trigger
- [x] Directory not rooted
  - [x] Check if it is possible to replace slashes for the user
- [x] Config file not loading

## Known Defects

- [x] Wait till screen is rendered (warp)
- [x] Show notice first
- [x] ~~Config file is always written~~ (not fixable)
- [x] Takes full daylight shot at night
- [x] Weekday enums don't work
- [x] Exception for key presses before save game is loaded
- [x] Day rules are not working
- [x] Release builds not building (verbose logging)
- [x] Warnings of file overlap are not showing up (Fixed February 7th)

## Improvements

- [x] Only follow events if a rule needs it
  - [x] Time Change
  - [x] Location Change
  - [x] Key press

## Test Cases

- [x] Triggers
  - [x] Warp
  - [x] Time change
  - [x] Key press
- [x] Warnings
  - [x] Overlapping file names
    - [x] [Location](warning_test_files/location_result.txt)
    - [x] [Weather](warning_test_files/weather_result.txt)
    - [x] [Date](warning_test_files/days_result.txt)
    - [x] Directory
    - [x] Game ID
    - [x] Time of day
    - [x] Different saves
  - [x] Inactive Rules
    - [x] [Location](warning_test_files/location_result.txt)
    - [x] [Weather](warning_test_files/weather_result.txt)
    - [x] [Date](warning_test_files/days_result.txt)
- [x] Others
  - [x] Is the date correct after midnight
  - [x] Bad user input in config
  - [x] Error message for invalid config
