**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/CompSciLauren/stardew-valley-daily-screenshot-mod**

----

# Testing

Here are the testing strategies available/recommended for this mod. Please feel free to use your best judgement depending on what changes you've added and how extensive they are.

## warning_test_files

This is a manual set of tests that involves running the game and comparing the console output to some expected result files. You can expect this process to take around 10 minutes.

See the [how_to_test.md](./warning_test_files/how_to_test.md) guide for how to use these tests.

## Unit Tests

It would be nice to have unit tests and these will hopefully be added in the future, but this is not available yet.

## Manual In-Game Testing

This is the primary method of testing.

In general, should make sure that the mod generally works as expected and make sure to test anything specific that was changed.

Below are some general examples of Happy-Path and Unhappy-Path test cases.

### Happy-Path Config Testing

The Happy-Path Config. Add this to the config.json file for testing:
``` json
{
  "AuditoryEffects": true,
  "VisualEffects": true,
  "ScreenshotNotifications": true,
  "SnapshotRules": [
    {
      "Name": "Daily Farm Picture",
      "ZoomLevel": 0.25,
      "Directory": "Default",
      "FileName": "Default",
      "Trigger": {
        "Days": "Daily",
        "Weather": "Any",
        "Location": "Farm",
        "Key": "None",
        "StartTime": 600,
        "EndTime": 2600
      }
    },
    {
      "Name": "Keypress Picture",
      "ZoomLevel": 1.0,
      "Directory": "/home/bob/SDV",
      "FileName": "None",
      "Trigger": {
        "Days": "Daily",
        "Weather": "Any",
        "Location": "Any",
        "Key": "Multiply",
        "StartTime": 600,
        "EndTime": 2600
      }
    }
  ]
}
```

1. Able to launch game and capture a screenshot using Happy-Path config.json file.
    * Game generally looks/behaves as expected.
    * No console errors or warnings.
    * Screenshot is taken and can see the notification with auditory (camera sound) and visual effects (flash).
        * Can be found in the designated folder.
        * Has the expected file name.
        * Was triggered under the correct conditions.
            * Days
            * Weather
            * Location
            * Key (if any)
            * StartTime
            * EndTime
    * Repeat this a few times for a few different configurations for the triggers.
1. Able to change the settings from the UI Config
    * The screenshot behavior updates as expected.
        * AuditoryEffects
        * VisualEffects
        * ScreenshotNotifications
        * SnapshotRules
            * Name
            * ZoomLevel
            * Directory
            * FileName
            * Triggers
                * Days
                * Weather
                * Location
                * Key
                * StartTime
                * EndTime
    * Other existing additional snapshot rules are NOT updated/reset/overridden. Only the global settings and first set of snapshot rules are updated.
1. Able to reset to default settings with the Default button from the UI Config.
    * The screenshot behavior updates as expected.
        * UI Config shows correct default options.
        * Screenshot behavior in-game and in designated folder are correct.
            * AuditoryEffects
            * VisualEffects
            * ScreenshotNotifications
            * SnapshotRules
                * Name
                * ZoomLevel
                * Directory
                * FileName
                * Triggers
                    * Days
                    * Weather
                    * Location
                    * Key
                    * StartTime
                    * EndTime
    * Other existing additional snapshot rules are NOT updated/reset/overridden. Only the global settings and first set of snapshot rules are updated.

### Unhappy-Path Config Testing - 1

The Happy-Path Config. Add this to the config.json file for testing:
``` json
{
  "SnapshotRules": [
    {
      "Name": "Daily Farm Picture",
      "ZoomLevel": 0.25,
      "Directory": "Default",
      "FileName": "Default",
      "Trigger": {
        "Days": "Daily",
        "Weather": "Any",
        "Location": "Farm",
        "Key": "None",
        "StartTime": 600,
        "EndTime": 2600
      }
    },
    {
      "Name": "Keypress Picture",
      "ZoomLevel": 1.0,
      "Directory": "/home/bob/SDV",
      "FileName": "None",
      "Trigger": {
        "Days": "Daily",
        "Weather": "Any",
        "Location": "Any",
        "Key": "Multiply",
        "StartTime": 600,
        "EndTime": 2600
      }
    }
  ]
}
```

Expected success Console output (something similar to this):
``` bash
[SMAPI] Loading mods...
[SMAPI] Loaded X mods:
[SMAPI]    Daily Screenshot 3.0.0 by CompSciLauren | Automatically takes a daily screenshot of your entire farm.
[SMAPI]    Generic Mod Config Menu 1.11.2 by spacechase0 | Adds an in-game UI to edit other mods' config options (for mods which support it).

[SMAPI] Launching mods...
[SMAPI] Mods loaded and ready!
[Daily Screenshot] Added "DailyScreenshot" config menu with "Generic Mod Config Menu".
```

1. Able to launch game and capture a screenshot using Unhappy-Path config.json file.
    * Game generally looks/behaves as expected.
    * No console errors or warnings.
    * For screenshots, it should just use the Default settings automatically for any specific config options that are missing (e.g. AuditoryEffects).
    * Screenshot is taken and can see the notification with auditory (camera sound) and visual effects (flash).
        * Can be found in the designated folder.
        * Has the expected file name.
        * Was triggered under the correct conditions.
            * Days
            * Weather
            * Location
            * Key (if any)
            * StartTime
            * EndTime
1. Able to change the settings from the UI Config
    * The screenshot behavior updates as expected.
        * AuditoryEffects
        * VisualEffects
        * ScreenshotNotifications
        * SnapshotRules
            * Name
            * ZoomLevel
            * Directory
            * FileName
            * Triggers
                * Days
                * Weather
                * Location
                * Key
                * StartTime
                * EndTime
1. Able to reset to default settings with the Default button from the UI Config.
    * The screenshot behavior updates as expected.
        * UI Config shows correct default options.
        * Screenshot behavior in-game and in designated folder are correct.
            * AuditoryEffects
            * VisualEffects
            * ScreenshotNotifications
            * SnapshotRules
                * Name
                * ZoomLevel
                * Directory
                * FileName
                * Triggers
                    * Days
                    * Weather
                    * Location
                    * Key
                    * StartTime
                    * EndTime

### Unhappy-Path Config Testing - 2

The Unhappy-Path Config. Add this to the config.json file for testing:
``` json
{
}
```

1. Warnings show up in the Console as expected.

Should see an output like this:
``` bash
[SMAPI] Launching mods...
[Daily Screenshot] Updating unnamed rule to be "Unnamed Rule 1"
[SMAPI] Mods loaded and ready!
```

### Unhappy-Path Config Testing - 3

The Unhappy-Path Config where Config file does not exist. Make sure there is no Config.json file in the Mod folder.

1. Warnings show up in the Console as expected.

Should see an output like this:
``` bash
[SMAPI] Launching mods...
[Daily Screenshot] Updating unnamed rule to be "Unnamed Rule 1"
[SMAPI] Mods loaded and ready!
```
